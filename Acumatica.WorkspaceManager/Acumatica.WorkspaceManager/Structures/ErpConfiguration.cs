using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using ConfigCore;

namespace ERPConfig
{
    public class ERPStepsCounts : BaseStepsCounts
    {
        public override Int32 NotSet { get { return 0; } }
        public override Int32 NewInstance { get { return 8; } }
        public override Int32 DBMaint { get { return 5; } }
        public override Int32 DBConection { get { return 6; } }
        public override Int32 CompanyConfig { get { return 3; } }
        public override Int32 ToolsInstall { get { return 1; } }
        public override Int32 NewDBLessInstance { get { return 3; } }
        public override Int32 NewAzureInstance { get { return 6; } }
        public override Int32 SiteMaint { get { return 1; } }
    }

    public class ERPConfigContext : ConfigContext
    {
        #region Settings
        public override BaseStepsCounts CurrentStepsCounts
        {
            get
            {
                return new ERPStepsCounts();
            }
        }
        public override Assembly CurrentAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly();
            }
        }
        public override String[] MainProjectFiles
        {
            get
            {
                return new[] { "Files\\Web.config", "Files\\main.aspx" };
            }
        }

        public override BaseMain CreateMainForm()
        {
            return null;// new MainERP();
        }
        #endregion

        #region Properties
        public override string ProductDBName
        {
            get
            {
                return Titles.ERPDBName;
            }
        }
        public override string ProductName
        {
            get
            {
                return Titles.ERPProductName;
            }
        }
        public override string MainSitePath
        {
            get
            {
                return SysData.AppPath + "Files\\";
            }
        }
        public override string PortalSitePath
        {
            get
            {
                return SysData.AppPath + "Portal\\";
            }
        }
        public override string DatabaseFilePath
        {
            get
            {
                return SysData.AppPath + "Database\\";
            }
        }
        public override string DataFilesPath
        {
            get
            {
                return SysData.AppPath + "Database\\Data\\";
            }
        }
        public override string DBType
        {
            get
            {
                return "Application";
            }
        }
        public override string ConfigFile
        {
            get { return "web.config"; }
        }
        #endregion

        protected ERPConfigContext()
            : base(new InstallerType(InstallerTypes.ERP))
        { }
    }
}
