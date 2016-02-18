using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.GlobalParts.Objs;

namespace ASPdatabaseNET.UI.GlobalParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class LogoBox : MRBPattern<LogoBoxModel, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public LogoBox()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='LogoBox jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            AjaxService.ASPdatabaseService.New(this, "Return").Global__GetLogoBoxModel();
        }
        public void Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<LogoBoxModel>();

            switch(this.Model.CustomLogoType)
            {
                case LogoBoxModel.CustomLogoTypes.None:
                    break;
                case LogoBoxModel.CustomLogoTypes.Image:
                    jF(".LogoLink").hide();
                    jF(".CustomLogoBox").show();
                    jF(".LogoImg")[0].style.display = "block";
                    jF(".LogoImg")[0].style.background = st.New("url('{0}') no-repeat").Format1(this.Model.LogoURL).TheString;
                    jF(".LogoImg").attr("title", this.Model.LogoText);
                    break;
                case LogoBoxModel.CustomLogoTypes.Text:
                    jF(".LogoLink").hide();
                    jF(".CustomLogoBox").show();
                    jF(".LogoTxt")[0].style.display = "block";
                    jF(".LogoTxt").html(this.Model.LogoText).attr("title", this.Model.LogoText);
                    break;
            }
        }






        //------------------------------------------------------------------------------------------ Events --






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
                .LogoBox { }
                .LogoBox .LogoLink { float: left; display:block; width: auto; line-height: 40px; margin-top: 18px; 
                                     margin-left: 15px; padding: 0px 14px; font-size: 1.25em; color:#fff; }
                .LogoBox .LogoLink:hover { background: #0f366b; }

                .LogoBox .CustomLogoBox { display:none; float:left; }
                .LogoBox .CustomLogoBox a { color: #fff; }
                .LogoBox .CustomLogoBox a:hover { background: #0f366b; }
                .LogoBox .CustomLogoBox .LogoImg { display:none; height: 40px; width: 300px; background: url('') no-repeat;
                                                   margin: 0.5625em 0em 0em 1.4375em; }
                .LogoBox .CustomLogoBox .LogoTxt { display:none; line-height: 40px; max-width: 400px; font-size: 1.4em; margin-left: .55em; padding-left: .55em; padding-top: .1em; overflow:hidden; white-space:nowrap }
                .LogoBox .CustomLogoBox .ByASPdatabaseNET { display:block; width: 17.08em; font-size: .75em; line-height: 2em; margin-left: 1.916em; padding-left: 1em; background: #14498f; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <a href='http://www.aspdatabase.net/' target='_blank' class='LogoLink'>ASPdatabase.NET</a>

                <div class='CustomLogoBox'>
                    <a class='LogoImg' href='ASPdatabase.NET.aspx'>&nbsp;</a>
                    <a class='LogoTxt' href='ASPdatabase.NET.aspx'>Logo Text</a>
                    <a class='ByASPdatabaseNET' href='https://www.aspdatabase.net/' target='_blank'>powered by ASPdatabase.NET</a>
                </div>
            ";
        }
    }
}
