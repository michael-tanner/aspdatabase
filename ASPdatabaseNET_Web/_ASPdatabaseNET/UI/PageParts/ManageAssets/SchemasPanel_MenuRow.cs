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
    public class SchemasPanel_MenuRow : MRBPattern<BasicAssetInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public SchemasPanel_MenuRow()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='SchemasPanel_MenuRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            if (this.Model != null)
            {
                jF2(".Label_SchemaName").html(this.Model.Schema);
            }
            this.BindUI();
        }


        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_Name()
        {
            // nothing to do here
        }
        public void BttnClick_Delete()
        {
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Delete_Response")
                .ManageAssets__Schemas__Delete(this.Model.ConnectionId, this.Model.Schema);
        }
        public void Delete_Response(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.OnChange.After.Fire();
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
                .SchemasPanel_MenuRow { font-size: .9em; }
                .SchemasPanel_MenuRow td { border: 1px solid #c3c3c3; border-top-width: 0px; line-height: 40px; }
                .SchemasPanel_MenuRow .td1 { border-left-width: 0px; padding-left: 10px; }
                .SchemasPanel_MenuRow .td2 { width: 165px; }
                .SchemasPanel_MenuRow .Bttn { color: #14498f; text-align:center; font-size: .78em; cursor: pointer; }
                .SchemasPanel_MenuRow .Bttn:hover { background: #888; color:#fff; }

                .SchemasPanel_MenuRow:hover { background: #ececec; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='td1 Label_SchemaName' On_Click='BttnClick_Name'></td>
                <td class='td2 Bttn' On_Click='BttnClick_Delete' >Delete</td>
            ";
        }
    }
}