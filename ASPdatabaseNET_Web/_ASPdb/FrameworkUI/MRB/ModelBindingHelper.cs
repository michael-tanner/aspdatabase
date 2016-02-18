using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdb.FrameworkUI.MRB
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ModelBindingHelper<M, VM> : jQueryContext
    {
        private IMRBPattern<M, VM> MRB;
        
        //------------------------------------------------------------------------------------- constructor --
        public ModelBindingHelper(IMRBPattern<M, VM> mrb)
        {
            this.MRB = mrb;
        }


        //----------------------------------------------------------------------------------------------------
        /// <summary>Use * in modelKey to bind all ModelKey elements</summary>
        public void Bind_ModelToUI(IMRBPattern<M, VM> mrb, string modelKey)
        {
            this.Bind_ModelListElements(mrb, modelKey);

            this.Bind_ModelKeyElements(mrb, modelKey);
        }




        //----------------------------------------------------------------------------------------------------
        public void Bind_ModelListElements(IMRBPattern<M, VM> mrb, string modelKey)
        {
            var jListElements = mrb.jF2("[ModelList]");
            if (jListElements == null)
                return;

            for (int i = 0; i < jListElements.length; i++)
            {
                var jListItem = J(jListElements[i]);

                if (modelKey == "*" || modelKey == jListItem.attr("ModelKey")) // filter based on ModelKey -- use * for all
                {
                    string modelList_CmdText = jListItem.attr("ModelList");
                    var cmd = ModelListCommand.New1(modelList_CmdText);

                    var tmpModelObj = JsObj.O(mrb).Property_Get(cmd.Model_Name, cmd.UseGetterMethod_Model_Name);
                    object[] optionsList = JsObj.O(tmpModelObj).Property_Get(cmd.Model_Collection, cmd.UseGetterMethod_Model_Collection).As<object[]>();
                    if (optionsList != null && optionsList.Length > 0)
                    {
                        this.Populate_SelectOptions(jListItem, cmd.Model_Collection, optionsList, cmd);
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void Populate_SelectOptions(jQuery jSelectElement, string listPropertiesSetting, object[] optionsObjects, ModelListCommand cmd)
        {
            jSelectElement.html("");

            for (int i = 0; i < optionsObjects.Length; i++)
            {
                var optionsObj = JsObj.O(optionsObjects[i]);

                string value = optionsObj.Property2(cmd.Item_Value, "");
                string text = optionsObj.Property2(cmd.Item_Text, "");
                string style = optionsObj.Property2(cmd.Item_Style, "");

                string styleAttribute = "";
                if (style.Length > 0)
                    styleAttribute = "style='" + style + "'";

                jSelectElement.append(JsStr.StrFormat3("<option value='{0}' {1} >{2}</option>", value, styleAttribute, text));
            }
        }





        //----------------------------------------------------------------------------------------------------
        private void Bind_ModelKeyElements(IMRBPattern<M, VM> mrb, string modelKey)
        {
            M model = mrb.GetModelWithoutFiringEvents();

            if (model == null)
                return;

            var jModelKeyElements = mrb.jF2("[ModelKey]");
            if (jModelKeyElements == null)
                return;

            for (int i = 0; i < jModelKeyElements.length; i++)
            {
                var modelKeyAttrValues = J(jModelKeyElements[i]).attr("ModelKey").split('|');
                for (int ii = 0; ii < modelKeyAttrValues.length; ii++)
                {
                    string tmpModelKey = modelKeyAttrValues[ii];
                    if (modelKey == "*" || modelKey == tmpModelKey)
                    {
                        switch (jModelKeyElements[i].tagName.toUpperCase())
                        {
                            case "DIV":
                                this.ProcessModelKey_ForDivOrSpan(model, jModelKeyElements[i], tmpModelKey);
                                break;
                            case "SPAN":
                                this.ProcessModelKey_ForDivOrSpan(model, jModelKeyElements[i], tmpModelKey);
                                break;
                            case "INPUT":
                                this.ProcessModelKey_ForInput(model, jModelKeyElements[i], tmpModelKey);
                                break;
                            case "TEXTAREA":
                                this.ProcessModelKey_ForInput(model, jModelKeyElements[i], tmpModelKey);
                                break;
                            case "SELECT":
                                this.ProcessModelKey_ForSelect(model, jModelKeyElements[i], tmpModelKey);
                                break;
                        }
                    }
                }
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void ProcessModelKey_ForDivOrSpan(M model, HtmlElement modelKeyElement, JsString modelKeyCmd)
        {
            var arr = modelKeyCmd.trim().split(':');
            if (arr.length < 2)
            {
                string modelKey = arr[0].trim();
                string modelValue = JsObj.O(model).Property(modelKey);
                J(modelKeyElement).html(modelValue);
            }
            else
            {
                string elementAttrName = arr[0].trim();
                string modelKey = arr[1].trim();
                string modelValue = JsObj.O(model).Property(modelKey);
                J(modelKeyElement).attr(elementAttrName, modelValue);
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void ProcessModelKey_ForInput(M model, HtmlElement modelKeyElement, JsString modelKeyCmd)
        {
            var arr = modelKeyCmd.trim().split(':');
            if (arr.length < 2)
            {
                string modelKey = arr[0].trim();
                string modelValue = JsObj.O(model).Property(modelKey);
                J(modelKeyElement).val(modelValue);

                
            }
            else
            {
                string elementAttrName = arr[0].trim();
                string modelKey = arr[1].trim();
                string modelValue = JsObj.O(model).Property(modelKey);
                J(modelKeyElement).attr(elementAttrName, modelValue);
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void ProcessModelKey_ForSelect(M model, HtmlElement modelKeyElement, JsString modelKeyCmd)
        {
            if (model != null && modelKeyElement != null && modelKeyCmd != null && modelKeyCmd.length > 0)
            {
                var selectElement = modelKeyElement.As<HtmlSelectElement>();
                var m = model;

                string value = JsObj.O(model).Property_Get(modelKeyCmd, false).As<string>();
                if (value != null)
                {
                    for (int i = 0; i < selectElement.options.length; i++)
                    {
                        var option = (HtmlOptionElement)selectElement.options[i];
                        if (value == option.value)
                        {
                            option.selected = true;
                            return;
                        }
                    }

                }
            }
        }
        


    }

}