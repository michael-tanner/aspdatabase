using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PagesFramework;

namespace ASPdatabaseNET.UI.Pages
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class OtherSamplePage : MRBPattern<string, string>, IPage
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public OtherSamplePage()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public new jQuery Get_jRoot() { return this.jRoot; }
        public string Get_HeaderColor() { return ""; }
        private PageIdentifier _pageId;
        public PageIdentifier PageId { get { return _pageId; } set { _pageId = value; } }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='OtherSamplePage jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            //this.Home_StarterBttns = new Nav_StarterBttns();
            //jRoot.find(".Td1").append(this.Home_StarterBttns.jRoot);
        }
        //----------------------------------------------------------------------------------------------------
        public void ConnectEvents_Sub()
        {
        }

        //---------------------------------------------------------------------------------- Event Handlers --



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
                .OtherSamplePage { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                OtherSamplePage
            ";
        }
    }
}