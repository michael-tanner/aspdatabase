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
    public class RowUI : MRBPattern<GridRow, GridViewModel>
    {
        public bool IsOdd;
        public bool IsSelected = false;
        public JsEvent_BeforeAfter OnSelection = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnOpenViewer = new JsEvent_BeforeAfter();

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public RowUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='RowUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            if (this.IsOdd)
                this.jRoot.addClass("RowUI_Alt");

            for (int i = 0; i < this.Model.Values.Length; i++)
            {
                string value = this.Model.Values[i];
                if (this.ViewModel.TableType == GridRequest.TableTypes.View)
                    if (value.Length > 100)
                        value = st.New(value).TruncateRight(value.Length - 99).TheString + "...";
                this.jRoot.append(JsStr.StrFormat1("<td>{0}</td>", value));
            }

            if(this.ViewModel.TableType == GridRequest.TableTypes.Table)
                jF(".Bttn_Open").attr("href", "#00-Record-" + this.Model.UniqueKey);
            else
            {
                jF(".Bttn_Open").hide();
                jF(".Bttn_Open2").show();
            }


            var thisObj = this;
            var jRootObj = this.jRoot;
            eval(@"
                jRootObj.find('td').click(function() { thisObj.Td_Clicked(this); });
                jRootObj.find('td').dblclick(function() { thisObj.Td_DoubleClicked(this); });
            ");
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Td_Clicked(object obj)
        {
            if(!J(obj).hasClass("Td_Open"))
            {
                this.SetSelection(!this.IsSelected);
                this.OnSelection.After.Fire();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void SetSelection(bool isSelected)
        {
            this.IsSelected = isSelected;
            if (this.IsSelected)
            {
                jRoot.addClass("RowUI_Selected");
                jF(".Checkbox_Select").attr("checked", true);
            }
            else
            {
                jRoot.removeClass("RowUI_Selected");
                jF(".Checkbox_Select").removeAttr("checked");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void Td_DoubleClicked(object obj)
        {
            switch(this.ViewModel.TableType)
            {
                case GridRequest.TableTypes.Table:
                    window.location = ("#00-Record-" + this.Model.UniqueKey).As<Location>();
                    break;
                case GridRequest.TableTypes.View:
                    this.Bttn_Open2_Click();
                    break;
            }
        }

        //----------------------------------------------------------------------------------------------------
        public void Bttn_Open2_Click()
        {
            this.OnOpenViewer.After.Fire1(this.Model);
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
                .RowUI { }
                .RowUI { }
                .RowUI td { background: #ededed; margin: 0px 1px 1px 0px; white-space:nowrap;
                            border: 1px solid #fff; border-width: 1px 1px 1px 0px; padding: 0.125em 0.375em; }
                .RowUI_Alt td { background: #cfd4da; }

                .RowUI .Td_CheckboxSelect { padding-top: .25em; padding-bottom: 0em; }
                .RowUI .Td_CheckboxSelect:hover { background: #093a79; }
                .RowUI .Td_CheckboxSelect .Checkbox_Select { }

                .RowUI .Td_Open { padding: 0em; }
                .RowUI .Td_Open .Bttn_Open { display:block; text-decoration:none; color: blue; padding: 0.125em 0.375em; }
                .RowUI .Td_Open .Bttn_Open:hover { background: #093a79; color: #fff; }
                .RowUI .Td_Open .Bttn_Open span { font-size: .9em; }

                .RowUI .Td_Open .Bttn_Open2 { display:none; text-decoration:none; color: blue; padding: 0.125em 0.375em; cursor:pointer; }
                .RowUI .Td_Open .Bttn_Open2:hover { background: #093a79; color: #fff; }
                .RowUI .Td_Open .Bttn_Open2 span { font-size: .9em; }


                .RowUI:hover td { background: #42a4ed; color: #fff; }
                .RowUI:hover .Bttn_Open { color: #fff; }
                .RowUI:hover .Bttn_Open2 { color: #fff; }

                .RowUI_Selected td { background: #2c6fc5; color: #fff; }
                .RowUI_Selected .Td_Open .Bttn_Open { color: #fff; }
                .RowUI_Selected .Td_Open .Bttn_Open2 { color: #fff; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='Td_CheckboxSelect'><input type='checkbox' class='Checkbox_Select' /></td>
                <td class='Td_Open'>
                    <a class='Bttn_Open' href='#'><span>Open</span></a>
                    <div class='Bttn_Open2' On_Click='Bttn_Open2_Click'><span>Open</span></div>
                </td>
            ";
        }
    }
}
