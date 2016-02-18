using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.MyAccount.Objs;

namespace ASPdatabaseNET.UI.PageParts.MyAccount
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class MyAccountMainUI : MRBPattern<MyAccountInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public MyAccountMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='MyAccountMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jF(".D_MainBox").hide();
            jF(".D_ResetPasswordBox").hide();
            AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").MyAccount__GetInfo();
        }
        public void GetInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            this.Model = ajaxResponse.ReturnObj.As<MyAccountInfo>();
            this.BindUI();
            jF(".D_ResetPasswordBox").hide();
            jF(".D_MainBox").show();
        }


        //----------------------------------------------------------------------------------------------------
        public void Save_Click()
        {
            this.Model.FirstName = jF(".T_FirstName").val().As<string>();
            this.Model.LastName = jF(".T_LastName").val().As<string>();
            this.Model.Email = jF(".T_Email").val().As<string>();
            AjaxService.ASPdatabaseService.New(this, "Save_Return").YesEncryption().MyAccount__Save(this.Model);
            jF(".D_MainBox").hide();
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            alert("Saved");
            this.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void ResetPassword_Click()
        {
            jF(".T_PasswordOld").val("");
            jF(".T_Password1").val("");
            jF(".T_Password2").val("");

            jF(".D_MainBox").hide();
            jF(".D_ResetPasswordBox").show();
        }

        //----------------------------------------------------------------------------------------------------
        public void SavePassword_Click()
        {
            AjaxService.ASPdatabaseService.New(this, "Save_Return")
                .YesEncryption()
                .MyAccount__ResetPassword(
                    this.Model, 
                    jF(".T_PasswordOld").val().As<string>(),
                    jF(".T_Password1").val().As<string>(),
                    jF(".T_Password2").val().As<string>());

            jF(".T_PasswordOld").val("");
        }
        //----------------------------------------------------------------------------------------------------
        public void CancelPassword_Click()
        {
            jF(".D_ResetPasswordBox").hide();
            jF(".D_MainBox").show();
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
                .MyAccountMainUI { }
                .MyAccountMainUI .S_Username { color:blue; }
                .MyAccountMainUI .Float1 { float:left; width: 18.75em; }
                .MyAccountMainUI input { width: 16em; border: 1px solid #14498f; }
                .MyAccountMainUI .Bttn1 { float:left; background: #14498f; color: #fff; margin-right: 1em; cursor:pointer; padding: .25em 1.2em; }
                .MyAccountMainUI .Bttn1:hover { background: #333; }

                .MyAccountMainUI .D_ResetPasswordBox { display:none; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head'>My Account Settings</div>
                <div class='Main'>
                    <div class='D_MainBox'>
                        <br />
                        Username: <span class='S_Username' ModelKey='Username'></span>
                        <br />
                        <br />
                        <div class='Float1'>
                            First Name <br />
                            <input type='text' class='T_FirstName' ModelKey='FirstName' />
                        </div>
                        <div class='Float1'>
                            Last Name <br />
                            <input type='text' class='T_LastName' ModelKey='LastName' />
                        </div>
                        <div class='clear'></div>
                        <br />
                        Email <br />
                        <input type='text' class='T_Email' ModelKey='Email' />
                        <br />
                        <br />
                        <br />
                        <div class='Bttn1' On_Click='Save_Click'>Save</div>
                        <div class='Bttn1' On_Click='ResetPassword_Click'>Reset Password</div>
                        <div class='clear'></div>
                    </div>

                    <div class='D_ResetPasswordBox'>
                        <br />
                        Username: <span class='S_Username' ModelKey='Username'></span>
                        <br />
                        <br />
                        <br />
                        <br />
                        Old Password <br />
                        <input type='password' class='T_PasswordOld' />
                        <br />
                        <br />
                        New Password <br />
                        <input type='password' class='T_Password1' />
                        <br />
                        <br />
                        Repeat New Password <br />
                        <input type='password' class='T_Password2' />
                        <br />
                        <br />
                        <br />
                        <div class='Bttn1' On_Click='SavePassword_Click'>Save Password</div>
                        <div class='Bttn1' On_Click='CancelPassword_Click'>Cancel</div>
                    </div>

                </div>
            ";
        }
    }
}