using System;
using System.IO;

namespace Acumatica.WorkspaceManager.Common
{
    public static class Utility
    {
        public static string GetTempDirectory()
        {
            string tempBaseDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            while (File.Exists(tempBaseDirectory) || Directory.Exists(tempBaseDirectory))
            {
                tempBaseDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            }

            return tempBaseDirectory;
        }

        public static void GetFileWizardPath(BuildPackage buildPackage, out string filePath, out string wizardPath)
        {
             filePath = GetPathFromKey(buildPackage.Key);
            string directory = Path.GetDirectoryName(filePath);

            string installDirectory = Path.Combine(directory, Constants.filesDirectory);

            //Data folder was moved with 2018R1
            if (buildPackage.MajorVersion >= 18)
                installDirectory = Path.Combine(installDirectory, Constants.installDirectory2018);

            wizardPath = Path.Combine(installDirectory, Constants.dataDirectory, Constants.wizardFilename);
        }

        public static string GetKeyFromPath(string filePath)
        {
            return filePath.Replace(GetLocalRepositoryFolder() + Path.DirectorySeparatorChar, string.Empty).Replace(Path.DirectorySeparatorChar, '/');
        }

        public static string GetPathFromKey(string key)
        {
            return Path.Combine(GetLocalRepositoryFolder(), key.Replace('/', Path.DirectorySeparatorChar));
        }

        public static string GetLocalRepositoryFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.localRepositoryName/*Resources.LocalRepositoryName*/);
        }
    }
}
