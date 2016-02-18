using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.Users.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class UsersViewModel
    {
        public int CurrentId = -1;

        public bool SaveJustHappened = false;
        public int CurrentTabId = -1;
        public MenuItemUI LastMenuItem = null;
    }
}