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
    public class SettingsMenu : MRBPattern<string, string>
    {
        public bool MenuJustOpened = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public SettingsMenu()
        {
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='SettingsMenu jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();
        }

        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            var userSession = Users.UserSessionClient.Get();

            if (userSession.IsAdmin)
                jF(".AdminOnly").show();
            else
                jF(".AdminOnly").hide();
        }

        //----------------------------------------------------------------------------------------------------
        public void ShowHide_UserImpersonationLink(bool impersonation_IsAllowed, bool impersonation_IsOn)
        {
            if (impersonation_IsAllowed)
            {
                jF(".Link_UserImpersonation").show();
                if (impersonation_IsOn)
                    jF(".Link_UserImpersonation").html("Turn Off User Impersonation").addClass("Link_UserImpersonation_On");
                else
                    jF(".Link_UserImpersonation").html("Turn On User Impersonation").removeClass("Link_UserImpersonation_On");
            }
            else
                jF(".Link_UserImpersonation").hide();
        }


        //---------------------------------------------------------------------------------- event handlers --
        public void Bttn_Settings_Click()
        {
            this.Close();
        }
        public void MenuLink_Click()
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
                .SettingsMenu { display: none; position: absolute; top: 0px; right: 0px; }
                .SettingsMenu .SettingsBttn { float:right; width: auto; padding: 4px 10px; line-height: 25px; font-size: .95em;
                                              background: #fff; color: #14498f; box-shadow: 1px 1px 3px #000; cursor: pointer; }
                .SettingsMenu .SettingsBttn:hover { background: #20579f; color: #fff; }
                .SettingsMenu .ShadowHider { position: absolute; right:0px; top: 30px; height: 3px; width: 100%; background: #fff; z-index:4; }

                .SettingsMenu .SettingsBox1 { position: absolute; right:0px; top: 33px; background: #fff; box-shadow: 1px 1px 3px #000; width: auto; }
                .SettingsMenu .SettingsBox1 a { display:block; width: 240px; border: 0px solid #fff; margin: 0px; padding: 8px 15px; background: #ffffff; color: #333; }
                .SettingsMenu .SettingsBox1 a:hover {                        border: 0px solid #fff; margin: 0px; padding: 8px 15px; background: #464f5b; color: #fff; }

                .SettingsMenu .SettingsBox1 .Link_UserImpersonation { display:none; }
                .SettingsMenu .SettingsBox1 .Link_UserImpersonation_On { background: #eee; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='SettingsBttn' On_Click='Bttn_Settings_Click'>Settings</div>
                <div class='clear'></div>
                <div class='ShadowHider'></div>
                <div class='SettingsBox1'>
                    <a href='#00-Connections'                On_Click='MenuLink_Click' class='AdminOnly' >Manage Connections</a>
                    <a href='#00-Users'                      On_Click='MenuLink_Click' class='AdminOnly' >Users & Permissions</a>
                    <a href='ASPdatabase.NET.aspx?ToggleImpersonation' On_Click='MenuLink_Click' class='Link_UserImpersonation'>Turn On User Impersonation</a>
                    <a href='#00-Subscription'               On_Click='MenuLink_Click' class='AdminOnly' >Product Subscription</a>
                    <a href='#00-SendFeedback'               On_Click='MenuLink_Click'>Send Feedback</a>
                    <a href='#00-About-ASPdatabase.NET'      On_Click='MenuLink_Click'>About ASPdatabase.NET</a>
                </div>
            ";
        }
    }
}