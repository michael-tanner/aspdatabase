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
    [Serializable()]
    public class AESKeyInfo : jQueryContext
    {
        public string A;
        public string Pass;
        public string B;
        public string Key;
        public string C;
        public string IV;
        public string D;

        //----------------------------------------------------------------------------------------------------
        public string ToJson()
        {
            string rtn = null;
            eval("rtn = $.toJSON(this);");
            return rtn;
        }
    }
}