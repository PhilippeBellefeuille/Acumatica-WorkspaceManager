using ConfigCore;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main
    {
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
    }
}
