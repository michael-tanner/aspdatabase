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
    public class SQLViewsPanel : MRBPattern<BasicAssetInfo[], int>
    {
        public bool HoldsHiddenItems = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public SQLViewsPanel(bool holdsHiddenItems)
        {
            this.HoldsHiddenItems = holdsHiddenItems;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='SQLViewsPanel jRoot'>");
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
                    var menuRow = new SQLViewsPanel_MenuRow(this.HoldsHiddenItems);
                    menuRow.Model = this.Model[i];
                    menuRow.Instantiate();
                    menuRow.OnChange.After.AddHandler(this, "SubItem_Changed", 0);
                    table.append(menuRow.jRoot);
                }
            }
        }


        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_NewSQLView()
        {
            alert("+ New SQL View");
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
            rtn += SQLViewsPanel_MenuRow.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .SQLViewsPanel { padding-top: 0px; }

                .SQLViewsPanel .BttnsBar { margin: 12px 0px; }
                .SQLViewsPanel .BttnsBar .Bttn { float:right; background: #14498f; color: #fff; line-height: 28px;
                                               padding: 0px 20px; cursor: pointer; margin-left: 20px; font-size: .9em; }
                .SQLViewsPanel .BttnsBar .Bttn:hover { background: #1d2d42; }
                
                .SQLViewsPanel .MenuTable { width: 100%; }
                .SQLViewsPanel .MenuTable th { font-weight: normal; color: #878787; font-size: .65em; border-bottom: 1px solid #c3c3c3; 
                                             text-align: left; padding: 0px 0px 1px 5px; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"

                <div class='BttnsBar'>
                    <div class='Bttn hide' On_Click='BttnClick_NewSQLView'>+ New SQL View</div>
                    <div class='clear'></div>
                </div>

                <div class='AutoResize'>
                    <table class='MenuTable'>
                        <tr>
                            <th>[Schema].[View Name]</th>
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