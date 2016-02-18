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
    public class ViewOptions : MRBPattern<string, GridViewModel>
    {
        public bool HasBeenOpen = false;
        public JsEvent_BeforeAfter OnRefresh = new JsEvent_BeforeAfter();

        public ViewOptions_FilterUI[] FilterUIs;
        public ViewOptions_SortUI[] SortUIs;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ViewOptions()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ViewOptions jRoot NoSelect'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            var filterAndSort = this.ViewModel.FilterAndSort;

            if (filterAndSort.FilterFields == null)
                filterAndSort.FilterFields = new ViewOptions_FilterField[0];
            for (int i = filterAndSort.FilterFields.Length; i < 5; i++)
                filterAndSort.FilterFields[i] = new ViewOptions_FilterField();

            if (filterAndSort.SortFields == null)
                filterAndSort.SortFields = new ViewOptions_SortField[0];
            for (int i = filterAndSort.SortFields.Length; i < 5; i++)
                filterAndSort.SortFields[i] = new ViewOptions_SortField();

            this.HasBeenOpen = true;
            if(this.FilterUIs == null)
            {
                this.FilterUIs = new ViewOptions_FilterUI[0];
                var holder_FilterUIs = jF(".Holder_FilterUIs").html("");
                for (int i = 0; i < 5; i++)
                {
                    this.FilterUIs[i] = new ViewOptions_FilterUI();
                    this.FilterUIs[i].ViewModel = this.ViewModel;
                    this.FilterUIs[i].Model = filterAndSort.FilterFields[i];
                    this.FilterUIs[i].Instantiate();
                    holder_FilterUIs.append(this.FilterUIs[i].jRoot);
                }
                this.SortUIs = new ViewOptions_SortUI[0];
                var holder_SortUIs = jF(".Holder_SortUIs").html("");
                for (int i = 0; i < 5; i++)
                {
                    this.SortUIs[i] = new ViewOptions_SortUI();
                    this.SortUIs[i].ViewModel = this.ViewModel;
                    this.SortUIs[i].Model = filterAndSort.SortFields[i];
                    this.SortUIs[i].Instantiate();
                    holder_SortUIs.append(this.SortUIs[i].jRoot);
                }
            }

            this.RefreshUI();
        }
        //----------------------------------------------------------------------------------------------------
        public void RefreshUI()
        {
            jF(".Txt_DisplayTopNRows").val(this.ViewModel.DisplayTopNRows.As<string>());

            for (int i = 0; i < this.FilterUIs.Length; i++)
                this.FilterUIs[i].Open();
            for (int i = 0; i < this.SortUIs.Length; i++)
                this.SortUIs[i].Open();
        }

        //----------------------------------------------------------------------------------------------------
        public void Update_FiltersAndSorts()
        {
            if(this.HasBeenOpen)
            {
                this.ViewModel.FilterAndSort.FilterFields = this.Get_FilterFields();
                this.ViewModel.FilterAndSort.SortFields = this.Get_SortFields();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public ViewOptions_FilterField[] Get_FilterFields()
        {
            var rtn = new ViewOptions_FilterField[0];
            if (this.FilterUIs == null)
                return rtn;
            int j = 0;
            for (int i = 0; i < this.FilterUIs.Length; i++)
            {
                var subModel = this.FilterUIs[i].GetCurrentModel();
                if (subModel != null)
                    rtn[j++] = subModel;
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public ViewOptions_SortField[] Get_SortFields()
        {
            var rtn = new ViewOptions_SortField[0];
            if (this.SortUIs == null)
                return rtn;
            int j = 0;
            for (int i = 0; i < this.SortUIs.Length; i++)
            {
                var subModel = this.SortUIs[i].GetCurrentModel();
                if (subModel != null)
                    rtn[j++] = subModel;
            }
            return rtn;
        }




        //------------------------------------------------------------------------------------------ Events --
        public void BttnClick_ApplyAndRefresh()
        {
            int tmp = ASPdb.FrameworkUI.IntStatic.Parse(jF(".Txt_DisplayTopNRows").val().As<string>(), this.ViewModel.DisplayTopNRows);
            this.ViewModel.DisplayTopNRows = tmp;
            jF(".Txt_DisplayTopNRows").val(tmp.As<string>());

            var gridRequest_Local = new GridRequest();
            gridRequest_Local.TableType = this.ViewModel.TableType;
            gridRequest_Local.Id = this.ViewModel.Id;
            gridRequest_Local.DisplayTopNRows = this.ViewModel.DisplayTopNRows;

            this.Update_FiltersAndSorts();
            gridRequest_Local.FilterFields = this.ViewModel.FilterAndSort.FilterFields;
            gridRequest_Local.SortFields = this.ViewModel.FilterAndSort.SortFields;
            try
            {
                string json = (new ASPdb.Ajax.AjaxHelper()).ToJson(gridRequest_Local);
                localStorage.setItem(this.ViewModel.LocalStorage_Key_ViewOptions, json);
            }
            catch { }

            this.Close();
            this.OnRefresh.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void BttnClick_Reset()
        {
            jF(".Txt_DisplayTopNRows").val((new GridViewModel()).DisplayTopNRows.As<string>());
            for (int i = 0; i < this.FilterUIs.Length; i++)
                this.FilterUIs[i].Reset();
            for (int i = 0; i < this.SortUIs.Length; i++)
                this.SortUIs[i].Reset();
            try { localStorage.setItem(this.ViewModel.LocalStorage_Key_ViewOptions, ""); }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        public void BttnClick_Cancel()
        {
            this.Close();
        }





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            return ""
                + ViewOptions_FilterUI.GetCssTree()
                + ViewOptions_SortUI.GetCssTree()
                + GetCssRoot();
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ViewOptions { }
                .ViewOptions .Div_BG { background: #464f5b; opacity: 0.65; position:fixed; width: 100%; height: 8000px; top: 0px; left: 0px; }

                .ViewOptions .InFrontBox { position:relative; background: #f4f4f4; min-height: 15em; box-shadow: 0.125em 0.125em .2em #777; }
                .ViewOptions .InFrontBox .TopLine { height: .5em; background: #eb640a; }
                .ViewOptions .InFrontBox .MainBox { padding: 0.75em 5em 2.5em 1.375em; color: #093a79; }
                .ViewOptions .InFrontBox .MainBox .Bttn1 { font-size: .9em; float:left; margin-right: 0.75em; line-height: 2em; padding: 0em 1em;
                                                           background: #093a79; color: #fff; border-radius: 0em; cursor:pointer; }
                .ViewOptions .InFrontBox .MainBox .Bttn1:hover { background: #000; }
                             
                .ViewOptions .InFrontBox .MainBox .Txt_DisplayTopNRows { border: 1px solid #093a79; width: 80px; text-align:center; }
                             
                .ViewOptions .InFrontBox .MainBox .SubHead { margin: 1.35em 0em .15em; font-size: 1.3em; }
                .ViewOptions .InFrontBox .MainBox .Holder_FilterUIs { }
                .ViewOptions .InFrontBox .MainBox .Holder_SortUIs { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Div_BG NoSelect' On_Click='Close'>&nbsp;</div>
                <div class='InFrontBox'>
                    <div class='TopLine'><span></span></div>
                    <div class='MainBox'>
                        <div class='Bttn1' On_Click='BttnClick_ApplyAndRefresh'>Apply & Refresh</div>
                        <div class='Bttn1' On_Click='BttnClick_Reset'>Reset</div>
                        <div class='Bttn1' On_Click='BttnClick_Cancel'>Cancel</div>
                        <div class='clear'></div>
                        <br />
                        <br />
                        Display Top <input type='text' class='Txt_DisplayTopNRows' /> Rows
                    
                        <div class='SubHead'>Filter</div>
                        <div class='Holder_FilterUIs'></div>

                        <div class='SubHead'>Sort By</div>
                        <div class='Holder_SortUIs'></div>
                    </div>
                </div>
            ";
        }
    }
}
