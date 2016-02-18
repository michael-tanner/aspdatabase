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
    public class Panel_UserUI : MRBPattern<UserInfo, UsersViewModel>
    {
        public Panel_UserUI_ResetPassword ResetPasswordUI;
        public Panel_UserUI_GroupItem[] GroupItems;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public Panel_UserUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Panel_UserUI jRoot AutoResize'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.ResetPasswordUI = new Panel_UserUI_ResetPassword();
            this.ResetPasswordUI.ViewModel = this.ViewModel;
            this.ResetPasswordUI.Instantiate();
            this.ResetPasswordUI.Close();
            this.ResetPasswordUI.OnClose.After.AddHandler(this, "ResetPasswordUI_Close", 0);
            jF(".Holder_ResetPassword").html("").append(this.ResetPasswordUI.jRoot);
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if(this.ResetPasswordUI.IsOpen)
                this.ResetPasswordUI.Close();

            jF(".Left").hide();
            jF(".Right").hide();
            jF(".Loading").show();
            AjaxService.ASPdatabaseService.New(this, "GetUser_Return").Users__GetUser(this.ViewModel.CurrentId);
        }
        public void GetUser_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<UserInfo>();
            this.BindUI();

            this.Model.Active = !this.Model.Active;
            this.ActiveBttn_Click();

            this.Model.IsAdmin = !this.Model.IsAdmin;
            this.AdminBttn_Click();

            var holder_PermissionGroups = jF(".Holder_PermissionGroups").html("");
            this.GroupItems = new Panel_UserUI_GroupItem[0];
            if(this.Model.UserGroups != null)
                for (int i = 0; i < this.Model.UserGroups.Length; i++)
                {
                    this.GroupItems[i] = new Panel_UserUI_GroupItem();
                    this.GroupItems[i].ViewModel = this.ViewModel;
                    this.GroupItems[i].Model = this.Model.UserGroups[i];
                    this.GroupItems[i].Instantiate();
                    holder_PermissionGroups.append(this.GroupItems[i].jRoot);
                }

            jF(".Loading").hide();
            jF(".Left").show();
            jF(".Right").show();
            if (this.Model.UserId < 0)
            {
                jF(".UserId").html("");
                jF(".ResetPasswordBttn").hide();
            }
            else
                jF(".ResetPasswordBttn").show();

        }






        //------------------------------------------------------------------------------------------ Events --
        public void SaveBttn_Click()
        {
            this.Model.Username = jF(".T_Username").val().As<string>();
            this.Model.Email = jF(".T_Email").val().As<string>();
            this.Model.FirstName = jF(".T_FirstName").val().As<string>();
            this.Model.LastName = jF(".T_LastName").val().As<string>();


            AjaxService.ASPdatabaseService.New(this, "SaveUser_Return").Users__SaveUser(this.Model);
        }
        public void SaveUser_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.CurrentId = ajaxResponse.ReturnObj.As<JsNumber>();
            this.OnChange.After.Fire();
            alert("Saved");
        }
        //----------------------------------------------------------------------------------------------------
        public void DeleteBttn_Click()
        {
            if(prompt("Are you certain? \n\nIf so, type \"delete user\"") == "delete user")
            {
                AjaxService.ASPdatabaseService.New(this, "DeleteUser_Return").Users__DeleteUser(this.Model.UserId);
            }
        }
        public void DeleteUser_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.CurrentId = -1;
            this.Model.UserId = -1;
            this.Close();
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void CancelBttn_Click()
        {
            this.Close();
        }
        //----------------------------------------------------------------------------------------------------
        public void ActiveBttn_Click()
        {
            this.Model.Active = !this.Model.Active;
            if (this.Model.Active)
            {
                jF(".ActiveBttn").addClass("ActiveBttn_On");
                jF(".C_Active").attr("checked", true);
            }
            else
            {
                jF(".ActiveBttn").removeClass("ActiveBttn_On");
                jF(".C_Active").removeAttr("checked");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void AdminBttn_Click()
        {
            this.Model.IsAdmin = !this.Model.IsAdmin;
            if (this.Model.IsAdmin)
            {
                jF(".AdminBttn").addClass("AdminBttn_On");
                jF(".C_Admin").attr("checked", true);
            }
            else
            {
                jF(".AdminBttn").removeClass("AdminBttn_On");
                jF(".C_Admin").removeAttr("checked");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ResetPasswordBttn_Click()
        {
            jF(".MainForm").hide();
            this.ResetPasswordUI.Model = this.Model;
            this.ResetPasswordUI.Open();
        }
        public void ResetPasswordUI_Close()
        {
            jF(".MainForm").show();
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += Panel_UserUI_ResetPassword.GetCssTree();
            rtn += Panel_UserUI_GroupItem.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .Panel_UserUI { }
                .Panel_UserUI .MainForm { margin-top: 2.1875em; padding: 0em 1.4em; background: #f8f8f8; border-left: 1px solid #fff; }
                .Panel_UserUI .MainForm .Loading {  line-height: 3.25em; text-align: center; color: #777; }
                .Panel_UserUI .MainForm .Left {  display:none; float:left; width: 13em; }
                .Panel_UserUI .MainForm .Right { display:none; float:right; width: 16em; }

                .Panel_UserUI .MainForm .ActiveBttn { font-size: .8em; position:relative; height: 2.265625em; color: #a1a1a1; cursor:pointer; }
                .Panel_UserUI .MainForm .ActiveBttn:hover { background: #6a6e73; color: #fff; }
                .Panel_UserUI .MainForm .ActiveBttn_On { background: #eb640a; color: #fff; }
                .Panel_UserUI .MainForm .ActiveBttn input { position:absolute; top: 0.625em; left: 1.25em; }
                .Panel_UserUI .MainForm .ActiveBttn span { position:absolute; top: 0.46875em; left: 3.75em; }

                .Panel_UserUI .MainForm .AdminBttn { font-size: .8em; position:relative; height: 2.265625em; color: #a1a1a1; cursor:pointer; margin-top: 1em; }
                .Panel_UserUI .MainForm .AdminBttn:hover { background: #6a6e73; color: #fff; }
                .Panel_UserUI .MainForm .AdminBttn_On { background: #eb640a; color: #fff; }
                .Panel_UserUI .MainForm .AdminBttn input { position:absolute; top: 0.625em; left: 1.25em; }
                .Panel_UserUI .MainForm .AdminBttn span { position:absolute; top: 0.46875em; left: 3.75em; }

                .Panel_UserUI .MainForm .Label1 { font-size: .8em; line-height: 3.90625em; color: #a1a1a1; }
                .Panel_UserUI .MainForm .Label1 .UserId { color: #1161ca; }

                .Panel_UserUI .MainForm .Label2 { font-size: .8em; line-height: 1.484375em; color: #a1a1a1; }

                .Panel_UserUI .MainForm input[type=text] { font-size: .9em; width: 13.944444em; border: 1px solid #3d7bcc; background: #fff; color: #3d7bcc; margin-bottom: 1.1em; line-height: 1.5em; padding: .25em; }

                .Panel_UserUI .MainForm .ResetPasswordBttn { font-size: .8em; line-height: 2.265625em; background: #3d7bcc; color: #fff; text-align:center; cursor:pointer; margin-bottom: 1.2em; }
                .Panel_UserUI .MainForm .ResetPasswordBttn:hover { background: #6a6e73; }

                .Panel_UserUI .MainForm .Label3 { font-size: .6em; line-height: 2.5em; color: #a1a1a1; white-space:nowrap; }
                .Panel_UserUI .MainForm .Label3 span { color: #1161ca; }

                .Panel_UserUI .MainForm .BttnsTable { width: 100%; }
                .Panel_UserUI .MainForm .BttnsTable td { font-size: .8em; width: 33%; text-align:center; line-height: 2.265625em; color: #fff; cursor:pointer; }
                .Panel_UserUI .MainForm .BttnsTable .SaveBttn { background: #14498f; }
                .Panel_UserUI .MainForm .BttnsTable .SaveBttn:hover { background: #6a6e73; }
                .Panel_UserUI .MainForm .BttnsTable .Bttn1 { background: #3d7bcc; border-left: 1px solid #fff; }
                .Panel_UserUI .MainForm .BttnsTable .Bttn1:hover { background: #6a6e73; }

                .Panel_UserUI .MainForm .Label_PermissionGroups { font-size: 1em; line-height: 1.875em; margin-top: 2.4375em; color: #a1a1a1; }
                .Panel_UserUI .MainForm .Holder_PermissionGroups { border-top: 1px solid #aac5e9; background: #fff; min-height: 21em; margin-bottom: 0.75em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='MainForm'>
                    <div class='Loading'>Loading ...</div>
                    <div class='Left'>
                        <div class='ActiveBttn' On_Click='ActiveBttn_Click'>
                            <input type='checkbox' class='C_Active' />
                            <span>Active</span>
                            &nbsp;
                        </div>
                        <div class='AdminBttn' On_Click='AdminBttn_Click'>
                            <input type='checkbox' class='C_Admin' />
                            <span>Admin</span>
                            &nbsp;
                        </div>
                        <div class='Label1'>UserId : <span class='UserId' ModelKey='UserId'></span></div>

                        <div class='Label2'>Username</div>
                        <input type='text' class='T_Username' ModelKey='Username' />

                        <div class='Label2'>Email</div>
                        <input type='text' class='T_Email' ModelKey='Email' />

                        <div class='Label2'>First Name</div>
                        <input type='text' class='T_FirstName' ModelKey='FirstName' />

                        <div class='Label2'>Last Name</div>
                        <input type='text' class='T_LastName' ModelKey='LastName' />

                        <div class='ResetPasswordBttn' On_Click='ResetPasswordBttn_Click'>Reset Password</div>

                        <div class='Label3'>Time Created : <span ModelKey='TimeCreated_Str'></span></div>
                        <div class='Label3'>Last Login : <span ModelKey='LastLoginTime_Str'></span></div>
                    </div>
                    <div class='Right'>
                        <table class='BttnsTable'><tr>
                            <td class='SaveBttn'   On_Click='SaveBttn_Click'  >Save</td>
                            <td class='Bttn1'      On_Click='DeleteBttn_Click'>Delete</td>
                            <td class='Bttn1'      On_Click='CancelBttn_Click'>Cancel</td>
                        </tr></table>
                        <div class='Label_PermissionGroups'>Permission Groups</div>
                        <div class='Holder_PermissionGroups'></div>
                    </div>
                    <div class='clear'></div>
                </div>
                <div class='Holder_ResetPassword'>
                </div>
            ";
        }
    }
}
