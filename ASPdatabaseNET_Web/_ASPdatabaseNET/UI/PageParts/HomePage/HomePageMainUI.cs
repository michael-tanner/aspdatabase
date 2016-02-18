using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.HomePage.Objs;

namespace ASPdatabaseNET.UI.PageParts.HomePage
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class HomePageMainUI : MRBPattern<HomePageInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public HomePageMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='HomePageMainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            UI.PagesFramework.BasePage.WindowResized();

            if (jF(".S_Welcome").html() == "Welcome")
                AjaxService.ASPdatabaseService.New(this, "GetInfo_Return").HomePage__GetInfo();
        }
        public void GetInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null)
                return;

            this.Model = ajaxResponse.ReturnObj.As<HomePageInfo>();
            jF(".S_Welcome").html("Welcome " + this.Model.FirstName);
            jF(".Column2").html(this.Model.HomeHTML);
            this.BindUI();

            if(this.Model.UserIsAdmin)
            {
                jF(".Box2a").show();
                jF(".Box2b").hide();
            }
            else
            {
                jF(".Box2a").hide();
                jF(".Box2b").show();
                jF(".Column2").html("");
            }
        }



        //----------------------------------------------------------------------------------------------------
        public void Feedback_Click()
        {
            window.location = "ASPdatabase.NET.aspx#00-SendFeedback".As<Location>();
        }
        public void About_Click()
        {
            window.location = "ASPdatabase.NET.aspx#00-About-ASPdatabase.NET".As<Location>();
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
                .HomePageMainUI { padding: 3.0625em 0em 0em 3.0625em; }
                .HomePageMainUI .Column1 { float:left; width: 18.75em; margin-right: 0.3125em; }
                .HomePageMainUI .Column2 { float:left; width: 22.5em; background: #f5f5f5; border-top: 0.6875em solid #e0e0e0; border-top-right-radius:4em; }

                .HomePageMainUI .Box1 { background: #2568c2; color: #fff; line-height: 6.875em; margin-bottom: 0.3125em; text-align:center; border-top-left-radius:2em; }
                .HomePageMainUI .Box1 span { font-size: 1.2em; }

                .HomePageMainUI .Box2 { background: #f96d49; color: #fff; line-height: 2.2375em; margin-bottom: 0.3125em; padding: 1.2em 0em 1.2em 1.1em; }
                .HomePageMainUI .Box2:hover { background: #e84a22; }                
                .HomePageMainUI .Box2 span { font-size: .8em; }
                .HomePageMainUI .Box2b { display:none; }

                .HomePageMainUI .Box3 { font-size: .7em; color: #555; background: #e4e4e4; line-height: 2em; margin-bottom: 0.3125em; text-align:center; cursor:pointer; }
                .HomePageMainUI .Box3:hover { background: #555; color: #fff; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Column1'>
                    <div class='Box1'>
                        <span class='S_Welcome'>Welcome</span>
                    </div>
                    <div class='Box2 Box2a' On_Click='Feedback_Click'>
                        <span On_Click='Feedback_Click'>
                        How can we improve ASPdatabase.NET? <br />
                        Send your Feedback & Questions >>
                        </span>
                    </div>
                    <div class='Box2 Box2b'>
                        <span>
                            &nbsp;
                        </span>
                    </div>
                    <div class='Box3' On_Click='About_Click'>
                        ASPdatabase.NET <span class='Copy' ModelKey='Version'></span>
                        <div class='Copy' ModelKey='CopyrightLine'></div>
                    </div>
                </div>
                <div class='Column2 AutoResize'>
                    &nbsp;
                </div>
            ";
        }
    }
}