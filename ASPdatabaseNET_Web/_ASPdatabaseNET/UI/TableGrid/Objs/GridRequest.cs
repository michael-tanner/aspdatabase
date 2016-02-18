using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.UI.TableGrid.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class GridRequest
    {
        public enum TableTypes { NotSet, Table, View };

        public TableTypes TableType = TableTypes.NotSet;
        public int Id = -1;

        public int DisplayTopNRows = -1;

        public ViewOptions_FilterField[] FilterFields;
        public ViewOptions_SortField[] SortFields;
    }
}