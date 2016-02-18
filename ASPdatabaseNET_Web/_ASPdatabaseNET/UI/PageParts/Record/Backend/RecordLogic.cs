using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.Record.Objs;
using ASPdb.UniversalADO;

namespace ASPdatabaseNET.UI.PageParts.Record.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class RecordLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static RecordInfo Get(string uniqueRowKey)
        {
            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            var usersPermissions = userSessionInfo.UserInfo.AllPermissions;

            var uniqueRowObj = UI.TableGrid.Objs.UniqueRowKey.GetFrom_Base64Json(uniqueRowKey);
            if (!uniqueRowObj.IsValid)
                throw new Exception("Invalid Key");
            if (uniqueRowObj.TableType != TableGrid.Objs.GridRequest.TableTypes.Table)
                throw new Exception("TableType not supported.");
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(uniqueRowObj.Id, false, true, false);
            var aspdb_Connection = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(tableStructure.ConnectionId);
            if (!aspdb_Connection.Active)
                throw new Exception("This connection is inactive.");

            var rtn = new RecordInfo()
            {
                TableType = uniqueRowObj.TableType,
                ConnectionId = tableStructure.ConnectionId,
                Id = uniqueRowObj.Id,
                Schema = tableStructure.Schema,
                TableName = tableStructure.TableName,
                Columns = tableStructure.Columns,
                ActionType = uniqueRowObj.ActionType,
                UniqueRowObj = uniqueRowObj,
                PermissionValues = usersPermissions.CheckPermissions(tableStructure.ConnectionId, tableStructure.Schema, tableStructure.TableId),
                ChangeHistory_IsOn = UI.PageParts.Record.Backend.HistoryLogic.DoSaveHistory
            };
            if (rtn.PermissionValues.View == false)
                throw new Exception("You do not have permission to view this record.");

            var tmp_PrimaryKeyNames = new List<string>();
            var tmp_PriamryKeyIndexPositions = new List<int>();
            for (int i = 0; i < rtn.Columns.Length; i++)
                if (rtn.Columns[i].IsPrimaryKey)
                {
                    tmp_PrimaryKeyNames.Add(rtn.Columns[i].ColumnName);
                    tmp_PriamryKeyIndexPositions.Add(i);
                }
            rtn.PrimaryKeyNames = tmp_PrimaryKeyNames.ToArray();
            rtn.PriamryKeyIndexPositions = tmp_PriamryKeyIndexPositions.ToArray();

            if(uniqueRowObj.ActionType == TableGrid.Objs.UniqueRowKey.ActionTypes.New)
            {
                rtn.FieldValues = new FieldValue[rtn.Columns.Length];
                for (int i = 0; i < rtn.Columns.Length; i++)
                {
                    var fieldValue = new FieldValue() { Index = i };

                    string defaultValue = rtn.Columns[i].DefaultValue;
                    if (defaultValue != null)
                        if (defaultValue.StartsWith("'") && defaultValue.EndsWith("'"))
                            defaultValue = defaultValue.Substring(1, defaultValue.Length - 2);

                    if (rtn.Columns[i].DataType_Name == "bit")
                        if (defaultValue != null)
                            if (defaultValue == "1" || defaultValue.ToLower() == "true")
                                defaultValue = "true";

                    if (defaultValue != null)
                    {
                        fieldValue.Value = defaultValue;
                        fieldValue.IsNull = false;
                    }
                    else
                    {
                        fieldValue.Value = "";
                        fieldValue.IsNull = true;
                    }
                    rtn.FieldValues[i] = fieldValue;
                }


            }
            else if (uniqueRowObj.ActionType == TableGrid.Objs.UniqueRowKey.ActionTypes.Clone)
            {

            }
            else
            {
                if (rtn.PrimaryKeyNames.Length != uniqueRowObj.Values.Length)
                    throw new Exception("Invalid PrimaryKey.");
                string tmpWhere = "";
                var sqlParameters = new List<object[]>();
                int p = 1;
                for (int i = 0; i < uniqueRowObj.Values.Length; i++)
                {
                    if (tmpWhere != "")
                        tmpWhere += " and ";
                    tmpWhere += String.Format(" [{0}] = @Param{1} ", rtn.PrimaryKeyNames[i], p);
                    sqlParameters.Add(new object[] { "@Param" + p, uniqueRowObj.Values[i] });
                    p++;
                }
                string sql = String.Format(@"
                select * from [{0}].[{1}]
                where {2}
            ", tableStructure.Schema, tableStructure.TableName, tmpWhere);

                rtn.FieldValues = new FieldValue[rtn.Columns.Length];
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
                {
                    foreach (object[] parameter in sqlParameters)
                        command.AddParameter(parameter[0].ToString(), parameter[1]);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        if(reader.Read())
                        {
                            for (int i = 0; i < rtn.Columns.Length; i++)
                            {
                                var fieldValue = new FieldValue() { Index = i };
                                fieldValue.Value = reader.GetString_OrNullDefault(rtn.Columns[i].ColumnName);
                                if (fieldValue.Value == null)
                                {
                                    fieldValue.Value = "";
                                    fieldValue.IsNull = true;
                                }
                                rtn.FieldValues[i] = fieldValue;
                            }
                        }
                        else
                        {
                            //for (int i = 0; i < rtn.Columns.Length; i++)
                            //    rtn.FieldValues[i] = new FieldValue() { Index = 1, Value = "", IsNull = true };
                            throw new Exception("\nData needs to be refreshed. \nTherefore the app will now return to the Records List.");
                        }
                    }
                }
            }

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static RecordInfo GetClone(string uniqueRowKey)
        {
            var rtn = Get(uniqueRowKey);
            rtn.ActionType = TableGrid.Objs.UniqueRowKey.ActionTypes.Clone;

            for (int i = 0; i < rtn.Columns.Length; i++)
                if (rtn.Columns[i].IsIdentity)
                    rtn.FieldValues[i].Value = "";

            return rtn;
        }

    
        //----------------------------------------------------------------------------------------------------
        public static string Save__OLD(RecordInfo recordInfo)
        {
            if (recordInfo.UniqueRowObj.TableType != TableGrid.Objs.GridRequest.TableTypes.Table)
                throw new Exception("TableType not supported.");
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(recordInfo.UniqueRowObj.Id, false, true, false);

            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            var permission = userSessionInfo.UserInfo.AllPermissions.CheckPermissions(tableStructure.ConnectionId, tableStructure.Schema, tableStructure.TableId);

            if (recordInfo.FieldValues == null || recordInfo.FieldValues.Length < 1)
                throw new Exception("Nothing to save.\n\nPlease edit a field before saving.");

            string sql = "";
            var sqlParameters = new List<object[]>();
            int p = 1;
            bool doUpdate = (recordInfo.ActionType == TableGrid.Objs.UniqueRowKey.ActionTypes.Get);

            if(doUpdate) // -- update
            {
                if (!permission.Edit)
                    throw new Exception("You do not have edit permission on this table.");

                string set = "", where = "";
                for (int i = 0; i < recordInfo.FieldValues.Length; i++)
                {
                    var column = recordInfo.Columns[recordInfo.FieldValues[i].Index];
                    if(!column.IsIdentity)
                    {
                        if (set != "")
                            set += ", ";
                        set += String.Format("[{0}] = @Param{1}", column.ColumnName, p);

                        object value = recordInfo.FieldValues[i].Value;
                        if (recordInfo.FieldValues[i].IsNull || value == null)
                            value = DBNull.Value;
                        sqlParameters.Add(new object[] { "@Param" + p, value });
                        p++;
                    }
                }
                for (int i = 0; i < recordInfo.UniqueRowObj.Values.Length; i++)
                {
                    string primaryKeyName = tableStructure.PrimaryKey.Columns[i].ColumnName;
                    if (where != "")
                        where += " and ";
                    where += String.Format(" ([{0}] = @Param{1} ) ", primaryKeyName, p);
                    sqlParameters.Add(new object[] { "@Param" + p, recordInfo.UniqueRowObj.Values[i] });
                    p++;
                }
                sql = String.Format(@"
                update [{0}].[{1}]
                set {2}
                where {3}
                ",
                    tableStructure.Schema, tableStructure.TableName,
                    set,
                    where);
            }
            else // -- insert new
            {
                if (!permission.Insert)
                    throw new Exception("You do not have insert permission on this table.");

                string columnsSQL = "", valuesSQL = "";

                for (int i = 0; i < recordInfo.FieldValues.Length; i++)
                {
                    var column = recordInfo.Columns[recordInfo.FieldValues[i].Index];
                    if (!column.IsIdentity)
                    {
                        if(columnsSQL != "")
                        {
                            columnsSQL += ", ";
                            valuesSQL += ", ";
                        }
                        columnsSQL += "[" + column.ColumnName + "]";
                        valuesSQL += "@Param" + p;
                        object value = recordInfo.FieldValues[i].Value;
                        if (recordInfo.FieldValues[i].IsNull || value == null)
                            value = DBNull.Value;
                        sqlParameters.Add(new object[] { "@Param" + p, value });
                        p++;
                    }
                }
                sql = String.Format(@"
                insert into [{0}].[{1}]
                ( {2} )
                values
                ( {3} )
                ",
                    tableStructure.Schema, tableStructure.TableName,
                    columnsSQL,
                    valuesSQL);
            }
            //ASPdb.Framework.Debug.WriteLine(sql);


            UI.PageParts.Record.Objs_History.HistoryRecord historyRecord1 = null;
            UI.PageParts.Record.Objs_History.HistoryRecord historyRecord2 = null;
            var keyValue = recordInfo.UniqueRowObj.Values;
            if (doUpdate)
            {
                historyRecord1 = new Objs_History.HistoryRecord()
                {
                    HistoryId = -1,
                    TableId = tableStructure.TableId,
                    KeyValue = keyValue,
                    HistoryType = PageParts.Record.Objs_History.HistoryRecord.HistoryTypes.Update,
                    IsPartial = false
                };
                HistoryLogic.Initialize_HistoryRecord_AllFields(historyRecord1, "Left");
            }

            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                foreach (object[] parameter in sqlParameters)
                    command.AddParameter(parameter[0].ToString(), parameter[1]);
                command.ExecuteNonQuery();
            }

            if (doUpdate)
            {
                historyRecord2 = new Objs_History.HistoryRecord()
                {
                    HistoryId = -1,
                    TableId = tableStructure.TableId,
                    KeyValue = keyValue,
                    HistoryType = PageParts.Record.Objs_History.HistoryRecord.HistoryTypes.Update,
                    IsPartial = false
                };
                HistoryLogic.Initialize_HistoryRecord_AllFields(historyRecord2, "Right");

                var newValuesDict = new Dictionary<string, Objs_History.Item>();
                foreach (var item in historyRecord2.HistoryJsonObj.Fields)
                    if (!newValuesDict.ContainsKey(item.cn))
                        newValuesDict.Add(item.cn, item);

                int historyCount = HistoryLogic.Get_HistoryCount(tableStructure.TableId, keyValue);
                historyRecord1.IsPartial = (historyCount > 0);

                var fieldsList = new List<Objs_History.Item>();
                foreach(var item1 in historyRecord1.HistoryJsonObj.Fields)
                {
                    var item2 = new Objs_History.Item();
                    if (newValuesDict.ContainsKey(item1.cn))
                        item2 = newValuesDict[item1.cn];
                    item1.v2 = item2.v2;
                    if(item1.v1 == item1.v2)
                    {
                        item1.match = true;
                        item1.v2 = null;
                    }
                    if (historyRecord1.IsPartial == false || item1.match == false)
                        fieldsList.Add(item1);
                }
                historyRecord1.HistoryJsonObj.Fields = fieldsList.ToArray();
                HistoryLogic.Save_HistoryRecord(historyRecord1);
            }





            if(recordInfo.UniqueRowObj.ActionType != TableGrid.Objs.UniqueRowKey.ActionTypes.Get)
                return "";
            else
                return recordInfo.UniqueRowObj.To_Base64Json();
        }



        //----------------------------------------------------------------------------------------------------
        public static string Save(RecordInfo recordInfo)
        {
            if (recordInfo.UniqueRowObj.TableType != TableGrid.Objs.GridRequest.TableTypes.Table)
                throw new Exception("TableType not supported.");
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(recordInfo.UniqueRowObj.Id, false, true, false);

            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            var permission = userSessionInfo.UserInfo.AllPermissions.CheckPermissions(tableStructure.ConnectionId, tableStructure.Schema, tableStructure.TableId);

            if (recordInfo.FieldValues == null || recordInfo.FieldValues.Length < 1)
                throw new Exception("Nothing to save.\n\nPlease edit a field before saving.");

            string sql = "";
            var sqlParameters = new List<object[]>();
            int p = 1;
            bool doUpdate = (recordInfo.ActionType == TableGrid.Objs.UniqueRowKey.ActionTypes.Get);

            if (doUpdate) // -- update -----------------------------------------------------------------------
            {
                if (!permission.Edit)
                    throw new Exception("You do not have edit permission on this table.");

                string set = "", where = "";
                for (int i = 0; i < recordInfo.FieldValues.Length; i++)
                {
                    var column = recordInfo.Columns[recordInfo.FieldValues[i].Index];
                    if (!column.IsIdentity)
                    {
                        if (set != "")
                            set += ", ";
                        set += String.Format("[{0}] = @Param{1}", column.ColumnName, p);

                        object value = recordInfo.FieldValues[i].Value;
                        if (recordInfo.FieldValues[i].IsNull || value == null)
                            value = DBNull.Value;
                        sqlParameters.Add(new object[] { "@Param" + p, value });
                        p++;
                    }
                }
                for (int i = 0; i < recordInfo.UniqueRowObj.Values.Length; i++)
                {
                    string primaryKeyName = tableStructure.PrimaryKey.Columns[i].ColumnName;
                    if (where != "")
                        where += " and ";
                    where += String.Format(" ([{0}] = @Param{1} ) ", primaryKeyName, p);
                    sqlParameters.Add(new object[] { "@Param" + p, recordInfo.UniqueRowObj.Values[i] });
                    p++;
                }
                sql = String.Format(@"
                update [{0}].[{1}]
                set {2}
                where {3}
                ",
                    tableStructure.Schema, tableStructure.TableName,
                    set,
                    where);


                Objs_History.HistoryRecord historyRecord1 = null;
                Objs_History.HistoryRecord historyRecord2 = null;
                var keyValue = recordInfo.UniqueRowObj.Values;
                historyRecord1 = new Objs_History.HistoryRecord()
                {
                    HistoryId = -1,
                    TableId = tableStructure.TableId,
                    KeyValue = keyValue,
                    HistoryType = PageParts.Record.Objs_History.HistoryRecord.HistoryTypes.Update,
                    IsPartial = false
                };
                HistoryLogic.Initialize_HistoryRecord_AllFields(historyRecord1, "Left");


                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql)) // Do Update
                {
                    foreach (object[] parameter in sqlParameters)
                        command.AddParameter(parameter[0].ToString(), parameter[1]);
                    command.ExecuteNonQuery();
                }

                historyRecord2 = new Objs_History.HistoryRecord()
                {
                    HistoryId = -1,
                    TableId = tableStructure.TableId,
                    KeyValue = keyValue,
                    HistoryType = PageParts.Record.Objs_History.HistoryRecord.HistoryTypes.Update,
                    IsPartial = false
                };
                HistoryLogic.Initialize_HistoryRecord_AllFields(historyRecord2, "Right");

                var newValuesDict = new Dictionary<string, Objs_History.Item>();
                foreach (var item in historyRecord2.HistoryJsonObj.Fields)
                    if (!newValuesDict.ContainsKey(item.cn))
                        newValuesDict.Add(item.cn, item);

                int historyCount = HistoryLogic.Get_HistoryCount(tableStructure.TableId, keyValue);
                historyRecord1.IsPartial = (historyCount > 0);

                var fieldsList = new List<Objs_History.Item>();
                foreach (var item1 in historyRecord1.HistoryJsonObj.Fields)
                {
                    var item2 = new Objs_History.Item();
                    if (newValuesDict.ContainsKey(item1.cn))
                        item2 = newValuesDict[item1.cn];
                    item1.v2 = item2.v2;
                    if (item1.v1 == item1.v2)
                    {
                        item1.match = true;
                        item1.v2 = null;
                    }
                    if (historyRecord1.IsPartial == false || item1.match == false)
                        fieldsList.Add(item1);
                }
                historyRecord1.HistoryJsonObj.Fields = fieldsList.ToArray();
                HistoryLogic.Save_HistoryRecord(historyRecord1);


            }
            else // -- insert new ----------------------------------------------------------------------------
            {
                if (!permission.Insert)
                    throw new Exception("You do not have insert permission on this table.");

                string columnsSQL = "", valuesSQL = "";

                for (int i = 0; i < recordInfo.FieldValues.Length; i++)
                {
                    var column = recordInfo.Columns[recordInfo.FieldValues[i].Index];
                    if (!column.IsIdentity)
                    {
                        if (columnsSQL != "")
                        {
                            columnsSQL += ", ";
                            valuesSQL += ", ";
                        }
                        columnsSQL += "[" + column.ColumnName + "]";
                        valuesSQL += "@Param" + p;
                        object value = recordInfo.FieldValues[i].Value;
                        if (recordInfo.FieldValues[i].IsNull || value == null)
                            value = DBNull.Value;
                        sqlParameters.Add(new object[] { "@Param" + p, value });
                        p++;
                    }
                }
                sql = String.Format(@"
                insert into [{0}].[{1}]
                ( {2} )
                OUTPUT inserted.*
                values
                ( {3} )
                ",
                    tableStructure.Schema, tableStructure.TableName,
                    columnsSQL,
                    valuesSQL);
                var keyValue = new string[0];
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql)) // Do Insert
                {
                    foreach (object[] parameter in sqlParameters)
                        command.AddParameter(parameter[0].ToString(), parameter[1]);
                    //command.ExecuteNonQuery();
                    using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        if (reader.Read())
                        {
                            var keyValue_List = new List<string>();
                            foreach (var item in tableStructure.PrimaryKey.Columns)
                            {
                                keyValue_List.Add(reader.Get(item.ColumnName, ""));
                            }
                            keyValue = keyValue_List.ToArray();
                        }
                }
                if(keyValue.Length > 0)
                {
                    var historyRecord = new Objs_History.HistoryRecord()
                    {
                        HistoryId = -1,
                        TableId = tableStructure.TableId,
                        KeyValue = keyValue,
                        HistoryType = PageParts.Record.Objs_History.HistoryRecord.HistoryTypes.Insert,
                        IsPartial = false
                    };
                    HistoryLogic.Initialize_HistoryRecord_AllFields(historyRecord, "Right");
                    HistoryLogic.Save_HistoryRecord(historyRecord);
                }
            }
            //ASPdb.Framework.Debug.WriteLine(sql);


            if (doUpdate)
                return recordInfo.UniqueRowObj.To_Base64Json();
            else
                return "";
        }
    
    }
}