namespace Acumatica.WorkspaceManager
{
    public static class Constants
    {
        public const string sqlServerBrowserServiceName = "SQL Server Browser";
        public const string MSSQLServerType = "MSSQLSERVER";
        public const string IISAppPoolUser = "IIS APPPOOL";
        public const string previewVersion = "PREVIEW";
        public const string defaultDBOwner = "dbo";
        public const string connectionString = "data source={0};Initial Catalog={1}; Integrated Security=yes";
        public const string dataReaderSecurity = "db_datareader";
        public const string dataWriterSecurity = "db_datawriter";
        public const string backupSecurity = "db_backupoperator";

        // Characters
        public const char versionEmptyChar = '0';
        public const char versionPromptChar = '_';
        public const char versionSeparatorChar = '.';

        // Image list
        public const string errorImage = "error";
        public const string successImage = "success";
        public const string warningImage = "warning";

        public const string localImage = "local";
        public const string remoteImage = "remote";

        // Grid columns
        public const string instanceStatusColumn = "InstanceStatus";
        public const string databaseColumn = "Database";
        public const string sitePathColumn = "SitePath";
        public const string siteVersionColumn = "SiteVersion";
        public const string dbVersionColumn = "DBVersion";
        public const string packageStatusColumn = "PackageStatus";
        public const string serverColumn = "Server";

        // Registry
        public const string acumaticaRegistryKey = "SOFTWARE\\Acumatica ERP";
        public const string uninstallRegistryKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall";
        public const string pathRegistryValue = "Path";
        public const string virtualDirectoryRegistryValue = "VirtDirName";
        public const string webSiteNameRegistryValue = "WebSiteName";
        public const string typeRegistryValue = "Type";
        public const string productNameRegistryValue = "Acumatica ERP";
        public const string displayNameRegistryValue = "DisplayName";
        public const string displayVersionRegistryValue = "DisplayVersion";
        public const string uninstallRegistryValue = "UninstallString";

        // Directory
        public const string acumaticaDirectory = "AcumaticaERP";
        public const string dataDirectory = "Data";
        public const string filesDirectory = "Files";
        public const string portalDirectory = "Portal";
        public const string databaseDirectory = "Database";
        public const string snapshotsDirectory = "Snapshots";
        public const string localRepositoryName = "AcumaticaBuildManager";
        public const string installDirectory2018 = "Acumatica ERP";

        // Files
        public const string backupFileExtension = ".bak";
        public const string backupFileFilter = "*.bak";
        public const string databaseArchiveFile = "database.zip";
        public const string defaultShellFilename = "Explorer.exe";
        public const string msiFilename = "msiexec.exe";
        public const string wizardFilename = "AcumaticaConfig.exe";
        public const string zipFileExtension = ".zip";
        public const string zipFileFilter = "Zip Archive|*.zip";
        public const string icacls = "icacls";
        public const string defaultSitePath = "c:\\AcumaticaSites";
        public const string webConfigFilename = "web.config";

        // Other
        public const string emptyVersionMask = "__.___.____";
        public const string backupDescription = "Full backup of {0}";
        public const string backupSetName = "{0} Backup";
        public const string snapshotsAppSetting = "SnapshotsFolder";

        // Characters
        public const string allFiles = "*";
        public const char backslash = '\\';
        public const char space = ' ';
        public const string slash = "/";
    }
}
