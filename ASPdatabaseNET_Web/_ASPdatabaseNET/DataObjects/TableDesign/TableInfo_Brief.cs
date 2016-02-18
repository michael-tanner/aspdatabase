using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdatabaseNET.DbInterfaces.TableObjects;

namespace ASPdatabaseNET.DataObjects.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TableInfo_Brief
    {
        public int ConnectiondId;
        public int TableId;
        public string Schema;
        public string TableName;
    }
}