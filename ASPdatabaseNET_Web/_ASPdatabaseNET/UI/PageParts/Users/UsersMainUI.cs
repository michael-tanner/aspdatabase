using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Users.Objs;

namespace ASPdatabaseNET.UI.PageParts.Users
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class UsersMainUI : MRBPattern<UsersMenuInfo, UsersViewModel>
    {
        public Panel_UserUI UserUI;
        public Panel_GroupUI GroupUI;

        public MenuItemUI[] MenuItemUIs_1; // Users - Active
        public MenuItemUI[] MenuItemUIs_2; // Users - Inactive
        public MenuItemUI[] MenuItemUIs_3; // Groups - Active
        public MenuItemUI[] MenuItemUIs_4; // Groups - Inactive
        
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public UsersMainUI()
        {
            this.ViewModel = new UsersViewModel();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='UsersMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.UserUI = new Panel_UserUI();
            this.UserUI.ViewModel = this.ViewModel;
            this.UserUI.Instantiate();
            this.UserUI.Close();
            this.UserUI.OnChange.After.AddHandler(this, "UserUI_Saved", 0);
            jF(".Holder_UserUI").append(this.UserUI.jRoot);

            this.GroupUI = new Panel_GroupUI();
            this.GroupUI.ViewModel = this.ViewModel;
            this.GroupUI.Instantiate();
            this.GroupUI.Close();
            this.GroupUI.OnChange.After.AddHandler(this, "GroupUI_Saved", 0);
            jF(".Holder_GroupUI").append(this.GroupUI.jRoot);
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jF(".BttnTd_1 span").html("(0)");
            jF(".BttnTd_2 span").html("(0)");
            jF(".BttnTd_3 span").html("(0)");
            jF(".BttnTd_4 span").html("(0)");

            AjaxService.ASPdatabaseService.New(this, "GetMenuInfo_Return").Users__GetMenuInfo();
        }
        //----------------------------------------------------------------------------------------------------
        public void GetMenuInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<UsersMenuInfo>();


            string subscriptionMsg = "";
            switch(this.Model.UserSubscriptions_Total)
            {
                case 0:
                    subscriptionMsg = "Your subscription allows for 1 limited user.  Click Here to see your subscription settings.";
                    break;
                case 1:
                    subscriptionMsg = "Your subscription allows for 1 user.  Click Here to see your subscription settings.";
                    break;
                default:
                    subscriptionMsg = "Your subscription allows for up to " + this.Model.UserSubscriptions_Total + " users.  Click Here to see your subscription settings.";
                    break;
            }
            jF(".LicenseInfo").html(subscriptionMsg);


            var menuHolder_1 = jF(".MenuHolder_1").html("<div class='MenuHead'>Users (Active)</div>");
            this.MenuItemUIs_1 = new MenuItemUI[0];
            if (this.Model.Users_Active != null)
                for (int i = 0; i < this.Model.Users_Active.Length; i++)
                {
                    this.MenuItemUIs_1[i] = new MenuItemUI();
                    this.MenuItemUIs_1[i].ViewModel = this.ViewModel;
                    this.MenuItemUIs_1[i].Model = this.Model.Users_Active[i];
                    this.MenuItemUIs_1[i].Instantiate();
                    this.MenuItemUIs_1[i].OnChange.After.AddHandler(this, "MenuItem_Clicked", 1);
                    menuHolder_1.append(this.MenuItemUIs_1[i].jRoot);
                }
            var menuHolder_2 = jF(".MenuHolder_2").html("<div class='MenuHead'>Users (Inactive)</div>");
            this.MenuItemUIs_2 = new MenuItemUI[0];
            if (this.Model.Users_Inactive != null)
                for (int i = 0; i < this.Model.Users_Inactive.Length; i++)
                {
                    this.MenuItemUIs_2[i] = new MenuItemUI();
                    this.MenuItemUIs_2[i].ViewModel = this.ViewModel;
                    this.MenuItemUIs_2[i].Model = this.Model.Users_Inactive[i];
                    this.MenuItemUIs_2[i].Instantiate();
                    this.MenuItemUIs_2[i].OnChange.After.AddHandler(this, "MenuItem_Clicked", 1);
                    menuHolder_2.append(this.MenuItemUIs_2[i].jRoot);
                }
            var menuHolder_3 = jF(".MenuHolder_3").html("<div class='MenuHead'>Groups (Active)</div>");
            this.MenuItemUIs_3 = new MenuItemUI[0];
            if (this.Model.Groups_Active != null)
                for (int i = 0; i < this.Model.Groups_Active.Length; i++)
                {
                    this.MenuItemUIs_3[i] = new MenuItemUI();
                    this.MenuItemUIs_3[i].ViewModel = this.ViewModel;
                    this.MenuItemUIs_3[i].Model = this.Model.Groups_Active[i];
                    this.MenuItemUIs_3[i].Instantiate();
                    this.MenuItemUIs_3[i].OnChange.After.AddHandler(this, "MenuItem_Clicked", 1);
                    menuHolder_3.append(this.MenuItemUIs_3[i].jRoot);
                }
            var menuHolder_4 = jF(".MenuHolder_4").html("<div class='MenuHead'>Groups (Inactive)</div>");
            this.MenuItemUIs_4 = new MenuItemUI[0];
            if (this.Model.Groups_Inactive != null)
                for (int i = 0; i < this.Model.Groups_Inactive.Length; i++)
                {
                    this.MenuItemUIs_4[i] = new MenuItemUI();
                    this.MenuItemUIs_4[i].ViewModel = this.ViewModel;
                    this.MenuItemUIs_4[i].Model = this.Model.Groups_Inactive[i];
                    this.MenuItemUIs_4[i].Instantiate();
                    this.MenuItemUIs_4[i].OnChange.After.AddHandler(this, "MenuItem_Clicked", 1);
                    menuHolder_4.append(this.MenuItemUIs_4[i].jRoot);
                }

            int count1 = 0, count2 = 0, count3 = 0, count4 = 0;
            if (this.Model.Users_Active != null) count1 = this.Model.Users_Active.Length;
            if (this.Model.Users_Inactive != null) count2 = this.Model.Users_Inactive.Length;
            if (this.Model.Groups_Active != null) count3 = this.Model.Groups_Active.Length;
            if (this.Model.Groups_Inactive != null) count4 = this.Model.Groups_Inactive.Length;
            jF(".BttnTd_1 span").html("(" + count1 + ")");
            jF(".BttnTd_2 span").html("(" + count2 + ")");
            jF(".BttnTd_3 span").html("(" + count3 + ")");
            jF(".BttnTd_4 span").html("(" + count4 + ")");


            if(this.ViewModel.SaveJustHappened)
            {
                if (this.ViewModel.LastMenuItem == null && this.ViewModel.CurrentId > -1)
                {
                    for (int i = 0; i < this.MenuItemUIs_1.Length; i++)
                        if (this.MenuItemUIs_1[i].Model.Id == this.ViewModel.CurrentId)
                            this.ViewModel.LastMenuItem = this.MenuItemUIs_1[i];
                    for (int i = 0; i < this.MenuItemUIs_2.Length; i++)
                        if (this.MenuItemUIs_2[i].Model.Id == this.ViewModel.CurrentId)
                            this.ViewModel.LastMenuItem = this.MenuItemUIs_2[i];
                    for (int i = 0; i < this.MenuItemUIs_3.Length; i++)
                        if (this.MenuItemUIs_3[i].Model.Id == this.ViewModel.CurrentId)
                            this.ViewModel.LastMenuItem = this.MenuItemUIs_3[i];
                    for (int i = 0; i < this.MenuItemUIs_4.Length; i++)
                        if (this.MenuItemUIs_4[i].Model.Id == this.ViewModel.CurrentId)
                            this.ViewModel.LastMenuItem = this.MenuItemUIs_4[i];
                }
                if (this.ViewModel.LastMenuItem != null)
                    this.MenuItem_Clicked(this.ViewModel.LastMenuItem);
                this.ViewModel.SaveJustHappened = false;
            }
            else
                this.TabClick_1();
            
            ASPdatabaseNET.UI.PagesFramework.BasePage.WindowResized();
        }


        //---------------------------------------------------------------------------------- event handlers --
        public void NewUser_Click()
        {
            this.TabClick(-1);
            this.ViewModel.CurrentId = -1;
            this.UserUI.Open();
            jF(".MenuHolder_NewUser").show();
        }
        public void NewGroup_Click()
        {
            this.TabClick(-1);
            this.ViewModel.CurrentId = -1;
            this.GroupUI.Open();
            jF(".MenuHolder_NewGroup").show();
        }
        //----------------------------------------------------------------------------------------------------
        public void TabClick_1() { this.TabClick(1); }
        public void TabClick_2() { this.TabClick(2); }
        public void TabClick_3() { this.TabClick(3); }
        public void TabClick_4() { this.TabClick(4); }
        //----------------------------------------------------------------------------------------------------
        public void TabClick(int tabId)
        {
            this.ViewModel.CurrentTabId = tabId;

            jF(".BttnTd").removeClass("On");
            jF(".MenuHolder").hide();
            if(tabId > 0)
            {
                jF(".BttnTd_" + tabId).addClass("On");
                jF(".MenuHolder_" + tabId).show();
            }

            if (this.UserUI.IsOpen)
                this.UserUI.Close();
            if (this.GroupUI.IsOpen)
                this.GroupUI.Close();

            this.TurnOff_AllMenuItems();
        }
        //----------------------------------------------------------------------------------------------------
        public void TurnOff_AllMenuItems()
        {
            if (this.MenuItemUIs_1 != null)
                for (int i = 0; i < this.MenuItemUIs_1.Length; i++)
                    if (this.MenuItemUIs_1[i].IsOn)
                        this.MenuItemUIs_1[i].TurnOff();
            if (this.MenuItemUIs_2 != null)
                for (int i = 0; i < this.MenuItemUIs_2.Length; i++)
                    if (this.MenuItemUIs_2[i].IsOn)
                        this.MenuItemUIs_2[i].TurnOff();
            if (this.MenuItemUIs_3 != null)
                for (int i = 0; i < this.MenuItemUIs_3.Length; i++)
                    if (this.MenuItemUIs_3[i].IsOn)
                        this.MenuItemUIs_3[i].TurnOff();
            if (this.MenuItemUIs_4 != null)
                for (int i = 0; i < this.MenuItemUIs_4.Length; i++)
                    if (this.MenuItemUIs_4[i].IsOn)
                        this.MenuItemUIs_4[i].TurnOff();
        }
        //----------------------------------------------------------------------------------------------------
        public void MenuItem_Clicked(MenuItemUI menuItemUI)
        {
            this.ViewModel.LastMenuItem = menuItemUI;
            if (this.UserUI.IsOpen)
                this.UserUI.Close();
            if (this.GroupUI.IsOpen)
                this.GroupUI.Close();

            this.ViewModel.CurrentId = menuItemUI.Model.Id;
            if (menuItemUI.Model.MenuType == UsersMenuItem.MenuTypes.User)
                this.UserUI.Open();
            else if (menuItemUI.Model.MenuType == UsersMenuItem.MenuTypes.Group)
                this.GroupUI.Open();

            
            this.TurnOff_AllMenuItems();
            if (this.ViewModel.CurrentTabId == 1)
            {
                if (this.MenuItemUIs_1 != null)
                    for (int i = 0; i < this.MenuItemUIs_1.Length; i++)
                        if (this.MenuItemUIs_1[i].Model.Id == menuItemUI.Model.Id)
                            this.MenuItemUIs_1[i].TurnOn();
            }
            else if (this.ViewModel.CurrentTabId == 2)
            {
                if (this.MenuItemUIs_2 != null)
                    for (int i = 0; i < this.MenuItemUIs_2.Length; i++)
                        if (this.MenuItemUIs_2[i].Model.Id == menuItemUI.Model.Id)
                            this.MenuItemUIs_2[i].TurnOn();
            }
            else if (this.ViewModel.CurrentTabId == 3)
            {
                if (this.MenuItemUIs_3 != null)
                    for (int i = 0; i < this.MenuItemUIs_3.Length; i++)
                        if (this.MenuItemUIs_3[i].Model.Id == menuItemUI.Model.Id)
                            this.MenuItemUIs_3[i].TurnOn();
            }
            else if (this.ViewModel.CurrentTabId == 4)
            {
                if (this.MenuItemUIs_4 != null)
                    for (int i = 0; i < this.MenuItemUIs_4.Length; i++)
                        if (this.MenuItemUIs_4[i].Model.Id == menuItemUI.Model.Id)
                            this.MenuItemUIs_4[i].TurnOn();
            }
        }

        //----------------------------------------------------------------------------------------------------
        public void UserUI_Saved()
        {
            this.ViewModel.SaveJustHappened = true;
            int tmpTabId = this.ViewModel.CurrentTabId;

            if (this.UserUI.Model.UserId < 0)
                this.ViewModel.LastMenuItem = null;

            if (tmpTabId < 0 && this.UserUI.Model.Active)
                tmpTabId = 1;
            else if (tmpTabId < 0 && !this.UserUI.Model.Active)
                tmpTabId = 2;
            else if (tmpTabId == 1 && !this.UserUI.Model.Active)
                tmpTabId = 2;
            else if (tmpTabId == 2 && this.UserUI.Model.Active)
                tmpTabId = 1;
            
            this.Open();
            this.TabClick(tmpTabId);
        }
        //----------------------------------------------------------------------------------------------------
        public void GroupUI_Saved()
        {
            this.ViewModel.SaveJustHappened = true;
            int tmpTabId = this.ViewModel.CurrentTabId;

            if (this.GroupUI.Model.GroupId < 0)
                this.ViewModel.LastMenuItem = null;

            if (tmpTabId < 0 && this.GroupUI.Model.Active)
                tmpTabId = 3;
            else if (tmpTabId < 0 && !this.GroupUI.Model.Active)
                tmpTabId = 4;
            else if (tmpTabId == 3 && this.GroupUI.Model.Active == false)
                tmpTabId = 4;
            else if (tmpTabId == 4 && this.GroupUI.Model.Active == true)
                tmpTabId = 3;

            this.Open();
            this.TabClick(tmpTabId);
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = MenuItemUI.GetCssTree() + Panel_UserUI.GetCssTree() + Panel_GroupUI.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .UsersMainUI { }
                .UsersMainUI .Main { position:relative; }

                .UsersMainUI .Main .NewBttns { position:absolute; top: -2em; width: 47.5em; }
                .UsersMainUI .Main .NewBttns .Bttn1 { float:right; background: #3d7bcc; color: #fff; padding: .4em 1em; margin-left: 1px; cursor:pointer; }
                .UsersMainUI .Main .NewBttns .Bttn1:hover { background: #0a346b; }

                .UsersMainUI .Main .LicenseInfo { display:block; border: 1px solid #e3e3e3; background: #e8e8e8; color: #6a6e73; 
                                                  padding-left: 1.171875em; line-height: 2em; font-size: .8em; margin-bottom: 1em; }
                .UsersMainUI .Main .LicenseInfo:hover { background: #6a6e73; border-color: #6a6e73; color: #fff; }

                .UsersMainUI .Main .BttnsTbl { margin-bottom: 1em; width: 100%; }
                .UsersMainUI .Main .BttnsTbl td { background: #a1a1a1; width: 25%; padding: .4em .75em; border-left: 1px solid #fff; cursor:pointer; font-size: .85em; text-align:center; color: #444; }
                .UsersMainUI .Main .BttnsTbl FirstTd { border-width: 0px; }
                .UsersMainUI .Main .BttnsTbl td:hover { background: #6a6e73; color: #fff; }
                .UsersMainUI .Main .BttnsTbl .On { background: #222; color: #fff; }

                .UsersMainUI .Main .MainTable001 { width: 47.5em; }
                .UsersMainUI .Main .MainTable001 tr { }
                .UsersMainUI .Main .MainTable001 tr td { }
                .UsersMainUI .Main .MainTable001 tr .Td001 { width: 11.875em; background: #eee; }
                .UsersMainUI .Main .MainTable001 tr .Td002 {  }
                .UsersMainUI .Main .MainTable001 tr .Td001 .MenuHolder { display:none; }
                .UsersMainUI .Main .MainTable001 tr .Td001 .MenuHolder .MenuHead { text-align:center; border-bottom: 1px solid #f9f9f9; color: #999; padding: .5em 0em; }
                .UsersMainUI .Main .MainTable001 tr .Td001 .MenuHolder_New { font-size: 1.4em; text-align:center; padding-top: 1.55em; color: #888; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head'>Users & Permissions</div>
                <div class='Main'>
                    <div class='NewBttns'>
                        <div class='Bttn1' On_Click='NewGroup_Click'>+ New Group</div>
                        <div class='Bttn1' On_Click='NewUser_Click'>+ New User</div>
                        <div class='clear'></div>
                    </div>
                    <a href='#00-Subscription' class='LicenseInfo'></a>
                    
                    <table class='BttnsTbl'>
                        <tr>
                            <td On_Click='TabClick_1' class='BttnTd BttnTd_1 FirstTd'>Users (Active)    <span>(0)</span></td>
                            <td On_Click='TabClick_2' class='BttnTd BttnTd_2'        >Users (Inactive)  <span>(0)</span></td>
                            <td On_Click='TabClick_3' class='BttnTd BttnTd_3'        >Groups (Active)   <span>(0)</span></td>
                            <td On_Click='TabClick_4' class='BttnTd BttnTd_4'        >Groups (Inactive) <span>(0)</span></td>
                        </td>
                    </table>

                    <table class='MainTable001'>
                        <tr>
                            <td class='Td001'>
                                <div class='AutoResize'>
                                    <div class='MenuHolder MenuHolder_1'>  </div>
                                    <div class='MenuHolder MenuHolder_2'>  </div>
                                    <div class='MenuHolder MenuHolder_3'>  </div>
                                    <div class='MenuHolder MenuHolder_4'>  </div>
                                    <div class='MenuHolder MenuHolder_New MenuHolder_NewUser'>Add<br />New User</div>
                                    <div class='MenuHolder MenuHolder_New MenuHolder_NewGroup'>Add<br />New Group</div>
                                </div>
                            </td>
                            <td class='Td002'>
                                <div class='Holder_UserUI'></div>
                                <div class='Holder_GroupUI'></div>
                            </td>
                        </tr>
                    </table>
                    
                    <div class='clear'></div>
                </div>
            ";
        }
    }
}