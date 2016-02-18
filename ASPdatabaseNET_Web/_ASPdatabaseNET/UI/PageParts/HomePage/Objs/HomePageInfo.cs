using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.UI.PageParts.HomePage.Objs
{
    public class HomePageInfo
    {
        public string CopyrightLine;
        public string Version;
        public string FirstName;
        public string HomeHTML;
        public bool HideTechnicalInfo_ForNonAdmins = false;
        public bool UserIsAdmin = false;
    }
}