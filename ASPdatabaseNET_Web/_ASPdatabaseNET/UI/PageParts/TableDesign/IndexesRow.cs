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
    public class IndexesRow : MRBPattern<Index, TableDesign_ViewModel>
    {
        public int Index = 0;
        public IndexesSubColumnSelector[] IndexesSubColumnSelectors;
        public bool Temp_Model_IsUnique;

        public JsEvent_BeforeAfter OnEdit_Enter = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnEdit_Exit = new JsEvent_BeforeAfter();

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public IndexesRow(int index)
        {
            this.Index = index;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='IndexesRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();

            if (this.Index == 0)
                jF("td").addClass("NoTopBorder");

            var label_ColumnNames = jF(".Label_ColumnNames");
            var label_IsUnique = jF(".Label_IsUnique");

            string columnsHtml = "";
            for (int i = 0; i < this.Model.Columns.Length; i++)
            {
                columnsHtml += this.Model.Columns[i].ColumnName;
                if (this.Model.Columns[i].SortDirection == IndexColumn.E_SortTypes.Descending)
                    columnsHtml += " <span>(DESC)</span>";
                columnsHtml += "<br />";
            }
            label_ColumnNames.html(columnsHtml);

            label_IsUnique.html("Is Unique");
            if (!this.Model.IsUnique)
            {
                label_IsUnique.html("Not Unique");
                label_IsUnique.addClass("NotUnique");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void DisableBttns()
        {
            jF(".ViewPartBttn").hide();
        }
        public void EnableBttns()
        {
            jF(".ViewPartBttn").show();
        }





        //------------------------------------------------------------------------------------------ Events --
        public void Edit_Click()
        {
            this.OnEdit_Enter.After.Fire();
            jF(".ViewPart").hide();
            jF(".EditPart").show();
            jF(".EditTDPart1").addClass("EditTDPart1_ON");
            jF(".EditTDPart2").addClass("EditTDPart2_ON");

            var holder_IndexesSubColumnSelectors = jF(".Holder_IndexesSubColumnSelectors");
            holder_IndexesSubColumnSelectors.html("");
            this.IndexesSubColumnSelectors = new IndexesSubColumnSelector[0];
            for (int i = 0; i < this.Model.Columns.Length; i++)
            {
                this.IndexesSubColumnSelectors[i] = new IndexesSubColumnSelector();
                this.IndexesSubColumnSelectors[i].ViewModel = this.ViewModel;
                this.IndexesSubColumnSelectors[i].Model = this.Model.Columns[i];
                this.IndexesSubColumnSelectors[i].OnChange.After.AddHandler(this, "IndexesSubColumnSelector_Changed", 1);
                this.IndexesSubColumnSelectors[i].Instantiate();
                holder_IndexesSubColumnSelectors.append(this.IndexesSubColumnSelectors[i].jRoot);
            }
            var lastSelector = this.IndexesSubColumnSelectors[this.IndexesSubColumnSelectors.Length - 1];
            this.IndexesSubColumnSelector_Changed(lastSelector); // this as the effect of inserting one empty selector

            this.Temp_Model_IsUnique = !this.Model.IsUnique;
            this.IsUnique_Click();

        }
        public void IndexesSubColumnSelector_Changed(IndexesSubColumnSelector subSelector)
        {
            var holder_IndexesSubColumnSelectors = jF2(".Holder_IndexesSubColumnSelectors");
            int i = this.IndexesSubColumnSelectors.Length;
            string tmpColumnName = this.IndexesSubColumnSelectors[i - 1].Model.ColumnName;
            if (tmpColumnName != null && tmpColumnName != "")
            {
                this.IndexesSubColumnSelectors[i] = new IndexesSubColumnSelector();
                this.IndexesSubColumnSelectors[i].ViewModel = this.ViewModel;
                this.IndexesSubColumnSelectors[i].OnChange.After.AddHandler(this, "IndexesSubColumnSelector_Changed", 1);
                this.IndexesSubColumnSelectors[i].Instantiate();
                holder_IndexesSubColumnSelectors.append(this.IndexesSubColumnSelectors[i].jRoot);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Delete_Click()
        {
            if(confirm("Are you sure?"))
            {
                AjaxService.ASPdatabaseService.New(this, "Delete_Return").TableDesign__Indexes__Delete(this.Model);
            }
        }
        public void Delete_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void IsUnique_Click()
        {
            this.Temp_Model_IsUnique = !this.Temp_Model_IsUnique;
            if (this.Temp_Model_IsUnique)
                jF2(".Checkbox_IsUnique").attr("checked", true);
            else
                jF2(".Checkbox_IsUnique").attr("checked", false);
        }
        //----------------------------------------------------------------------------------------------------
        public void Save_Click()
        {
            this.Model.IsUnique = this.Temp_Model_IsUnique;

            int j = 0;
            this.Model.Columns = new IndexColumn[0];
            for (int i = 0; i < this.IndexesSubColumnSelectors.Length; i++)
            {
                string tmpColumnName = this.IndexesSubColumnSelectors[i].Model.ColumnName;
                if (tmpColumnName != null && tmpColumnName != "")
                {
                    this.IndexesSubColumnSelectors[i].Model.ColumnId = j;
                    this.Model.Columns[j] = this.IndexesSubColumnSelectors[i].Model;
                    j++;
                }
            }
            AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__Indexes__Update(this.Model);
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void CancelEdit_Click()
        {
            jF(".EditTDPart1").removeClass("EditTDPart1_ON");
            jF(".EditTDPart2").removeClass("EditTDPart2_ON");
            jF(".EditPart").hide();
            jF(".ViewPart").show();
            this.OnEdit_Exit.After.Fire();
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
                .IndexesRow { font-size: .9em; }
                .IndexesRow td { border-top: 6px solid #fff; background: #eeeef2; line-height: 32px; min-height: 32px; }
                .IndexesRow .td1 { width: 280px; background: #fff; }
                .IndexesRow .td2 { padding-left: 20px; white-space:nowrap; overflow:hidden; }
                .IndexesRow .td3 { width: 120px; }
                .IndexesRow .td4 { width: 54px; text-align:center; vertical-align:top; padding: 3px 6px 3px 0px; }
                .IndexesRow .td5 { width: 74px; text-align:center; vertical-align:top; padding: 3px 6px 3px 0px; }

                .IndexesRow .IndexNameDiv { padding-left: 15px; background: #e8e8ed; line-height: 32px;
                                            white-space:nowrap; overflow:hidden; }

                .IndexesRow .Label_ColumnNames span { color: #aaa; font-size: .75em; padding-left: 2px; }
                .IndexesRow .NotUnique { color: #ccc; }

                .IndexesRow .NoTopBorder { border-top-width: 0px; }

                .IndexesRow .Bttn { background: #fff; color: #222; border: 1px solid #bfbfbf; cursor:pointer; line-height: 24px; }
                .IndexesRow .Bttn:hover { background: #595959; border-color: #595959; color: #fff; }
                .IndexesRow .Bttn_WithTopMargin { margin-top: 28px; }

                .IndexesRow .ViewPart { }
                .IndexesRow .EditPart { display:none; }
                .IndexesRow .EditPart_WithTopMargin { margin-top: 7px; }
                .IndexesRow .EditTDPart1 { }
                .IndexesRow .EditTDPart1_ON { background: #4e927d; color: #fff; }
                .IndexesRow .EditTDPart2 { }
                .IndexesRow .EditTDPart2_ON { background: #6cc5aa; color: #fff; }

                .IndexesRow .Bttn_IsUnique { border: 1px solid #caefe4; margin-right: 15px; cursor:pointer;
                                             text-align:left; font-size: .8em; padding: 7px 0px 5px 10px; line-height: 20px; }
                .IndexesRow .Bttn_IsUnique:hover { background: #98e0ca; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='td1'>
                    <div class='IndexNameDiv EditTDPart1' ModelKey='IndexName'>&nbsp;</div>
                </td>
                <td class='td2 EditTDPart2'>
                    <div class='Label_ColumnNames ViewPart'></div>
                    <div class='EditPart EditPart_WithTopMargin Holder_IndexesSubColumnSelectors'>
                    </div>
                </td>
                <td class='td3 EditTDPart2'>
                    <div class='Label_IsUnique ViewPart'></div>
                    <div class='EditPart EditPart_WithTopMargin'>
                        <div class='Bttn_IsUnique' On_Click='IsUnique_Click'>
                            <input type='checkbox' class='Checkbox_IsUnique' />
                            Is Unique
                        </div>
                    </div>
                </td>
                <td class='td4 EditTDPart2'>
                    <div class='Bttn                    ViewPart ViewPartBttn' On_Click='Edit_Click'>Edit</div>
                    <div class='Bttn Bttn_WithTopMargin EditPart             ' On_Click='Save_Click'>Save</div>
                </td>
                <td class='td5 EditTDPart2'>
                    <div class='Bttn                    ViewPart ViewPartBttn' On_Click='Delete_Click'>Delete</div>
                    <div class='Bttn Bttn_WithTopMargin EditPart             ' On_Click='CancelEdit_Click'>Cancel</div>
                </td>
            ";
        }
    }
}
