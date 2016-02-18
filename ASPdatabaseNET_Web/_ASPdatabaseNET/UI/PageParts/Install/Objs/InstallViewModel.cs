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
    public class InstallViewModel
    {
        public bool LegalHasBeenAccepted = false;
        public bool IsFirstAttempt_AddingConnectionString;
    }
}