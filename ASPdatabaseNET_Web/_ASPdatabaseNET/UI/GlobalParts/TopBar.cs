using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.GlobalParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TopBar : MRBPattern<string, string>
    {
        public LogoBox LogoBox;
        public SettingsMenu SettingsMenu;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public TopBar()
        {
            this.Instantiate();
        }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='TopBar jRoot NoSelect'>");
            jRoot.append(this.GetHtmlRoot());

            this.LogoBox = new LogoBox();
            this.LogoBox.Instantiate();
            jF(".Holder_LogoBox").append(this.LogoBox.jRoot);
            this.LogoBox.Open();

            this.SettingsMenu = new SettingsMenu();
            jF2(".SettingsMenu_Holder").append(this.SettingsMenu.jRoot);

            var ajaxSender = ASPdb.Security.AjaxSender.GetObj();
            ajaxSender.OnReady.After.AddHandler(this, "AjaxSender_OnReady", 0);
            ajaxSender.Initialize();

            var thisObj = this;
            eval("$(document).click(function(){ thisObj.Document_Click(); });");
        }
        //----------------------------------------------------------------------------------------------------
        public void AjaxSender_OnReady()
        {
            AjaxService.ASPdatabaseService.New(this, "GetUserSession_Return")
                .YesEncryption()
                .Authentication__GetUserSession();
        }
        //----------------------------------------------------------------------------------------------------
        public void GetUserSession_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            var userSession = ajaxResponse.ReturnObj.As<Users.UserSessionClient>();

            Users.UserSessionClient.Set(userSession);
            if (userSession == null || !userSession.IsLoggedIn)
                window.location = "ASPdatabase.NET.aspx?Logout".As<Location>();

            jF(".A_Username")
                .html("(" + userSession.UserInfo.Username + ")")
                .attr("title", "(" + userSession.UserInfo.Username + ")");

            this.SettingsMenu.ShowHide_UserImpersonationLink(userSession.Impersonation_IsAllowed, userSession.Impersonation_IsOn);
            if (userSession.Impersonation_IsAllowed && userSession.Impersonation_IsOn)
                this.Enable_Impersonation();
        }
        //----------------------------------------------------------------------------------------------------
        public void Enable_Impersonation()
        {
            jF(".SearchDiv").hide();
            jF(".ImpersonationDiv").show();
            AjaxService.ASPdatabaseService.New(this, "GetImpersonationList_Return")
                .YesEncryption()
                .Authentication__GetImpersonationList();
        }
        //----------------------------------------------------------------------------------------------------
        public void GetImpersonationList_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }
            var usersList = ajaxResponse.ReturnObj.As<Users.UserInfo[]>();
            var userSession = Users.UserSessionClient.Get();

            var select = jF(".Select_UserImpersonation");
            select.html("<option value='-1'></option>");

            for (int i = 0; i < usersList.Length; i++)
            {
                var u = usersList[i];
                string name = "";
                if (u.LastName.Length > 0 && u.FirstName.Length > 0)
                    name = u.LastName + ", " + u.FirstName;
                else name = u.LastName + " " + u.FirstName;

                string className = "";
                if (userSession.Impersonation_ActualUser.UserId == u.UserId)
                    className = "Select_You";
                else if (!u.Active)
                    className = "Select_Inactive";

                string inactiveLabel = "";
                if (!u.Active)
                    inactiveLabel = "[Inactive]";

                select.append(JsStr.StrFormat5("<option value='{0}' class='{1}'>{2} ({3}) {4}</option>",
                    u.UserId.As<string>(), className, name, u.Username, inactiveLabel));
            }
            select.val(userSession.UserInfo.UserId.As<string>());
        }


        //---------------------------------------------------------------------------------- event handlers --
        public void Document_Click()
        {
            if (this.SettingsMenu.IsOpen && !this.SettingsMenu.MenuJustOpened)
                this.SettingsMenu.Close();
            this.SettingsMenu.MenuJustOpened = false;
        }
        public void Bttn_Settings_Click()
        {
            this.SettingsMenu.Open();
            this.SettingsMenu.MenuJustOpened = true;
        }
        //----------------------------------------------------------------------------------------------------
        public void Select_UserImpersonation_Change()
        {
            int userId = jF(".Select_UserImpersonation").val().As<JsNumber>();
            AjaxService.ASPdatabaseService.New(this, "SetImpersonationUser_Return")
                .Authentication__SetImpersonationUser(userId);
        }
        //----------------------------------------------------------------------------------------------------
        public void SetImpersonationUser_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }

            window.location = "ASPdatabase.NET.aspx".As<Location>();
        }

        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += SettingsMenu.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
            .TopBar { height: 75px; line-height: 75px; background: #14498f; color: #fff; }
            .TopBar .Holder_LogoBox { float:left; }

            .TopBar .SearchDiv { float:left; }
            .TopBar .SearchDiv .SearchBox { margin-left: 75px; width: 450px; border: 1px solid #6d8eba; background: #14498f; color: #6d8eba; padding: 3px 8px; }
            
            .TopBar .ImpersonationDiv { display:none; float:left; margin-left: 80px; font-size: .8em; }
            .TopBar .ImpersonationDiv .Select_UserImpersonation { width: 250px; min-width: 250px; max-width: 250px; }
            .TopBar .ImpersonationDiv .Select_UserImpersonation .Select_You { color: blue; }
            .TopBar .ImpersonationDiv .Select_UserImpersonation .Select_Inactive { color: #bbb; }
            
            .TopBar .RightBttns { float: right; width: auto; margin-right: 25px; }
            .TopBar .RightBttns a { float: left; color: #fff; display: block; margin-top: 21px; margin-left: 3px;
                                    width: auto; padding: 4px 10px; line-height: 25px; font-size: .95em; background: #14498f; }
            .TopBar .RightBttns a:hover { padding: 3px 9px; border: 1px solid #8fabd1; }
            .TopBar .RightBttns .SettingsMenu_Holder { position: relative; z-index: 500; top: 21px; right: 75px; }
            .TopBar .RightBttns .A_Username { max-width: 140px; overflow:hidden; white-space:nowrap; }

            @media (max-width: 1180px) {
                .TopBar .SearchDiv .SearchBox { margin-left: 40px; width: 350px; }
            }
            @media (max-width: 1050px) {
                .TopBar .SearchDiv .SearchBox { margin-left: 30px; width: 280px; }
            }
            @media (max-width: 970px) {
                .TopBar .SearchDiv .SearchBox { margin-left: 20px; width: 210px; font-size: .9em; }
            }
            @media (max-width: 890px) {
                .TopBar { height: 50px; line-height: 50px; font-size: .85em; }
                .TopBar .LogoText { }
                .TopBar .LogoLink { margin-top: 6px; }
                .TopBar .SearchDiv .SearchBox { display:none; }
                .TopBar .RightBttns a { margin-top: 9px; }
                .TopBar .RightBttns .SettingsMenu_Holder { top: 9px; right: 68px; }
            }


            @media (max-width: 1100px) {
                .TopBar .ImpersonationDiv { display:none; float:left; margin-left: 40px; font-size: .8em; }
                .TopBar .ImpersonationDiv .Select_UserImpersonation { width: 200px; min-width: 200px; max-width: 200px; }
            }
            @media (max-width: 1000px) {
                .TopBar .ImpersonationDiv { display:none; float:left; margin-left: 20px; font-size: .8em; }
                .TopBar .ImpersonationDiv .Select_UserImpersonation { width: 130px; min-width: 130px; max-width: 130px; }
            }
            @media (max-width: 890px) {
                .TopBar .ImpersonationDiv { display:none; float:left; margin-left: 20px; font-size: .8em; }
                .TopBar .Label_Impersonate { display:none; }
                .TopBar .ImpersonationDiv .Select_UserImpersonation { width: 130px; min-width: 130px; max-width: 130px; }
            }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='Holder_LogoBox'></div>

            <div class='SearchDiv'>
                <input type='text' class='SearchBox' value='' placeholder='Google Web Search' />
            </div>            

            <div class='ImpersonationDiv'>
                <span class='Label_Impersonate'>Impersonate User</span>
                <select class='Select_UserImpersonation' On_Change='Select_UserImpersonation_Change'>
                    <option>Loading</option>
                </select>
            </div>

            <div class='RightBttns'>
                <div class='SettingsMenu_Holder'></div>
                <a href='#00-Home'>Home</a>  
                <a href='#00-MyAccount' class='A_Username'>&nbsp;</a>
                <a On_Click='Bttn_Settings_Click'>Settings</a>
                <a href='?Logout'>Logout</a>
            </div>
            <div class='clear'></div>
            ";
        }
    }
}