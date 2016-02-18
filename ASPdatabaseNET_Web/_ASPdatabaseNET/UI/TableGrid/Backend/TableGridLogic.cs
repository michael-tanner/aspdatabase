using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.TableGrid.Objs;
using ASPdb.UniversalADO;

namespace ASPdatabaseNET.UI.TableGrid.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class TableGridLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static GridResponse GetGrid(GridRequest gridRequest, bool truncateValues)
        {
            GridResponse rtn = null;
            switch (gridRequest.TableType)
            {
                case GridRequest.TableTypes.Table: rtn = Load_Table(gridRequest, truncateValues, false); break;
                case GridRequest.TableTypes.View: rtn = Load_Table(gridRequest, false, true); break;
            }
            if (rtn != null)
                rtn.IsInDemoMode = Users.UserSessionLogic.IsInDemoMode;

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static GridResponse Load_Table(GridRequest gridRequest, bool truncateValues, bool isAView)
        {
            var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
            var usersPermissions = userSessionInfo.UserInfo.AllPermissions;

            DbInterfaces.TableObjects.TableStructure tableStructure = null;
            if(!isAView)
                tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(gridRequest.Id, false, true, false);
            else tableStructure = DbInterfaces.SQLServerInterface.Views__Get(gridRequest.Id, true, false);
            

            var aspdb_Connection = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(tableStructure.ConnectionId);
            if (!aspdb_Connection.Active)
                throw new Exception("This connection is inactive.");

            var uniqueRowKey_ForNew = new UniqueRowKey() { TableType = GridRequest.TableTypes.Table, Id = tableStructure.TableId, ActionType = UniqueRowKey.ActionTypes.New };
            var rtn = new GridResponse()
            {
                TableType = GridRequest.TableTypes.Table,
                ConnectionId = tableStructure.ConnectionId,
                Id = gridRequest.Id,
                TableName = tableStructure.TableName,
                Schema = tableStructure.Schema,
                UniqueKey_ForNewRecord = uniqueRowKey_ForNew.To_Base64Json(),
                PermissionValues = usersPermissions.CheckPermissions(tableStructure.ConnectionId, tableStructure.Schema, tableStructure.TableId),
                IsAdmin = usersPermissions.IsAdmin
            };
            if (isAView)
                rtn.TableType = GridRequest.TableTypes.View;
            if (rtn.PermissionValues.View == false)
                throw new Exception("You do not have permission to view this table/view.");

            Helper1__PopulateAdditionalTableName(rtn);

            //var columnsDict = new Dictionary<string, DbInterfaces.TableObjects.Column>();
            //foreach (var item in tableStructure.Columns)
            //    if (!columnsDict.ContainsKey(item.ColumnName.Trim().ToLower()))
            //        columnsDict.Add(item.ColumnName.Trim().ToLower(), item);

            var primaryKeyNames_L = new string[0];
            if (tableStructure.PrimaryKey != null)
                primaryKeyNames_L = (from c in tableStructure.PrimaryKey.Columns select c.ColumnName.Trim().ToLower()).ToArray();

            var columnNames_L = (from c in tableStructure.Columns select c.ColumnName.Trim().ToLower()).ToArray();
            List<object[]> sqlParameters;
            string sqlPart1, sqlPart2_OrderBy;
            Load_Table__BuildBottomSQL(gridRequest, rtn, true, columnNames_L, out sqlParameters, out sqlPart1, out sqlPart2_OrderBy);

            string top = String.Format(" top {0} ", gridRequest.DisplayTopNRows);
            if (gridRequest.DisplayTopNRows < 1)
                top = " top 100 ";
            if (gridRequest.DisplayTopNRows == -2)
                top = " ";


            string sql_Select = String.Format(@"
                select {0} 
                * 
                {1}
                {2}
                "
                , top
                , sqlPart1
                , sqlPart2_OrderBy);

            string sql_TotalCount = String.Format(@"
                select count(*) as [Count1] 
                {0}
                "
                , sqlPart1);
            
            
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(rtn.ConnectionId, sql_Select))
            {
                foreach (var param in sqlParameters)
                    command.AddParameter(param[0].ToString(), param[1]);

                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    // build header
                    var headersList = new List<GridHeaderItem>();
                    var tmp1 = new List<string>(); var tmp2 = new List<int>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var headerItem = new GridHeaderItem();
                        headerItem.IndexPosition = i;
                        headerItem.FieldName = reader.GetName(i);
                        headerItem.DataTypeName = reader.GetDataTypeName(i);
                        headersList.Add(headerItem);

                        string key_L = headerItem.FieldName.Trim().ToLower();
                        if (primaryKeyNames_L.Contains(key_L))
                        {
                            headerItem.IsPrimaryKey = true;
                            tmp1.Add(headerItem.FieldName);
                            tmp2.Add(headerItem.IndexPosition);
                        }
                    }
                    rtn.HeaderItems = headersList.ToArray();
                    rtn.PrimaryKeyNames = tmp1.ToArray();
                    rtn.PriamryKeyIndexPositions = tmp2.ToArray();


                    int truncateLength = 100;
                    var rowsList = new List<GridRow>();
                    while (reader.Read())
                    {
                        var row = new GridRow();
                        row.Values = new string[rtn.HeaderItems.Length];
                        for (int i = 0; i < rtn.HeaderItems.Length; i++)
                        {
                            string value = reader.Get(i, "");
                            value = HttpContext.Current.Server.HtmlEncode(value);
                            if (truncateValues)
                                if (value.Length > truncateLength + 1)
                                {
                                    value = value.Substring(0, truncateLength);
                                    var chopLength = value.Split(new char[] { ' ' }).Last().Length;
                                    if(chopLength < 21)
                                        value = value.Substring(0, value.Length - chopLength) + "...";
                                    else value += " ...";
                                }
                            row.Values[i] = value;
                        }
                        var uniqueRowKey = Get_UniqueRowKey("t", rtn.Id, row, rtn.PriamryKeyIndexPositions);
                        row.UniqueKey = uniqueRowKey.To_Base64Json();
                        rowsList.Add(row);
                    }
                    rtn.Rows = rowsList.ToArray();

                    rtn.Count_DisplayItems = rtn.Rows.Length;
                }
            }
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(rtn.ConnectionId, sql_TotalCount))
            {
                foreach (var param in sqlParameters)
                    command.AddParameter(param[0].ToString(), param[1]);
                using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    if (reader.Read())
                        rtn.Count_TotalItems = reader.Get("Count1", -1);
                }
            }


            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private static void Load_Table__BuildBottomSQL(GridRequest gridRequest, GridResponse gridResponse, bool includeOrderBy, string[] columnNames_L
            , out List<object[]> sqlParameters, out string sqlPart1, out string sqlPart2_OrderBy)
        {
            sqlParameters = new List<object[]>();
            sqlPart1 = "";
            sqlPart2_OrderBy = "";

            string where = "";
            string orderby = "";

            var whereStatements = new List<string>();
            int p = 1;
            if(gridRequest.FilterFields != null)
                foreach(var filterField in gridRequest.FilterFields)
                    if (!String.IsNullOrEmpty(filterField.FieldName) && filterField.FilterType != ViewOptions_FilterField.FilterTypes.NotSet && !String.IsNullOrEmpty(filterField.Value))
                    {
                        if (!columnNames_L.Contains(filterField.FieldName.Trim().ToLower()))
                            throw new Exception("Invalid FilterField.");
                        switch(filterField.FilterType)
                        {
                            case ViewOptions_FilterField.FilterTypes.Equals:
                                whereStatements.Add(String.Format(" [{0}] = @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, filterField.Value });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.Not:
                                whereStatements.Add(String.Format(" [{0}] <> @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, filterField.Value });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.Contains:
                                whereStatements.Add(String.Format(" [{0}] like @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, "%" + filterField.Value + "%" });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.StartsWith:
                                whereStatements.Add(String.Format(" [{0}] like @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, "" + filterField.Value + "%" });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.EndsWith:
                                whereStatements.Add(String.Format(" [{0}] like @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, "%" + filterField.Value + "" });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.LessThan:
                                whereStatements.Add(String.Format(" [{0}] < @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, filterField.Value });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.LessThanOrEqual:
                                whereStatements.Add(String.Format(" [{0}] <= @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, filterField.Value });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.GreaterThanOrEqual:
                                whereStatements.Add(String.Format(" [{0}] >= @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, filterField.Value });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.GreaterThan:
                                whereStatements.Add(String.Format(" [{0}] > @Param{1} ", filterField.FieldName, p));
                                sqlParameters.Add(new object[] { "@Param" + p, filterField.Value });
                                p++;
                                break;
                            case ViewOptions_FilterField.FilterTypes.In:
                                var arr1 = filterField.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr1 != null && arr1.Length > 0 && arr1[0].Trim().Length > 0)
                                {
                                    string sqlItems = "";
                                    foreach (var item in arr1)
                                        if (item.Trim().Length > 0)
                                        {
                                            if (sqlItems != "")
                                                sqlItems += ", ";
                                            sqlItems += "@Param" + p;
                                            sqlParameters.Add(new object[] { "@Param" + p, item.Trim() });
                                            p++;
                                        }
                                    whereStatements.Add(String.Format(" [{0}] in ({1}) ", filterField.FieldName, sqlItems));
                                }
                                break;
                            case ViewOptions_FilterField.FilterTypes.NotIn:
                                var arr2 = filterField.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (arr2 != null && arr2.Length > 0 && arr2[0].Trim().Length > 0)
                                {
                                    string sqlItems = "";
                                    foreach (var item in arr2)
                                        if (item.Trim().Length > 0)
                                        {
                                            if (sqlItems != "")
                                                sqlItems += ", ";
                                            sqlItems += "@Param" + p;
                                            sqlParameters.Add(new object[] { "@Param" + p, item.Trim() });
                                            p++;
                                        }
                                    whereStatements.Add(String.Format(" [{0}] not in ({1}) ", filterField.FieldName, sqlItems));
                                }
                                break;
                        }
                    }
            if(whereStatements.Count() > 0)
                foreach(var statement in whereStatements)
                {
                    if (where == "") where += " where ";
                    else where += " and ";
                    where += String.Format(" ({0}) ", statement);
                }


            if(includeOrderBy && gridRequest.SortFields != null)
                foreach(var sortField in gridRequest.SortFields)
                    if (!String.IsNullOrEmpty(sortField.FieldName))
                    {
                        if (!columnNames_L.Contains(sortField.FieldName.Trim().ToLower()))
                            throw new Exception("Invalid SortField.");
                        if (orderby == "") orderby += " order by "; else orderby += ", ";
                        orderby += "[" + sortField.FieldName + "]";
                        if (sortField.Descending)
                            orderby += " desc";
                    }

            sqlPart1 = String.Format(@"
                from {0}
                {1}
            "
            , gridResponse.TableName_FullSQLName
            , where);

            sqlPart2_OrderBy = orderby;
        }
        //----------------------------------------------------------------------------------------------------
        private static void Helper1__PopulateAdditionalTableName(GridResponse rtn)
        {
            rtn.TableName_FullSQLName = String.Format("[{0}].[{1}]", rtn.Schema, rtn.TableName);

            if (rtn.Schema.Contains(" ") && rtn.TableName.Contains(" "))
                rtn.TableName_FullNameLabel = String.Format("[{0}].[{1}]", rtn.Schema, rtn.TableName);
            else if (rtn.Schema.Contains(" "))
                rtn.TableName_FullNameLabel = String.Format("[{0}].{1}", rtn.Schema, rtn.TableName);
            else if (rtn.TableName.Contains(" "))
                rtn.TableName_FullNameLabel = String.Format("{0}.[{1}]", rtn.Schema, rtn.TableName);
            else
                rtn.TableName_FullNameLabel = String.Format("{0}.{1}", rtn.Schema, rtn.TableName);

            bool hideSchemaPrefix = false; // to be pulled from settings
            if (hideSchemaPrefix)
                rtn.TableName_FullNameLabel = rtn.TableName;
        }
        //----------------------------------------------------------------------------------------------------
        private static UniqueRowKey Get_UniqueRowKey(string type, int id, GridRow gridRow, int[] priamryKeyIndexPositions)
        {
            var rtn = new UniqueRowKey();

            rtn.TableType = GridRequest.TableTypes.Table;
            rtn.Id = id;
            rtn.Values = new string[priamryKeyIndexPositions.Length];
            for (int i = 0; i < rtn.Values.Length; i++)
                rtn.Values[i] = gridRow.Values[priamryKeyIndexPositions[i]];
            return rtn;
        }











        ////----------------------------------------------------------------------------------------------------
        //public static GridResponse Load_View(GridRequest gridRequest)
        //{
        //    var rtn = new GridResponse() { TableType = GridRequest.TableTypes.View };
            
        //    // TASK : Check permissions


        //    var userSessionInfo = ASPdatabaseNET.Users.UserSessionLogic.GetUser();
        //    var usersPermissions = userSessionInfo.UserInfo.AllPermissions;

        //    var tableStructure = DbInterfaces.SQLServerInterface.Views__Get(gridRequest.Id, true, false);
        //    var aspdb_Connection = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(tableStructure.ConnectionId);
        //    if (!aspdb_Connection.Active)
        //        throw new Exception("This connection is inactive.");



            
        //    return rtn;
        //}








        //----------------------------------------------------------------------------------------------------
        public static bool DeleteRecords(string[] keysToDelete)
        {
            var allPermissions = ASPdatabaseNET.Users.UserSessionLogic.GetUser().UserInfo.AllPermissions;

            var dict_ByTableId = new Dictionary<int, List<UniqueRowKey>>();
            //var uniqueRowKeys_List = new List<UniqueRowKey>();
            foreach (var keyString in keysToDelete)
            {
                var uniqueRowKey = UniqueRowKey.GetFrom_Base64Json(keyString);
                if (uniqueRowKey.IsValid && uniqueRowKey.ActionType != UniqueRowKey.ActionTypes.New)
                    if (uniqueRowKey.TableType == GridRequest.TableTypes.Table)
                    {
                        if (!dict_ByTableId.ContainsKey(uniqueRowKey.Id))
                            dict_ByTableId.Add(uniqueRowKey.Id, new List<UniqueRowKey>());
                        dict_ByTableId[uniqueRowKey.Id].Add(uniqueRowKey);
                        //uniqueRowKeys_List.Add(uniqueRowKey);
                    }
            }
            foreach(int tableId in dict_ByTableId.Keys)
            {
                var list = dict_ByTableId[tableId];
                if (list != null && list.Count() > 0)
                {
                    var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(tableId, false, false, false);
                    if (!allPermissions.CheckPermissions(tableStructure.ConnectionId, tableStructure.Schema, tableStructure.TableId).Delete)
                        throw new Exception("You do not have delete permission on this table.");

                    var primaryKeyNames = new string[0];
                    if (tableStructure.PrimaryKey != null)
                        primaryKeyNames = (from c in tableStructure.PrimaryKey.Columns select c.ColumnName.Trim()).ToArray();

                    var whereStatements = new List<string>();
                    var sqlParameters = new List<object[]>();
                    int p = 1;
                    foreach (var item in list)
                    {
                        if (primaryKeyNames.Length != item.Values.Length)
                            throw new Exception("Invalid PrimaryKey.");
                        string tmpWhere = "";
                        for (int i = 0; i < item.Values.Length; i++)
                        {
                            if (tmpWhere != "")
                                tmpWhere += " and ";
                            tmpWhere += String.Format(" [{0}] = @Param{1} ", primaryKeyNames[i], p);
                            sqlParameters.Add(new object[] { "@Param" + p, item.Values[i] });
                            p++;
                        }
                        whereStatements.Add(tmpWhere);
                    }
                    string sqlWhere = "";
                    foreach (string wherePart in whereStatements)
                    {
                        if (sqlWhere != "")
                            sqlWhere += " or ";
                        sqlWhere += "(" + wherePart + ")";
                    }
                    string sql = String.Format("delete from [{0}].[{1}] where {2}", tableStructure.Schema, tableStructure.TableName, sqlWhere);


                    // -- Save History:
                    foreach (int tmpTableId in dict_ByTableId.Keys)
                        foreach (var uniqueRowKey in dict_ByTableId[tmpTableId])
                        {
                            var historyRecord = new UI.PageParts.Record.Objs_History.HistoryRecord()
                            {
                                HistoryId = -1,
                                TableId = tmpTableId,
                                KeyValue = uniqueRowKey.Values,
                                HistoryType = PageParts.Record.Objs_History.HistoryRecord.HistoryTypes.Delete,
                                IsPartial = false
                            };
                            UI.PageParts.Record.Backend.HistoryLogic.Initialize_HistoryRecord_AllFields(historyRecord, "Left");
                            UI.PageParts.Record.Backend.HistoryLogic.Save_HistoryRecord(historyRecord);
                        }

                    //ASPdb.Framework.Debug.WriteLine(sql);
                    using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
                    {
                        foreach (object[] parameter in sqlParameters)
                            command.AddParameter(parameter[0].ToString(), parameter[1]);
                        command.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }
    }
}