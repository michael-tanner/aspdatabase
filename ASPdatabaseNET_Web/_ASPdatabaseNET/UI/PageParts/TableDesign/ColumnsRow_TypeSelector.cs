using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ColumnsRow_TypeSelector : MRBPattern<string, TableDesign_ViewModel>
    {
        public ColumnsRow_TypeSelectorItem[] TypeSelectorItems;
        public bool MouseIsOver = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ColumnsRow_TypeSelector()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ColumnsRow_TypeSelector jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            var thisObj = this;
            var jRootObj = this.jRoot;
            eval("jRootObj.mouseover(function(){ thisObj.MouseIsOver = true; });");
            eval("jRootObj.mouseout(function(){ thisObj.MouseIsOver = false; });");
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if(this.TypeSelectorItems == null)
            {
                string[] typesArr = new string[] { "int", "bigint", "bit", "datetime", "--", "nvarchar(50)", "ntext", "--", "decimal(18, 0)", "float", "money", "--" };
                if(this.ViewModel.IsCreateNew)
                    typesArr = new string[] { "int - Identity(1,1)", "bigint - Identity(1,1)", "--", "int", "bit", "datetime", "--", "nvarchar(50)", "ntext", "--", "decimal(18, 0)", "float", "money", "--" };

                this.TypeSelectorItems = new ColumnsRow_TypeSelectorItem[0];
                var holder = jF(".Holder_Items").html("");
                for (int i = 0; i < typesArr.Length; i++)
                {
                    this.TypeSelectorItems[i] = new ColumnsRow_TypeSelectorItem();
                    this.TypeSelectorItems[i].Model = typesArr[i];
                    this.TypeSelectorItems[i].Instantiate();
                    this.TypeSelectorItems[i].OnChange.After.AddHandler(this, "Item_OnChange", 1);
                    holder.append(this.TypeSelectorItems[i].jRoot);
                }
            }
            
        }

        //----------------------------------------------------------------------------------------------------
        public void DelayClose()
        {
            var thisObj = this;
            if (this.MouseIsOver)
                eval("setTimeout(function(){ thisObj.Close(); }, 150);");
            else
                this.Close();
        }





        //------------------------------------------------------------------------------------------ Events --
        public void Item_OnChange(string value)
        {
            this.Model = value;
            this.OnChange.After.Fire();
        }






        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += ColumnsRow_TypeSelectorItem.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ColumnsRow_TypeSelector { width: 10em; background: #f5f5f5; border: 1px solid #99bbf2; box-shadow: 1px 1px 5px #555; }
                .ColumnsRow_TypeSelector .Holder_Items { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Holder_Items'></div>
            ";
        }
    }
}
