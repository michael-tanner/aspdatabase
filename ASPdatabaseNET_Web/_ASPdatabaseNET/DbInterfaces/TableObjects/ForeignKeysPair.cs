using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DbInterfaces.TableObjects
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ForeignKeysPair
    {
        public ForeignKey[] InboundKeys;
        public ForeignKey[] OutboundKeys;
    }
}