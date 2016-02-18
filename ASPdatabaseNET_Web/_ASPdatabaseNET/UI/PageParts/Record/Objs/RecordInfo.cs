using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdatabaseNET.UI.TableGrid.Objs;

namespace ASPdatabaseNET.UI.PageParts.Record.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class RecordInfo
    {
        public UI.TableGrid.Objs.GridRequest.TableTypes TableType = TableGrid.Objs.GridRequest.TableTypes.NotSet;
        public UI.TableGrid.Objs.UniqueRowKey.ActionTypes ActionType = UniqueRowKey.ActionTypes.Get;
        public int ConnectionId;
        public int Id;
        public UniqueRowKey UniqueRowObj;
        public string Schema;
        public string TableName;

        public string[] PrimaryKeyNames;
        public int[] PriamryKeyIndexPositions;
        public DbInterfaces.TableObjects.Column[] Columns;

        public FieldValue[] FieldValues;

        public ASPdatabaseNET.Users.PermissionValues PermissionValues;

        public bool ChangeHistory_IsOn;

    }

}