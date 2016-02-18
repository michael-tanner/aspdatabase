using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdb.UniversalADO;
using ASPdatabaseNET.DataObjects.SQLObjects;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using ASPdatabaseNET.DataObjects.DatabaseConnections;
using ASPdatabaseNET.DataObjects.BaseSQLObjects;

namespace ASPdatabaseNET.DataAccess
{
    //----------------------------------------------------------------------------------------------------////
    public class SQLObjectsCRUD
    {

        //----------------------------------------------------------------------------------------------------
        public static List<ASPdb_Connection> ASPdb_Connection__GetAll(bool useCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var cache = ASPdatabaseNET.Memory.AppCache.Get();
            if(useCache)
                if (cache.ASPdb_Database_List != null && cache.ASPdb_Database_List.Count > 0)
                    return cache.ASPdb_Database_List;

            var rtn = new List<ASPdb_Connection>();
            string sql = @"
                select T1.*, T2.Username from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections] as T1 
                left join [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Users] as T2 on T1.CreatedByUserId = T2.UserId
                order by [ConnectionName]";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    DateTime? nullDateTime = null;
                    while (reader.Read())
                    {
                        var item = new ASPdb_Connection()
                        {
                            ConnectionId = reader.Get("ConnectionId", -1),
                            SiteId = reader.Get("SiteId", -1),
                            ConnectionName = reader.Get("ConnectionName", ""),
                            ConnectionType = reader.Get("ConnectionType", ""),
                            ParametersType = reader.Get("ParametersType", ""),
                            Active = reader.Get("Active", false),
                            DateTimeCreated = reader.Get("DateTimeCreated", nullDateTime),
                            CreatedByUserId = reader.Get("CreatedByUserId", -1),
                            CreatedByUsername = reader.Get("Username", ""),
                            Param_ServerAddress = reader.Get("Param_ServerAddress", ""),
                            Param_DatabaseName = reader.Get("Param_DatabaseName", ""),
                            Param_U = reader.Get("Param_U", ""),
                            Param_P = ASPdb_Connection__GetDecryptedPassword_OrNull(reader.Get("Param_P", "")),
                            Param_ConnectionString = reader.Get("Param_ConnectionString", "")
                        };
                        rtn.Add(item);
                    }
                }
            }
            if (useCache)
            {
                cache.ASPdb_Database_List = rtn;
                foreach (var item in rtn)
                    item.Param_P = ""; // don't leave passwords text in cache
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static ASPdb_Connection ASPdb_Connection__Get(int connectionId)
        {
            return ASPdb_Connection__Get(connectionId, true);
        }
        //----------------------------------------------------------------------------------------------------
        public static ASPdb_Connection ASPdb_Connection__Get(int connectionId, bool useCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                var items = ASPdb_Connection__GetAll(useCache);
                foreach (var item in items)
                    if (item.ConnectionId == connectionId)
                        return item;
            }
            catch { }
            return null;
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_Connection__Delete(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = "delete from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections] where [ConnectionId] = @ConnectionId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ConnectionId", connectionId);
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_Connection__Save(ASPdb_Connection connectionInfo)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (connectionInfo == null)
                throw new Exception("Cannot save -- connectionInfo is null.");

            if (connectionInfo.ConnectionId < 0)
                ASPdb_Connection__SaveInsert(connectionInfo);
            else
                ASPdb_Connection__SaveUpdate(connectionInfo);

            ASPdatabaseNET.Memory.AppCache.Reset();
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_Connection__SaveInsert(ASPdb_Connection connectionInfo)
        {
            ASPdb_Connection__CheckForDuplicateConnectionName(connectionInfo);
            int? userId = null;
            try { userId = Users.UserSessionLogic.GetUser().UserInfo.UserId; } catch { }

            string sql = @" insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections]
                                (SiteId, ConnectionName, Active, DateTimeCreated)
                            values (@SiteId, @ConnectionName, @Active, @DateTimeCreated)";
            if(userId.HasValue)
                sql = @"    insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections]
                                (SiteId, ConnectionName, Active, DateTimeCreated, CreatedByUserId)
                            values (@SiteId, @ConnectionName, @Active, @DateTimeCreated, @CreatedByUserId)";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@SiteId", connectionInfo.SiteId);
                command.AddParameter("@ConnectionName", connectionInfo.ConnectionName);
                command.AddParameter("@Active", false);
                command.AddParameter("@DateTimeCreated", DateTime.Now);
                if (userId.HasValue)
                    command.AddParameter("@CreatedByUserId", userId);
                command.Command.ExecuteNonQuery();
            }
            sql = "select [ConnectionId] from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections] where [SiteId] = @SiteId and [ConnectionName] = @ConnectionName";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@SiteId", connectionInfo.SiteId);
                command.AddParameter("@ConnectionName", connectionInfo.ConnectionName);
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    if (reader.Read())
                        connectionInfo.ConnectionId = reader.Get("ConnectionId", -1);
                }
            }
            if (connectionInfo.ConnectionId < 0)
                throw new Exception("Error while attempting to save new database connection.  Please check connections list and try again.");

            ASPdb_Connection__SaveUpdate(connectionInfo);
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_Connection__SaveUpdate(ASPdb_Connection connectionInfo)
        {
            ASPdb_Connection__CheckForDuplicateConnectionName(connectionInfo);

            string encrypted_Password = ASPdb_Connection__GetEncryptedPassword_OrNull(connectionInfo);

            string sql_PasswordStatement = "";
            if (encrypted_Password != null) sql_PasswordStatement = " [Param_P] = @Param_P, ";

            string sql = @"
                update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections] 
                set 
                    [ConnectionName] = @ConnectionName,
                    [ConnectionType] = @ConnectionType,
                    [ParametersType] = @ParametersType,
                    [Active] = @Active,
                    [Param_ServerAddress] = @Param_ServerAddress,
                    [Param_DatabaseName] = @Param_DatabaseName,
                    [Param_U] = @Param_U,
                    " + sql_PasswordStatement + @"
                    [Param_ConnectionString] = @Param_ConnectionString
                where [ConnectionId] = @ConnectionId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ConnectionId", connectionInfo.ConnectionId);
                command.AddParameter("@ConnectionName", connectionInfo.ConnectionName);
                command.AddParameter("@ConnectionType", connectionInfo.ConnectionType);
                command.AddParameter("@ParametersType", connectionInfo.ParametersType);
                command.AddParameter("@Active", connectionInfo.Active);
                command.AddParameter("@Param_ServerAddress", connectionInfo.Param_ServerAddress);
                command.AddParameter("@Param_DatabaseName", connectionInfo.Param_DatabaseName);
                command.AddParameter("@Param_U", connectionInfo.Param_U);
                if(encrypted_Password != null)
                    command.AddParameter("@Param_P", encrypted_Password);
                command.AddParameter("@Param_ConnectionString", connectionInfo.Param_ConnectionString);
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_Connection__CheckForDuplicateConnectionName(ASPdb_Connection connectionInfo)
        {
            bool isADuplicate = false;
            string sql = "select [ConnectionId], [ConnectionName] from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Connections] where SiteId = @SiteId and ConnectionName = @ConnectionName";
            try
            {
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@SiteId", connectionInfo.SiteId);
                    command.AddParameter("@ConnectionName", connectionInfo.ConnectionName);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if (reader.Read())
                        {
                            int connectionId = reader.Get("ConnectionId", -1);
                            string connectionName = reader.Get("ConnectionName", "").Trim().ToLower();
                            if (connectionInfo.ConnectionName.Trim().ToLower() == connectionName)
                            {
                                if (connectionInfo.ConnectionId != connectionId)
                                    isADuplicate = true;
                                else if (connectionId <= 0)
                                    isADuplicate = true;
                            }
                        }
                    }
                }
            }
            catch { }
            if (isADuplicate)
                throw new Exception(String.Format("The Database Connection Name \"{0}\" already exists." +
                    "\n\nPlease choose a different name.", connectionInfo.ConnectionName.Trim()));
        }

        //----------------------------------------------------------------------------------------------------
        private static string ASPdb_Connection__Purpose = "ASPdatabase.NET - Private Password Purpose";
        //----------------------------------------------------------------------------------------------------
        private static string ASPdb_Connection__GetEncryptedPassword_OrNull(ASPdb_Connection connectionInfo)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                string p = connectionInfo.Param_P;
                string p2 = p.Replace("#", "").Trim();
                if (p != null && p.Length > 0 && p2.Length > 0)
                {
                    return ASPdb.Security.MachineEncryption.Protect(p, ASPdb_Connection__Purpose);
                }
            }
            catch { }
            return null;
        }
        //----------------------------------------------------------------------------------------------------
        private static string ASPdb_Connection__GetDecryptedPassword_OrNull(string encryptedString)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                return ASPdb.Security.MachineEncryption.Unprotect(encryptedString, ASPdb_Connection__Purpose);
            }
            catch { }
            return null;
        }





        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Always uses cache (if already stored in cache).
        /// Does not perform table sync.
        /// </summary>
        public static ASPdb_Table ASPdb_Table__Get(int tableId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var cache = ASPdatabaseNET.Memory.AppCache.Get();
            if (cache.ASPdb_Table_Dictionary2 != null)
                if (cache.ASPdb_Table_Dictionary2.ContainsKey(tableId) && cache.ASPdb_Table_Dictionary2[tableId] != null)
                    return cache.ASPdb_Table_Dictionary2[tableId]; // return from cache if it's there

            ASPdb_Table rtn = null;

            int connectionId = -1;
            string sql = "select [ConnectionId] from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables] where [TableId] = @TableId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@TableId", tableId);
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    if (reader.Read())
                        connectionId = reader.Get("ConnectionId", -1);
                    if (connectionId < 0)
                        return null;
                }
            }
            var aspdb_Tables = ASPdb_Table__GetAll(connectionId, false);
            for (int i = 0; i < aspdb_Tables.Count; i++)
                if (aspdb_Tables[i].TableId == tableId)
                {
                    rtn = aspdb_Tables[i];
                    i = aspdb_Tables.Count + 1;
                }

            // store in cache before returning
            if (rtn != null)
            {
                if (cache.ASPdb_Table_Dictionary2 == null)
                    cache.ASPdb_Table_Dictionary2 = new Dictionary<int, ASPdb_Table>();
                if (cache.ASPdb_Table_Dictionary2.ContainsKey(tableId))
                    cache.ASPdb_Table_Dictionary2[tableId] = rtn;
                else cache.ASPdb_Table_Dictionary2.Add(tableId, rtn);
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static List<BaseTableInfo> ASPdb_Table__GetRawList_NoCache(int connectionId)
        {
            var aspdb_Connection = ASPdb_Connection__Get(connectionId);
            return ASPdb_Table__GetRawList_NoCache(aspdb_Connection);
        }
        //----------------------------------------------------------------------------------------------------
        public static List<BaseTableInfo> ASPdb_Table__GetRawList_NoCache(ASPdb_Connection aspdb_Connection)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new List<BaseTableInfo>();

            if (aspdb_Connection.E_ConnectionType == ASPdb_Connection.Enum_ConnectionTypes.SQLServer ||
                aspdb_Connection.E_ConnectionType == ASPdb_Connection.Enum_ConnectionTypes.SQLServerAzure)
            {
                string sql = @"
                    select 
	                    t2.schema_id,
	                    t2.[name] [SchemaName], 
	                    t1.[name] [TableName], 
	                    t1.object_id,
	                    t1.create_date, 
	                    t1.modify_date
                    from sys.tables t1
                    inner join 
	                    sys.schemas t2 on t1.schema_id = t2.schema_id
                    order by t2.[name], t1.[name]
                    ";

                //sql = sql.Replace("[", "`").Replace("]", "`");

                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(aspdb_Connection.GetConnectionString(), sql))
                {
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        DateTime? nullDate = null;
                        while (reader.Read())
                        {
                            var item = new BaseTableInfo();
                            item.SchemaId = reader.Get("schema_id", "");
                            item.SchemaName = reader.Get("SchemaName", "");
                            item.TableName = reader.Get("TableName", "");
                            item.ObjectId = reader.Get("object_id", "");
                            item.CreateDate = reader.Get("create_date", nullDate);
                            item.ModifyDate = reader.Get("modify_date", nullDate);
                            rtn.Add(item);
                        }
                    }
                }
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static List<ASPdb_Table> ASPdb_Table__GetAll(int connectionId, bool resetCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                var cache = ASPdatabaseNET.Memory.AppCache.Get();
                if (resetCache)
                    cache.ASPdb_Table_Dictionary1 = null;
                var cacheDict = cache.ASPdb_Table_Dictionary1;

                if (cacheDict == null)
                {
                    cache.ASPdb_Table_Dictionary1 = new Dictionary<int, Memory.AppCache.CacheHolder_Tables>();
                    cacheDict = cache.ASPdb_Table_Dictionary1;
                }
                else if (cacheDict.ContainsKey(connectionId)
                    && cacheDict[connectionId] != null
                    && cacheDict[connectionId].ASPdb_Table_List != null
                    && cacheDict[connectionId].ASPdb_Table_List.Count > 0)
                { 
                    return cacheDict[connectionId].ASPdb_Table_List; 
                }

                var rtn = new List<ASPdb_Table>();
                foreach (var item in ASPdb_Table__GetRawList_NoCache(connectionId))
                {
                    rtn.Add(new ASPdb_Table(connectionId, item.SchemaName, item.TableName));
                }

                if (!cacheDict.ContainsKey(connectionId))
                    cacheDict.Add(connectionId, new Memory.AppCache.CacheHolder_Tables());

                rtn = (from r in rtn
                       orderby r.Schema, r.TableName
                       select r).ToList();

                ASPdb_Table__SyncTablesWithProperties(connectionId, rtn);
                cache.ASPdb_Table_Dictionary1[connectionId].ASPdb_Table_List = rtn;
                return rtn;
            }
            catch(Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                throw new Exception("connectionId: " + connectionId.ToString() + " ... " + exc.Message + "\n\n" + exc.StackTrace);
            }
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_Table__SyncTablesWithProperties(int connectionId, List<ASPdb_Table> tablesList)
        {
            var dict = new Dictionary<string, ASPdb_Table>();
            foreach (var item in tablesList)
                if (!dict.ContainsKey(item.UniqueNameKey))
                    dict.Add(item.UniqueNameKey, item);

            ASPdb_Table__SyncTablesWithProperties_Helper1__GetAndPopulate(connectionId, tablesList, dict);

            bool newRecsWereInserted = false;
            foreach (var item in tablesList)
            {
                if (item.TableId < 0)
                {
                    newRecsWereInserted = true;
                    item.ConnectionId = connectionId;
                    ASPdb_Table__SyncTablesWithProperties_Helper2__Insert(item);
                }
            }
            if(newRecsWereInserted)
                ASPdb_Table__SyncTablesWithProperties_Helper1__GetAndPopulate(connectionId, tablesList, dict); // this will retrieve & assign newly inserted TableIds
            
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_Table__SyncTablesWithProperties_Helper1__GetAndPopulate(int connectionId, List<ASPdb_Table> tablesList, Dictionary<string, ASPdb_Table> dict)
        {
            string sql = "select * from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables] where [ConnectionId] = @ConnectionId order by [TableName]";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ConnectionId", connectionId);
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        string db_TableName = reader.Get("TableName", "");
                        string db_Schema = reader.Get("Schema", "");
                        string db_UniqueTableNameKey = db_Schema.ToLower() + "." + db_TableName.ToLower();
                        if (dict.ContainsKey(db_UniqueTableNameKey))
                        {
                            ASPdb_Table item = dict[db_UniqueTableNameKey];
                            item.TableId = reader.Get("TableId", -1);
                            item.ConnectionId = reader.Get("ConnectionId", -1);
                            item.TableName = db_TableName;
                            item.Schema = db_Schema;
                            item.Hide = reader.Get("Hide", false);
                        }
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_Table__SyncTablesWithProperties_Helper2__Insert(ASPdb_Table aspdb_Table)
        {
            string sql = String.Format(@"
                insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables]
                ([ConnectionId], [TableName], [Schema], [Hide])
                values
                (@ConnectionId, @TableName, @Schema, 0)
            ");
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ConnectionId", aspdb_Table.ConnectionId);
                command.AddParameter("@TableName", aspdb_Table.TableName);
                command.AddParameter("@Schema", aspdb_Table.Schema);
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_Table__ShowHide(int connectionId, int tableId, bool hide)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_Table = ASPdb_Table__Get(tableId);
            if (aspdb_Table.ConnectionId != connectionId)
                throw new Exception ("ConnectionId appears to be invalid.");

            string sql = "update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables] set [Hide] = @Hide where [TableId] = @TableId";
            using(DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@Hide", hide);
                command.AddParameter("@TableId", tableId);
                command.Command.ExecuteNonQuery();
            }
            aspdb_Table.Hide = hide;
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_Table__Rename(int connectionId, int tableId, string newName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_Table = ASPdb_Table__Get(tableId);
            if (aspdb_Table.ConnectionId != connectionId)
                throw new Exception("ConnectionId appears to be invalid.");

            string sql = "sp_rename @CurrentName, @NewName";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionId, sql))
            {
                command.AddParameter("@CurrentName", aspdb_Table.Schema + "." + aspdb_Table.TableName);
                command.AddParameter("@NewName", newName);
                command.Command.ExecuteNonQuery();
            }
            string sql2 = "update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables] set [TableName] = @TableName where [TableId] = @TableId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql2))
            {
                command.AddParameter("@TableName", newName);
                command.AddParameter("@TableId", tableId);
                command.Command.ExecuteNonQuery();
            }
            aspdb_Table.TableName = newName;
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_Table__Delete(int connectionId, int tableId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_Table = ASPdb_Table__Get(tableId);
            if (aspdb_Table.ConnectionId != connectionId)
                throw new Exception("ConnectionId appears to be invalid.");

            string sql = String.Format("drop table [{0}].[{1}]", aspdb_Table.Schema, aspdb_Table.TableName);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }





        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Always uses cache (if already stored in cache).
        /// Does not perform table sync.
        /// </summary>
        public static ASPdb_View ASPdb_View__Get(int viewId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var cache = ASPdatabaseNET.Memory.AppCache.Get();
            if (cache.ASPdb_View_Dictionary2 != null)
                if (cache.ASPdb_View_Dictionary2.ContainsKey(viewId) && cache.ASPdb_View_Dictionary2[viewId] != null)
                    return cache.ASPdb_View_Dictionary2[viewId]; // return from cache if it's there

            ASPdb_View rtn = null;

            int connectionId = -1;
            string sql = "select [ConnectionId] from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Views] where [ViewId] = @ViewId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ViewId", viewId);
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    if (reader.Read())
                        connectionId = reader.Get("ConnectionId", -1);
                    if (connectionId < 0)
                        return null;
                }
            }
            var aspdb_Views = ASPdb_View__GetAll(connectionId);
            for (int i = 0; i < aspdb_Views.Count; i++)
                if (aspdb_Views[i].ViewId == viewId)
                {
                    rtn = aspdb_Views[i];
                    i = aspdb_Views.Count + 1;
                }

            // store in cache before returning
            if (rtn != null)
            {
                if (cache.ASPdb_View_Dictionary2 == null)
                    cache.ASPdb_View_Dictionary2 = new Dictionary<int, ASPdb_View>();
                if (cache.ASPdb_View_Dictionary2.ContainsKey(viewId))
                    cache.ASPdb_View_Dictionary2[viewId] = rtn;
                else cache.ASPdb_View_Dictionary2.Add(viewId, rtn);
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static List<BaseViewInfo> ASPdb_View__GetRawList_NoCache(int connectionId)
        {
            var aspdb_Connection = ASPdb_Connection__Get(connectionId);
            return ASPdb_View__GetRawList_NoCache(aspdb_Connection);
        }
        //----------------------------------------------------------------------------------------------------
        public static List<BaseViewInfo> ASPdb_View__GetRawList_NoCache(ASPdb_Connection aspdb_Connection)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new List<BaseViewInfo>();

            if (aspdb_Connection.E_ConnectionType == ASPdb_Connection.Enum_ConnectionTypes.SQLServer ||
                aspdb_Connection.E_ConnectionType == ASPdb_Connection.Enum_ConnectionTypes.SQLServerAzure)
            {
                string sql = @"
                    select 
	                    t2.schema_id,
	                    t2.[name] [SchemaName], 
	                    t1.[name] [ViewName], 
	                    t1.object_id,
	                    t1.create_date, 
	                    t1.modify_date
                    from sys.views t1
                    inner join 
	                    sys.schemas t2 on t1.schema_id = t2.schema_id
                    order by t2.[name], t1.[name]
                    ";
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(aspdb_Connection.GetConnectionString(), sql))
                {
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        DateTime? nullDate = null;
                        while (reader.Read())
                        {
                            var item = new BaseViewInfo();
                            item.SchemaId = reader.Get("schema_id", "");
                            item.SchemaName = reader.Get("SchemaName", "");
                            item.ViewName = reader.Get("ViewName", "");
                            item.ObjectId = reader.Get("object_id", "");
                            item.CreateDate = reader.Get("create_date", nullDate);
                            item.ModifyDate = reader.Get("modify_date", nullDate);
                            rtn.Add(item);
                        }
                    }
                }
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static List<ASPdb_View> ASPdb_View__GetAll(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                var cache = ASPdatabaseNET.Memory.AppCache.Get();
                var cacheDict = cache.ASPdb_View_Dictionary1;

                if (cacheDict == null)
                {
                    cache.ASPdb_View_Dictionary1 = new Dictionary<int, Memory.AppCache.CacheHolder_Views>();
                    cacheDict = cache.ASPdb_View_Dictionary1;
                }
                else if (cacheDict.ContainsKey(connectionId)
                    && cacheDict[connectionId] != null
                    && cacheDict[connectionId].ASPdb_View_List != null
                    && cacheDict[connectionId].ASPdb_View_List.Count > 0)
                {
                    return cacheDict[connectionId].ASPdb_View_List;
                }

                var rtn = new List<ASPdb_View>();
                foreach (var item in ASPdb_View__GetRawList_NoCache(connectionId))
                {
                    rtn.Add(new ASPdb_View(connectionId, item.SchemaName, item.ViewName));
                }

                if (!cacheDict.ContainsKey(connectionId))
                    cacheDict.Add(connectionId, new Memory.AppCache.CacheHolder_Views());

                rtn = (from r in rtn
                       orderby r.ViewName
                       select r).ToList();

                ASPdb_View__SyncViewsWithProperties(connectionId, rtn);
                cache.ASPdb_View_Dictionary1[connectionId].ASPdb_View_List = rtn;
                return rtn;
            }
            catch (Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                return new List<ASPdb_View>();
                //throw new Exception("connectionId: " + connectionId.ToString() + " ... " + exc.Message + "\n\n" + exc.StackTrace);
            }
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_View__SyncViewsWithProperties(int connectionId, List<ASPdb_View> viewsList)
        {
            var dict = new Dictionary<string, ASPdb_View>();
            foreach (var item in viewsList)
                if (!dict.ContainsKey(item.UniqueNameKey))
                    dict.Add(item.UniqueNameKey, item);

            ASPdb_View__SyncViewsWithProperties_Helper1__GetAndPopulate(connectionId, viewsList, dict);

            bool newRecsWereInserted = false;
            foreach (var item in viewsList)
            {
                if (item.ViewId < 0)
                {
                    newRecsWereInserted = true;
                    item.ConnectionId = connectionId;
                    ASPdb_View__SyncViewsWithProperties_Helper2__Insert(item);
                }
            }
            if (newRecsWereInserted)
                ASPdb_View__SyncViewsWithProperties_Helper1__GetAndPopulate(connectionId, viewsList, dict); // this will retrieve & assign newly inserted ViewIds

        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_View__SyncViewsWithProperties_Helper1__GetAndPopulate(int connectionId, List<ASPdb_View> viewsList, Dictionary<string, ASPdb_View> dict)
        {
            string sql = "select * from [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Views] where [ConnectionId] = @ConnectionId order by [ViewName]";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ConnectionId", connectionId);
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        string db_ViewName = reader.Get("ViewName", "");
                        string db_Schema = reader.Get("Schema", "");
                        string db_UniqueNameKey = db_Schema.ToLower().Trim() + "." + db_ViewName.ToLower().Trim();
                        if (dict.ContainsKey(db_UniqueNameKey))
                        {
                            ASPdb_View item = dict[db_UniqueNameKey];
                            item.ViewId = reader.Get("ViewId", -1);
                            item.ConnectionId = reader.Get("ConnectionId", -1);
                            item.ViewName = db_ViewName;
                            item.Schema = db_Schema;
                            item.Hide = reader.Get("Hide", false);
                        }
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        private static void ASPdb_View__SyncViewsWithProperties_Helper2__Insert(ASPdb_View aspdb_View)
        {
            string sql = String.Format(@"
                insert into [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Views]
                ([ConnectionId], [ViewName], [Schema], [Hide])
                values
                (@ConnectionId, @ViewName, @Schema, 0)
            ");
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@ConnectionId", aspdb_View.ConnectionId);
                command.AddParameter("@ViewName", aspdb_View.ViewName);
                command.AddParameter("@Schema", aspdb_View.Schema);
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_View__ShowHide(int connectionId, int viewId, bool hide)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_View = ASPdb_View__Get(viewId);
            if (aspdb_View.ConnectionId != connectionId)
                throw new Exception("ConnectionId appears to be invalid.");

            string sql = "update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Views] set [Hide] = @Hide where [ViewId] = @ViewId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@Hide", hide);
                command.AddParameter("@ViewId", viewId);
                command.Command.ExecuteNonQuery();
            }
            aspdb_View.Hide = hide;
            Memory.AppCache.Reset();
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdb_View__Rename(int connectionId, int viewId, string newName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_View = ASPdb_View__Get(viewId);
            if (aspdb_View.ConnectionId != connectionId)
                throw new Exception("ConnectionId appears to be invalid.");

            string sql = "sp_rename @CurrentName, @NewName";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionId, sql))
            {
                command.AddParameter("@CurrentName", aspdb_View.Schema + "." + aspdb_View.ViewName);
                command.AddParameter("@NewName", newName);
                command.Command.ExecuteNonQuery();
            }
            string sql2 = "update [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Views] set [ViewName] = @ViewName where [ViewId] = @ViewId";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql2))
            {
                command.AddParameter("@ViewName", newName);
                command.AddParameter("@ViewId", viewId);
                command.Command.ExecuteNonQuery();
            }
            aspdb_View.ViewName = newName;
            Memory.AppCache.Reset();
        }

    }
}