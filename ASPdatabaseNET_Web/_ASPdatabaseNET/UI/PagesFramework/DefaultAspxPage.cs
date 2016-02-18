using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Reflection;
using ASPdatabaseNET.AjaxService;

namespace ASPdatabaseNET.UI.PagesFramework
{
    //----------------------------------------------------------------------------------------------------////
    public class DefaultAspxPage
    {
        public string QueryString_JS
        {
            get { return HttpContext.Current.Request.QueryString["JS"]; }
        }
        public string QueryString_CSS
        {
            get { return HttpContext.Current.Request.QueryString["CSS"]; }
        }
        public string QueryString_IMG
        {
            get { return HttpContext.Current.Request.QueryString["IMG"]; }
        }
        public string QueryString_Page
        {
            get { return HttpContext.Current.Request.QueryString["Page"]; }
        }
        public bool QueryString_AjaxRequest
        {
            get
            {
                bool rtn = false;
                Boolean.TryParse(HttpContext.Current.Request.QueryString["AjaxRequest"], out rtn);
                return rtn;
            }
        }
        public bool QueryString_IsAgreementPage
        {
            get
            {
                string queryString_L = HttpContext.Current.Request.Url.Query.ToLower();
                if (queryString_L.StartsWith("?subscriptionagreement"))
                    return true;
                else if (queryString_L.StartsWith("?legal") || queryString_L.StartsWith("?agreement"))
                {
                    HttpContext.Current.Response.Redirect("~/ASPdatabase.NET.aspx?SubscriptionAgreement");
                    return false;
                }
                else
                    return false;
            }
        }

