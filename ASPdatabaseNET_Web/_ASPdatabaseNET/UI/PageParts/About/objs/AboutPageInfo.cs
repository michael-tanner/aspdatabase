using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.About.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AboutPageInfo
    {
        public string SubscriptionAgreement;
        public string CompanyName;
        public string CopyrightYear;
        public string Version;
        public string VersionDate;
        public string CopyrightLine;
    }
}