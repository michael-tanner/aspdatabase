using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs;

namespace ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.UIParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class EditAppItem : MRBPattern<AppPropertiesItem, TableDesign_ViewModel>
    {
        public AppPropertiesItem.AppColumnTypes Temp_AppColumnType;
        public AppPropertiesInfo ParentModel;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public EditAppItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='EditAppItem jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            this.BindUI();

            jF(".ModBox").show();
            this.Temp_AppColumnType = this.Model.AppColumnType;
            if (this.Model.IsPrimaryKey)
                jF(".Radio1b").hide();
            else
                jF(".Radio1b").show();


            DropdownList tmpDropdownList = null;
            if (this.ParentModel.DropdownListItems != null)
                for (int i = 0; i < this.ParentModel.DropdownListItems.Length; i++)
                    if (st.New(this.ParentModel.DropdownListItems[i].ColumnName).ToLower().Trim().TheString == st.New(this.Model.ColumnName).ToLower().Trim().TheString)
                        tmpDropdownList = this.ParentModel.DropdownListItems[i];
            string s = "";
            if(tmpDropdownList != null)
                for (int i = 0; i < tmpDropdownList.Items.Length; i++)
                    if (i > 0) 
                        s += "\n" + tmpDropdownList.Items[i];
                    else 
                        s += tmpDropdownList.Items[i];
            jF(".TextArea1").val(s);

            switch(this.Model.AppColumnType)
            {
                case AppPropertiesItem.AppColumnTypes.Default:
                    this.AppColumnType_Click_Default();
                    break;
                case AppPropertiesItem.AppColumnTypes.DropdownList:
                    this.AppColumnType_Click_DropdownList();
                    break;
            }
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Click_Save()
        {
            jF(".ModBox").hide();
            this.Model.AppColumnType = this.Temp_AppColumnType;
            this.OnChange.After.Fire();
        }
        public string[] GetDropdownValues()
        {
            var dropdownValues = st.New(jF(".TextArea1").val().As<string>());
            var arr = dropdownValues.Split("\n", false);
            var rtn = new string[0];
            for (int i = 0; i < arr.Length; i++)
                rtn[i] = arr[i].TheString;
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public void AppColumnType_Click_Default()
        {
            jF(".Radio_AppColumnType_Default").prop("checked", true);
            jF(".DropdownListDiv").hide();
            this.Temp_AppColumnType = AppPropertiesItem.AppColumnTypes.Default;
        }
        public void AppColumnType_Click_DropdownList()
        {
            jF(".Radio_AppColumnType_DropdownList").prop("checked", true);
            jF(".DropdownListDiv").show();
            this.Temp_AppColumnType = AppPropertiesItem.AppColumnTypes.DropdownList;
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
                .EditAppItem { }
                .EditAppItem .Div_BG { background: #464f5b; opacity: 0.65; position:fixed; width: 100%; height: 8000px; top: 0px; left: 0px; z-index: 999; }
                .EditAppItem .ModBox { position:absolute; top: 1.5em; left: 0.5em; width: 40em; min-height: 10em; padding: 1em; background: #fff; box-shadow: .05em .05em .4em #000; z-index: 1000; }
                .EditAppItem .ModBox .Bttn { font-size: .9em; float:left; cursor:pointer; margin-right: 1em; padding: .5em 1.5em; background: #14498f; color: #fff; }
                .EditAppItem .ModBox .Bttn:hover { background: #333; }
                .EditAppItem .ModBox .Radio1 { cursor:pointer; padding: .2em .4em; }
                .EditAppItem .ModBox .Radio1:hover { background: #0b94da; color: #fff; }

                .EditAppItem .ModBox .DropdownListDiv { display:none; }
                .EditAppItem .ModBox .DropdownListDiv .TextArea1 { width: 24em; height: 14em; margin-top: .5em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Div_BG NoSelect' On_Click='Close'>&nbsp;</div>
                <div class='ModBox'>
                    <div class='Bttn' On_Click='Click_Save'>Save</div>
                    <div class='Bttn' On_Click='Close'     >Cancel</div>
                    <div class='clear'></div>
                    <br />
                    Custom App Properties : <span ModelKey='ColumnName'></span>
                    <br />
                    <br />
                    <div On_Click='AppColumnType_Click_Default'      class='Radio1        '><input type='radio' name='AppColumnType' value='Default'      class='Radio_AppColumnType_Default'      /> Default</div>
                    <div On_Click='AppColumnType_Click_DropdownList' class='Radio1 Radio1b'><input type='radio' name='AppColumnType' value='DropdownList' class='Radio_AppColumnType_DropdownList' /> DropdownList</div>
                    <br />
                    <div class='DropdownListDiv'>
                        Set the list of Dropdown values here. <br />
                        Put each value on a new line.
                        <br />
                        <textarea class='TextArea1'></textarea>
                    </div>
                </div>
            ";
        }
    }
}
