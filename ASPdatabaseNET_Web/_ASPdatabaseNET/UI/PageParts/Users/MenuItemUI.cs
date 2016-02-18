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
    public class MenuItemUI : MRBPattern<UsersMenuItem, UsersViewModel>
    {
        public bool IsOn = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public MenuItemUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='MenuItemUI jRoot'>");
            //this.jRoot.append(this.GetHtmlRoot());
            this.jRoot.append(this.Model.DisplayName);

            var t = this;
            var j = this.jRoot;
            eval("j.click(function(){ t.Click(); });");
        }


        //----------------------------------------------------------------------------------------------------
        public void TurnOn()
        {
            jRoot.addClass("MenuItemUI_On");
            this.IsOn = true;
        }
        public void TurnOff()
        {
            jRoot.removeClass("MenuItemUI_On");
            this.IsOn = false;
        }



        //------------------------------------------------------------------------------------------ Events --
        public void Click()
        {
            this.OnChange.After.Fire1(this);
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
                .MenuItemUI { border-bottom: 1px solid #f9f9f9; line-height: 2.2em; cursor: pointer; font-size: .75em; padding: 0em .5em; white-space:nowrap; }
                .MenuItemUI:hover { background: #777; color: #ccc; }
                .MenuItemUI b { font-weight:normal; color: #1161ca; }
                .MenuItemUI:hover b { color: #c3daf8; }
                .MenuItemUI_On { background: #555; color: #ddd; }
                .MenuItemUI_On b { color: #c3daf8; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            ";
        }
    }
}
