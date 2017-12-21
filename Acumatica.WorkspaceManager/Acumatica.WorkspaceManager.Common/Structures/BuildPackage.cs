using System;
using System.Globalization;

namespace Acumatica.WorkspaceManager.Common
{
    public class BuildPackage
    {
        public static bool TryCreate(string key, out BuildPackage erpPackage)
        {
            int majorVersion = 0;
            int minorVersion = 0;
            int buildNumber = 0;
            
            var keyParts = key.Split('/');
            if (keyParts.Length > 2)
            {
                var buildRaw = keyParts[2];
                var buildParts = buildRaw.Split('.');

                if (buildParts.Length == 3
                    && int.TryParse(buildParts[0], out majorVersion)
                    && int.TryParse(buildParts[1], out minorVersion)
                    && int.TryParse(buildParts[2], out buildNumber))
                {
                    erpPackage = new BuildPackage(majorVersion, minorVersion, buildNumber, key);
                    return true;
                }


            }

            erpPackage = null;
            return false;
        }

        private BuildPackage(int majorVersion, int minorVersion, int buildNumber, string key)
        {
            this.MajorVersion = majorVersion;
            this.MinorVersion = minorVersion;
            this.BuildNumber = buildNumber;
            this.Key = key;
            this.VersionString = FormatVersion('0');

            this.IsRemote = false;
            this.IsLocal = false;
        }

        public int MajorVersion { get; }
        public int MinorVersion { get; }
        public int BuildNumber { get; }
        public bool IsRemote { get; private set; }
        public bool IsInstalled { get; private set; }
        public bool IsPreview { get; private set; }
        public bool IsLocal { get; private set; }

        public string Key { get; }
        public string VersionString { get; }

        public string FormatVersion(char paddingChar)
        {
            const int majorVersionLength = 2;
            const int minorVersionLength = 3;
            const int buildNumberLength = 4;

            string version = string.Concat(Convert.ToString(MajorVersion, CultureInfo.InvariantCulture).PadLeft(majorVersionLength, paddingChar),
                                           Constants.versionSeparatorChar,
                                           Convert.ToString(MinorVersion, CultureInfo.InvariantCulture).PadLeft(minorVersionLength, paddingChar),
                                           Constants.versionSeparatorChar,
                                           Convert.ToString(BuildNumber, CultureInfo.InvariantCulture).PadLeft(buildNumberLength, paddingChar));
            return version;
        }

        public void SetIsRemote()
        {
            this.IsRemote = true;
        }

        public void SetIsInstalled(bool isInstalled)
        {
            this.IsInstalled = isInstalled;
        }

        public void SetIsLocal(bool isLocal)
        {
            this.IsLocal = isLocal;
        }

        public void SetIsPreview(bool isPreview)
        {
            this.IsPreview = isPreview;
        }
    }
}
