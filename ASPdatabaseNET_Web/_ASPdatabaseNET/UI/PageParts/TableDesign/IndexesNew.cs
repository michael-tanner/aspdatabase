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
    public class IndexesNew : MRBPattern<Index, TableDesign_ViewModel>
    {
        public IndexesSubColumnSelector[] IndexesSubColumnSelectors;

        public bool AutoGenerateName = true;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public IndexesNew()
        {
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='IndexesNew jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            var holder_SubColumns = jF2(".Holder_SubColumns");
            holder_SubColumns.html("");

            this.Model = new Index();
            this.Model.ConnectionId = this.ViewModel.ConnectionId;
            this.Model.Schema = this.ViewModel.TableStructure.Schema;
            this.Model.TableName = this.ViewModel.TableStructure.TableName;

            this.IndexesSubColumnSelectors = new IndexesSubColumnSelector[0];
            for (int i = 0; i < 3; i++)
            {
                this.IndexesSubColumnSelectors[i] = new IndexesSubColumnSelector();
                this.IndexesSubColumnSelectors[i].ViewModel = this.ViewModel;
                this.IndexesSubColumnSelectors[i].OnChange.After.AddHandler(this, "IndexesSubColumnSelector_Changed", 1);
                this.IndexesSubColumnSelectors[i].Instantiate();
                holder_SubColumns.append(this.IndexesSubColumnSelectors[i].jRoot);
            }

            this.AutoGenerateName = true;
            jF(".Checkbox_AutoGenerateName").attr("checked", true);
            jF(".Div_IndexName").hide();
            jF(".Txt_IndexName").val("");

            jF(".Checkbox_IsUnique").attr("checked", false);
        }




        //------------------------------------------------------------------------------------------ Events --
        public void AutoGenerateName_Click()
        {
            this.AutoGenerateName = !this.AutoGenerateName;
            if(this.AutoGenerateName)
            {
                jF(".Checkbox_AutoGenerateName").attr("checked", true);
                jF(".Div_IndexName").hide();
            }
            else
            {
                jF(".Checkbox_AutoGenerateName").attr("checked", false);
                jF(".Div_IndexName").show();
                jF(".Txt_IndexName").focus();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void IndexesSubColumnSelector_Changed(IndexesSubColumnSelector subSelector)
        {
            var holder_SubColumns = jF2(".Holder_SubColumns");
            int i = this.IndexesSubColumnSelectors.Length;
            string tmpColumnName = this.IndexesSubColumnSelectors[i - 1].Model.ColumnName;
            if (tmpColumnName != null && tmpColumnName != "")
            {
                this.IndexesSubColumnSelectors[i] = new IndexesSubColumnSelector();
                this.IndexesSubColumnSelectors[i].ViewModel = this.ViewModel;
                this.IndexesSubColumnSelectors[i].OnChange.After.AddHandler(this, "IndexesSubColumnSelector_Changed", 1);
                this.IndexesSubColumnSelectors[i].Instantiate();
                holder_SubColumns.append(this.IndexesSubColumnSelectors[i].jRoot);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void IsUnique_Click()
        {
            this.Model.IsUnique = !this.Model.IsUnique;
            if(this.Model.IsUnique)
                jF2(".Checkbox_IsUnique").attr("checked", true);
            else
                jF2(".Checkbox_IsUnique").attr("checked", false);
        }
        //----------------------------------------------------------------------------------------------------
        public void SaveBttn_Click()
        {
            if (this.AutoGenerateName) 
                this.Model.IndexName = "";
            else this.Model.IndexName = jF(".Txt_IndexName").val().As<string>();

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
            AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__Indexes__Create(this.Model);
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void CancelBttn_Click()
        {
            this.Close();
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
                .IndexesNew { background: #6cc5aa; color: #fff; border-bottom: 6px solid #fff; }
                .IndexesNew .Head1 { background: #14498f; line-height: 26px; padding: 0px 15px; text-align: right; }
                .IndexesNew table { width: 100%; }
                .IndexesNew table td { vertical-align:top; padding: 10px 15px; }
                .IndexesNew table .td1 { width: 275px; padding: 5px 0px 15px 5px; }
                .IndexesNew table .td2 { padding-top: 5px; }
                .IndexesNew table .td3 { width: 105px; }
                .IndexesNew table .td4 { width: 134px;  padding: 10px 6px 0px 0px; }

                .IndexesNew .Div_AutoGenerateName { font-size: .7em; cursor:pointer; padding: 10px; }
                .IndexesNew .Div_AutoGenerateName:hover { background: #98e0ca; color: #fff; }

                .IndexesNew .Div_IndexName { padding: 0px 0px 0px 10px; }
                .IndexesNew .Div_IndexName span { font-size: .8em; }
                .IndexesNew .Div_IndexName .Txt_IndexName { width: 240px; }

                .IndexesNew .Bttn { text-align: center; border: 1px solid #caefe4; margin-bottom: 10px; cursor:pointer; line-height: 30px; }
                .IndexesNew .Bttn:hover { background: #98e0ca; }

                .IndexesNew .Bttn_IsUnique { text-align:left; font-size: .8em; padding: 7px 0px 5px 10px; line-height: 20px; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head1'>Add New Index</div>
                <table>
                    <tr>
                        <td class='td1'>
                            <div class='Div_AutoGenerateName' On_Click='AutoGenerateName_Click'>
                                <input type='checkbox' checked='checked' class='Checkbox_AutoGenerateName' />
                                Auto-generate Index Name
                            </div>
                            <div class='Div_IndexName'>
                                <span>Index Name</span>
                                <br />
                                <input type='text' class='Txt_IndexName' />
                            </div>
                        </td>
                        <td class='td2 Holder_SubColumns'>
                            
                        </td>
                        <td class='td3'>
                            <div class='Bttn Bttn_IsUnique' On_Click='IsUnique_Click'>
                                <input type='checkbox' class='Checkbox_IsUnique' />
                                Is Unique
                            </div>
                        </td>
                        <td class='td4'>
                            <div class='Bttn' On_Click='SaveBttn_Click'>Save</div>
                            <div class='Bttn' On_Click='CancelBttn_Click'>Cancel</div>
                        </td>
                    </tr>
                </table>
            ";
        }
    }
}
