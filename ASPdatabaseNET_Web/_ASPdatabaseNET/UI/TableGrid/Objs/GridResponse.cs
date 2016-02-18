using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.TableGrid.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class GridResponse
    {
        public GridRequest GridRequest;
        public GridRequest.TableTypes TableType = GridRequest.TableTypes.NotSet;
        public int ConnectionId;
        public int Id;
        public string Schema;
        public string TableName;
        public string TableName_FullNameLabel; // ex: "dbo.CustomersTable"
        public string TableName_FullSQLName;

        public int Count_DisplayItems = 0;
        public int Count_TotalItems = 0;

        public string[] PrimaryKeyNames;
        public int[] PriamryKeyIndexPositions;
        public GridHeaderItem[] HeaderItems;
        public GridRow[] Rows;

        public string UniqueKey_ForNewRecord;

        public bool IsAdmin;
        public Users.PermissionValues PermissionValues;

        public bool IsInDemoMode;
    }
}