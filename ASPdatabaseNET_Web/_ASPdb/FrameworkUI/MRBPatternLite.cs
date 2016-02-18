//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using SharpKit.JavaScript;
//using SharpKit.jQuery;
//using SharpKit.Html;
//using ASPdb.FrameworkUI.MRB;

//namespace ASPdb.FrameworkUI
//{
//    //----------------------------------------------------------------------------------------------------////
//    [JsType(JsMode.Prototype, ASPdb.Config.SharpKitConfig.SharpKit_BuildPath)]
//    public class MRBPatternLite<M, VM> : jQueryContext, IMRBPattern<M, VM>
//    {
//        //------------------------------
//        protected ModelBindingHelper<M, VM> ModelBinder;

//        //------------------------------
//        public M Model { get; set; }
//        public VM ViewModel { get; set; }

//        //------------------------------
//        public jQuery jRoot { get; set; }

//        //------------------------------
//        //public JsEvent_BeforeAfter OnChange = new JsEvent_BeforeAfter();

//        public void Open() { }
//        public void Close() { }

//        //------------------------------------------------------------------------------------- constructor --
//        public MRBPatternLite()
//        {
//            this.ModelBinder = new ModelBindingHelper<M, VM>(this);
//        }

//        //----------------------------------------------------------------------------------------------------
//        public void BindUI()
//        {
//            if (ModelBinder == null)
//                return;

//            this.ModelBinder.Bind_ModelToUI(this, "*");
//        }
//        //----------------------------------------------------------------------------------------------------
//        public void BindUI_Single(string modelKey)
//        {
//            if (ModelBinder == null)
//                return;

//            this.ModelBinder.Bind_ModelToUI(this, modelKey);
//        }




//        //----------------------------------------------------------------------------------------------------
//        public M GetModelWithoutFiringEvents()
//        {
//            return this.Model;
//        }
//        //----------------------------------------------------------------------------------------------------
//        public jQuery jF(string selector)
//        {
//            try
//            {
//                return jRoot.find(selector);
//            }
//            catch
//            {
//                return null;
//            }
//        }
//        //----------------------------------------------------------------------------------------------------
//        /// <summary>Excludes children under .jRoot ... This method needs to be researched & optimized more.</summary>
//        public jQuery jF_ThisRootOnly(string selector)
//        {
//            try
//            {
//                var list_ToExclude = this.jF(".jRoot").find(selector);
//                var list_AllItems = this.jF(selector);

//                int k = 0;
//                var list_Filtered = new HtmlElement[0];
//                for (int i = 0; i < list_AllItems.length; i++)
//                {
//                    bool keepItem = true;
//                    for (int j = 0; j < list_ToExclude.length; j++)
//                    {
//                        if (list_ToExclude[j] == list_AllItems[i])
//                        {
//                            keepItem = false;
//                            j = list_ToExclude.length + 1;
//                        }
//                    }
//                    if (keepItem)
//                        list_Filtered[k++] = list_AllItems[i];
//                }
//                return J(list_Filtered);
//            }
//            catch
//            {
//                return null;
//            }
//        }


//        //----------------------------------------------------------------------------------------------------
//        public void Instantiate()
//        {
//            this.Instantiate_Sub();
//            this.ConnectEvents();
//        }
//        //--------------------------------------------------
//        private void Instantiate_Sub()
//        {
//        }



//        //----------------------------------------------------------------------------------------------------
//        public void ConnectEvents()
//        {
//            this.ConnectEvents_Sub();
//        }
//        //--------------------------------------------------
//        private void ConnectEvents_Sub()
//        {
//        }
//        //--------------------------------------------------
//        protected void BindEvents()
//        {
//            ModelBinding_EventsHelper<M, VM>.BindEvents(this);
//        }



//        //-------------------------------------------------------------------------------------- CSS & HTML --
//        public static string GetCssTree()
//        {
//            string rtn = "";
//            rtn += GetCssRoot();
//            return rtn;
//        }
//        //----------------------------------------------------------------------------------------------------
//        public static string GetCssRoot()
//        {
//            return "";
//        }
//        //----------------------------------------------------------------------------------------------------
//        public string GetHtmlRoot()
//        {
//            return "";
//        }

//    }
//}