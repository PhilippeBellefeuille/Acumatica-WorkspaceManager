using Acumatica.WorkspaceManager.BackupRestore;
using Acumatica.WorkspaceManager.Common;
using ConfigCore;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main
    {
        #region Events

        private void DatabaseRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            bool isCreateNewDatabase = CreateNewDatabaseRadioButton.Checked;

            DatabaseNameLabel.Enabled = isCreateNewDatabase;
            DatabaseNameTextBox.Enabled = isCreateNewDatabase;
            DatabaseListBox.Enabled = !isCreateNewDatabase;
        }

        private void PickBackupFileButton_Click(object sender, EventArgs e)
        {
            BackupFilePathTextBox.Text = PickBackupFile();
            string backupFile = BackupFilePathTextBox.Text;

            if (File.Exists(backupFile) && Path.GetExtension(backupFile) == ".zip")
            {
                string instanceName = Path.GetFileNameWithoutExtension(backupFile);

                if (!String.IsNullOrWhiteSpace(instanceName))
                {
                    InstanceNameTextBox.Enabled = true;
                    InstanceNameTextBox.Text = instanceName;
                    DatabaseNameTextBox.Text = instanceName;
                    VirtualDirectoryNameTextBox.Text = instanceName;
                    SitePathTextBox.Text = Path.Combine(@"c:\AcumaticaSites\", instanceName);
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
                InitialDirectory = @"c:\AcumaticaSites",
                Title = "Select Instance Path"
            };

            if (folderSelectDialog.Show(Handle))
            {
                SitePathTextBox.Text = folderSelectDialog.FileName;
            }
        }
        #endregion

        #region Methods
        private void ExecuteRestoreAction(object sender)
        {
            string backupFile = BackupFilePathTextBox.Text;
            string instanceName = InstanceNameTextBox.Text;
            string virtualDirectory = VirtualDirectoryNameTextBox.Text;
            string databaseName = DatabaseNameTextBox.Text;
            string websiteName = Convert.ToString(WebsitesListBox.SelectedItem, CultureInfo.InvariantCulture);
            string appPool = Convert.ToString(AppPoolListBox.SelectedItem, CultureInfo.InvariantCulture);
            string sitePath = SitePathTextBox.Text;

            Website website = new Website(instanceName,
                                          virtualDirectory,
                                          "REM-LT-12/" + databaseName,
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
                                           DatabaseSettingsPanel.Visible,
                                           WebsiteSettingsPanel.Visible);

            RestorePanel.Visible = false;
            InstancePanel.Visible = true;
            InstancePanel.Focus();
            ReloadInstance(website.InstanceName);
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
                Filter = "Zip Archive|*.zip",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Title = "Load Backup File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK &&
                !string.IsNullOrWhiteSpace(openFileDialog.FileName))
            {
                return openFileDialog.FileName;
            }

            return null;
        }
        #endregion
    }
}
