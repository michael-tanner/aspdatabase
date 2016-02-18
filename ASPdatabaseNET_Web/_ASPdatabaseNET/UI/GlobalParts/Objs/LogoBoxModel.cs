using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.GlobalParts.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class LogoBoxModel
    {
        public enum CustomLogoTypes { None, Image, Text };

        public CustomLogoTypes CustomLogoType = CustomLogoTypes.None;
        public string LogoURL;
        public string LogoText;
    }
}