using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.Record.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class RecordViewModel
    {
        public enum EditModes { Off, New, Edit, Clone, ReadOnly };

        public EditModes EditMode = EditModes.Off;
        public string UniqueRowKey;
        public RecordInfo RecordInfo = null;

        public UI.PageParts.TableDesign.AppProperties.Objs.AppPropertiesInfo AppPropertiesInfo;
    }
}