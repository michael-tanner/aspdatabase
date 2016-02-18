using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.ConnectionProperties
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ConnectionPropertiesBttns : MRBPattern<string, string>
    {
        public JsEvent_BeforeAfter OnSave = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnDelete = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnRequestTest = new JsEvent_BeforeAfter();
        public bool IsNewConnection = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public ConnectionPropertiesBttns()
        {
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ConnectionPropertiesBttns jRoot NoSelect'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jF2(".TestMenu").hide();
            jF2(".SaveMenu").hide();

            var deleteBttn = jF2(".DeleteBttn");
            if (this.IsNewConnection)
            {
                deleteBttn.html("");
                deleteBttn.removeClass("Bttn");
                deleteBttn.addClass("Bttn_NoHover");
            }
            else
            {
                deleteBttn.html("Delete Connection");
                deleteBttn.removeClass("Bttn_NoHover");
                deleteBttn.addClass("Bttn");
            }
        }
        



        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_TestConnection()
        {
            this.BttnClick_TestConnection2(false);
        }
        public void BttnClick_TestConnection2(bool saveOnSuccess)
        {
            jF2(".TestMenu").show();
            jF2(".SaveMenu").hide();
            jF2(".BttnSave").hide();

            jF2(".TestResponse").html("Testing Connection ... Please Wait");
            this.OnRequestTest.After.Fire1(saveOnSuccess);
        }
        public void Set_TestResponse(bool testConnection_Passed, string testConnection_Message, bool saveOnSuccess)
        {
            string html = "";
            if (testConnection_Passed)
            {
                html += "<div class='Passed'>Connection Success</div>";
                jF2(".BttnSave").show();
            }
            else
            {
                html += "<div class='Failed'>Connection Did Not Work</div>";
                jF2(".BttnSave").hide();
            }
            html += "<div class='ScrollBox'>" + testConnection_Message + "</div>";

            jF2(".TestResponse").html(html);

            if (testConnection_Passed && saveOnSuccess)
            {
                alert("Connection successfully tested and will now be saved.");
                this.SaveMenuClick_SaveOnly();
            }
        }
        public void BttnClick_Save()
        {
            jF2(".TestMenu").hide();
            jF2(".SaveMenu").show();
        }
        public void BttnClick_Delete()
        {
            if (this.IsNewConnection)
                return;
            this.OnDelete.After.Fire();
        }
        public void BttnClick_Cancel()
        {
            this.OnClose.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void BttnClick_Save2()
        {
            this.SaveMenuClick_SaveOnly();
        }
        public void BttnClick_Dismiss()
        {
            jF2(".TestMenu").hide();
        }
        //----------------------------------------------------------------------------------------------------
        public void SaveMenuClick_TestAndSave()
        {
            this.BttnClick_TestConnection2(true);
        }
        public void SaveMenuClick_SaveOnly()
        {
            this.BttnClick_Dismiss();
            this.OnSave.After.Fire();
        }
        public void SaveMenuClick_Cancel()
        {
            jF2(".SaveMenu").hide();
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
                .ConnectionPropertiesBttns { }
                .ConnectionPropertiesBttns .Bttn_NoHover { float: right; width: 7.08em; height: 4.166em; text-align: center; z-index: 3; position:relative;
                                                           margin-left: 0.4166em; background: #e4e4e4; color: #fff; font-size: .75em; border-radius: 0.3333em; }
                .ConnectionPropertiesBttns .Bttn { float: right; width: 7.08em; height: 4.166em; text-align: center; z-index: 3; position:relative; cursor: pointer;
                                                   margin-left: 0.5555em; background: #14498f; color: #fff; font-size: .75em; border-radius: 0em; }
                .ConnectionPropertiesBttns .Bttn_1Line { line-height: 3.75em; height: 3.75em; }
                .ConnectionPropertiesBttns .Bttn_2Lines { padding-top: 0.625em; height: 3.125em; }
                .ConnectionPropertiesBttns .Bttn:hover { background: #071f40; }

                .ConnectionPropertiesBttns .TestBttn { z-index: 1; }
                .ConnectionPropertiesBttns .SaveBttn { z-index: 1; }


                .ConnectionPropertiesBttns .TestMenu { display:none; width: 7.08333em; font-size: .75em; position:absolute; top: 0px; right: 22.92em; z-index: 2; }
                .ConnectionPropertiesBttns .TestMenu .LabelBttn1 { background: #235ca7; color: #678dc7; text-align: center; 
                                                                   padding-top: 0.8333em; height: 3.333em; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 { background: #235ca7; color: #fff; width: 25em; position:absolute; right: 0px; padding: 1.0833em 1.6666em; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestHead { display:none; letter-spacing: 0.0833333em; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestResponse { margin: 0.16666em 0em 2.5em; line-height: 1.5em; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestResponse .Passed { font-size: 1.5em; color: #57f25b; background: #4e7dbb; padding: 0.1111em 0.2777em 0.2222em; margin-bottom: 0.3333em; text-align: center; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestResponse .Failed { font-size: 1.5em; color: #c60606; background: #bed4f0; padding: 0.1666em 0.2777em 0.2222em; margin-bottom: 0.3333em; text-align: center; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestResponse .ScrollBox { overflow-y: auto; overflow-x: hidden; max-height: 27em; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestResponse .TableItem1 { font-size: .9em; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .TestResponse .TableItem1 span { color: #9cb8da; }

                .ConnectionPropertiesBttns .TestMenu .Sub1 .BttnSave { float: right; width: 12.5em; background: #1d5094; margin-right: 0.6666em;
                                                                          line-height: 2.6666em; text-align: center; cursor: pointer; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .BttnSave:hover { background: #071f40; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .BttnDismiss { float: right; width: 7.5em; background: #1d5094;
                                                                          line-height: 2.6666em; text-align: center; cursor: pointer; }
                .ConnectionPropertiesBttns .TestMenu .Sub1 .BttnDismiss:hover { background: #071f40; }



                .ConnectionPropertiesBttns .SaveMenu { display:none; width: 18.333em; font-size: .75em; position:absolute; top: 0px; right: 4em; z-index: 2; }
                .ConnectionPropertiesBttns .SaveMenu .LabelBttn1 { background: #235ca7; color: #678dc7; text-align: center; 
                                                                     line-height: 4.1666em; width: 7.0833em; float: left; padding-bottom: 0.3333em; }
                .ConnectionPropertiesBttns .SaveMenu .Sub1 { background: #235ca7; color: #fff; }
                .ConnectionPropertiesBttns .SaveMenu .Sub1 .Bttn1 { line-height: 3.3333em; cursor: pointer; padding-left: 2.33333em; }
                .ConnectionPropertiesBttns .SaveMenu .Sub1 .Bttn1:hover { background: #071f40; }
                .ConnectionPropertiesBttns .SaveMenu .Sub1 .Bttn1 span { color: #7197cc; font-size: .8em; padding-left: 1.066em; }
                .ConnectionPropertiesBttns .SaveMenu .Sub1 .Bttn1:hover span { color: #5477a9; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Bttn Bttn_1Line'             On_Click='BttnClick_Cancel'         >Cancel</div>
                <div class='Bttn Bttn_2Lines DeleteBttn' On_Click='BttnClick_Delete'         >Delete Connection</div>
                <div class='Bttn Bttn_1Line SaveBttn'    On_Click='BttnClick_Save'           >Save</div>
                <div class='Bttn Bttn_2Lines TestBttn'   On_Click='BttnClick_TestConnection' >Test Connection</div>

                <div class='TestMenu'>
                    <div class='LabelBttn1'>Test Connection</div>
                    <div class='Sub1'>
                        <div class='TestHead'>Testing Connection</div>
                        <div class='TestResponse YesSelect'></div>
                        <div class='BttnDismiss' On_Click='BttnClick_Dismiss'>Dismiss</div>
                        <div class='BttnSave'    On_Click='BttnClick_Save2'>Save Connection</div>
                        <div class='clear'></div>
                    </div>
                </div>

                <div class='SaveMenu'>
                    <div class='LabelBttn1'>Save</div>
                    <div class='clear'>
                    <div class='Sub1'>
                        <div class='Bttn1' On_Click='SaveMenuClick_TestAndSave' >Test & Save Connection</div>
                        <div class='Bttn1' On_Click='SaveMenuClick_SaveOnly'    >Save Without Testing</div>
                        <div class='Bttn1' On_Click='SaveMenuClick_Cancel'      >Cancel <span>(Closes Save Options)</span></div>
                    </div>
                </div>
            ";
        }
    }
}