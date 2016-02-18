using System;

namespace ASPdatabaseNET_Web
{
    public partial class ASPdatabase_NET : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            (new ASPdatabaseNET.UI.PagesFramework.DefaultAspxPage()).Run();
        }
    }
}