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
    public class Panel_UserUI_ResetPassword : MRBPattern<UserInfo, UsersViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public Panel_UserUI_ResetPassword()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Panel_UserUI_ResetPassword jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            this.BindUI();
            jF(".P1").focus();
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Save_Click()
        {
            string p1 = jF(".P1").val().As<string>();
            string p2 = jF(".P2").val().As<string>();
            if (p1 != p2) { alert("Passwords don't match."); jF(".P1").focus(); return; }
            if (p1.Length < 8) { alert("Passwords must be at least 8 characters."); jF(".P1").focus(); return; }

            AjaxService.ASPdatabaseService.New(this, "SavePassword_Return").Users__SavePassword(this.Model.UserId, p1);
        }
        public void SavePassword_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if(UI.Errors.ExceptionHandler.Check(ajaxResponse))
            {
                jF(".P1").focus();
                return;
            }
            alert("Password Saved");
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
                .Panel_UserUI_ResetPassword { width: 13.125em; padding: 2em 0em 0em 3em; }
                .Panel_UserUI_ResetPassword .Head1 { color: #3d7bcc; font-size: 1.5em; margin-bottom: 0.9375em; }
                .Panel_UserUI_ResetPassword .Label1 { font-size: .8em; line-height: 2em; color: #a1a1a1; }
                .Panel_UserUI_ResetPassword .Label1 span { color: #1161ca; }
                .Panel_UserUI_ResetPassword input  { font-size: .9em; width: 13.944444em; border: 1px solid #3d7bcc; background: #fff; color: #3d7bcc; 
                                                     margin-bottom: 1.5em; line-height: 1.5em; padding: .25em; }
                .Panel_UserUI_ResetPassword .BttnsTable1 { width: 100%; }
                .Panel_UserUI_ResetPassword .BttnsTable1 td { width: 50%; font-size: .8em; text-align:center; line-height: 2.265625em; color: #fff; cursor:pointer; }
                .Panel_UserUI_ResetPassword .BttnsTable1 .Td1 { background: #14498f; }
                .Panel_UserUI_ResetPassword .BttnsTable1 .Td2 { background: #3d7bcc; border-left: 1px solid #fff; }
                .Panel_UserUI_ResetPassword .BttnsTable1 td:hover { background: #6a6e73; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head1'>Reset Password</div>
                <div class='Label1'>UserId : <span ModelKey='UserId'></span></div>
                <div class='Label1'>Username : <span ModelKey='Username'></span></div>
                <br />
                <div class='Label1'>New Password</div>
                <input type='password' class='P1' />
                <div class='Label1'>Repeat New Password</div>
                <input type='password' class='P2' />
                <table class='BttnsTable1'>
                    <tr>
                        <td class='Td1' On_Click='Save_Click'>Save</td>
                        <td class='Td2' On_Click='Close'>Cancel</td>
                    </tr>
                </table>
            ";
        }
    }
}
