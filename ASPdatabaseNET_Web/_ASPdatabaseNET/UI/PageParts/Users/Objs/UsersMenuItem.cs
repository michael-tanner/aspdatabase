using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.Users.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class UsersMenuItem
    {
        public enum MenuTypes { NotSet, User, Group };

        public MenuTypes MenuType = MenuTypes.NotSet;
        public int Id = -1;
        public string DisplayName;
        public bool Active;
    }
}