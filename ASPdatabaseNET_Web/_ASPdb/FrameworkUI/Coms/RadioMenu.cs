using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdb.FrameworkUI.Coms
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class RadioMenu : MRBPattern<string, MRB.GenericUIList>
    {
        public string RadioGroupName;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public RadioMenu(string radioGroupName)
        {
            this.RadioGroupName = radioGroupName;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Coms_RadioMenu jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        public void OnModel_Set()
        {
            if (this.IsOpen)
                this.UpdateUI();
        }

        //----------------------------------------------------------------------------------------------------
        public void Open_Sub()
        {
            this.jRoot.html("");
            if (this.ViewModel == null || this.ViewModel.List == null || this.ViewModel.List.Length < 1)
                return;

            for (int i = 0; i < this.ViewModel.List.Length; i++)
            {
                var item = this.ViewModel.List[i];
                string indexClass = "ItemIndex_" + i;

                string html = JsStr.StrFormat4(@"
                    <div class='Item {0}'>
                        <input type='radio' name='{1}' value='{2}' /> 
                        {3}
                    </div>",
                    indexClass, this.RadioGroupName, item.Value, item.Text);
                this.jRoot.append(html);
                Evt.Attach_Click(this, indexClass, "Div_Click", i);
            }
            this.UpdateUI();
        }
        //----------------------------------------------------------------------------------------------------
        public void UpdateUI()
        {
            jF2(".Item").removeClass("Selected");
            jF2(".Item input").attr("checked", false);
            for (int i = 0; i < this.ViewModel.List.Length; i++)
            {
                var item = this.ViewModel.List[i];
                if (item.Value == this.Model)
                {
                    var div = jF2(".ItemIndex_" + i);
                    div.addClass("Selected");
                    div.find("input").attr("checked", true);

                    i = this.ViewModel.List.Length + 1; // end loop
                }
            }
        }

        //------------------------------------------------------------------------------------------ Events --
        public void Div_Click(EventInfo eventInfo)
        {
            this.OnChange.Before.Fire();
            var index = eventInfo.PassThruDataObj.As<JsNumber>();
            this.Model = this.ViewModel.List[index].Value;
            this.UpdateUI();
            this.OnChange.After.Fire();
        }

        //----------------------------------------------------------------------------------------------------
        public string GetSelectedValue()
        {
            // consider this instead: $("input[name=radioName]:checked")

            var items = jF("input[type=radio]");
            for (int i = 0; i < items.length; i++)
            {
                var item = J(items[i]);
                if (item.attr("checked") == "checked")
                    return item.val().As<string>();
            }
            return null;
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
                .Coms_RadioMenu { }
                ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"";
        }

    }
}