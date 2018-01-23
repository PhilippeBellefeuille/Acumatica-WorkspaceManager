using Acumatica.WorkspaceManager.Builds;
using Acumatica.WorkspaceManager.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Acumatica.WorkspaceManager.Install
{
    public static class InstallManager
    {
        public static string GetInstalledVersion()
        {
            var acumaticaRegistryKey = GetAcumaticaRegistryKeyValues();
            if (acumaticaRegistryKey == null)
                return null;

            return (string)acumaticaRegistryKey[Constants.displayVersionRegistryValue];
        }

        public delegate void ProcessExitedDelegate();

        public static void InstallAcumatica(BuildPackage buildPackage, EventHandler callback)
        {
            string installerPath = Utility.GetPathFromKey(buildPackage.Key);          
            string installerDirectory = Path.Combine(Path.GetDirectoryName(installerPath), Constants.filesDirectory);
            string installerTempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(installerPath));
            File.Copy(installerPath, installerTempPath, true);

            Process process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo(Constants.msiFilename, string.Format("/a \"{0}\" /qb targetdir=\"{1}\"", installerTempPath, installerDirectory))
            };
            
            process.Exited += delegate
            {
                File.Delete(installerTempPath);
            };

            process.Exited += callback;
            process.Start();
            process.WaitForExit();
        }
        
        public static void LaunchAcumaticaWizard(BuildPackage buildPackage)
        {
            string filePath;
            string wizardPath;

            Utility.GetFileWizardPath(buildPackage, out filePath, out wizardPath);

            Process.Start(new ProcessStartInfo(wizardPath));
        }

        public static void OpenPackageFolder(BuildPackage buildPackage)
        {
            string directory = Path.GetDirectoryName(Utility.GetPathFromKey(buildPackage.Key));

            if (Directory.Exists(directory))
            {
                Process.Start(directory);
                return;
            }

            throw new Exception(string.Format(Messages.missingInstallDirectoryError, Environment.NewLine, directory));
        }

        public static void UninstallPackage(BuildPackage buildPackage)
        {
            string directory = Path.GetDirectoryName(Utility.GetPathFromKey(buildPackage.Key));

            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
                buildPackage.SetIsLocal(false);
                buildPackage.SetIsInstalled(false);
                return;
            }

            throw new Exception(string.Format(Messages.missingInstallDirectoryError, Environment.NewLine, directory));
        }

        public static void DownloadPackage(BuildPackage buildPackage)
        {
            BuildManager.DownloadPackage(buildPackage, delegate (int percentDone, long counter, long total)
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
            });

            if (File.Exists(Utility.GetPathFromKey(buildPackage.Key)))
            {
                buildPackage.SetIsLocal(true);
            }
            else
            {
                throw new Exception(Messages.fileTransferError);
            }
        }

        public static void InstallPackage(BuildPackage buildPackage)
        {
            InstallManager.InstallAcumatica(buildPackage, (object sender, EventArgs e) =>
            {
                string filePath;
                string wizardPath;

                Utility.GetFileWizardPath(buildPackage, out filePath, out wizardPath);

                if (File.Exists(wizardPath))
                {
                    buildPackage.SetIsLocal(true);
                    buildPackage.SetIsInstalled(true);
                }
                else
                {
                    throw new Exception(Messages.installError);
                }
            });
        }

        public static void RemovePackage(BuildPackage buildPackage)
        {
            string filePath = Utility.GetPathFromKey(buildPackage.Key);

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
                throw new Exception(Messages.removeFileError);
            }
        }

        private static Process GetInstallAcumaticaProcess(string path, ProcessExitedDelegate del)
        {
            Process p = new Process();
            p.StartInfo.FileName = Constants.msiFilename;
            p.StartInfo.Arguments = string.Format("/i {0} /qn", path);

            p.Exited += (object sender, EventArgs e) =>
            {
                del();
            };

            p.Start();

            return p;
        }

        private static Process GetUninstallAcumaticaProcess(ProcessExitedDelegate del)
        {
            var acumaticaRegistryKey = GetAcumaticaRegistryKeyValues();
            if (acumaticaRegistryKey == null)
                return null;

            var uninstalledCommand = (string)acumaticaRegistryKey[Constants.uninstallRegistryValue];
            var acumaticaPackageGuid = uninstalledCommand.Replace("MsiExec.exe /I", string.Empty);
            Process p = new Process();
            p.StartInfo.FileName = Constants.msiFilename;
            p.StartInfo.Arguments = string.Format("/x {0} /qn", acumaticaPackageGuid);
            //p.StartInfo.Arguments = "/x \"C:\\MyApplication.msi\"/qn";
            p.Exited += (object sender, EventArgs e) =>
            {
                del();
            };
            p.Start();

            return p;
        }
        
        private static Dictionary<string, object> GetAcumaticaRegistryKeyValues()
        {
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(Constants.uninstallRegistryKey))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var displayName = subkey.GetValue(Constants.displayNameRegistryValue);
                        if (displayName != null && (string)displayName == Constants.productNameRegistryValue)
                        {
                            var registryKeyValues = new Dictionary<string, object>();

                            foreach (var valueName in subkey.GetValueNames())
                            {
                                registryKeyValues.Add(valueName, subkey.GetValue(valueName));
                            }

                            return registryKeyValues;
                        }
                    }
                }
            }

            return null;
        }
    }
}
