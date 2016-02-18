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
    public class GroupInfo
    {
        public int GroupId;
        public string GroupName;
        public bool Active;
        public string TimeCreated_Str;

        public Permission[] Permissions;
    }
}