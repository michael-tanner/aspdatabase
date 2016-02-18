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
    public class ForeignKeysEdit_ColumnsSelector : MRBPattern<ForeignKeyColumn, TableDesign_ViewModel>
    {
        public ForeignKeysEdit.RelationshipSides RelationshipSide = ForeignKeysEdit.RelationshipSides.NotSet;
        public bool IsNew = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ForeignKeysEdit_ColumnsSelector(bool isNew, ForeignKeysEdit.RelationshipSides relationshipSide, TableDesign_ViewModel viewModel)
        {
            this.IsNew = isNew;
            this.RelationshipSide = relationshipSide;
            this.ViewModel = viewModel;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='ForeignKeysEdit_ColumnsSelector jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.Refresh(false);
        }
        //----------------------------------------------------------------------------------------------------
        public void Refresh(bool disable)
        {
            if (disable) 
                jF("select").attr("disabled", "disabled").addClass("SelectGrayed");
            else 
                jF("select").removeAttr("disabled").removeClass("SelectGrayed");


            var leftSelect = jF(".Select_Column_ForeignKey");
            var rightSelect = jF(".Select_Column_PrimaryKey");
            leftSelect.html("<option value=''></option>");
            rightSelect.html("<option value=''></option>");

            var otherTable = this.ViewModel.TableStructure_TempOtherTable;
            var thisTable = this.ViewModel.TableStructure;
            if (this.RelationshipSide == ForeignKeysEdit.RelationshipSides.Inbound)
            {
                // Left - Other Table
                if(otherTable != null)
                    for (int i = 0; i < otherTable.Columns.Length; i++)
                        leftSelect.append(JsStr.StrFormat2("<option value='{0}'>{1}</option>", otherTable.Columns[i].ColumnName, otherTable.Columns[i].ColumnName));
                // Right - This Table
                for (int i = 0; i < thisTable.Columns.Length; i++)
                    rightSelect.append(JsStr.StrFormat2("<option value='{0}'>{1}</option>", thisTable.Columns[i].ColumnName, thisTable.Columns[i].ColumnName));
            }
            else if (this.RelationshipSide == ForeignKeysEdit.RelationshipSides.Outbound)
            {
                // Left - This Table
                for (int i = 0; i < thisTable.Columns.Length; i++)
                    leftSelect.append(JsStr.StrFormat2("<option value='{0}'>{1}</option>", thisTable.Columns[i].ColumnName, thisTable.Columns[i].ColumnName));
                // Right - Other Table
                if (otherTable != null)
                    for (int i = 0; i < otherTable.Columns.Length; i++)
                        rightSelect.append(JsStr.StrFormat2("<option value='{0}'>{1}</option>", otherTable.Columns[i].ColumnName, otherTable.Columns[i].ColumnName));
            }

            if (!this.IsNew)
            {
                jF(".Select_Column_ForeignKey").val(this.Model.ForeignKey_ColumnName);
                jF(".Select_Column_PrimaryKey").val(this.Model.PrimaryKey_ColumnName);
            }
        }


        //----------------------------------------------------------------------------------------------------
        public bool IsNotEmpty()
        {
            var pk = jF(".Select_Column_ForeignKey").val().As<string>();
            var fk = jF(".Select_Column_PrimaryKey").val().As<string>();

            if (pk.Length > 0)
                return true;
            if (fk.Length > 0)
                return true;

            return false;
        }

        //----------------------------------------------------------------------------------------------------
        public ForeignKeyColumn GetModel_ForSave()
        {
            var rtn = new ForeignKeyColumn();
            rtn.ForeignKey_ColumnName = jF(".Select_Column_ForeignKey").val().As<string>();
            rtn.PrimaryKey_ColumnName = jF(".Select_Column_PrimaryKey").val().As<string>();
            if (rtn.ForeignKey_ColumnName.Length < 1 || rtn.PrimaryKey_ColumnName.Length < 1)
                return null;
            return rtn;
        }



        //------------------------------------------------------------------------------------------ Events --
        public void Select_Changed()
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
                .ForeignKeysEdit_ColumnsSelector { }
                .ForeignKeysEdit_ColumnsSelector td { padding-top: 3px; }
                .ForeignKeysEdit_ColumnsSelector select { width: 192px; min-width: 192px; max-width: 192px; margin-top:0px; }
                .ForeignKeysEdit_ColumnsSelector .SelectGrayed { background: #e7e7e7; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='Td1b'><select class='Select_Column_ForeignKey' On_Change='Select_Changed'></select></td>
                <td class='Td2b'> --&gt; </td>
                <td class='Td3b'><select class='Select_Column_PrimaryKey' On_Change='Select_Changed'></select></td>
            ";
        }
    }
}
