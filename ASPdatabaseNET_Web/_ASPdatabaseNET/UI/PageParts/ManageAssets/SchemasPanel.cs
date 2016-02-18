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
    /// <summary>ViewModel is ConnectionId</summary>
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class SchemasPanel : MRBPattern<BasicAssetInfo[], int>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public SchemasPanel()
        {
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='SchemasPanel jRoot'>");
            jRoot.append(this.GetHtmlRoot());
            jRoot.hide();

            this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            UI.PagesFramework.BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        public void OnModel_Set()
        {
            this.Refresh();
        }
        //----------------------------------------------------------------------------------------------------
        public void Refresh()
        {
            var table = jF2(".MenuTable");
            table.find(".jRoot").remove();
            if (this.Model != null)
            {
                for (int i = 0; i < this.Model.Length; i++)
                {
                    var menuRow = new SchemasPanel_MenuRow();
                    menuRow.Model = this.Model[i];
                    menuRow.Instantiate();
                    menuRow.OnChange.After.AddHandler(this, "SubItem_Changed", 0);
                    table.append(menuRow.jRoot);
                }
            }
        }


        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_NewSchema()
        {
            jF2(".NewSchemaBox").show();
            jF2(".Txt_NewSchemaName").focus();
            jF2(".Txt_NewSchemaName").val("");
        }
        public void BttnClick_SaveNewSchema()
        {
            jF2(".NewSchemaBox").hide();
            jF2(".NewSchemaBox_PleaseWait").show();
            string newSchemaName = jF2(".Txt_NewSchemaName").val().As<string>();
            AjaxService.ASPdatabaseService.New(this, "Save_Response").ManageAssets__Schemas__SaveNew(this.ViewModel, newSchemaName);
        }
        public void Save_Response(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            jF2(".NewSchemaBox_PleaseWait").hide();
            jF2(".Txt_NewSchemaName").val("");
            this.OnChange.After.Fire();
        }
        public void BttnClick_CancelNewSchema()
        {
            jF2(".NewSchemaBox").hide();
        }
        public void SubItem_Changed()
        {
            this.OnChange.After.Fire();
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += SchemasPanel_MenuRow.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .SchemasPanel { padding-top: 0px; }

                .SchemasPanel .BttnsBar { margin: 12px 0px; position:relative; }
                .SchemasPanel .BttnsBar .Bttn { float:right; background: #14498f; color: #fff; line-height: 28px;
                                               padding: 0px 20px; cursor: pointer; margin-left: 20px; font-size: .9em; }
                .SchemasPanel .BttnsBar .Bttn:hover { background: #1d2d42; }

                .SchemasPanel .BttnsBar .NewSchemaBox { display:none; position:absolute; right: 0px; width: 500px; color: #fff; 
                                                        background: #14498f; font-size: .8em; padding: 9px 14px; }
                .SchemasPanel .BttnsBar .NewSchemaBox input { width: 190px; margin-left: 11px; }
                .SchemasPanel .BttnsBar .NewSchemaBox .Bttn1 { float:right; width:auto; cursor:pointer; border: 1px solid #6185b4;
                                                               margin-left: 9px; padding: 1px 8px; }
                .SchemasPanel .BttnsBar .NewSchemaBox .Bttn1:hover { background: #000; border: 1px solid #000; }
                .SchemasPanel .BttnsBar .NewSchemaBox_PleaseWait { display:none; position:absolute; right: 0px; width: 500px; color: #fff; 
                                                        background: #14498f; font-size: .8em; padding: 9px 14px;  }


                .SchemasPanel .MenuTable { width: 100%; }
                .SchemasPanel .MenuTable th { font-weight: normal; color: #878787; font-size: .65em; border-bottom: 1px solid #c3c3c3; 
                                             text-align: left; padding: 0px 0px 1px 5px; }
                .SchemasPanel .MenuTable .th1 { }
                .SchemasPanel .MenuTable .th2 { text-align: center; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"

                <div class='BttnsBar'>
                    <div class='NewSchemaBox'>
                        New Schema Name
                        <input type='text' class='Txt_NewSchemaName' />
                        <div class='Bttn1 Bttn_CancelNewSchema' On_Click='BttnClick_CancelNewSchema'>Cancel</div>
                        <div class='Bttn1 Bttn_SaveNewSchema' On_Click='BttnClick_SaveNewSchema'>Save</div>
                        <div class='clear'></div>
                    </div>
                    <div class='NewSchemaBox_PleaseWait'>Please Wait</div>
                    <div class='Bttn' On_Click='BttnClick_NewSchema'>+ New Schema</div>
                    <div class='clear'></div>
                </div>

                <div class='AutoResize'>
                    <table class='MenuTable'>
                        <tr>
                            <th class='th1'>Schema Name</th>
                            <th class='th2'>(Only if Schema is empty)</th>
                        </tr>
                    </table>
                </div>
                
            ";
        }
    }
}