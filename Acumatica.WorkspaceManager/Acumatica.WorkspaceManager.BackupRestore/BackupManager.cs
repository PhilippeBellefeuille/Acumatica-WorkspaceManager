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
                FileName = Path.ChangeExtension(website.InstanceName, Constants.zipFileExtension),
                Filter = Constants.zipFileFilter,
                Title = Messages.saveBackupTitle
            };

            PXWait.HideWait();

            if (saveFileDialog.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(saveFileDialog.FileName))
            {
                PXWait.ShowWait();

                // Restore z-index after save file dialog has been closed
                PXWait.SetForeground();
                PXWait.ShowProgress(-1, Messages.creatingTempDirectoryProgress);

                if (File.Exists(saveFileDialog.FileName))
                {
                    File.Delete(saveFileDialog.FileName);
                }

                string fileName = Path.GetFileName(saveFileDialog.FileName);
                string tempBaseDirectory = Utility.GetTempDirectory();
                string tempDatabaseDirectory = Path.Combine(tempBaseDirectory, Constants.databaseDirectory);
                string tempWebsiteArchivePath = Path.Combine(tempBaseDirectory, Path.ChangeExtension(website.WebsiteType, Constants.zipFileExtension));
                string tempDatabaseArchivePath = Path.Combine(tempBaseDirectory, Constants.databaseArchiveFile);

                Directory.CreateDirectory(tempBaseDirectory);

                if (isDatabase)
                {
                    PXWait.ShowProgress(-1, Messages.startDatabaseBackupProgress);

                    Directory.CreateDirectory(tempDatabaseDirectory);
                    string dbName = website.Database.Substring(website.Database.IndexOf(Constants.slash) + 1);

                    Server server = new Server(serverConnection);
                    Database database = default(Database);

                    if (server == null || !server.Databases.Contains(dbName))
                    {
                        throw new Exception(Messages.connectionError);
                    }

                    database = server.Databases[dbName];

                    RecoveryModel recoverymod = database.DatabaseOptions.RecoveryModel;

                    Backup backup = new Backup();
                    backup.Action = BackupActionType.Database;
                    backup.BackupSetDescription = string.Format(Constants.backupDescription, dbName);
                    backup.BackupSetName = string.Format(Constants.backupSetName, dbName);
                    // Compression not supported by SQL Express
                    // backup.CompressionOption = BackupCompressionOptions.On;
                    backup.Database = dbName;
                    backup.Incremental = false;

                    string backupFileName = string.Concat(dbName, Constants.backupFileExtension);
                    string backupPath = Path.Combine(server.BackupDirectory, backupFileName);

                    BackupDeviceItem backupFile = new BackupDeviceItem(backupPath, DeviceType.File);
                    backup.Devices.Add(backupFile);
                    backup.LogTruncation = BackupTruncateLogType.Truncate;

                    backup.PercentComplete += delegate (object sender, PercentCompleteEventArgs e)
                    {
                        PXWait.ShowProgress(e.Percent, string.Format(Messages.creatingDatabaseBackupProgress, Convert.ToString(e.Percent, CultureInfo.InvariantCulture)));
                    };

                    backup.SqlBackup(server);

                    PXWait.ShowProgress(-1, Messages.packagingDatabaseBackupProgress);
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
                    PXWait.ShowProgress(-1, Messages.packagingWebsiteProgress);
                    ZipFile.CreateFromDirectory(website.SitePath, tempWebsiteArchivePath, CompressionLevel.NoCompression, false, Encoding.UTF8);
                }

                // Compressing archive
                PXWait.ShowProgress(-1, Messages.compressingArchiveProgress);
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
                Process.Start(Constants.defaultShellFilename, string.Format("/select, \"{0}\"", saveFileDialog.FileName));
            }
        }
        #endregion Public
    }
}
