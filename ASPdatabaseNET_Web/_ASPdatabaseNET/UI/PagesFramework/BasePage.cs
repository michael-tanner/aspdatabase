using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PagesFramework
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Global, ASPdatabaseNET.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ASPdatabaseNET_Global : jQueryContext
    {
        //----------------------------------------------------------------------------------------------------
        public static void ASPdatabaseNET_Start(string ready)
        {
            eval(" $(document).ready(function(){ ASPdatabaseNET_Start__DocumentIsReady(); }); ");
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdatabaseNET_Start__DocumentIsReady()
        {
            J("body").append("<style>" + ASPdatabaseNET.UI.PagesFramework.BasePage.GetCssTree() + "</style>");

            var corePage = new ASPdatabaseNET.UI.PagesFramework.BasePage();
            corePage.Instantiate();
            J("body").append(corePage.jRoot);
            corePage.Open();
        }

        //----------------------------------------------------------------------------------------------------
        public static void ASPdatabaseNET_LoginPage(string ready)
        {
            eval(" $(document).ready(function(){ ASPdatabaseNET_LoginPage__DocumentIsReady(); }); ");
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdatabaseNET_LoginPage__DocumentIsReady()
        {
            J("body").append("<style>" + ASPdatabaseNET.UI.PagesFramework.BasePage.GetCssTree() + "</style>");

            var loginPage = new ASPdatabaseNET.UI.PageParts.Login.LoginMainUI();
            loginPage.Instantiate();
            J("body").append(loginPage.jRoot);
            loginPage.Open();
        }

        //----------------------------------------------------------------------------------------------------
        public static void ASPdatabaseNET_InstallPage(string ready)
        {
            eval(" $(document).ready(function(){ ASPdatabaseNET_InstallPage__DocumentIsReady(); }); ");
        }
        //----------------------------------------------------------------------------------------------------
        public static void ASPdatabaseNET_InstallPage__DocumentIsReady()
        {
            J("body").append("<style>" + ASPdatabaseNET.UI.PagesFramework.BasePage.GetCssTree() + "</style>");

            var installPage = new ASPdatabaseNET.UI.PageParts.Install.InstallMainUI();
            installPage.Instantiate();
            J("body").append(installPage.jRoot);
            installPage.Open();
        }

    }



    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class BasePage : MRBPattern<string, string>
    {
        public IPage SubPage;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public BasePage()
        {
            this.AttachThisToDocument();
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
        public void AttachThisToDocument()
        {
            var thisObj = this;
            eval("document.ASPdatabaseNET_BasePage = thisObj;");
        }
        //------------------------------------------------------------------------------------------ static --
        public static BasePage GetFromDocument()
        {
            BasePage rtn = null;
            try
            {
                eval("rtn = document.ASPdatabaseNET_BasePage;");
            }
            catch { rtn = null; }

            return rtn;
        }




        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='BasePage jRoot'>");
            jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        public void ConnectEvents_Sub()
        {
            var thisObj = this;
            eval(@"$(window).on('hashchange', function() { thisObj.OnWindow_HashChange(); });");
            eval("$(window).resize(function() { thisObj.WindowResized_InstancePassThru(); });");
        }

        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            BasePage.WindowResized();

            if (window.location.hash == "")
                window.location.hash = "#00-Home";

            PageIdentifier.GetFromUrlHash().Launch();

            // this needs to be called at least one time to setup KeyboardController:
            //var keyboardController = Keyboard.KeyboardController.GetFromDocument(); // turned off in ASPdatabase
        }

        //----------------------------------------------------------------------------------------------------
        public void LaunchPage(PageIdentifier pageId)
        {
            this.SubPage = pageId.CreateAndGetPageObj();
            jRoot.find(".PageContent").html("");
            jRoot.find(".PageContent").append(this.SubPage.Get_jRoot());
            this.SubPage.Open();
            BasePage.WindowResized();
        }

        
        //---------------------------------------------------------------------------------- Event Handlers --
        public bool IgnoreNext_HashChange = false;
        public void OnWindow_HashChange()
        {
            if(this.IgnoreNext_HashChange)
            {
                this.IgnoreNext_HashChange = false;
                return;
            }
            document.title = "ASPdatabase.NET";

            var newPageIdentifier = PageIdentifier.GetFromUrlHash();

            if (this.SubPage.PageId.PageType == newPageIdentifier.PageType)
                this.SubPage.PageId = newPageIdentifier;
            else
                newPageIdentifier.Launch();
        }
        //----------------------------------------------------------------------------------------------------
        public void WindowResized_InstancePassThru()
        {
            BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        public static void WindowResized()
        {
            int windowHeight = J(window).height().As<JsNumber>();
            int windowWidth = J(window).width().As<JsNumber>();

            var autoResizeItems = J(".AutoResize");
            for (int i = 0; i < autoResizeItems.length; i++)
            {
                var jAutoResize = J(autoResizeItems[i]);
                int height = windowHeight - jAutoResize.offset().top - Get_AutoResize_BottomSpace(jAutoResize);
                jAutoResize.height(height);
            }

            var autoResizeXYItems = J(".AutoResizeXY");
            for (int i = 0; i < autoResizeXYItems.length; i++)
            {
                var jAutoResize = J(autoResizeXYItems[i]);
                int height = windowHeight - jAutoResize.offset().top - Get_AutoResize_BottomSpace(jAutoResize);
                int width = windowWidth - jAutoResize.offset().left - Get_AutoResize_RightSpace(jAutoResize);
                jAutoResize.height(height);
                jAutoResize.width(width);
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static int Get_AutoResize_BottomSpace(jQuery jItem)
        {
            int rtn = 0;
            try
            {
                rtn = jItem.attr("AutoResize_BottomSpace").As<JsNumber>();
                if (rtn.As<object>() == null || (1 * rtn) < 1)
                    rtn = 0;
            }
            catch { rtn = 0; }

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static int Get_AutoResize_RightSpace(jQuery jItem)
        {
            int rtn = 0;
            try
            {
                rtn = jItem.attr("AutoResize_RightSpace").As<JsNumber>();
                if (rtn.As<object>() == null || (1 * rtn) < 1)
                    rtn = 0;
            }
            catch { rtn = 0; }

            return rtn;
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += PageParts.Login.LoginMainUI.GetCssTree();
            rtn += PageParts.Install.InstallMainUI.GetCssTree();
            rtn += GlobalParts.TopBar.GetCssTree();
            rtn += GlobalParts.LeftNav.GetCssTree();
            rtn += Pages.EverythingPage.GetCssTree();
            rtn += Pages.OtherSamplePage.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
            .AutoResize { overflow-x: hidden; overflow-y: auto; }
            .AutoResizeXY { overflow: auto; }

            .BasePage {  }


html, body, div, span, applet, object, iframe,
h1, h2, h3, h4, h5, h6, p, blockquote, pre,
a, abbr, acronym, address, big, cite, code,
del, dfn, em, img, ins, kbd, q, s, samp,
small, strike, strong, sub, sup, tt, var,
b, u, i, center,
dl, dt, dd, ol, ul, li,
fieldset, form, label, legend,
table, caption, tbody, tfoot, thead, tr, th, td,
article, aside, canvas, details, embed, 
figure, figcaption, footer, header, hgroup, 
menu, nav, output, ruby, section, summary,
time, mark, audio, video, input {
	margin: 0;
	padding: 0;
	border: 0;
	font-size: 100%;
	font-family: Verdana, Geneva, sans-serif;
	vertical-align: baseline;
}
/* HTML5 display-role reset for older browsers */
article, aside, details, figcaption, figure, 
footer, header, hgroup, menu, nav, section { display: block; }
ol, ul { list-style: none; }
blockquote, q { quotes: none; }
blockquote:before, blockquote:after, q:before, q:after { content: ''; content: none; }
table { border-collapse: collapse; border-spacing: 0; }
td { vertical-align: top; }
.clear{ clear: both; }
.hide { display: none; }
body { background: #ffffff; color: #222222; overflow: hidden; cursor: default; }
a { text-decoration: none; }
input[type=button] { padding: 5px 10px; }
input[type=text] { padding: 2px; }
input[type=button] {
  -webkit-appearance: none;
  -webkit-border-radius: 0;
}
ASPdatabaseNET { display: none; }
.Table100 { width: 100%; }
.NoSelect { 
-webkit-touch-callout: none;
-webkit-user-select: none;
-khtml-user-select: none;
-moz-user-select: none;
-ms-user-select: none;
user-select: none; }
.YesSelect { 
-webkit-touch-callout: initial;
-webkit-user-select: initial;
-khtml-user-select: initial;
-moz-user-select: initial;
-ms-user-select: initial;
user-select: initial; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='PageContent'>
            </div>
            ";
        }
    
    }
}
