using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Arr
    {
        //----------------------------------------------------------------------------------------------------
        public static int Len(object arrayObj)
        {
            int len = 0;
            try
            {
                HtmlContext.eval("len = arrayObj.length;");
            }
            catch { }
            return len;
        }
        //----------------------------------------------------------------------------------------------------
        public static T GetNewGenericArray<T>(T tmpNullObj)
        {
            T rtn = tmpNullObj;
            HtmlContext.eval("rtn = new Array();");
            return rtn;
        }

    }
}