using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class QueryString : jQueryContext
    {
        //------------------------------------------------------------------------------------------ static --
        /// <summary>Case Insensitive</summary>
        /// <param name="key">Query String Parameter Key -- Case Insensitive</param>
        /// <returns>Query String Parameter Value</returns>
        public static string Get(JsString key)
        {
            string rtn = "";
            try
            {
                string name = key; //.toLowerCase();
                string location_search = location.search.As<JsString>(); //.toLowerCase();
                string[] results = new string[2];

                // http://stackoverflow.com/questions/901115/how-can-i-get-query-string-values
                string eval1 = "    name = name.replace(/[\\[]/, \"\\\\\\[\").replace(/[\\]]/, \"\\\\\\]\");                 ";
                string eval2 = "    var regex = new RegExp(\"[\\\\?&]\" + name + \"=([^&#]*)\", \"i\");                      ";
                string eval3 = "    var results = regex.exec(location_search);                                               ";
                string eval4 = "    rtn = results == null ? \"\" : decodeURIComponent(results[1].replace(/\\+/g, \" \"));    ";

                eval(eval1);
                eval(eval2);
                eval(eval3);
                eval(eval4);

            }
            catch 
            { 
                rtn = ""; 
            }

            return rtn;
        }

        //------------------------------------------------------------------------------------------ static --
        public static bool Get_AsBool(string key)
        {
            string value = Get(key);
            value = value.ToLower().Trim();

            if (value == "true" || value == "1")
                return true;

            return false;
        }

        //------------------------------------------------------------------------------------------ static --
        /// <summary>Returns first value from Query String (immediately after question mark) if does not contain an '=' sign; else returns empty string.</summary>
        public static string GetFirstValue_NonKeyParameter()
        {
            string rtn = "";

            string firstItem = window.location.search.split('&')[0].replace("?", "");
            if (!StringStatic.Contains(firstItem, "=", false))
                rtn = firstItem;

            return rtn;
        }


    }
}