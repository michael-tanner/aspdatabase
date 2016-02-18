using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AppPropertiesItem
    {
        public enum AppColumnTypes { Default = 1, DropdownList = 2 };

        public int Index;
        public string ColumnName;
        public string DataType_Name;
        public bool IsPrimaryKey;
        public bool IsIdentity;
        public AppColumnTypes AppColumnType = AppColumnTypes.Default;
        public string AppColumnType_Str;
        public string AdditionalInfo;
    }
}