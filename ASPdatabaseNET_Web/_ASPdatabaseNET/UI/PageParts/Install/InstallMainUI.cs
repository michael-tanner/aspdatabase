using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Install.Objs;

namespace ASPdatabaseNET.UI.PageParts.Install
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class InstallMainUI : MRBPattern<InstallInfo, InstallViewModel>
    {
        ASPdb.Security.AjaxSender AjaxSender;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public InstallMainUI()
        {
            this.ViewModel = new InstallViewModel();
            this.ViewModel.IsFirstAttempt_AddingConnectionString = true;
            this.ViewModel.LegalHasBeenAccepted = false;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='InstallMainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            window.location.hash = "#";
            jF(".Screen").hide();
            jF(".Screen_Loading").show();

            this.AjaxSender = ASPdb.Security.AjaxSender.GetObj();
            this.AjaxSender.OnReady.After.AddHandler(this, "AjaxSender_Ready", 0);
            if (!this.AjaxSender.IsReady)
                this.AjaxSender.Initialize();
        }
        //----------------------------------------------------------------------------------------------------
        public void AjaxSender_Ready()
        {
            AjaxService.ASPdatabaseService.New(this, "GetInstallState_Return").Install__GetInstallState();
        }
        //----------------------------------------------------------------------------------------------------
        public void GetInstallState_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if(ajaxResponse.Error != null)
            {
                jF(".Screen").hide();
                jF(".Screen_Error").show().html(ajaxResponse.Error.Message + "<br /><br />To try again, refresh your browser.");
                return;
            }
            this.Model = ajaxResponse.ReturnObj.As<Objs.InstallInfo>();
            jF(".SubscriptionAgreement").html(this.Model.AboutPageInfo.SubscriptionAgreement);

            if (!this.ViewModel.LegalHasBeenAccepted)
                this.Model.InstallState = InstallInfo.InstallStates.LegalNotYetAccepted;

            
            jF(".Screen").hide();
            switch(this.Model.InstallState)
            {
                case InstallInfo.InstallStates.LegalNotYetAccepted:
                    jF(".Screen_GetStarted").show();
                    break;
                case InstallInfo.InstallStates.NoConnectionString:
                    if(this.ViewModel.IsFirstAttempt_AddingConnectionString)
                    {
                        this.ViewModel.IsFirstAttempt_AddingConnectionString = false;
                        jF(".Screen_NeedConnectionString").show();
                    }
                    else
                    {
                        jF(".Screen_ConnectionStringNotFound").show();
                        jF(".Label_InvalidConnectionString").html(this.Model.ResponseMsg);
                    }
                    break;
                case InstallInfo.InstallStates.CannotConnectToDB:
                        jF(".Screen_ConnectionStringNotFound").show();
                        jF(".Label_InvalidConnectionString").html(this.Model.ResponseMsg);
                    break;
                case InstallInfo.InstallStates.DatabaseNotReady:
                    if (!this.AjaxSender.IsReady)
                        this.AjaxSender.Initialize();
                    else
                        jF(".Screen_GetUserInfo").show();
                    break;
                case InstallInfo.InstallStates.Installed:
                    jF(".Screen_InstallComplete").show();
                    break;
            }
        }







        //------------------------------------------------------------------------------------------ Events --
        public void Bttn_GetStarted_Click()
        {
            jF(".Screen_GetStarted").hide();
            jF(".Screen_Legal").show();
        }
        //----------------------------------------------------------------------------------------------------
        public void Bttn_LegalNo_Click()
        {
            alert("You have clicked to not accept to this subscription agreement.\n\nThis page will now disappear.");
            J(".InstallMainUI").html("");
        }
        public void Bttn_LegalAccept_Click()
        {
            this.ViewModel.LegalHasBeenAccepted = true;
            AjaxSender_Ready();
        }
        //----------------------------------------------------------------------------------------------------
        public void NextBttn1_Click()
        {
            AjaxService.ASPdatabaseService.New(this, "GetInstallState_Return").Install__GetInstallState();
        }
        public void NextBttn2_Click()
        {
            jF(".Screen").hide();
            jF(".Screen_Loading").show();
            this.AjaxSender.IsReady = false;
            var thisObj = this;
            eval("setTimeout(function() { thisObj.NextBttn1_Click() }, 500);");
        }
        public void NextBttn3_Click()
        {
            if (!this.ValidateInfo())
                return;
            jF(".Screen_GetUserInfo").hide();
            jF(".Screen_InstallSQL").show();
        }
        public void NextBttn4_Click()
        {
            if (!this.ValidateInfo())
                return;
            var u = jF(".T_U").val().As<string>();
            var p = jF(".T_P").val().As<string>();
            var firstName = jF(".T_FirstName").val().As<string>() + "";
            var lastName = jF(".T_LastName").val().As<string>() + "";
            var email = jF(".T_Email").val().As<string>() + "";

            AjaxService.ASPdatabaseService.New(this, "GetInstallState_Return").YesEncryption().Install__InstallSQL(u, p, firstName, lastName, email);
        }
        private bool ValidateInfo()
        {
            var u = jF(".T_U").val().As<string>();
            var p = jF(".T_P").val().As<string>();
            var p2 = jF(".T_P2").val().As<string>();

            if (u == null || u.Length < 1)
            {
                alert("Please enter a Username");
                jF(".T_U").focus();
                return false;
            }
            if (p == null || p.Length < 1)
            {
                alert("Password must be at least 8 characters.\nPassword must contain letters and numbers.");
                jF(".T_P").focus();
                return false;
            }
            if(p != p2)
            {
                alert("Password do not match");
                jF(".T_P").focus();
                return false;
            }

            return true;
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
                .InstallMainUI { }
                .InstallMainUI .TopBar { height: 75px; line-height: 75px; background: #14498f; color: #fff; }
                .InstallMainUI .TopBar .LogoLink { float: left; display:block; width: auto; line-height: 40px; margin-top: 18px; 
                                                 margin-left: 15px; padding: 0px 14px; font-size: 1.25em; color:#fff; }
                .InstallMainUI .TopBar .LogoLink:hover { background: #0f366b; }

                .InstallMainUI .InstallBoxWrap { margin: auto; width: 38em; max-width: 38em; min-width: 38em; }

                .InstallMainUI .TopTitleSpace { margin-top: 2em; color:#5d5d5d; }
                .InstallMainUI .BodySpace { background: #5d5d5d; color: #fff; min-height: 15em; }

                .InstallMainUI .TopTitleSpace .InnerDiv1 { font-size: 1.8em; margin-bottom: .2em; }
                .InstallMainUI .BodySpace .InnerDiv2 { font-size: .8em; padding: 1.3em 0em; line-height: 1.4em; }
                .InstallMainUI .BodySpace .InnerDiv2 .SubHead { font-size: 1.7em; margin-bottom: 1.8em; }

                .InstallMainUI .Screen { display:none; }
                .InstallMainUI .Screen_Loading { }
                .InstallMainUI .Screen_Legal { }
                .InstallMainUI .Screen_Legal a { color: #fff; background: #777; padding: 0em .3em; margin: .5em 0em; float:left; display:block; }
                .InstallMainUI .Screen_Legal a:hover { background: #bbb; }
                .InstallMainUI .Screen_Legal .SubscriptionAgreement { overflow-y:scroll; max-height: 22em; background: #fff; color: #000; padding: .7em; }
                .InstallMainUI .Screen_NeedConnectionString { }
                .InstallMainUI .Screen_ConnectionStringNotFound { }
                .InstallMainUI .Screen_GetUserInfo { }
                .InstallMainUI .Screen_InstallSQL { }
                .InstallMainUI .Screen_Error { }
                .InstallMainUI .Screen_InstallComplete { }

                .InstallMainUI .Bttn { float:left; padding: .6em 2em; background: #ddd; color: #000; cursor:pointer; margin-right: 1em; }
                .InstallMainUI .Bttn:hover { background: #5d5d5d; color: #fff; }

                .InstallMainUI .Screen_GetUserInfo span { color: #999; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopBar'>
                    <a href='http://www.aspdatabase.net/' target='_blank' class='LogoLink'>ASPdatabase.NET</a>
                </div>

                <div class='TopTitleSpace'>
                    <div class='InstallBoxWrap'>
                        <div class='InnerDiv1'>Install</div>
                    </div>
                </div>
                <div class='BodySpace'>
                    <div class='InstallBoxWrap'>
                        <div class='InnerDiv2'>
                            <div class='SubHead'>Hello! Welcome to ASPdatabase.NET</div>

                            <div class='Screen Screen_Loading'>
                                Loading ...
                            </div>

                            <div class='Screen Screen_GetStarted'>
                                This installer will:<br />
                                 - Ask you to accept the Agreement<br />
                                 - Confirm you have a database connection string in the app's Web.config<br />
                                 - Create application tables<br />
                                 - Create an admin user<br />
                                <br />
                                <br />
                                <br />
                                <div class='Bttn' On_Click='Bttn_GetStarted_Click'>Start &gt;&gt;</div>
                                <div class='clear'></div>
                            </div>

                            <div class='Screen Screen_Legal'>
                                Do you accept this Subscription Agreement? <i>(applies to both free & paid accounts)</i> 
                                <br />
                                <a href='ASPdatabase.NET.aspx?SubscriptionAgreement' target='_blank'>[Open in new window]</a>
                                <div class='clear'></div>
                                
                                <div class='SubscriptionAgreement'></div>
                                <br />
                                <div class='Bttn Bttn_LegalNo' On_Click='Bttn_LegalNo_Click'>No</div>
                                <div class='Bttn Bttn_LegalAccept' On_Click='Bttn_LegalAccept_Click'>I Accept</div>
                                <div class='clear'></div>
                                <br />
                            </div>

                            <div class='Screen Screen_NeedConnectionString'>
                                To get started, please add a SQL Server connection string in the Web.config file.
                                <br /><br />
                                ASPdatabase.NET needs a SQL Server database to store application settings. You can either create a new SQL Server database for this purpose, or use an existing one. All application tables will be created using the schema name ""ASPdb"".
                                <br /><br />
                                If you have questions about this, please contact us.
                                <br />
                                <br />
                                <br />
                                <div class='Bttn NextBttn1' On_Click='NextBttn1_Click'>Click Here When Ready &gt;&gt;</div>
                                <div class='clear'></div>
                            </div>

                            <div class='Screen Screen_ConnectionStringNotFound'>
                                <div class='Label_InvalidConnectionString'></div>
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <div class='Bttn NextBttn1' On_Click='NextBttn2_Click'>Try Again &gt;&gt;</div>
                                <div class='clear'></div>
                            </div>

                            <div class='Screen Screen_GetUserInfo'>
                                Username<br />
                                <input type='text' class='T_U' />
                                <br />
                                <br />
                                Password<br />
                                <input type='password' class='T_P' />
                                <br />
                                <br />
                                Repeat Password<br />
                                <input type='password' class='T_P2' />
                                <br />
                                <br />
                                First Name<br />
                                <input type='text' class='T_FirstName' /> <span>(optional)</span>
                                <br />
                                <br />
                                Last Name<br />
                                <input type='text' class='T_LastName' /> <span>(optional)</span>
                                <br />
                                <br />
                                Email<br />
                                <input type='text' class='T_Email' /> <span>(optional)</span>
                                <br />
                                <br />
                                <div class='Bttn NextBttn3' On_Click='NextBttn3_Click'>Next &gt;&gt;</div>
                                <div class='clear'></div>
                            </div>

                            <div class='Screen Screen_InstallSQL'>
                                SQL tables will now be created.
                                <br />
                                <br />
                                <br />
                                <br />
                                <br />
                                <div class='Bttn NextBttn4' On_Click='NextBttn4_Click'>Next &gt;&gt;</div>
                                <div class='clear'></div>
                            </div>

                            <div class='Screen Screen_Error'>
                            </div>

                            <div class='Screen Screen_InstallComplete'>
                                ASPdatabase.NET is installed.<br />
                                Please refresh your browser.
                            </div>

                        </div>
                    </div>
                </div>


            ";
        }
    }
}
