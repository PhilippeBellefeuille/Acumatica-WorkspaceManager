using Acumatica.WorkspaceManager.Builds;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static void InstallAcumatica(BuildPackage buildPackage, ProcessExitedDelegate del = null)
        {
            var path = BuildManager.GetPathFromKey(buildPackage.Key);

            var uninstallProcess = GetUninstallAcumaticaProcess(() =>
                                        {
                                            GetInstallAcumaticaProcess(path, del);
                                        });

            
            if (uninstallProcess == null)
            {
                GetInstallAcumaticaProcess(path, del);
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