        //----------------------------------------------------------------------------------------------------
        public void Run()
        {
            string scriptCacheVersion = Config.SystemProperties.Version;

            if (this.QueryString_AjaxRequest)
            {
                (new ASPdatabaseService()).GatewayEntry_Run();
            }
            else if (this.CheckFor_ScriptAndStyle())
            {
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Private);
                HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddDays(60));
                HttpContext.Current.Response.End();
            }
            else if(!String.IsNullOrEmpty(this.QueryString_IMG))
            {
                this.Return_IMG();
            }
            else if(this.QueryString_IsAgreementPage)
            {
                Return_SubscriptionAgreement(true);
            }
            else if(UI.PageParts.Install.Backend.InstallLogic.IsInInstallState())
            {
                ASPdb.FrameworkUI.Cookies.ServerSide_PopulateDefaultCookies();
                this.GetGenericHTML(scriptCacheVersion, "Install | ASPdatabase.NET", "ASPdatabaseNET_InstallPage();");
            }
            else if (HttpContext.Current.Request.Url.Query.ToLower().StartsWith("?login"))
            {
                ASPdb.FrameworkUI.Cookies.ServerSide_PopulateDefaultCookies();
                this.GetGenericHTML(scriptCacheVersion, "Login | ASPdatabase.NET", "ASPdatabaseNET_LoginPage();");
            }
            else if (HttpContext.Current.Request.Url.Query.ToLower().StartsWith("?logout"))
            {
                Users.UserSessionLogic.DoLogout();
                HttpContext.Current.Response.Redirect("ASPdatabase.NET.aspx?Login");
            }
            else if (HttpContext.Current.Request.Url.Query.ToLower().StartsWith("?toggleimpersonation"))
            {
                Users.UserSessionLogic.ToggleImpersonation();
                HttpContext.Current.Response.Redirect("ASPdatabase.NET.aspx");
            }
            else if (HttpContext.Current.Request.QueryString["Upload"] == "Excel")
            {
                if (!Users.UserSessionLogic.GetUser().IsLoggedIn)
                    HttpContext.Current.Response.Redirect("~/ASPdatabase.NET.aspx?Login");

                string guidKey = HttpContext.Current.Request.QueryString["Key"];
                int tableId = -1;
                Int32.TryParse(HttpContext.Current.Request.QueryString["TableId"], out tableId);
                UI.TableGrid.Backend.ImportExportLogic.Do_ExcelUpload(guidKey, tableId);
            }
            else if (HttpContext.Current.Request.QueryString["Download"] == "Excel")
            {
                if (!Users.UserSessionLogic.GetUser().IsLoggedIn)
                    HttpContext.Current.Response.Redirect("~/ASPdatabase.NET.aspx?Login");

                string gridRequestBase64 = HttpContext.Current.Request.QueryString["Key"];
                gridRequestBase64 = HttpContext.Current.Server.UrlDecode(gridRequestBase64);
                UI.TableGrid.Backend.ImportExportLogic.Do_ExcelDownload(gridRequestBase64);
            }
            else
            {
                ASPdb.FrameworkUI.Cookies.ServerSide_PopulateDefaultCookies();
                if(!Users.UserSessionLogic.GetUser().IsLoggedIn)
                    HttpContext.Current.Response.Redirect("~/ASPdatabase.NET.aspx?Login");

                this.GetGenericHTML(scriptCacheVersion, "ASPdatabase.NET", "ASPdatabaseNET_Start();");
            }
        }


        //----------------------------------------------------------------------------------------------------
        private void GetGenericHTML(string scriptCacheVersion, string title, string javascriptStartCommand)
        {

            int baseLen = Config.SystemProperties.CopyrightLine.Length - 36;
            HttpContext.Current.Response.Write(String.Format(@"<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>{0}</title>
    <link rel=""shortcut icon"" href=""ASPdatabase.NET.aspx?IMG=favicon"" />
    <script src=""ASPdatabase.NET.aspx?01&JS=ASPdatabase.NET&{1}""></script>
    <script>{2}</script>
</head>
<body>
    <ASPdatabaseNET>  ASPdatabase.NET {3}          </ASPdatabaseNET>
    <ASPdatabaseNET>  Download at www.ASPdatabase.net  {4}  </ASPdatabaseNET>
    <ASPdatabaseNET>  {5}  </ASPdatabaseNET>
</body>
</html>",
                title,
                scriptCacheVersion,
                javascriptStartCommand,
                this.MinPad(Config.SystemProperties.Version, baseLen + 12),
                this.MinPad("", baseLen + 3),
                this.MinPad(Config.SystemProperties.CopyrightLine, baseLen + 2)));
        }
        //----------------------------------------------------------------------------------------------------
        private string MinPad(string str, int minStringLen)
        {
            int padLen = minStringLen - str.Length;
            for (int i = 0; i < padLen; i++)
                str += " ";
            return str;
        }


        //----------------------------------------------------------------------------------------------------
        private bool CheckFor_ScriptAndStyle()
        {
            var return_JS_FileNames = new List<string>();
            var return_CSS_FileNames = new List<string>();

            switch (this.QueryString_JS)
            {
                case "jQuery":
                    //return_JS_FileNames.Add(@"~\App_Data\JS\jquery-1.10.2.min.js");
                    return_JS_FileNames.Add(@"~\App_Data\JS\jquery-1.8.0.min.js");
                    break;
                case "jQuery.Map":
                    return_JS_FileNames.Add(@"~\App_Data\JS\jquery-1.10.2.min.map");
                    break;
                case "jQuery.JSON":
                    return_JS_FileNames.Add(@"~\App_Data\JS\jquery.json-2.3.js");
                    break;
                case "ASPdb":
                    return_JS_FileNames.Add(@"~\App_Data\JS\ASPdb.js");
                    break;
                case "ASPdatabase.NET":
                    string jsText = UI.Assets.JsText.Get();
                    HttpContext.Current.Response.ContentType = "application/javascript";
                    HttpContext.Current.Response.Write(jsText);
                    return true;
                    //break;
            }
            switch (this.QueryString_CSS)
            {
                case "ASPdatabase.NET":
                    return_CSS_FileNames.Add(@"~\App_Data\CSS\ASPdatabase.NET.css");
                    break;
            }

            bool rtn = false;
            try
            {
                if (return_JS_FileNames.Count > 0)
                {
                    rtn = true;
                    HttpContext.Current.Response.ContentType = "application/javascript";
                    foreach (string fileName in return_JS_FileNames)
                    {
                        HttpContext.Current.Response.Write(File.ReadAllText(HttpContext.Current.Server.MapPath(fileName)));
                        HttpContext.Current.Response.Write(System.Environment.NewLine);
                    }
                }
                else if (return_CSS_FileNames.Count > 0)
                {
                    rtn = true;
                    HttpContext.Current.Response.ContentType = "text/css";
                    foreach (string fileName in return_CSS_FileNames)
                    {
                        HttpContext.Current.Response.Write(File.ReadAllText(HttpContext.Current.Server.MapPath(fileName)));
                        HttpContext.Current.Response.Write(System.Environment.NewLine);
                    }
                }
            }
            catch (Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.Write("Error Getting Script File");
                HttpContext.Current.Response.End();
                rtn = true;
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        private void Return_IMG()
        {
            string resourceName = null;
            string resourcePrefix = "ASPdatabaseNET_Web._ASPdatabaseNET.UI.Assets";
            string contentType = "";
            string filename = "";
            switch (this.QueryString_IMG.ToLower())
            {
                case "favicon":
                    resourceName = resourcePrefix + ".Graphics.favicon.ico";
                    contentType = "image/x-icon";
                    filename = "favicon.ico";
                    break;
                case "sprite1":
                    resourceName = resourcePrefix + ".Graphics.Sprite1.png";
                    contentType = "image/png";
                    filename = "Sprite1.png";
                    break;
            }


            var assembly = Assembly.GetExecutingAssembly();
            if (resourceName != null)
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    byte[] bytes = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bytes, 0, (int)stream.Length);

                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.ContentType = contentType;
                    HttpContext.Current.Response.AppendHeader("Content-Disposition", String.Format(@"inline; filename=""{0}""; ", filename));
                    HttpContext.Current.Response.BinaryWrite(bytes);
                }
            }
            else
                HttpContext.Current.Response.Redirect("~/");
        }
        //----------------------------------------------------------------------------------------------------
        public static string Return_SubscriptionAgreement(bool sendInHTTPResponse)
        {
            string resourcePrefix = "ASPdatabaseNET_Web._ASPdatabaseNET.UI.Assets";
            string resourceName = resourcePrefix + String.Format(".SubscriptionAgreement.{0}.html", Config.SystemProperties.Version);
            string resourceName_Default = resourcePrefix + ".SubscriptionAgreement.html";

            var assembly = Assembly.GetExecutingAssembly();
            byte[] bytes = null;
            try
            {
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    bytes = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bytes, 0, (int)stream.Length);
                }
            }
            catch
            {
                resourceName = resourceName_Default;
                ASPdb.Framework.Debug.WriteLine("Return_SubscriptionAgreement() :: " + resourceName);
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    bytes = new byte[stream.Length];
                    stream.Position = 0;
                    stream.Read(bytes, 0, (int)stream.Length);
                }
            }

            string rtn = System.Text.Encoding.Default.GetString(bytes);
            rtn = rtn.Replace("“", "&ldquo;")
                .Replace("Ò", "&ldquo;")
                .Replace("”", "&rdquo;")
                .Replace("Ó", "&rdquo;")
                .Replace("Õ", "&rsquo;");
            
            if (sendInHTTPResponse)
            {
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.ContentType = "text/HTML";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", @"inline; filename=""" + resourceName + @"""; ");
                HttpContext.Current.Response.Write(rtn);
            }

            return rtn;
        }
    }
}