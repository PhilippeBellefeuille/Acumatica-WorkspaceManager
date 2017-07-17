using Acumatica.WorkspaceManager.Builds;
using Acumatica.WorkspaceManager.Install;
using ConfigCore;
using ConfigCore.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main : Form
    {
        private string versionMask;

        public Main()
        {
            InitializeComponent();
        }

        #region Event Handlers
        private void AcumaticaLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(AcumaticaLinkLabel.Text);
            }
            catch (Exception ex)
            {
                SysData.ShowException(ex.Message, ErrorLevel.Error);
            }
        }

        private void BuildPackageBindingSource_CurrentChanged(object sender, EventArgs e)
        {
            BuildPackage buildPackage = BuildPackageBindingSource.Current as BuildPackage;

            if (buildPackage != null)
            {
                EnableControls(buildPackage);
            }
        }

        private void Control_Click(object sender, EventArgs e)
        {
            BuildPackage buildPackage = BuildPackageBindingSource.Current as BuildPackage;

            if (buildPackage != null)
            {
                try
                {
                    new Thread(new ThreadStart(delegate
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            PXWait.StartWait(this);
                            ExecuteCommand(sender, buildPackage);
                            PXWait.StopWait();

                            //BuildPackageDataGridView.Update();
                            EnableControls(buildPackage);
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
        
        private void Main_Load(object sender, EventArgs e)
        {
            ReloadBuildPackage();
        }

        private void ReloadDataEventHandler(object sender, EventArgs e)
        {
            if (sender == ReloadButton)
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
        #endregion

        #region Commands
        private void ExecuteCommand(object sender, BuildPackage buildPackage)
        {
            if (sender == DownloadButton)
            {
                DownloadBuildPackage(buildPackage, delegate (int percentDone, long counter, long total)
                {
                    const long oneMegabyte = 1048576;
                    long transferedMegaBytes = counter / oneMegabyte;
                    long totalMegaBytes = total / oneMegabyte;
                    int transferedDigitCount = (int)Math.Log10(transferedMegaBytes) + 1;
                    int totalDigitCount = (int)Math.Log10(totalMegaBytes) + 1;

                    PXWait.ShowProgress(percentDone, 
                                        string.Concat(Convert.ToString(transferedMegaBytes, CultureInfo.InvariantCulture).PadLeft(totalDigitCount),
                                                      " MB / ",
                                                      Convert.ToString(totalMegaBytes, CultureInfo.InvariantCulture),
                                                      " MB downloaded",
                                                      string.Empty.PadRight(totalDigitCount - transferedDigitCount)));
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
                return;
            }

            throw new Exception("Failed to download remote file to local directory.");
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

        #region Private Methods
        private bool CompareVersionMask(BuildPackage buildPackage, string filterVersion)
        {
            const int majorVersionLength = 1;
            const int minorVersionLength = 2;
            const int buildNumberLength = 4;
            const char versionPromptChar = '_';
            const char versionSeparatorChar = '.';

            string version = string.Concat(Convert.ToString(buildPackage.MajorVersion, CultureInfo.InvariantCulture).PadLeft(majorVersionLength, versionPromptChar),
                                           versionSeparatorChar,
                                           Convert.ToString(buildPackage.MinorVersion, CultureInfo.InvariantCulture).PadLeft(minorVersionLength, versionPromptChar),
                                           versionSeparatorChar,
                                           Convert.ToString(buildPackage.BuildNumber, CultureInfo.InvariantCulture).PadLeft(buildNumberLength, versionPromptChar));

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

        private void EnableControls(BuildPackage buildPackage)
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
        }

        private void ReloadBuildPackage()
        {
            new Thread(new ThreadStart(delegate
            {
                Invoke((MethodInvoker)delegate
                {
                    PXWait.StartWait(this);

                    try
                    {
                        BuildPackageBindingSource.Clear();

                        foreach (BuildPackage buildPackage in BuildManager.GetBuildPackages(delegate (int percentDone, long counter, long total)
                                                              {
                                                                   PXWait.ShowProgress(-1, string.Concat("Loading build package: ", counter));
                                                              }))
                        {
                            BuildPackageBindingSource.Add(buildPackage);
                        }
                    }
                    catch (Exception ex)
                    {
                        SysData.ShowException(ex.Message, ErrorLevel.Error);
                    }

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
                FilteredLabel.Visible = (versionMask != emptyVersionMask);
                
                BuildPackageBindingSource.CurrencyManager.SuspendBinding();

                for (int i = 0; i < BuildPackageDataGridView.Rows.Count; i++)
                {
                    DataGridViewRow row = BuildPackageDataGridView.Rows[i];

                    if (row != null)
                    {
                        BuildPackage buildPackage = row.DataBoundItem as BuildPackage;
                        row.Visible = (buildPackage != null &&
                                        CompareVersionMask(buildPackage, VersionMaskedTextBox.Text) &&
                                        ((buildPackage.IsLocal && ShowLocalCheckBox.Checked) ||
                                        (buildPackage.IsRemote && ShowRemoteCheckBox.Checked) ||
                                        (buildPackage.IsInstalled && ShowInstalledCheckBox.Checked)));
                    }
                }

                BuildPackageBindingSource.CurrencyManager.ResumeBinding();

                if (BuildPackageDataGridView.SelectedRows.Count == 0 ||
                    (BuildPackageDataGridView.SelectedRows[0].DataBoundItem != BuildPackageBindingSource.Current ||
                     BuildPackageDataGridView.SelectedRows[0].Visible == false))
                {
                    BuildPackageDataGridView.ClearSelection();
                    EnableControls(null);
                }
            }
            catch (Exception ex)
            {
                SysData.ShowException(ex.Message, ErrorLevel.Error);
            }
        }
        #endregion
    }
}
