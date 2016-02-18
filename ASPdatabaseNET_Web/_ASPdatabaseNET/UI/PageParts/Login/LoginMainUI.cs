using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.Login
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class LoginMainUI : MRBPattern<string, string>
    {
        public UI.GlobalParts.LogoBox LogoBox;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public LoginMainUI()
        {
            this.DoPing();
        }
        //----------------------------------------------------------------------------------------------------
        public void DoPing()
        {
            AjaxService.ASPdatabaseService.New(this, "DoPing_Return").PingServerTime();
        }
        public void DoPing_Return()
        {
            var thisObj = this;
            eval("setTimeout(function(){ thisObj.DoPing(); }, 90000);"); // 90 seconds
        }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='LoginMainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.LogoBox = new GlobalParts.LogoBox();
            this.LogoBox.Instantiate();
            jF(".TopBar").append(this.LogoBox.jRoot);
            this.LogoBox.Open();

            var thisObj = this;
            var u = jF(".Txt_Username");
            var p = jF(".Txt_Password");
            eval("u.keydown(function(event) { thisObj.OnKeyDown(event) });");
            eval("p.keydown(function(event) { thisObj.OnKeyDown(event) });");

        }
        private void Open_Sub()
        {
            jF(".MessageDiv").hide();
            jF(".FormDiv").show();
            jF(".Txt_Username").focus();
            AjaxService.ASPdatabaseService.New(this, "IsInDemoMode_Return").Authentication__IsInDemoMode();
        }
        public void IsInDemoMode_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            try
            {
                if(ajaxResponse.ReturnObj.As<bool>() == true)
                {
                    jF(".Txt_Username").val("demo");
                    jF(".Txt_Password").val("1234Demo1234");
                }
            }
            catch { }
        }









        //------------------------------------------------------------------------------------------ Events --
        public void OnKeyDown(Event evt)
        {
            jF(".ErrorMessage").hide();
            switch (evt.keyCode)
            {
                case 37: /* Left  */ break;
                case 38: /* Up    */ break;
                case 39: /* Right */ break;
                case 40: /* Down  */ break;
                case 13: /* Enter */
                    this.LoginBttn_Click();
                    break;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void LoginBttn_Click()
        {
            var ajaxSender = ASPdb.Security.AjaxSender.GetObj();
            ajaxSender.OnReady.After.AddHandler(this, "LoginBttn_Step2", 0);
            if (!ajaxSender.IsReady)
            {
                jF(".ErrorMessage").hide();
                jF(".MessageDiv").show().html("Loading Security Keys");
                jF(".FormDiv").hide();
                ajaxSender.Initialize();
            }
            else
            {
                //alert("Error. Please refresh your browser.");
                //console.log("In LoginMainUI.LoginBttn_Click() :: ajaxSender.IsReady == true.");
                this.LoginBttn_Step2();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void LoginBttn_Step2()
        {
            jF(".ErrorMessage").hide();
            jF(".MessageDiv").show().html("Authenticating");
            jF(".FormDiv").hide();

            string u = jF(".Txt_Username").val().As<string>();
            string p = jF(".Txt_Password").val().As<string>();
            jF(".Txt_Password").val("");
            AjaxService.ASPdatabaseService.New(this, "Login_Return")
                .YesEncryption()
                .Authentication__Login(u, p);
        }
        //----------------------------------------------------------------------------------------------------
        public void Login_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            jF(".MessageDiv").hide();
            jF(".FormDiv").show();
            if(ajaxResponse.Error != null)
            {
                jF(".ErrorMessage").html(ajaxResponse.Error.Message).show();
                jF(".Txt_Username").focus();
                return;
            }
            jF(".FormDiv").hide();
            var userInfo = ajaxResponse.ReturnObj.As<ASPdatabaseNET.Users.UserInfo>();

            string hash = window.location.hash;
            if(userInfo.UserId > -1)
                window.location = ("ASPdatabase.NET.aspx" + hash).As<Location>();
        }





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += UI.GlobalParts.LogoBox.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .LoginMainUI { }
                .LoginMainUI .TopBar { height: 75px; line-height: 75px; background: #14498f; color: #fff; }
                .LoginMainUI .TopBar .LogoLink { float: left; display:block; width: auto; line-height: 40px; margin-top: 18px; 
                                                 margin-left: 15px; padding: 0px 14px; font-size: 1.25em; color:#fff; }
                .LoginMainUI .TopBar .LogoLink:hover { background: #0f366b; }

                .LoginMainUI .LoginBoxWrap { margin: auto; width: 400px; max-width: 400px; min-width: 400px; }
                .LoginMainUI .LoginBorderBox { border: 1px solid #e4e4e4; margin: 50px 0px; box-shadow: 1px 1px 17px #ddd; }
                .LoginMainUI .MessageDiv { display:block; margin: 15px; }
                .LoginMainUI .FormDiv { display:none; margin: 50px 60px 50px 80px; color: #888; font-size: .8em; }

                .LoginMainUI .Txt { border: 1px solid #14498f; width: 240px; padding: 3px; margin: 0px; font-size: 1.5em; margin-bottom: 20px; }
                .LoginMainUI .Bttn_Login { background: #14498f; color: #fff; font-size: 1.2em; }
                .LoginMainUI .Bttn_Login:hover { background: #787e86; }

                .LoginMainUI .ErrorMessage { display:none; margin-top: 30px; color: red; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopBar'>
                    
                </div>

                <div class='LoginBoxWrap'>
                    <div class='LoginBorderBox'>

                        <div class='MessageDiv'>Loading</div>
                        <div class='FormDiv'>
                            Username
                            <br />
                            <input type='text' class='Txt Txt_Username' value='' autocomplete='off' autocapitalize='off' />
                            <br />
                            Password
                            <br />
                            <input type='password' class='Txt Txt_Password' value='' />
                            <br />
                            <input type='button' class='Bttn_Login' value='Login' On_Click='LoginBttn_Click' />
                            <div class='ErrorMessage'></div>
                        </div>

                    </div>
                </div>


            ";
        }
    }
}
