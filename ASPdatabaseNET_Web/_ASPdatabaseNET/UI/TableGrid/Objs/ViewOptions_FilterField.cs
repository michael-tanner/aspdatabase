using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.TableGrid.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ViewOptions_FilterField
    {
        public enum FilterTypes { NotSet, Equals, Contains, StartsWith, EndsWith, GreaterThan, LessThan, InBetween, In, NotIn, Not, GreaterThanOrEqual, LessThanOrEqual };

        public string FieldName;
        public FilterTypes FilterType = FilterTypes.NotSet;
        public string Value;
    }
}