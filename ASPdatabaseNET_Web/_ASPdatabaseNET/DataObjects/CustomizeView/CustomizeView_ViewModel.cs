using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DataObjects.CustomizeView
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class CustomizeView_ViewModel
    {
        public enum FilterOptionsModeEnums { NotSet, AdHocFilter, AppFiew };

        public FilterOptionsModeEnums Mode = FilterOptionsModeEnums.NotSet;

        //------------------------------------------------------------------------------------- Constructor --
        public CustomizeView_ViewModel()
        {
        }
    }
}