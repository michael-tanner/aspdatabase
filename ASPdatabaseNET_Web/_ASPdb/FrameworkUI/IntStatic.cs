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
    public class IntStatic : jQueryContext
    {
        //----------------------------------------------------------------------------------------------------
        public static int Parse(string str, int defaultValue)
        {
            try
            {
                int rtn = parseInt(str);
                if(jQuery.isNumeric(rtn))
                    return rtn;
            }
            catch { }

            return defaultValue;
        }
    }
}