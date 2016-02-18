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
    public class JsStr
    {
        public string String;

        //------------------------------------------------------------------------------------- constructor --
        public JsStr()
        {
        }

        //------------------------------------------------------------------------------------------ static --
        public static JsStr S(string inputString)
        {
            var rtn = new JsStr();
            rtn.String = inputString;
            return rtn;
        }

        //------------------------------------------------------------------------------------------ static --
        /// <summary>Use is Javascript to replace ALL occurrences of oldValue.</summary>
        public static string Replace(string inputString, string oldValue, string newValue)
        {
            return inputString.As<JsString>().split(oldValue).join(newValue);
        }
        //----------------------------------------------------------------------------------------------------
        public string Replace1(string oldValue, string newValue)
        {
            return JsStr.Replace(this.String, oldValue, newValue);
        }
        //----------------------------------------------------------------------------------------------------
        public JsStr Replace2(string oldValue, string newValue)
        {
            this.String = JsStr.Replace(this.String, oldValue, newValue);
            return this;
        }

        //----------------------------------------------------------------------------------------------------
        public JsStr ToLower()
        {
            this.String = this.String.As<JsString>().toLowerCase();
            return this;
        }
        //----------------------------------------------------------------------------------------------------
        public JsStr ToUpper()
        {
            this.String = this.String.As<JsString>().toUpperCase();
            return this;
        }
        //----------------------------------------------------------------------------------------------------
        public JsStr Trim()
        {
            this.String = this.String.As<JsString>().trim();
            return this;
        }




        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat1(JsString str, string param0)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat2(JsString str, string param0, string param1)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat3(JsString str, string param0, string param1, string param2)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat4(JsString str, string param0, string param1, string param2, string param3)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2)
                .Replace2("{3}", param3).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat5(JsString str, string param0, string param1, string param2, string param3, string param4)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2)
                .Replace2("{3}", param3)
                .Replace2("{4}", param4).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat6(JsString str, string param0, string param1, string param2, string param3, string param4, string param5)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2)
                .Replace2("{3}", param3)
                .Replace2("{4}", param4)
                .Replace2("{5}", param5).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat7(JsString str, string param0, string param1, string param2, string param3, string param4, string param5, string param6)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2)
                .Replace2("{3}", param3)
                .Replace2("{4}", param4)
                .Replace2("{5}", param5)
                .Replace2("{6}", param6).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat8(JsString str, string param0, string param1, string param2, string param3, string param4, string param5, string param6, string param7)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2)
                .Replace2("{3}", param3)
                .Replace2("{4}", param4)
                .Replace2("{5}", param5)
                .Replace2("{6}", param6)
                .Replace2("{7}", param7).String;
        }
        //------------------------------------------------------------------------------------------ static --
        public static string StrFormat9(JsString str, string param0, string param1, string param2, string param3, string param4, string param5, string param6, string param7, string param8)
        {
            return JsStr.S(str)
                .Replace2("{0}", param0)
                .Replace2("{1}", param1)
                .Replace2("{2}", param2)
                .Replace2("{3}", param3)
                .Replace2("{4}", param4)
                .Replace2("{5}", param5)
                .Replace2("{6}", param6)
                .Replace2("{7}", param7)
                .Replace2("{8}", param8).String;
        }

    }
}