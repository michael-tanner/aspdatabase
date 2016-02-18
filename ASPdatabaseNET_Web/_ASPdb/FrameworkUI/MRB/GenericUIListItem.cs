using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdb.FrameworkUI.MRB
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class GenericUIListItem : jQueryContext
    {
        public string Value = "";
        public string Text = "";
        public string Style = "";

        //------------------------------------------------------------------------------------- Constructor --
        public static GenericUIListItem New1(string value)
        {
            var rtn = new GenericUIListItem();
            rtn.Value = value;
            rtn.Text = value;
            return rtn;
        }
        //------------------------------------------------------------------------------------- Constructor --
        public static GenericUIListItem New2(string value, string text)
        {
            var rtn = new GenericUIListItem();
            rtn.Value = value;
            rtn.Text = text;
            return rtn;
        }
        //------------------------------------------------------------------------------------- Constructor --
        public static GenericUIListItem New3(string value, string text, string style)
        {
            var rtn = new GenericUIListItem();
            rtn.Value = value;
            rtn.Text = text;
            rtn.Style = style;
            return rtn;
        }

    }
}