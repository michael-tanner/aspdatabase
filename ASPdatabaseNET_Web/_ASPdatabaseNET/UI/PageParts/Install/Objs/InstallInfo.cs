using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.Install.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class InstallInfo
    {
        public enum InstallStates { NotSet, LegalNotYetAccepted, NoConnectionString, CannotConnectToDB, DatabaseNotReady, Installed };

        public InstallStates InstallState = InstallStates.NotSet;
        public UI.PageParts.About.Objs.AboutPageInfo AboutPageInfo;
        public string ConnectionString = "";

        public string ResponseMsg;
    }
}