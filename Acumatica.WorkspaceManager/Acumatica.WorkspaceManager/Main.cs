using Acumatica.WorkspaceManager.Builds;
using Acumatica.WorkspaceManager.Install;
using ConfigCore;
using Microsoft.Web.Administration;
using Microsoft.Win32;
using PX.BulkInsert.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using PX.WebConfig;

namespace Acumatica.WorkspaceManager
{
    public partial class Main : Form
    {
        private string selectedRowKey;
        private string versionMask;
        private string instanceNameFilter;

        public Main()
        {
            InitializeComponent();
        }

        #region Event Handlers
        private void BackButton_Click(object sender, EventArgs e)
        {
            BackupRestorePanel.Visible = false;
            BuildPackagePanel.Visible = false;
            MenuPanel.Visible = true;
        }

        private void BuildPackageBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            BuildPackage buildPackage = BuildPackageBindingSource.Current as BuildPackage;

            if (buildPackage != null)
            {
                EnableControlsBuildPackage(buildPackage);
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            BuildPackage buildPackage = GetSelectedBuildPackage();

            if (buildPackage != null)
            {
                try
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            PXWait.StartWait(this);
                            ExecuteAction(sender, buildPackage);
                            FilterBuildPackages();
                            PXWait.StopWait();
                        });
                    })).Start();
                }
                catch (Exception ex)
                {
                    if (PXWait.IsStarted || PXWait.IsShown)
                    {
                        PXWait.StopWait();
                    }

                    SysData.ShowException(ex.Message, ErrorLevel.Error);
                }
            }
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                LinkLabel linkLabel = sender as LinkLabel;

                if (linkLabel != null)
                {
                    Process.Start(linkLabel.Text);
                }
            }
            catch (Exception ex)
            {
                SysData.ShowException(ex.Message, ErrorLevel.Error);
            }
        }
                
        private void MenuItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MenuPanel.Visible = false;

            if (sender == BuildPackageMenuLinkLabel)
            {
                BuildPackagePanel.Visible = true;

                if (versionMask == null)
                {
                    ReloadBuildPackage();
                }
            }   
            else if (sender == BackupRestoreMenuLinkLabel)
            {
                BackupRestorePanel.Visible = true;
                ReloadWebSites();
            }         
        }

        private void ReloadDataEventHandler(object sender, EventArgs e)
        {
            if (sender == BuildPackageReloadButton)
            {
                ReloadBuildPackage();
            }
            else if ((sender == VersionMaskedTextBox && versionMask != VersionMaskedTextBox.Text) ||
                     sender == ShowRemoteCheckBox ||
                     sender == ShowLocalCheckBox ||
                     sender == ShowInstalledCheckBox)
            {
                FilterBuildPackages();
            }
        }

        private void VersionMaskedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && versionMask != VersionMaskedTextBox.Text)
            {
                FilterBuildPackages();
            }
        }

        private void WebSitesDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = WebSitesDataGridView.Rows[e.RowIndex] as DataGridViewRow;

            if (row != null)
            {
                WebSite webSite = row.DataBoundItem as WebSite;

                if (webSite != null)
                {
                    Color errorColor = Color.LightSalmon;
                    Color successColor = Color.White;
                    Color warningColor = Color.LightYellow;

                    Color errorTextColor = Color.DarkRed;
                    Color successTextColor = Color.Black;

                    if (e.ColumnIndex == WebSitesDataGridView.Columns["WebSiteStatus"].Index)
                    {
                        FormatWebSiteCellStatus(e, row, webSite, errorTextColor, warningColor, successTextColor);
                    }
                    else if (e.ColumnIndex == WebSitesDataGridView.Columns["Database"].Index)
                    {
                        FormatWebSiteCellDatabase(e, row, webSite, errorColor, warningColor, successColor);
                    }
                    else if (e.ColumnIndex == WebSitesDataGridView.Columns["SitePath"].Index)
                    {
                        FormatWebSiteCellSitePath(e, row, webSite, errorColor, warningColor, successColor);
                    }
                    else if (e.ColumnIndex == WebSitesDataGridView.Columns["SiteVersion"].Index)
                    {
                        FormatWebSiteCellSiteVersion(e, row, webSite, errorColor, warningColor, successColor);
                    }
                    else if (e.ColumnIndex == WebSitesDataGridView.Columns["DBVersion"].Index)
                    {
                        FormatWebSiteCellDBVersion(e, row, webSite, errorColor, warningColor, successColor);
                    }
                }
            }
        }

        private void WebSitesNameFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            FilterWebSites();
        }

        private void BuildPackageDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = BuildPackageDataGridView.Rows[e.RowIndex] as DataGridViewRow;

            if (row != null)
            {
                BuildPackage buildPackage = row.DataBoundItem as BuildPackage;

                if (buildPackage != null)
                {
                    if (e.ColumnIndex == BuildPackageDataGridView.Columns["BuildPackageStatus"].Index)
                    {
                        if (buildPackage.IsInstalled)
                        {
                            e.Value = StatusImageList.Images["success"];
                        }
                        else if (buildPackage.IsLocal)
                        {
                            e.Value = StatusImageList.Images["local"];
                        }
                        else if (buildPackage.IsRemote)
                        {
                            e.Value = StatusImageList.Images["remote"];
                        }
                        else
                        {
                            e.Value = new Bitmap(16, 16);
                        }
                    }
                }
            }
        }
        #endregion

        #region Actions
        private void ExecuteAction(object sender, BuildPackage buildPackage)
        {
            if (sender == DownloadButton)
            {
                DownloadBuildPackage(buildPackage, delegate (int percentDone, long counter, long total)
                {
                    const long oneMegabyte = 1048576;
                    long transferedMegaBytes = counter / oneMegabyte;
                    long totalMegaBytes = total / oneMegabyte;
                    int totalDigitCount = totalMegaBytes > 0 ? (int)Math.Log10(totalMegaBytes) + 1 : 0;

                    PXWait.ShowProgress(percentDone, 
                                        string.Concat(Convert.ToString(transferedMegaBytes, CultureInfo.InvariantCulture).PadLeft(totalDigitCount),
                                                      " MB / ",
                                                      Convert.ToString(totalMegaBytes, CultureInfo.InvariantCulture),
                                                      " MB downloaded"));
                }); ;
            }
            else if (sender == RemoveButton)
            {
                RemoveBuildPackage(buildPackage);
            }
            else if (sender == InstallButton)
            {
                InstallBuildPackage(buildPackage);
            }
            else if (sender == LaunchButton)
            {
                LaunchAcumaticaWizard(buildPackage);
            }
            else if (sender == OpenFolderButton)
            {
                OpenBuildPackageFolder(buildPackage);
            }
            else if (sender == UninstallButton)
            {
                UninstallBuildPackage(buildPackage);
            }
        }

        private void DownloadBuildPackage(BuildPackage buildPackage, BuildManager.ProgressCallbackDelegate progressCallback)
        {
            BuildManager.DownloadPackage(buildPackage, progressCallback);

            if (File.Exists(BuildManager.GetPathFromKey(buildPackage.Key)))
            {
                buildPackage.SetIsLocal(true);
            }
            else
            {
                throw new Exception("Failed to download remote file to local directory.");
            }
        }

        private void InstallBuildPackage(BuildPackage buildPackage)
        {
            InstallManager.InstallAcumatica(buildPackage, (object sender, EventArgs e) =>
            {
                string filePath = BuildManager.GetPathFromKey(buildPackage.Key);
                string directory = Path.GetDirectoryName(filePath);
                string installDirectory = Path.Combine(directory, "Files");
                string wizardPath = Path.Combine(installDirectory, "Data", "AcumaticaConfig.exe");

                if (File.Exists(wizardPath))
                {
                    buildPackage.SetIsLocal(true);
                    buildPackage.SetIsInstalled(true);
                }
                else
                {
                    throw new Exception("Failed to install Acumatica ERP.");
                }
            });
        }

        private void RemoveBuildPackage(BuildPackage buildPackage)
        {
            string filePath = BuildManager.GetPathFromKey(buildPackage.Key);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);

                string directory = Path.GetDirectoryName(filePath);

                if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
                {
                    Directory.Delete(directory, true);
                    buildPackage.SetIsInstalled(false);
                }

                buildPackage.SetIsLocal(false);
            }
            else
            {
                throw new Exception("Failed to remove local file.");
            }
        }

        private void LaunchAcumaticaWizard(BuildPackage buildPackage)
        {
            InstallManager.LaunchAcumaticaWizard(buildPackage);
        }

        private void OpenBuildPackageFolder(BuildPackage buildPackage)
        {
            string directory = Path.GetDirectoryName(BuildManager.GetPathFromKey(buildPackage.Key));

            if (Directory.Exists(directory))
            {
                Process.Start(directory);
                return;
            }

            throw new Exception(string.Concat("Install directory not found: ", Environment.NewLine, directory));
        }

        private void UninstallBuildPackage(BuildPackage buildPackage)
        {
            string directory = Path.GetDirectoryName(BuildManager.GetPathFromKey(buildPackage.Key));

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                buildPackage.SetIsLocal(false);
                buildPackage.SetIsInstalled(false);
                return;
            }

            throw new Exception(string.Concat("Install directory not found: ", Environment.NewLine, directory));
        }
        #endregion
        
        private void ReloadWebSites()
        {
            new Thread(new ThreadStart(delegate
            {
                Invoke((MethodInvoker)delegate
                {try
                    {
                        PXWait.StartWait(this);
                        const string registryKey = @"SOFTWARE\Acumatica ERP";
                        List<WebSite> webSites = new List<WebSite>();

                        foreach (RegistryView registryView in new RegistryView[] { RegistryView.Registry32, RegistryView.Registry64 })
                        {
                            RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView);

                            if (baseKey != null)
                            {
                                RegistryKey key = baseKey.OpenSubKey(registryKey, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

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
                                            string path = subKey.GetValue("Path") as string;
                                            string configFile = Path.Combine(path, "web.config");
                                            string virtualName = subKey.GetValue("VirtDirName") as string;
                                            string webSiteName = subKey.GetValue("WebSiteName") as string;

                                            ServerManager server = new ServerManager();
                                            SiteCollection sites = server.Sites;
                                            Site site = sites[webSiteName];
                                            Microsoft.Web.Administration.Application app = site.Applications[string.Concat("/", virtualName)];
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
                                                    url.Append(virtualName);
                                                    url.Append("/Main.aspx");

                                                    if (!File.Exists(configFile))
                                                    {
                                                        webSites.Add(new WebSite(subKeyName,
                                                                                 null,
                                                                                 null,
                                                                                 null,
                                                                                 path,
                                                                                 url.ToString(),
                                                                                 null));
                                                        continue;
                                                    }

                                                    WebConfiguration wm = new WebConfiguration(configFile, true);
                                                    DbmsProviderService ServerType = ProviderLocator.GetProviderByPxDataProviderClassName(wm.DatabaseProvider);

                                                    ConnectionDefinition connection = DatabaseProvider.ParseConnectionString(DatabaseProvider.GetProvider(ServerType.name), wm.ConnectionString);
                                                    DBInfo DatabaseInfo = DBInfo.DatabaseTest(connection);
                                                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Path.Combine(path, "bin", "px.data.dll"));

                                                    webSites.Add(new WebSite(subKeyName,
                                                                             string.Concat(connection.Server, "/", connection.Database),
                                                                             new Version(fvi.FileVersion).ToString(),
                                                                             DatabaseInfo.GetVersion(),
                                                                             path,
                                                                             url.ToString(),
                                                                             ServerType.name.ToUpperInvariant()));

                                                    PXWait.ShowProgress((int)(((float)counter / total) * 100f),
                                                                        string.Concat("Scanning instances: ",
                                                                                      Convert.ToString(++counter, CultureInfo.InvariantCulture).PadLeft(totalDigitCount),
                                                                                      "/",
                                                                                      Convert.ToString(total, CultureInfo.InvariantCulture)));
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        BuildPackageBindingSource.CurrencyManager.SuspendBinding();
                        WebSitesBindingSource.Clear();

                        foreach (WebSite webSite in webSites)
                            WebSitesBindingSource.Add(webSite);

                        BuildPackageBindingSource.CurrencyManager.ResumeBinding();
                        PXWait.StopWait();
                    }
                    catch (Exception ex)
                    {
                        ex = ex;
                    }
                });
            })).Start();
        }

        private void FilterWebSites()
        {
            try
            {
                instanceNameFilter = InstanceNameFilterTextBox.Text;
                InstanceNameFilteredLabel.Visible = !string.IsNullOrWhiteSpace(instanceNameFilter);

                // Save selection
                WebSite selectedWebSite = GetSelectedWebSite();

                if (selectedWebSite != null)
                {
                    selectedRowKey = selectedWebSite.InstanceName;
                }

                // Freeze binding
                WebSitesBindingSource.CurrencyManager.SuspendBinding();
                
                // Filter rows
                for (int i = 0; i < WebSitesDataGridView.Rows.Count; i++)
                {
                    DataGridViewRow row = WebSitesDataGridView.Rows[i];

                    if (row != null)
                    {
                        WebSite webSite = row.DataBoundItem as WebSite;

                        row.Visible = (webSite != null && 
                                       (string.IsNullOrWhiteSpace(instanceNameFilter) || 
                                        webSite.InstanceName.ToUpperInvariant().Contains(instanceNameFilter.ToUpperInvariant())));
                    }
                }

                // Resume binding
                WebSitesBindingSource.CurrencyManager.ResumeBinding();

                // Restore selection
                WebSitesDataGridView.ClearSelection();

                if (selectedRowKey != null)
                {
                    for (int i = 0; i < WebSitesDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = WebSitesDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            WebSite webSite = row.DataBoundItem as WebSite;

                            if (webSite.InstanceName == selectedRowKey)
                            {
                                WebSitesDataGridView.Rows[i].Selected = true;
                                WebSitesDataGridView.FirstDisplayedScrollingRowIndex = i;
                                EnableControlsWebSite(webSite);
                                break;
                            }
                        }
                    }
                }

                // Select first displayed row
                if (WebSitesDataGridView.SelectedRows.Count == 0 &&
                    WebSitesDataGridView.DisplayedRowCount(true) > 0)
                {
                    for (int i = 0; i < WebSitesDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = WebSitesDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            WebSite webSite = row.DataBoundItem as WebSite;
                            WebSitesDataGridView.Rows[i].Selected = true;
                            WebSitesDataGridView.FirstDisplayedScrollingRowIndex = i;
                            selectedRowKey = webSite.InstanceName;
                            EnableControlsWebSite(webSite);
                            break;
                        }
                    }
                }

                // No selection
                if (WebSitesDataGridView.SelectedRows.Count == 0)
                {
                    EnableControlsWebSite(null);
                }
            }
            catch (Exception ex)
            {
                SysData.ShowException(ex.Message, ErrorLevel.Error);
            }
        }

        private void EnableControlsWebSite(WebSite webSite)
        {
            if (webSite != null)
            {
                BackupButton.Enabled = true;
            }
            else
            {
                BackupButton.Enabled = false;
            }

            BuildPackageDataGridView.Update();
        }

        #region Private Methods

        #region Build Package
        private bool CompareVersionMaskBuildPackage(BuildPackage buildPackage, string filterVersion)
        {
            const int majorVersionLength = 1;
            const int minorVersionLength = 2;
            const int buildNumberLength = 4;
            const char versionEmptyChar = '0';
            const char versionPromptChar = '_';
            const char versionSeparatorChar = '.';

            string version = string.Concat(Convert.ToString(buildPackage.MajorVersion, CultureInfo.InvariantCulture).PadLeft(majorVersionLength, versionPromptChar),
                                           versionSeparatorChar,
                                           Convert.ToString(buildPackage.MinorVersion, CultureInfo.InvariantCulture).PadLeft(minorVersionLength, versionEmptyChar),
                                           versionSeparatorChar,
                                           Convert.ToString(buildPackage.BuildNumber, CultureInfo.InvariantCulture).PadLeft(buildNumberLength, versionEmptyChar));

            for (int i = 0; i < version.Length; i++)
            {
                char charFilter = filterVersion[i];
                char charVersion = version[i];

                if (charFilter != versionPromptChar &&
                    charFilter != versionSeparatorChar &&
                    charFilter != charVersion)
                {
                    return false;
                }
            }

            return true;
        }

        private void EnableControlsBuildPackage(BuildPackage buildPackage)
        {
            if (buildPackage != null)
            {
                string filePath = BuildManager.GetPathFromKey(buildPackage.Key);
                string directory = Path.GetDirectoryName(filePath);
                string installDirectory = Path.Combine(directory, "Files");
                string wizardPath = Path.Combine(installDirectory, "Data", "AcumaticaConfig.exe");

                bool isDirectory = Directory.Exists(directory);
                bool isInstallDirectory = Directory.Exists(installDirectory);
                bool isInstallFile = File.Exists(filePath);
                bool isAcumaticaWizard = File.Exists(wizardPath);

                DownloadButton.Enabled = !isInstallFile;
                RemoveButton.Enabled = isInstallFile;
                InstallButton.Enabled = isInstallFile;
                UninstallButton.Enabled = isDirectory;
                OpenFolderButton.Enabled = isDirectory;
                LaunchButton.Enabled = isInstallDirectory && isAcumaticaWizard;
            }
            else
            {
                DownloadButton.Enabled = false;
                RemoveButton.Enabled = false;
                InstallButton.Enabled = false;
                UninstallButton.Enabled = false;
                OpenFolderButton.Enabled = false;
                LaunchButton.Enabled = false;
            }

            BuildPackageDataGridView.Update();
        }

        private BuildPackage GetSelectedBuildPackage()
        {
            return BuildPackageDataGridView.SelectedRows.Count > 0 ? BuildPackageDataGridView.SelectedRows[0].DataBoundItem as BuildPackage : null;
        }

        private WebSite GetSelectedWebSite()
        {
            return WebSitesDataGridView.SelectedRows.Count > 0 ? WebSitesDataGridView.SelectedRows[0].DataBoundItem as WebSite : null;
        }

        private void ReloadBuildPackage()
        {
            new Thread(new ThreadStart(delegate
            {
                Invoke((MethodInvoker)delegate
                {
                    PXWait.StartWait(this);
                    List<BuildPackage> buildPackages = new List<BuildPackage>();

                    try
                    {
                        foreach (BuildPackage buildPackage in BuildManager.GetBuildPackages(delegate (int percentDone, long counter, long total)
                                                              {
                                                                  PXWait.ShowProgress(-1, string.Concat(" Loading build package: ", Convert.ToString(counter, CultureInfo.InvariantCulture)));
                                                              }))
                        {
                            buildPackages.Add(buildPackage);
                        }

                        FilterBuildPackages();
                    }
                    catch (Exception ex)
                    {
                        SysData.ShowException(ex.Message, ErrorLevel.Error);
                    }

                    BuildPackageBindingSource.CurrencyManager.SuspendBinding();
                    BuildPackageBindingSource.Clear();

                    foreach (BuildPackage buildPackage in buildPackages)
                        BuildPackageBindingSource.Add(buildPackage);
                    
                    BuildPackageBindingSource.CurrencyManager.ResumeBinding();
                    PXWait.StopWait();
                });
            })).Start();
        }

        private void FilterBuildPackages()
        {
            try
            {
                const string emptyVersionMask = "_.__.____";
                versionMask = VersionMaskedTextBox.Text;
                BuildPackageFilteredLabel.Visible = (versionMask != null && versionMask != emptyVersionMask);

                // Save selection
                BuildPackage selectedBuildPackage = GetSelectedBuildPackage();

                if (selectedBuildPackage != null)
                {
                    selectedRowKey = selectedBuildPackage.Key;
                }
                
                // Freeze binding
                BuildPackageBindingSource.CurrencyManager.SuspendBinding();

                // Filter rows
                for (int i = 0; i < BuildPackageDataGridView.Rows.Count; i++)
                {
                    DataGridViewRow row = BuildPackageDataGridView.Rows[i];

                    if (row != null)
                    {
                        BuildPackage buildPackage = row.DataBoundItem as BuildPackage;

                        row.Visible = (buildPackage != null &&
                                       CompareVersionMaskBuildPackage(buildPackage, VersionMaskedTextBox.Text) &&
                                       ((buildPackage.IsLocal && ShowLocalCheckBox.Checked) ||
                                       (buildPackage.IsRemote && ShowRemoteCheckBox.Checked) ||
                                       (buildPackage.IsInstalled && ShowInstalledCheckBox.Checked)));
                    }
                }

                // Resume binding
                BuildPackageBindingSource.CurrencyManager.ResumeBinding();

                // Restore selection
                BuildPackageDataGridView.ClearSelection();

                if (selectedRowKey != null)
                {
                    for (int i = 0; i < BuildPackageDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = BuildPackageDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            BuildPackage buildPackage = row.DataBoundItem as BuildPackage;

                            if (buildPackage.Key == selectedRowKey)
                            {
                                BuildPackageDataGridView.Rows[i].Selected = true;
                                BuildPackageDataGridView.FirstDisplayedScrollingRowIndex = i;
                                EnableControlsBuildPackage(buildPackage);
                                break;
                            }
                        }
                    }
                }

                // Select first displayed row
                if (BuildPackageDataGridView.SelectedRows.Count == 0 &&
                    BuildPackageDataGridView.DisplayedRowCount(true) > 0)
                {
                    for (int i = 0; i < BuildPackageDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = BuildPackageDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            BuildPackage buildPackage = row.DataBoundItem as BuildPackage;
                            BuildPackageDataGridView.Rows[i].Selected = true;
                            BuildPackageDataGridView.FirstDisplayedScrollingRowIndex = i;
                            selectedRowKey = selectedBuildPackage.Key;
                            EnableControlsBuildPackage(buildPackage);
                            break;
                        }
                    }
                }
                
                // No selection
                if (BuildPackageDataGridView.SelectedRows.Count == 0)
                {
                    EnableControlsBuildPackage(null);
                }
            }
            catch (Exception ex)
            {
                SysData.ShowException(ex.Message, ErrorLevel.Error);
            }
        }
        #endregion Build Package

        #endregion

        #region Format WebSite Cells

        private void FormatWebSiteCellDBVersion(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, WebSite webSite, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = "Can't get version.";
                row.Cells["DBVersion"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            else if (webSite.SiteVersion != webSite.DBVersion)
            {
                e.CellStyle.BackColor = warningColor;
                row.Cells["DBVersion"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
                row.Cells["DBVersion"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void FormatWebSiteCellSiteVersion(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, WebSite webSite, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = "Can't get version.";
                row.Cells["SiteVersion"].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            else if (webSite.SiteVersion != webSite.DBVersion)
            {
                e.CellStyle.BackColor = warningColor;
                row.Cells["SiteVersion"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
                row.Cells["SiteVersion"].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void FormatWebSiteCellSitePath(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, WebSite webSite, Color errorColor, Color warningColor, Color successColor)
        {
            if (e.Value == null /*|| !Directory.Exists(e.Value as string)*/)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = "Can't find site directory.";
                row.Cells["SitePath"].ToolTipText = "Can't find site directory.";
            }
            else
            {
                e.CellStyle.BackColor = successColor;
                row.Cells["SitePath"].ToolTipText = string.Empty;
            }
        }

        private void FormatWebSiteCellDatabase(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, WebSite webSite, Color errorColor, Color warningColor, Color successColor)
        {
            const string MSSQLServerType = "MSSQLSERVER";

            if (e.Value == null)
            {
                e.CellStyle.BackColor = errorColor;
                e.Value = "Can't read web.config file.";
            }
            else if (webSite.ServerType != null && webSite.ServerType.ToUpperInvariant().Trim() != MSSQLServerType)
            {
                e.CellStyle.BackColor = errorColor;
            }
            else
            {
                e.CellStyle.BackColor = successColor;
            }
        }

        private void FormatWebSiteCellStatus(DataGridViewCellFormattingEventArgs e, DataGridViewRow row, WebSite webSite, Color errorTextColor, Color warningColor, Color successTextColor)
        {
            const string MSSQLServerType = "MSSQLSERVER";
            bool isSameVersion = (webSite.SiteVersion != null && webSite.SiteVersion == webSite.DBVersion);
            bool isDatabase = (webSite.Database != null);
            bool isWebConfig = (webSite.Database != null);
            bool isSiteVersion = (webSite.SiteVersion != null);
            bool isDBVersion = (webSite.DBVersion != null);
            bool isMSSql = (webSite.ServerType != null && webSite.ServerType.ToUpperInvariant().Trim() == MSSQLServerType);

            if (!isWebConfig || !isSiteVersion || !isDBVersion || !isDatabase || !isMSSql)
            {
                e.Value = StatusImageList.Images["error"];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = errorTextColor;

                    if (!isWebConfig || !isDatabase)
                        cell.ToolTipText = "Can't read web.config file.";
                    else if (!isMSSql)
                        cell.ToolTipText = string.Concat("Can't backup/restore ", webSite.ServerType, " databases.");
                    else if (!isSiteVersion || !isDBVersion)
                        cell.ToolTipText = "Can't get version.";
                }
            }
            else if (!isSameVersion)
            {
                e.Value = StatusImageList.Images["warning"];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = successTextColor;
                    cell.ToolTipText = "Database version doesn't coincide with the site's version.";
                }
            }
            else
            {
                e.Value = StatusImageList.Images["success"];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.Style.ForeColor = successTextColor;
                    cell.ToolTipText = string.Empty;
                }
            }
        }

        #endregion Format WebSite Cells
    }
}
