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
    public class IndexesSubColumnSelector : MRBPattern<IndexColumn, TableDesign_ViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public IndexesSubColumnSelector()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='IndexesSubColumnSelector jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            var dropdown = jF(".Select_ColumnNames");
            dropdown.html("<option value=''></option>");
            if(this.ViewModel.TableStructure.Columns != null)
                for (int i = 0; i < this.ViewModel.TableStructure.Columns.Length; i++)
                {
                    var columnName = this.ViewModel.TableStructure.Columns[i].ColumnName;
                    dropdown.append("<option value='" + columnName + "'>" + columnName + "</option>");
                }
            if (this.Model == null)
            {
                this.Model = new IndexColumn();
                this.Model.SortDirection = IndexColumn.E_SortTypes.Ascending;
            }
            else
            {
                dropdown.val(this.Model.ColumnName);
                if (this.Model.SortDirection == IndexColumn.E_SortTypes.Descending)
                    jF(".Checkbox_IsDESC").attr("checked", true);
            }
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Select_ColumnNames_Changed()
        {
            this.Model.ColumnName = jF(".Select_ColumnNames").val().As<string>();
            this.OnChange.After.Fire1(this);
        }
        public void IsDESC_Click()
        {
            if(this.Model.SortDirection == IndexColumn.E_SortTypes.Ascending)
            {
                this.Model.SortDirection = IndexColumn.E_SortTypes.Descending;
                jF(".Checkbox_IsDESC").attr("checked", true);
            }
            else
            {
                this.Model.SortDirection = IndexColumn.E_SortTypes.Ascending;
                jF(".Checkbox_IsDESC").attr("checked", false);
            }
            this.OnChange.After.Fire1(this);
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
                .IndexesSubColumnSelector { line-height: 30px; }
                .IndexesSubColumnSelector .Div1 { float:left; width: 210px; }
                .IndexesSubColumnSelector .Div2 { float:left; width: 110px; padding-left: 10px; font-size: .8em; cursor: pointer; }
                .IndexesSubColumnSelector .Div2:hover { background: #98e0ca; }

                .IndexesSubColumnSelector .Select_ColumnNames { width: 200px; min-width: 200px; max-width: 200px; }
                .IndexesSubColumnSelector .Checkbox_IsDESC { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Div1'>
                    <select class='Select_ColumnNames' On_Change='Select_ColumnNames_Changed'>
                    </select>
                </div>
                <div class='Div2' On_Click='IsDESC_Click'>
                    <input type='checkbox' class='Checkbox_IsDESC' />
                    <span>Descending</span>
                </div>
                <div class='clear'></div>
            ";
        }
    }
}
