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
    public class Panel_UserUI_GroupItem : MRBPattern<UserToGroup_Assignment, UsersViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public Panel_UserUI_GroupItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Panel_UserUI_GroupItem jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();

            this.Model.IsMember = !this.Model.IsMember;
            this.Click();
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Click()
        {
            this.Model.IsMember = !this.Model.IsMember;
            if(this.Model.IsMember)
            {
                jF(".Item").addClass("Item_On");
                jF(".C_IsMember").attr("checked", true);
            }
            else
            {
                jF(".Item").removeClass("Item_On");
                jF(".C_IsMember").removeAttr("checked");
            }
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
                .Panel_UserUI_GroupItem { border-bottom: 1px solid #aac5e9; }
                .Panel_UserUI_GroupItem .Item { position:relative; line-height: 1.625em; padding: 0em 1em 0em 2.5em; overflow:hidden; white-space:nowrap; cursor:pointer; }
                .Panel_UserUI_GroupItem .Item:hover { background: #3d7bcc; color: #fff; }
                .Panel_UserUI_GroupItem .Item_On { background: #dee9f6; }
                .Panel_UserUI_GroupItem .Item input { position:absolute; top: 0.375em; left: .9em; }
                .Panel_UserUI_GroupItem .Item div { font-size: .8em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Item' On_Click='Click'>
                    <div ModelKey='GroupName'></div>
                    <input type='checkbox' class='C_IsMember' />
                </div>
            ";
        }
    }
}
