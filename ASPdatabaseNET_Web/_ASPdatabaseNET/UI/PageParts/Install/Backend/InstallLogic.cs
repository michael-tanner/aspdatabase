using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using ASPdb.UniversalADO;
using ASPdatabaseNET.UI.PageParts.Install.Objs;

namespace ASPdatabaseNET.UI.PageParts.Install.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class InstallLogic
    {
        private static string AppKey_IsInstalled = "ASPdatabaseNET.UI.PageParts.Install.Backend.IsInstalled";

        //----------------------------------------------------------------------------------------------------
        public static bool IsInInstallState()
        {
            ASPdb.Framework.Debug.WriteLine("001");
            ASPdatabaseNET.AjaxService.ASPdatabaseService.SetSetVal();
            var cache = Memory.AppCache.Get();
            if (cache.AnyData.ContainsKey(AppKey_IsInstalled))
                if (cache.AnyData[AppKey_IsInstalled].ToString() == "true")
                {
                    CheckForSQLStructureUpdates();
                    return false;
                }
            ASPdb.Framework.Debug.WriteLine("002");
            var installInfo = GetInstallState();
            if(installInfo.InstallState == InstallInfo.InstallStates.Installed)
            {
                if (cache.AnyData.ContainsKey(AppKey_IsInstalled))
                    cache.AnyData[AppKey_IsInstalled] = "true";
                else
                    cache.AnyData.Add(AppKey_IsInstalled, "true");

                CheckForSQLStructureUpdates();
                return false;
            }
            ASPdb.Framework.Debug.WriteLine("003");
            return true;
        }

        //----------------------------------------------------------------------------------------------------
        public static InstallInfo GetInstallState()
        {
            var rtn = new InstallInfo() { InstallState = InstallInfo.InstallStates.NotSet };
            rtn.AboutPageInfo = UI.PageParts.About.Backend.AboutPageLogic.Get();

            string exc_Message = "";

            try { rtn.ConnectionString = ConfigurationManager.ConnectionStrings["ASPdb_AppData"].ConnectionString; }
            catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); }
            if (String.IsNullOrEmpty(rtn.ConnectionString))
            {
                rtn.InstallState = InstallInfo.InstallStates.NoConnectionString;
            }
            else
            {
                try
                {
                    rtn.InstallState = InstallInfo.InstallStates.Installed;
                    var tableNames = new string[] 
                    { 
                        "ASPdb_Connections",
                        "ASPdb_Permissions",
                        "ASPdb_Tables",
                        "ASPdb_UserGroups",
                        "ASPdb_Users",
                        "ASPdb_UsersToGroups",
                        "ASPdb_Values"
                    };
                    string sql = "";
                    foreach (string table in tableNames)
                    {
                        sql = String.Format("select top 1 * from [{0}].[{1}];", Config.SystemProperties.AppSchema, table);
                        using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(rtn.ConnectionString, sql))
                        {
                            using (DbReaderWrapper reader = command.ExecuteReaderWrapper()) { if (reader.Read()) { } }
                        }
                    }
                    int usersCount = 0;
                    sql = String.Format("select count(*) as [Count1] from [{0}].[ASPdb_Users] where Len([Password]) > 0", Config.SystemProperties.AppSchema);
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(rtn.ConnectionString, sql))
                    {
                        using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        {
                            if (reader.Read()) { usersCount = reader.Get("Count1", 0); }
                        }
                    }
                    if (usersCount < 1)
                        throw new Exception("Users table is empty.");
                }
                catch (Exception exc)
                {
                    ASPdb.Framework.Debug.RecordException(exc);
                    string exc_L = exc.Message.ToLower();
                    if (exc_L.Contains("cannot open database") || exc_L.Contains("login failed"))
                        rtn.InstallState = InstallInfo.InstallStates.CannotConnectToDB;
                    else
                        rtn.InstallState = InstallInfo.InstallStates.DatabaseNotReady;

                    exc_Message = exc.Message; // +"<br />" + exc.StackTrace;
                    //throw exc;
                }
            }

            var cache = Memory.AppCache.Get();
            if (cache.AnyData.ContainsKey(AppKey_IsInstalled))
                cache.AnyData.Remove(AppKey_IsInstalled);
            if (rtn.InstallState == InstallInfo.InstallStates.Installed)
                cache.AnyData.Add(AppKey_IsInstalled, "true");

            rtn.ResponseMsg = rtn.InstallState.ToString();
            switch(rtn.InstallState)
            {
                case InstallInfo.InstallStates.NoConnectionString:
                    rtn.ResponseMsg = "Connection string not found.";
                    break;
                case InstallInfo.InstallStates.CannotConnectToDB:
                    rtn.ResponseMsg = "Could not connect to database. <hr />" + exc_Message;
                    break;
                case InstallInfo.InstallStates.DatabaseNotReady:
                    break;
                case InstallInfo.InstallStates.Installed:
                    break;
            }

            ASPdb.Framework.Debug.WriteLine("002a..." + rtn.InstallState.ToString());
            
            return rtn;
        }



        //----------------------------------------------------------------------------------------------------
        public static InstallInfo InstallSQL(string adminUser, string adminPass, string firstName, string lastName, string email)
        {
            var rtn = GetInstallState();
            string appSchema = Config.SystemProperties.AppSchema;
            string sql = "";

            sql = SQLCreate__SCHEMA(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "SCHEMA", "1") + "<br />\n";

            sql = SQLCreate__ASPdb_Users(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_Users", "2") + "<br />\n";

            sql = SQLCreate__ASPdb_Connections(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_Connections", "3") + "<br />\n";

            sql = SQLCreate__ASPdb_Tables(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_Tables", "4") + "<br />\n";

            sql = SQLCreate__ASPdb_UserGroups(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_UserGroups", "5") + "<br />\n";

            sql = SQLCreate__ASPdb_UsersToGroups(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_UsersToGroups", "6") + "<br />\n";

            sql = SQLCreate__ASPdb_Values(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_Values", "7") + "<br />\n";

            sql = SQLCreate__ASPdb_Permissions(appSchema);
            rtn.ResponseMsg += ExeSQL(rtn.ConnectionString, sql, "ASPdb_Permissions", "8") + "<br />\n";

            try
            {
                var userInfo = new Users.Objs.UserInfo()
                {
                    UserId = -1,
                    Username = adminUser,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    IsAdmin = true,
                    Active = true
                };
                UI.PageParts.Users.Backend.UsersLogic.SaveUser(userInfo);
                UI.PageParts.Users.Backend.UsersLogic.SavePassword(userInfo.UserId, adminPass);
            }
            catch(Exception exc) { throw exc; }
            finally
            {
                sql = String.Format("delete from [{0}].[ASPdb_Users] where Len([Password]) < 3", Config.SystemProperties.AppSchema);
                ExeSQL(rtn.ConnectionString, sql, "", "");
            }


            try
            {
                string samplesConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["ASPdb_AppData"].ConnectionString;
                if(!String.IsNullOrEmpty(samplesConnStr))
                {
                    bool connectionAlreadyExist = false;
                    var connectionResponse = ASPdatabaseNET.DataAccess.DatabaseConnectionsCRUD.GetList();
                    foreach (var item in connectionResponse.ActiveConnections)
                        if (item.ConnectionName.ToLower() == "ASPdb_AppData".ToLower())
                            connectionAlreadyExist = true;
                    foreach (var item in connectionResponse.HiddenConnections)
                        if (item.ConnectionName.ToLower() == "ASPdb_AppData".ToLower())
                            connectionAlreadyExist = true;
                    
                    if (!connectionAlreadyExist)
                    {
                        var connectionInfo = new DataObjects.SQLObjects.ASPdb_Connection()
                        {
                            ConnectionId = -1,
                            Active = true,
                            ConnectionName = "ASPdb_AppData",
                            ConnectionType = "SQLServer",
                            ParametersType = "ConnectionString",
                            Param_ConnectionString = samplesConnStr
                        };
                        ASPdatabaseNET.DataAccess.DatabaseConnectionsCRUD.Save(connectionInfo);
                    }
                }
            }
            catch (Exception exc) 
            {
                throw new Exception(exc.Message + "<hr />" + exc.StackTrace);
            }

            rtn.InstallState = GetInstallState().InstallState;
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private static string ExeSQL(string connectionString, string sql, string successMsg, string failedMsg)
        {
            try
            {
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionString, sql))
                {
                    command.ExecuteNonQuery();
                }
                return successMsg;
            }
            catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); return failedMsg; }
        }


        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__SCHEMA(string appSchema)
        {
            string sql = String.Format(@"
            CREATE SCHEMA [{0}]
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_Users(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_Users](
	            [UserId] [int] IDENTITY(8000,1) NOT NULL,
	            [Username] [nvarchar](50) NOT NULL,
	            [Password] [nvarchar](255) NOT NULL,
	            [FirstName] [nvarchar](100) NOT NULL,
	            [LastName] [nvarchar](100) NOT NULL,
	            [Email] [nvarchar](255) NOT NULL,
	            [Active] [bit] NOT NULL,
	            [TimeCreated] [datetime] NOT NULL,
	            [LastLoginTime] [datetime] NULL,
	            [RequirePasswordReset] [bit] NOT NULL,
	            [IsAdmin] [bit] NOT NULL,
             CONSTRAINT [PK_ASPdb_Users] PRIMARY KEY CLUSTERED 
            (
	            [UserId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
            );  
            ALTER TABLE [{0}].[ASPdb_Users] ADD  DEFAULT (N'') FOR [FirstName];
            ALTER TABLE [{0}].[ASPdb_Users] ADD  DEFAULT (N'') FOR [LastName];
            ALTER TABLE [{0}].[ASPdb_Users] ADD  DEFAULT (N'') FOR [Email];
            ALTER TABLE [{0}].[ASPdb_Users] ADD  DEFAULT ((1)) FOR [Active];
            ALTER TABLE [{0}].[ASPdb_Users] ADD  DEFAULT ((0)) FOR [RequirePasswordReset];
            ALTER TABLE [{0}].[ASPdb_Users] ADD  DEFAULT ((0)) FOR [IsAdmin];

            CREATE UNIQUE NONCLUSTERED INDEX [IX_ASPdb_Users] ON [{0}].[ASPdb_Users]
            (
	            [Username] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_Connections(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_Connections](
	            [ConnectionId] [int] IDENTITY(7000,1) NOT NULL,
	            [SiteId] [int] NOT NULL,
	            [ConnectionName] [nvarchar](100) NOT NULL,
	            [ConnectionType] [nvarchar](50) NOT NULL,
	            [ParametersType] [nvarchar](50) NOT NULL,
	            [Active] [bit] NOT NULL,
	            [DateTimeCreated] [datetime] NULL,
	            [CreatedByUserId] [int] NULL,
	            [Param_ServerAddress] [nvarchar](255) NOT NULL,
	            [Param_DatabaseName] [nvarchar](255) NOT NULL,
	            [Param_U] [nvarchar](255) NOT NULL,
	            [Param_P] [ntext] NOT NULL,
	            [Param_ConnectionString] [ntext] NOT NULL,
             CONSTRAINT [PK_ASPdb_Databases] PRIMARY KEY CLUSTERED 
            (
	            [ConnectionId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
            );
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_SiteId]  DEFAULT ((1)) FOR [SiteId];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_DatabaseType]  DEFAULT ('') FOR [ConnectionType];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_ParametersType]  DEFAULT ('') FOR [ParametersType];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Active]  DEFAULT ((1)) FOR [Active];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_ServerAddress]  DEFAULT ('') FOR [Param_ServerAddress];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_DatabaseName]  DEFAULT ('') FOR [Param_DatabaseName];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_Username]  DEFAULT ('') FOR [Param_U];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_P]  DEFAULT ('') FOR [Param_P];
            ALTER TABLE [{0}].[ASPdb_Connections] ADD  CONSTRAINT [DF_ASPdb_Databases_Param_ConnectionString_1]  DEFAULT ('') FOR [Param_ConnectionString];
            ALTER TABLE [{0}].[ASPdb_Connections]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_Databases_ASPdb_Users] FOREIGN KEY([CreatedByUserId])
             REFERENCES [{0}].[ASPdb_Users] ([UserId]);
            ALTER TABLE [{0}].[ASPdb_Connections] CHECK CONSTRAINT [FK_ASPdb_Databases_ASPdb_Users];
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_Tables(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_Tables](
	            [TableId] [int] IDENTITY(1,1) NOT NULL,
	            [ConnectionId] [int] NOT NULL,
	            [Schema] [nvarchar](100) NOT NULL,
	            [TableName] [nvarchar](250) NOT NULL,
	            [Hide] [bit] NOT NULL,
             CONSTRAINT [PK_ASPdb_Tables] PRIMARY KEY CLUSTERED 
            (
	            [TableId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)  
            ) ON [PRIMARY];
            ALTER TABLE [{0}].[ASPdb_Tables] ADD  DEFAULT (N'dbo') FOR [Schema];
            ALTER TABLE [{0}].[ASPdb_Tables] ADD  DEFAULT ((0)) FOR [Hide];
            ALTER TABLE [{0}].[ASPdb_Tables]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_Tables_ASPdb_Connections] FOREIGN KEY([ConnectionId])
            REFERENCES [{0}].[ASPdb_Connections] ([ConnectionId]);
            ALTER TABLE [{0}].[ASPdb_Tables] CHECK CONSTRAINT [FK_ASPdb_Tables_ASPdb_Connections];
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_UserGroups(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_UserGroups](
	            [GroupId] [int] IDENTITY(1,1) NOT NULL,
	            [GroupName] [nvarchar](50) NOT NULL,
	            [Active] [bit] NOT NULL,
	            [TimeCreated] [datetime] NOT NULL,
             CONSTRAINT [PK_ASPdb_UserGroups] PRIMARY KEY CLUSTERED 
            (
	            [GroupId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
            );
            ALTER TABLE [{0}].[ASPdb_UserGroups] ADD  DEFAULT ((1)) FOR [Active];
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_UsersToGroups(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_UsersToGroups](
	            [UserId] [int] NOT NULL,
	            [GroupId] [int] NOT NULL,
             CONSTRAINT [PK_ASPdb_UserAssignments] PRIMARY KEY CLUSTERED 
            (
	            [UserId] ASC,
	            [GroupId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
            );
            ALTER TABLE [{0}].[ASPdb_UsersToGroups]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_UserGroups] FOREIGN KEY([GroupId])
            REFERENCES [{0}].[ASPdb_UserGroups] ([GroupId])
            ON UPDATE CASCADE
            ON DELETE CASCADE;
            ALTER TABLE [{0}].[ASPdb_UsersToGroups] CHECK CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_UserGroups];
            ALTER TABLE [{0}].[ASPdb_UsersToGroups]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_Users] FOREIGN KEY([UserId])
            REFERENCES [{0}].[ASPdb_Users] ([UserId])
            ON UPDATE CASCADE
            ON DELETE CASCADE;
            ALTER TABLE [{0}].[ASPdb_UsersToGroups] CHECK CONSTRAINT [FK_ASPdb_UserAssignments_ASPdb_Users]
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_Values(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_Values](
	            [ValueId] [int] IDENTITY(1,1) NOT NULL,
	            [Key] [nvarchar](50) NOT NULL,
	            [Value] [ntext] NULL,
             CONSTRAINT [PK_ASPdb_ASPdb_Values] PRIMARY KEY CLUSTERED 
            (
	            [ValueId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
            );
            ", appSchema);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        private static string SQLCreate__ASPdb_Permissions(string appSchema)
        {
            string sql = String.Format(@"
            CREATE TABLE [{0}].[ASPdb_Permissions](
	            [PermissionId] [int] IDENTITY(1,1) NOT NULL,
	            [GroupId] [int] NOT NULL,
	            [PermissionType] [nvarchar](1) NOT NULL,
	            [ConnectionId] [int] NULL,
	            [Schema] [nvarchar](100) NULL,
	            [TableId] [int] NULL,
	            [View] [bit] NOT NULL,
	            [Edit] [bit] NOT NULL,
	            [Insert] [bit] NOT NULL,
	            [Delete] [bit] NOT NULL,
             CONSTRAINT [PK_dbo_ASPdb_UserGroups_ToTables] PRIMARY KEY CLUSTERED 
            (
	            [PermissionId] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 
            );
            ALTER TABLE [{0}].[ASPdb_Permissions] ADD  DEFAULT (N'N') FOR [PermissionType];
            ALTER TABLE [{0}].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [View];
            ALTER TABLE [{0}].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [Edit];
            ALTER TABLE [{0}].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [Insert];
            ALTER TABLE [{0}].[ASPdb_Permissions] ADD  DEFAULT ((0)) FOR [Delete];
            ALTER TABLE [{0}].[ASPdb_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_ASPdb_Permissions] FOREIGN KEY([GroupId])
            REFERENCES [{0}].[ASPdb_UserGroups] ([GroupId])
            ON UPDATE CASCADE
            ON DELETE CASCADE;
            ALTER TABLE [{0}].[ASPdb_Permissions] CHECK CONSTRAINT [FK_ASPdb_ASPdb_Permissions];
            ALTER TABLE [{0}].[ASPdb_Permissions]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_ASPdb_Permissions_0001] FOREIGN KEY([TableId])
            REFERENCES [{0}].[ASPdb_Tables] ([TableId])
            ON UPDATE CASCADE
            ON DELETE CASCADE;
            ALTER TABLE [{0}].[ASPdb_Permissions] CHECK CONSTRAINT [FK_ASPdb_ASPdb_Permissions_0001];
            ", appSchema);
            return sql;
        }




        //----------------------------------------------------------------------------------------------------
        private static void CheckForSQLStructureUpdates()
        {
            string installedVersion = Config.ASPdb_Values.GetFirstValue("InstalledVersion", "");
            string lastInstallTime = Config.ASPdb_Values.GetFirstValue("LastInstallTime", "");

            if (installedVersion == Config.SystemProperties.Version)
                return;

            //--------------------
            bool add__ASPdb_Tables__AppProperties = false;
            bool add__ASPdb_Views = false;
            bool add__ASPdb_History = false;

            //--------------------
            try
            {
                string sql = String.Format("select top 1 [{2}] from [{0}].[{1}]", Config.SystemProperties.AppSchema, "ASPdb_Tables", "AppProperties");
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    reader.Read();
            }
            catch { add__ASPdb_Tables__AppProperties = true; }
            try
            {
                string sql = String.Format("select top 1 * from [{0}].[{1}]", Config.SystemProperties.AppSchema, "ASPdb_Views");
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    reader.Read();
            }
            catch { add__ASPdb_Views = true; }
            try
            {
                string sql = String.Format("select top 1 * from [{0}].[{1}]", Config.SystemProperties.AppSchema, "ASPdb_History");
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    reader.Read();
            }
            catch { add__ASPdb_History = true; }



            //--------------------
            if(add__ASPdb_Tables__AppProperties)
            {
                string sql = String.Format(@"ALTER TABLE [{0}].[{1}] ADD [{2}] {3}", Config.SystemProperties.AppSchema, "ASPdb_Tables", "AppProperties", "ntext");
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.ExecuteNonQuery();
                }
            }
            if(add__ASPdb_Views)
            {
                string sql = String.Format(@"
                CREATE TABLE [{0}].[ASPdb_Views](
	                [ViewId] [int] IDENTITY(1,1) NOT NULL,
	                [ConnectionId] [int] NOT NULL,
	                [ViewName] [nvarchar](250) NOT NULL,
	                [Schema] [nvarchar](100) NOT NULL,
	                [Hide] [bit] NOT NULL,
	                [UserFilter] [nvarchar](250) NOT NULL,
                 CONSTRAINT [PK_ASPdb_Views__001] PRIMARY KEY CLUSTERED 
                (
	                [ViewId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                ) ON [PRIMARY];
                ALTER TABLE [ASPdb].[ASPdb_Views] ADD  DEFAULT (N'dbo') FOR [Schema];
                ALTER TABLE [ASPdb].[ASPdb_Views] ADD  DEFAULT ((0)) FOR [Hide];
                ALTER TABLE [ASPdb].[ASPdb_Views] ADD  DEFAULT (N'') FOR [UserFilter];
                ALTER TABLE [ASPdb].[ASPdb_Views]  WITH CHECK ADD  CONSTRAINT [FK_ASPdb_Views_ASPdb_Connections_001] FOREIGN KEY([ConnectionId])
                REFERENCES [ASPdb].[ASPdb_Connections] ([ConnectionId]);
                ALTER TABLE [ASPdb].[ASPdb_Views] CHECK CONSTRAINT [FK_ASPdb_Views_ASPdb_Connections_001];
                ",  Config.SystemProperties.AppSchema);

                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.ExecuteNonQuery();
                }
            }
            if (add__ASPdb_History)
            {
                string sql = String.Format(@"
                CREATE TABLE [{0}].[ASPdb_History](
	                [HistoryId] [int] IDENTITY(1,1) NOT NULL,
	                [TableId] [int] NOT NULL,
	                [KeyValue] [nvarchar](50) NOT NULL,
	                [KeyValueIsTruncated] [bit] NOT NULL,
                    [Revision] [int] NOT NULL,
	                [HistoryType] [nvarchar](10) NOT NULL,
	                [IsPartial] [bit] NOT NULL,
	                [TimeSaved] [datetime] NOT NULL,
	                [ByUserId] [int] NOT NULL,
	                [ByUsername] [nvarchar](50) NOT NULL,
	                [HistoryJSON] [ntext] NOT NULL,
                 CONSTRAINT [PK_ASPdb_ASPdb_History__6] PRIMARY KEY CLUSTERED 
                (
	                [HistoryId] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

                CREATE NONCLUSTERED INDEX [IX_ASPdb_ASPdb_History_0001] ON [ASPdb].[ASPdb_History]
                (
	                [TableId] ASC,
	                [KeyValue] ASC
                )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
                ", Config.SystemProperties.AppSchema);

                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.ExecuteNonQuery();
                }
            }


            Config.ASPdb_Values.Set("InstalledVersion", Config.SystemProperties.Version);
            Config.ASPdb_Values.Set("LastInstallTime", DateTime.Now.ToString());
        }








    }
}