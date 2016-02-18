using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdb.Security
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Util : jQueryContext
    {
        public static void TempTest()
        {
            eval("$(document).ready(function() { ASPdb.Security.Util.TempTest2(); });");
        }
        public static void TempTest2()
        {
            var util = new Util();

            var ajaxSender = ASPdb.Security.AjaxSender.GetObj();
            if(!ajaxSender.IsReady)
            {
                ajaxSender.OnReady.After.AddHandler(util, "AjaxSender_OnReady", 0);
                ajaxSender.Initialize();
            }
        }

        //----------------------------------------------------------------------------------------------------
        public void AjaxSender_OnReady()
        {
            var ajaxSender = ASPdb.Security.AjaxSender.GetObj();
            string publicKey = ajaxSender.PublicKeyPem;
            J(".Text1").val(ajaxSender.AESIndex + "\n\n" + publicKey);
        }



        //----------------------------------------------------------------------------------------------------
        public static string GetRandomBase64(int minLength, int maxLength)
        {
            // if( is Client) { }

            return "aaa";
        }
    }
}