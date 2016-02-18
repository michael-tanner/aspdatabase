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
    public class EventInfo
    {
        public HtmlElement ElementAttachedTo = null;
        public string EventType = "";
        public object ObjectToCallOn = null;
        public string MethodToCall = "";
        public Event EventObj = null;
        public object PassThruDataObj = null;
    }
}