using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.PageParts.Record.Objs_History
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class HistoryRecord
    {
        public enum HistoryTypes { NotSet, Insert, Update, Delete };

        public int HistoryId;
        public int TableId;
        public string[]  KeyValue;
        public int Revision;
        public HistoryTypes HistoryType = HistoryTypes.NotSet;
        public bool IsPartial;

        public DateTime TimeSaved;
        public string TimeSaved_String;
        public int ByUserId;
        public string ByUsername;

        public HistoryJsonObj HistoryJsonObj;

        public Item_3Values[] Fields_3ValArr;
        public string TimeLapsedString;
    }
}