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
    public class ModelBinding_EventsHelper<M, VM> : jQueryContext
    {

        //------------------------------------------------------------------------------------------ static --
        public static void BindEvents(IMRBPattern<M, VM> mrb)
        {
            GenericBinder(mrb, "On_Click", "click");
            GenericBinder(mrb, "On_Change", "change");
            GenericBinder(mrb, "On_Focus", "focus");
            GenericBinder(mrb, "On_Blur", "blur");
        }

        //------------------------------------------------------------------------------------------ static --
        private static void GenericBinder(IMRBPattern<M, VM> mrb, string methodPropertyName, string eventType)
        {
            var onChangeItems = mrb.jF2("[" + methodPropertyName + "]");
            for (int i = 0; i < onChangeItems.length; i++)
            {
                var item = onChangeItems[i];
                string method = J(item).attr(methodPropertyName);
                Evt.Attach_ToElement(eventType, mrb, item, method, null);
            }
        }



    }
}