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
    public class ForeignKeysItem : MRBPattern<ForeignKey, TableDesign_ViewModel>
    {
        public ForeignKeysEdit ForeignKeysEdit;

        public JsEvent_BeforeAfter OnEdit_Enter = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnEdit_Exit = new JsEvent_BeforeAfter();

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ForeignKeysItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ForeignKeysItem jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();

            jF(".ConstraintName").html(this.Model.ConstraintName);
            jF(".ForeignKey_Table").html("<span>" + this.Model.ForeignKey_Schema + ".</span>" + this.Model.ForeignKey_TableName);
            jF(".PrimaryKey_Table").html("<span>" + this.Model.PrimaryKey_Schema + ".</span>" + this.Model.PrimaryKey_TableName);
            jF(".ForeignKey_Table").attr("title", this.Model.ForeignKey_Schema + "." + this.Model.ForeignKey_TableName);
            jF(".PrimaryKey_Table").attr("title", this.Model.PrimaryKey_Schema + "." + this.Model.PrimaryKey_TableName);
            string columnsFK = "";
            string columnsArrows = "";
            string columnsPK = "";
            for (int i = 0; i < this.Model.Columns.Length; i++)
            {
                columnsFK += this.Model.Columns[i].ForeignKey_ColumnName + "<br />";
                columnsArrows += "--&gt;<br />";
                columnsPK += this.Model.Columns[i].PrimaryKey_ColumnName + "<br />";
            }
            jF(".ForeignKey_Columns").html(columnsFK);
            jF(".ColumnsArrowTd").html(columnsArrows);
            jF(".PrimaryKey_Columns").html(columnsPK);

            string strDeleteRule = this.GetCascadeOptionString(this.Model.DeleteRule);
            string strUpdateRule = this.GetCascadeOptionString(this.Model.UpdateRule);
            if (strDeleteRule != "NoAction")
            {
                jF(".Td_DeleteRule").html("Delete Rule: " + strDeleteRule);
                jF(".DeleteUpdateRulesRow").show();
            }
            if (strUpdateRule != "NoAction")
            {
                jF(".Td_UpdateRule").html("Update Rule: " + strUpdateRule);
                jF(".DeleteUpdateRulesRow").show();
            }

        }
        //----------------------------------------------------------------------------------------------------
        public string GetCascadeOptionString(ForeignKey.E_CascadeOptions cascadeOption)
        {
            switch (cascadeOption)
            {
                case ForeignKey.E_CascadeOptions.NoAction:
                    return "NoAction";
                case ForeignKey.E_CascadeOptions.Cascade:
                    return "Cascade";
                case ForeignKey.E_CascadeOptions.SetDefault:
                    return "SetDefault";
                case ForeignKey.E_CascadeOptions.SetNull:
                    return "SetNull";
                default:
                    return "NoAction";
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void DisableBttns()
        {
            jF2(".Bttn2").hide();
        }
        public void EnableBttns()
        {
            jF2(".Bttn2").show();
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Edit_Click()
        {
            if(this.ForeignKeysEdit == null)
            {
                var relSide = TableDesign.ForeignKeysEdit.RelationshipSides.Inbound;
                if (this.Model.RelationshipSide == ForeignKey.E_RelationshipSides.ForeignKeySide)
                    relSide = TableDesign.ForeignKeysEdit.RelationshipSides.Outbound;
                this.ForeignKeysEdit = new ForeignKeysEdit(false, relSide, this.ViewModel);
                this.ForeignKeysEdit.Model = this.Model;
                this.ForeignKeysEdit.Instantiate();
                this.ForeignKeysEdit.OnClose.After.AddHandler(this, "EditControl_Close", 0);
                this.ForeignKeysEdit.OnChange.After.AddHandler(this, "EditControl_Saved", 0);
                jF2(".EditDiv").append(this.ForeignKeysEdit.jRoot);
            }
            this.OnEdit_Enter.After.Fire();
            jF2(".ViewDiv").hide();
            this.ForeignKeysEdit.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void Delete_Click()
        {
            if(confirm("Are you sure?"))
            {
                AjaxService.ASPdatabaseService.New(this, "Delete_Return").TableDesign__ForeignKeys__Delete(this.Model);
            }
        }
        public void Delete_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void EditControl_Close()
        {
            jF2(".ViewDiv").show();
            //this.OnClose.After.Fire();
            this.OnEdit_Exit.After.Fire();
        }
        public void EditControl_Saved()
        {
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
                .ForeignKeysItem { margin: 0px 0px 20px; }
                .ForeignKeysItem .ViewDiv {  }
                .ForeignKeysItem .EditDiv { }

                .ForeignKeysItem .ViewDiv .TopHead { line-height: 16px; background: #0b94da; color: #fff; }
                .ForeignKeysItem .ViewDiv .TopHead .ConstraintName { font-size: .6em; color: #b3daee; float: left; padding-left: 10px; width: 296px; overflow:hidden; white-space:nowrap; }
                .ForeignKeysItem .ViewDiv .TopHead .Bttn2 { float: right; font-size: .7em; border-left: 1px solid #fff; padding: 0px 17px; cursor:pointer; }
                .ForeignKeysItem .ViewDiv .TopHead .Bttn2:hover { background: #555; }
                .ForeignKeysItem .ViewDiv .Table2 { width: 100%; }
                .ForeignKeysItem .ViewDiv .Table2 td { line-height: 22px; }
                .ForeignKeysItem .ViewDiv .Table2 .Td1a { width: 50%; text-align: right; }
                .ForeignKeysItem .ViewDiv .Table2 .Td2a { width: 40px; min-width: 40px; max-width: 40px; text-align:center; }
                .ForeignKeysItem .ViewDiv .Table2 .Td3a { width: 50%; }
                .ForeignKeysItem .ViewDiv .Table2 .TableRow {  }
                .ForeignKeysItem .ViewDiv .Table2 .TableRow td { font-size: .9em; background: #e7e7e7; white-space:nowrap; }
                .ForeignKeysItem .ViewDiv .Table2 .TableRow span { font-size: .7em; }
                .ForeignKeysItem .ViewDiv .Table2 .ColumnRow {  }
                .ForeignKeysItem .ViewDiv .Table2 .ColumnRow td { font-size: .7em; background: #f3f3f3; }
                .ForeignKeysItem .ViewDiv .Table2 .ColumnRow .ColumnsArrowTd { color: #999; }
                .ForeignKeysItem .ViewDiv .Table2 .ForeignKey_Table   { max-width: 200px; overflow-x:hidden; float:right; }
                .ForeignKeysItem .ViewDiv .Table2 .PrimaryKey_Table   { max-width: 200px; overflow-x:hidden; }
                .ForeignKeysItem .ViewDiv .Table2 .ForeignKey_Columns { max-width: 200px; overflow-x:hidden; float:right; }
                .ForeignKeysItem .ViewDiv .Table2 .PrimaryKey_Columns { max-width: 200px; overflow-x:hidden; }
                .ForeignKeysItem .ViewDiv .DeleteUpdateRulesRow { display:none;  }
                .ForeignKeysItem .ViewDiv .DeleteUpdateRulesRow td { border-top: 1px solid #e7e7e7; background: #f3f3f3;
                                                                     font-size: .8em; color: #999; }
                .ForeignKeysItem .ViewDiv .DeleteUpdateRulesRow .Td_DeleteRule { text-align:right; }
                .ForeignKeysItem .ViewDiv .DeleteUpdateRulesRow .Td_UpdateRule { text-align:left; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='ViewDiv'>
                    <div class='TopHead'>
                        <div class='ConstraintName'></div>
                        <div class='Bttn2' On_Click='Delete_Click'>Delete</div>
                        <div class='Bttn2' On_Click='Edit_Click'>Edit</div>
                        <div class='clear'></div>
                    </div>
                    <table class='Table2'>
                        <tr class='TableRow'>
                            <td class='Td1a'><div class='ForeignKey_Table'></div></td>
                            <td class='Td2a'>--&gt;</td>
                            <td class='Td3a'><div class='PrimaryKey_Table'></div></td>
                        </tr>
                        <tr class='ColumnRow'>
                            <td class='Td1a'><div class='ForeignKey_Columns'></div></td>
                            <td class='Td2a ColumnsArrowTd'>--&gt;</td>
                            <td class='Td3a'><div class='PrimaryKey_Columns'></div></td>
                        </tr>
                        <tr class='DeleteUpdateRulesRow'>
                            <td class='Td_DeleteRule'></td>
                            <td class=''></td>
                            <td class='Td_UpdateRule'></td>
                        </tr>
                    </table>
                </div>
                <div class='EditDiv'>
                </div>
            ";
        }
    }
}
