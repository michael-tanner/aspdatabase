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
    public class SQLViewsPanel_MenuRow : MRBPattern<BasicAssetInfo, string>
    {
        public bool HoldsHiddenItems = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public SQLViewsPanel_MenuRow(bool holdsHiddenItems)
        {
            this.HoldsHiddenItems = holdsHiddenItems;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='SQLViewsPanel_MenuRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            if (this.Model != null)
            {
                jF2(".Label_SQLViewName").html(this.GetDisplayName());
            }
            if (this.HoldsHiddenItems)
                jF2(".td2").html("Unhide");
            else 
                jF2(".td2").html("Hide");

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
        public void BttnClick_Name()
        {
            alert("BttnClick_Name: " + this.Model.Schema + "." + this.Model.GenericName);
        }
        public void BttnClick_Hide()
        {
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Views_ShowHide_Response")
                .ManageAssets__Views_ShowHide(this.Model.ConnectionId, this.Model.GenericId, !this.HoldsHiddenItems);
        }
        public void Views_ShowHide_Response(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.OnChange.After.Fire();
        }
        public void BttnClick_Permissions()
        {
            alert("02");
        }
        public void BttnClick_Rename()
        {
            string newName = prompt("New View Name", this.Model.GenericName);
            if (newName != null)
                ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Rename_Return")
                    .ManageAssets__Views_Rename(this.Model.ConnectionId, this.Model.GenericId, newName);
        }
        public void Rename_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.OnChange.After.Fire();
        }
        public void BttnClick_EditSQL()
        {
            alert("03");
        }
        public void BttnClick_Delete()
        {
            alert("04");
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
                .SQLViewsPanel_MenuRow { font-size: .9em; }
                .SQLViewsPanel_MenuRow td { border: 1px solid #c3c3c3; border-top-width: 0px; line-height: 40px; }
                .SQLViewsPanel_MenuRow .td1 { color: #aaa; border-left-width: 0px; padding-left: 10px; } 
                .SQLViewsPanel_MenuRow .td1 span { color: #222; }
                .SQLViewsPanel_MenuRow .td2 { width: 60px; }
                .SQLViewsPanel_MenuRow .td3 { width: 100px; }
                .SQLViewsPanel_MenuRow .td4 { width: 70px; }
                .SQLViewsPanel_MenuRow .td5 { width: 80px; }
                .SQLViewsPanel_MenuRow .td6 { width: 70px; }
                .SQLViewsPanel_MenuRow .Bttn { color: #14498f; text-align:center; font-size: .78em; cursor: pointer; }
                .SQLViewsPanel_MenuRow .Bttn:hover { background: #888; color:#fff; }

                .SQLViewsPanel_MenuRow:hover { background: #ececec; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='td1 Label_SQLViewName' On_Click='BttnClick_Name'></td>
                <td class='td2 Bttn' On_Click='BttnClick_Hide'        >Hide</td>
                <td class='td4 Bttn' On_Click='BttnClick_Rename'      >Rename</td>
                <td class='td5 Bttn hide' On_Click='BttnClick_EditSQL'     >Edit SQL</td>
                <td class='td6 Bttn hide' On_Click='BttnClick_Delete'      >Delete</td>
            ";
        }
    }
}