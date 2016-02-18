using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.Record.Objs_History
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Item
    {
        public string cn; // ColumnName
        public bool match = false;
        public string v1; // Value1 --- Before Change
        public string v2; // Value2 --- After Change
    }
}