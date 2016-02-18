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
    public class Panel_GroupUI_PermissionItem : MRBPattern<Permission, UsersViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public Panel_GroupUI_PermissionItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Panel_GroupUI_PermissionItem jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            string name = "";
            string itemClass = "";
            string holderClass = "";
            string titleTag = "";
            switch(this.Model.PermissionType)
            {
                case Permission.PermissionTypes.Connection:
                    name = this.Model.ConnectionName;
                    itemClass = "Item_C";
                    holderClass = "Holder_C";
                    titleTag = "CONNECTION : " + name;
                    break;
                case Permission.PermissionTypes.Schema:
                    name = this.Model.Schema;
                    itemClass = "Item_S";
                    holderClass = "Holder_S";
                    titleTag = "SCHEMA : " + name;
                    break;
                case Permission.PermissionTypes.Table:
                    name = this.Model.TableName;
                    itemClass = "Item_T";
                    holderClass = "Holder_T";
                    titleTag = "TABLE : " + name;
                    break;
            }
            jF(".Item").html(name).addClass(itemClass).attr("title", titleTag);
            jF(".Holder_Checkboxes").addClass(holderClass);

            this.Click_HELPER(this.Model.View, "View");
            this.Click_HELPER(this.Model.Edit, "Edit");
            this.Click_HELPER(this.Model.Insert, "Insert");
            this.Click_HELPER(this.Model.Delete, "Delete");
        }






        //------------------------------------------------------------------------------------------ Events --
        private void Click_HELPER(bool isClicked, string name)
        {
            if (isClicked)
            {
                jF(".Div_" + name).addClass("DivItem_On");
                jF(".C_" + name).attr("checked", true);
            }
            else
            {
                jF(".Div_" + name).removeClass("DivItem_On");
                jF(".C_" + name).removeAttr("checked");
            }
        }
        public void Click_View()
        {
            this.Model.View = !this.Model.View;
            this.Click_HELPER(this.Model.View, "View");
        }
        public void Click_Edit()
        {
            this.Model.Edit = !this.Model.Edit;
            this.Click_HELPER(this.Model.Edit, "Edit");
        }
        public void Click_Insert()
        {
            this.Model.Insert = !this.Model.Insert;
            this.Click_HELPER(this.Model.Insert, "Insert");
        }
        public void Click_Delete()
        {
            this.Model.Delete = !this.Model.Delete;
            this.Click_HELPER(this.Model.Delete, "Delete");
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
                .Panel_GroupUI_PermissionItem { position:relative; border-bottom: 1px solid #aac5e9; }
                .Panel_GroupUI_PermissionItem .Item { font-size: .7em; padding-left: .6em; line-height: 1.7857em; overflow:hidden; white-space:nowrap; color: #14498f; }
                .Panel_GroupUI_PermissionItem .Item_C { background: #dee9f6; padding-left: 0.6em; }
                .Panel_GroupUI_PermissionItem .Item_S { background: #fdfcd7; padding-left: 1.2em; }
                .Panel_GroupUI_PermissionItem .Item_T { background: #fff;    padding-left: 1.8em; color: #3d7bcc; }

                .Panel_GroupUI_PermissionItem .Holder_Checkboxes { position:absolute; top: 0em; left: 10.5em; }
                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .DivItem { float:left; width: 2.25em; text-align:center; cursor:pointer; padding-top: 0em; border-right: 1px solid #fff; }
                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .DivItem:hover { background: #4d8cde; }
                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .DivItem input { cursor:pointer; }

                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .Div_View { border-left: 1px solid #fff; }
                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .Div_Edit { }
                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .Div_Insert { }
                .Panel_GroupUI_PermissionItem .Holder_Checkboxes .Div_Delete { }

                .Panel_GroupUI_PermissionItem .Holder_C .DivItem  { background: #dee9f6; }
                .Panel_GroupUI_PermissionItem .Holder_S .DivItem  { background: #fdfcd7; }
                .Panel_GroupUI_PermissionItem .Holder_T .DivItem  { background: #fff;    }

                .Panel_GroupUI_PermissionItem .Holder_C .DivItem_On  { background: #8eb8ee; }
                .Panel_GroupUI_PermissionItem .Holder_S .DivItem_On  { background: #8eb8ee; }
                .Panel_GroupUI_PermissionItem .Holder_T .DivItem_On  { background: #8eb8ee; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Item'></div>
                <div class='Holder_Checkboxes'>
                    <div class='DivItem Div_View'   On_Click='Click_View'  ><input type='checkbox' class='C_View'   /></div>
                    <div class='DivItem Div_Edit'   On_Click='Click_Edit'  ><input type='checkbox' class='C_Edit'   /></div>
                    <div class='DivItem Div_Insert' On_Click='Click_Insert'><input type='checkbox' class='C_Insert' /></div>
                    <div class='DivItem Div_Delete' On_Click='Click_Delete'><input type='checkbox' class='C_Delete' /></div>
                </div>
            ";
        }
    }
}
