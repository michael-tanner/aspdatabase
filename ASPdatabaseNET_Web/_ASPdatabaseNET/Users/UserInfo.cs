using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.Users
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    [Serializable()]
    public class UserInfo
    {
        public int UserId;
        public string Username;
        public string Email;
        public string FirstName;
        public string LastName;
        public bool Active;
        public bool IsAdmin;

        public AllPermissionsInfo AllPermissions;
    }
}