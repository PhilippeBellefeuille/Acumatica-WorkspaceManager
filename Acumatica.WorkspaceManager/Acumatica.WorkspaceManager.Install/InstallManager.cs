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

            return (string)acumaticaRegistryKey["DisplayVersion"];
        }

        public delegate void ProcessExitedDelegate();

        public static void InstallAcumatica(BuildPackage buildPackage, EventHandler callback)
        {
            string installerPath = BuildManager.GetPathFromKey(buildPackage.Key);          
            string installerDirectory = Path.Combine(Path.GetDirectoryName(installerPath), "Files");
            string installerTempPath = Path.Combine(Path.GetTempPath(), Path.GetFileName(installerPath));
            File.Copy(installerPath, installerTempPath, true);

            Process process = new Process()
            {
                EnableRaisingEvents = true,
                StartInfo = new ProcessStartInfo("msiexec.exe", string.Format("/a \"{0}\" /qb targetdir=\"{1}\"", installerTempPath, installerDirectory))
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
            string path = BuildManager.GetPathFromKey(buildPackage.Key);
            string directory = Path.GetDirectoryName(path);
            string wizardPath = Path.Combine(directory, "Files", "Data", "AcumaticaConfig.exe");

            Process.Start(new ProcessStartInfo(wizardPath));
        }

        public static void OpenPackageFolder(BuildPackage buildPackage)
        {
            string directory = Path.GetDirectoryName(BuildManager.GetPathFromKey(buildPackage.Key));

            if (Directory.Exists(directory))
            {
                Process.Start(directory);
                return;
            }

            throw new Exception(string.Concat("Install directory not found: ", Environment.NewLine, directory));
        }

        public static void UninstallPackage(BuildPackage buildPackage)
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

            if (File.Exists(BuildManager.GetPathFromKey(buildPackage.Key)))
            {
                buildPackage.SetIsLocal(true);
            }
            else
            {
                throw new Exception("Failed to download remote file to local directory.");
            }
        }

        public static void InstallPackage(BuildPackage buildPackage)
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

        public static void RemovePackage(BuildPackage buildPackage)
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

        private static Process GetInstallAcumaticaProcess(string path, ProcessExitedDelegate del)
        {
            Process p = new Process();
            p.StartInfo.FileName = "msiexec.exe";
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

            var uninstalledCommand = (string)acumaticaRegistryKey["UninstallString"];
            var acumaticaPackageGuid = uninstalledCommand.Replace("MsiExec.exe /I", string.Empty);
            Process p = new Process();
            p.StartInfo.FileName = "msiexec.exe";
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
            string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
            using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
            {
                foreach (string subkey_name in key.GetSubKeyNames())
                {
                    using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                    {
                        var displayName = subkey.GetValue("DisplayName");
                        if (displayName != null && (string)displayName == "Acumatica ERP")
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
