using Acumatica.WorkspaceManager.Common;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager.BackupRestore
{
    public static class BackupManager
    {
        #region Public 
        public static void BackupInstance(ServerConnection serverConnection, Website website, bool isDatabase, bool isWebsite)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                FileName = Path.ChangeExtension(website.InstanceName, ".zip"),
                Filter = "Zip Archive|*.zip",
                Title = "Save Backup File"
            };

            PXWait.HideWait();

            if (saveFileDialog.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
            {
                PXWait.ShowWait();

                // Restore z-index after save file dialog has been closed
                PXWait.SetForeground();
                PXWait.ShowProgress(-1, "Creating temporary directories...");

                if (File.Exists(saveFileDialog.FileName))
                {
                    File.Delete(saveFileDialog.FileName);
                }

                string fileName = Path.GetFileName(saveFileDialog.FileName);
                string tempBaseDirectory = Utility.GetTempDirectory();
                string tempDatabaseDirectory = Path.Combine(tempBaseDirectory, "Database");
                string tempWebsiteArchivePath = Path.Combine(tempBaseDirectory, Path.ChangeExtension(website.WebsiteType, ".zip"));
                string tempDatabaseArchivePath = Path.Combine(tempBaseDirectory, "database.zip");

                Directory.CreateDirectory(tempBaseDirectory);

                if (isDatabase)
                {
                    PXWait.ShowProgress(-1, "Creating database backup...");

                    Directory.CreateDirectory(tempDatabaseDirectory);
                    string dbName = website.Database.Substring(website.Database.IndexOf('/') + 1);

                    Server server = new Server(serverConnection);
                    Database db = default(Database);
                    db = server.Databases[dbName];

                    RecoveryModel recoverymod = db.DatabaseOptions.RecoveryModel;

                    Backup backup = new Backup();
                    backup.Action = BackupActionType.Database;
                    backup.BackupSetDescription = string.Concat("Full backup of ", dbName);
                    backup.BackupSetName = string.Concat(dbName, " Backup");
                    backup.CompressionOption = BackupCompressionOptions.On;
                    backup.Database = dbName;
                    backup.Incremental = false;

                    string backupFileName = string.Concat(dbName, ".bak");
                    string backupPath = Path.Combine(server.BackupDirectory, backupFileName);

                    BackupDeviceItem backupFile = new BackupDeviceItem(backupPath, DeviceType.File);
                    backup.Devices.Add(backupFile);
                    backup.LogTruncation = BackupTruncateLogType.Truncate;

                    backup.PercentComplete += delegate (object sender, PercentCompleteEventArgs e)
                    {
                        PXWait.ShowProgress(e.Percent, string.Format("Creating database backup: {0}%", Convert.ToString(e.Percent, CultureInfo.InvariantCulture)));
                    };

                    backup.SqlBackup(server);

                    PXWait.ShowProgress(-1, "Packaging database backup...");
                    File.Move(backupPath, Path.Combine(tempDatabaseDirectory, backupFileName));
                    ZipFile.CreateFromDirectory(tempDatabaseDirectory, tempDatabaseArchivePath, CompressionLevel.NoCompression, false, Encoding.UTF8);

                    if (Directory.Exists(tempDatabaseDirectory))
                    {
                        Directory.Delete(tempDatabaseDirectory, true);
                    }
                }

                if (isWebsite)
                {
                    // Packaging website
                    PXWait.ShowProgress(-1, "Packaging website files...");
                    ZipFile.CreateFromDirectory(website.SitePath, tempWebsiteArchivePath, CompressionLevel.NoCompression, false, Encoding.UTF8);
                }

                // Compressing archive
                PXWait.ShowProgress(-1, "Compressing archive...");
                ZipFile.CreateFromDirectory(tempBaseDirectory, saveFileDialog.FileName, CompressionLevel.Optimal, false, Encoding.UTF8);

                // Clean up temp directory
                if (Directory.Exists(tempWebsiteArchivePath))
                {
                    Directory.Delete(tempWebsiteArchivePath, true);
                }

                if (Directory.Exists(tempBaseDirectory))
                {
                    Directory.Delete(tempBaseDirectory, true);
                }

                // Open archive in shell
                Process.Start("Explorer.exe", string.Format("/select, \"{0}\"", saveFileDialog.FileName));
            }
        }
        #endregion Public
    }
}
