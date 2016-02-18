using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.TableGrid.Objs;
using ASPdatabaseNET.UI.PageParts.Record.Objs_History;

namespace ASPdatabaseNET.UI.PageParts.Record.HistoryUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class HistoryMainUI : MRBPattern<HistorySummary, string>
    {
        public UniqueRowKey UniqueRowKey;

        public HistoryMenuItem[] MenuItems;

        public HistoryRecord SubModel;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public HistoryMainUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if(this.UniqueRowKey == null || this.UniqueRowKey.TableType != GridRequest.TableTypes.Table)
            {
                alert("Invalid Row Key");
                this.Close();
                return;
            }
            jF(".Label2").html("Loading...");
            jF(".MenuTable").html("");

            jF(".EmptyPanel").show();
            jF(".Loading2").hide();
            jF(".CompareTbl").hide();
            jF(".FieldView").hide();

            int tableId = this.UniqueRowKey.Id;
            string[] keyValue = this.UniqueRowKey.Values;
            AjaxService.ASPdatabaseService.Bind(this).History__GetSummary(tableId, keyValue, 100);

            UI.PagesFramework.BasePage.WindowResized();
        }
        //----------------------------------------------------------------------------------------------------
        public void AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.SetModel(ajaxResponse);

            string keyString1 = "";
            string keyString2 = "";
            bool commanFound = false;
            for (int i = 0; i < this.Model.KeyValue.Length; i++) 
            {
                if(i > 0) 
                {
                    keyString1 += ", "; 
                    keyString2 += ", "; 
                }
                string item = this.Model.KeyValue[i];
                keyString1 += item;
                keyString2 += "\"" + item + "\"";
                if (st.New(item).Contains(",", false))
                    commanFound = true;
            }
            if (commanFound)
                keyString1 = keyString2;

            string label2 = st.New("[{0}].[{1}].[{2}] : {3}").Format4(this.Model.ConnectionName, this.Model.Schema, this.Model.TableName, keyString1).TheString;
            jF(".Label2").html(label2);

            var menuTable = jF(".MenuTable").html("");
            this.MenuItems = new HistoryMenuItem[0];
            for (int i = 0; i < this.Model.HistoryRecords.Length; i++)
            {
                this.MenuItems[i] = new HistoryMenuItem();
                this.MenuItems[i].Model = this.Model.HistoryRecords[i];
                this.MenuItems[i].Instantiate();
                this.MenuItems[i].OnChange.After.AddHandler(this, "HistoryMenuItem_Change", 1);
                menuTable.append(this.MenuItems[i].jRoot);
            }

            if (this.Model.HistoryRecords.Length < 1)
                jF(".EmptyPanel").html("This record does not have any saved history.");
            else
                jF(".EmptyPanel").html("(Select a Time Point)");

            UI.PagesFramework.BasePage.WindowResized();
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Div_BG_Click()
        {
            this.Close();
        }

        public void CloseBttn_Click()
        {
            this.Close();
        }
        //----------------------------------------------------------------------------------------------------
        public void HistoryMenuItem_Change(HistoryMenuItem menuItem)
        {
            for (int i = 0; i < this.MenuItems.Length; i++)
                this.MenuItems[i].Deselect();

            jF(".EmptyPanel").hide();
            jF(".Loading2").show();
            jF(".CompareTbl").hide();
            jF(".FieldView").hide();
            AjaxService.ASPdatabaseService.Bind2(this, "AjaxReturn2").History__GetRecord(menuItem.Model.TableId, menuItem.Model.HistoryId);
        }
        public void AjaxReturn2(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            jF(".EmptyPanel").hide();
            jF(".Loading2").hide();
            jF(".CompareTbl").show();
            jF(".FieldView").hide();
            this.SubModel = ajaxResponse.ReturnObj.As<HistoryRecord>();

            var compareTbl = jF(".CompareTbl");
            compareTbl.find(".TrRow").detach();
            for (int i = 0; i < this.SubModel.Fields_3ValArr.Length; i++)
            {
                var item = this.SubModel.Fields_3ValArr[i];

                string v1 = item.v1;
                string v2 = item.v2;
                string v3 = item.v3;
                if (v1 == null) v1 = "&nbsp;";
                if (v2 == null) v2 = "&nbsp;";
                if (v3 == null) v3 = "&nbsp;";

                string matchTDsClass = "";
                if (!item.match)
                    matchTDsClass = "Hightlight";

                string altClass = "TrRow1";
                if (i % 2 == 1)
                    altClass = "TrRow2";
                compareTbl.append(st.New(@"
                    <tr class='TrRow {0}' Index='{1}'>
                        <td><div>{2}</div></td>
                        <td class='{6}'><div>{3}</div></td>
                        <td class='{6}'><div>{4}</div></td>
                        <td><div>{5}</div></td>
                    </tr>").Format7(altClass, i, item.ColumnName_OrigCasing, v1, v2, v3, matchTDsClass).TheString);
            }
            var thisObj = this;
            eval(@" compareTbl.find('.TrRow').click(function(){ thisObj.FieldItem_Click( $(this).attr('Index') ); }); ");
        }
        //----------------------------------------------------------------------------------------------------
        public void FieldItem_Click(int index)
        {
            var item = this.SubModel.Fields_3ValArr[index];
            jF(".CompareTbl").hide();
            jF(".FieldView").show();
            jF(".Label_cn").html(item.ColumnName_OrigCasing);
            jF(".Label_v1").html(item.v1);
            jF(".Label_v2").html(item.v2);
            jF(".Label_v3").html(item.v3);

        }
        //----------------------------------------------------------------------------------------------------
        public void Label3_Click()
        {
            jF(".CompareTbl").show();
            jF(".FieldView").hide();
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            return
                HistoryMenuItem.GetCssTree() +
                GetCssRoot();
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
            .HistoryMainUI { }
            .HistoryMainUI .Div_BG { background: #464f5b; opacity: 0.65; position:fixed; width: 100%; height: 8000px; top: 0px; left: 0px; }
            .HistoryMainUI .InFrontBox { position:absolute; background: #fff; min-height: 35em; box-shadow: 0.125em 0.125em .2em #777; left: 0px; width: 100%; }
            .HistoryMainUI .InFrontBox .TopDiv { background: #e4e4e4; color: #444; line-height: 4em; margin-bottom: 0em; }
            .HistoryMainUI .InFrontBox .TopDiv .Label1 { float:left; padding: 0em 2em; border-right: 1px solid #fff; }
            .HistoryMainUI .InFrontBox .TopDiv .Label2 { float:left; padding: 0em 0em 0em 2em; }
            .HistoryMainUI .InFrontBox .TopDiv .CloseBttn { float:right; cursor:pointer; padding: 0em 2em; border-left: 1px solid #fff; }
            .HistoryMainUI .InFrontBox .TopDiv .CloseBttn:hover { background: #555; color: #fff; }

            .HistoryMainUI .InFrontBox .LabelSub { font-size: .7em; color: #777; padding-left: 2.9em; line-height: 2.3em; border-bottom: 1px solid #e4e4e4; margin-bottom: .7em; }

            .HistoryMainUI .InFrontBox .MainTbl1 { width: 100% }
            .HistoryMainUI .InFrontBox .MainTbl1 td { }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd1 { width: 25em; padding: 0em .5em; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd1 .MenuTable { width: 100%; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 { padding-right: .5em; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .EmptyPanel { color: #6993ae; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .Loading2 {  }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl { width: 100%; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl th { background: #888; color: #fff; font-weight: normal; border-right: 1px solid #fff; line-height: 2.4em; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl td { font-size: .75em; border-right: 1px solid #fff; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .CompTd1 { width: 25%; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .CompTd2 { width: 25%; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .CompTd3 { width: 25%; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .CompTd4 { width: 25%; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .TrRow:hover td { background: #0a598c; color: #fff; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .TrRow1 td { background: #f9f9f9; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .TrRow2 td { background: #eaeaea; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl td div { white-space:nowrap; max-width: 18em; max-height: 1.5em; line-height: 1.5em; overflow:hidden; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .TrRow1 .Hightlight { background: #fffbd7; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .CompareTbl .TrRow2 .Hightlight { background: #fff59c; }

            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .FieldView { }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .FieldView .Label3 { background: #e4e4e4; cursor:pointer; padding-left: 1.5em; line-height: 2.4em; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .FieldView .Label3:hover { background: #0a598c; color: #fff; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .FieldView .IndentDiv { padding: .2em 0em 0em 3em; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .FieldView .Label4 { background: #888; color: #fff; padding-left: .8em; line-height: 1.8em; }
            .HistoryMainUI .InFrontBox .MainTbl1 .MainTd2 .FieldView .Label5 { padding: .2em 0em 1em .8em; min-height: 1.2em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetTemplate()
        {
            return @"
<css>

<html>
-d #HistoryMainUI
--d #Div_BG .NoSelect evt.Click html='&nbsp;'
--d #InFrontBox
---d #TopDiv
----d #Label1 html='History Tracking'
----d #Label2 html='&nbsp;'
----d #CloseBttn evt.Click html='Close'
----d .clear

---d #LabelSub html='History Tracking is only able to track changes made within ASPdatabase.NET. Changes made outside of this system are not tracked.'
---t #MainTbl1
----tr
-----td #MainTd1
------d .AutoResize
-------t #MenuTable
-----td #MainTd2
------d #EmptyPanel html='(Select a Time Point)'
------d #Loading2 html='Loading...'
------d .AutoResize
-------t #CompareTbl
--------tr .HeadTr
---------th .CompTd1 html='Field Name'
---------th .CompTd2 html='Before Change'
---------th .CompTd3 html='After Change'
---------th .CompTd4 html='Current Value'
-------d #FieldView
--------d #Label3 evt.Click html='&lt;&lt; Fields Chart'
--------d #IndentDiv
---------d .Label4 html='Field Name'
---------d #Label_cn .Label5
---------d .Label4 html='Before Change'
---------d #Label_v1 .Label5
---------d .Label4 html='After Change'
---------d #Label_v2 .Label5
---------d .Label4 html='Current Value'
---------d #Label_v3 .Label5
            ";
        }
    }
}
