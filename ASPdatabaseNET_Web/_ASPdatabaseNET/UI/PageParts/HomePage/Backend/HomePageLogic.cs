using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.HomePage.Objs;

namespace ASPdatabaseNET.UI.PageParts.HomePage.Backend
{
    public class HomePageLogic
    {
        public static string SessionKey = "ASPdatabaseNET.UI.PageParts.HomePage.Backend.HomePageLogic.HomeHTML";


        public static HomePageInfo GetInfo()
        {
            AjaxService.ASPdatabaseService.GetSetVal();

            bool isAdmin = ASPdatabaseNET.Users.UserSessionLogic.GetUser().UserInfo.IsAdmin;

            var rtn = new HomePageInfo()
            {
                Version = Config.SystemProperties.Version,
                CopyrightLine = Config.SystemProperties.CopyrightLine,
                UserIsAdmin = isAdmin
            };
            rtn.FirstName = ASPdatabaseNET.Users.UserSessionLogic.GetUser().UserInfo.FirstName;

            string html = null;
            try
            {
                html = (string)HttpContext.Current.Session[SessionKey];
            }
            catch { }

            if(String.IsNullOrEmpty(html))
            {
                try
                {
                    var siteId = ASPdatabaseNET.Subscription.Objs.SiteIdObj.GetNew();
                    string appVersion = Config.SystemProperties.Version;
                    var checkAppVersionResponse = (new ASPdatabaseNET.AjaxService.ASPdatabaseService()).SubscriptionService__CheckAppVersion(siteId, appVersion);
                    html = checkAppVersionResponse.HomeHTML;
                    HttpContext.Current.Session[SessionKey] = html;
                }
                catch { }
            }
            rtn.HomeHTML = html;

            return rtn;
        }
    }
}