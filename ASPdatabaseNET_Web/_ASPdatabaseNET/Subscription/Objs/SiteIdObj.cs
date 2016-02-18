using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.Subscription.Objs
{
    public class SiteIdObj
    {
        public string DomainName;
        public string AppURL;
        public string IPLocal;

        public static SiteIdObj GetNew()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            return new SiteIdObj()
            {
                IPLocal = HttpContext.Current.Request.UserHostAddress,
                DomainName = HttpContext.Current.Request.Url.Host.ToLower(),
                AppURL = HttpContext.Current.Request.Url.AbsoluteUri
            };
        }
    }
}