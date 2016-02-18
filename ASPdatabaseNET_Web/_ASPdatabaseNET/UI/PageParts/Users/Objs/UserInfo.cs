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
    public class UserInfo
    {
        public int UserId;
        public string Username;
        public string FirstName;
        public string LastName;
        public string Email;
        public bool Active;
        public string TimeCreated_Str;
        public string LastLoginTime_Str;
        public bool RequirePasswordReset;
        public bool IsAdmin;
        public UserToGroup_Assignment[] UserGroups;
    }
}