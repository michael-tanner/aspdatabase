using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DataObjects.Nav
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class NavDatabaseInfo
    {
        public int ConnectionId;
        public string ConnectionName;
        public NavSectionInfo Section_Tables;
        //public NavSectionInfo Section_AppViews;
        public NavSectionInfo Section_Views;

        public bool IsOpen = false;
    }
}