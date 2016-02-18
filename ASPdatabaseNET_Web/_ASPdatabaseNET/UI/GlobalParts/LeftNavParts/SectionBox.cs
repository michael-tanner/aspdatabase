using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DataObjects.Nav;

namespace ASPdatabaseNET.UI.GlobalParts.LeftNavParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class SectionBox : MRBPattern<NavSectionInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public SectionBox()
        {
            this.Instantiate();
        }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='SectionBox jRoot'>");
            jRoot.append(this.GetHtmlRoot());

            if (this.Model != null && this.Model.IsOpen)
                jF2(".SectionLinks").show();

            var holder = jF2(".SectionLinks").html("");
            if(this.Model != null && this.Model.Items != null)
            {
                string previousSchema = "";
                int schemasCount = 0;
                for (int i = 0; i < this.Model.Items.Length; i++)
                    if (previousSchema != this.Model.Items[i].Schema)
                    {
                        previousSchema = this.Model.Items[i].Schema;
                        schemasCount++;
                    }
                previousSchema = "";
                for (int i = 0; i < this.Model.Items.Length; i++)
                {
                    var itemInfo = this.Model.Items[i];
                    if (schemasCount > 1)
                        if (previousSchema != itemInfo.Schema)
                            holder.append("<div class='Schema' title='Schema: [" + itemInfo.Schema + "]'>" + itemInfo.Schema + "</div>");
                    previousSchema = itemInfo.Schema;
                    holder.append(JsStr.StrFormat2("<a href='{0}'>{1}</a>", itemInfo.URL, itemInfo.Name));
                }
            }
            this.BindUI();
        }


        //----------------------------------------------------------------------------------------------------
        public void SectionNameBttn_Click()
        {
            if (this.Model.IsOpen)
                jF2(".SectionLinks").hide();
            else
                jF2(".SectionLinks").show();
            this.Model.IsOpen = !this.Model.IsOpen;
            this.OnChange.After.Fire();
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
            .SectionBox { }
            .SectionBox .SectionNameBttn { line-height: 1.52777em; padding-left: 0.83333em; background: #eb640a; color: #fff; margin-bottom: 1px; cursor: pointer; border-top-left-radius:.35em; }
            .SectionBox .SectionNameBttn:hover { background: #111 }
            .SectionBox .SectionLinks { display: none; font-size: 1em; border-left:1px solid #d9d9d9; }
            .SectionBox .SectionLinks .Schema { border-bottom: 1px solid #c1c1c1; color: #aaa; line-height: 1.45833em; padding-left: 2px; }
            .SectionBox .SectionLinks a { display: block; border-bottom: 1px solid #c1c1c1; color: blue; line-height: 1.45833em; padding-left: 2px; }
            .SectionBox .SectionLinks a:hover { background: #fff; color: #000; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='SectionNameBttn' On_Click='SectionNameBttn_Click' ModelKey='SectionName'></div>
            <div class='SectionLinks'>
            </div>
            ";
        }
    }
}