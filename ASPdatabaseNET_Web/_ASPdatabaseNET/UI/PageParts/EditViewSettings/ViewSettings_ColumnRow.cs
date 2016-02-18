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
    public class ViewSettings_ColumnRow : MRBPattern<string, string>
    {
        public bool IsPK = false;
        public int OrdinalPosition = -1;
        public int DisplayOrder = -1;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ViewSettings_ColumnRow()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='ViewSettings_ColumnRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();


            jF2(".td1").html(":: &nbsp; <span>" + this.DisplayOrder.As<JsString>() + "</span> &nbsp; ↑ &nbsp; ↓");

            if (this.IsPK)
                jF2(".PK").show();

            jF2(".td3").html(this.OrdinalPosition.As<JsString>());

            if (OrdinalPosition == 2)
            {
                jF2(".Txt_Sort").val("1");
                jF2(".SortArrow").html("↑");
                jF2(".SortArrow").addClass("SortArrow_On");
            }

        }

        //------------------------------------------------------------------------------------------ Events --
        public void TdClick_SetAltName()
        {
            jF2(".Txt_AltColumnName").show();
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
                .ViewSettings_ColumnRow { line-height: 25px; }
                .ViewSettings_ColumnRow td { border-bottom: 1px solid #d6d6d6; }
                .ViewSettings_ColumnRow .td1 { }
                .ViewSettings_ColumnRow .td2 { text-align: center; }
                .ViewSettings_ColumnRow .td3 { text-align: center; }
                .ViewSettings_ColumnRow .td4 { }
                .ViewSettings_ColumnRow .td5 { text-align: center; font-size: .7em; line-height: 12px; color: #7996cf; cursor: pointer; }
                .ViewSettings_ColumnRow .td6 { text-align: center; }
                .ViewSettings_ColumnRow .td7 { text-align: left; padding-left: 21px; }
                .ViewSettings_ColumnRow .td8 { }
                .ViewSettings_ColumnRow .td9 { }

                .ViewSettings_ColumnRow .td1 span { color: #bbb; }

                .ViewSettings_ColumnRow .PK { display:none; width: 25px; border: 1px solid #7fa7dd; background: #d6e1f0;
                                              line-height: 17px; margin: 3px 0px 0px 5px; font-size: .9em; }
                .ViewSettings_ColumnRow .Txt_AltColumnName { display:none; width: 258px; border: 1px solid #555; }
                .ViewSettings_ColumnRow .td5:hover { color: #3e5b92; background: #dbe3f3; }
                .ViewSettings_ColumnRow .Checkbox_Hide { }
                .ViewSettings_ColumnRow .Txt_Sort { width: 25px; border: 1px solid #85a3cb; line-height: 12px; height: 12px;
                                                    margin-top: 3px; text-align: center; font-size: .85em; }
                .ViewSettings_ColumnRow .SortArrow { padding: 0px 8px 2px; margin: 0px 7px; font-size: 1.2em; }
                .ViewSettings_ColumnRow .SortArrow_On:hover { background: #666; color: #fff; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='td1'>::</td>
                <td class='td2'>
                    <div class='PK'>PK</div>
                </td>
                <td class='td3'></td>
                <td class='td4'>
                    <div>Column Name 001</div>
                    <input type='text' class='Txt_AltColumnName' />
                </td>
                <td class='td5' On_Click='TdClick_SetAltName'>Set Alt<br />Name</td>
                <td class='td6'><input type='checkbox' class='Checkbox_Hide' /></td>
                <td class='td7'><input type='text' class='Txt_Sort' /><span class='SortArrow'>&nbsp;</span></td>
                <td class='td8'></td>
                <td class='td9'></td>
            ";
        }
    }
}