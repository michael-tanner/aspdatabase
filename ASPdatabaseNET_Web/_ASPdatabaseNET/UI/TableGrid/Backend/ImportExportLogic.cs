using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using ASPdatabaseNET.UI.TableGrid.Objs;

namespace ASPdatabaseNET.UI.TableGrid.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class ImportExportLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static void Do_ExcelDownload(string gridRequestBase64)
        {
            try
            {
                byte[] bytes = Convert.FromBase64String(gridRequestBase64);
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                var gridRequest = (new ASPdb.Ajax.AjaxHelper()).FromJson<UI.TableGrid.Objs.GridRequest>(json);
                gridRequest.DisplayTopNRows = -2; // get all
                var gridResponse = TableGridLogic.GetGrid(gridRequest, false);

                string fullTableName = gridResponse.Schema + "." + gridResponse.TableName;
                if(fullTableName.Contains(" "))
                    fullTableName = "[" + gridResponse.Schema + "].[" + gridResponse.TableName + "]";

                var package = new ExcelPackage();
                package.Workbook.Worksheets.Add(fullTableName);
                var worksheet1 = package.Workbook.Worksheets.Last();
                worksheet1.View.FreezePanes(2, 1);

                int c = 1;
                foreach (var item in gridResponse.HeaderItems)
                {
                    worksheet1.Cells[1, c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet1.Cells[1, c].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 212, 218));
                    worksheet1.Cells[1, c].Value = item.FieldName;
                    worksheet1.Column(c).AutoFit(12);
                    c++;
                }
                int r = 2;
                foreach (var row in gridResponse.Rows)
                {
                    c = 1;
                    foreach (string value in row.Values)
                    {
                        worksheet1.Cells[r, c].Value = value;
                        c++;
                    }
                    r++;
                }

                package.Workbook.Worksheets.Add("Info");
                var worksheet2 = package.Workbook.Worksheets.Last();
                r = 1;
                c = 1;
                worksheet2.Cells[r, c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet2.Cells[r, c].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 212, 218));
                worksheet2.Cells[r, c++].Value = "";
                worksheet2.Cells[r, c].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet2.Cells[r, c].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(207, 212, 218));
                worksheet2.Cells[r, c++].Value = "";
                r++;
                c = 1;
                var userInfo = Users.UserSessionLogic.GetUser().UserInfo;
                worksheet2.Cells[r, c++].Value = "Downloaded By";
                worksheet2.Cells[r, c++].Value = String.Format("{0} {1} ({2})", userInfo.FirstName, userInfo.LastName, userInfo.Username).Trim();
                r++;
                c = 1;
                worksheet2.Cells[r, c++].Value = "Download Time";
                worksheet2.Cells[r, c++].Value = DateTime.Now.ToString();
                r++;
                c = 1;
                string tableOrViewName = "Table Name";
                if (gridRequest.TableType == GridRequest.TableTypes.View) tableOrViewName = "View Name";
                worksheet2.Cells[r, c++].Value = tableOrViewName;
                worksheet2.Cells[r, c++].Value = fullTableName;
                r++;
                c = 1;
                worksheet2.Cells[r, c++].Value = "ConnectionId";
                worksheet2.Cells[r, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet2.Cells[r, c++].Value = gridResponse.ConnectionId;
                r++;
                c = 1;
                worksheet2.Cells[r, c++].Value = "Column Count";
                worksheet2.Cells[r, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet2.Cells[r, c++].Value = gridResponse.HeaderItems.Length;
                r++;
                c = 1;
                worksheet2.Cells[r, c++].Value = "Row Count";
                worksheet2.Cells[r, c].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                worksheet2.Cells[r, c++].Value = gridResponse.Rows.Length;
                r++;
                c = 1;
                r++;
                c = 1;
                worksheet2.Cells[r, c++].Value = "Download Application";
                worksheet2.Cells[r, c].Style.Font.Color.SetColor(System.Drawing.Color.FromArgb(27, 91, 167));
                worksheet2.Cells[r, c++].Value = "ASPdatabase.NET";
                
                for (int i = 1; i < 3; i++)
                    worksheet2.Column(i).AutoFit(16);

                string fileName = gridResponse.TableName;
                byte[] excelBytes = package.GetAsByteArray();
                HttpContext.Current.Response.Buffer = true;
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet ";
                HttpContext.Current.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename=\"{0}.xlsx\"; size={1};", fileName, excelBytes.Length.ToString()));
                HttpContext.Current.Response.BinaryWrite(excelBytes);
                //HttpContext.Current.Response.End(); // This causes an error in the Excel file.              
            }
            catch(Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                HttpContext.Current.Response.Write("An error occurred in the Excel Downloader: " + exc.Message);
            }
        }


        //----------------------------------------------------------------------------------------------------
        public static void Do_ExcelUpload(string guidKey, int tableId)
        {
            ImportExportLogic.CleanExpiredMemory();

            var importExcelInfo = new ImportExcelInfo() { GuidKey = guidKey, UploadTime = DateTime.Now, TableId = tableId };
            var dict = GetMemoryDict();
            if (!dict.ContainsKey(guidKey))
                dict.Add(guidKey, importExcelInfo);
            else 
                throw new Exception("GuidKey '" + guidKey + "' already in memory.");

            string s = "";
            try
            {
                if (String.IsNullOrEmpty(guidKey))
                    throw new Exception("GuidKey is null or empty");
                if (HttpContext.Current.Request.Files.Count < 1)
                    throw new Exception("No file attached");

                var fileData = HttpContext.Current.Request.Files[0];

                if (!fileData.FileName.ToLower().EndsWith(".xlsx"))
                    throw new Exception("Must be .xlsx Excel file.");

                importExcelInfo.FileName = fileData.FileName;
                importExcelInfo.ContentType = fileData.ContentType;
                importExcelInfo.ContentLength = fileData.ContentLength;
                importExcelInfo.ExcelPackage = new ExcelPackage(fileData.InputStream);

                s += String.Format(@"
                    FileName: {0}<br />
                    ContentType: {1}<br />
                    ContentLength: {2}<br />
                    ", fileData.FileName, fileData.ContentType, fileData.ContentLength);

            }
            catch(Exception exc)
            {
                ASPdb.Framework.Debug.RecordException(exc);
                importExcelInfo.ExceptionMessage = exc.Message;
                importExcelInfo.ExceptionStacktrace = exc.StackTrace;
            }

            HttpContext.Current.Response.Write(s);
        }

        //----------------------------------------------------------------------------------------------------
        private static string AppKey = "ASPdatabaseNET.UI.TableGrid.Backend.ImportExportLogic";
        public static Dictionary<string, ImportExcelInfo> GetMemoryDict()
        {
            Dictionary<string, ImportExcelInfo> rtn = null;
            try
            {
                rtn = (Dictionary<string, ImportExcelInfo>)HttpContext.Current.Application[AppKey];
            }
            catch { }
            if(rtn == null)
            {
                rtn = new Dictionary<string, ImportExcelInfo>();
                HttpContext.Current.Application[AppKey] = rtn;
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static ImportExcelInfo GetFileFromMemory(string guidKey)
        {
            var dict = GetMemoryDict();
            if (dict.ContainsKey(guidKey))
                return dict[guidKey];
            else
                return null;
        }
        //----------------------------------------------------------------------------------------------------
        public static ImportExcelInfo Get_ImportExcelInfo_ForClient(string guidKey)
        {
            var obj = GetFileFromMemory(guidKey);
            if (obj == null)
                return null;
            var rtn = new ImportExcelInfo()
            {
                GuidKey = obj.GuidKey,
                TableId = obj.TableId,
                UploadTime = obj.UploadTime,
                FileName = obj.FileName,
                ContentLength = obj.ContentLength,
                ContentType = obj.ContentType,
                ExceptionMessage = obj.ExceptionMessage,
                ExceptionStacktrace = obj.ExceptionStacktrace,
                ExcelPackage = null
            };

            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(rtn.TableId, false);
            var tableColumns = new List<ImportExcelInfo_TableColumn>();
            foreach(var column in tableStructure.Columns)
            {
                tableColumns.Add(new ImportExcelInfo_TableColumn()
                {
                    ColumnName = column.ColumnName,
                    IsSelectedAsUniqueKey = column.IsPrimaryKey,
                    IsPrimaryKey = column.IsPrimaryKey,
                    IsIdentity = column.IsIdentity,
                    DataType = column.DataType,
                    AllowNulls = column.AllowNulls,
                    DefaultValue = column.DefaultValue,
                    DefaultValue_Orig = column.DefaultValue
                });
            }
            rtn.TableColumns = tableColumns.ToArray();

            var worksheets = new List<ImportExcelInfo_Worksheet>();
            foreach (var item in obj.ExcelPackage.Workbook.Worksheets)
            {
                var wColumns = new List<ImportExcelInfo_WorksheetColumn>();
                int emptyColCount = 0;
                for (int i = 1; i <= item.Cells.Count(); i++)
                {
                    bool columnAdded = false;
                    try
                    {
                        string value = item.Cells[1, i].Value.ToString();
                        if (value != null && value.Trim() != "")
                        {
                            wColumns.Add(new ImportExcelInfo_WorksheetColumn()
                            {
                                Index = i,
                                ColumnName = value.Trim()
                            });
                            emptyColCount = 0;
                            columnAdded = true;
                        }
                    }
                    catch { }
                    if (!columnAdded) emptyColCount++;
                    if (emptyColCount > 10) break;
                }
                worksheets.Add(new ImportExcelInfo_Worksheet()
                {
                    WorksheetName = item.Name,
                    Columns = wColumns.ToArray()
                });
            }
            rtn.Worksheets = worksheets.ToArray();

            foreach (var worksheet in rtn.Worksheets)
                FindMatches(worksheet.Columns, rtn.TableColumns);

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public class MatchHolder
        {
            public string LeftName;
            public string MatchName;
            public string RightName;
        }
        private static void FindMatches(ImportExcelInfo_WorksheetColumn[] worksheetColumns, ImportExcelInfo_TableColumn[] tableColumns)
        {
            var unmatched_Left = (from r in worksheetColumns select new MatchHolder() { LeftName = r.ColumnName.Trim(), MatchName = r.ColumnName.Trim().ToLower() }).ToList();
            var unmatched_Right = (from r in tableColumns select new MatchHolder() { RightName = r.ColumnName.Trim(), MatchName = r.ColumnName.Trim().ToLower() }).ToList();
            var matched = new List<MatchHolder>();

            if (unmatched_Left.Count > 0) // Pass 1
            {
                FindMatches__Helper(unmatched_Left, unmatched_Right, matched);
            }
            if (unmatched_Left.Count > 0) // Pass 2
            {
                foreach (var item in unmatched_Left)
                    item.MatchName = item.MatchName.Replace(" ", "").Replace("_", "").Replace(".", "").Replace("-", "").Replace("&", "and");
                foreach (var item in unmatched_Right)
                    item.MatchName = item.MatchName.Replace(" ", "").Replace("_", "").Replace(".", "").Replace("-", "").Replace("&", "and");

                FindMatches__Helper(unmatched_Left, unmatched_Right, matched);
            }

            var dict = new Dictionary<string, string>();
            foreach (var item in matched)
                if (!dict.ContainsKey(item.LeftName))
                    dict.Add(item.LeftName, item.RightName);
            foreach (var item in worksheetColumns)
                if (dict.ContainsKey(item.ColumnName))
                    item.MapTo = dict[item.ColumnName];
        }
        private static void FindMatches__Helper(List<MatchHolder> unmatched_Left, List<MatchHolder> unmatched_Right, List<MatchHolder> matched)
        {
            var dict = new Dictionary<string, MatchHolder>();
            foreach (var item in unmatched_Right)
                if (!dict.ContainsKey(item.MatchName))
                    dict.Add(item.MatchName, item);

            var unmatched_Left2 = new List<MatchHolder>();
            foreach (var left in unmatched_Left)
                unmatched_Left2.Add(left);

            foreach (var left in unmatched_Left2)
                if (dict.ContainsKey(left.MatchName))
                {
                    var right = dict[left.MatchName];
                    unmatched_Left.Remove(left);
                    unmatched_Right.Remove(right);
                    matched.Add(right);
                    right.LeftName = left.LeftName;
                }
        }


        //----------------------------------------------------------------------------------------------------
        public static void CleanExpiredMemory()
        {
            var dict = GetMemoryDict();
            var keysToRemove = new List<string>();
            foreach (var item in dict.Values)
                if (item.UploadTime.AddMinutes(60) < DateTime.Now)
                    keysToRemove.Add(item.GuidKey);
            foreach (var key in keysToRemove)
                if (dict.ContainsKey(key))
                    dict.Remove(key);
        }
    }

}