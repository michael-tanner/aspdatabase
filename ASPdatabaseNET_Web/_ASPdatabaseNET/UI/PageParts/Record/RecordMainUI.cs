using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Record.Objs;

namespace ASPdatabaseNET.UI.PageParts.Record
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class RecordMainUI : MRBPattern<RecordInfo, RecordViewModel>
    {
        public HistoryUI.HistoryMainUI HistoryMainUI;
        public FieldUI[] FieldUIs;
        public bool TableGrid_ControlHasData = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public RecordMainUI()
        {
            this.ViewModel = new RecordViewModel();

            
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='RecordMainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jRoot.hide();
            this.ViewModel.UniqueRowKey = PagesFramework.PageIdentifier.GetFromUrlHash().PageParam2;
            if (this.ViewModel.UniqueRowKey.Length < 1)
                window.location = "#00-Home".As<Location>();

            this.ViewModel.EditMode = RecordViewModel.EditModes.Off;
            jF(".Navigator").hide();
            jF(".Box3_StandardBttns").show();
            jF(".Box3_SaveBttns").hide();

            AjaxService.ASPdatabaseService.New(this, "Get_Return").Record__Get(this.ViewModel.UniqueRowKey);
        }
        //----------------------------------------------------------------------------------------------------
        public void Get_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
            {
                if (ajaxResponse.Error.Message == "\nData needs to be refreshed. \nTherefore the app will now return to the Records List.")
                    this.RecordsList_Click(true);
                else
                    return;
            }
            this.Model = ajaxResponse.ReturnObj.As<RecordInfo>();
            this.ViewModel.RecordInfo = this.Model;

            document.title = "[" + this.Model.TableName + "] - ASPdatabase.NET";

            string tableName = this.Model.Schema + "." + this.Model.TableName;
            jF(".Txt_TableName").html(tableName);

            UI.PagesFramework.BasePage.WindowResized();

            var tableGridMain = UI.Pages.EverythingPage.Get_The_EverythingPage().TableGridMain;
            this.TableGrid_ControlHasData = (tableGridMain != null && tableGridMain.IsInstantiated);
            this.UpdateNavigator(tableGridMain);

            this.FieldUIs = new FieldUI[0];
            var holder_FieldUIs = jF(".Holder_FieldUIs").html("");
            var model = this.Model;
            var fieldValues = this.Model.FieldValues;

            for (int i = 0; i < this.Model.FieldValues.Length; i++)
            {
                this.FieldUIs[i] = new FieldUI();
                this.FieldUIs[i].ViewModel = this.ViewModel;
                this.FieldUIs[i].Model = this.Model.FieldValues[i];
                this.FieldUIs[i].Instantiate();
                holder_FieldUIs.append(this.FieldUIs[i].jRoot);
            }

            jRoot.show();
            jF(".ReadOnlyBar").hide();
            jF(".HideIfNoPK").show();
            if (this.Model.PrimaryKeyNames == null || this.Model.PrimaryKeyNames.Length < 1)
            {
                jF(".ReadOnlyBar").show();
                jF(".HideIfNoPK").hide();
                this.Model.ActionType = TableGrid.Objs.UniqueRowKey.ActionTypes.Get;
                this.ViewModel.EditMode = RecordViewModel.EditModes.ReadOnly;
                return;
            }

            jF(".Bttn_Edit").hide();
            jF(".Bttn_Clone").hide();
            jF(".Bttn_Delete").hide();
            if (this.Model.PermissionValues.Edit) jF(".Bttn_Edit").show();
            if (this.Model.PermissionValues.Insert) jF(".Bttn_Clone").show();
            if (this.Model.PermissionValues.Delete) jF(".Bttn_Delete").show();

            if (this.Model.ActionType != TableGrid.Objs.UniqueRowKey.ActionTypes.Get)
                this.Edit_Click();

            if (this.Model.ChangeHistory_IsOn)
                jF(".Bttn_History").show();
            else
                jF(".Bttn_History").hide();

            UI.PagesFramework.BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        private void UpdateNavigator(TableGrid.TableGridMain tableGridMain)
        {
            jF(".Navigator").hide();
            if (this.TableGrid_ControlHasData)
            {
                jF(".Navigator").show();

                string previousLink = "";
                string index = "?";
                string nextLink = "";
                var rowUIs = tableGridMain.RowUIs;
                for (int i = 0; i < rowUIs.Length; i++)
                {
                    if (rowUIs[i].Model.UniqueKey == this.ViewModel.UniqueRowKey)
                    {
                        index = "" + (i + 1);
                        try { previousLink = "#00-Record-" + rowUIs[i - 1].Model.UniqueKey; }
                        catch { }
                        try { nextLink = "#00-Record-" + rowUIs[i + 1].Model.UniqueKey; }
                        catch { }
                        i = rowUIs.Length + 1; // end loop
                    }
                }
                jF(".Arrow").hide();
                jF(".Arrow_Off").show();
                if (previousLink != "")
                {
                    jF(".Arrow_Left").show().attr("href", previousLink);
                    jF(".Arrow_Left_Off").hide();
                }
                jF(".NavigatorText").html(index);
                if (nextLink != "")
                {
                    jF(".Arrow_Right").show().attr("href", nextLink);
                    jF(".Arrow_Right_Off").hide();
                }
            }
        }






        //------------------------------------------------------------------------------------------ Events --
        public void RecordsList_Click(bool refreshGrid)
        {
            var basePage = UI.PagesFramework.BasePage.GetFromDocument();
            var everythingPage = UI.Pages.EverythingPage.Get_The_EverythingPage();
            var tableGridMain = everythingPage.TableGridMain;

            if (tableGridMain.IsInstantiated)
            {
                this.Close();
                tableGridMain.Open();
                if(refreshGrid)
                    tableGridMain.Refresh();
                
                everythingPage.CurrentSubUI = tableGridMain;
                basePage.IgnoreNext_HashChange = true;
            }
            window.location = ("#00-Table-" + this.Model.Id).As<Location>();

            UI.PagesFramework.BasePage.WindowResized();
        }
        public void Edit_Click()
        {
            if (this.ViewModel.AppPropertiesInfo == null)
                AjaxService.ASPdatabaseService.New(this, "AppProperties__Get_Return").TableDesign__AppProperties__Get(this.Model.Id, true);
            else
                this.Edit_Click_2();
        }
        public void AppProperties__Get_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.AppPropertiesInfo = ajaxResponse.ReturnObj.As<UI.PageParts.TableDesign.AppProperties.Objs.AppPropertiesInfo>();
            this.Edit_Click_2();
        }
        public void Edit_Click_2()
        {
            jF(".Box3_StandardBttns").hide();
            jF(".Box3_SaveBttns").show();
            jF(".Navigator").hide();
            this.ViewModel.EditMode = RecordViewModel.EditModes.Edit;
            for (int i = 0; i < this.FieldUIs.Length; i++)
                this.FieldUIs[i].EditModeChanged();
        }
        //----------------------------------------------------------------------------------------------------
        public void Clone_Click()
        {
            AjaxService.ASPdatabaseService.New(this, "Get_Return").Record__GetClone(this.ViewModel.UniqueRowKey);
        }
        public void Delete_Click()
        {
            string msg = "Are you sure you want to permanently delete this record?";
            msg += "\n\nIf so, please type \"delete\" to confirm.";
            string promptResponse = prompt(msg);
            if (promptResponse != null && promptResponse.As<JsString>().toLowerCase() == "delete")
            {
                var keysToDelete = new string[1];
                keysToDelete[0] = this.ViewModel.UniqueRowKey;
                AjaxService.ASPdatabaseService.New(this, "DeleteRecords_Return").TableGrid__DeleteRecords(keysToDelete);
            }
        }
        public void DeleteRecords_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.RecordsList_Click(true);
        }
        //----------------------------------------------------------------------------------------------------
        public void History_Click()
        {
            if (this.HistoryMainUI == null)
            {
                this.HistoryMainUI = new HistoryUI.HistoryMainUI();
                this.HistoryMainUI.Instantiate();
                jF(".Holder_HistoryMainUI").append(this.HistoryMainUI.jRoot);
            }
            this.HistoryMainUI.UniqueRowKey = this.Model.UniqueRowObj;
            this.HistoryMainUI.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void Refresh_Click()
        {
            AjaxService.ASPdatabaseService.New(this, "Get_Return").Record__Get(this.ViewModel.UniqueRowKey);
        }
        public void Save_Click()
        {
            var recordInfoForSave = new RecordInfo();
            recordInfoForSave.TableType = this.Model.TableType;
            recordInfoForSave.ActionType = this.Model.ActionType;
            recordInfoForSave.ConnectionId = this.Model.ConnectionId;
            recordInfoForSave.Id = this.Model.Id;
            recordInfoForSave.UniqueRowObj = this.Model.UniqueRowObj;
            recordInfoForSave.Columns = this.Model.Columns;
            recordInfoForSave.FieldValues = new FieldValue[0];
            int j = 0;
            for (int i = 0; i < this.FieldUIs.Length; i++)
                if (!this.FieldUIs[i].Column.IsIdentity)
                {
                    if (this.Model.ActionType == TableGrid.Objs.UniqueRowKey.ActionTypes.Get)
                    {
                        if (this.FieldUIs[i].CheckForValueChange())
                            recordInfoForSave.FieldValues[j++] = this.FieldUIs[i].GetValueForSave();
                    }
                    else
                    {
                        recordInfoForSave.FieldValues[j++] = this.FieldUIs[i].GetValueForSave();
                    }
                }
            AjaxService.ASPdatabaseService.New(this, "Save_Return").Record__Save(recordInfoForSave);
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.UniqueRowKey = ajaxResponse.ReturnObj.As<JsString>();

            var everythingPage = UI.Pages.EverythingPage.Get_The_EverythingPage();
            var tableGridMain = everythingPage.TableGridMain;
            if (tableGridMain.IsInstantiated)
                tableGridMain.Refresh();

            jF(".SavedBar").slideDown(200).delay(400).slideUp(200);
            jF(".BttnBar").slideUp(200).delay(400).slideDown(200);
            if (this.Model.ActionType == TableGrid.Objs.UniqueRowKey.ActionTypes.Get)
            {
                this.Close();
                this.Open();
            }
            else
            {
                this.RecordsList_Click(false);
            }
        }


        public void CancelEdit_Click()
        {
            this.ViewModel.EditMode = RecordViewModel.EditModes.Off;
            if (this.Model.ActionType != TableGrid.Objs.UniqueRowKey.ActionTypes.Get)
                this.RecordsList_Click(false);

            for (int i = 0; i < this.FieldUIs.Length; i++)
                this.FieldUIs[i].EditModeChanged();

            jF(".Box3_StandardBttns").show();
            jF(".Box3_SaveBttns").hide();
            if (this.TableGrid_ControlHasData)
                jF(".Navigator").show();
        }





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            return ""
                + FieldUI.GetCssTree() 
                + HistoryUI.HistoryMainUI.GetCssTree()
                + GetCssRoot();
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .RecordMainUI { padding: 0.75em 0em 0em 0.75em; width: 55em; max-width: 55em; }
                .RecordMainUI .Txt_TableName { font-size: 1.2em; padding-bottom: 0.625em; color: #eb640a; }
                .RecordMainUI .BttnBar { background: #093a79; color: #fff; line-height: 2.1875em; border-bottom: 0.375em solid #fff; }
                .RecordMainUI .BttnBar .Box1 { float:left; width: 13.6875em; }
                .RecordMainUI .BttnBar .Box2 { float:left; width: 13em; }
                .RecordMainUI .BttnBar .Box3 { float:right; width: 25em; }
                .RecordMainUI .BttnBar .Bttn { float:right; cursor:pointer; background: #093a79; color: #fff; padding: 0em .75em; }
                .RecordMainUI .BttnBar .Bttn:hover { background: #001d44; }
                .RecordMainUI .BttnBar .Bttn_RecordsList { float:left; }

                .RecordMainUI .BttnBar .Navigator { }
                .RecordMainUI .BttnBar .Navigator .Arrow_Off { float:left; text-align:center; width: 2.1875em; background: #7490b4; }
                .RecordMainUI .BttnBar .Navigator .Arrow { display:none; float:left; text-align:center; width: 2.1875em; background: #7490b4; color: #ececec; cursor:pointer; }
                .RecordMainUI .BttnBar .Navigator .Arrow:hover { background: #3f679b; }
                .RecordMainUI .BttnBar .Navigator .NavigatorText { float:left; text-align:center; background: #ececec; color: #093a79; font-size: 1.25em; padding: 0em 1em; width: 1em; }

                .RecordMainUI .BttnBar .Box3_StandardBttns { }
                .RecordMainUI .BttnBar .Box3_SaveBttns { display:none; }

                .RecordMainUI .BttnBar .Bttn_Save { background: #0da218; }
                .RecordMainUI .BttnBar .Bttn_CancelEdit { }

                .RecordMainUI .SavedBar { display:none; background: #0da218; text-align:center; color: #fff; line-height: 2.1875em; border-bottom: 0.375em solid #fff; } 
                .RecordMainUI .ReadOnlyBar { display:none; background: #8b79c3; text-align:left; color: #fff; line-height: 2.1875em; border-bottom: 0.375em solid #fff; padding-left: 1em; }
                .RecordMainUI .ReadOnlyBar span { font-size: .7em; padding-left: .8em; }

                .RecordMainUI .Holder_FieldUIs { width: 100%; font-size: .95em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Holder_HistoryMainUI'></div>
                <div class='Txt_TableName'></div>
                <div class='BttnBar'>
                    <div class='Box1'>
                        <div class='Bttn Bttn_RecordsList' On_Click='RecordsList_Click'>&lt;&lt; Records List</div>
                    </div>
                    <div class='Box2 HideIfNoPK'>
                        <div class='Navigator NoSelect'>
                            <a   class='Arrow Arrow_Left' href='#'>&lt;</a>
                            <div class='Arrow_Off Arrow_Left_Off'>&nbsp;</div>
                            <div class='NavigatorText'>&nbsp;</div>
                            <a   class='Arrow Arrow_Right' href='#'>&gt;</a>
                            <div class='Arrow_Off Arrow_Right_Off' title='End of current set'>?</div>
                        </div>
                    </div>
                    <div class='Box3 Box3_StandardBttns HideIfNoPK'>
                        <div class='Bttn Bttn_Refresh' On_Click='Refresh_Click'>Refresh</div>
                        <div class='Bttn Bttn_History hide' On_Click='History_Click'>History</div>
                        <div class='Bttn Bttn_Delete' On_Click='Delete_Click'>Delete</div>
                        <div class='Bttn Bttn_Clone' On_Click='Clone_Click'>Clone</div>
                        <div class='Bttn Bttn_Edit' On_Click='Edit_Click'>Edit</div>
                        <div class='clear'></div>
                    </div>
                    <div class='Box3 Box3_SaveBttns'>
                        <div class='Bttn Bttn_CancelEdit' On_Click='CancelEdit_Click'>Cancel</div>
                        <div class='Bttn Bttn_Save' On_Click='Save_Click'>Save</div>
                        <div class='clear'></div>
                    </div>
                    <div class='clear'></div>
                </div>
                <div class='SavedBar'>
                    Saved
                </div>
                <div class='ReadOnlyBar'>
                    This table does not have a primary key set.<br />
                    Therefore this record cannot be identified for editing.
                </div>

                <div class='AutoResize'>
                    <table class='Holder_FieldUIs'>
                    </table>
                </div>


            ";
        }
    }
}
