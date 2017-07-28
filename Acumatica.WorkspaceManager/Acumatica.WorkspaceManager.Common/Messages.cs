using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.WorkspaceManager
{
    public static class Messages
    {
        // Dialog titles
        public const string selectInstancePathTitle = "Select Instance Path";
        public const string loadBackupFileTitle = "Load Backup File";
        public const string saveBackupTitle = "Save Backup File";

        // Progress
        public const string startingSQLBrowserProgress = "Starting SQL Server Browser...";
        public const string discoveringDatabaseProgress = "Discovering database servers...";
        public const string connectingBuildServerProgress = "Connecting to build server...";
        public const string loadingBuildPackageProgress = "Loading build package: {0}";
        public const string scanningInstancesProgress = "Scanning instances: {0}/{1}";
        public const string creatingTempDirectoryProgress = "Creating temporary directories...";
        public const string startDatabaseBackupProgress = "Creating database backup...";
        public const string creatingDatabaseBackupProgress = "Creating database backup: {0}%";
        public const string packagingDatabaseBackupProgress = "Packaging database backup...";
        public const string packagingWebsiteProgress = "Packaging website files...";
        public const string compressingArchiveProgress = "Compressing archive...";
        public const string deletingWebsiteProgress = "Deleting website...";
        public const string deletingDatabaseProgress = "Deleting database...";
        public const string extractingArchiveProgress = "Extracting archive...";
        public const string extractingDatabaseProgress = "Extracting database backup...";
        public const string extractingWebsiteProgress = "Extracting website files...";
        public const string restoringDatabaseProgress = "Restoring database backup: {0}%";
        public const string droppingDatabaseProgress = "Dropping existing database...";
        public const string creatingWebsiteProgress = "Creating new website...";
        public const string restoringWebsiteProgress = "Restoring website files...";
        public const string setAccessRightsProgress = "Setting access rights...";

        // Error
        public const string instanceAlreadyExistsError = "Instance {0} already exists. Delete this instance before initiating restore operation.";
        public const string missingBackupFileError = "Missing backup file.";
        public const string missingDatabaseNameError = "Missing database name.";
        public const string missingInstanceNameError = "Missing instance name.";
        public const string missingVirtualDirectoryError = "Missing virtual directory.";
        public const string missingWebsiteNameError = "Missing website name.";
        public const string missingApplicationPoolError = "Missing application pool.";
        public const string missingInstancePathError = "Missing instance path.";
        public const string versionFetchError = "Can't get version.";
        public const string sitePathError = "Can't find site directory.";
        public const string webConfigError = "Can't read web.config file.";
        public const string serverTypeError = "Can't backup/restore {0} databases.";
        public const string versionMismatchError = "Database version doesn't coincide with the site's version.";
        public const string missingDatabaseError = "Database backup is missing from the archive.";
        public const string databaseExtractingError = "Can't extract database backup from the archive.";
        public const string missingWebsiteError = "Website files not found in backup archive.";
        public const string missingInstallDirectoryError = "Install directory not found: {0}{1}";
        public const string fileTransferError = "Failed to download remote file to local directory.";
        public const string installError = "Failed to install Acumatica ERP.";
        public const string removeFileError = "Failed to remove local file.";
        public const string downloadError = "Cannot download package that is not available online.";
        public const string deletePackageError = "Only local packages can be deleted.";
        public const string connectionError = "Database connection error.";
    }
}
