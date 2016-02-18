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
    public class UsersMenuInfo
    {
        public int UserSubscriptions_Total = 0;
        public int UserSubscriptions_Active = 0;
        public string UserSubscriptions_Message = "";

        public UsersMenuItem[] Users_Active;
        public UsersMenuItem[] Users_Inactive;
        public UsersMenuItem[] Groups_Active;
        public UsersMenuItem[] Groups_Inactive;
    }
}