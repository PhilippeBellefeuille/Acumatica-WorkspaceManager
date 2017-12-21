using ConfigCore;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main
    {
        #region Events
        private void Button_Click(object sender, EventArgs e)
        {
            if (sender == RestoreBackButton)
            {
                RestorePanel.Visible = false;
                InstancePanel.Visible = true;
            }
            else if (sender == PackageBackButton)
            {
                PackagePanel.Visible = false;
                MenuPanel.Visible = true;
            }
            else if (sender == InstanceBackButton)
            {
                InstancePanel.Visible = false;
                MenuPanel.Visible = true;
            }
            else if (sender == ServerSettingsButton)
            {
                InstancePanel.Visible = false;
                DatabasePanel.Visible = true;
            }
            else if (sender == DatabaseBackButton)
            {
                DatabasePanel.Visible = false;
                InstancePanel.Visible = true;
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
                PXWait.StopWait();
                SysData.ShowException(ex.ToString(), ErrorLevel.Error);
            }
        }

        private void MenuItem_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MenuPanel.Visible = false;

            if (sender == PackageMenuLinkLabel)
            {
                PackagePanel.Visible = true;

                if (packageVersionFilter == null)
                {
                    ReloadPackage();
                }

                PackageDataGridView.Focus();
            }
            else if (sender == BackupRestoreMenuLinkLabel)
            {
                InstancePanel.Visible = true;

                if (instanceNameFilter == null)
                {
                    ReloadInstance(null);
                }

                InstanceDataGridView.Focus();
            }
        }
        
        private void StartSQLBrowserButton_Click(object sender, EventArgs e)
        {
                new Thread(new ThreadStart(delegate
                {
                    Invoke((MethodInvoker)delegate
                    {
                        try
                        {
                            PXWait.StartWait(this);
                            PXWait.ShowProgress(-1, Messages.startingSQLBrowserProgress);

                            ServiceController sqlBrowser = new ServiceController(Constants.sqlServerBrowserServiceName);

                            if (sqlBrowser != null)
                            {
                                sqlServerBrowserServiceStatus = sqlBrowser.Status;
                                ServiceHelper.ChangeStartMode(sqlBrowser, System.ServiceProcess.ServiceStartMode.Automatic);

                                if (sqlBrowser.Status != ServiceControllerStatus.Running)
                                {
                                    sqlBrowser.Start();
                                    sqlBrowser.WaitForStatus(ServiceControllerStatus.Running);
                                }
                            }

                            LoadServerList();
                        }
                        catch (Exception ex)
                        {
                            PXWait.StopWait();
                            SysData.ShowException(ex.ToString(), ErrorLevel.Error);
                        }
                    });
                })).Start();
        }

        private void SQLServerAuthRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            LoginLabel.Enabled = SQLServerAuthRadioButton.Checked;
            DatabaseLoginTextBox.Enabled = SQLServerAuthRadioButton.Checked;
            PasswordLabel.Enabled = SQLServerAuthRadioButton.Checked;
            DatabasePasswordTextBox.Enabled = SQLServerAuthRadioButton.Checked;
        }
        #endregion

        #region Methods
        private void ExecuteAction(object sender)
        {
            if (PackagePanel.Visible)
            {
                ExecutePackageAction(sender);
                FilterPackage();
                PackageDataGridView.Focus();
            }
            else if (InstancePanel.Visible)
            {
                ExecuteInstanceAction(sender);
                FilterInstances(null);
                InstancePanel.Focus();
            }
            else if (RestorePanel.Visible)
            {
                ExecuteRestoreAction(sender);
            }
        }

        private void LoadServerList()
        {
            try
            {
                ServiceController sqlBrowser = new ServiceController(Constants.sqlServerBrowserServiceName);
                bool isSQLBrowserRunning = (sqlBrowser != null && sqlBrowser.Status == ServiceControllerStatus.Running);
                string selectedServer = ServerNameComboBox.Text;
                ServerNameComboBox.Items.Clear();

                if (isSQLBrowserRunning)
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            PXWait.StartWait(this);
                            PXWait.ShowProgress(-1, Messages.discoveringDatabaseProgress);

                            foreach (DataRow serverRow in SmoApplication.EnumAvailableSqlServers(true).Rows)
                            {
                                ServerNameComboBox.Items.Add(serverRow.ItemArray[serverRow.Table.Columns.IndexOf(serverRow.Table.Columns[Constants.serverColumn])]);
                            }

                            if (!string.IsNullOrWhiteSpace(selectedServer) &&
                                ServerNameComboBox.Items.Contains(selectedServer))
                            {
                                ServerNameComboBox.Text = selectedServer;
                                StartSQLBrowserButton.ImageIndex = StatusImageList.Images.IndexOfKey(Constants.successImage);
                            }
                            else if (ServerNameComboBox.Items.Count > 0)
                            {
                                ServerNameComboBox.Text = ServerNameComboBox.Items[0] as string;
                                StartSQLBrowserButton.ImageIndex = StatusImageList.Images.IndexOfKey(Constants.warningImage);
                            }
                            else
                            {
                                ServerNameComboBox.Text = string.Empty;
                                StartSQLBrowserButton.ImageIndex = StatusImageList.Images.IndexOfKey(Constants.errorImage);
                            }

                            sqlServerBrowserServiceStatus = sqlBrowser.Status;
                            ServiceHelper.ChangeStartMode(sqlBrowser, System.ServiceProcess.ServiceStartMode.Automatic);

                            if (sqlBrowser.Status != sqlServerBrowserServiceStatus)
                            {
                                switch (sqlBrowser.Status)
                                {
                                    case ServiceControllerStatus.Paused:
                                        sqlBrowser.Pause();
                                        break;
                                    case ServiceControllerStatus.Running:
                                        sqlBrowser.Start();
                                        break;
                                    default:
                                        sqlBrowser.Stop();
                                        break;
                                }
                            }

                            PXWait.StopWait();
                        });
                    })).Start();
                }
                else
                {
                    ServerNameComboBox.Items.Add(Environment.MachineName);

                    if (ServerNameComboBox.Items.Count > 0)
                    {
                        ServerNameComboBox.Text = ServerNameComboBox.Items[0] as string;
                        StartSQLBrowserButton.ImageIndex = StatusImageList.Images.IndexOfKey(Constants.warningImage);
                    }

                    PXWait.StopWait();
                }
            }
            catch (Exception ex)
            {
                PXWait.StopWait();
                SysData.ShowException(ex.ToString(), ErrorLevel.Error);
            }
        }
        #endregion
    }
}
