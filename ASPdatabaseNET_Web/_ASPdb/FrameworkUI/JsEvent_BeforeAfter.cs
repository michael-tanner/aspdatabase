using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;

namespace ASPdb.FrameworkUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class JsEvent_BeforeAfter : jQueryContext
    {
        public JsEventDelegate Before;
        public JsEventDelegate After;

        //------------------------------------------------------------------------------------- Constructor --
        public JsEvent_BeforeAfter()
        {
            this.Before = new JsEventDelegate();
            this.After = new JsEventDelegate();
        }
    }
}