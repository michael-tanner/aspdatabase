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
    public class FieldValue
    {
        public int Index;
        public string Value;
        public bool IsNull = false;
    }
}