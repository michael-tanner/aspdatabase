using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DataObjects.Nav
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class NavSectionInfo
    {
        public enum SectionTypes { NotSet, Tables, Views };

        public SectionTypes SectionType = SectionTypes.NotSet;
        public string SectionName;
        public NavSectionItemInfo[] Items;

        public bool IsOpen = false;
    }
}