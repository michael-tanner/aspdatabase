using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.SendFeedback.Objs;

namespace ASPdatabaseNET.UI.PageParts.SendFeedback
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class SendFeedbackMainUI : MRBPattern<FeedbackInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public SendFeedbackMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='SendFeedbackMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jF(".Main1").hide();
            jF(".Main2").hide();
            jF(".HideOnAnonymous").show();
            jF(".DivCheckbox").removeClass("DivCheckbox_On");
            jF("input[type=text]").val("");
            jF("input[type=checkbox]").removeAttr("checked");
            jF("textarea").val("");
            AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").Feedback__GetInfo();
        }
        public void GetInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<FeedbackInfo>();
            this.BindUI();
            jF(".Main1").show();
            jF(".T_Name").focus();
        }



        
        //------------------------------------------------------------------------------------------ Events --
        public void D_RequestFollowup_Click()
        {
            this.Model.RequestFollowup = !this.Model.RequestFollowup;
            if(this.Model.RequestFollowup)
            {
                jF(".D_RequestFollowup").addClass("DivCheckbox_On");
                jF(".C_RequestFollowup").attr("checked", true);
            }
            else
            {
                jF(".D_RequestFollowup").removeClass("DivCheckbox_On");
                jF(".C_RequestFollowup").removeAttr("checked");
            }
        }
        public void D_SendAnonymously_Click()
        {
            this.Model.Anonymous = !this.Model.Anonymous;
            if(this.Model.Anonymous)
            {
                jF(".D_SendAnonymously").addClass("DivCheckbox_On");
                jF(".C_SendAnonymously").attr("checked", true);
                jF(".HideOnAnonymous").hide();
            }
            else
            {
                jF(".D_SendAnonymously").removeClass("DivCheckbox_On");
                jF(".C_SendAnonymously").removeAttr("checked");
                jF(".HideOnAnonymous").show();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void B_SendFeedBack_Click()
        {
            this.Model.Name = "";
            this.Model.Email = "";
            if(!this.Model.Anonymous)
            {
                this.Model.Name = jF(".T_Name").val().As<string>();
                this.Model.Email = jF(".T_Email").val().As<string>();
            }
            this.Model.Message = jF(".T_Message").val().As<string>();

            AjaxService.ASPdatabaseService.New(this, "Send_Return").Feedback__Send(this.Model);
        }
        public void Send_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;


            jF(".Main1").hide();
            jF(".Main2").show().html(ajaxResponse.ReturnObj.As<string>());
        }
        //----------------------------------------------------------------------------------------------------
        public void B_Cancel_Click()
        {
            window.location = "ASPdatabase.NET.aspx#00-Home".As<Location>();
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
                .SendFeedbackMainUI { }
                .SendFeedbackMainUI a { font-size: .7em; font-style: italic; }
                .SendFeedbackMainUI a:hover { background: #ddd; }
                .SendFeedbackMainUI input[type=text] { border:1px solid #2c5eab; width: 16em; }
                .SendFeedbackMainUI .T_Name { }
                .SendFeedbackMainUI .T_Email { }
                .SendFeedbackMainUI .D_AppVersion { border:1px solid #2c5eab; width: 16em; background: #e0e0e0; line-height: 1.375em; padding: 0em 0.1875em; }
                .SendFeedbackMainUI .C_RequestFollowup { }
                .SendFeedbackMainUI .C_SendAnonymously { }
                .SendFeedbackMainUI .T_Message { width: 35em; height: 9.5em; }
                .SendFeedbackMainUI .B_SendFeedBack { float:left; padding: .2em .9em; background: #134084; color: #fff; cursor:pointer; }
                .SendFeedbackMainUI .B_SendFeedBack:hover { background: #444; }

                .SendFeedbackMainUI .Bttn { float:left; padding: .2em .9em; background: #134084; color: #fff; cursor:pointer; margin-left: 1em; }
                .SendFeedbackMainUI .Bttn:hover { background: #444; }

                .SendFeedbackMainUI .DivCheckbox { float:left; padding: .2em 1em; width: 14em; border: 1px solid #fff; border-radius: .4em; cursor:pointer; }
                .SendFeedbackMainUI .DivCheckbox:hover { border-color: #888; background: #bbb; }
                .SendFeedbackMainUI .DivCheckbox_On { border-color: #134083; background: #5f8cd0; color: #fff; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head'>Send Feedback to Michael Tanner <a href='https://www.aspdatabase.net/' target='_blank'>(of ASPdatabase.NET LLC)</a></div>
                <div class='Main Main1'>
                    <br />
                    <br />
                    <div class='HideOnAnonymous'>
                        Name <br />
                        <input type='text' class='T_Name' ModelKey='Name' />
                        <br />
                        <br />
                        Email <br />
                        <input type='text' class='T_Email' ModelKey='Email' />
                        <br />
                        <br />
                        AppVersion <br />
                        <div class='D_AppVersion' ModelKey='AppVersion'>&nbsp;</div>
                        <br />
                        <br />

                        <div class='D_RequestFollowup DivCheckbox' On_Click='D_RequestFollowup_Click'>
                            <input type='checkbox' class='C_RequestFollowup' />
                            Request Followup
                        </div>
                        <div class='clear'></div>
                        <br />
                    </div>

                    <div class='D_SendAnonymously DivCheckbox' On_Click='D_SendAnonymously_Click'>
                        <input type='checkbox' class='C_SendAnonymously' />
                        Send Anonymously
                    </div>
                    <div class='clear'></div>
                    <br />

                    Message <br />
                    <textarea class='T_Message'></textarea>
                    <br />
                    <br />
                    <div class='B_SendFeedBack' On_Click='B_SendFeedBack_Click'>Send Feedback</div>
                    <div class='Bttn' On_Click='B_Cancel_Click'>Cancel</div>
                    <div class='clear'></div>
                </div>
                <div class='Main Main2'>
                </div>
            ";
        }
    }
}