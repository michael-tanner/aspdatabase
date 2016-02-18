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
    public class JsHelper : jQueryContext
    {
        //------------------------------------------------------------------------------------------ static --
        public static bool IsVisible1(HtmlElement element)
        {
            try
            {
                if (GetCalculatedStyle(element, "display").As<JsString>() != "none")
                    return true;
            }
            catch { }
            return false;
        }
        //------------------------------------------------------------------------------------------ static --
        public static bool IsVisible2(jQuery jRoot, string elementClassName)
        {
            try
            {
                if (!StringStatic.StartsWith(elementClassName, ".", false))
                    elementClassName = "." + elementClassName;

                return IsVisible1(jRoot.find(elementClassName)[0]);
            }
            catch { }
            return false;
        }

        //------------------------------------------------------------------------------------------ static --
        /// <summary>Element must already be inserted into DOM to work </summary>
        public static object GetCalculatedStyle(HtmlElement element, string styleProperty)
        {
            object rtn = null;
            string evalString = @"
	                if (element.currentStyle)
                    {
		                rtn = element.currentStyle[styleProperty];
                    }
	                else if (window.getComputedStyle)
                    {
                        var computedStyle = document.defaultView.getComputedStyle(element, null);
		                rtn = computedStyle.getPropertyValue(styleProperty);
                    }
            ";
            eval(evalString);
            return rtn;
        }


    }
}