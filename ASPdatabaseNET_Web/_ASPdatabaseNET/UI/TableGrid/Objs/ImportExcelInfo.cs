using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using OfficeOpenXml;

namespace ASPdatabaseNET.UI.TableGrid.Objs
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ImportExcelInfo
    {
        public string GuidKey;
        public int TableId;

        public DateTime UploadTime;
        public string FileName;
        public string ContentType;
        public int ContentLength;
        public ExcelPackage ExcelPackage;

        public string ExceptionMessage = null;
        public string ExceptionStacktrace = null;

        public ImportExcelInfo_TableColumn[] TableColumns;
        public ImportExcelInfo_Worksheet[] Worksheets;
    }
    
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ImportExcelInfo_Worksheet
    {
        public string WorksheetName;
        public ImportExcelInfo_WorksheetColumn[] Columns;
        public string CustomMapText;
    }
    //----------------------------------------------------------------------------------------------------////
    public class ImportExcelInfo_TableColumn
    {
        public string ColumnName;
        public bool IsPrimaryKey;
        public bool IsIdentity;
        public string DataType;
        public bool AllowNulls;
        public string DefaultValue_Orig;
        public string DefaultValue;

        public bool IsSelectedAsUniqueKey = false;
    }
    //----------------------------------------------------------------------------------------------------////
    public class ImportExcelInfo_WorksheetColumn
    {
        public string ColumnName;
        public int Index;
        public string MapTo = "";
    }
}