using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.Errors
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ExceptionHandler : jQueryContext
    {
        //----------------------------------------------------------------------------------------------------
        public static bool Check(ASPdb.Ajax.AjaxResponse response)
        {
            if (response.Error == null)
                return false;

            console.log("Error: " + response.Error.Message);
            console.log("StackTrace: " + response.Error.StackTrace);
            alert("Error: " + response.Error.Message);

            if (response.Error.Message == "User Session Expired")
                window.location = ("ASPdatabase.NET.aspx?Login" + window.location.hash).As<Location>();

            return true;
        }
    }
}