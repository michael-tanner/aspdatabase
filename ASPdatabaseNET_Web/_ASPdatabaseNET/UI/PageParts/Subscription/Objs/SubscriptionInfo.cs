using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
namespace ASPdatabaseNET.UI.PageParts.Subscription.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class SubscriptionInfo
    {
        public string SubscriptionKey = "";
        public int SubscriptionCount = 0;
        public string LastCheck_MinutesLapsed = "";
    }
}