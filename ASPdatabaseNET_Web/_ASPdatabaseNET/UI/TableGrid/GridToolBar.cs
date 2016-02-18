using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.TableGrid.Objs;

namespace ASPdatabaseNET.UI.TableGrid
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class GridToolBar : MRBPattern<GridResponse, GridViewModel>
    {
        public JsEvent_BeforeAfter OnRefresh = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnImportExportClick = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnViewOptionsClick = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnRequestToDeleteSelection = new JsEvent_BeforeAfter();

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public GridToolBar()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='GridToolBar jRoot NoSelect'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if (this.Model == null)
                return;
            
            jF(".TitleLabel").html(this.Model.TableName_FullNameLabel);

            jF(".Bttn_RecordCount").html("(" + this.Model.Count_DisplayItems + " of " + this.Model.Count_TotalItems + ")");
            this.SelectionCountChanged();

            if (this.Model.TableType == GridRequest.TableTypes.Table)
                jF(".Bttn_DesignTable").show().attr("href", "#00-TableDesign-" + this.Model.Id);
            else
                jF(".Bttn_DesignTable").hide();

            jF(".Bttn_Insert").hide();
            jF(".Bttn_Download").hide();
            jF(".Bttn_DesignTable").hide();
            if (this.Model.TableType == GridRequest.TableTypes.Table)
            {
                if (this.Model.IsAdmin) jF(".Bttn_DesignTable").show();
                if (this.Model.PermissionValues.Insert) jF(".Bttn_Insert").show();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void SelectionCountChanged()
        {
            if (this.Model.PermissionValues.Delete && this.Model.TableType == GridRequest.TableTypes.Table)
                if (this.ViewModel.SelectionCount < 1)
                {
                    jF(".Bttn_Delete").hide();
                    jF(".Bttn_RecordCount").show();
                }
                else
                {
                    jF(".Bttn_RecordCount").hide();
                    jF(".Bttn_Delete").show().html("Delete Selected (" + this.ViewModel.SelectionCount + ")");
                }
        }
        //----------------------------------------------------------------------------------------------------
        public void SetFilterCount(int count)
        {
            if (count < 0)
                jF(".Bttn_ViewOptions").html("View Options");
            else
                jF(".Bttn_ViewOptions").html("View Options (" + count + ")");
        }




        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_RecordCount()
        {
        }
        public void BttnClick_DeleteSelected()
        {
            int n = this.ViewModel.SelectionCount;
            string msg = "Are you sure you want to permanently delete this record?";
            if (n > 1)
                msg = "Are you certain you want to permanently delete these " + n + " records?";
            msg += "\n\nIf so, please type \"delete\" to confirm.";

            string promptResponse = prompt(msg);
            if (promptResponse != null && promptResponse.As<JsString>().toLowerCase() == "delete")
                this.OnRequestToDeleteSelection.After.Fire();
        }
        public void BttnClick_Refresh()
        {
            this.OnRefresh.After.Fire();
        }
        public void BttnClick_Insert()
        {
            window.location = ("#00-Record-" + this.Model.UniqueKey_ForNewRecord).As<Location>();
        }
        public void BttnClick_Download()
        {
            alert("BttnClick_Download");
        }
        public void BttnClick_ImportExport()
        {
            this.OnImportExportClick.After.Fire();
        }
        public void BttnClick_ViewOptions()
        {
            this.OnViewOptionsClick.After.Fire();
        }





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = ""
                + ViewOptions.GetCssTree()
                + GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .GridToolBar { position:relative; background: #093a79; color: #fff; line-height: 2.1875em; margin-bottom: 5px;
                               white-space:nowrap; overflow:hidden; }
                .GridToolBar .TitleLabel { font-size: 1em; padding-left: 20px;
                                           white-space:nowrap; overflow:hidden; }
                .GridToolBar .RightSide { position:absolute; top: 0px; right: 0px; line-height: 2.1875em; background: #093a79; }


                .GridToolBar .RightSide .Bttn { font-size: .75em; display:block; float:right; color:#fff; padding: 0em .6em; widthz: 2.5em; text-align: center; cursor:pointer; }
                .GridToolBar .RightSide .Bttn:hover { background: #001d44; }
                .GridToolBar .RightSide .Bttn_ViewOptions { width:auto; background: #eb640a; padding: 0em 1.4em; margin-left: .4em; }
                .GridToolBar .RightSide .Bttn_ViewOptions:hover { background: #f4f4f4; color: #eb640a; }
                .GridToolBar .RightSide .Bttn_ImportExport_ { line-height: 1.2625em; padding: .3em .64em; font-size: .7em; }
                .GridToolBar .RightSide .Bttn_DesignTable { }
                .GridToolBar .RightSide .Bttn_Download { }
                .GridToolBar .RightSide .Bttn_Insert { }
                .GridToolBar .RightSide .Bttn_Refresh { }
                .GridToolBar .RightSide .Bttn_Delete { display:none; font-size: .6em; width: auto; padding: 0em 1.4em;
                                                       line-height: 2.2em; background: #ffffb1; color: #093a79; margin: .75em 1em 0em; border-radius: .9em; }
                .GridToolBar .RightSide .Bttn_Delete:hover { background: #ffff00; color: #000; }
                .GridToolBar .RightSide .Bttn_RecordCount { font-size: .6em; width: auto; padding: 0em 1.4em; cursor:default; }
                .GridToolBar .RightSide .Bttn_RecordCount:hover { background: #093a79; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TitleLabel YesSelect'></div>
                <div class='RightSide'>
                    <div class='Bttn Bttn_ViewOptions'  On_Click='BttnClick_ViewOptions'    title='View Options'    >View Options</div>
                    <div class='Bttn Bttn_ImportExport' On_Click='BttnClick_ImportExport'   title='Export' >Export</div>
                    <a   class='Bttn Bttn_DesignTable'  href='#'                            title='Design Table'    >Design</a>
                    <div class='Bttn Bttn_Download'     On_Click='BttnClick_Download'       title='Download'        >(D)</div>
                    <div class='Bttn Bttn_Insert'       On_Click='BttnClick_Insert'         title='Insert'          >+ New</div>
                    <div class='Bttn Bttn_Refresh'      On_Click='BttnClick_Refresh'        title='Refresh'         >Refresh</div>
                    <div class='Bttn Bttn_Delete'       On_Click='BttnClick_DeleteSelected' title='Delete Selected' >Delete Selected (0)</div>
                    <div class='Bttn Bttn_RecordCount'  On_Click='BttnClick_RecordCount'    title=''                >(0 of 0)</div>
                </div>
            ";
        }
    }
}
