using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Subscription.Objs;

namespace ASPdatabaseNET.UI.PageParts.Subscription
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class SubscriptionMainUI : MRBPattern<SubscriptionInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public SubscriptionMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='RegistrationMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            this.ShowLoading();

            AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").Subscription__GetInfo();
        }
        public void GetInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            jF(".D_Loading").hide();
            jF(".T_SubscriptionKey").val("");

            this.Model = ajaxResponse.ReturnObj.As<SubscriptionInfo>();

            if(this.Model.SubscriptionKey.Length < 1)
            {
                jF(".D_SubscriptionEmpty").show();
                jF(".T_SubscriptionKey").focus();
            }
            else
            {
                jF(".D_SubscriptionSet").show();
                jF(".D_SubscriptionKey").html(this.Model.SubscriptionKey);
                if (this.Model.SubscriptionCount == 1)
                    jF(".D_SubscriptionCount").html("1 User");
                else
                    jF(".D_SubscriptionCount").html(this.Model.SubscriptionCount + " Users");

                string lastCheckMsg = "Your subscription was last validated with www.aspdatabase.net ";
                if (this.Model.LastCheck_MinutesLapsed == "1")
                    lastCheckMsg += "1 minute ago.";
                else lastCheckMsg += this.Model.LastCheck_MinutesLapsed + " minutes ago."; 
                jF(".D_LastCheckMsg").html(lastCheckMsg);
            }

        }
        //----------------------------------------------------------------------------------------------------
        public void ShowLoading()
        {
            jF(".D_Loading").show();
            jF(".D_SubscriptionEmpty").hide();
            jF(".D_SubscriptionSet").hide();
        }

        
        //------------------------------------------------------------------------------------------ Events --
        public void B_SaveKey_Click()
        {
            string subscriptionKey = jF(".T_SubscriptionKey").val().As<string>();
            if(subscriptionKey.Length < 36 || subscriptionKey.Length > 200)
            {
                alert("Invalid Subscription Key");
                return;
            }
            this.ShowLoading();
            AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").Subscription__SaveKey(subscriptionKey);
        }
        public void Bttn_Remove()
        {
            if (prompt("Are you sure you want to remove your subscription?\n\nIf so, type 'remove'.") != "remove")
                return;

            this.Model = new SubscriptionInfo();
            this.ShowLoading();
            AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").Subscription__RemoveKey();
        }
        public void Bttn_Refresh()
        {
            this.ShowLoading();
            AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").Subscription__Refresh();
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
                .RegistrationMainUI { }
                .RegistrationMainUI .D_Loading { display:none; }
                .RegistrationMainUI .D_SubscriptionEmpty { display:none; }
                .RegistrationMainUI .D_SubscriptionSet { display:none; }
                .RegistrationMainUI a { }
                .RegistrationMainUI a:hover { background: #555; color: #fff; }

                .RegistrationMainUI .D_SubscriptionEmpty .T_SubscriptionKey { width: 44em; border: 1px solid #444; background: #f3f3f3; padding: .3em; margin-top: .5em; }
                .RegistrationMainUI .D_SubscriptionEmpty .B_SaveKey { float:left; background: #144184; color: #fff; padding: .5em 2em; cursor:pointer; }
                .RegistrationMainUI .D_SubscriptionEmpty .B_SaveKey:hover { background: #444; }

                .RegistrationMainUI .D_SubscriptionSet .Label1 { width:40em; margin-top: .4em; background: #fcffa9; padding: .3em .8em; }
                .RegistrationMainUI .D_SubscriptionSet .Bttn { font-size: .9em; float:left; color: #fff; background: #144184; margin-right: .8em; cursor:pointer; padding: .2em 1em; }
                .RegistrationMainUI .D_SubscriptionSet .Bttn:hover { background: #444; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head'>Product Subscription</div>
                <div class='Main'>
                    <div class='D_Loading'>Loading...</div>
                    <div class='D_SubscriptionEmpty'>
                        <br />
                        Enter your Subscription Key here:
                        <br />
                        <input type='text' class='T_SubscriptionKey' />
                        <br />
                        <br />
                        <div class='B_SaveKey' On_Click='B_SaveKey_Click'>Save Key</div>
                        <div class='clear'></div>
                        <br />
                        <br />
                        <br />
                        <br />
                        If you don't yet have a Subscription Key, please go here to learn more:
                        <br />
                        <br />
                        <a href='https://www.aspdatabase.net/Subscribe/' target='_blank'>https://www.aspdatabase.net/Subscribe/</a>
                    </div>
                    <div class='D_SubscriptionSet'>
                        <br />
                        Your Subscription Key:
                        <br />
                        <div class='D_SubscriptionKey Label1'></div>
                        <br />
                        <br />
                        Your current subscription allows for up to:
                        <br />
                        <div class='D_SubscriptionCount Label1'></div>
                        <br />
                        <br />
                        <div class='D_LastCheckMsg hide'></div>
                        <br />
                        <div class='Bttn' On_Click='Bttn_Remove'>Remove Subscription Key</div>
                        <div class='Bttn' On_Click='Bttn_Refresh'>Refresh Subscription Count from www.aspdatabase.net</div>
                        <div class='clear'></div>
                        <br />
                        <br />
                        <br />
                        <br />
                        <br />
                        Learn more about Subscriptions here:
                        <a href='https://www.aspdatabase.net/Subscribe/' target='_blank'>https://www.aspdatabase.net/Subscribe/</a>
                    </div>
                </div>
            ";
        }
    }
}