using Microsoft.SqlServer.Management.Common;

namespace Acumatica.WorkspaceManager.Common
{
    public class Website
    {
        public Website(string instanceName, string virtualDirectory, string database, string siteVersion, string dbVersion, string sitePath, string url, string serverType, string websiteType, string webSiteName)
        {
            this.InstanceName = instanceName;
            this.VirtualDirectory = virtualDirectory;
            this.Database = database;
            this.SiteVersion = siteVersion;
            this.DBVersion = dbVersion;
            this.SitePath = sitePath;
            this.Url = url;
            this.ServerType = serverType;
            this.WebsiteType = websiteType;
            this.WebsiteName = webSiteName;
        }

        public string InstanceName { get; }
        public string VirtualDirectory { get; }
        public string Database { get; }
        public string SiteVersion { get; }
        public string DBVersion { get; }
        public string SitePath { get; }
        public string Url { get; }
        public string ServerType { get; }
        public string WebsiteType { get; set; }
        public string WebsiteName { get; } 
    }
}
