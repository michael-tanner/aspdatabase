using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DataObjects.ManageAssets;

namespace ASPdatabaseNET.UI.PageParts.ManageAssets
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TablesPanel_MenuRow : MRBPattern<BasicAssetInfo, string>
    {
        public bool HoldsHiddenItems = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public TablesPanel_MenuRow(bool holdsHiddenItems)
        {
            this.HoldsHiddenItems = holdsHiddenItems;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='TablesPanel_MenuRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            if (this.Model != null)
            {
                jF2(".Label_TableName").html(this.GetDisplayName());
                jF2(".A_DesignStructure").attr("href", "#00-TableDesign-" + this.Model.GenericId);
                jF2(".A_Permissions").attr("href", "#00-TablePermissions-" + this.Model.GenericId);
            }
            if (this.HoldsHiddenItems)
                jF2(".Bttn_ShowHide").html("Unhide");
            else
                jF2(".Bttn_ShowHide").html("Hide");

                this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private string GetDisplayName()
        {
            string rtn = "";

            if (this.Model.UseSquareBrackets_Schema)
                rtn += "[" + this.Model.Schema + "].";
            else
                rtn += this.Model.Schema + ".";

            if (this.Model.UseSquareBrackets_GenericName)
                rtn += "[<span>" + this.Model.GenericName + "</span>]";
            else
                rtn += "<span>" + this.Model.GenericName + "</span>";

            return rtn;
        }


        //------------------------------------------------------------------------------------------ Events --
        public void TablesMethod_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.OnChange.After.Fire();
        }
        public void BttnClick_Name()
        {
            window.location = ("#00-Table-" + this.Model.GenericId).As<Location>();
        }
        public void BttnClick_Hide()
        {
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "TablesMethod_Return")
                .ManageAssets__Tables_ShowHide(this.Model.ConnectionId, this.Model.GenericId, !this.HoldsHiddenItems);
        }
        public void BttnClick_Rename()
        {
            string newName = prompt("New Table Name", this.Model.GenericName);
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "TablesMethod_Return")
                .ManageAssets__Tables_Rename(this.Model.ConnectionId, this.Model.GenericId, newName);
        }
        public void BttnClick_Delete()
        {
            if (prompt("Are you sure?\n\nIf so, type \"delete table\"") == "delete table")
                ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "TablesMethod_Return")
                    .ManageAssets__Tables_Delete(this.Model.ConnectionId, this.Model.GenericId);
        }
        public void BttnClick_Export()
        {
            alert("Export: " + this.Model.GenericId);
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
                .TablesPanel_MenuRow { font-size: .9em; }
                .TablesPanel_MenuRow td { border: 1px solid #c3c3c3; border-top-width: 0px; line-height: 40px; }
                .TablesPanel_MenuRow .td1 { color: #aaa; border-left-width: 0px; padding-left: 10px; }
                .TablesPanel_MenuRow .td1 span { color: #222; }
                .TablesPanel_MenuRow .td2 { width: 55px; }
                .TablesPanel_MenuRow .td3 { width: 68px; }
                .TablesPanel_MenuRow .td4 { width: 85px; }
                .TablesPanel_MenuRow .td5_a { width: 61px; }
                .TablesPanel_MenuRow .td6 { width: 65px; line-height: 16px; padding-top: 3px; }
                .TablesPanel_MenuRow .td7 { width: 51px; }
                .TablesPanel_MenuRow .Bttn { color: #14498f; text-align:center; font-size: .78em; cursor: pointer; }
                .TablesPanel_MenuRow .Bttn:hover { background: #888; color:#fff; }
                .TablesPanel_MenuRow .Bttn .a4 { color: #14498f; display:block; line-height: 16px; padding: 3px 0px 5px; }
                .TablesPanel_MenuRow .Bttn .a5 { color: #14498f; display:block; line-height: inherit; }
                .TablesPanel_MenuRow .Bttn a:hover { background: #888; color:#fff; }

                .TablesPanel_MenuRow .td4:hover { cursor: default; background: #ececec; color: #14498f; }
                .TablesPanel_MenuRow .td5:hover { cursor: default; background: #ececec; color: #14498f; }

                .TablesPanel_MenuRow:hover { background: #ececec; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
//            return @"
//                <td class='td1 Label_TableName' On_Click='BttnClick_Name'></td>
//                <td class='td2 Bttn Bttn_ShowHide' On_Click='BttnClick_Hide'>Hide</td>
//                <td class='td3 Bttn'>
//                    <a class='a4 A_DesignStructure' href='#'>Design<br />Structure</a>
//                </td>
//                <td class='td4 Bttn'>
//                    <a class='a5 A_Permissions' href='#'>Permissions</a>
//                </td>
//                <td class='td5_a Bttn' On_Click='BttnClick_Rename'       >Rename</td>
//                <td class='td6 Bttn' On_Click='BttnClick_Delete' >Drop or<br />Truncate</td>
//                <td class='td7 Bttn' On_Click='BttnClick_Export'         >Export</td>
//            ";
            return @"
                <td class='td1 Label_TableName' On_Click='BttnClick_Name'></td>
                <td class='td2 Bttn Bttn_ShowHide' On_Click='BttnClick_Hide'>Hide</td>
                <td class='td3 Bttn'>
                    <a class='a4 A_DesignStructure' href='#'>Design<br />Structure</a>
                </td>
                <td class='td5_a Bttn' On_Click='BttnClick_Rename'       >Rename</td>
                <td class='td6 Bttn' On_Click='BttnClick_Delete' >Delete<br />Table</td>
                <td class='td7'>&nbsp;</td>
            ";
        }
    }
}