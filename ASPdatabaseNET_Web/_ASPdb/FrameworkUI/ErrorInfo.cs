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
    public class ErrorInfo
    {
        public string Message = "";
        public string StackTrace = "";

        //------------------------------------------------------------------------------------------ static --
        public static ErrorInfo New(Exception exc)
        {
            var rtn = new ErrorInfo();
            rtn.Message = exc.Message;
            rtn.StackTrace = exc.StackTrace;
            return rtn;
        }
    }
}