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
    public class IndexesPanel : MRBPattern<Index[], TableDesign_ViewModel>
    {
        public IndexesNew IndexesNew;
        public IndexesRow[] IndexesRows;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public IndexesPanel(TableDesign_ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='IndexesPanel jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.jRoot.hide();

            this.IndexesNew = new IndexesNew();
            this.IndexesNew.ViewModel = this.ViewModel;
            this.IndexesNew.OnChange.After.AddHandler(this, "IndexesNew_OnSave", 0);
            this.IndexesNew.Close();
            this.IndexesNew.OnClose.After.AddHandler(this, "IndexesNew_Close", 0);
            jF2(".Holder_IndexesNew").append(this.IndexesNew.jRoot);
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            this.IndexesNew.Close();
            UI.PagesFramework.BasePage.WindowResized();

            jF2(".Table_IndexItems").html("");

            var minTableStructure = this.ViewModel.Get_Minified_TableStructure();
            AjaxService.ASPdatabaseService.New(this, "GetIndexes_Return").TableDesign__Indexes__Get(minTableStructure);
        }
        public void GetIndexes_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }

            this.Model = ajaxResponse.ReturnObj.As<Index[]>();

            if (this.Model.Length < 1)
            {
                jF(".WhenZero_Hide").hide();
                jF(".WhenZero_Show").show();
            }
            else
            {
                jF(".WhenZero_Hide").show();
                jF(".WhenZero_Show").hide();

                var table_IndexItems = jF2(".Table_IndexItems");
                this.IndexesRows = new IndexesRow[0];
                for (int i = 0; i < this.Model.Length; i++)
                {
                    this.IndexesRows[i] = new IndexesRow(i);
                    this.IndexesRows[i].Model = this.Model[i];
                    this.IndexesRows[i].ViewModel = this.ViewModel;
                    this.IndexesRows[i].OnChange.After.AddHandler(this, "IndexesRow_OnChange", 0);
                    this.IndexesRows[i].OnEdit_Enter.After.AddHandler(this, "IndexesRow_OnEdit_Enter", 0);
                    this.IndexesRows[i].OnEdit_Exit.After.AddHandler(this, "IndexesRow_OnEdit_Exit", 0);
                    this.IndexesRows[i].Instantiate();
                    table_IndexItems.append(this.IndexesRows[i].jRoot);
                }
            }
        }




        //------------------------------------------------------------------------------------------ Events --
        public void NewIndex_Click()
        {
            this.IndexesRow_OnEdit_Enter();
            jF2(".Bttn_NewIndex").hide();
            this.IndexesNew.Open();
        }
        public void IndexesNew_Close()
        {
            jF2(".Bttn_NewIndex").show();
            this.IndexesRow_OnEdit_Exit();
        }
        //----------------------------------------------------------------------------------------------------
        public void IndexesNew_OnSave()
        {
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void IndexesRow_OnChange()
        {
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void IndexesRow_OnEdit_Enter()
        {
            if (this.IndexesRows != null)
                for (int i = 0; i < this.IndexesRows.Length; i++)
                    this.IndexesRows[i].DisableBttns();
            jF2(".Bttn_NewIndex").hide();
        }
        public void IndexesRow_OnEdit_Exit()
        {
            if (this.IndexesRows != null)
                for (int i = 0; i < this.IndexesRows.Length; i++)
                    this.IndexesRows[i].EnableBttns();
            jF2(".Bttn_NewIndex").show();
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += IndexesNew.GetCssTree();
            rtn += IndexesRow.GetCssTree();
            rtn += IndexesSubColumnSelector.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .IndexesPanel { width: 100%; position:relative; padding-top: 37px; }
                .IndexesPanel .TopColorBar { position:absolute; top: -1px; width: inherit; min-width: 200px; height: 8px; background: #6cc5aa; margin-bottom: 35px; }

                .IndexesPanel .HeadBar { color: #0c5eca; font-size: .9em; margin-bottom: 6px; }
                .IndexesPanel .HeadBar .Label1 { float:left; width: 280px; line-height: 24px; font-weight:bold; padding-left: 15px; }
                .IndexesPanel .HeadBar .Label2 { float:left; width: 285px; line-height: 24px; font-weight:bold; }
                .IndexesPanel .HeadBar .Bttn_NewIndex { float:right; cursor:pointer; width: 140px; text-align: center;
                                                        line-height: 24px; background: #14498f; color: #fff; }
                .IndexesPanel .HeadBar .Bttn_NewIndex:hover { background: #333; }

                .IndexesPanel .Label_ZeroIndexes { display:none; background: #eee; line-height: 50px; padding-left: 20px; }

                .IndexesPanel .ScrollArea { overflow-y:auto; padding-right: 5px; }
                .IndexesPanel .Table_IndexItems { width: 100% }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopColorBar'></div>
                <div class='HeadBar'>
                    <div class='Label1 WhenZero_Hide'>Index Name</div>
                    <div class='Label2 WhenZero_Hide'>Columns in Index</div>
                    <div class='Bttn_NewIndex' On_Click='NewIndex_Click'>+ New Index</div>
                    <div class='clear'></div>
                </div>
                <div class='ScrollArea AutoResize'>
                    <div class='Holder_IndexesNew'></div>
                    <div class='Label_ZeroIndexes WhenZero_Show'>This table has 0 indexes</div>
                    <table class='Table_IndexItems'>
                    </table>
                    <br />
                </div>
            ";
        }
    }
}