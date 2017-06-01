using Acumatica.WorkspaceManager.Install;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acumatica.WorkspaceManager.Tests
{
    [TestClass]
    public class InstallTest
    {
        [TestMethod]
        public void HasInstalledVersion()
        {
            var version = InstallManager.GetInstalledVersion();

            Assert.IsNotNull(version);
        }

        [TestMethod]
        public void UninstallAcumatica()
        {
            //InstallManager.UninstallAcumatica();
        }
    }
}
