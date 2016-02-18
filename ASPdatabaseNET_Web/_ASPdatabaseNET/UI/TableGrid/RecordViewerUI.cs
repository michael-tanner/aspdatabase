using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.TableGrid.Objs;

namespace ASPdatabaseNET.UI.TableGrid
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class RecordViewerUI : MRBPattern<GridRow, string>
    {
        public GridResponse GridResponse;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public RecordViewerUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='RecordViewerUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jF(".Txt_TableName").html(this.GridResponse.Schema + "." + this.GridResponse.TableName);
            jF(".NavigatorText").html((this.Model.DisplayIndex + 1).As<string>());

            if(this.Model.DisplayIndex > 0)
            {
                jF(".Arrow_Left_Off").hide();
                jF(".Arrow_Left").show();
            }
            else
            {
                jF(".Arrow_Left").hide();
                jF(".Arrow_Left_Off").show();
            }

            if(this.Model.DisplayIndex < this.GridResponse.Rows.Length - 1)
            {
                jF(".Arrow_Right_Off").hide();
                jF(".Arrow_Right").show();
            }
            else
            {
                jF(".Arrow_Right").hide();
                jF(".Arrow_Right_Off").show();
            }


            var holder_FieldUIs = jF(".Holder_FieldUIs").html("");
            for (int i = 0; i < this.Model.Values.Length; i++)
            {
                string fieldName = this.GridResponse.HeaderItems[i].FieldName;
                string value = this.Model.Values[i];

                holder_FieldUIs.append(st.New(@"
                <tr>
                    <td class='NameTd'><div class='NameDiv'>{0}</div></td>
                    <td class='ValueTd'>
                        <div class='ValueDiv'>{1}</div>
                    </td>
                </tr>
                ").Format2(fieldName, value).TheString);
            }
            UI.PagesFramework.BasePage.WindowResized();
        }

        




        //------------------------------------------------------------------------------------------ Events --
        public void RecordsList_Click()
        {
            this.Close();
        }
        public void Left_Click()
        {
            this.Model = this.GridResponse.Rows[this.Model.DisplayIndex - 1];
            this.Open();
        }
        public void Right_Click()
        {
            this.Model = this.GridResponse.Rows[this.Model.DisplayIndex + 1];
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
                .RecordViewerUI { paddingz: 0.75em 0em 0em 0.75em; width: 55em; max-width: 55em; }
                .RecordViewerUI .Txt_TableName { font-size: 1.2em; padding-bottom: 0.625em; color: #eb640a; }
                .RecordViewerUI .BttnBar { background: #093a79; color: #fff; line-height: 2.1875em; border-bottom: 0.375em solid #fff; }
                .RecordViewerUI .BttnBar .Box1 { float:left; width: 15em; }
                .RecordViewerUI .BttnBar .Box2 { float:left; width: 13em; }
                .RecordViewerUI .BttnBar .Box3 { float:right; width: 25em; }
                .RecordViewerUI .BttnBar .Bttn { float:right; cursor:pointer; background: #093a79; color: #fff; padding: 0em .75em; }
                .RecordViewerUI .BttnBar .Bttn:hover { background: #001d44; }
                .RecordViewerUI .BttnBar .Bttn_RecordsList { float:left; }
                .RecordViewerUI .BttnBar .Bttn_Off { float:right; background: #093a79; color: #fff; padding: 0em .75em; }

                .RecordViewerUI .BttnBar .Navigator { }
                .RecordViewerUI .BttnBar .Navigator .Arrow_Off { float:left; text-align:center; width: 2.1875em; background: #7490b4; }
                .RecordViewerUI .BttnBar .Navigator .Arrow { display:none; float:left; text-align:center; width: 2.1875em; background: #7490b4; color: #ececec; cursor:pointer; }
                .RecordViewerUI .BttnBar .Navigator .Arrow:hover { background: #3f679b; }
                .RecordViewerUI .BttnBar .Navigator .NavigatorText { float:left; text-align:center; background: #ececec; color: #093a79; font-size: 1.25em; padding: 0em 1em; width: 1em; }

                .RecordViewerUI .BttnBar .Box3_StandardBttns { }
                .RecordViewerUI .BttnBar .Box3_SaveBttns { display:none; }

                .RecordViewerUI .Holder_FieldUIs { width: 100%; }
                .RecordViewerUI .Holder_FieldUIs tr { line-height: 1.875em; border-bottom: 0.125em solid #fff; background: #f4f4f4; }
                .RecordViewerUI .Holder_FieldUIs tr .NameTd { background: #eee; font-size: .8em; line-height: 2.34375em; width: 18em; max-width: 18em; min-width: 18em; overflow:hidden; }
                .RecordViewerUI .Holder_FieldUIs tr .ValueTd { font-size: .8em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Txt_TableName'></div>
                <div class='BttnBar'>
                    <div class='Box1'>
                        <div class='Bttn Bttn_RecordsList' On_Click='RecordsList_Click'>&lt;&lt; Records List</div>
                    </div>
                    <div class='Box2 HideIfNoPK'>
                        <div class='Navigator NoSelect'>
                            <div class='Arrow Arrow_Left' On_Click='Left_Click'>&lt;</div>
                            <div class='Arrow_Off Arrow_Left_Off'>&nbsp;</div>
                            <div class='NavigatorText'>&nbsp;</div>
                            <div class='Arrow Arrow_Right' On_Click='Right_Click'>&gt;</div>
                            <div class='Arrow_Off Arrow_Right_Off' title='End of current set'>?</div>
                        </div>
                    </div>
                    <div class='Box3 Box3_StandardBttns HideIfNoPK'>
                        <div class='Bttn_Off'>(Read-Only View)</div>
                        <div class='clear'></div>
                    </div>
                    <div class='clear'></div>
                </div>

                <div class='AutoResize'>
                    <table class='Holder_FieldUIs'>
                    </table>
                </div>
            ";
        }
    }
}
