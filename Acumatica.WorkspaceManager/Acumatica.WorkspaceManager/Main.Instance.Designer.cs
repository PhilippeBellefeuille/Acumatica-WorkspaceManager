using Acumatica.WorkspaceManager.BackupRestore;
using Acumatica.WorkspaceManager.Common;
using ConfigCore;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using PX.BulkInsert.Installer;
using PX.WebConfig;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main
    {
        #region Variables
        private bool isInstanceSortOrderDescending;
        #endregion

        #region Events
        private void InstanceDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = InstanceDataGridView.Rows[e.RowIndex] as DataGridViewRow;

            if (row != null)
            {
                Website website = row.DataBoundItem as Website;

                if (website != null)
                {
                    Color errorColor = Color.LightSalmon;
                    Color successColor = Color.White;
                    Color warningColor = Color.LightYellow;

                    Color errorTextColor = Color.DarkRed;
                    Color successTextColor = Color.Black;

                    if (e.ColumnIndex == InstanceDataGridView.Columns[Constants.instanceStatusColumn].Index)
                    {
                        FormatInstanceCellStatus(e, row, website, errorTextColor, warningColor, successTextColor);
                    }
                    else if (e.ColumnIndex == InstanceDataGridView.Columns[Constants.databaseColumn].Index)
                    {
                        FormatInstanceCellDatabase(e, row, website, errorColor, warningColor, successColor);
                    }
                    else if (e.ColumnIndex == InstanceDataGridView.Columns[Constants.sitePathColumn].Index)
                    {
                        FormatInstanceCellSitePath(e, row, website, errorColor, warningColor, successColor);
                    }
                    else if (e.ColumnIndex == InstanceDataGridView.Columns[Constants.siteVersionColumn].Index)
                    {
                        FormatInstanceCellSiteVersion(e, row, website, errorColor, warningColor, successColor);
                    }
                    else if (e.ColumnIndex == InstanceDataGridView.Columns[Constants.dbVersionColumn].Index)
                    {
                        FormatInstanceCellDBVersion(e, row, website, errorColor, warningColor, successColor);
                    }
                }
            }
        }

        private void InstanceDataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            isInstanceSortOrderDescending = !isInstanceSortOrderDescending;

            InstanceBindingSource.CurrencyManager.SuspendBinding();

            List<Website> websites = ((BindingList<Website>)((BindingSource)InstanceDataGridView.DataSource).List).ToList();
            InstanceBindingSource.Clear();

            foreach (Website website in isInstanceSortOrderDescending ? websites.OrderByDescending(x => x.InstanceName) :
                                                                        websites.OrderBy(x => x.InstanceName))
            {
                InstanceBindingSource.Add(website);
            }

            InstanceBindingSource.CurrencyManager.ResumeBinding();
            FilterInstances(null);
        }
        #endregion

        #region Format Cells

        private void FormatInstanceCellDBVersion(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, Website website, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = Messages.versionFetchError;
                row.Cells[Constants.dbVersionColumn].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            else if (website.SiteVersion != website.DBVersion)
            {
                e.CellStyle.BackColor = warningColor;
                row.Cells[Constants.dbVersionColumn].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
                row.Cells[Constants.dbVersionColumn].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void FormatInstanceCellSiteVersion(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, Website website, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = Messages.versionFetchError;
                row.Cells[Constants.siteVersionColumn].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            else if (website.SiteVersion != website.DBVersion)
            {
                e.CellStyle.BackColor = warningColor;
                row.Cells[Constants.siteVersionColumn].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
                row.Cells[Constants.siteVersionColumn].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void FormatInstanceCellSitePath(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, Website website, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = Messages.sitePathError;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
                row.Cells[Constants.sitePathColumn].ToolTipText = string.Empty;
            }
        }

        private void FormatInstanceCellDatabase(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, Website website, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = Messages.webConfigError;
            }
            else if (website.ServerType != null && website.ServerType.ToUpperInvariant().Trim() != Constants.MSSQLServerType)
            {
                e.CellStyle.BackColor = errorColor;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
            }
        }

        private void FormatInstanceCellStatus(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, Website website, Color errorTextColor, Color warningColor, Color successTextColor)
        {
            bool isSameVersion = (website.SiteVersion != null && website.SiteVersion == website.DBVersion);
            bool isDatabase = (website.Database != null);
            bool isWebConfig = (website.Database != null);
            bool isSiteVersion = (website.SiteVersion != null);
            bool isDBVersion = (website.DBVersion != null);
            bool isMSSql = (website.ServerType != null && website.ServerType.ToUpperInvariant().Trim() == Constants.MSSQLServerType);

            if (!isWebConfig || !isSiteVersion || !isDBVersion || !isDatabase || !isMSSql)
            {
                e.Value = StatusImageList.Images[Constants.errorImage];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = errorTextColor;

                    if (!isWebConfig || !isDatabase)
                        cell.ToolTipText = Messages.webConfigError;
                    else if (!isMSSql)
                        cell.ToolTipText = string.Format(Messages.serverTypeError, website.ServerType);
                    else if (!isSiteVersion || !isDBVersion)
                        cell.ToolTipText = Messages.versionFetchError;
                }
            }
            else if (!isSameVersion)
            {
                e.Value = StatusImageList.Images[Constants.warningImage];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = successTextColor;
                    cell.ToolTipText = Messages.versionMismatchError;
                }
            }
            else
            {
                e.Value = StatusImageList.Images[Constants.successImage];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = successTextColor;
                    cell.ToolTipText = string.Empty;
                }
            }
        }

        #endregion

        #region Methods
        private void EnableInstanceControls(Website website)
        {
            bool isInstance = (website != null);
            bool isDatabase = (isInstance && !string.IsNullOrWhiteSpace(website.Database));
            bool isUrl = (isInstance && !string.IsNullOrWhiteSpace(website.Url));
            bool isWebsite = (isInstance && !string.IsNullOrWhiteSpace(website.SitePath));

            OpenWebsiteButton.Enabled = isUrl;

            InstanceBackupButton.Enabled = (isDatabase || isWebsite);
            BackupEverythingMenuItem.Enabled = (isDatabase && isWebsite);
            BackupDatabaseMenuItem.Enabled = isDatabase;
            BackupWebsiteMenuItem.Enabled = isWebsite;

            InstanceDeleteButton.Enabled = (isDatabase || isWebsite);
            DeleteEverythingMenuItem.Enabled = (isDatabase && isWebsite);
            DeleteDatabaseMenuItem.Enabled = isDatabase;
            DeleteWebsiteMenuItem.Enabled = isWebsite;
        }

        private void ExecuteInstanceAction(object sender)
        {
            Website website = GetSelectedInstance();

            if (sender == OpenWebsiteButton)
            {
                Process.Start(website.Url);
            }
            else
            {
                bool isBackupAll = (sender == BackupEverythingMenuItem);
                bool isBackupDatabase = (isBackupAll || sender == BackupDatabaseMenuItem);
                bool isBackupWebsite = (isBackupAll || sender == BackupWebsiteMenuItem);

                bool isRestoreAll = (sender == RestoreEverythingMenuItem);
                bool isRestoreDatabase = (isRestoreAll || sender == RestoreDatabaseMenuItem);
                bool isRestoreWebsite = (isRestoreAll || sender == RestoreWebsiteMenuItem);

                bool isDeleteAll = (sender == DeleteEverythingMenuItem);
                bool isDeleteDatabase = (isDeleteAll || sender == DeleteDatabaseMenuItem);
                bool isDeleteWebsite = (isDeleteAll || sender == DeleteWebsiteMenuItem);

                if (website != null)
                {
                    if (isBackupAll || isBackupDatabase || isBackupWebsite)
                    {
                        BackupManager.BackupInstance(BuildServerConnection(ServerNameComboBox.Text), website, isBackupDatabase, isBackupWebsite);
                    }
                    else if (isRestoreAll || isRestoreDatabase || isRestoreWebsite)
                    {
                        InstancePanel.Visible = false;

                        if (isRestoreWebsite)
                        {
                            LoadWebsitesList();
                        }

                        if (isRestoreDatabase)
                        {
                            LoadAppPoolsList();
                            LoadDatabaseList();
                        }

                        WebsiteSettingsPanel.Visible = isRestoreWebsite;
                        SitePathLabel.Visible = isRestoreWebsite;
                        SitePathTextBox.Visible = isRestoreWebsite;
                        PickSitePathButton.Visible = isRestoreWebsite;
                        InstanceNameLabel.Visible = isRestoreWebsite;
                        InstanceNameTextBox.Visible = isRestoreWebsite;
                        DatabaseSettingsPanel.Visible = isRestoreDatabase;

                        RestorePanel.Visible = true;
                        ActiveControl = BrowseBackupFileButton;
                    }
                    else if (isDeleteAll || isDeleteDatabase || isDeleteWebsite)
                    {
                        DeleteManager.DeleteInstance(website, isDeleteDatabase, isDeleteWebsite);
                        ReloadInstance(null);
                    }
                }
            }
        }

        private void FilterInstances(string selectedInstance)
        {
            try
            {
                instanceNameFilter = InstanceNameFilterTextBox.Text;
                InstanceNameFilteredLabel.Visible = !string.IsNullOrWhiteSpace(instanceNameFilter);

                // Save selection
                Website selectedWebsite = GetSelectedInstance();

                if (selectedInstance != null)
                {
                    selectedRow = selectedInstance;
                }
                else if (selectedWebsite != null)
                {
                    selectedRow = selectedWebsite.InstanceName;
                }

                // Freeze binding
                InstanceBindingSource.CurrencyManager.SuspendBinding();

                // Filter rows
                for (int i = 0; i < InstanceDataGridView.Rows.Count; i++)
                {
                    DataGridViewRow row = InstanceDataGridView.Rows[i];

                    if (row != null)
                    {
                        Website website = row.DataBoundItem as Website;

                        row.Visible = (website != null &&
                                       (string.IsNullOrWhiteSpace(instanceNameFilter) ||
                                        website.InstanceName.ToUpperInvariant().Contains(instanceNameFilter.ToUpperInvariant())));
                    }
                }

                // Resume binding
                InstanceBindingSource.CurrencyManager.ResumeBinding();

                // Restore selection
                InstanceDataGridView.ClearSelection();

                if (selectedRow != null)
                {
                    for (int i = 0; i < InstanceDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = InstanceDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            Website website = row.DataBoundItem as Website;

                            if (website.InstanceName == selectedRow)
                            {
                                InstanceDataGridView.Rows[i].Selected = true;
                                InstanceDataGridView.FirstDisplayedScrollingRowIndex = i;
                                EnableInstanceControls(website);
                                break;
                            }
                        }
                    }
                }

                // Select first displayed row
                if (InstanceDataGridView.SelectedRows.Count == 0 &&
                    InstanceDataGridView.DisplayedRowCount(true) > 0)
                {
                    for (int i = 0; i < InstanceDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = InstanceDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            Website website = row.DataBoundItem as Website;
                            InstanceDataGridView.Rows[i].Selected = true;
                            InstanceDataGridView.FirstDisplayedScrollingRowIndex = i;
                            selectedRow = website.InstanceName;
                            EnableInstanceControls(website);
                            break;
                        }
                    }
                }

                // No selection
                if (InstanceDataGridView.SelectedRows.Count == 0)
                {
                    EnableInstanceControls(null);
                }
            }
            catch (Exception ex)
            {
                PXWait.StopWait();
                SysData.ShowException(ex.ToString(), ErrorLevel.Error);
            }
        }

        private List<Website> GetInstances()
        {
            List<Website> websites = new List<Website>();

            foreach (RegistryView registryView in new RegistryView[] { RegistryView.Registry32, RegistryView.Registry64 })
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, registryView);

                if (baseKey != null)
                {
                    RegistryKey key = baseKey.OpenSubKey(Constants.acumaticaRegistryKey, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                    if (key != null)
                    {
                        string[] subKeys = key.GetSubKeyNames();

                        int counter = 0;
                        int total = subKeys.Length;
                        int totalDigitCount = subKeys.Length > 0 ? (int)Math.Log10(subKeys.Length) + 1 : 0;

                        foreach (string subKeyName in subKeys)
                        {
                            RegistryKey subKey = key.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                            if (subKey != null)
                            {
                                try
                                {
                                    string path = subKey.GetValue(Constants.pathRegistryValue) as string;
                                    string configFile = Path.Combine(path, Constants.webConfigFilename);
                                    string virtualDirectory = subKey.GetValue(Constants.virtualDirectoryRegistryValue) as string;
                                    string websiteName = subKey.GetValue(Constants.webSiteNameRegistryValue) as string;
                                    string websiteType = subKey.GetValue(Constants.typeRegistryValue) as string;

                                    ServerManager server = new ServerManager();
                                    SiteCollection sites = server.Sites;
                                    Site site = sites[websiteName];
                                    String applicationName = string.Concat(Constants.slash, virtualDirectory);

                                    Microsoft.Web.Administration.Application application = site.Applications[applicationName];

                                    foreach (Microsoft.Web.Administration.Binding binding in site.Bindings)
                                    {
                                        if (String.Compare(binding.Protocol, Titles.Http, true) == 0)
                                        {
                                            string port = binding.EndPoint.Port.ToString();
                                            string hostName = binding.Host;
                                            string ip = binding.EndPoint.Address.ToString();

                                            if (String.Compare(ip, Titles.NullIP, true) == 0)
                                                ip = "";

                                            StringBuilder url = new StringBuilder("http://");

                                            if (String.IsNullOrEmpty(hostName))
                                                url.Append("localhost");
                                            else
                                                url.Append(hostName);

                                            if (!String.IsNullOrEmpty(port))
                                            {
                                                url.Append(":");
                                                url.Append(port);
                                            }

                                            url.Append("/");
                                            url.Append(virtualDirectory);
                                            url.Append("/Main.aspx");

                                            if (!File.Exists(configFile))
                                            {
                                                websites.Add(new Website(subKeyName,
                                                                         virtualDirectory,
                                                                         null,
                                                                         null,
                                                                         null,
                                                                         path,
                                                                         url.ToString(),
                                                                         null,
                                                                         websiteType,
                                                                         websiteName));
                                                continue;
                                            }

                                            WebConfiguration webConfiguration = new WebConfiguration(configFile, true);
                                            DbmsProviderService ServerType = ProviderLocator.GetProviderByPxDataProviderClassName(webConfiguration.DatabaseProvider);

                                            ConnectionDefinition connection = DatabaseProvider.ParseConnectionString(DatabaseProvider.GetProvider(ServerType.name), webConfiguration.ConnectionString);
                                            DBInfo databaseInfo = DBInfo.DatabaseTest(connection);

                                            websites.Add(new Website(subKeyName,
                                                                     virtualDirectory,
                                                                     string.Concat(connection.Server, Constants.slash, connection.Database),
                                                                     webConfiguration.Version,
                                                                     databaseInfo.GetVersion(),
                                                                     path,
                                                                     url.ToString(),
                                                                     ServerType.name.ToUpperInvariant(),
                                                                     websiteType,
                                                                     websiteName));

                                            PXWait.ShowProgress((int)(((float)counter / total) * 100f),
                                                                string.Format(Messages.scanningInstancesProgress,
                                                                              Convert.ToString(++counter, CultureInfo.InvariantCulture).PadLeft(totalDigitCount),
                                                                              Convert.ToString(total, CultureInfo.InvariantCulture)));
                                        }
                                    }
                                }
                                catch
                                {
                                    // Website not found
                                    ++counter;
                                }
                            }
                        }
                    }
                }
            }

            return websites;
        }

        private Website GetSelectedInstance()
        {
            return InstanceDataGridView.SelectedRows.Count > 0 &&
                   InstanceDataGridView.SelectedRows[0].Visible ? InstanceDataGridView.SelectedRows[0].DataBoundItem as Website : null;
        }

        private void ReloadInstance(string selectedInstance)
        {
            new Thread(new ThreadStart(delegate
            {
                Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        PXWait.StartWait(this);

                        List<Website> websites = GetInstances();

                        InstanceBindingSource.CurrencyManager.SuspendBinding();
                        InstanceBindingSource.Clear();

                        foreach (Website website in isInstanceSortOrderDescending ? websites.OrderByDescending(x => x.InstanceName) :
                                                                                    websites.OrderBy(x => x.InstanceName))
                        {
                            InstanceBindingSource.Add(website);
                        }

                        InstanceBindingSource.CurrencyManager.ResumeBinding();

                        FilterInstances(selectedInstance);

                        PXWait.StopWait();
                    }
                    catch (Exception ex)
                    {
                        PXWait.StopWait();
                        SysData.ShowException(ex.ToString(), ErrorLevel.Error);
                    }
                });
            })).Start();
        }
        #endregion
    }
}
