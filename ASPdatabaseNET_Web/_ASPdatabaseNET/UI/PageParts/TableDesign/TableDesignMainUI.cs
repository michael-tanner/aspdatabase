using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DbInterfaces.TableObjects;
using ASPdatabaseNET.DataObjects.TableDesign;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TableDesignMainUI : MRBPattern<TableStructure, TableDesign_ViewModel>
    {
        public enum PanelTypes { NotSet, Columns, PrimaryKey, ForeignKeys, Indexes, AppProperties };

        public PanelTypes LastPanelTypeOnSaved = PanelTypes.NotSet;
        public ColumnsPanel ColumnsPanel;
        public PrimaryKeyPanel PrimaryKeyPanel;
        public ForeignKeysPanel ForeignKeysPanel;
        public IndexesPanel IndexesPanel;
        public AppPropertiesPanel AppPropertiesPanel;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public TableDesignMainUI()
        {
            this.ViewModel = new TableDesign_ViewModel();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='TableDesignMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.ColumnsPanel = new ColumnsPanel(this.ViewModel);
            this.PrimaryKeyPanel = new PrimaryKeyPanel(this.ViewModel);
            this.ForeignKeysPanel = new ForeignKeysPanel(this.ViewModel);
            this.IndexesPanel = new IndexesPanel(this.ViewModel);
            this.AppPropertiesPanel = new TableDesign.AppPropertiesPanel(this.ViewModel);
            var panelsHolder = jF2(".PanelsHolder");
            panelsHolder.append(this.ColumnsPanel.jRoot);
            panelsHolder.append(this.PrimaryKeyPanel.jRoot);
            panelsHolder.append(this.ForeignKeysPanel.jRoot);
            panelsHolder.append(this.IndexesPanel.jRoot);
            panelsHolder.append(this.AppPropertiesPanel.jRoot);

            this.PrimaryKeyPanel.OnChange.After.AddHandler(this, "PrimaryKeyPanel_Changed", 0);
            this.ForeignKeysPanel.OnChange.After.AddHandler(this, "ForeignKeysPanel_Changed", 0);
            this.IndexesPanel.OnChange.After.AddHandler(this, "IndexesPanel_Changed", 0);

            this.ColumnsPanel.OnHasPendingChanges.After.AddHandler(this, "OnSubPanel_HasPendingChanges", 0);
            this.ColumnsPanel.OnGotoTab.After.AddHandler(this, "OnGotoTab_Method", 1);
            this.ColumnsPanel.OnSaved.After.AddHandler(this, "OnSubPanelSaved", 1);
        }
        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            this.Get_HashInputs();
            this.ViewModel.AllTables_InDb = null;
            
            jF2(".TopSection_CreateTable").hide();
            jF2(".TopSection_DesignTable").hide();
            jF2(".Tab").hide();
            jF2(".PanelsHolder").hide();
            jF2(".Bttn_Close").html("Close Designer");
            if (this.ViewModel.IsCreateNew)
            {
                AjaxService.ASPdatabaseService.New(this, "GetInfo_ForCreateNew_Return")
                    .TableDesign__GetInfo_ForCreateNew(this.ViewModel.ConnectionId);
            }
            else
            {
                AjaxService.ASPdatabaseService.New(this, "GetInfo_ForModify_Return")
                    .TableDesign__GetInfo_ForModify(this.ViewModel.TableId);
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void Get_HashInputs()
        {
            this.ViewModel.ConnectionId = -1;
            this.ViewModel.TableId = -1;
            try
            {
                string input1 = PagesFramework.PageIdentifier.GetFromUrlHash().PageParam2;
                string input2 = PagesFramework.PageIdentifier.GetFromUrlHash().PageParam3;

                string input1_L = JsStr.S(input1).ToLower().String;
                if (input1_L == "new")
                {
                    this.ViewModel.IsCreateNew = true;
                    if (!isNaN(input2))
                        this.ViewModel.ConnectionId = 1 * input2.As<JsNumber>();
                }
                else
                {
                    this.ViewModel.IsCreateNew = false;
                    if (!isNaN(input1))
                        this.ViewModel.TableId = 1 * input1.As<JsNumber>();
                }
            }
            catch { }
        }



        //------------------------------------------------------------------------------------------ Events --
        public void GetInfo_ForCreateNew_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var response = ajaxResponse.ReturnObj.As<TableDesignResponse>();
            this.ViewModel.Set(response);
            this.ViewModel.IsCreateNew = true;

            this.Model = new TableStructure();
            this.Model.ConnectionId = this.ViewModel.ConnectionId;
            this.ColumnsPanel.Model = this.Model;
            this.ViewModel.TableStructure = this.Model;
            

            jF2(".TopSection_CreateTable").show();
            jF2(".Label1_ConnectionName").html(response.ConnectionName);
            jF2(".Txt_TableName").focus().val("");
            jF2(".Tab_ForCreate").show();
            jF2(".PanelsHolder").show();

            var select_Schema = jF2(".Select_Schema");
            select_Schema.html("");
            for (int i = 0; i < response.Schemas.Length; i++)
            {
                if (JsStr.S(response.Schemas[i]).ToLower().String == "dbo")
                    select_Schema.append("<option selected='selected'>" + response.Schemas[i] + "</option>");
                else
                    select_Schema.append("<option>" + response.Schemas[i] + "</option>");
            }
            this.TabClick_Columns();

            ASPdatabaseNET.UI.PagesFramework.BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        public void GetInfo_ForModify_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var response = ajaxResponse.ReturnObj.As<TableDesignResponse>();
            this.ViewModel.Set(response);
            this.ViewModel.IsCreateNew = false;
            this.Model = response.TableStructure;

            this.ColumnsPanel.Model = this.Model;

            jF2(".TopSection_DesignTable").show();
            jF2(".Tab_ForModify").show();
            jF2(".PanelsHolder").show();

            this.BindUI();
            this.Populate_JumpToSelect();
            switch (this.LastPanelTypeOnSaved)
            {
                case PanelTypes.Columns: this.TabClick_Columns(); break;
                case PanelTypes.PrimaryKey: this.TabClick_PrimaryKey(); break;
                case PanelTypes.ForeignKeys: this.TabClick_ForeignKeys(); break;
                case PanelTypes.Indexes: this.TabClick_Indexes(); break;
                case PanelTypes.AppProperties: this.TabClick_AppProperties(); break;
                default: this.TabClick_Columns(); break;
            }
            this.LastPanelTypeOnSaved = PanelTypes.NotSet;

            ASPdatabaseNET.UI.PagesFramework.BasePage.WindowResized();
        }

        //----------------------------------------------------------------------------------------------------
        private void Populate_JumpToSelect()
        {
            var select = jF2(".Select_JumpTo");
            select.html("<option value='-1'></option>");
            var tables = this.ViewModel.AllTables_InDb;
            if (tables == null)
                return;
            for (int i = 0; i < tables.Length; i++)
            {
                var item = tables[i];
                string s = JsStr.StrFormat3("<option value='{0}'>{1}.{2}</option>", item.TableId.As<string>(), item.Schema, item.TableName);
                select.append(s);
            }
        }


        //----------------------------------------------------------------------------------------------------
        public void BttnClick_Create()
        {
            this.Model.Schema = jF2(".Select_Schema").val().As<string>();
            this.Model.TableName = jF2(".Txt_TableName").val().As<string>();
            this.ColumnsPanel.SaveToModel();
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "CreateTable_Return").TableDesign__CreateTable(this.Model);
        }
        public void CreateTable_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var response = ajaxResponse.ReturnObj.As<TableDesignResponse>();
            if (response.TableId < 0)
            {
                alert("Could not determin new TableId");
                this.BttnClick_Cancel();
            }
            else
            {
                string url = "#00-TableDesign-" + response.TableId;
                eval("window.location = url;");
            }
        }


        //----------------------------------------------------------------------------------------------------
        public void BttnClick_Cancel()
        {
            string url = "#00-ManageAssets-" + this.ViewModel.ConnectionId;
            if (this.ViewModel.ConnectionId < 0)
                url = "#00-StartPage";
            eval("window.location = url;");
        }
        //----------------------------------------------------------------------------------------------------
        public void TabClick_Columns()
        {
            this.CloseAllTabs();
            jF2(".Tab_Columns").addClass("Selected").addClass("Tab_Columns_Selected");
            this.ColumnsPanel.Open();
        }
        public void TabClick_PrimaryKey()
        {
            this.CloseAllTabs();
            jF2(".Tab_PrimaryKey").addClass("Selected").addClass("Tab_PrimaryKey_Selected");
            this.PrimaryKeyPanel.Open();
        }
        public void TabClick_ForeignKeys()
        {
            this.CloseAllTabs();
            jF2(".Tab_ForeignKeys").addClass("Selected").addClass("Tab_ForeignKeys_Selected");
            this.ForeignKeysPanel.Open();
        }
        public void TabClick_Indexes()
        {
            this.CloseAllTabs();
            jF2(".Tab_Indexes").addClass("Selected").addClass("Tab_Indexes_Selected");
            this.IndexesPanel.Open();
        }
        public void TabClick_AppProperties()
        {
            this.CloseAllTabs();
            jF2(".Tab_AppProperties").addClass("Selected").addClass("Tab_AppProperties_Selected");
            this.AppPropertiesPanel.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void CloseAllTabs()
        {
            jF2(".Tab").removeClass("Selected")
                .removeClass("Tab_Columns_Selected")
                .removeClass("Tab_PrimaryKey_Selected")
                .removeClass("Tab_ForeignKeys_Selected")
                .removeClass("Tab_Indexes_Selected")
                .removeClass("Tab_AppProperties_Selected");

            if (this.ColumnsPanel != null && this.ColumnsPanel.IsOpen)
                this.ColumnsPanel.Close();
            if (this.PrimaryKeyPanel != null && this.PrimaryKeyPanel.IsOpen)
                this.PrimaryKeyPanel.Close();
            if (this.ForeignKeysPanel != null && this.ForeignKeysPanel.IsOpen)
                this.ForeignKeysPanel.Close();
            if (this.IndexesPanel != null && this.IndexesPanel.IsOpen)
                this.IndexesPanel.Close();
            if (this.AppPropertiesPanel != null && this.AppPropertiesPanel.IsOpen)
                this.AppPropertiesPanel.Close();
        }

        //----------------------------------------------------------------------------------------------------
        public void OnSubPanel_HasPendingChanges()
        {
            jF2(".Bttn_Close").html("Undo Changes & Close Designer");
        }
        public void OnGotoTab_Method(string tabType)
        {
            switch (tabType)
            {
                case "PK": this.TabClick_PrimaryKey();
                    break;
                case "FK": this.TabClick_ForeignKeys();
                    break;
                case "IX": this.TabClick_Indexes();
                    break;
            }
        }
        public void OnSubPanelSaved(string strPanelType)
        {
            switch (strPanelType)
            {
                case "Columns":
                    this.LastPanelTypeOnSaved = PanelTypes.Columns;
                    break;
                case "ForeignKeys":
                    this.LastPanelTypeOnSaved = PanelTypes.ForeignKeys;
                    break;
                case "PrimaryKey":
                    this.LastPanelTypeOnSaved = PanelTypes.PrimaryKey;
                    break;
                case "Indexes":
                    this.LastPanelTypeOnSaved = PanelTypes.Indexes;
                    break;
                case "AppProperties":
                    this.LastPanelTypeOnSaved = PanelTypes.AppProperties;
                    break;
                default:
                    this.LastPanelTypeOnSaved = PanelTypes.NotSet;
                    break;
            }
            this.Open();
        }

        //----------------------------------------------------------------------------------------------------
        public void PrimaryKeyPanel_Changed()
        {
            this.LastPanelTypeOnSaved = PanelTypes.PrimaryKey;
            this.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void ForeignKeysPanel_Changed()
        {
            this.LastPanelTypeOnSaved = PanelTypes.ForeignKeys;
            this.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void IndexesPanel_Changed()
        {
            this.LastPanelTypeOnSaved = PanelTypes.Indexes;
            this.Open();
        }

        //----------------------------------------------------------------------------------------------------
        public void JumpTo_Change()
        {
            int tableId = jF2(".Select_JumpTo").val().As<JsNumber>();
            if (tableId < 0)
                return;
            window.location.hash = "#00-TableDesign-" + tableId;
        }
        //----------------------------------------------------------------------------------------------------
        public void TableName_Click()
        {
            window.location = ("#00-Table-" + this.Model.TableId).As<Location>();
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += ColumnsPanel.GetCssTree();
            rtn += PrimaryKeyPanel.GetCssTree();
            rtn += ForeignKeysPanel.GetCssTree();
            rtn += IndexesPanel.GetCssTree();
            rtn += AppPropertiesPanel.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .TableDesignMainUI { width: 946px; }
                .TableDesignMainUI .TopSection_CreateTable { display: none; position: relative; }
                .TableDesignMainUI .TopSection_DesignTable { display: none; position: relative; }


                .TableDesignMainUI .TopSection_CreateTable .NameBar { margin-top: 14px; }
                .TableDesignMainUI .TopSection_CreateTable .NameBar .Dot1 { float: left; margin: 4px 2px 0px 3px; font-size: 1.1em; }
                .TableDesignMainUI .TopSection_CreateTable .NameBar .Dot2 { float: left; margin: 4px 3px 0px 2px; font-size: 1.1em; }
                .TableDesignMainUI .TopSection_CreateTable .NameBar .Label1 {        font-size: 16px; border: 1px solid #f9c364; border-width: 1px 0px 1px 1px; background: #fefe9f; display: block; float: left; line-height: 24px; padding: 0px 8px; max-width: 200px; overflow: hidden; white-space:nowrap; }
                .TableDesignMainUI .TopSection_CreateTable .NameBar .Select_Schema { font-size: 16px; border: 1px solid #f9c364; border-width: 1px 0px 1px 0px; background: #fefe9f; display: block; float: left; height:26px; min-width: 90px; border-left: 1px solid #fefe9f; border-right: 1px solid #fefe9f; }
                .TableDesignMainUI .TopSection_CreateTable .NameBar .Txt_TableName { font-size: 16px; border: 1px solid #f9c364; border-width: 1px 1px 1px 0px; background: #fefe9f; display: block; float: left; line-height: 20px; width: 325px; }
                .TableDesignMainUI .TopSection_CreateTable .NameBar .Label_TableName { float:left; position:relative; top: -14px; left: -122px; font-size: .6em; color: #999; width: 120px; text-align: right; }
                                                     
                .TableDesignMainUI .TopSection_CreateTable .BttnsBar { position: absolute; top: 6px; width: 100%; }
                .TableDesignMainUI .TopSection_CreateTable .BttnsBar .Bttn { float: right; background: #14498f; color: #fff; line-height: 40px; padding: 0px 25px; 
                                                                             cursor: pointer; margin-left: 5px; }
                .TableDesignMainUI .TopSection_CreateTable .BttnsBar .Bttn:hover { background: #333; }
                .TableDesignMainUI .TopSection_CreateTable .BttnsBar .Bttn_Create:hover { background: #e14738; }

                .TableDesignMainUI .TopSection_CreateTable .BttnsBar .Inactive { background: #ddd; cursor: default; }
                .TableDesignMainUI .TopSection_CreateTable .BttnsBar .Inactive:hover { background: #ddd; }




                .TableDesignMainUI .TopSection_DesignTable .NameBar { margin-top: 14px; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .For { color: #aaaaaa; padding-right: 1px; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Label1 { background: #fefe9f; color: #888; padding: 3px 8px; border: 1px solid #f9c364; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Txt_TableRename { display:none; border: 1px solid #14498f; background: #fefe9f; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Label1_Period {         border-width: 1px 0px 1px 0px; padding: 3px 3px; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Label1_ConnectionName { border-width: 1px 0px 1px 1px; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Label1_Schema {         border-width: 1px 0px 1px 0px; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Label1_TableName {      border-width: 1px 1px 1px 0px; cursor:pointer; color: #000; }
                .TableDesignMainUI .TopSection_DesignTable .NameBar .Label1_TableName:hover { background: #fff; }
                

                .TableDesignMainUI .TopSection_DesignTable .BttnsBar { position: absolute; top: 6px; width: 100%; }
                .TableDesignMainUI .TopSection_DesignTable .BttnsBar .Bttn { float: right; background: #14498f; color: #fff; line-height: 40px; padding: 0px 25px; 
                                                                             cursor: pointer; margin-left: 5px; }
                .TableDesignMainUI .TopSection_DesignTable .BttnsBar .Bttn:hover { background: #333; }
                .TableDesignMainUI .TopSection_DesignTable .BttnsBar .Inactive { background: #ddd; cursor: default; }
                .TableDesignMainUI .TopSection_DesignTable .BttnsBar .Inactive:hover { background: #ddd; }

                .TableDesignMainUI .TopSection_DesignTable .JumpToBox { position:absolute; top: 6px; left: 280px; width: 350px; font-size: .8em; color: #999; }
                .TableDesignMainUI .TopSection_DesignTable .JumpToBox .Select_JumpTo { min-width: 250px; max-width: 250px; border-color: #bbb; }


                .TableDesignMainUI .TabsBar { line-height: 28px; color: #222; margin-top: 32px; font-size: .9em; }
                .TableDesignMainUI .TabsBar .Tab { float: left; cursor: pointer; z-index: 2; position: relative; padding-top: 1px;
                                                   width: 135px; text-align: center; background: #e5e5ea; margin-right: 10px; }
                .TableDesignMainUI .TabsBar .Tab:hover { background: #555; color: #fff; }
                .TableDesignMainUI .TabsBar .Selected { line-height: 28px; width: 133px;
                                                        border: 1px solid #585858; border-bottom-width: 0px; background: #fff; }
                .TableDesignMainUI .TabsBar .Tab_Columns { margin-left: 28px; }
                .TableDesignMainUI .TabsBar .Tab_CreateNote {       width: 420px; font-size: .8em; text-align:center; cursor: default; margin-right: 18px;
                                                                    border: 1px solid #dfdfe5; border-width: 1px 1px 0px 1px; padding-top: 0px;
                                                                    background: #fafafa; color: #8c8b8b;  }
                .TableDesignMainUI .TabsBar .Tab_CreateNote:hover { background: #fafafa; color: #8c8b8b; }
                .TableDesignMainUI .TabsBar .HozLine { position: relative; top: -1px; border-bottom: 1px solid #585858; z-index:1; }


                .TableDesignMainUI .TabsBar .Tab_Columns_Selected { background: #979aa5; color: #fff; cursor:default; }
                .TableDesignMainUI .TabsBar .Tab_Columns_Selected:hover { background: #979aa5; }

                .TableDesignMainUI .TabsBar .Tab_PrimaryKey_Selected { background: #f26b0b; color: #fff; cursor:default; }
                .TableDesignMainUI .TabsBar .Tab_PrimaryKey_Selected:hover { background: #f26b0b; }

                .TableDesignMainUI .TabsBar .Tab_ForeignKeys_Selected { background: #0b94da; color: #fff; cursor:default; }
                .TableDesignMainUI .TabsBar .Tab_ForeignKeys_Selected:hover { background: #0b94da; }

                .TableDesignMainUI .TabsBar .Tab_Indexes_Selected { background: #6cc5aa; color: #fff; cursor:default; }
                .TableDesignMainUI .TabsBar .Tab_Indexes_Selected:hover { background: #6cc5aa; }

                .TableDesignMainUI .TabsBar .Tab_AppProperties_Selected { background: #173a67; color: #fff; cursor:default; }
                .TableDesignMainUI .TabsBar .Tab_AppProperties_Selected:hover { background: #173a67; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopSection_CreateTable'>
                    <div class='Head'>Create Table</div>
                    <div class='BttnsBar'>
                        <div class='Bttn Bttn_Cancel' On_Click='BttnClick_Cancel'>Cancel</div>
                        <div class='Bttn Bttn_Create' On_Click='BttnClick_Create'>Create Table</div>
                        <div class='clear'></div>
                    </div>
                    <div class='NameBar'>
                        <div class='Label1 Label1_ConnectionName'></div>
                        <div class='Dot1'>.</div>
                        <select class='Select_Schema'>
                        </select>
                        <div class='Dot2'>.</div>
                        <input type='text' class='Txt_TableName' value='' />
                        <div class='Label_TableName'>Table Name</div>
                        <div class='clear'></div>
                    </div>
                </div>


                <div class='TopSection_DesignTable'>
                    <div class='Head'>Design Table</div>
                    <div class='BttnsBar'>
                        <div class='Bttn Bttn_Close' On_Click='BttnClick_Cancel'>Close Designer</div>
                        <div class='clear'></div>
                    </div>
                    <div class='JumpToBox'>
                        Jump To
                        <select class='Select_JumpTo' On_Change='JumpTo_Change'></select>
                    </div>
                    <div class='NameBar'>
                        <span class='Label1 Label1_ConnectionName' ModelKey='ConnectionName'></span
                            ><span class='Label1 Label1_Period'>.</span><span class='Label1 Label1_Schema' ModelKey='Schema'></span
                            ><span class='Label1 Label1_Period'>.</span><span class='Label1 Label1_TableName' ModelKey='TableName' On_Click='TableName_Click'></span
                            >
                    </div>
                </div>


                <div class='TabsBar'>
                    <div class='Tab Tab_ForModify Tab_ForCreate Tab_Columns'       On_Click='TabClick_Columns'       >Columns</div>
                    <div class='Tab               Tab_ForCreate Tab_CreateNote'                                      >Foreign Keys & Indexes can be set once the table is created</div>
                    <div class='Tab Tab_ForModify               Tab_PrimaryKey'    On_Click='TabClick_PrimaryKey'    >Primary Key</div>
                    <div class='Tab Tab_ForModify               Tab_ForeignKeys'   On_Click='TabClick_ForeignKeys'   >Foreign Keys</div>
                    <div class='Tab Tab_ForModify               Tab_Indexes'       On_Click='TabClick_Indexes'       >Indexes</div>
                    <div class='Tab Tab_ForModify               Tab_AppProperties' On_Click='TabClick_AppProperties' >App Properties</div>
                    <div class='clear'></div>
                    <div class='HozLine'></div>
                </div>

                <div class='PanelsHolder'></div>
            ";
        }

    }
}