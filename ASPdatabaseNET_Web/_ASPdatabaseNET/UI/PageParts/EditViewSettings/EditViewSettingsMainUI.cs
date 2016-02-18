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
    public class EditViewSettingsMainUI : MRBPattern<string, string>
    {
        public JsEvent_BeforeAfter OnSave = new JsEvent_BeforeAfter();

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public EditViewSettingsMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ViewSettingsMainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();

            int ordinalPosition = 1;
            int displayOrder = 1;

            var table = jF2(".Table3");
            for (int i = 0; i < 3; i++)
            {
                var columnRow = new ViewSettings_ColumnRow();
                if (i < 2)
                    columnRow.IsPK = true;
                columnRow.OrdinalPosition = ordinalPosition++;
                columnRow.DisplayOrder = displayOrder++;
                columnRow.Instantiate();
                table.append(columnRow.jRoot);
            }
            var sectionRow = new ViewSettings_SectionRow();
            sectionRow.DisplayOrder = displayOrder++;
            sectionRow.Instantiate();
            table.append(sectionRow.jRoot);
            for (int i = 0; i < 6; i++)
            {
                var columnRow = new ViewSettings_ColumnRow();
                columnRow.OrdinalPosition = ordinalPosition++;
                columnRow.DisplayOrder = displayOrder++;
                columnRow.Instantiate();
                table.append(columnRow.jRoot);
            }
            sectionRow = new ViewSettings_SectionRow();
            sectionRow.DisplayOrder = displayOrder++;
            sectionRow.Instantiate();
            table.append(sectionRow.jRoot);
            for (int i = 0; i < 4; i++)
            {
                var columnRow = new ViewSettings_ColumnRow();
                columnRow.OrdinalPosition = ordinalPosition++;
                columnRow.DisplayOrder = displayOrder++;
                columnRow.Instantiate();
                table.append(columnRow.jRoot);
            }

        }

        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_Ok()
        {
            this.Close();
        }
        public void BttnClick_Cancel()
        {
            this.Close();
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += ViewSettings_ColumnRow.GetCssTree();
            rtn += ViewSettings_SectionRow.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            string width1 = "946px";

            return @"
                .ViewSettingsMainUI { position: relative; }
                .ViewSettingsMainUI .Head { width: " + width1 + @"; color: #173a67; font-size: 1.6em; margin-bottom: 35px; }
                .ViewSettingsMainUI .BttnsBar { position: absolute; top: 0px; width: " + width1 + @"; }
                .ViewSettingsMainUI .BttnsBar .Bttn { float: right; background: #14498f; color: #fff; line-height: 40px; 
                                                      padding: 0px 25px; cursor: pointer; margin-left: 5px; }
                .ViewSettingsMainUI .BttnsBar .Bttn:hover { background: #333; }
                .ViewSettingsMainUI .BttnsBar .Inactive { background: #ddd; cursor: default; }
                .ViewSettingsMainUI .BttnsBar .Inactive:hover { background: #ddd; }

                .ViewSettingsMainUI .Body1 { width: " + width1 + @"; }
                .ViewSettingsMainUI .Body1 .NameBox { float: left; width: 351px; }
                .ViewSettingsMainUI .Body1 .NameBox .NameBoxItem { display: block; }
                .ViewSettingsMainUI .Body1 .NameBox .Label1 { font-size: .7em; margin-bottom: 3px; }
                .ViewSettingsMainUI .Body1 .NameBox input { border: 1px solid #14498f; line-height: 25px; width: 351px; }

                .ViewSettingsMainUI .Body1 .SubBttnsBox { float: right; width: 402px; margin-top: 16px; }
                .ViewSettingsMainUI .Body1 .SubBttnsBox .Bttn2 { float: right; background: #14498f; color: #fff; line-height: 31px; text-align: center; 
                                                                 width: 196px; cursor: pointer; margin-left: 5px; font-size: .75em; }
                .ViewSettingsMainUI .Body1 .SubBttnsBox .Bttn2:hover { background: #333; }
                .ViewSettingsMainUI .Body1 .SubBttnsBox .Bttn2_1st { margin-left: 0px; }

                .ViewSettingsMainUI .Body1 .Body2 { margin: 30px 0px 0px 20px; }

                .ViewSettingsMainUI .Body1 .Body2 .Table1 { width: 100%; margin-bottom: 25px; }
                .ViewSettingsMainUI .Body1 .Body2 .Table1 .td1 { line-height: 31px; width: 180px; text-align: center; cursor: pointer; 
                                                                 background: #14498f; color: #fff; font-size: .85em; }
                .ViewSettingsMainUI .Body1 .Body2 .Table1 .td1:hover { background: #333; }
                .ViewSettingsMainUI .Body1 .Body2 .Table1 .td2 { line-height: 29px; font-size: .75em; background: #eee; 
                                                                 padding: 0px 0px 0px 12px;
                                                                 border: 1px dashed #bbb; border-left-width: 0px; }

                .ViewSettingsMainUI .Table3 { width: 100%; font-size: .8em; }
                .ViewSettingsMainUI .Table3 th { border-bottom: 2px solid #12325d; color: #6d6d6d; font-weight: bold; 
                                                 vertical-align: middle; text-align: center; padding-bottom: 2px; }
                .ViewSettingsMainUI .Table3 .th1 { width: 95px; font-size: .8em; }
                .ViewSettingsMainUI .Table3 .th2 { width: 35px; }
                .ViewSettingsMainUI .Table3 .th3 { width: 60px; font-size: .8em; }
                .ViewSettingsMainUI .Table3 .th4 { }
                .ViewSettingsMainUI .Table3 .th5 { width: 50px; }
                .ViewSettingsMainUI .Table3 .th6 { width: 40px; }
                .ViewSettingsMainUI .Table3 .th7 { width: 100px; }
                .ViewSettingsMainUI .Table3 .th8 { width: 140px; }
                .ViewSettingsMainUI .Table3 .th9 { width: 140px; font-size: .8em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='BttnsBar'>
                    <div class='Bttn' On_Click='BttnClick_Cancel'>Cancel</div>
                    <div class='Bttn' On_Click='BttnClick_Ok'>Ok</div>
                    <div class='clear'></div>
                </div>
                <div class='Head'>Edit View Settings</div>
                <div class='Body1'>
                    <div class='NameBox'>
                        <div class='Label1 NameBoxItem'>App View Name</div>
                        <input class='NameBoxItem Txt_NameBox' type='text' />
                    </div>
                    <div class='SubBttnsBox'>
                        <div class='Bttn2'>See All Implied Permissions</div>
                        <div class='Bttn2 Bttn2_1st'>Global Permissions</div>
                        <div class='clear'></div>
                    </div>
                    <div class='clear'></div>

                    <div class='Body2'>
                        <table class='Table1'>
                            <tr>
                                <td class='td1'>+ Column Section</td>
                                <td class='td2'>
                                    Use these sections to group & organize columns.&nbsp; 
                                    Permissions can be applied separately to each column section.
                                </td>
                            </tr>
                        </table>

                        <table class='Table3'>
                            <tr>
                                <th class='th1'>Display<br />Order</th>
                                <th class='th2'></th>
                                <th class='th3'>Ordinal<br />Position</th>
                                <th class='th4'>Column Name</th>
                                <th class='th5'></th>
                                <th class='th6'>Hide</th>
                                <th class='th7'>Sort<br /></th>
                                <th class='th8'>Filter</th>
                                <th class='th9'>Display<br />Settings</th>
                            </tr>
                        </table>
                    </div>
                </div>
            ";
        }
    }
}