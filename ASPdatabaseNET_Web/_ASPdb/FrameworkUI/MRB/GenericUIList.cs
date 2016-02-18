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
    public class GenericUIList : jQueryContext
    {
        public bool IsClientCode = false;
        public GenericUIListItem[] List;
        public List<GenericUIListItem> _internalList;

        //------------------------------------------------------------------------------------- constructor --
        public GenericUIList()
        {
            try { var app = HttpContext.Current.Application; }
            catch { this.IsClientCode = true; }

            this.List = new GenericUIListItem[0];
            if (!this.IsClientCode)
                _internalList = new List<GenericUIListItem>();
        }

        //----------------------------------------------------------------------------------------------------
        public void Add0(GenericUIListItem item)
        {
            if (this.IsClientCode)
                this.List[this.List.Length] = item;
            else
            {
                _internalList.Add(item);
                this.List = _internalList.ToArray(); // this can be improved
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Add1(string value)
        {
            if (this.IsClientCode)
                this.Add0(GenericUIListItem.New1(value));
        }
        //----------------------------------------------------------------------------------------------------
        public void Add2(string value, string text)
        {
            if (this.IsClientCode)
                this.Add0(GenericUIListItem.New2(value, text));
        }
        //----------------------------------------------------------------------------------------------------
        public void Add3(string value, string text, string style)
        {
            if (this.IsClientCode)
                this.Add0(GenericUIListItem.New3(value, text, style));
        }

    }
}