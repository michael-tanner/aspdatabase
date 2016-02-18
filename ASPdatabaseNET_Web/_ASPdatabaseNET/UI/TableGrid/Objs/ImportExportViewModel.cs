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
    public class ImportExportViewModel
    {
        public string GuidKey = "";

        public GridRequest GridRequest;

        public ImportExcelInfo_Worksheet SelectedWorksheet;

        public bool IsInDemoMode;
    }
}