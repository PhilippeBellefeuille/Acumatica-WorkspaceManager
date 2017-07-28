using Acumatica.WorkspaceManager.Builds;
using Acumatica.WorkspaceManager.Common;
using Acumatica.WorkspaceManager.Install;
using ConfigCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main
    {
        #region Events
        private void PackageDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = PackageDataGridView.Rows[e.RowIndex] as DataGridViewRow;

            if (row != null)
            {
                BuildPackage buildPackage = row.DataBoundItem as BuildPackage;

                if (buildPackage != null)
                {
                    if (e.ColumnIndex == PackageDataGridView.Columns[Constants.packageStatusColumn].Index)
                    {
                        if (buildPackage.IsInstalled)
                        {
                            e.Value = StatusImageList.Images[Constants.successImage];
                        }
                        else if (buildPackage.IsLocal)
                        {
                            e.Value = StatusImageList.Images[Constants.localImage];
                        }
                        else if (buildPackage.IsRemote)
                        {
                            e.Value = StatusImageList.Images[Constants.remoteImage];
                        }
                        else
                        {
                            e.Value = new Bitmap(16, 16);
                        }
                    }
                }
            }
        }

        private void PackageVersionMaskedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && packageVersionFilter != PackageVersionMaskedTextBox.Text)
            {
                FilterPackage();
            }
        }
        #endregion

        #region Methods
        private void ExecutePackageAction(object sender)
        {
            BuildPackage buildPackage = GetSelectedPackage();

            if (buildPackage != null)
            {
                if (sender == DownloadButton)
                {
                    InstallManager.DownloadPackage(buildPackage);
                }
                else if (sender == RemoveButton)
                {
                    InstallManager.RemovePackage(buildPackage);
                }
                else if (sender == InstallButton)
                {
                    InstallManager.InstallPackage(buildPackage);
                }
                else if (sender == LaunchButton)
                {
                    InstallManager.LaunchAcumaticaWizard(buildPackage);
                }
                else if (sender == OpenFolderButton)
                {
                    InstallManager.OpenPackageFolder(buildPackage);
                }
                else if (sender == UninstallButton)
                {
                    InstallManager.UninstallPackage(buildPackage);
                }
            }
        }
        
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
            
            for (int i = 0; i < filterVersion.Length && i < version.Length; i++)
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

        private void EnablePackageControls(BuildPackage buildPackage)
        {
            bool isBuildPackage = (buildPackage != null);
            string filePath = (isBuildPackage ? BuildManager.GetPathFromKey(buildPackage.Key) : null);
            string directory = (filePath != null ? Path.GetDirectoryName(filePath) : null);
            string installDirectory = (directory != null ? Path.Combine(directory, Constants.filesDirectory) : null);
            string wizardPath = (installDirectory != null ? Path.Combine(installDirectory, Constants.dataDirectory, Constants.wizardFilename) : null);

            bool isDirectory = Directory.Exists(directory);
            bool isInstallDirectory = Directory.Exists(installDirectory);
            bool isInstallFile = File.Exists(filePath);
            bool isAcumaticaWizard = File.Exists(wizardPath);

            DownloadButton.Enabled = (isBuildPackage && !isInstallFile);
            RemoveButton.Enabled = isInstallFile;
            InstallButton.Enabled = isInstallFile;
            UninstallButton.Enabled = isDirectory;
            OpenFolderButton.Enabled = isDirectory;
            LaunchButton.Enabled = (isInstallDirectory && isAcumaticaWizard);
        }

        private void FilterPackage()
        {
            try
            {
                packageVersionFilter = PackageVersionMaskedTextBox.Text;
                PackageFilteredLabel.Visible = (packageVersionFilter != null && packageVersionFilter != Constants.emptyVersionMask);

                // Save selection
                BuildPackage selectedBuildPackage = GetSelectedPackage();

                if (selectedBuildPackage != null)
                {
                    selectedRow = selectedBuildPackage.Key;
                }

                // Freeze binding
                PackageBindingSource.CurrencyManager.SuspendBinding();

                // Filter rows
                for (int i = 0; i < PackageDataGridView.Rows.Count; i++)
                {
                    DataGridViewRow row = PackageDataGridView.Rows[i];

                    if (row != null)
                    {
                        BuildPackage buildPackage = row.DataBoundItem as BuildPackage;

                        row.Visible = (buildPackage != null &&
                                       CompareVersionMaskBuildPackage(buildPackage, PackageVersionMaskedTextBox.Text) &&
                                       ((buildPackage.IsLocal && ShowLocalCheckBox.Checked) ||
                                       (buildPackage.IsRemote && ShowRemoteCheckBox.Checked) ||
                                       (buildPackage.IsInstalled && ShowInstalledCheckBox.Checked)));
                    }
                }

                // Resume binding
                PackageBindingSource.CurrencyManager.ResumeBinding();

                // Restore selection
                PackageDataGridView.ClearSelection();

                if (selectedRow != null)
                {
                    for (int i = 0; i < PackageDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = PackageDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            BuildPackage buildPackage = row.DataBoundItem as BuildPackage;

                            if (buildPackage.Key == selectedRow)
                            {
                                PackageDataGridView.Rows[i].Selected = true;
                                PackageDataGridView.FirstDisplayedScrollingRowIndex = i;
                                EnablePackageControls(buildPackage);
                                break;
                            }
                        }
                    }
                }

                // Select first displayed row
                if (PackageDataGridView.SelectedRows.Count == 0 &&
                    PackageDataGridView.DisplayedRowCount(true) > 0)
                {
                    for (int i = 0; i < PackageDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = PackageDataGridView.Rows[i];

                        if (row != null && row.Visible)
                        {
                            BuildPackage buildPackage = row.DataBoundItem as BuildPackage;
                            PackageDataGridView.Rows[i].Selected = true;
                            PackageDataGridView.FirstDisplayedScrollingRowIndex = i;
                            selectedRow = selectedBuildPackage.Key;
                            EnablePackageControls(buildPackage);
                            break;
                        }
                    }
                }

                // No selection
                if (PackageDataGridView.SelectedRows.Count == 0)
                {
                    EnablePackageControls(null);
                }
            }
            catch (Exception ex)
            {
                PXWait.StopWait();
                SysData.ShowException(ex.ToString(), ErrorLevel.Error);
            }
        }

        private BuildPackage GetSelectedPackage()
        {
            return PackageDataGridView.SelectedRows.Count > 0 &&
                   PackageDataGridView.SelectedRows[0].Visible ? PackageDataGridView.SelectedRows[0].DataBoundItem as BuildPackage : null;
        }

        private void ReloadPackage()
        {
            new Thread(new ThreadStart(delegate
            {
                Invoke((MethodInvoker)delegate
                {
                    PXWait.StartWait(this);
                    List<BuildPackage> buildPackages = new List<BuildPackage>();

                    try
                    {
                        PXWait.ShowProgress(-1, Messages.connectingBuildServerProgress);

                        BuildManager.GetBuildPackages(delegate (int percentDone, long counter, long total)
                        {
                            PXWait.ShowProgress(-1, string.Format(Messages.loadingBuildPackageProgress, Convert.ToString(counter, CultureInfo.InvariantCulture)));
                        }).ToList().ForEach(buildPackage => buildPackages.Add(buildPackage));

                        PackageBindingSource.CurrencyManager.SuspendBinding();
                        PackageBindingSource.Clear();

                        foreach (BuildPackage buildPackage in buildPackages)
                            PackageBindingSource.Add(buildPackage);

                        PackageBindingSource.CurrencyManager.ResumeBinding();

                        FilterPackage();
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
