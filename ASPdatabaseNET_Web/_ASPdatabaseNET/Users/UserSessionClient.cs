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
    public class UserSessionClient : jQueryContext
    {
        public UserInfo UserInfo;
        public bool IsLoggedIn;
        public bool IsAdmin;

        public bool Impersonation_IsAllowed = false;
        public bool Impersonation_IsOn = false;
        public UserInfo Impersonation_ActualUser = null;



        //------------------------------------------------------------------------------------------ Static --
        public static void Set(UserSessionClient userSessionClient)
        {
            var value = userSessionClient;
            eval("document.ASPdatabaseNET_Users_UserSessionClient = value;");
        }
        //------------------------------------------------------------------------------------------ Static --
        public static UserSessionClient Get()
        {
            try
            {
                UserSessionClient rtn = null;
                eval("rtn = document.ASPdatabaseNET_Users_UserSessionClient;");
                return rtn;
            }
            catch { }
            return null;
        }
    }
}