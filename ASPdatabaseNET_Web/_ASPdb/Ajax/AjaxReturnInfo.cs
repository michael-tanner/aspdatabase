using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit;
using SharpKit.Html;
using SharpKit.JavaScript;
using SharpKit.jQuery;

namespace ASPdb.Ajax
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AjaxReturnInfo
    {
        public object Callback_Object { get; set; }
        public string Callback_Method { get; set; }
        public object DataObj { get; set; }

        //----------------------------------------------------------------------------------------------------
        public static AjaxReturnInfo New(object callback_Object, string callback_Method)
        {
            var rtn = new AjaxReturnInfo();
            rtn.Callback_Object = callback_Object;
            rtn.Callback_Method = callback_Method;
            rtn.DataObj = null;
            return rtn;
        }
    }
}