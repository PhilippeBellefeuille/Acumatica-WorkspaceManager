using Acumatica.WorkspaceManager.Builds;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
