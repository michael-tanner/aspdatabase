using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.MyAccount.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class MyAccountInfo
    {
        public int UserId;
        public string Username;
        public string FirstName;
        public string LastName;
        public string Email;
    }
}