using System;
using Acumatica.WorkspaceManager.Builds;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Acumatica.WorkspaceManager.Tests
{
    [TestClass]
    public class BuildsTest
    {
        [TestMethod]
        public void Download61Packages()
        {
            foreach (var buildPackage in BuildManager.GetBuildPackages())
            {
                if (buildPackage.MajorVersion == 6 && buildPackage.MinorVersion == 10)
                    BuildManager.DownloadPackage(buildPackage);
            }
        }

        [TestMethod]
        public void Local61PackagesAreTheSameAsRemote()
        {
            foreach (var buildPackage in BuildManager.GetBuildPackages())
            {
                var v = buildPackage.Key;
                //if (buildPackage.MajorVersion == 6 && buildPackage.MinorVersion == 10)
                    //RemoteBuildManager.DownloadPackage(buildPackage);
            }
        }
    }
}
