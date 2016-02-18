using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.About.Objs;

namespace ASPdatabaseNET.UI.PageParts.About.Backend
{
    public class AboutPageLogic
    {
        public static AboutPageInfo Get()
        {
            return new AboutPageInfo()
            {
                SubscriptionAgreement = UI.PagesFramework.DefaultAspxPage.Return_SubscriptionAgreement(false),
                CompanyName = Config.SystemProperties.CompanyName,
                CopyrightYear = Config.SystemProperties.CopyrightYear,
                Version = Config.SystemProperties.Version,
                VersionDate = Config.SystemProperties.VersionDate,
                CopyrightLine = Config.SystemProperties.CopyrightLine
            };
        }
    }
}