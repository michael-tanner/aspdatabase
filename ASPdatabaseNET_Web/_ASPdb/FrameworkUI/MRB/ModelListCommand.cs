using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdb.FrameworkUI.MRB
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ModelListCommand : jQueryContext
    {
        // ex: <select ... ModelList=' ViewModel.ExecuteFusionProOptions | Value | Text | Style '></select>
        public string ModelList_CommandText;

        public bool UseGetterMethod_Model_Name = false;
        public bool UseGetterMethod_Model_Collection = false;

        // below are set with default values
        public string Model_Name = "ViewModel";
        public string Model_Collection = "";
        public string Item_Value = "Value";
        public string Item_Text = "Text";
        public string Item_Style = "Style";


        //------------------------------------------------------------------------------------- constructor --
        public ModelListCommand()
        {
        }

        //----------------------------------------------------------------------------------------------------
        public void ParseCommandText()
        {
            var arr1 = this.ModelList_CommandText.As<JsString>().split('.');
            if (arr1.length < 2)
                return;

            string item1 = arr1[0].trim();
            string item2 = arr1[1];

            var arr2 = item2.As<JsString>().split('|');

            string item2a = arr2[0].trim();
            string item3 = "";
            string item4 = "";
            string item5 = "";

            if (arr2.length > 1) item3 = arr2[1].trim();
            if (arr2.length > 2) item4 = arr2[2].trim();
            if (arr2.length > 3) item5 = arr2[3].trim();

            // defaults if empty
            if (item1.Length < 1) item1 = "ViewModel";
            // -- no default for Model_CollectionProperty
            if (item3.Length < 1) item3 = "Value";
            if (item4.Length < 1) item4 = "Text";
            if (item5.Length < 1) item5 = "Style";

            this.Model_Name = item1;
            this.Model_Collection = item2a;
            this.Item_Value = item3;
            this.Item_Text = item4;
            this.Item_Style = item5;
        }

        //------------------------------------------------------------------------------------------ static --
        public static ModelListCommand New1(string modelList_CommandText)
        {
            var rtn = new ModelListCommand();
            rtn.ModelList_CommandText = modelList_CommandText;
            rtn.ParseCommandText();
            return rtn;
        }
    }
}