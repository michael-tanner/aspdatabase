using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.EditViewSettings
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ViewSettings_SectionRow : MRBPattern<string, string>
    {
        public int DisplayOrder = -1;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ViewSettings_SectionRow()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='ViewSettings_SectionRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();

            jF2(".td1").html(":: &nbsp; <span>" + this.DisplayOrder.As<JsString>() + "</span> &nbsp; ↑ &nbsp; ↓");
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
                .ViewSettings_SectionRow { line-height: 25px; }
                .ViewSettings_SectionRow td { background: #3e74bb; color: #fff; border-top: 3px solid #12325d; vertical-align: middle; }
                .ViewSettings_SectionRow .td1 { }
                .ViewSettings_SectionRow .td2 { }

                .ViewSettings_SectionRow .td1 span { color: #89acda; }

                .ViewSettings_SectionRow .Label1 { float: right; width: 145px; padding-right: 15px; 
                                                   text-align: center; line-height: 50px; font-size: 1.2em; }
                .ViewSettings_SectionRow .Div_SectionName { float: right; width: 295px; margin-bottom: 3px; }
                .ViewSettings_SectionRow .Div_SectionName .Label2 { font-size: .8em; line-height: 17px; }
                .ViewSettings_SectionRow .Div_SectionName .Txt_SectionName { width: 260px; line-height: 15px; font-size: 1.2em; }

                .ViewSettings_SectionRow .Div_CheckboxOptions { float: right; width: 165px; font-size: .8em; line-height: 12px; margin: 2px 0px 4px; }
                .ViewSettings_SectionRow .Div_CheckboxOptions div { }

                .ViewSettings_SectionRow .Bttn { float: right; background: #14498f; cursor: pointer; text-align: center; 
                                                 font-size: .9em; padding: 8px 17px; margin: 4px 5px 0px 0px; }
                .ViewSettings_SectionRow .Bttn:hover { background: #333; }
                .ViewSettings_SectionRow .Bttn_Permissions { }
                .ViewSettings_SectionRow .Bttn_Delete { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='td1'>::</td>
                
                <td class='td2' colspan='8'>
                    <div class='Bttn Bttn_Delete'>X Delete</div>
                    <div class='Bttn Bttn_Permissions'>Permissions</div>
                    <div class='Div_CheckboxOptions'>
                        <div><input type='checkbox' /> Hide on Grid</div>
                        <div><input type='checkbox' /> Hide on Record Editor</div>
                        <div title='Collapsed by Default in Record Editor'><input type='checkbox' /> Collapsed by Default</div>
                    </div>
                    <div class='Div_SectionName'>
                        <div class='Label2'>Section Name</div>
                        <input type='text' class='Txt_SectionName' />
                    </div>
                    <div class='Label1'>Column Section</div>
                    <div class='clear'></div>
                </td>
            ";
        }
    }
}