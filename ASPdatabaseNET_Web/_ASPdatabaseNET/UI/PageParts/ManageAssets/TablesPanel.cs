using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DataObjects.ManageAssets;

namespace ASPdatabaseNET.UI.PageParts.ManageAssets
{
    //----------------------------------------------------------------------------------------------------////
    /// <summary>ViewModel is ConnectionId</summary>
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TablesPanel : MRBPattern<BasicAssetInfo[], int>
    {
        public bool HoldsHiddenItems = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public TablesPanel(bool holdsHiddenItems)
        {
            this.HoldsHiddenItems = holdsHiddenItems;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='TablesPanel jRoot'>");
            jRoot.append(this.GetHtmlRoot());
            jRoot.hide();

            this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            UI.PagesFramework.BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        public void OnModel_Set()
        {
            this.Refresh();
        }
        //----------------------------------------------------------------------------------------------------
        public void Refresh()
        {
            var table = jF2(".MenuTable");
            table.find(".jRoot").remove();
            if (this.Model != null)
            {
                for (int i = 0; i < this.Model.Length; i++)
                {
                    var menuRow = new TablesPanel_MenuRow(this.HoldsHiddenItems);
                    menuRow.Model = this.Model[i];
                    menuRow.Instantiate();
                    menuRow.OnChange.After.AddHandler(this, "SubItem_Changed", 0);
                    table.append(menuRow.jRoot);
                }
            }
            UI.PagesFramework.BasePage.WindowResized();
        }


        //------------------------------------------------------------------------------------------ Events --
        public void Bttn_CreateTable_Click()
        {
            string url = "#00-TableDesign-New-" + this.ViewModel;
            eval("window.location = url;");
        }
        public void Bttn_ImportTable_Click()
        {
            alert("+ Import Table");
        }
        public void SubItem_Changed()
        {
            this.OnChange.After.Fire();
        }



        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += TablesPanel_MenuRow.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .TablesPanel { }

                .TablesPanel .BttnsBar { margin: 12px 0px; }
                .TablesPanel .BttnsBar .Bttn { float:right; background: #14498f; color: #fff; line-height: 28px;
                                               padding: 0px 20px; cursor: pointer; margin-left: 20px; font-size: .9em; }
                .TablesPanel .BttnsBar .Bttn:hover { background: #1d2d42; }
                
                .TablesPanel .MenuTable { width: 100%; }
                .TablesPanel .MenuTable th { font-weight: normal; color: #878787; font-size: .65em; border-bottom: 1px solid #c3c3c3; 
                                             text-align: left; padding: 0px 0px 1px 5px; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"

                <div class='BttnsBar'>
                    <div class='Bttn hide' On_Click='Bttn_ImportTable_Click'>+ Import Table</div>
                    <div class='Bttn' On_Click='Bttn_CreateTable_Click'>+ Create Table</div>
                    <div class='clear'></div>
                </div>

                <div class='AutoResize'>
                    <table class='MenuTable'>
                        <tr>
                            <th>[Schema].[Table Name]</th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                            <th></th>
                        </tr>
                    </table>
                </div>
            ";
        }
    }
}