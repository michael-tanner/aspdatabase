using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath, InlineFields = false)]
    public class Cookies : jQueryContext
    {
        //------------------------------------------------------------------------------------------ static --
        public static string Get(string cookieName)
        {
            var nameEQ = cookieName + "=";
            var ca = document.cookie.split(';');
            for (var i = 0; i < ca.length; i++)
            {
                var c = ca[i];
                while (c.charAt(0).As<char>() == ' ')
                    c = c.substring(1, c.length);
                if (c.indexOf(nameEQ) == 0)
                    return c.substring(nameEQ.Length, c.length);
            }
            return null;
        }
        //------------------------------------------------------------------------------------------ static --
        public static void Set(string cookieName, string value, int expireDays)
        {
            var expires = "";
            if(expireDays < -1 || expireDays > 0) // values -1 or 0 indicate no expiration set
            {
                var date = new JsDate();
		        date.setTime(date.getTime()+(expireDays*24*60*60*1000));
		        expires = "; expires=" + date.toGMTString();
	        }
            document.cookie = cookieName + "=" + value + expires + "; path=/";
        }
        //------------------------------------------------------------------------------------------ static --
        public static void Erase(string cookieName)
        {
            Set(cookieName, "", -2);
        }



        //------------------------------------------------------------------------------------------ static --
        public static string ASPdb_BaseUrl
        {
            get { return Get("ASPdb_BaseUrl"); }
        }
        //------------------------------------------------------------------------------------------ static --
        public static string ASPdb_AjaxGatewayUrl
        {
            get { return Get("ASPdb_AjaxGatewayUrl"); }
        }


        //------------------------------------------------------------------------------------------ static --
        [JsMethod(Export=false)]
        public static void ServerSide_PopulateDefaultCookies()
        {
            var page = ((System.Web.UI.Page)HttpContext.Current.Handler);
            string basePath = page.ResolveUrl("~/");

            string fixedCookiePath = "";
            var request = HttpContext.Current.Request;
            if (request.ApplicationPath.Length > 1)
            {
                int slashCount = request.ApplicationPath.Split(new char[] { '/' }).Length - 1;
                var arr1 = request.Url.PathAndQuery.Split(new char[] { '/' });
                for (int i = 0; i < slashCount; i++)
                    if (arr1.Length > slashCount)
                        fixedCookiePath += "/" + arr1[i + 1];
            }
            if (string.IsNullOrEmpty(fixedCookiePath))
                fixedCookiePath = request.ApplicationPath;


            ServerSide_SetCookie("ASPdb_BaseUrl", basePath, fixedCookiePath, null);

            ServerSide_SetCookie("ASPdb_AjaxGatewayUrl", basePath + "ASPdatabase.NET.aspx?AjaxRequest=true", fixedCookiePath, null); 
        }
        //------------------------------------------------------------------------------------------ static --
        [JsMethod(Export = false)]
        private static void ServerSide_SetCookie(string cookieName, string value, string cookiePath, DateTime? expireTime)
        {
            var cookie = new HttpCookie(cookieName, value);

            if(!string.IsNullOrEmpty(cookiePath))
                cookie.Path = cookiePath;

            if (expireTime.HasValue)
                cookie.Expires = expireTime.Value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }


    }
}