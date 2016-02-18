using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdb.FrameworkUI.MRB;
using ASPdatabaseNET.UI.PagesFramework;
using ASPdatabaseNET.UI.GlobalParts;
using ASPdatabaseNET.UI.TableGrid;

namespace ASPdatabaseNET.UI.Pages
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class EverythingPage : MRBPattern<string, string>, IPage
    {
        public TopBar TopBar;
        public LeftNav LeftNav;

        public IMRBPattern2 CurrentSubUI;
        public TableGridMain TableGridMain;
        public PageParts.HomePage.HomePageMainUI HomePageMainUI;
        public PageParts.MyAccount.MyAccountMainUI MyAccountMainUI;
        public PageParts.Connections.ConnectionsMainUI ConnectionsMainUI;
        public PageParts.ConnectionProperties.ConnectionPropertiesMainUI ConnectionPropertiesMainUI;
        public PageParts.ManageAssets.ManageAssetsMainUI ManageAssetsMainUI;
        public PageParts.TableDesign.TableDesignMainUI TableDesignMainUI;
        public PageParts.TablePermissions.TablePermissionsMainUI TablePermissionsMainUI;
        public PageParts.Users.UsersMainUI UsersMainUI;
        public PageParts.Subscription.SubscriptionMainUI SubscriptionMainUI;
        public PageParts.OtherSettings.OtherSettingsMainUI OtherSettingsMainUI;
        public PageParts.SendFeedback.SendFeedbackMainUI SendFeedbackMainUI;
        public PageParts.About.AboutMainUI AboutMainUI;
        public PageParts.Record.RecordMainUI RecordMainUI;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public EverythingPage()
        {
            var thisObj = this;
            eval("document.The_EverythingPage = thisObj;");
        }
        //----------------------------------------------------------------------------------------------------
        public static EverythingPage Get_The_EverythingPage()
        {
            EverythingPage rtn = null;
            try { eval("rtn = document.The_EverythingPage;"); }
            catch { }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new jQuery Get_jRoot() { return this.jRoot; }
        public string Get_HeaderColor() { return ""; }
        private PageIdentifier _pageId;
        public PageIdentifier PageId
        {
            get { return _pageId; }
            set
            {
                _pageId = value;
                string param1 = JsStr.S(this.PageId.PageParam1).ToLower().Trim().String;
                string param2 = JsStr.S(this.PageId.PageParam2).ToLower().Trim().String;
                IMRBPattern2 previousSubUI = this.CurrentSubUI;
                IMRBPattern2 tempSubUI = null;
                //   -----------------------------------------------------------------------------------------
                if (param1 == "table" || param1 == "view") // removed: "savedquery"
                {
                    if (this.TableGridMain != null)
                    {
                        if (previousSubUI != null) // && previousSubUI != tempSubUI)
                            previousSubUI.Close();

                        this.TableGridMain.ViewModel = new TableGrid.Objs.GridViewModel();
                        switch(param1)
                        {
                            case "table": this.TableGridMain.ViewModel.TableType = TableGrid.Objs.GridRequest.TableTypes.Table; break;
                            case "view": this.TableGridMain.ViewModel.TableType = TableGrid.Objs.GridRequest.TableTypes.View; break;
                            default: this.TableGridMain.ViewModel.TableType = TableGrid.Objs.GridRequest.TableTypes.NotSet; break;
                        }
                        this.TableGridMain.ViewModel.Id = -1;
                        try { int tmp = 1 * this.PageId.PageParam2.As<JsNumber>(); this.TableGridMain.ViewModel.Id = tmp; }
                        catch { }
                        
                        this.TableGridMain.Instantiate();
                        jF2(".TableGridMain_Holder").html("");
                        jF2(".TableGridMain_Holder").append(this.TableGridMain.jRoot);
                        this.CurrentSubUI = this.TableGridMain;
                    }
                }
                else
                {
                    switch (param1)
                    {
                        case "home": tempSubUI = this.HomePageMainUI;
                            break;
                        case "myaccount": tempSubUI = this.MyAccountMainUI;
                            break;
                        case "connections": tempSubUI = this.ConnectionsMainUI;
                            break;
                        case "connectionproperties": tempSubUI = this.ConnectionPropertiesMainUI;
                            break;
                        case "manageassets": tempSubUI = this.ManageAssetsMainUI;
                            break;
                        case "tabledesign": tempSubUI = this.TableDesignMainUI;
                            break;
                        case "tablepermissions": tempSubUI = this.TablePermissionsMainUI;
                            break;
                        case "users": tempSubUI = this.UsersMainUI;
                            break;
                        case "subscription": tempSubUI = this.SubscriptionMainUI;
                            break;
                        case "othersettings": tempSubUI = this.OtherSettingsMainUI;
                            break;
                        case "sendfeedback": tempSubUI = this.SendFeedbackMainUI;
                            break;
                        case "about": tempSubUI = this.AboutMainUI;
                            break;
                        case "record": tempSubUI = this.RecordMainUI;
                            break;
                    }
                    if (tempSubUI != null)
                    {
                        if (previousSubUI != null && previousSubUI != tempSubUI)
                            previousSubUI.Close();

                        if (!tempSubUI.Get_IsInstantiated())
                        {
                            tempSubUI.Instantiate();
                            jF2(".TD_MainBox").append(tempSubUI.Get_jRoot());
                        }
                        tempSubUI.Open();
                        this.CurrentSubUI = tempSubUI;
                    }
                }

            }
        }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='StartPage jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.TopBar = new TopBar();
            jF2(".TopBarHolder").append(this.TopBar.jRoot);

            this.LeftNav = new LeftNav();
            jF2(".TD_LeftNav").append(this.LeftNav.jRoot);

            this.TableGridMain = new TableGridMain();
            this.HomePageMainUI = new PageParts.HomePage.HomePageMainUI();
            this.MyAccountMainUI = new PageParts.MyAccount.MyAccountMainUI();
            this.ConnectionsMainUI = new PageParts.Connections.ConnectionsMainUI();
            this.ConnectionPropertiesMainUI = new PageParts.ConnectionProperties.ConnectionPropertiesMainUI();
            this.ManageAssetsMainUI = new PageParts.ManageAssets.ManageAssetsMainUI();
            this.TableDesignMainUI = new PageParts.TableDesign.TableDesignMainUI();
            this.TablePermissionsMainUI = new PageParts.TablePermissions.TablePermissionsMainUI();
            this.UsersMainUI = new PageParts.Users.UsersMainUI();
            this.SubscriptionMainUI = new PageParts.Subscription.SubscriptionMainUI();
            this.OtherSettingsMainUI = new PageParts.OtherSettings.OtherSettingsMainUI();
            this.SendFeedbackMainUI = new PageParts.SendFeedback.SendFeedbackMainUI();
            this.AboutMainUI = new PageParts.About.AboutMainUI();
            this.RecordMainUI = new PageParts.Record.RecordMainUI();

            this.PageId = this.PageId; // triggers this.TableGridMain to get set & instantiated
        }
        //----------------------------------------------------------------------------------------------------
        public void ConnectEvents_Sub()
        {
        }

        //---------------------------------------------------------------------------------- Event Handlers --





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = ""
                + GetCssRoot()
                + TableGrid.TableGridMain.GetCssTree()
                + PageParts.HomePage.HomePageMainUI.GetCssTree()
                + PageParts.MyAccount.MyAccountMainUI.GetCssTree()
                + PageParts.Connections.ConnectionsMainUI.GetCssTree()
                + PageParts.ConnectionProperties.ConnectionPropertiesMainUI.GetCssTree()
                + PageParts.ManageAssets.ManageAssetsMainUI.GetCssTree()
                + PageParts.TableDesign.TableDesignMainUI.GetCssTree()
                + PageParts.TablePermissions.TablePermissionsMainUI.GetCssTree()
                + PageParts.Users.UsersMainUI.GetCssTree()
                + PageParts.Subscription.SubscriptionMainUI.GetCssTree()
                + PageParts.OtherSettings.OtherSettingsMainUI.GetCssTree()
                + PageParts.SendFeedback.SendFeedbackMainUI.GetCssTree()
                + PageParts.About.AboutMainUI.GetCssTree()
                + PageParts.Record.RecordMainUI.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .StartPage { font-size: 16px; }
                .StartPage .TopBarHolder { }
                .StartPage .Table_StartPage { width: 100%; }
                .StartPage .Table_StartPage .TD_LeftNav { width: 15em; min-width: 15em; background: #e4e4e4; }
                .StartPage .Table_StartPage .TD_MainBox { width: 100%; }


                .StartPage .Table_StartPage .TD_MainBox .MainUI { padding: 2em 1.75em 0em 2.625em; }
                .StartPage .Table_StartPage .TD_MainBox .MainUI .Head { width: 29.6875em; color: #173a67; font-size: 1.6em; }
                .StartPage .Table_StartPage .TD_MainBox .MainUI .Main { width: 47.5em; padding-top: 1.125em; }

                @media (max-width: 1050px) { .StartPage { font-size: .95em; } }
                @media (max-width: 1000px) { .StartPage { font-size: .9em;  } }
                @media (max-width: 950px)  { .StartPage { font-size: .85em; } }
                @media (max-width: 900px)  { .StartPage { font-size: .8em;  } }
                @media (max-width: 850px)  { .StartPage { font-size: .75em; } }
                @media (max-width: 800px)  { .StartPage { font-size: .7em;  } }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopBarHolder'></div>
                <table class='Table_StartPage'>
                    <tr>
                        <td class='TD_LeftNav'></td>
                        <td class='TD_MainBox'>
                            <div class='TableGridMain_Holder'></div>
                        </td>
                    </tr>
                </table>
            ";
        }
    }
}