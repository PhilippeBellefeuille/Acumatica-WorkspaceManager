using Acumatica.WorkspaceManager.Builds;
using Acumatica.WorkspaceManager.Install;
using ConfigCore;
using ConfigCore.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Acumatica.WorkspaceManager
{
    public partial class Main : Form
    {
        private IEnumerable<BuildPackage> buildPackages;
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

                            BuildPackageDataGridView.Update();
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
            ReloadBuildPackage(true);
        }

        private void ReloadDataEventHandler(object sender, EventArgs e)
        {
            ReloadBuildPackage(sender != VersionMaskedTextBox);
        }
        #endregion

        #region Commands
        private void ExecuteCommand(object sender, BuildPackage buildPackage)
        {
            if (sender == DownloadButton)
                DownloadBuildPackage(buildPackage);
            else if (sender == InstallButton)
                InstallBuildPackage(buildPackage);
            else if (sender == LaunchButton)
                LaunchAcumaticaWizard(buildPackage);
            else if (sender == OpenFolderButton)
                OpenBuildPackageFolder(buildPackage);
            else if (sender == UninstallButton)
                UninstallBuildPackage(buildPackage);
        }

        private void DownloadBuildPackage(BuildPackage buildPackage)
        {
            BuildManager.DownloadPackage(buildPackage);

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
            });
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
            string filePath = BuildManager.GetPathFromKey(buildPackage.Key);
            string directory = Path.GetDirectoryName(filePath);
            string wizardPath = Path.Combine(directory, "Data", "AcumaticaConfig.exe");

            bool isLocal = buildPackage.IsLocal;
            bool isRemote = buildPackage.IsRemote;
            bool isInstallDirectory = (directory != null && Directory.Exists(directory));
            bool isInstallFile = (filePath != null && File.Exists(filePath));
            bool isAcumaticaWizard = (wizardPath != null && File.Exists(wizardPath));

            DownloadButton.Enabled = !isLocal;
            InstallButton.Enabled = isLocal && isInstallFile;
            UninstallButton.Enabled = isLocal && isInstallDirectory;
            OpenFolderButton.Enabled = isLocal && isInstallDirectory;
            LaunchButton.Enabled = isLocal && isInstallDirectory && isAcumaticaWizard;
        }

        private void ReloadBuildPackage(bool forceReload)
        {
            if (forceReload || versionMask != VersionMaskedTextBox.Text)
            {
                versionMask = VersionMaskedTextBox.Text;

                new Thread(new ThreadStart(delegate
                {
                    Invoke((MethodInvoker)delegate
                    {
                        PXWait.StartWait(this);

                        try
                        {
                            BuildPackageBindingSource.Clear();

                            if (forceReload || buildPackages == null)
                            {
                                buildPackages = BuildManager.GetBuildPackages();
                            }

                            foreach (BuildPackage buildPackage in buildPackages)
                            {
                                if (CompareVersionMask(buildPackage, VersionMaskedTextBox.Text) &&
                                    ((buildPackage.IsLocal && ShowLocalCheckBox.Checked) ||
                                     (buildPackage.IsRemote && ShowRemoteCheckBox.Checked)))
                                {
                                    BuildPackageBindingSource.Add(buildPackage);
                                }
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
        }
        #endregion
    }
}
