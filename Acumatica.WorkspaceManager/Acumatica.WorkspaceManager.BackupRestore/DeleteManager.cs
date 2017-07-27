using Acumatica.WorkspaceManager.Common;
using ConfigCore;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.IO;

namespace Acumatica.WorkspaceManager.BackupRestore
{
    public static class DeleteManager
    {
        public static void DeleteInstance(Website website, bool isDatabase, bool isWebsite)
        {
            if (isWebsite)
            {
                SiteTypes websiteType;

                if (Enum.TryParse(website.WebsiteType, out websiteType))
                {
                    PXWait.ShowProgress(-1, "Deleting website...");
                    SitesManagment.DeleteSite(new VirtSiteInfo(website.InstanceName,
                                                               websiteType,
                                                               website.WebsiteName,
                                                               website.VirtualDirectory,
                                                               website.SitePath,
                                                               true));
                    if (Directory.Exists(website.SitePath))
                    {
                        Directory.Delete(website.SitePath, true);
                    }
                }
            }

            if (isDatabase)
            {
                string dbServer = website.Database.Substring(0, website.Database.IndexOf('/'));
                string dbName = website.Database.Substring(website.Database.IndexOf('/') + 1);

                Server server = new Server(new ServerConnection(dbServer));
                Database db = default(Database);

                if (server.Databases.Contains(dbName))
                {
                    PXWait.ShowProgress(-1, "Deleting database...");
                    db = server.Databases[dbName];
                    server.KillDatabase(dbName);
                }
            }
        }
    }
}
