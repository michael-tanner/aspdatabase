using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Record.Objs;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs;

namespace ASPdatabaseNET.UI.PageParts.Record
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class FieldUI : MRBPattern<FieldValue, RecordViewModel>
    {
        public enum InputTypes { NotSet, SingleLine, MultiLineSmall, MultiLineBig, TrueFalse, DropdownList };

        public InputTypes InputType = InputTypes.NotSet;
        public DbInterfaces.TableObjects.Column Column;

        public FieldValue EditModel;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public FieldUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='FieldUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.EditModel = new FieldValue();
            this.EditModel.Index = this.Model.Index;
            this.EditModel.Value = this.Model.Value;
            this.EditModel.IsNull = this.Model.IsNull;

            this.Column = this.ViewModel.RecordInfo.Columns[this.Model.Index];

            this.InputType = InputTypes.SingleLine;
            bool isFixedText = false;
            switch (this.Column.DataType_Name.As<JsString>().toLowerCase())
            {
                case "bit": this.InputType = InputTypes.TrueFalse; break;
                case "ntext": this.InputType = InputTypes.MultiLineBig; break;
                case "text": this.InputType = InputTypes.MultiLineBig; break;
                case "image": this.InputType = InputTypes.MultiLineBig; break;
                case "xml": this.InputType = InputTypes.MultiLineBig; break;
                //case "nvarchar": this.InputType = InputTypes.MultiLineSmall; break;
                //case "nchar": this.InputType = InputTypes.MultiLineSmall; break;
                //case "char": this.InputType = InputTypes.MultiLineSmall; break;
                //case "varbinary": this.InputType = InputTypes.MultiLineSmall; break;
                //case "varchar": this.InputType = InputTypes.MultiLineSmall; break;
                case "nvarchar": this.InputType = InputTypes.SingleLine; break;
                case "nchar": this.InputType = InputTypes.SingleLine; break;
                case "char": this.InputType = InputTypes.SingleLine; break;
                case "varbinary": this.InputType = InputTypes.SingleLine; break;
                case "varchar": this.InputType = InputTypes.SingleLine; break;
            }
            if (isFixedText)
                if (this.Column.MaxLength > 99)
                    this.InputType = InputTypes.MultiLineSmall;

            jF(".NameDiv").html(this.Column.ColumnName);

            jF(".ValueDiv").text(this.Model.Value);
            string value = jF(".ValueDiv").html();
            value = JsStr.S(value).Replace1("\n", "<br />");
            jF(".ValueDiv").html(value);


            if (this.Column.IsPrimaryKey)
                jF(".NameDiv").addClass("PrimaryKeyDiv");

            if (!this.Column.AllowNulls)
                jF(".NullHolder").hide();
            else if (this.EditModel.IsNull)
            {
                this.EditModel.IsNull = false; // used because of toggle in next line
                this.Null_Click();
            }


            var thisObj = this;
            var jRootObj = this.jRoot;
            eval("jRootObj.find('.TextInput').keyup(function(event) { thisObj.Input_Changed(event, true) });");
        }

        //----------------------------------------------------------------------------------------------------
        public void EditModeChanged()
        {
            if(this.ViewModel.EditMode == RecordViewModel.EditModes.Off)
            {
                jF(".EditSection").hide();
                jF(".ValueDiv").show();
                jF(".IdentityLabel").hide();
            }
            else
            {
                jF(".ValueDiv").hide();
                jF(".EditSection").show();
                jF(".EditSection").removeClass("EditSection_Altered");

                var dropdownList = this.CheckFor_DropdownList();
                if (dropdownList != null)
                    this.InputType = InputTypes.DropdownList;

                switch(this.InputType)
                {
                    case InputTypes.SingleLine:
                        jF(".Txt_SingleLine").show().val(this.Model.Value);
                        break;
                    case InputTypes.MultiLineSmall:
                        jF(".Txt_MultiLineSmall").show().val(this.Model.Value);
                        break;
                    case InputTypes.MultiLineBig:
                        jF(".Txt_MultiLineLarge").show().val(this.Model.Value);
                        break;
                    case InputTypes.TrueFalse:
                        jF(".Div_TrueFalse").show();
                        this.EditModel.Value = this.Model.Value.As<JsString>().toLowerCase().trim();
                        this.TrueFalse_Update();
                        break;
                    case InputTypes.DropdownList:
                        jF(".NullHolder").hide();
                        if (this.EditModel.IsNull)
                            this.EditModel.IsNull = false;
                        var selectDropdown = jF(".SelectDropdown").show().html("");
                        for (int i = 0; i < dropdownList.Items.Length; i++)
                            if (dropdownList.Items[i] == this.Model.Value)
                                selectDropdown.append("<option selected='selected'>" + dropdownList.Items[i] + "</option>");
                            else
                                selectDropdown.append("<option>" + dropdownList.Items[i] + "</option>");
                        break;
                }

                if(this.Column.IsIdentity)
                {
                    jF(".Txt_SingleLine").hide();
                    jF(".IdentityLabel").show().html("[Identity]");
                    if(this.Model.Value != null && this.Model.Value.Length > 0)
                        jF(".IdentityLabel").html("[Identity] : " + this.Model.Value);
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        public DropdownList CheckFor_DropdownList()
        {
            var appPropInfo = this.ViewModel.AppPropertiesInfo;
            if (appPropInfo == null)
                return null;

            string columnName = st.New(this.Column.ColumnName).Trim().ToLower().TheString;
            AppPropertiesItem appPropertiesItem = null;
            for (int i = 0; i < appPropInfo.Columns.Length; i++)
                if (st.New(appPropInfo.Columns[i].ColumnName).Trim().ToLower().TheString == columnName)
                    appPropertiesItem = appPropInfo.Columns[i];

            if (appPropertiesItem != null && appPropertiesItem.AppColumnType == AppPropertiesItem.AppColumnTypes.DropdownList && appPropInfo.DropdownListItems != null)
                for (int i = 0; i < appPropInfo.DropdownListItems.Length; i++)
                    if (st.New(appPropInfo.DropdownListItems[i].ColumnName).Trim().ToLower().TheString == columnName)
                    {
                        return appPropInfo.DropdownListItems[i];
                    }
            return null;
        }

        //----------------------------------------------------------------------------------------------------
        public bool CheckForValueChange()
        {
            var tmpModel = this.GetValueForSave();

            if (tmpModel.IsNull != this.Model.IsNull)
                return true;

            if (this.InputType == InputTypes.TrueFalse)
            {
                if (tmpModel.Value.As<JsString>().toLowerCase() != this.Model.Value.As<JsString>().toLowerCase())
                    return true;
            }
            else
            {
                if (tmpModel.Value != this.Model.Value)
                    return true;
            }
            return false;
        }
        //----------------------------------------------------------------------------------------------------
        public FieldValue GetValueForSave()
        {
            var rtn = this.EditModel;
            if (!rtn.IsNull)
            {
                switch (this.InputType)
                {
                    case InputTypes.SingleLine:
                        rtn.Value = jF(".Txt_SingleLine").val().As<string>();
                        break;
                    case InputTypes.MultiLineSmall:
                        rtn.Value = jF(".Txt_MultiLineSmall").val().As<string>();
                        break;
                    case InputTypes.MultiLineBig:
                        rtn.Value = jF(".Txt_MultiLineLarge").val().As<string>();
                        break;
                    case InputTypes.TrueFalse:
                        rtn.Value = this.EditModel.Value;
                        break;
                    case InputTypes.DropdownList:
                        rtn.Value = jF(".SelectDropdown").val().As<string>();
                        break;
                }
            }
            return rtn;
        }





        //------------------------------------------------------------------------------------------ Events --
        public void Input_Changed(Event evt, bool deselectNull)
        {
            if (this.CheckForValueChange())
                jF(".EditSection").addClass("EditSection_Altered");
            else
                jF(".EditSection").removeClass("EditSection_Altered");

            if (deselectNull)
                if (this.EditModel.IsNull)
                    this.Null_Click();

            //document.title = "" + evt.keyCode;
            //switch (evt.keyCode)
            //{
            //    case 27: // Esc
            //        jF2(className).blur();
            //        break;
            //    case 37: // Left
            //        break;
            //    case 38: // Up
            //        this.OnUpOrDownArrow.After.Fire2(this.ColumnPosition_Property - 2, className);
            //        break;
            //    case 39: // Right
            //        break;
            //    case 40: // Down
            //        this.OnUpOrDownArrow.After.Fire2(this.ColumnPosition_Property, className);
            //        break;
            //    case 13: // Enter
            //        break;
            //}
        }

        //----------------------------------------------------------------------------------------------------
        public void Null_Click()
        {
            this.EditModel.IsNull = !this.EditModel.IsNull;

            if(this.EditModel.IsNull)
            {
                jF(".NullHolder").addClass("NullHolder_On");
                jF(".Checkbox_Null").attr("checked", true);
            }
            else
            {
                jF(".NullHolder").removeClass("NullHolder_On");
                jF(".Checkbox_Null").removeAttr("checked");
            }
            this.Input_Changed(null, false);
        }
        //----------------------------------------------------------------------------------------------------
        public void TrueFalse_Click()
        {
            if (this.EditModel.Value == "true")
                this.EditModel.Value = "false";
            else
                this.EditModel.Value = "true";
            this.TrueFalse_Update();
            this.Input_Changed(null, true);
        }
        //----------------------------------------------------------------------------------------------------
        public void TrueFalse_Update()
        {
            if (this.EditModel.Value == "true")
            {
                jF(".Div_TrueFalse").addClass("Div_TrueFalse_On");
                jF(".Checkbox_TrueFalse").attr("checked", true);
                jF(".Label_TrueFalse").html("True");
            }
            else if (this.EditModel.Value == "false")
            {
                jF(".Div_TrueFalse").removeClass("Div_TrueFalse_On");
                jF(".Checkbox_TrueFalse").removeAttr("checked");
                jF(".Label_TrueFalse").html("False");
            }
            else
            {
                jF(".Div_TrueFalse").removeClass("Div_TrueFalse_On");
                jF(".Checkbox_TrueFalse").removeAttr("checked");
                jF(".Label_TrueFalse").html("&nbsp;");
            }
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
                .FieldUI { line-height: 1.875em; border-bottom: 0.125em solid #fff; background: #f4f4f4; }
                .FieldUI .NameTd { background: #eee; font-size: .8em; line-height: 2.34375em; width: 18em; max-width: 18em; min-width: 18em; overflow:hidden; }
                .FieldUI .ValueTd { font-size: .8em; }

                .FieldUI .NameTd .NameDiv { padding-left: .75em; }
                .FieldUI .NameTd .PrimaryKeyDiv { border-left: .35em solid #eb640a; padding-left: .45em; }

                .FieldUI .ValueTd .ValueDiv { line-height: 1.54em; margin: 0.384615em 0em; }
                .FieldUI .ValueTd .IdentityLabel { display:none; }

                .FieldUI .ValueTd .EditSection { display:none; padding-left: .75em; }
                .FieldUI .ValueTd .EditSection_Altered { border-left: .35em solid #0da218; padding-left: .45em; }
                .FieldUI .ValueTd .EditSection .InputHolder { float:left; width: 42.6em; }
                .FieldUI .ValueTd .EditSection .InputHolder .Txt_SingleLine { display:none; width: 40em; padding: 0.4em; border: 1px solid #093a79; }
                .FieldUI .ValueTd .EditSection .InputHolder .Txt_MultiLineSmall { display:none; width: 38.3906em; padding: 0.4em; height: 4em;   border: 1px solid #093a79; }
                .FieldUI .ValueTd .EditSection .InputHolder .Txt_MultiLineLarge { display:none; width: 38.3906em; padding: 0.4em; height: 20em;  border: 1px solid #093a79; }
                .FieldUI .ValueTd .EditSection .InputHolder .Div_TrueFalse { display:none; cursor:pointer; border-radius: .35em; color: #888; width: 8em; }
                .FieldUI .ValueTd .EditSection .InputHolder .Div_TrueFalse:hover { background: #0b346a; color: #fff; }
                .FieldUI .ValueTd .EditSection .InputHolder .Div_TrueFalse_On { background: #3172c8; color: #fff; }
                .FieldUI .ValueTd .EditSection .InputHolder .Div_TrueFalse .Checkbox_TrueFalse { float: left; margin: 0.703125em 0.625em 0em 0.78125em; }
                .FieldUI .ValueTd .EditSection .InputHolder .Div_TrueFalse .Label_TrueFalse { float:left; }
                .FieldUI .ValueTd .EditSection .InputHolder .SelectDropdown { display:none; }

                .FieldUI .ValueTd .EditSection .NullHolder { float:left; position:relative; width: 5.5em; cursor:pointer; color: #656565; border: 1px solid #f4f4f4; border-radius: .4em; }
                .FieldUI .ValueTd .EditSection .NullHolder:hover { background: #aabace; color: #fff; }
                .FieldUI .ValueTd .EditSection .NullHolder input { position:absolute; top: 0.625em; left: 0.625em; }
                .FieldUI .ValueTd .EditSection .NullHolder span { padding-left: 2.1875em; }
                .FieldUI .ValueTd .EditSection .NullHolder_On { background: #fffeb4; color: #000; border-color: #aabace; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='NameTd'><div class='NameDiv'></div></td>
                <td class='ValueTd'>
                    <div class='ValueDiv'></div>
                    <div class='IdentityLabel' title='Auto-Increment Number'></div>
                    <div class='EditSection'>
                        <div class='InputHolder'>
                            <input type='text' class='Txt_SingleLine TextInput' />
                            <textarea class='Txt_MultiLineSmall TextInput'></textarea>
                            <textarea class='Txt_MultiLineLarge TextInput'></textarea>
                            <div class='Div_TrueFalse' On_Click='TrueFalse_Click'>
                                <input type='checkbox' class='Checkbox_TrueFalse' />
                                <div class='Label_TrueFalse'></div>
                                <div class='clear'></div>
                            </div>
                            <select class='SelectDropdown'></select>
                        </div>
                        <div class='NullHolder' On_Click='Null_Click'>
                            <input type='checkbox' class='Checkbox_Null' />
                            <span>Null</span>
                        </div>
                        <div class='clear'></div>
                    </div>
                </td>
            ";
        }
    }
}
