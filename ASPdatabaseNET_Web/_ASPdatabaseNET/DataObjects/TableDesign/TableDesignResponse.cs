using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdatabaseNET.DbInterfaces.TableObjects;
using ASPdatabaseNET.DataObjects.SQLObjects;

namespace ASPdatabaseNET.DataObjects.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TableDesignResponse
    {
        public enum ResponseTypesEnum
        {
            NotSet,
            GetInfo_ForCreateNew,
            GetInfo_ForModify,
            CreateTable,
            UpdateTable,
            DeleteTable
        };

        public ResponseTypesEnum ResponseType = ResponseTypesEnum.NotSet;

        public int ConnectionId;
        public string ConnectionName;

        public string Schema;
        public int TableId;
        public string TableName;

        public string[] Schemas;
        public ASPdb_Table[] AllTables_InDb;

        public TableStructure TableStructure;

    }
}