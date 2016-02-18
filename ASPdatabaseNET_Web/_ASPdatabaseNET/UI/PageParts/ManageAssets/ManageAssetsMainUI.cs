using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.ManageAssets
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ManageAssetsMainUI : MRBPattern<string, string>
    {
        public int ConnectionId = -1;
        public TablesPanel TablesPanel_Active;
        public TablesPanel TablesPanel_Hidden;
        public SQLViewsPanel SQLViewsPanel_Active;
        public SQLViewsPanel SQLViewsPanel_Hidden;
        public SchemasPanel SchemasPanel;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ManageAssetsMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ManageAssetsMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.TablesPanel_Active = new TablesPanel(false);
            jF2(".MainBody").append(this.TablesPanel_Active.jRoot);

            this.TablesPanel_Hidden = new TablesPanel(true);
            jF2(".MainBody").append(this.TablesPanel_Hidden.jRoot);

            this.SQLViewsPanel_Active = new SQLViewsPanel(false);
            jF2(".MainBody").append(this.SQLViewsPanel_Active.jRoot);

            this.SQLViewsPanel_Hidden = new SQLViewsPanel(true);
            jF2(".MainBody").append(this.SQLViewsPanel_Hidden.jRoot);


            this.SchemasPanel = new SchemasPanel();
            jF2(".MainBody").append(this.SchemasPanel.jRoot);


            this.TablesPanel_Active.OnChange.After.AddHandler(this, "Refresh", 0);
            this.TablesPanel_Hidden.OnChange.After.AddHandler(this, "Refresh", 0);
            this.SQLViewsPanel_Active.OnChange.After.AddHandler(this, "Refresh", 0);
            this.SQLViewsPanel_Hidden.OnChange.After.AddHandler(this, "Refresh", 0);
            this.SchemasPanel.OnChange.After.AddHandler(this, "Refresh", 0);

            this.BindUI();
            this.MenuBttnClick_Tables_Active();
        }

        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            jF2(".FrontLevelPage").show();

            this.Populate_ConnectionId();
            jF2(".Bttn_ManageConnection").attr("href", "#00-ConnectionProperties-" + this.Get_StringBased_ConnectionId() + "-MA");
            this.TablesPanel_Active.ViewModel = this.ConnectionId;
            this.TablesPanel_Hidden.ViewModel = this.ConnectionId;
            this.SQLViewsPanel_Active.ViewModel = this.ConnectionId;
            this.SQLViewsPanel_Hidden.ViewModel = this.ConnectionId;
            this.SchemasPanel.ViewModel = this.ConnectionId;

            this.Refresh();
        }
        //----------------------------------------------------------------------------------------------------
        private void Populate_ConnectionId()
        {
            int id = -1;
            try
            {
                string str = PagesFramework.PageIdentifier.GetFromUrlHash().PageParam2;
                if (isNaN(str))
                    id = -1;
                else if (str != "")
                    id = 1 * str.As<JsNumber>();
            }
            catch { }
            this.ConnectionId = id;
        }
        //----------------------------------------------------------------------------------------------------
        private string Get_StringBased_ConnectionId()
        {
            if (this.ConnectionId < 0)
                return "Null";
            else
                return (1 * this.ConnectionId).As<string>();
        }


        //----------------------------------------------------------------------------------------------------
        public void Refresh()
        {
            jF2(".MA_Main").hide();
            jF2(".LoadingLabel").show();
            if (this.ConnectionId < 0)
            {
                jF2(".LoadingLabel").html("Invalid ConnectionId");
                return;
            }
            jF2(".LoadingLabel").html("Loading ...");
            AjaxService.ASPdatabaseService.New(this, "GetAssetsLists_AjaxReturn").ManageAssets__GetAssetsLists(this.ConnectionId);

            //UI.PagesFramework.BasePage.WindowResized(); // ?????
        }
        public void GetAssetsLists_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var response = ajaxResponse.ReturnObj.As<DataObjects.ManageAssets.ManageAssetResponse>();
            var assetsListsInfo = response.AssetsListsInfo;

            jF2(".Menu_TablesActive span").html("(" + assetsListsInfo.Tables_Active.Length + ")");
            jF2(".Menu_TablesHidden span").html("(" + assetsListsInfo.Tables_Hidden.Length + ")");
            jF2(".Menu_SQLViewsActive span").html("(" + assetsListsInfo.SqlViews_Active.Length + ")");
            jF2(".Menu_SQLViewsHidden span").html("(" + assetsListsInfo.SqlViews_Hidden.Length + ")");
            jF2(".Menu_Schemas span").html("(" + assetsListsInfo.Schemas.Length + ")");

            jF2(".LoadingLabel").hide();
            jF2(".MA_Main").show();

            this.TablesPanel_Active.Model = assetsListsInfo.Tables_Active;
            this.TablesPanel_Hidden.Model = assetsListsInfo.Tables_Hidden;
            this.SQLViewsPanel_Active.Model = assetsListsInfo.SqlViews_Active;
            this.SQLViewsPanel_Hidden.Model = assetsListsInfo.SqlViews_Hidden;
            this.SchemasPanel.Model = assetsListsInfo.Schemas;


            jF2(".Label_ConnectionName").html("[" + assetsListsInfo.ConnectionName + "]");
        }



        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_Refresh()
        {
            this.Refresh();
        }

        //----------------------------------------------------------------------------------------------------
        public void OnClose_ConnectionPropertiesUI()
        {
            jF2(".FrontLevelPage").show();
        }



        //----------------------------------------------------------------------------------------------------
        public void CloseAllPanels()
        {
            var objs = new ASPdb.FrameworkUI.MRB.IMRBPattern2[5];
            objs[0] = this.TablesPanel_Active;
            objs[1] = this.TablesPanel_Hidden;
            objs[2] = this.SQLViewsPanel_Active;
            objs[3] = this.SQLViewsPanel_Hidden;
            objs[4] = this.SchemasPanel;

            for(int i=0; i<5; i++)
                if (objs[i] != null && objs[i].Get_IsOpen())
                    objs[i].Close();
        }
        //----------------------------------------------------------------------------------------------------
        public void MenuBttnClick_Tables_Active()
        {
            jF2(".LeftMenu .MenuItem").removeClass("Selected");
            jF2(".Menu_TablesActive").addClass("Selected");
            this.CloseAllPanels();
            this.TablesPanel_Active.Open();
        }
        public void MenuBttnClick_Tables_Hidden()
        {
            jF2(".LeftMenu .MenuItem").removeClass("Selected");
            jF2(".Menu_TablesHidden").addClass("Selected");
            this.CloseAllPanels();
            this.TablesPanel_Hidden.Open();
        }
        public void MenuBttnClick_SQLViews_Active()
        {
            jF2(".LeftMenu .MenuItem").removeClass("Selected");
            jF2(".Menu_SQLViewsActive").addClass("Selected");
            this.CloseAllPanels();
            this.SQLViewsPanel_Active.Open();
        }
        public void MenuBttnClick_SQLViews_Hidden()
        {
            jF2(".LeftMenu .MenuItem").removeClass("Selected");
            jF2(".Menu_SQLViewsHidden").addClass("Selected");
            this.CloseAllPanels();
            this.SQLViewsPanel_Hidden.Open();
        }
        public void MenuBttnClick_Schemas()
        {
            jF2(".LeftMenu .MenuItem").removeClass("Selected");
            jF2(".Menu_Schemas").addClass("Selected");
            this.CloseAllPanels();
            this.SchemasPanel.Open();
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += PageParts.EditViewSettings.EditViewSettingsMainUI.GetCssTree();
            rtn += TablesPanel.GetCssTree();
            rtn += SQLViewsPanel.GetCssTree();
            rtn += SchemasPanel.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ManageAssetsMainUI { }
                .ManageAssetsMainUI .FrontLevelPage { display:block; }
                .ManageAssetsMainUI .MA_SubHead { font-size: 1.1em; padding: 10px 0px 32px 0px; }
                .ManageAssetsMainUI .MA_SubHead .Label1 { margin-right: 10px; }
                .ManageAssetsMainUI .MA_SubHead .Bttn { display:inline; font-size: .7em; 
                                                        padding: 2px 10px 3px; border: 1px solid #c3c3c3; color: #14498f; cursor: pointer; }
                .ManageAssetsMainUI .MA_SubHead .Bttn:hover { background: #c3c3c3; color: #111; border-color: #aaa; }
                .ManageAssetsMainUI .MA_SubHead .Bttn span { font-size: .7em; color: #aaa; }


                .ManageAssetsMainUI .LoadingLabel { display:none; }

                .ManageAssetsMainUI .MA_Main { display:none; width: 920px; position:relative; }

                .ManageAssetsMainUI .MA_Main .LeftMenu { position:absolute; top: 0px; left: 0px; z-index: 2;
                                                         width: 211px; background: #e4e4e4; font-size: .8em; }
                .ManageAssetsMainUI .MA_Main .LeftMenu .MenuItem { padding: 6px 7px; cursor:pointer;
                                                                   border-right: 1px solid #585858; }
                .ManageAssetsMainUI .MA_Main .LeftMenu .MenuItem:hover  { background: #b3b3b3; }
                .ManageAssetsMainUI .MA_Main .LeftMenu .Selected { background: #fff;
                                                                   padding: 5px 7px; 
                                                                   border-top: 1px solid #585858;
                                                                   border-bottom: 1px solid #585858;
                                                                   border-right: 1px solid #fff; }

                .ManageAssetsMainUI .MA_Main .MainBody { position: absolute; top: 0px; left: 210px; z-index: 1;
                                                         width: 710px; padding-left: 25px;
                                                         background: #fff; border: 1px solid #585858; border-width: 1px 0px 0px 1px; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='FrontLevelPage'>
                    <div class='Head'>Manage Assets</div>
                    <div class='MA_SubHead'>
                        <span class='Label1'>Connection: <span class='Label_ConnectionName'>[]</span></span>
                        <a class='Bttn' href='#00-Connections'>Return to All Connections</a>
                        <a class='Bttn Bttn_ManageConnection' href='#'>Manage This Connection</a>
                        <div class='Bttn Bttn_Refresh' On_Click='BttnClick_Refresh'>Refresh</div>
                    </div>

                    <div class='LoadingLabel'></div>

                    <div class='MA_Main'>

                        <div class='LeftMenu'>
                            <div class='MenuItem Menu_TablesActive' On_Click='MenuBttnClick_Tables_Active'>
                                Tables - Active 
                                <span>(0)</span>
                            </div>
                            <div class='MenuItem Menu_TablesHidden' On_Click='MenuBttnClick_Tables_Hidden'>
                                Tables - Hidden 
                                <span>(0)</span>
                            </div>
                            <div class='MenuItem Menu_SQLViewsActive' On_Click='MenuBttnClick_SQLViews_Active'>
                                SQL Views - Active 
                                <span>(0)</span>
                            </div>
                            <div class='MenuItem Menu_SQLViewsHidden' On_Click='MenuBttnClick_SQLViews_Hidden'>
                                SQL Views - Hidden 
                                <span>(0)</span>
                            </div>
                            <div class='MenuItem Menu_Schemas' On_Click='MenuBttnClick_Schemas'>
                                Schemas
                                <span>(0)</span>
                            </div>
                        </div>

                        <div class='MainBody'>
                        </div>

                    </div>
                </div>
            ";
        }
    }
}