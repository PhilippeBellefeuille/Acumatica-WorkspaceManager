namespace Acumatica.WorkspaceManager.Builds
{
    public class WebSite
    {
        public WebSite(string instanceName, string database, string siteVersion, string dbVersion, string sitePath, string url, string serverType)
        {
            this.InstanceName = instanceName;
            this.Database = database;
            this.SiteVersion = siteVersion;
            this.DBVersion = dbVersion;
            this.SitePath = sitePath;
            this.Url = url;
            this.ServerType = serverType;
        }

        public string InstanceName { get; }
        public string Database { get; }
        public string SiteVersion { get; private set; }
        public string DBVersion { get; private set; }
        public string SitePath { get; private set; }
        public string Url { get; private set; }
        public string ServerType { get; private set; }
    }
}
