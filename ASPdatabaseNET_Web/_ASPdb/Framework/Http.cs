using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using ASPdb.Ajax;

namespace ASPdb.Framework
{
    public class Http
    {
        //----------------------------------------------------------------------------------------------------
        public static string Fetch(string url)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "GET";
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                return (new StreamReader(webResponse.GetResponseStream())).ReadToEnd();
            }
            catch { return ""; }
        }

        //----------------------------------------------------------------------------------------------------
        public static string Fetch_AjaxPost(string url, object ajaxRequest_Object)
        {
            string ajaxRequest_JsonString = (new AjaxHelper()).ToJson(ajaxRequest_Object);
            return Fetch_AjaxPost(url, ajaxRequest_JsonString);
        }
        //----------------------------------------------------------------------------------------------------
        public static string Fetch_AjaxPost(string url, string ajaxRequest_JsonString)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";

                using (var writer = new StreamWriter(webRequest.GetRequestStream()))
                {
                    writer.Write("AjaxRequest=" + ajaxRequest_JsonString);
                }
                
                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                return (new StreamReader(webResponse.GetResponseStream())).ReadToEnd();
            }
            catch (Exception exc) { ASPdb.Framework.Debug.RecordException(exc); return ""; }
        }
       
    }
}