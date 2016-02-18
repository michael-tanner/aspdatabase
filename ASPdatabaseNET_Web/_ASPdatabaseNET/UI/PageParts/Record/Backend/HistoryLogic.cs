using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdb.UniversalADO;
using ASPdatabaseNET.UI.PageParts.Record.Objs_History;
using System.Web.Script.Serialization;

namespace ASPdatabaseNET.UI.PageParts.Record.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class HistoryLogic
    {
        private static bool? _doSaveHistory = null;
        public static bool DoSaveHistory
        {
            get
            {
                if(_doSaveHistory == null)
                {
                    _doSaveHistory = true;
                    try
                    {
                        if (System.Configuration.ConfigurationManager.AppSettings["HistoryTracking"].ToLower().Trim() == "false")
                            _doSaveHistory = false;
                    }
                    catch { }
                }
                return _doSaveHistory.GetValueOrDefault(true);
            }
        }

        //----------------------------------------------------------------------------------------------------
        private static string History_ConnectionString()
        {
            return CoreDbConfig.ConnectionString;
        }

        //----------------------------------------------------------------------------------------------------
        public static string PrimaryKey_ToString(string[] keyValueArr)
        {
            return (new JavaScriptSerializer()).Serialize(keyValueArr);
        }
        //----------------------------------------------------------------------------------------------------
        public static string[] PrimaryKey_ToArray(string keyValueStr)
        {
            return (new JavaScriptSerializer()).Deserialize<string[]>(keyValueStr);
        }


        //----------------------------------------------------------------------------------------------------
        public static HistorySummary Get_HistorySummary(int tableId, string[] keyValue, int maxReturnCount)
        {
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(tableId, false, true, false);
            var aspdb_Connection = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(tableStructure.ConnectionId);
            var rtn = new HistorySummary()
            {
                ConnectionId = tableStructure.ConnectionId,
                TableId = tableId,
                ConnectionName = aspdb_Connection.ConnectionName,
                Schema = tableStructure.Schema,
                TableName = tableStructure.TableName,
                KeyValue = keyValue,
                HistoryCount = Get_HistoryCount(tableId, keyValue),
                HistoryRecords = Get_HistoryRecords(tableId, keyValue, maxReturnCount)
            };
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static int Get_HistoryCount(int tableId, string[] keyValue)
        {
            int rtn = -1;
            string keyValueStr = PrimaryKey_ToString(keyValue);
            string keyValueStr_Orig = keyValueStr;
            bool keyValueIsTruncated = (keyValueStr.Length > 50);
            if (keyValueIsTruncated)
                keyValueStr = keyValueStr.Substring(0, 50);

            if(!keyValueIsTruncated)
            {
                string sql = String.Format("select count(*) as [Count1] from [{0}].[{1}] where [TableId] = @TableId and [KeyValue] = @KeyValue and [KeyValueIsTruncated] = @KeyValueIsTruncated",
                    Config.SystemProperties.AppSchema, "ASPdb_History");
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(History_ConnectionString(), sql))
                {
                    command.AddParameter("@TableId", tableId);
                    command.AddParameter("@KeyValue", keyValueStr);
                    command.AddParameter("@KeyValueIsTruncated", false);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        if (reader.Read())
                            rtn = reader.Get("Count1", -1);
                }
            }
            else
            {
                rtn = 0;
                string sql = String.Format("select [HistoryId], [HistoryJSON] from [{0}].[{1}] where [TableId] = @TableId and [KeyValue] = @KeyValue and [KeyValueIsTruncated] = @KeyValueIsTruncated",
                    Config.SystemProperties.AppSchema, "ASPdb_History");
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(History_ConnectionString(), sql))
                {
                    command.AddParameter("@TableId", tableId);
                    command.AddParameter("@KeyValue", keyValueStr);
                    command.AddParameter("@KeyValueIsTruncated", true);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        if (reader.Read())
                            try
                            {
                                int historyId = reader.Get("HistoryId", -1);
                                var historyJsonObj = (new JavaScriptSerializer()).Deserialize<HistoryJsonObj>(reader.Get("HistoryJSON", ""));
                                if (historyJsonObj.KeyValue == keyValueStr_Orig)
                                    rtn++;
                            }
                            catch { }
                }
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static HistoryRecord[] Get_HistoryRecords(int tableId, string[] keyValue, int maxReturnCount)
        {
            var rtn = new List<HistoryRecord>();
            string keyValueStr = PrimaryKey_ToString(keyValue);
            string keyValueStr_Orig = keyValueStr;
            bool keyValueIsTruncated = (keyValueStr.Length > 50);
            if (keyValueIsTruncated)
                keyValueStr = keyValueStr.Substring(0, 50);

            string topSql = "";
            if (maxReturnCount > 0)
                topSql = "top " + maxReturnCount;
            string columnsToGet = " [HistoryId], [TableId], [KeyValue], [Revision], [HistoryType], [IsPartial], [TimeSaved], [ByUserId], [ByUsername] ";
            if (keyValueIsTruncated)
                columnsToGet = " * ";
            string sql = String.Format(@"
                    select 
                    {0}
                    {1}
                    from [{2}].[{3}] where [TableId] = @TableId and [KeyValue] = @KeyValue and [KeyValueIsTruncated] = @KeyValueIsTruncated
                    order by [Revision] desc, [HistoryId] desc ",
                    topSql, columnsToGet, Config.SystemProperties.AppSchema, "ASPdb_History");
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(History_ConnectionString(), sql))
            {
                command.AddParameter("@TableId", tableId);
                command.AddParameter("@KeyValue", keyValueStr);
                command.AddParameter("@KeyValueIsTruncated", keyValueIsTruncated);
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    while (reader.Read())
                    {
                        try
                        {
                            if (keyValueIsTruncated)
                                if ((new JavaScriptSerializer()).Deserialize<HistoryJsonObj>(reader.Get("HistoryJSON", "")).KeyValue != keyValueStr_Orig)
                                    continue;
                        } 
                        catch { continue; }
                        var historyType = HistoryRecord.HistoryTypes.NotSet;
                        switch (reader.Get("HistoryType", ""))
                        {
                            case "Insert": historyType = HistoryRecord.HistoryTypes.Insert; break;
                            case "Update": historyType = HistoryRecord.HistoryTypes.Update; break;
                            case "Delete": historyType = HistoryRecord.HistoryTypes.Delete; break;
                        }
                        DateTime? nullDateTime = null;

                        var timeSaved = reader.Get("TimeSaved", nullDateTime);
                        string timeLapsedString = "";
                        if(timeSaved.HasValue)
                        {
                            var days = (DateTime.Now.Date - timeSaved.Value.Date).TotalDays;

                            if (timeSaved.Value.Date == DateTime.Now.Date)
                                timeLapsedString = "today";
                            else if (timeSaved.Value.Date == DateTime.Now.AddDays(-1).Date)
                                timeLapsedString = "yesterday";
                            else
                                timeLapsedString = days + " days ago";

                            
                        }


                        rtn.Add(new HistoryRecord()
                        {
                            HistoryId = reader.Get("HistoryId", -1),
                            TableId = reader.Get("TableId", -1),
                            KeyValue = keyValue,
                            Revision = reader.Get("Revision", -1),
                            HistoryType = historyType,
                            IsPartial = reader.Get("IsPartial", false),
                            TimeSaved = reader.Get("TimeSaved", DateTime.Now),
                            TimeSaved_String = reader.Get("TimeSaved", nullDateTime).ToString(),
                            ByUserId = reader.Get("ByUserId", -1),
                            ByUsername = reader.Get("ByUsername", ""),
                            HistoryJsonObj = null,
                            TimeLapsedString = timeLapsedString
                        });
                    }
            }
            return rtn.ToArray();
        }
        //----------------------------------------------------------------------------------------------------
        public static HistoryRecord Get_HistoryRecord(int tableId, int historyId)
        {
            HistoryRecord rtn = null;
            string sql = String.Format("select * from [{0}].[{1}] where [TableId] = @TableId and [HistoryId] = @HistoryId",
                Config.SystemProperties.AppSchema, "ASPdb_History");
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(History_ConnectionString(), sql))
            {
                command.AddParameter("@TableId", tableId);
                command.AddParameter("@HistoryId", historyId);
                using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    if (reader.Read())
                    {
                        string keyValueStr = reader.Get("KeyValue", "");
                        bool keyValueIsTruncated = reader.Get("KeyValueIsTruncated", false);

                        HistoryJsonObj historyJsonObj = null;
                        try { historyJsonObj = (new JavaScriptSerializer()).Deserialize<HistoryJsonObj>(reader.Get("HistoryJSON", "")); }
                        catch { }
                        if (keyValueIsTruncated && historyJsonObj != null)
                            keyValueStr = historyJsonObj.KeyValue;
                        string[] keyValue = PrimaryKey_ToArray(keyValueStr);

                        var historyType = HistoryRecord.HistoryTypes.NotSet;
                        switch(reader.Get("HistoryType", ""))
                        {
                            case "Insert": historyType = HistoryRecord.HistoryTypes.Insert; break;
                            case "Update": historyType = HistoryRecord.HistoryTypes.Update; break;
                            case "Delete": historyType = HistoryRecord.HistoryTypes.Delete; break;
                        }
                        DateTime? nullDateTime = null;
                        rtn = new HistoryRecord()
                        {
                            HistoryId = reader.Get("HistoryId", -1),
                            TableId = reader.Get("TableId", -1),
                            KeyValue = keyValue,
                            HistoryType = historyType,
                            IsPartial = reader.Get("IsPartial", false),
                            TimeSaved = reader.Get("TimeSaved", DateTime.Now),
                            TimeSaved_String = reader.Get("TimeSaved", nullDateTime).ToString(),
                            ByUserId = reader.Get("ByUserId", -1),
                            ByUsername = reader.Get("ByUsername", ""),
                            HistoryJsonObj = historyJsonObj
                        };
                    }
            }


            var itemsDict = new Dictionary<string, Item>();
            foreach (var item in rtn.HistoryJsonObj.Fields)
                if (!itemsDict.ContainsKey(item.cn))
                    itemsDict.Add(item.cn, item);
            var curHistoryRecord = new HistoryRecord()
            {
                HistoryId = -1,
                TableId = tableId,
                KeyValue = rtn.KeyValue
            };
            Initialize_HistoryRecord_AllFields(curHistoryRecord, "right");
            var fields_3ValArr = new List<Item_3Values>();
            foreach(var item in curHistoryRecord.HistoryJsonObj.Fields)
            {
                string cn = item.cn;
                string v1 = null;
                string v2 = null;
                string v3 = item.v2;
                bool match = true;
                if(itemsDict.ContainsKey(cn))
                {
                    v1 = itemsDict[cn].v1;
                    v2 = itemsDict[cn].v2;
                    match = itemsDict[cn].match;
                    itemsDict.Remove(cn);
                }
                fields_3ValArr.Add(new Item_3Values()
                {
                    ColumnName_OrigCasing = cn,
                    cn = cn,
                    v1 = v1,
                    v2 = v2,
                    v3 = v3,
                    match = match // (v1 == v2)
                });
            }
            if (itemsDict.Count > 0)
                foreach (var item in itemsDict.Values) // -- in cases where columns have been removed from table structure
                    fields_3ValArr.Add(new Item_3Values()
                    {
                        ColumnName_OrigCasing = item.cn,
                        cn = item.cn,
                        v1 = item.v1,
                        v2 = item.v2,
                        v3 = null,
                        match = false
                    });
            rtn.Fields_3ValArr = fields_3ValArr.ToArray();

            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(tableId, false, true, false);
            var dict = new Dictionary<string, string>();
            foreach(var item in tableStructure.Columns)
            {
                string key = item.ColumnName.Trim().ToLower();
                string value = item.ColumnName.Trim();
                if (!dict.ContainsKey(key))
                    dict.Add(key, value);
            }
            foreach (var item in rtn.Fields_3ValArr)
                if (dict.ContainsKey(item.cn))
                    item.ColumnName_OrigCasing = dict[item.cn];




            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static int Save_HistoryRecord(HistoryRecord historyRecord)
        {
            int rtn = -1;
            if (!DoSaveHistory)
                return rtn;

            string keyValueStr = PrimaryKey_ToString(historyRecord.KeyValue);

            historyRecord.HistoryJsonObj.KeyValue = keyValueStr;
            string historyJson = (new JavaScriptSerializer()).Serialize(historyRecord.HistoryJsonObj);

            
            bool keyValueIsTruncated = (keyValueStr.Length > 50);
            if (keyValueIsTruncated)
                keyValueStr = keyValueStr.Substring(0, 50);

            var userInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser().UserInfo;
            historyRecord.ByUserId = userInfo.UserId;
            historyRecord.ByUsername = userInfo.Username;
            historyRecord.TimeSaved = DateTime.Now;
            historyRecord.TimeSaved_String = historyRecord.TimeSaved.ToString();

            int revision = 1;
            var topHistoryRecord = Get_HistoryRecords(historyRecord.TableId, historyRecord.KeyValue, 1);
            if (topHistoryRecord != null && topHistoryRecord.Length > 0 && topHistoryRecord[0].Revision > 0)
                revision = topHistoryRecord[0].Revision + 1;

            string sql = String.Format(@"
                insert into [{0}].[{1}]
                ([TableId], [KeyValue], [KeyValueIsTruncated], [Revision], [HistoryType], [IsPartial], [TimeSaved], [ByUserId], [ByUsername], [HistoryJSON])
                values
                (@TableId, @KeyValue, @KeyValueIsTruncated, @Revision, @HistoryType, @IsPartial, @TimeSaved, @ByUserId, @ByUsername, @HistoryJSON);
                
                SELECT @@IDENTITY AS [Identity]; "
                , Config.SystemProperties.AppSchema, "ASPdb_History");
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(History_ConnectionString(), sql))
            {
                command.AddParameter("@TableId", historyRecord.TableId);
                command.AddParameter("@KeyValue", keyValueStr);
                command.AddParameter("@KeyValueIsTruncated", keyValueIsTruncated);
                command.AddParameter("@Revision", revision);
                command.AddParameter("@HistoryType", historyRecord.HistoryType.ToString());
                command.AddParameter("@IsPartial", historyRecord.IsPartial);
                command.AddParameter("@TimeSaved", historyRecord.TimeSaved);
                command.AddParameter("@ByUserId", historyRecord.ByUserId);
                command.AddParameter("@ByUsername", historyRecord.ByUsername);
                command.AddParameter("@HistoryJSON", historyJson);
                using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    if (reader.Read())
                        rtn = reader.Get("Identity", -1);
            }
            return rtn;
        }


        //----------------------------------------------------------------------------------------------------
        public static void Initialize_HistoryRecord_AllFields(HistoryRecord historyRecord, string populateSide)
        {
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(historyRecord.TableId, false, true, false);

            var valuesDict = new Dictionary<string, string>();

            string sqlWhere = "";
            int pkCount = tableStructure.PrimaryKey.Columns.Length;
            for (int i = 0; i < pkCount; i++)
            {
                if (sqlWhere != "")
                    sqlWhere += " and ";
                sqlWhere += String.Format(" [{0}] = @Value{1} ", tableStructure.PrimaryKey.Columns[i].ColumnName, i);
            }
            string sql = String.Format(@"select * from [{0}].[{1}] where {2}", 
                tableStructure.Schema, tableStructure.TableName, sqlWhere);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                for (int i = 0; i < pkCount; i++)
                {
                    string pkValue = "";
                    try { pkValue = historyRecord.KeyValue[i]; }
                    catch { }
                    command.AddParameter("@Value" + i, pkValue);
                }
                using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    if (reader.Read())
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string columnName = reader.GetName(i);
                            string columnName_L = columnName.Trim().ToLower();
                            string value = reader.GetString_OrNullDefault(columnName);
                            if (!valuesDict.ContainsKey(columnName_L))
                                valuesDict.Add(columnName_L, value);
                        }
            }

            var fieldsList = new List<Item>();
            foreach(string key_L in valuesDict.Keys)
            {
                var item = new Item() { cn = key_L, v1 = null, v2 = null };
                string value = valuesDict[key_L];
                if(populateSide.ToLower() == "left")
                    item.v1 = value;
                else if (populateSide.ToLower() == "right")
                    item.v2 = value;
                fieldsList.Add(item);
            }
            if (historyRecord.HistoryJsonObj == null)
                historyRecord.HistoryJsonObj = new HistoryJsonObj();
            historyRecord.HistoryJsonObj.Fields = fieldsList.ToArray();
        }
    }
}