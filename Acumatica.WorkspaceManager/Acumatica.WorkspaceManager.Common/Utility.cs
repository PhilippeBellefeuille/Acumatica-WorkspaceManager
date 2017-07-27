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
    }
}
