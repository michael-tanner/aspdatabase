using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Users.Objs;

namespace ASPdatabaseNET.UI.PageParts.Users
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Panel_GroupUI_ConnectionItem : MRBPattern<Permission, UsersViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public Panel_GroupUI_ConnectionItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Panel_GroupUI_ConnectionItem jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();
            jF(".Item").attr("title", this.Model.ConnectionName + " (Id: " + this.Model.ConnectionId + ")");
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Click()
        {
            this.OnChange.After.Fire1(this);
            jF(".Item").addClass("Item_On");
        }
        //----------------------------------------------------------------------------------------------------
        public void RemoveClass__Item_On()
        {
            jF(".Item").removeClass("Item_On");
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .Panel_GroupUI_ConnectionItem { border-bottom: 1px solid #aac5e9; }
                .Panel_GroupUI_ConnectionItem .Item { font-size: .7em; padding-left: .6em; line-height: 1.7857em; overflow:hidden; white-space:nowrap; color: #14498f; cursor:pointer; }
                .Panel_GroupUI_ConnectionItem .Item:hover { background: #14498f; color: #fff; }
                .Panel_GroupUI_ConnectionItem .Item_On { background: #dee9f6; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Item' ModelKey='ConnectionName' On_Click='Click'></div>
            ";
        }
    }
}
