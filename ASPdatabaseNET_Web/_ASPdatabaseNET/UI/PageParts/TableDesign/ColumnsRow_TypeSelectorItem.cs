using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ColumnsRow_TypeSelectorItem : MRBPattern<string, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ColumnsRow_TypeSelectorItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ColumnsRow_TypeSelectorItem jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();

            if (this.Model != "--")
                jF(".ItemTS").html(this.Model);
            else
            {
                jF(".ItemTS").hide();
                jF(".Divider").show();
            }

        }






        //------------------------------------------------------------------------------------------ Events --
        public void ItemTS_Click()
        {
            this.OnChange.After.Fire1(this.Model);
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
                .ColumnsRow_TypeSelectorItem { }
                .ColumnsRow_TypeSelectorItem .ItemTS { font-size: .8em; padding-left: .65em; cursor: pointer; }
                .ColumnsRow_TypeSelectorItem .ItemTS:hover { background: #397bd2; color: #fff; }
                .ColumnsRow_TypeSelectorItem .Divider { display:none; background: #dedede; line-height: .8em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='ItemTS' On_Click='ItemTS_Click'></div>
                <div class='Divider'>&nbsp;</div>
            ";
        }
    }
}
