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
    public class TableGridMain : MRBPattern<GridResponse, GridViewModel>
    {
        public GridToolBar GridToolBar;
        public ImportExportUI ImportExportUI;
        public ViewOptions ViewOptions;
        public RowUI[] RowUIs;
        public RecordViewerUI RecordViewerUI;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public TableGridMain()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='TableGridMain jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            try
            {
                string json = localStorage.getItem(this.ViewModel.LocalStorage_Key_ViewOptions);
                if (json != null && json.Length > 0)
                {
                    var gridRequest_Local = (new ASPdb.Ajax.AjaxHelper()).FromJson<GridRequest>(json);
                    this.ViewModel.DisplayTopNRows = gridRequest_Local.DisplayTopNRows;
                    this.ViewModel.FilterAndSort.FilterFields = gridRequest_Local.FilterFields;
                    this.ViewModel.FilterAndSort.SortFields = gridRequest_Local.SortFields;
                }
            }
            catch { }

            this.GridToolBar = new GridToolBar();
            this.GridToolBar.ViewModel = this.ViewModel;
            this.GridToolBar.Instantiate();
            this.GridToolBar.OnRefresh.After.AddHandler(this, "GridToolBar_OnRefresh", 0);
            this.GridToolBar.OnImportExportClick.After.AddHandler(this, "GridToolBar_OnImportExportClick", 0);
            this.GridToolBar.OnViewOptionsClick.After.AddHandler(this, "GridToolBar_OnViewOptionsClick", 0);
            this.GridToolBar.OnRequestToDeleteSelection.After.AddHandler(this, "GridToolBar_OnRequestToDeleteSelection", 0);
            jF(".Holder_GridToolBar").append(this.GridToolBar.jRoot);

            this.ImportExportUI = new ImportExportUI();
            this.ImportExportUI.Instantiate();
            this.ImportExportUI.Close();
            jF(".Holder_ImportExportUI").append(this.ImportExportUI.jRoot);

            this.ViewOptions = new ViewOptions();
            this.ViewOptions.ViewModel = this.ViewModel;
            this.ViewOptions.Instantiate();
            this.ViewOptions.OnRefresh.After.AddHandler(this, "ViewOptions_OnRefresh", 0);
            this.ViewOptions.Close();
            jF(".Holder_ViewOptions").append(this.ViewOptions.jRoot);

            AjaxService.ASPdatabaseService.New(this, "GetGrid_Return").TableGrid__GetGrid(this.GetGridRequest());
            UI.PagesFramework.BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        public GridRequest GetGridRequest()
        {
            var rtn = new GridRequest();
            rtn.TableType = this.ViewModel.TableType;
            rtn.Id = this.ViewModel.Id;
            rtn.DisplayTopNRows = this.ViewModel.DisplayTopNRows;

            this.ViewOptions.Update_FiltersAndSorts();
            rtn.FilterFields = this.ViewModel.FilterAndSort.FilterFields;
            rtn.SortFields = this.ViewModel.FilterAndSort.SortFields;
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private bool LastAttemptWas_InvalidFilterField = false;
        public void GetGrid_Return(ASPdb.Ajax.AjaxResponse response)
        {
            if (UI.Errors.ExceptionHandler.Check(response))
            {
                if (response.Error.Message == "Invalid FilterField." && !this.LastAttemptWas_InvalidFilterField)
                {
                    this.LastAttemptWas_InvalidFilterField = true;
                    this.ViewModel.FilterAndSort = new FilterAndSort();
                    this.ViewModel.FilterAndSort.FilterFields = new ViewOptions_FilterField[0];
                    this.ViewModel.FilterAndSort.SortFields = new ViewOptions_SortField[0];
                    this.ViewOptions.BttnClick_ApplyAndRefresh();
                }
                else
                    return;
            }
            this.LastAttemptWas_InvalidFilterField = false;

            if (this.RecordViewerUI != null)
                this.RecordViewerUI = null;

            this.Model = response.ReturnObj.As<GridResponse>();

            document.title = "[" + this.Model.TableName + "] - ASPdatabase.NET";

            if (this.ViewModel.FilterAndSort.FilterFields != null && this.ViewModel.FilterAndSort.FilterFields.Length > 0)
                this.GridToolBar.SetFilterCount(this.ViewModel.FilterAndSort.FilterFields.Length);
            else if (this.ViewModel.FilterAndSort.SortFields != null && this.ViewModel.FilterAndSort.SortFields.Length > 0)
                this.GridToolBar.SetFilterCount(0);
            else
                this.GridToolBar.SetFilterCount(-1);

            this.ViewModel.SelectionCount = 0;
            this.ViewModel.AllColumnNames = new string[0];
            for (int i = 0; i < this.Model.HeaderItems.Length; i++)
                this.ViewModel.AllColumnNames[i] = this.Model.HeaderItems[i].FieldName;
            this.BindUI();

            this.GridToolBar.Model = this.Model;
            this.GridToolBar.Open();


            var tableGridTable = jF(".TableGridTable").html("");

            string headerHTML = "<tr>" + "<th class='SelectAllBox' title='Select/Deselect All'><span></span></th>" + "<th></th>";
            for (int i = 0; i < this.Model.HeaderItems.Length; i++)
                headerHTML += JsStr.StrFormat1("<th>{0}</th>", this.Model.HeaderItems[i].FieldName);
            headerHTML += "</tr>";
            tableGridTable.html(headerHTML);
            var thisObj = this;
            var jRootObj = this.jRoot;
            eval("jRootObj.find('.SelectAllBox').click(function(){ thisObj.SelectAllBox_Click(); });");

            this.RowUIs = new RowUI[0];
            for (int i = 0; i < this.Model.Rows.Length; i++)
            {
                this.RowUIs[i] = new RowUI();
                this.RowUIs[i].IsOdd = (i % 2 == 1);
                this.RowUIs[i].ViewModel = this.ViewModel;
                this.RowUIs[i].Model = this.Model.Rows[i];
                this.RowUIs[i].Model.DisplayIndex = i;
                this.RowUIs[i].OnSelection.After.AddHandler(this, "RowUI_OnSelection", 0);
                this.RowUIs[i].OnOpenViewer.After.AddHandler(this, "RowUI_OnOpenViewer", 1);
                this.RowUIs[i].Instantiate();
                tableGridTable.append(this.RowUIs[i].jRoot);
            }

            UI.PagesFramework.BasePage.WindowResized();
        }


        //---------------------------------------------------------------------------------- event handlers --
        public void GridToolBar_OnImportExportClick()
        {
            this.ImportExportUI.ViewModel.IsInDemoMode = this.Model.IsInDemoMode;
            if (this.ImportExportUI.IsOpen)
                this.ImportExportUI.Close();
            else
            {
                this.ImportExportUI.ViewModel.GridRequest = this.GetGridRequest();
                //this.ImportExportUI.ViewModel.FilterAndSort = this.ViewModel.FilterAndSort;
                this.ImportExportUI.Open();
            }
        }
        public void GridToolBar_OnViewOptionsClick()
        {
            if (this.ViewOptions.IsOpen)
                this.ViewOptions.Close();
            else
                this.ViewOptions.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void Refresh()
        {
            AjaxService.ASPdatabaseService.New(this, "GetGrid_Return").TableGrid__GetGrid(this.GetGridRequest());
        }
        //----------------------------------------------------------------------------------------------------
        public void GridToolBar_OnRefresh()
        {
            this.Refresh();
        }
        //----------------------------------------------------------------------------------------------------
        public void ViewOptions_OnRefresh()
        {
            this.Refresh();
        }
        //----------------------------------------------------------------------------------------------------
        public void RowUI_OnSelection()
        {
            int count = 0;
            for (int i = 0; i < this.RowUIs.Length; i++)
                if (this.RowUIs[i].IsSelected)
                    count++;
            this.ViewModel.SelectionCount = count;
            this.GridToolBar.SelectionCountChanged();
        }
        //----------------------------------------------------------------------------------------------------
        public void SelectAllBox_Click()
        {
            bool selection = (this.ViewModel.SelectionCount < 1);
            for (int i = 0; i < this.RowUIs.Length; i++)
                if (this.RowUIs[i].IsSelected != selection)
                    this.RowUIs[i].SetSelection(selection);
            this.RowUI_OnSelection();
        }
        //----------------------------------------------------------------------------------------------------
        public void GridToolBar_OnRequestToDeleteSelection()
        {
            var keysToDelete = new string[0];
            int j = 0;
            for (int i = 0; i < this.RowUIs.Length; i++)
                if (this.RowUIs[i].IsSelected)
                    keysToDelete[j++] = this.RowUIs[i].Model.UniqueKey;

            AjaxService.ASPdatabaseService.New(this, "DeleteRecords_Return").TableGrid__DeleteRecords(keysToDelete);
        }
        //----------------------------------------------------------------------------------------------------
        public void DeleteRecords_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Refresh();
        }

        //----------------------------------------------------------------------------------------------------
        public void RowUI_OnOpenViewer(GridRow gridRow)
        {
            if(this.RecordViewerUI == null)
            {
                this.RecordViewerUI = new RecordViewerUI();
                this.RecordViewerUI.Instantiate();
                this.RecordViewerUI.OnClose.After.AddHandler(this, "RecordViewerUI_OnClose", 0);
                jRoot.append(this.RecordViewerUI.jRoot);
            }
            this.RecordViewerUI.Model = gridRow;
            this.RecordViewerUI.GridResponse = this.Model;

            jF(".FrontLevelPage").hide();
            this.RecordViewerUI.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void RecordViewerUI_OnClose()
        {
            jF(".FrontLevelPage").show();
        }



        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += RowUI.GetCssTree();
            rtn += GridToolBar.GetCssTree();
            rtn += ImportExportUI.GetCssTree();
            rtn += RecordViewerUI.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .TableGridMain { margin: 12px; }
                .TableGridMain .FrontLevelPage { display:block; position:relative; }

                .TableGridMain .Holder_GridToolBar { max-width: 880px; }
                .TableGridMain .Holder_ImportExportUI { max-width: 880px; min-width: 55em; position:absolute; top: 2.25em; left: 0em; z-index: 1000; }
                .TableGridMain .Holder_ViewOptions { max-width: 880px; min-width: 55em; position:absolute; top: 2.25em; left: 0em; z-index: 1000; }

                .TableGridMain .TableScrollDiv { font-size: .9em; }
                .TableGridMain .TableScrollDiv .TableGridTable { font-size: .85em; }
                .TableGridMain .TableScrollDiv .TableGridTable tr th { background: #464f5b; color: #fff; font-weight: normal; text-align: left; white-space:nowrap; 
                                                                       border: 1px solid #fff; border-width: 1px 1px 1px 0px; padding: 2px 6px; }

                .TableGridMain .TableScrollDiv .TableGridTable .SelectAllBox { cursor:pointer; }
                .TableGridMain .TableScrollDiv .TableGridTable .SelectAllBox:hover { background: #42a4ed; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='FrontLevelPage'>
                <div class='Holder_GridToolBar'></div>
                <div class='Holder_ImportExportUI'></div>
                <div class='Holder_ViewOptions'></div>

                <div class='TableScrollDiv AutoResizeXY' AutoResize_RightSpace='0'>
                    <table class='TableGridTable'>
                    </table>
                </div>
            </div>
            ";
        }
    }
}