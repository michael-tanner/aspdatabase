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
    public class HistorySummary
    {
        public int ConnectionId;
        public int TableId;
        public string ConnectionName;
        public string Schema;
        public string TableName;
        public string[] KeyValue;

        public int HistoryCount;
        public HistoryRecord[] HistoryRecords;
    }
}