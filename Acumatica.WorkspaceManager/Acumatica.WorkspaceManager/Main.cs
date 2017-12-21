using ConfigCore;
using Microsoft.SqlServer.Management.Common;
using PX.WebConfig;
using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
        private ServiceControllerStatus sqlServerBrowserServiceStatus;
        #endregion

        #region Constructor
        public Main()
        {
            InitializeComponent();

            SysData.Context = ReflectionHelper.CreateInstance<ERPConfig.ERPConfigContext>();
            Size desiredSize = new Size(800, 480);
            MinimumSize = desiredSize;
            Size = desiredSize;
            ServerNameComboBox.Text = Environment.MachineName;
        }
        #endregion

        #region Events
        private void ActionControl_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(delegate
            {
                Invoke((MethodInvoker)delegate
                {
                    try
                    {
                        PXWait.StartWait(this);
                        ExecuteAction(sender);
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
                     sender == ShowInstalledCheckBox ||
                     sender == ShowPreviewCheckBox)
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
        #endregion
    }
}
