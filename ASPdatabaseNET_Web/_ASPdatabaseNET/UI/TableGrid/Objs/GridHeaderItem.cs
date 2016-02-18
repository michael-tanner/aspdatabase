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
    public class GridHeaderItem
    {
        public enum DataTypes { NotSet, String_TextLine, String_TextBox, Int, OtherNumber, DateTime, Bool, Other };

        public string FieldName;
        public int IndexPosition;
        public DataTypes DataType = DataTypes.NotSet;
        public string DataTypeName = "NotSet";
        public bool IsPrimaryKey = false;
    }
}