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
    public class ErrorHandler : jQueryContext
    {
        //------------------------------------------------------------------------------------------ static --
        public static bool Check(ErrorInfo error)
        {
            if (error == null)
            {
                return false;
            }
            else
            {
                alert("Error\n" + error.Message + "\n\n" + error.StackTrace);
                return true;
            }
        }
    }
}