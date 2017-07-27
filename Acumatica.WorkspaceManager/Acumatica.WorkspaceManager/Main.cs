using ConfigCore;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main : Form
    {
        #region Variables
        private string instanceNameFilter;
        private string packageVersionFilter;
        private string selectedRow;
        #endregion

        #region Constructor
        public Main()
        {
            InitializeComponent();

            SysData.Context = ReflectionHelper.CreateInstance<ERPConfig.ERPConfigContext>();
            Size desiredSize = new Size(800, 480);
            MinimumSize = desiredSize;
            Size = desiredSize;
        }
        #endregion

        #region Events
        private void ActionControl_Click(object sender, EventArgs e)
        {
            try
            {
                new Thread(new ThreadStart(delegate
                {
                    Invoke((MethodInvoker)delegate
                    {
                        PXWait.StartWait(this);
                        ExecuteAction(sender);
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

        private void BackButton_Click(object sender, EventArgs e)
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
        }

        private void BindingSource_CurrentChanged(object sender, EventArgs e)
        {
            if (sender == PackageBindingSource)
            {
                EnablePackageControls(GetSelectedPackage());
            }
            else if (sender == InstanceBindingSource)
            {
                EnableInstanceControls(GetSelectedInstance());
            }
        }

        private void DataGridView_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsLetter(e.KeyChar) || char.IsDigit(e.KeyChar))
            {
                DataGridView dataGridView = sender as DataGridView;
                int index = 0;

                if (dataGridView.SelectedRows.Count > 0)
                {
                    index = dataGridView.SelectedRows[0].Index + 1;
                }

                for (int i = index; i < (dataGridView.Rows.Count + index); i++)
                {
                    if (dataGridView.Rows[i % dataGridView.Rows.Count].Cells[1].Value.ToString().StartsWith(e.KeyChar.ToString(), true, CultureInfo.InvariantCulture))
                    {
                        dataGridView.Rows.Cast<DataGridViewRow>().Where(row => row.Selected).ToList().ForEach(row => row.Selected = false);
                        dataGridView.Rows[i % dataGridView.Rows.Count].Cells[0].Selected = true;
                        return;
                    }
                }
            }
        }
        
        private void ReloadControl_EventHandler(object sender, EventArgs e)
        {
            if (sender == PackageReloadButton)
            {
                ReloadPackage();
            }
            else if ((sender == PackageVersionMaskedTextBox && packageVersionFilter != PackageVersionMaskedTextBox.Text) ||
                     sender == ShowRemoteCheckBox ||
                     sender == ShowLocalCheckBox ||
                     sender == ShowInstalledCheckBox)
            {
                FilterPackage();
            }
            else if (sender == InstanceReloadButton)
            {
                ReloadInstance(null);
            }
            else if (sender == InstanceNameFilterTextBox)
            {
                FilterInstances(null);
            }
            else if (sender == ReloadWebsitesButton)
            {
                LoadWebsitesList();
            }
            else if (sender == ReloadAppPoolsButton)
            {
                LoadAppPoolsList();
            }
        }

        private void SQLServerAuthRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            LoginLabel.Enabled = SQLServerAuthRadioButton.Checked;
            DatabaseLoginTextBox.Enabled = SQLServerAuthRadioButton.Checked;
            PasswordLabel.Enabled = SQLServerAuthRadioButton.Checked;
            DatabasePasswordTextBox.Enabled = SQLServerAuthRadioButton.Checked;
        }

        protected override void WndProc(ref Message message)
        {
            const uint WM_SYSCOMMAND = 0x0112;
            const uint SC_RESTORE = 0xF120;
            const uint WM_ACTIVATEAPP = 0x001C;

            if (message.Msg == WM_ACTIVATEAPP || (message.Msg == WM_SYSCOMMAND && (int)message.WParam == SC_RESTORE))
            {
                PXWait.SetForeground();
            }

            base.WndProc(ref message);
        }
        #endregion

        #region Methods
        private ServerConnection BuildServerConnection(string dbServer)
        {
            if (SQLServerAuthRadioButton.Checked)
            {
                return new ServerConnection(dbServer, 
                                            DatabaseLoginTextBox.Text.Trim(),
                                            DatabasePasswordTextBox.Text.Trim());
            }
            else if (!string.IsNullOrWhiteSpace(dbServer))
            {
                return new ServerConnection(dbServer);
            }

            return new ServerConnection();
        }

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
                const string serviceName = "SQL Server Browser";

                bool isSQLBrowserRunning = new ServiceController(serviceName).Status == ServiceControllerStatus.Running;
                StartSQLBrowserButton.Visible = !isSQLBrowserRunning;
                StartSQLBrowserLabel.Visible = !isSQLBrowserRunning;
                ServerNameComboBox.Items.Clear();

                if (isSQLBrowserRunning)
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            PXWait.StartWait(this);
                            PXWait.ShowProgress(-1, "Discovering database servers...");

                            foreach (DataRow serverRow in SmoApplication.EnumAvailableSqlServers(true).Rows)
                            {
                                ServerNameComboBox.Items.Add(serverRow.ItemArray[serverRow.Table.Columns.IndexOf(serverRow.Table.Columns["Server"])]);
                            }

                            if (ServerNameComboBox.Items.Count > 0)
                            {
                                ServerNameComboBox.Text = ServerNameComboBox.Items[0] as string;
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
                    }

                    PXWait.StopWait();
                }
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
        #endregion

        private void DatabaseListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OverwriteDatabaseRadioButton.Checked && DatabaseListBox.SelectedItem != null)
            {
                DatabaseNameTextBox.Text = DatabaseListBox.SelectedItem as string;
            }
        }

        private void StartSQLBrowserButton_Click(object sender, EventArgs e)
        {
            try
            {
                new Thread(new ThreadStart(delegate
                {
                    Invoke((MethodInvoker)delegate
                    {
                        PXWait.StartWait(this);
                        PXWait.ShowProgress(-1, "Starting SQL Server Browser...");

                        const string serviceName = "SQL Server Browser";
                        ServiceController sqlBrowser = new ServiceController(serviceName);
                        ServiceController svc = new ServiceController(serviceName);
                        ServiceHelper.ChangeStartMode(svc, System.ServiceProcess.ServiceStartMode.Automatic);

                        if (sqlBrowser.Status != ServiceControllerStatus.Running)
                        {
                            sqlBrowser.Start();
                            sqlBrowser.WaitForStatus(ServiceControllerStatus.Running);
                        }

                        LoadServerList();
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

        private void Main_Load(object sender, EventArgs e)
        {
            LoadServerList();
        }
    }
}
