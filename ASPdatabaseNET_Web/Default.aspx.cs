using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ASPdb.UniversalADO;
using System.Data.SqlClient;
using System.Data;

namespace ASPdatabaseNET_Web
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("ASPdatabase.NET.aspx");
        }
    }
}