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
    public class StringStatic : jQueryContext
    {

        //----------------------------------------------------------------------------------------------------
        public static bool Contains(string inputString, string value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                inputString = inputString.As<JsString>().toLowerCase();
                value = value.As<JsString>().toLowerCase();
            }

            if (inputString == null || inputString == "")
                return false;

            if (value == null || value == "")
                return false;

            return (inputString.IndexOf(value) >= 0);
        }

        //----------------------------------------------------------------------------------------------------
        public static int RemoveNonNumericChars(string inputString, int defaultIfError)
        {
            int rtn = -1;
            try
            {
                eval("rtn = inputString.replace(/\\D/g,'');");
            }
            catch
            {
                rtn = defaultIfError;
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public static bool IsNullOrEmpty(string value)
        {
            if (value == null) return true;
            if (value.Length < 1) return true;

            return false;
        }
        //----------------------------------------------------------------------------------------------------
        public static bool IsNullOrWhiteSpace(string value)
        {
            if (value != null)
                value = value.Trim();

            return IsNullOrEmpty(value);
        }
        //----------------------------------------------------------------------------------------------------
        public static bool StartsWith(JsString inputString, JsString value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return (inputString.toLowerCase().slice(0, value.length) == value.toLowerCase());
            }
            else
            {
                return (inputString.slice(0, value.length) == value);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static bool EndsWith(JsString inputString, JsString value, bool ignoreCase)
        {
            if (ignoreCase)
            {
                return (inputString.toLowerCase().slice(-value.length) == value.toLowerCase());
            }
            else
            {
                return (inputString.slice(-value.length) == value);
            }
        }



        //----------------------------------------------------------------------------------------------------
        public static string Replace_StartOfString(string inputString, int replaceStringLength, string replaceValue)
        {
            alert("Not Implemented");
            return "";
        }
        //----------------------------------------------------------------------------------------------------
        public static string Replace_EndOfString(string inputString, int replaceStringLength, string replaceValue)
        {
            alert("Not Implemented");
            return "";
        }

    }
}