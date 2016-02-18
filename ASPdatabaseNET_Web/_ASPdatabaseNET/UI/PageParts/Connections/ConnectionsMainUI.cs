using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.Connections
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ConnectionsMainUI : MRBPattern<string, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ConnectionsMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ConnectionsMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }

        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            jF2(".ActiveItemsDiv").html("");
            jF2(".HiddenItemsDiv").html("");
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "GetList_AjaxReturn").DatabaseConnections__GetList();

            jF2(".FrontLevelPage").show();
            UI.PagesFramework.BasePage.WindowResized();
        }


        //------------------------------------------------------------------------------------------ Events --
        public void GetList_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            var dbConnResponse = ajaxResponse.ReturnObj.As<ASPdatabaseNET.DataObjects.DatabaseConnections.DatabaseConnectionResponse>();

            jF2(".Span_ActiveConnections_Count").html("(" + dbConnResponse.ActiveConnections.Length + ")");
            jF2(".Span_HiddenConnections_Count").html("(" + dbConnResponse.HiddenConnections.Length + ")");
            jF2(".SectionDiv_NoConnections").show();

            jF2(".SectionDiv_Active").hide();
            if (dbConnResponse.ActiveConnections.Length > 0)
            {
                var div1 = jF2(".ActiveItemsDiv");
                for (int i = 0; i < dbConnResponse.ActiveConnections.Length; i++)
                {
                    var item = dbConnResponse.ActiveConnections[i];
                    string tmpIdClass = "TmpId_" + item.ConnectionId;
                    string html = ASPdb.FrameworkUI.JsStr.StrFormat3(@"
                            <div class='ConnectionItem'>
                                <div class='NameLabel'>{0}</div>
                                <div class='BttnsHolder'>
                                    <a class='Bttn_ModifyConnection {1}' href='#00-ConnectionProperties-{2}'>Edit Connection</a>
                                    <a class='Bttn_ManageAssets' href='#00-ManageAssets-{2}'>Manage Assets</a>
                                </div>
                                <div class='clear'></div>
                            </div>", item.ConnectionName, tmpIdClass, item.ConnectionId.As<string>());
                    div1.append(html);
                }
                jF2(".SectionDiv_NoConnections").hide();
                jF2(".SectionDiv_Active").show();
            }

            jF2(".SectionDiv_Hidden").hide();
            if (dbConnResponse.HiddenConnections.Length > 0)
            {
                var div2 = jF2(".HiddenItemsDiv");
                for (int i = 0; i < dbConnResponse.HiddenConnections.Length; i++)
                {
                    var item = dbConnResponse.HiddenConnections[i];
                    string tmpIdClass = "TmpId_" + item.ConnectionId;
                    string html = ASPdb.FrameworkUI.JsStr.StrFormat3(@"
                            <div class='ConnectionItem'>
                                <div class='NameLabel'>{0}</div>
                                <div class='BttnsHolder'>
                                    <a class='Bttn_ModifyConnection {1}' href='#00-ConnectionProperties-{2}'>Edit Connection</a>
                                </div>
                                <div class='clear'></div>
                            </div>", item.ConnectionName, tmpIdClass, item.ConnectionId.As<string>());
                    div2.append(html);
                }
                jF2(".SectionDiv_NoConnections").hide();
                jF2(".SectionDiv_Hidden").show();
            }

        }
        //----------------------------------------------------------------------------------------------------
        public void BttnAdd_Click()
        {
            //this.OpenConnectionPropertiesUI(-1);
        }



        //----------------------------------------------------------------------------------------------------
        public void OnClose_ConnectionPropertiesUI()
        {
            this.Open();
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
                .ConnectionsMainUI { }
                .ConnectionsMainUI .FrontLevelPage { display:block; }
                .ConnectionsMainUI .Head { position:relative; }
                .ConnectionsMainUI .Head .BttnAdd { position:absolute; display:block; top: 2px; right: 0px; width: auto; font-size: .65em; cursor: pointer;
                                                    background: #14498f; color: #fff; line-height:2.375em; padding: 0px 1.5em; border-radius: 0em; }
                .ConnectionsMainUI .Head .BttnAdd:hover { background: #222; }

                .ConnectionsMainUI .Main { }
                .ConnectionsMainUI .Main .LicenseInfo { display:block; border: 1px solid #afafaf; background: #e8e8e8; color: #6a6e73; 
                                                        padding: .25em 1em .25em 1em; line-height: 1.3em; font-size: .8em; }
                .ConnectionsMainUI .Main .FeedbackInfo { display:block; border: 1px solid #d3d3d3; background: #f9f9f9; color: #6a6e73; 
                                                         padding: .25em 1em .25em 1em; line-height: 1.3em; font-size: .8em; 
                                                         border-top-width: 0px; }
                .ConnectionsMainUI .Main .LicenseInfo:hover { background: #6a6e73; border-color: #6a6e73; color: #fff; }
                .ConnectionsMainUI .Main .FeedbackInfo:hover { background: #6a6e73; border-color: #6a6e73; color: #fff; }

                .ConnectionsMainUI .Main .SubHead { background: #5b6776; color: #fff; line-height: 1.75em; padding-left: 1.25em;
                                                    margin: 2.5em 0em 1.25em; }
                .ConnectionsMainUI .Main .SubHead span { }
                .ConnectionsMainUI .Main .ItemsListHolder { margin-left: 3.125em; border-top: 1px solid #d3d3d3; }

                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem { position:relative; line-height: 2.375em; border-bottom: 1px solid #d3d3d3; background: #e8e8e8; }
                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem .NameLabel { padding-left: 1.5em; white-space:nowrap; }
                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem .BttnsHolder { position:absolute; top: 0px; right: 0px; }
                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem .BttnsHolder a { float:right; cursor:pointer; color: #00f; background: #e8e8e8; font-size: .8em; padding: 0em 2em; }
                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem .BttnsHolder a:hover { background: #585858; color: #fff; }
                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem .BttnsHolder .Bttn_ModifyConnection { }
                .ConnectionsMainUI .Main .ItemsListHolder .ConnectionItem .BttnsHolder .Bttn_ManageAssets { }

                .ConnectionsMainUI .SectionDiv_Active { display:none; }
                .ConnectionsMainUI .SectionDiv_Hidden { display:none; }
                .ConnectionsMainUI .SectionDiv_NoConnections { display:none; margin: 1.5em 0em 0em 1em; line-height: 1.8125em; }
                .ConnectionsMainUI .SectionDiv_NoConnections span { background: #eee; }
                
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='FrontLevelPage'>
                    <div class='Head'>
                        Database Connections
                        <a class='BttnAdd' href='#00-ConnectionProperties-New'>+ Add Connection</a>
                    </div>
                    <div class='Main'>
                        <a href='#00-Subscription' class='LicenseInfo'>
                            Free accounts are allowed 1 connection. Paid accounts can have unlimited connections.<br /> 
                            Check your subscription settings (click here).
                        </a>
                        <div class='hide'>
                        <a href='#00-SendFeedback' class='FeedbackInfo'>
                            Supported database connections include: Microsoft SQL Server, SQL Server Azure.<br />
                            Click Here to provide feedback on what other database types you would like to see in future versions.
                        </a>
                        </div>
                        
                        <div class='AutoResize'>
                            <div class='SubHead SectionDiv_Active'>
                                Active Connections
                                <span class='Span_ActiveConnections_Count'>(0)</span>
                            </div>
                            <div class='ItemsListHolder ActiveItemsDiv SectionDiv_Active'>
                            </div>

                            <div class='SubHead SectionDiv_Hidden'>
                                Hidden Connections
                                <span class='Span_HiddenConnections_Count'>(0)</span>
                            </div>
                            <div class='ItemsListHolder HiddenItemsDiv SectionDiv_Hidden'>
                            </div>

                            <div class='SectionDiv_NoConnections'>
                                There are currently 0 database connections.<br />
                                Click the <span>[+ Add Connection]</span> button above to create a new database connections.
                            </div>
                        </div>

                    </div>
                </div>
            ";
        }
    }
}