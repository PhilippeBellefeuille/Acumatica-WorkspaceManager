using System;
using PX.BulkInsert;
using PX.BulkInsert.Installer;
using PX.WebConfig;
using PX.Config.Common;
using PX.DbServices.Points;
using System.Linq;
using System.Collections.Generic;
using ConfigCore;
using PX.DbServices.Model;

namespace Acumatica.WorkspaceManager
{
	public class DBInfo
	{
		public String Name { get; protected set; }
		public DatabaseActions Act { get; protected set; }
		protected DBVersionCollection Vers { get; set; }
		public String Message { get; protected set; }
		public Boolean ValidDB 
		{
			get 
			{
				if (Act == DatabaseActions.CantOpen || Act == DatabaseActions.NotSet || Act == DatabaseActions.ConflictingObjects) return false;
				//if (Act == DatabaseActions.UserDefined && (!SysData.Context.CurrentType.IsStudio)) return false;
				//if (ConfigCore.SysData.Context.CurrentType.IsStudio && 
                //    Vers.GetDBType() == DatabaseTypes.Application) return false;

				return true;
			}
		}

		protected DBInfo(String name)
		{
			Name = name;
			Act = DatabaseActions.NotSet;
			Vers = new DBVersionCollection(Name);			
		}
		public static DBInfo DatabaseTest()
		{
			return new DBInfo(Titles.MasterDB);
		}
		public static DBInfo  DatabaseTest(String database)
		{
			return DatabaseTest(DatabaseProvider.GetDefinition(ConfigCore.SysData.DBName.Value));
		}
		internal static DBInfo DatabaseTest(ConnectionDefinition connection)
		{
			DBInfo di = new DBInfo(connection.Database);

			#region Try Connect
			SqlEventType type = SqlEventType.Default;
			try
			{
                DatabaseProvider.GetProvider(connection, new SimpleExecutionObserver()).TryConnect();
			}
			catch (Exception ex)
			{
				Logger.WriteException(ex.Message);
				di.Act = DatabaseActions.CantOpen;
				di.Message = Errors.ServConnectFailed;
				if (type == SqlEventType.LoginFailed)
					di.Message = Errors.ServLoginFailed.Format(connection.Database);
				return di;
			}
			#endregion

			#region Get Versions
			try
			{
				BaseInstallProvider prov = DatabaseProvider.GetProvider(connection);
				foreach (var version in prov.GetVersions())
				{
					di.Vers.AddVersion(version);
				}
				List<PX.DbServices.Points.DbmsBase.ObjectInDifferentSchema> list = prov.Point.getSchemaBoundObjects().ToList();
				if(list.Count > 0)
				{
					di.Act = DatabaseActions.ConflictingObjects;
					di.Message = Errors.ServDataboundObjects.Format(connection.Database, string.Join(", ", list));
					return di;
				}
			}
			catch (Exception ex)
			{
				Logger.WriteException(ex.Message);
				di.Act = DatabaseActions.UserDefined;
				di.Message = Errors.DBVersUser.Format(di.Name);
				return di;
			}
			#endregion
            /*
			var versionCheck = di.Vers.TestVersion();
			if( versionCheck != DBVersionCollection.VersionCheckResult.Passed ) {
				String errorMessage = di.Vers.getErrorMessage(versionCheck);
				Logger.WriteException(errorMessage);

				di.Act = DatabaseActions.UserDefined;
				di.Message = errorMessage;
				return di;
			}


			#region Check Versions
			Int32 res = PXVersion.Compare(di.GetVersion(), PXVersion.CurVersion);
			if (res > 0)
			{
				di.Act = DatabaseActions.LaterVersion;
				di.Message = Errors.DBVersLater.Format(di.Name);
			}
			if (res == 0)
			{
				di.Act = DatabaseActions.CurrentVersion;
				di.Message = Errors.DBVersCurrent.Format(di.Name);
			}
			if (res < 0)
			{
				di.Act = DatabaseActions.EarlierVersion;
				di.Message = Errors.DBVersEarlier.Format(di.Name);
			}
			if (PXVersion.Compare(di.GetVersion(), PXVersionHelper.MinimumVersion) < 0)
			{
				di.Act = DatabaseActions.MinimumVersion;
				di.Message = Errors.DBVersMinimum.Format(di.Name, PXVersionHelper.MinimumVersion);
			}
			#endregion
            */
			//di.Act = DatabaseActions.Valid;
			return di;
		}

		public String GetVersion()
		{
			return Vers.GetVersion();
		}
	}
}
