using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdb.FrameworkUI.Coms;
using ASPdatabaseNET.DataObjects.DatabaseConnections;

namespace ASPdatabaseNET.UI.PageParts.ConnectionProperties
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ConnectionPropertiesMainUI : MRBPattern<DataObjects.SQLObjects.ASPdb_Connection, string>
    {
        public int ConnectionId = -1;
        public bool IsNewConnection
        {
            get
            {
                return (this.ConnectionId < 0);
            }
        }
        public int Temp_PasswordLength = 0;

        public ConnectionPropertiesBttns ConnectionPropertiesBttns;
        public RadioMenu RadioMenu_ActiveStatus;
        public RadioMenu RadioMenu_ConnectionTypes;
        public RadioMenu RadioMenu_ParametersTypes;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ConnectionPropertiesMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ConnectionPropertiesUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.ConnectionPropertiesBttns = new ConnectionPropertiesBttns();
            jF2(".Holder_TopBttnsBar").append(this.ConnectionPropertiesBttns.jRoot);
            this.ConnectionPropertiesBttns.OnRequestTest.After.AddHandler(this, "SubBttnClick_Test", 1);
            this.ConnectionPropertiesBttns.OnSave.After.AddHandler(this, "SubBttnClick_Save", 0);
            this.ConnectionPropertiesBttns.OnDelete.After.AddHandler(this, "SubBttnClick_Delete", 0);
            this.ConnectionPropertiesBttns.OnClose.After.AddHandler(this, "ExitPage", 0);

            this.RadioMenu_ActiveStatus = new RadioMenu("RadioMenu_ActiveStatus");
            jF2(".Holder_RadioMenu_ActiveStatus").append(this.RadioMenu_ActiveStatus.jRoot);
            this.RadioMenu_ActiveStatus.ViewModel = this.Get_RadioList_ActiveStatus();
            this.RadioMenu_ActiveStatus.Open();

            this.RadioMenu_ConnectionTypes = new RadioMenu("RadioMenu_ConnectionTypes");
            jF2(".Holder_RadioMenu_ConnectionTypes").append(this.RadioMenu_ConnectionTypes.jRoot);
            this.RadioMenu_ConnectionTypes.OnChange.After.AddHandler(this, "RadioMenu_ConnectionTypes_Change", 0);
            this.RadioMenu_ConnectionTypes.ViewModel = this.Get_RadioList_ConnectionTypes();
            this.RadioMenu_ConnectionTypes.Open();

            this.RadioMenu_ParametersTypes = new RadioMenu("RadioMenu_ParametersTypes");
            jF2(".Holder_RadioMenu_ParametersTypes").append(this.RadioMenu_ParametersTypes.jRoot);
            this.RadioMenu_ParametersTypes.OnChange.After.AddHandler(this, "RadioMenu_ParametersTypes_Change", 0);

            this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private ASPdb.FrameworkUI.MRB.GenericUIList Get_RadioList_ActiveStatus()
        {
            var rtn = new ASPdb.FrameworkUI.MRB.GenericUIList();
            rtn.Add1("Active");
            rtn.Add1("Hidden");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private ASPdb.FrameworkUI.MRB.GenericUIList Get_RadioList_ConnectionTypes()
        {
            var rtn = new ASPdb.FrameworkUI.MRB.GenericUIList();
            rtn.Add2("SQLServer", "SQL Server");
            rtn.Add2("SQLServerAzure", "SQL Server Azure");
            //rtn.Add2("MySQL", "MySQL");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private ASPdb.FrameworkUI.MRB.GenericUIList Get_RadioList_ParametersTypes___SQLServer()
        {
            var rtn = new ASPdb.FrameworkUI.MRB.GenericUIList();
            rtn.Add2("WindowsIntegratedSecurity", "Connection with Windows Integrated Security (recommended)");
            rtn.Add2("SQLServerCredentials", "Connection with SQL Server Username & Password");
            rtn.Add2("ConnectionString", "Custom Connection String");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private ASPdb.FrameworkUI.MRB.GenericUIList Get_RadioList_ParametersTypes___SQLServerAzure()
        {
            var rtn = new ASPdb.FrameworkUI.MRB.GenericUIList();
            rtn.Add2("SQLServerCredentials", "Connection with SQL Server Username & Password");
            rtn.Add2("ConnectionString", "Custom Connection String");
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private ASPdb.FrameworkUI.MRB.GenericUIList Get_RadioList_ParametersTypes___MySQL()
        {
            var rtn = new ASPdb.FrameworkUI.MRB.GenericUIList();
            rtn.Add2("???", "??? - Connection with Windows Integrated Security (recommended) - ???");
            rtn.Add2("MySQLCredentials", "Connection with MySQL Username & Password");
            rtn.Add2("ConnectionString", "Custom Connection String");
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            try { this.ConnectionId = 1 * PagesFramework.PageIdentifier.GetFromUrlHash().PageParam2.As<JsNumber>(); }
            catch { this.ConnectionId = -1; }
            if (isNaN(this.ConnectionId)) this.ConnectionId = -1;

            this.ConnectionPropertiesBttns.IsNewConnection = this.IsNewConnection;
            this.ConnectionPropertiesBttns.Open();

            jF2(".PleaseWait").hide();
            jF2(".HideInitially").hide();
            if (this.IsNewConnection)
            {
                this.Model = new DataObjects.SQLObjects.ASPdb_Connection();
                this.Load_FromModel();
            }
            else
            {
                jF2(".PleaseWait").show();

                var thisObj = this;
                if (ASPdb.Security.AjaxSender.GetObj().IsReady)
                    this.SendAjaxGet();
                else
                    eval("setTimeout(function(){ thisObj.SendAjaxGet() }, 2500);");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void SendAjaxGet()
        {
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Get_AjaxReturn").YesEncryption().DatabaseConnections__Get(this.ConnectionId);
        }


        //------------------------------------------------------------------------------------------ Events --
        public void Get_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            this.Model = ajaxResponse.ReturnObj
                .As<DataObjects.DatabaseConnections.DatabaseConnectionResponse>().SingleConnectionInfo;

            this.Load_FromModel();
        }
        //----------------------------------------------------------------------------------------------------
        private void Load_FromModel()
        {
            jF2(".PleaseWait").hide();
            jF2(".HideInitially").show();

            jF2(".Section_ConnectionMethod").hide();
            jF2(".Section_Properties").hide();

            this.BindUI();

            if (this.Model.Active)
                this.RadioMenu_ActiveStatus.Model = "Active";
            else
                this.RadioMenu_ActiveStatus.Model = "Hidden";

            this.RadioMenu_ConnectionTypes.Model = this.Model.ConnectionType;
            this.RadioMenu_ParametersTypes.Model = this.Model.ParametersType;
            this.RadioMenu_ConnectionTypes_Change();
            this.RadioMenu_ParametersTypes_Change();

            jF2(".Txt_ServerAddress").val(this.Model.Param_ServerAddress);
            jF2(".Txt_DatabaseName").val(this.Model.Param_DatabaseName);
            jF2(".Txt_Username").val(this.Model.Param_U);
            jF2(".Txt_Password").val(this.Model.Param_P);
            jF2(".Txt_ConnectionString").val(this.Model.Param_ConnectionString);

            this.Temp_PasswordLength = this.Model.Param_P.Length;

            if (this.IsNewConnection || !this.Model.Active)
            {
                jF2(".InfoBox").hide();
                jF2(".Bttn_GoToAssetManager").hide();
            }
            else
            {
                jF2(".InfoBox").show();
                jF2(".Bttn_GoToAssetManager").show();
                jF2(".Bttn_GoToAssetManager").attr("href", "#00-ManageAssets-" + this.ConnectionId);
            }
        }


        //----------------------------------------------------------------------------------------------------
        private void Update_Model_FromUI()
        {
            this.Model.Active = (this.RadioMenu_ActiveStatus.Model == "Active");
            this.Model.ConnectionName = jF2(".Txt_ConnectionName").val().As<string>();
            this.Model.ConnectionType = this.RadioMenu_ConnectionTypes.Model;
            this.Model.ParametersType = this.RadioMenu_ParametersTypes.Model;

            this.Model.Param_ServerAddress = jF2(".Txt_ServerAddress").val().As<string>();
            this.Model.Param_DatabaseName = jF2(".Txt_DatabaseName").val().As<string>();
            this.Model.Param_U = jF2(".Txt_Username").val().As<string>();
            this.Model.Param_P = jF2(".Txt_Password").val().As<string>();
            this.Model.Param_ConnectionString = jF2(".Txt_ConnectionString").val().As<string>();
        }
        //----------------------------------------------------------------------------------------------------
        public void SubBttnClick_Test(bool saveOnSuccess)
        {
            this.Update_Model_FromUI();
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Test_AjaxReturn").YesEncryption().DatabaseConnections__Test(this.Model, saveOnSuccess);
        }
        public void Test_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var obj = ajaxResponse.ReturnObj.As<DatabaseConnectionResponse>();
            this.ConnectionPropertiesBttns.Set_TestResponse(
                obj.TestConnection_Passed, 
                obj.TestConnection_Message,
                obj.SaveOnSuccess);
        }
        //----------------------------------------------------------------------------------------------------
        public void SubBttnClick_Save()
        {
            var txt_ConnectionName = jF2(".Txt_ConnectionName");
            if (txt_ConnectionName.val().As<string>().Length < 1)
            {
                alert("Please give this connection a name.");
                txt_ConnectionName.focus();
                return;
            }

            this.OnChange.Before.Fire();
            this.Update_Model_FromUI();
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Save_AjaxReturn").YesEncryption().DatabaseConnections__Save(this.Model);
        }
        public void Save_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            this.OnChange.After.Fire();
            this.ExitPage();
        }
        //----------------------------------------------------------------------------------------------------
        public void SubBttnClick_Delete()
        {
            if (confirm("Are you sure you want to delete this connection?\n\nThere is no undo for this!"))
            {
                ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Delete_AjaxReturn").YesEncryption().DatabaseConnections__Delete(this.ConnectionId);
            }
        }
        public void Delete_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ExitPage();
        }
        //----------------------------------------------------------------------------------------------------
        public void ExitPage()
        {
            if (PagesFramework.PageIdentifier.GetFromUrlHash().PageParam3 == "MA" && !this.IsNewConnection)
                window.location.href = "#00-ManageAssets-" + this.ConnectionId;
            else
                window.location.href = "#00-Connections";
        }
        //----------------------------------------------------------------------------------------------------
        public void Txt_Password_OnFocus()
        {
            var txt_Password = jF2(".Txt_Password");
            string value = txt_Password.val().As<string>();
            value = JsStr.Replace(value, "#", "");
            if (value.Length < 1)
                txt_Password.val("");
        }
        //----------------------------------------------------------------------------------------------------
        public void Txt_Password_OnBlur()
        {
            var txt_Password = jF2(".Txt_Password");
            string value = txt_Password.val().As<string>();
            value = JsStr.Replace(value, "#", "");
            if (value.Length < 1)
            {
                string s = "";
                for (int i = 0; i < this.Temp_PasswordLength; i++) 
                    s += "#";
                txt_Password.val(s);
            }
        }




        //----------------------------------------------------------------------------------------------------
        public void RadioMenu_ConnectionTypes_Change()
        {
            bool hideParameters = false;
            switch (this.RadioMenu_ConnectionTypes.Model)
            {
                case "SQLServer":
                    this.RadioMenu_ParametersTypes.ViewModel = this.Get_RadioList_ParametersTypes___SQLServer();
                    break;
                case "SQLServerAzure":
                    this.RadioMenu_ParametersTypes.ViewModel = this.Get_RadioList_ParametersTypes___SQLServerAzure();
                    break;
                case "MySQL":
                    this.RadioMenu_ParametersTypes.ViewModel = this.Get_RadioList_ParametersTypes___MySQL();
                    break;
                default:
                    hideParameters = true;
                    break;
            }
            jF2(".Section_Properties").hide();
            if (hideParameters)
            {
                jF2(".Section_ConnectionMethod").hide();
            }
            else
            {
                jF2(".Section_ConnectionMethod").show();
                this.RadioMenu_ParametersTypes.Open();
                this.RadioMenu_ParametersTypes.Model = this.Model.ParametersType;
                this.RadioMenu_ParametersTypes_Change();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void RadioMenu_ParametersTypes_Change()
        {
            jF2(".Element_X").hide();

            jF2(".Section_Properties").show();
            if (this.RadioMenu_ParametersTypes.GetSelectedValue() == null)
                jF2(".Section_Properties").hide();

            var flags = new int[] { 0, 0, 0, 0, 0 };
            switch (this.RadioMenu_ParametersTypes.Model)
            {
                case "WindowsIntegratedSecurity": flags = new int[] { 1, 1, 0, 0, 0 };
                    break;
                case "SQLServerCredentials": flags = new int[] { 1, 1, 1, 1, 0 };
                    break;
                case "ConnectionString": flags = new int[] { 0, 0, 0, 0, 1 };
                    break;
                case "MySQLCredentials": flags = new int[] { 1, 1, 1, 1, 0 };
                    break;
                default:
                    jF2(".Section_Properties").hide();
                    break;
            }
            

            if (flags[0] == 1) jF2(".Element_ServerAddress").show();
            if (flags[1] == 1) jF2(".Element_ConnectionName").show();
            if (flags[2] == 1) jF2(".Element_Username").show();
            if (flags[3] == 1) jF2(".Element_Password").show();
            if (flags[4] == 1) jF2(".Element_ConnectionString").show();
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += ConnectionPropertiesBttns.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ConnectionPropertiesUI { }
                .ConnectionPropertiesUI .PleaseWait { margin: 1.5em 1em; }
                .ConnectionPropertiesUI .Holder_TopBttnsBar { position: relative; top: -55px; right: 0px; width: 47.5em; }

                .ConnectionPropertiesUI .LeftColumn { float: left; width: 32em; }
                .ConnectionPropertiesUI .RightColumn { float: left; width: 10em; margin-left: .9em; }


                .ConnectionPropertiesUI .LeftColumn .Label1 { color: #14498f; font-size: 1.15em; margin-bottom: .2em; }
                .ConnectionPropertiesUI .LeftColumn .Txt_ConnectionName { background: #becada; color: #173a67; line-height: 2em; height: 2em; font-size: 1.3em;
                                                                          width: 22em; padding: 0em .8em; border: .125em solid #173a67; }
                .ConnectionPropertiesUI .LeftColumn .Txt_ConnectionName:focus { background: #fff; }
                .ConnectionPropertiesUI .LeftColumn .Label2 { color: #959595; font-size: .58em; margin-top: 2px; margin-right: 7em; text-align: right; }

                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType { margin-top: 1.1875em; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType .SectionHead1 { position: relative; top: 1px;
                                                                                          background: #929292; color: #fff; line-height: 1.69117em; font-size: .85em;
                                                                                          width: 13em; text-align: center; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType .SectionBody1 { border: 1px solid #d4d4d4; background: #ededed; padding: 0.5625em 0.2em 0.5625em 0.6875em; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType .SectionBody1 .Left1 { float: left; width: 11em; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType .SectionBody1 .Right1 { float: left; width: 26em; margin-left: 1.2em; padding: 0.35em 0.89em; color: #14498f; 
                                                                                                    font-size: .65em; cursor:pointer; line-height: 1.60714em; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType .SectionBody1 .Right1:hover { background: #aaa; color: #fff; }

                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionType .SectionBody1 .Left1 .Item { font-size: .8em; }

                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionMethod { font-size: .9em; border: 1px solid #d4d4d4; background: #ededed; padding: 0.625em 0.76em; margin-top: 2em; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionMethod .Label { color: #173a67; margin-bottom: 0.5555em; }
                .ConnectionPropertiesUI .LeftColumn .Section_ConnectionMethod .Item { font-size: .9em; }


                .ConnectionPropertiesUI .LeftColumn .Section_Properties { font-size: .9em; border: 1px solid #d4d4d4; background: #ededed; padding: 1.04em 0.76em 0.347em 2.7em; margin: 2.08em 0em; }
                .ConnectionPropertiesUI .LeftColumn .Section_Properties .Label { color: #173a67; font-size: .9em; }
                .ConnectionPropertiesUI .LeftColumn .Section_Properties input { width: 30em; margin-bottom: 1.04166em; border: 1px solid #b7b7b7; }
                .ConnectionPropertiesUI .LeftColumn .Section_Properties textarea { width: 30em; height: 4.75em; margin-bottom: 1.04166em; border: 1px solid #b7b7b7; }

                .ConnectionPropertiesUI .LeftColumn .Section_Properties .Txt_Password { margin-bottom: 0.35em; }
                .ConnectionPropertiesUI .LeftColumn .Section_Properties .SubLabel_Password { margin-bottom: 1.852em; font-size: .75em; }


                .ConnectionPropertiesUI .RightColumn .Holder_RadioMenu_ActiveStatus { border: 1px solid #d4d4d4; background: #ededed; padding: .5em 0.6875em; margin-top: .25em; }

                .ConnectionPropertiesUI .RightColumn .InfoBox { margin: 0.9375em 0em 0em 1.625em; }
                .ConnectionPropertiesUI .RightColumn .InfoBox .Label { color: #929292; font-size: 0.64em; }
                .ConnectionPropertiesUI .RightColumn .InfoBox .Data { color: #173a67; font-size: .8em; margin: 0em 0em 0.9375em 0.85em; }

                .ConnectionPropertiesUI .RightColumn .Bttn_GoToAssetManager { display:block; border: 1px solid #d4d4d4; line-height: 1.875em; 
                                                                              text-align: center; color: #14498f; 
                                                                              font-size: .7em; cursor: pointer; }
                .ConnectionPropertiesUI .RightColumn .Bttn_GoToAssetManager:hover { background: #d4d4d4; }



                .ConnectionPropertiesUI .Coms_RadioMenu .Item { cursor:pointer; line-height: 1.44em; padding-left: .75em; font-size: .95em; border-radius: 0.19em; }
                .ConnectionPropertiesUI .Coms_RadioMenu .Item:hover { background: #5f5f5f; color: #fff; }
                .ConnectionPropertiesUI .Coms_RadioMenu .Item input { margin-right: 1px; cursor: pointer; }
                .ConnectionPropertiesUI .Coms_RadioMenu .Selected { background: #d3d3d3; }


            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head'>Modify Database Connection</div>
                <div class='Main'>
                    <div class='PleaseWait'>
                        Loading ... Please Wait
                    </div>
                    <div class='Holder_TopBttnsBar HideInitially'>
                    </div>

                    <div class='LeftColumn HideInitially'>
                        <div class='Label1'>Database Connection Name</div>
                        <input type='text' class='Txt_ConnectionName' ModelKey='ConnectionName' />
                        <div class='Label2'>The name as vou would like it displayed in the ASPdatabase.NET application.</div>

                        <div class='Section_ConnectionType'>
                            <div class='SectionHead1'>Database Type</div>
                            <div class='SectionBody1'>
                                <div class='Left1 Holder_RadioMenu_ConnectionTypes'>
                                </div>
                                <a class='Right1' href='#00-SendFeedback'>
                                    Would you like to connect to other types of databases? 
                                    If so CLICK HERE to send your feedback.
                                </a>
                                <div class='clear'></div>
                            </div>
                        </div>

                        <div class='Section_ConnectionMethod'>
                            <div class='Label'>How would you like to connect to your database?</div>
                            <div class='Holder_RadioMenu_ParametersTypes'></div>
                        </div>

                        <div class='Section_Properties'>
                            <div class='Label Element_X Element_ServerAddress'>
                            Server Address</div>
                            <input class='Element_X Element_ServerAddress Txt_ServerAddress' type='text' />

                            <div class='Label Element_X Element_ConnectionName'>
                            Database Name</div>
                            <input class='Element_X Element_ConnectionName Txt_DatabaseName' type='text' />

                            <div class='Label Element_X Element_Username'>
                            Username</div>
                            <input class='Element_X Element_Username Txt_Username' type='text' />

                            <div class='Label Element_X Element_Password'>
                            Password</div>
                            <input class='Element_X Element_Password Txt_Password' type='password' 
                                On_Focus='Txt_Password_OnFocus' 
                                On_Blur='Txt_Password_OnBlur' />
                            <div class='Element_X Element_Password SubLabel_Password'>
                                Passwords are stored using machine-specific encryption.<br />
                                If you move this application to a different web server you will need to reenter this password.
                                Industry standards are used for encryption; however, ASPdatabase.NET is not responsible for any security breach on your system.
                            </div>

                            <div class='Label Element_X Element_ConnectionString'>
                            Connection String</div>
                            <textarea class='Element_X Element_ConnectionString Txt_ConnectionString'></textarea>

                        </div>
                    </div>

                    <div class='RightColumn HideInitially'>
                        <div class='Holder_RadioMenu_ActiveStatus'>
                        </div>
                        <div class='InfoBox'>
                            <div class='Label'>Connection Id</div>
                            <div class='Data' ModelKey='ConnectionId'></div>

                            <div class='Label'>Date Created</div>
                            <div class='Data' ModelKey='DateTimeCreated_String'></div>

                            <div class='hide'>
                            <div class='Label'>Created By</div>
                            <div class='Data' ModelKey='CreatedByUsername'></div>

                            <div class='Label'>Table Count</div>
                            <div class='Data' ModelKey='TableCount'></div>
                            </div>

                        </div>
                        <a class='HideInitially Bttn_GoToAssetManager'>Go to Asset Manager</a>
                    </div>
                    

                    <div class='clear'></div>
                </div>
            ";
        }


    }
}