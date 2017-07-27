using Acumatica.WorkspaceManager.Common;
using ConfigCore;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Acumatica.WorkspaceManager.BackupRestore
{
    public static class RestoreManager
    {
        #region Public
        public static void RestoreInstance(ServerConnection serverConnection, Website website, string backupFilePath, string appPool, bool isDatabase, bool isWebsite)
        {
            PXWait.ShowProgress(-1, "Extracting archive...");
            string extractArchiveDirectory = Utility.GetTempDirectory();
            string extractDatabasePath = Path.Combine(extractArchiveDirectory, "database.zip");

            string extractDatabaseDirectory = Utility.GetTempDirectory();
            string extractWebsiteDirectory = Utility.GetTempDirectory();

            Directory.CreateDirectory(extractDatabaseDirectory);
            Directory.CreateDirectory(extractWebsiteDirectory);

            ZipFile.ExtractToDirectory(backupFilePath, extractArchiveDirectory);

            if (isDatabase)
            {
                if (!File.Exists(extractDatabasePath))
                {
                    throw new Exception("Database backup is missing from the archive.");
                }

                string backupFile = ExtractingDatabaseBackup(extractDatabasePath, extractDatabaseDirectory);

                if (!string.IsNullOrWhiteSpace(backupFile))
                {
                    string dbName = website.Database.Substring(website.Database.IndexOf('/') + 1);
                    Server server = new Server(serverConnection);
                    RestoreDatabase(server, dbName, website, backupFile);
                    SetDBSecurity(server, dbName, appPool);
                }
                else
                {
                    throw new Exception("Can't extract database backup from the archive.");
                }
            }

            if (isWebsite)
            {
                RestoreWebsite(website,
                               appPool,
                               extractArchiveDirectory,
                               extractDatabaseDirectory,
                               extractWebsiteDirectory);
            }
        }

        private static string ExtractingDatabaseBackup(string extractDatabasePath, string extractDatabaseDirectory)
        {
            PXWait.ShowProgress(-1, "Extracting database backup...");
            ZipFile.ExtractToDirectory(extractDatabasePath, extractDatabaseDirectory);

            string[] backupFiles = Directory.GetFiles(extractDatabaseDirectory, "*.bak", SearchOption.TopDirectoryOnly);

            return backupFiles.Length > 0 ? backupFiles[0] : null;
        }

        private static void SetDBSecurity(Server server, string dbName, string appPool)
        {
            Login login = null;
            string IISLogin = Path.Combine("IIS APPPOOL", appPool);

            foreach (Login serverLogin in server.Logins)
            {
                if (serverLogin.Name.Equals(IISLogin, StringComparison.OrdinalIgnoreCase))
                {
                    login = serverLogin;
                    break;
                }
            }

            if (login == null)
            {
                login = new Login(server, IISLogin);
                login.LoginType = LoginType.WindowsUser;
                login.Create();
            }

            Database database = server.Databases[dbName];
            User user = GetDBUser(appPool, database);
            Schema schemaOwner = null;

            if (user != null)
            {
                foreach (Schema schema in database.Schemas)
                {
                    if (schema.Name == appPool && schema.Owner == appPool)
                    {
                        schema.Owner = "dbo";
                        schemaOwner = schema;
                        schema.Alter();
                        break;
                    }
                }

                user.Drop();
            }

            CreateDBUser(appPool, server, dbName, login);
            SetDBRoleSecurity(appPool, database);

            if (schemaOwner != null)
            {
                schemaOwner.Owner = appPool;
            }
        }

        private static void RestoreWebsite(Website website, string appPool, string extractArchiveDirectory, string extractDatabaseDirectory, string extractWebsiteDirectory)
        {
            SiteTypes websiteType;
            PXWait.ShowProgress(-1, "Extracting website files...");
            string extractWebsitePath = Path.Combine(extractArchiveDirectory, Path.ChangeExtension(Enum.GetName(typeof(SiteTypes), SiteTypes.RegularSite), ".zip"));

            if (!File.Exists(extractWebsitePath))
            {
                extractWebsitePath = Path.Combine(extractArchiveDirectory, Path.ChangeExtension(Enum.GetName(typeof(SiteTypes), SiteTypes.CompanyPortal), ".zip"));
                website.WebsiteType = Enum.GetName(typeof(SiteTypes), SiteTypes.CompanyPortal);

                if (!File.Exists(extractWebsitePath))
                {
                    throw new Exception("Website files not found in backup archive.");
                }
            }
            else
            {
                website.WebsiteType = Enum.GetName(typeof(SiteTypes), SiteTypes.RegularSite);
            }

            ZipFile.ExtractToDirectory(extractWebsitePath, extractWebsiteDirectory);

            if (Enum.TryParse(website.WebsiteType, out websiteType))
            {
                CreateWebsite(website, appPool, websiteType);
                MoveWebsiteFiles(website, extractWebsiteDirectory);
                SetAccessRights(website);
            }

            DeleteTempDirectory(extractArchiveDirectory, extractDatabaseDirectory, extractWebsiteDirectory);
        }

        private static void RestoreDatabase(Server server, string dbName, Website website, string backupFile)
        {
            Restore restore = new Restore();
                        
            string backupFileLocation = Path.Combine(server.BackupDirectory, Path.GetFileName(backupFile));

            if (File.Exists(backupFileLocation))
            {
                File.Delete(backupFileLocation);
            }

            DropExistingDatabase(server, dbName);

            File.Move(backupFile, backupFileLocation);
            SetBackupFileAccessRights(backupFileLocation);
            restore.Devices.AddDevice(backupFileLocation, DeviceType.File);
            restore.Database = dbName;
            restore.NoRecovery = false;

            restore.PercentComplete += delegate (object sender, PercentCompleteEventArgs e)
            {
                PXWait.ShowProgress(e.Percent, string.Format("Restoring database backup: {0}%", Convert.ToString(e.Percent, CultureInfo.InvariantCulture)));
            };

            restore.SqlRestore(server);
        }

        private static void DropExistingDatabase(Server server, string dbName)
        {
            if (server.Databases.Contains(dbName))
            {
                PXWait.ShowProgress(-1, "Dropping existing database...");
                server.KillDatabase(dbName);
            }
        }

        private static void CreateWebsite(Website website, string appPool, SiteTypes websiteType)
        {
            PXWait.ShowProgress(-1, "Creating new website...");
            CreateWebsiteDirectory(website);
            VirtSiteInfo virtSiteInfo = new VirtSiteInfo(website.InstanceName, websiteType, website.WebsiteName, website.VirtualDirectory, website.SitePath, false);
            virtSiteInfo.IISInfo.AppPool = new IISPoolInfo(appPool);
            SitesManagment.CreateSite(virtSiteInfo);
        }

        private static void CreateWebsiteDirectory(Website website)
        {
            string exeLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            foreach (string directory in new string[]
                                         {
                                             Path.Combine(exeLocation, "Files"),
                                             Path.Combine(exeLocation, "Portal"),
                                             website.SitePath
                                         })
            {
                if (Directory.Exists(directory))
                {
                    Directory.Delete(directory, true);
                }

                Directory.CreateDirectory(directory);
            }
        }

        private static void MoveWebsiteFiles(Website website, string extractWebsiteDirectory)
        {
            PXWait.ShowProgress(-1, "Restoring website files...");
            MoveDirectory(extractWebsiteDirectory, website.SitePath);
        }

        private static void SetAccessRights(Website website)
        {
            PXWait.ShowProgress(-1, "Setting access rights...");

            Process.Start(new ProcessStartInfo()
            {
                Arguments = string.Format("\"{0}\" /reset /T /C", website.SitePath),
                FileName = "icacls",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            }).WaitForExit();
        }

        private static void DeleteTempDirectory(string extractArchiveDirectory, string extractDatabaseDirectory, string extractWebsiteDirectory)
        {
            DeleteDirectory(extractArchiveDirectory);
            DeleteDirectory(extractDatabaseDirectory);
            DeleteDirectory(extractWebsiteDirectory);
        }

        private static void DeleteDirectory(string folder)
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, true);
            }
        }
        #endregion Public

            #region Private
        private static User CreateDBUser(string appPool, Server server, string dbName, Login login)
        {
            User user = new User(server.Databases[dbName], appPool);
            user.Login = login.Name;
            user.Create();
            return user;
        }

        private static User GetDBUser(string appPool, Database database)
        {
            User user = null;

            foreach (User databaseUser in database.Users)
            {
                if (databaseUser.Name.Equals(appPool, StringComparison.OrdinalIgnoreCase))
                {
                    user = databaseUser;
                    break;
                }
            }

            return user;
        }

        public static void MoveDirectory(string source, string target)
        {
            const string allFiles = "*";
            const char backslash = '\\';
            const char space = ' ';

            // Remove trailing backslash
            string sourcePath = source.TrimEnd(backslash, space);
            string targetPath = target.TrimEnd(backslash, space);

            // Copy files to target directory
            foreach (IGrouping<string, string> folder in Directory.EnumerateFiles(sourcePath, allFiles, SearchOption.AllDirectories).GroupBy(s => Path.GetDirectoryName(s)))
            {
                string targetFolder = folder.Key.Replace(sourcePath, targetPath);

                if (!Directory.Exists(targetFolder))
                {
                    Directory.CreateDirectory(targetFolder);
                }

                foreach (string file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));

                    bool isDirectory = (File.GetAttributes(file) & FileAttributes.Directory) == FileAttributes.Directory;

                    if (!isDirectory)
                    {
                        if (File.Exists(targetFile))
                        {
                            File.Delete(targetFile);
                        }

                        File.Move(file, targetFile);
                    }
                }
            }

            // Delete source directory
            if (Directory.Exists(source))
            {
                Directory.Delete(source, true);
            }
        }

        private static void SetBackupFileAccessRights(string fullPath)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(fullPath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                                                                     FileSystemRights.FullControl, 
                                                                     InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit,
                                                                     PropagationFlags.NoPropagateInherit, 
                                                                     AccessControlType.Allow));
            directoryInfo.SetAccessControl(directorySecurity);
        }

        private static void SetDBRoleSecurity(string appPool, Database database)
        {
            foreach (string dbRole in new string[]
                                      {
                                                      "db_datareader",
                                                      "db_datawriter",
                                                      "db_backupoperator"
                                      })
            {
                if (!database.Roles[dbRole].EnumMembers().Contains(appPool))
                {
                    database.Roles[dbRole].AddMember(appPool);
                }
            }
        }
        #endregion Private
    }
}
