using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DbInterfaces.TableObjects;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ForeignKeysEdit : MRBPattern<ForeignKey, TableDesign_ViewModel>
    {
        public enum RelationshipSides { NotSet, Inbound, Outbound };
        public RelationshipSides RelationshipSide = RelationshipSides.NotSet;
        public bool IsNew = false;

        public ForeignKeysEdit_ColumnsSelector[] ColumnsSelectors;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ForeignKeysEdit(bool isNew, RelationshipSides relationshipSide, TableDesign_ViewModel viewModel)
        {
            this.IsNew = isNew;
            this.RelationshipSide = relationshipSide;
            this.ViewModel = viewModel;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ForeignKeysEdit jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            if (this.IsNew && this.RelationshipSide == RelationshipSides.Inbound)
            {
                jF(".EditHead1").show();
            }
            else if (this.IsNew && this.RelationshipSide == RelationshipSides.Outbound)
            {
                jF(".EditHead2").show();
            }
            else if (!this.IsNew && this.RelationshipSide == RelationshipSides.Inbound)
            {
                string s = jF(".EditHead3 .MaxWidth").html() + "<span>" + this.Model.ConstraintName + "</span>";
                jF(".EditHead3 .MaxWidth").html(s);
                jF(".EditHead3").show();
            }
            else if (!this.IsNew && this.RelationshipSide == RelationshipSides.Outbound)
            {
                string s = jF(".EditHead4 .MaxWidth").html() + "<span>" + this.Model.ConstraintName + "</span>";
                jF(".EditHead4 .MaxWidth").html(s);
                jF(".EditHead4").show();
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if (this.IsNew)
            {
                this.Model = new ForeignKey();
                this.Model.ConnectionId = this.ViewModel.ConnectionId;
                this.Model.ConstraintName = "";
                if (this.RelationshipSide == RelationshipSides.Inbound)
                    this.Model.RelationshipSide = ForeignKey.E_RelationshipSides.ForeignKeySide;
                else
                    this.Model.RelationshipSide = ForeignKey.E_RelationshipSides.PrimaryKeySide;

                this.Model.Columns = new ForeignKeyColumn[0];
                for (int i = 0; i < 3; i++)
                    this.Model.Columns[i] = new ForeignKeyColumn();
            }

            this.ColumnsSelectors = new ForeignKeysEdit_ColumnsSelector[0];
            this.ViewModel.TableStructure_TempOtherTable = null;
            jF2(".Table_ColumnsSelectors").html("");
            for (int i = 0; i < this.Model.Columns.Length; i++)
                this.Insert_ColumnsSelector(this.Model.Columns[i]);
            if (!this.IsNew)
                this.Insert_ColumnsSelector(new ForeignKeyColumn());

            string thisTable = "";
            if (this.ViewModel != null && this.ViewModel.TableStructure != null)
                thisTable = this.ViewModel.TableStructure.Schema + "." + this.ViewModel.TableStructure.TableName;
            switch (this.RelationshipSide)
            {
                case RelationshipSides.Inbound:
                    this.Populate_TablesSelect(jF(".Select_Table_ForeignKey"));
                    jF(".Select_Table_ForeignKey").show();
                    if (!this.IsNew)
                    {
                        jF(".Select_Table_ForeignKey").val(this.ViewModel.GetTableId(this.Model.ForeignKey_Schema, this.Model.ForeignKey_TableName).As<string>());
                        this.Select_Table_ForeignKey_Change();
                    }

                    jF(".Div_Table_PrimaryKey").show().html(thisTable);
                    break;
                case RelationshipSides.Outbound:
                    jF(".Div_Table_ForeignKey").show().html(thisTable);

                    this.Populate_TablesSelect(jF(".Select_Table_PrimaryKey"));
                    jF(".Select_Table_PrimaryKey").show();
                    if (!this.IsNew)
                    {
                        jF(".Select_Table_PrimaryKey").val(this.ViewModel.GetTableId(this.Model.PrimaryKey_Schema, this.Model.PrimaryKey_TableName).As<string>());
                        this.Select_Table_PrimaryKey_Change();
                    }
                    break;
            }

            switch (this.Model.DeleteRule)
            {
                case ForeignKey.E_CascadeOptions.NoAction: jF(".DeleteRuleRadio1").attr("checked", true);
                    break;
                case ForeignKey.E_CascadeOptions.Cascade: jF(".DeleteRuleRadio2").attr("checked", true);
                    break;
                case ForeignKey.E_CascadeOptions.SetDefault: jF(".DeleteRuleRadio3").attr("checked", true);
                    break;
                case ForeignKey.E_CascadeOptions.SetNull: jF(".DeleteRuleRadio4").attr("checked", true);
                    break;
            }
            switch (this.Model.UpdateRule)
            {
                case ForeignKey.E_CascadeOptions.NoAction: jF(".UpdateRuleRadio1").attr("checked", true);
                    break;
                case ForeignKey.E_CascadeOptions.Cascade: jF(".UpdateRuleRadio2").attr("checked", true);
                    break;
                case ForeignKey.E_CascadeOptions.SetDefault: jF(".UpdateRuleRadio3").attr("checked", true);
                    break;
                case ForeignKey.E_CascadeOptions.SetNull: jF(".UpdateRuleRadio4").attr("checked", true);
                    break;
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void Insert_ColumnsSelector(ForeignKeyColumn column)
        {
            int i = this.ColumnsSelectors.Length;

            this.ColumnsSelectors[i] = new ForeignKeysEdit_ColumnsSelector(this.IsNew, this.RelationshipSide, this.ViewModel);
            this.ColumnsSelectors[i].Model = column;
            this.ColumnsSelectors[i].Instantiate();
            this.ColumnsSelectors[i].OnChange.After.AddHandler(this, "ColumnsSelector_Changed", 0);
            jF2(".Table_ColumnsSelectors").append(this.ColumnsSelectors[i].jRoot);
        }

        //----------------------------------------------------------------------------------------------------
        private void Populate_TablesSelect(jQuery select)
        {
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





        //------------------------------------------------------------------------------------------ Events --
        public void Save_Click()
        {
            if (!this.PopulateModel_FromUI())
                return;
            var m = this.Model;

            if (this.IsNew)
                AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__ForeignKeys__Create(this.Model);
            else
                AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__ForeignKeys__Update(this.Model);
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public bool PopulateModel_FromUI()
        {
            int tableId_FK = this.ViewModel.TableId;
            int tableId_PK = this.ViewModel.TableId;
            if (this.RelationshipSide == RelationshipSides.Inbound)
                tableId_FK = jF(".Select_Table_ForeignKey").val().As<JsNumber>();
            else
                tableId_PK = jF(".Select_Table_PrimaryKey").val().As<JsNumber>();
            if(tableId_FK < 0 || tableId_PK < 0)
            { alert("Please select a table"); return false; }
            var table_FK = this.ViewModel.GetTableInfo(tableId_FK);
            var table_PK = this.ViewModel.GetTableInfo(tableId_PK);

            this.Model.ForeignKey_Schema = table_FK.Schema;
            this.Model.ForeignKey_TableName = table_FK.TableName;
            this.Model.PrimaryKey_Schema = table_PK.Schema;
            this.Model.PrimaryKey_TableName = table_PK.TableName;

            this.Model.Columns = new ForeignKeyColumn[0];
            int j = 0;
            for (int i = 0; i < this.ColumnsSelectors.Length; i++)
            {
                var foreignKeyColumn = this.ColumnsSelectors[i].GetModel_ForSave();
                if (foreignKeyColumn != null)
                {
                    foreignKeyColumn.OrdinalPosition = j + 1;
                    this.Model.Columns[j] = foreignKeyColumn;
                    j++;
                }
            }

            string deleteRule = jF(".DeleteRuleRadio:checked").val().As<string>();
            string updateRule = jF(".UpdateRuleRadio:checked").val().As<string>();
            switch (deleteRule)
            {
                case "NoAction": this.Model.DeleteRule = ForeignKey.E_CascadeOptions.NoAction; break;
                case "Cascade": this.Model.DeleteRule = ForeignKey.E_CascadeOptions.Cascade; break;
                case "SetDefault": this.Model.DeleteRule = ForeignKey.E_CascadeOptions.SetDefault; break;
                case "SetNull": this.Model.DeleteRule = ForeignKey.E_CascadeOptions.SetNull; break;
                default: this.Model.DeleteRule = ForeignKey.E_CascadeOptions.NoAction; break;
            }
            switch (updateRule)
            {
                case "NoAction": this.Model.UpdateRule = ForeignKey.E_CascadeOptions.NoAction; break;
                case "Cascade": this.Model.UpdateRule = ForeignKey.E_CascadeOptions.Cascade; break;
                case "SetDefault": this.Model.UpdateRule = ForeignKey.E_CascadeOptions.SetDefault; break;
                case "SetNull": this.Model.UpdateRule = ForeignKey.E_CascadeOptions.SetNull; break;
                default: this.Model.UpdateRule = ForeignKey.E_CascadeOptions.NoAction; break;
            }

            return true;
        }
        //----------------------------------------------------------------------------------------------------
        public void Cancel_Click()
        {
            this.Close();
        }
        //----------------------------------------------------------------------------------------------------
        public void Select_Table_ForeignKey_Change()
        {
            int tableId = jF2(".Select_Table_ForeignKey").val().As<JsNumber>();
            this.ViewModel.TableStructure_TempOtherTable = null;
            this.Refresh_ColumnsSelectors(true);
            if (tableId >= 0)
                AjaxService.ASPdatabaseService.New(this, "GetTableInfo_Return").TableDesign__GetInfo_ForModify(tableId);
        }
        public void Select_Table_PrimaryKey_Change()
        {
            int tableId = jF2(".Select_Table_PrimaryKey").val().As<JsNumber>();
            this.ViewModel.TableStructure_TempOtherTable = null;
            this.Refresh_ColumnsSelectors(true);
            if (tableId >= 0)
                AjaxService.ASPdatabaseService.New(this, "GetTableInfo_Return").TableDesign__GetInfo_ForModify(tableId);
        }
        //----------------------------------------------------------------------------------------------------
        public void GetTableInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var response = ajaxResponse.ReturnObj.As<ASPdatabaseNET.DataObjects.TableDesign.TableDesignResponse>();
            this.ViewModel.TableStructure_TempOtherTable = response.TableStructure;
            this.Refresh_ColumnsSelectors(false);
        }
        //----------------------------------------------------------------------------------------------------
        public void Refresh_ColumnsSelectors(bool disable)
        {
            for (int i = 0; i < this.ColumnsSelectors.Length; i++)
                this.ColumnsSelectors[i].Refresh(disable);
        }
        //----------------------------------------------------------------------------------------------------
        public void ColumnsSelector_Changed()
        {
            if (this.ColumnsSelectors == null)
                this.ColumnsSelectors = new ForeignKeysEdit_ColumnsSelector[0];

            int i = this.ColumnsSelectors.Length - 1;
            if (i < 0 || this.ColumnsSelectors[i].IsNotEmpty())
                this.Insert_ColumnsSelector(new ForeignKeyColumn());
        }

        //----------------------------------------------------------------------------------------------------
        public void DeleteRuleRadio1_Click() { jF(".DeleteRuleRadio1").attr("checked", true); }
        public void DeleteRuleRadio2_Click() { jF(".DeleteRuleRadio2").attr("checked", true); }
        public void DeleteRuleRadio3_Click() { jF(".DeleteRuleRadio3").attr("checked", true); }
        public void DeleteRuleRadio4_Click() { jF(".DeleteRuleRadio4").attr("checked", true); }
        //----------------------------------------------------------------------------------------------------
        public void UpdateRuleRadio1_Click() { jF(".UpdateRuleRadio1").attr("checked", true); }
        public void UpdateRuleRadio2_Click() { jF(".UpdateRuleRadio2").attr("checked", true); }
        public void UpdateRuleRadio3_Click() { jF(".UpdateRuleRadio3").attr("checked", true); }
        public void UpdateRuleRadio4_Click() { jF(".UpdateRuleRadio4").attr("checked", true); }



        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += ForeignKeysEdit_ColumnsSelector.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ForeignKeysEdit { background: #e7e7e7; margin-bottom: 20px; border: 2px solid #14498f; }
                .ForeignKeysEdit .EditHead { display:none; font-size: .9em; background: #14498f; color: #fff; line-height: 24px; padding-left: 10px; }
                .ForeignKeysEdit .EditHead1 { }
                .ForeignKeysEdit .EditHead2 { }
                .ForeignKeysEdit .EditHead3 { background: #0b94da; }
                .ForeignKeysEdit .EditHead4 { background: #0b94da; }
                .ForeignKeysEdit .EditHead1z span { background: #fefe9f; color: #222; padding: 0px 3px; }
                .ForeignKeysEdit .EditHead2z span { background: #fefe9f; color: #222; padding: 0px 3px; }
                .ForeignKeysEdit .EditHead .MaxWidth { overflow:hidden; max-width: 433px; white-space:nowrap; }
                .ForeignKeysEdit .EditHead .MaxWidth span { color: #b3daee; font-size: .7em; }
                .ForeignKeysEdit .Body1 { padding: 6px; }
                .ForeignKeysEdit .Bttn3 { float:right; border: 1px solid #14498f; color: #14498f; background: #f5f5f5;
                                          padding: 3px 10px; margin: 0px 0px 8px 8px; cursor:pointer; }
                .ForeignKeysEdit .Bttn3:hover { background: #555; border-color: #555; color: #fff; }

                .ForeignKeysEdit .Table3 { width: 100%; }
                .ForeignKeysEdit .Table3 .Td1b { width: 50%; }
                .ForeignKeysEdit .Table3 .Td2b { width: 34px; min-width: 40px; max-width: 40px; text-align:center; }
                .ForeignKeysEdit .Table3 .Td3b { width: 50%; }

                .ForeignKeysEdit .Table3 .LabelRow td { font-size: .7em; padding: 12px 0px 4px; color: #888; }
                .ForeignKeysEdit .Table3 .TablesRow td { padding-bottom: 16px; }
                .ForeignKeysEdit .Table3 .TablesRow select { display:none; width: 188px; min-width: 188px; max-width: 188px; margin-top:0px; }
                .ForeignKeysEdit .Table3 .TablesRow .DivLabel { display:none; width: 188px; overflow:hidden; white-space:nowrap;
                                                                text-align:center; border: 1px solid #a9a9a9; background-color: #f0f0f0; 
                                                                font-size: .8em; line-height: 18px; }
                .ForeignKeysEdit .Table3 .ColumnsLabelRow { }
                .ForeignKeysEdit .Table3 .ColumnsLabelRow td { font-size: .7em; padding: 1px 10px; background: #d8d8d8; color: #777; }

                .ForeignKeysEdit .Divider4 { font-size: 1px; background: #d8d8d8; line-height: 10px; margin-top: 16px; }
                .ForeignKeysEdit .Table4 { width: 100%; }
                .ForeignKeysEdit .Table4 td { width: 50%; font-size: .8em; }
                .ForeignKeysEdit .Table4 .Td_Left { }
                .ForeignKeysEdit .Table4 .Td_Right { }
                .ForeignKeysEdit .Table4 .LabelDiv { font-weight: bold; padding: 5px 10px; }
                .ForeignKeysEdit .Table4 .RadioItemDiv { cursor:pointer; padding: 0px 20px; font-size: .8em; color: #444; line-height: 17px; }
                .ForeignKeysEdit .Table4 .RadioItemDiv:hover { background: #bcbcbc; }
                .ForeignKeysEdit .Table4 radio { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='EditHead EditHead1'><span>New</span> Relationship TO This Table (Inbound)</div>
                <div class='EditHead EditHead2'><span>New</span> Relationship FROM This Table (Outbound)</div>
                <div class='EditHead EditHead3'><div class='MaxWidth'>Edit Inbound Relationship: </div></div>
                <div class='EditHead EditHead4'><div class='MaxWidth'>Edit Outbound Relationship: </div></div>
                <div class='Body1'>
                    <div class='Bttn3' On_Click='Cancel_Click'>Cancel</div>
                    <div class='Bttn3' On_Click='Save_Click'>Save</div>
                    <div class='clear'></div>
                    <table class='Table3'>
                        <tr class='LabelRow'>
                            <td class='Td1b'>Foreign Key Table</td>
                            <td class='Td2b'></td>
                            <td class='Td3b'>Primary Key Table</td>
                        </tr>
                        <tr class='TablesRow'>
                            <td class='Td1b'>
                                <select    class='Select_Table_ForeignKey' On_Change='Select_Table_ForeignKey_Change'></select>
                                <div class='DivLabel Div_Table_ForeignKey'></div>
                            </td>
                            <td class='Td2b'> --&gt; </td>
                            <td class='Td3b'>
                                <select    class='Select_Table_PrimaryKey' On_Change='Select_Table_PrimaryKey_Change'></select>
                                <div class='DivLabel Div_Table_PrimaryKey'></div>
                            </td>
                        </tr>
                        <tr class='ColumnsLabelRow'>
                            <td class='Td1b'>Columns</td>
                            <td class='Td2b'></td>
                            <td class='Td3b'></td>
                        </tr>
                    </table>

                    <table class='Table3 Table_ColumnsSelectors'>
                    </table>

                    <div class='Divider4'>&nbsp;</div>
                    <table class='Table4'>
                        <tr>
                            <td class='Td_Left'>
                                <div class='LabelDiv'>Delete Rule</div>
                                <div class='RadioItemDiv' On_Click='DeleteRuleRadio1_Click'>
                                    <input type='radio' name='DeleteRule' class='DeleteRuleRadio DeleteRuleRadio1' value='NoAction' /> No Action
                                </div>
                                <div class='RadioItemDiv' On_Click='DeleteRuleRadio2_Click'>
                                    <input type='radio' name='DeleteRule' class='DeleteRuleRadio DeleteRuleRadio2' value='Cascade' /> Cascade
                                </div>
                                <div class='RadioItemDiv' On_Click='DeleteRuleRadio3_Click'>
                                    <input type='radio' name='DeleteRule' class='DeleteRuleRadio DeleteRuleRadio3' value='SetDefault' /> Set Default
                                </div>
                                <div class='RadioItemDiv' On_Click='DeleteRuleRadio4_Click'>
                                    <input type='radio' name='DeleteRule' class='DeleteRuleRadio DeleteRuleRadio4' value='SetNull' /> Set Null
                                </div>
                            </td>
                            <td class='Td_Right'>
                                <div class='LabelDiv'>Update Rule</div>
                                <div class='RadioItemDiv' On_Click='UpdateRuleRadio1_Click'>
                                    <input type='radio' name='UpdateRule' class='UpdateRuleRadio UpdateRuleRadio1' value='NoAction' /> No Action
                                </div>
                                <div class='RadioItemDiv' On_Click='UpdateRuleRadio2_Click'>
                                    <input type='radio' name='UpdateRule' class='UpdateRuleRadio UpdateRuleRadio2' value='Cascade' /> Cascade
                                </div>
                                <div class='RadioItemDiv' On_Click='UpdateRuleRadio3_Click'>
                                    <input type='radio' name='UpdateRule' class='UpdateRuleRadio UpdateRuleRadio3' value='SetDefault' /> Set Default
                                </div>
                                <div class='RadioItemDiv' On_Click='UpdateRuleRadio4_Click'>
                                    <input type='radio' name='UpdateRule' class='UpdateRuleRadio UpdateRuleRadio4' value='SetNull' /> Set Null
                                </div>
                            </td>
                        </tr>
                    <table>
                </div>
            ";
        }
    }
}
