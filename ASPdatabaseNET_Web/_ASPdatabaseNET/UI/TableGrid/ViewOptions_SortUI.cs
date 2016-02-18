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
    public class ViewOptions_SortUI : MRBPattern<ViewOptions_SortField, GridViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ViewOptions_SortUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ViewOptions_SortUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            var select = jF(".Select_SortColumns").html("").append("<option value=''></option>");
            for (int i = 0; i < this.ViewModel.AllColumnNames.Length; i++)
            {
                string name = this.ViewModel.AllColumnNames[i];
                select.append("<option value='" + name + "'>" + name + "</option>");
            }
            if (this.Model.FieldName != null && this.Model.FieldName.Length > 0)
                select.val(this.Model.FieldName);

            this.Model.Descending = !this.Model.Descending;
            this.Descending_Click();
        }
        //----------------------------------------------------------------------------------------------------
        public void Reset()
        {
            jF(".Select_SortColumns").val("");
            if (this.Model.Descending)
                this.Descending_Click();
        }
        //----------------------------------------------------------------------------------------------------
        public ViewOptions_SortField GetCurrentModel()
        {
            this.Model.FieldName = jF(".Select_SortColumns").val().As<string>();

            if (this.Model.FieldName.Length < 1)
                return null;
            else
                return this.Model;
        }





        //------------------------------------------------------------------------------------------ Events --
        public void Descending_Click()
        {
            this.Model.Descending = !this.Model.Descending;
            if(this.Model.Descending)
            {
                jF(".Checkbox_Descending").attr("checked", true);
                jF(".Div_Descending").addClass("Div_Descending_On");
            }
            else
            {
                jF(".Checkbox_Descending").removeAttr("checked");
                jF(".Div_Descending").removeClass("Div_Descending_On");
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
                .ViewOptions_SortUI { padding-bottom: .2em; }
                .ViewOptions_SortUI .Select_SortColumns { float:left; width: 20em; max-width: 20em; }
                .ViewOptions_SortUI .Div_Descending { float:left; margin-left: .5em; padding: .2em .75em; cursor:pointer; border-radius: .3em; color: #b3c3dc; }
                .ViewOptions_SortUI .Div_Descending:hover { background: #ccc; }
                .ViewOptions_SortUI .Div_Descending_On { color: #093a8c; }
                .ViewOptions_SortUI .Div_Descending .Checkbox_Descending { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <select class='Select_SortColumns'>
                </select>
                <div class='Div_Descending' On_Click='Descending_Click'>
                    ( <input type='checkbox' class='Checkbox_Descending' /> descending )
                </div>
                <div class='clear'></div>
            ";
        }
    }
}
