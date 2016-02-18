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
    public class ViewOptions_FilterUI : MRBPattern<ViewOptions_FilterField, GridViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ViewOptions_FilterUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ViewOptions_FilterUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            var select = jF(".Select_FilterColumns").html("").append("<option value=''></option>");
            for (int i = 0; i < this.ViewModel.AllColumnNames.Length; i++)
            {
                string name = this.ViewModel.AllColumnNames[i];
                select.append("<option value='" + name + "'>" + name + "</option>");
            }
            if (this.Model.FieldName != null && this.Model.FieldName.Length > 0)
                select.val(this.Model.FieldName);

            string filterType_S = "Equals";
            switch (this.Model.FilterType)
            {
                case ViewOptions_FilterField.FilterTypes.Equals: filterType_S = "Equals"; break;
                case ViewOptions_FilterField.FilterTypes.Not: filterType_S = "Not"; break;
                case ViewOptions_FilterField.FilterTypes.Contains: filterType_S = "Contains"; break;
                case ViewOptions_FilterField.FilterTypes.StartsWith: filterType_S = "StartsWith"; break;
                case ViewOptions_FilterField.FilterTypes.EndsWith: filterType_S = "EndsWith"; break;
                case ViewOptions_FilterField.FilterTypes.LessThan: filterType_S = "LessThan"; break;
                case ViewOptions_FilterField.FilterTypes.LessThanOrEqual: filterType_S = "LessThanOrEqual"; break;
                case ViewOptions_FilterField.FilterTypes.GreaterThanOrEqual: filterType_S = "GreaterThanOrEqual"; break;
                case ViewOptions_FilterField.FilterTypes.GreaterThan: filterType_S = "GreaterThan"; break;
                case ViewOptions_FilterField.FilterTypes.In: filterType_S = "In"; break;
                case ViewOptions_FilterField.FilterTypes.NotIn: filterType_S = "NotIn"; break;
            }
            jF(".Select_FilterTypes").val(filterType_S);

            jF(".Txt_FilterValue").val(this.Model.Value);
        }
        //----------------------------------------------------------------------------------------------------
        public void Reset()
        {
            jF(".Select_FilterColumns").val("");
            jF(".Select_FilterTypes").val("Equals");
            jF(".Txt_FilterValue").val("");
        }
        //----------------------------------------------------------------------------------------------------
        public ViewOptions_FilterField GetCurrentModel()
        {
            this.Model.FieldName = jF(".Select_FilterColumns").val().As<string>();
            switch (jF(".Select_FilterTypes").val().As<string>())
            {
                case "Equals": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.Equals; break;
                case "Not": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.Not; break;
                case "Contains": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.Contains; break;
                case "StartsWith": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.StartsWith; break;
                case "EndsWith": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.EndsWith; break;
                case "LessThan": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.LessThan; break;
                case "LessThanOrEqual": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.LessThanOrEqual; break;
                case "GreaterThanOrEqual": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.GreaterThanOrEqual; break;
                case "GreaterThan": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.GreaterThan; break;
                case "In": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.In; break;
                case "NotIn": this.Model.FilterType = ViewOptions_FilterField.FilterTypes.NotIn; break;
                default: this.Model.FilterType = ViewOptions_FilterField.FilterTypes.NotSet; break;
            }
            this.Model.Value = jF(".Txt_FilterValue").val().As<string>();

            if (this.Model.FieldName.Length < 1 || this.Model.FilterType == ViewOptions_FilterField.FilterTypes.NotSet || this.Model.Value.Length < 1)
                return null;
            else
                return this.Model;
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
                .ViewOptions_FilterUI { padding-bottom: .3em; }
                .ViewOptions_FilterUI .Select_FilterColumns { float:left; width: 17em; max-width: 17em; margin-right: 1em; }
                .ViewOptions_FilterUI .Select_FilterTypes { float:left; width: 9em; max-width: 9em; margin-right: 1em; }
                .ViewOptions_FilterUI .Txt_FilterValue { width: 20em; max-width: 20em; border: 1px solid #093a79; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <select class='Select_FilterColumns'>
                </select>
                <select class='Select_FilterTypes'>
                    <option value='Equals'>&nbsp; ==</span></option>
                    <option value='Not'>&nbsp; Not</span></option>
                    <option value='Contains'>&nbsp; Contains</option>
                    <option value='StartsWith'>&nbsp; Starts With</option>
                    <option value='EndsWith'>&nbsp; Ends With</option>
                    <option value='LessThan'>&nbsp; &lt;</option>
                    <option value='LessThanOrEqual'>&nbsp; &lt;=</option>
                    <option value='GreaterThanOrEqual'>&nbsp; &gt;=</option>
                    <option value='GreaterThan'>&nbsp; &gt;</option>
                    <option value='In'>&nbsp; In (comma separated list)</option>
                    <option value='NotIn'>&nbsp; Not In (comma separated list)</option>
                </select>
                <input type='text' class='Txt_FilterValue' />
            ";
        }
    }
}
