using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.UIParts;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AppPropertiesPanel : MRBPattern<AppPropertiesInfo, TableDesign_ViewModel>
    {
        public AppPropertyItemUI[] UIItems;
        public EditAppItem EditAppItem;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public AppPropertiesPanel(TableDesign_ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='AppPropertiesPanel jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.EditAppItem = new EditAppItem();
            this.EditAppItem.Instantiate();
            this.EditAppItem.ViewModel = this.ViewModel;
            this.EditAppItem.OnChange.After.AddHandler(this, "EditAppItem_Change", 0);
            this.EditAppItem.Close();
            this.jRoot.append(this.EditAppItem.jRoot);
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            AjaxService.ASPdatabaseService.New(this, "Get_Return").TableDesign__AppProperties__Get(this.ViewModel.TableId, false);
        }
        public void Get_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<AppPropertiesInfo>();

            this.UIItems = new AppPropertyItemUI[0];
            var holder_UIItems = jF(".Holder_UIItems").html("");
            for (int i = 0; i < this.Model.Columns.Length; i++)
            {
                this.UIItems[i] = new AppPropertyItemUI();
                this.UIItems[i].ViewModel = this.ViewModel;
                this.UIItems[i].Model = this.Model.Columns[i];
                this.UIItems[i].Instantiate();
                this.UIItems[i].OnChange.After.AddHandler(this, "UIItem_Click", 1);
                holder_UIItems.append(this.UIItems[i].jRoot);
            }
        }






        //------------------------------------------------------------------------------------------ Events --
        public void UIItem_Click(AppPropertiesItem itemModel)
        {
            this.EditAppItem.Model = itemModel;
            this.EditAppItem.ParentModel = this.Model;
            this.EditAppItem.Open();
        }
        public void EditAppItem_Change()
        {
            jF(".Holder_UIItems").html();

            var subModel = this.EditAppItem.Model;
            if (this.Model.DropdownListItems == null)
                this.Model.DropdownListItems = new DropdownList[0];
            DropdownList tmpDropdownList = null;
            for (int i = 0; i < this.Model.DropdownListItems.Length; i++)
                if (st.New(this.Model.DropdownListItems[i].ColumnName).ToLower().Trim().TheString == st.New(subModel.ColumnName).ToLower().Trim().TheString)
                    tmpDropdownList = this.Model.DropdownListItems[i];
            if(tmpDropdownList == null)
            {
                tmpDropdownList = new DropdownList();
                this.Model.DropdownListItems[this.Model.DropdownListItems.Length] = tmpDropdownList;
            }
            tmpDropdownList.ColumnName = subModel.ColumnName;
            tmpDropdownList.Items = this.EditAppItem.GetDropdownValues();

            AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__AppProperties__Save(this.ViewModel.TableId, this.Model);
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            this.EditAppItem.Close();
            UI.Errors.ExceptionHandler.Check(ajaxResponse);
            this.Open();
        }





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += AppPropertyItemUI.GetCssTree();
            rtn += EditAppItem.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .AppPropertiesPanel { width: 100%; position:relative; padding-top: 37px; }
                .AppPropertiesPanel .TopColorBar { position:absolute; top: -1px; width: inherit; min-width: 200px; height: 8px; background: #173a67; margin-bottom: 35px; }

                .AppPropertiesPanel .ScrollArea { overflow-y:auto; padding-right: 5px; }

                .AppPropertiesPanel .Holder_UIItems { border-top: 1px solid #888; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopColorBar'></div>

                <div class='ScrollArea AutoResize'>
                    <table class='Holder_UIItems'></table>
                </div>
            ";
        }
    }
}
