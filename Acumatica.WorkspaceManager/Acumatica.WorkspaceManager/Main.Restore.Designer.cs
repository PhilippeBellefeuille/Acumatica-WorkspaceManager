using Acumatica.WorkspaceManager.BackupRestore;
using Acumatica.WorkspaceManager.Common;
using ConfigCore;
using Microsoft.Win32;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main
    {
        #region Events
        private void DatabaseListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OverwriteDatabaseRadioButton.Checked && DatabaseListBox.SelectedItem != null)
            {
                DatabaseNameTextBox.Text = DatabaseListBox.SelectedItem as string;
            }
        }

        private void DatabaseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            bool isCreateNewDatabase = CreateNewDatabaseRadioButton.Checked;

            DatabaseNameLabel.Enabled = isCreateNewDatabase;
            DatabaseNameTextBox.Enabled = isCreateNewDatabase;
            DatabaseListBox.Enabled = !isCreateNewDatabase;
        }
        
        private void InstanceNameTextBox_TextChanged(object sender, EventArgs e)
        {
            string instanceName = InstanceNameTextBox.Text.Trim();
            DatabaseNameTextBox.Text = instanceName;
            VirtualDirectoryNameTextBox.Text = instanceName;

            if (!string.IsNullOrWhiteSpace(SitePathTextBox.Text) && !SitePathTextBox.Text.EndsWith(instanceName))
            {
                SitePathTextBox.Text = Path.Combine(Directory.GetParent(SitePathTextBox.Text).FullName, instanceName);
            }
        }

        private void PickBackupFileButton_Click(object sender, EventArgs e)
        {
            BackupFilePathTextBox.Text = PickBackupFile();
            string backupFile = BackupFilePathTextBox.Text;

            if (File.Exists(backupFile) && Path.GetExtension(backupFile) == Constants.zipFileExtension)
            {
                string instanceName = Path.GetFileNameWithoutExtension(backupFile);

                if (!String.IsNullOrWhiteSpace(instanceName))
                {
                    InstanceNameTextBox.Enabled = true;
                    InstanceNameTextBox.Text = instanceName;
                    DatabaseNameTextBox.Text = instanceName;
                    VirtualDirectoryNameTextBox.Text = instanceName;
                    SitePathTextBox.Text = Path.Combine(Constants.defaultSitePath, instanceName);
                    InstanceNameLabel.Enabled = true;
                    SitePathLabel.Enabled = true;
                    PickSitePathButton.Enabled = true;
                    DatabaseSettingsPanel.Enabled = true;
                    WebsiteSettingsPanel.Enabled = true;
                    WebsiteSettingsPanel.Enabled = true;
                    AppPoolSettingsPanel.Enabled = true;
                    ExecuteRestoreButton.Enabled = true;
                }
            }
        }

        private void PickSitePathButton_Click(object sender, EventArgs e)
        {
            FolderSelectDialog folderSelectDialog = new FolderSelectDialog
            {
                InitialDirectory = Constants.defaultSitePath,
                Title = Messages.selectInstancePathTitle
            };

            if (folderSelectDialog.Show(Handle))
            {
                string instanceName = InstanceNameTextBox.Text.Trim();
                string path = folderSelectDialog.FileName;

                if (!path.EndsWith(instanceName))
                {
                    path = Path.Combine(path, instanceName);
                }

                SitePathTextBox.Text = path;
            }
        }
        #endregion

        #region Methods
        private void ExecuteRestoreAction(object sender)
        {
            bool isRestoreDatabase = DatabaseSettingsPanel.Visible;
            bool isRestoreWebsite = WebsiteSettingsPanel.Visible;

            string backupFile = BackupFilePathTextBox.Text.Trim();
            string instanceName = InstanceNameTextBox.Text.Trim();
            string virtualDirectory = VirtualDirectoryNameTextBox.Text.Trim();
            string databaseName = DatabaseNameTextBox.Text.Trim();
            string websiteName = Convert.ToString(WebsitesListBox.SelectedItem, CultureInfo.InvariantCulture);
            string appPool = Convert.ToString(AppPoolListBox.SelectedItem, CultureInfo.InvariantCulture);
            string sitePath = SitePathTextBox.Text.Trim();

            ValidateRestore(isRestoreDatabase, isRestoreWebsite, backupFile, instanceName, virtualDirectory, databaseName, websiteName, appPool, sitePath);

            Website website = new Website(instanceName,
                                          virtualDirectory,
                                          string.Concat(ServerNameComboBox.Text, Constants.slash, databaseName),
                                          null,
                                          null,
                                          sitePath,
                                          null,
                                          null,
                                          Enum.GetName(typeof(SiteTypes), SiteTypes.RegularSite),
                                          websiteName);

            RestoreManager.RestoreInstance(BuildServerConnection(ServerNameComboBox.Text),
                                           website,
                                           backupFile,
                                           appPool,
                                           isRestoreDatabase,
                                           isRestoreWebsite);

            RestorePanel.Visible = false;
            InstancePanel.Visible = true;
            InstancePanel.Focus();
            ReloadInstance(website.InstanceName);
        }

        private bool IsInstance(string instanceName)
        {
            foreach (RegistryView registryView in new RegistryView[] { RegistryView.Registry32, RegistryView.Registry64 })
            {
                RegistryKey baseKey = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, registryView);

                if (baseKey != null)
                {
                    RegistryKey key = baseKey.OpenSubKey(Constants.acumaticaRegistryKey, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);

                    if (key != null)
                    {
                        string[] subKeys = key.GetSubKeyNames();

                        foreach (string subKeyName in subKeys)
                        {
                            if (subKeyName.Trim() == instanceName.Trim())
                            {
                                return true;
                            }

                            RegistryKey subKey = key.OpenSubKey(subKeyName, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
                            string virtualDirectory = subKey.GetValue(Constants.virtualDirectoryRegistryValue) as string;

                            if (virtualDirectory != null && virtualDirectory.Trim() == instanceName)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        private void LoadAppPoolsList()
        {
            AppPoolListBox.Items.Clear();
            IISManagment.FillAppPoolsList();
            IISManagment.ApplicationPools.ToList().ForEach(poolInfo => AppPoolListBox.Items.Add(poolInfo.Name));

            if (AppPoolListBox.Items.Count == 1)
            {
                AppPoolListBox.SelectedIndex = 0;
            }

            for (int i = 0; i < AppPoolListBox.Items.Count; i++)
            {
                if (AppPoolListBox.Items[i].ToString().Contains(Titles.DefaultAspNet4AppPool))
                    AppPoolListBox.SelectedIndex = i;
            }

            for (int i = 0; i < AppPoolListBox.Items.Count; i++)
            {
                if (AppPoolListBox.Items[i].ToString().Contains(Titles.ClassicAspNet4AppPool))
                    AppPoolListBox.SelectedIndex = i;
            }

            for (int i = 0; i < AppPoolListBox.Items.Count; i++)
            {
                if (AppPoolListBox.Items[i].ToString().Contains(Titles.DefaultAppPool))
                    AppPoolListBox.SelectedIndex = i;
            }
        }

        private void LoadDatabaseList()
        {
            DatabaseListBox.Items.Clear();
            DatabaseListBox.Items.AddRange(DatabaseProvider.GetProvider().DatabaseList().ToArray());
        }

        private void LoadWebsitesList()
        {
            WebsitesListBox.Items.Clear();
            IISManagment.FillSitesList();
            IISManagment.Instances.ToList().ForEach(instance => WebsitesListBox.Items.Add(instance.DisplayName));

            if (WebsitesListBox.Items.Count == 1)
            {
                WebsitesListBox.SelectedIndex = 0;
            }
            
            for (int i = 0; i < WebsitesListBox.Items.Count; i++)
            {
                if (WebsitesListBox.Items[i].ToString().Contains(Titles.DefWebSite))
                {
                    WebsitesListBox.SelectedIndex = i;
                }
            }
        }
        
        private string PickBackupFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = Constants.zipFileFilter,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = Messages.loadBackupFileTitle
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        private void ValidateRestore(bool isRestoreDatabase, bool isRestoreWebsite, string backupFile, string instanceName, string virtualDirectory, string databaseName, string websiteName, string appPool, string sitePath)
        {
            if (isRestoreWebsite && !string.IsNullOrWhiteSpace(instanceName) && IsInstance(instanceName.Trim()))
            {
                throw new Exception(string.Format(Messages.instanceAlreadyExistsError, instanceName.Trim()));
            }

            if (string.IsNullOrWhiteSpace(backupFile))
            {
                throw new Exception(Messages.missingBackupFileError);
            }

            if (isRestoreWebsite && string.IsNullOrWhiteSpace(instanceName))
            {
                throw new Exception(Messages.missingInstanceNameError);
            }

            if (isRestoreWebsite && string.IsNullOrWhiteSpace(virtualDirectory))
            {
                throw new Exception(Messages.missingVirtualDirectoryError);
            }

            if (isRestoreDatabase && string.IsNullOrWhiteSpace(databaseName))
            {
                throw new Exception(Messages.missingDatabaseNameError);
            }

            if (isRestoreWebsite && string.IsNullOrWhiteSpace(websiteName))
            {
                throw new Exception(Messages.missingWebsiteNameError);
            }

            if (string.IsNullOrWhiteSpace(appPool))
            {
                throw new Exception(Messages.missingApplicationPoolError);
            }

            if (isRestoreWebsite && string.IsNullOrWhiteSpace(sitePath))
            {
                throw new Exception(Messages.missingInstancePathError);
            }
        }
        #endregion
    }
}
