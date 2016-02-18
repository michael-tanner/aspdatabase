using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.TableGrid.Objs;

namespace ASPdatabaseNET.UI.TableGrid
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ImportExportUI_UploadExcel : MRBPattern<ImportExcelInfo, ImportExportViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ImportExportUI_UploadExcel()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ImportExportUI_UploadExcel jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            var thisObj = this;
            var jObj = this.jRoot;

            // http://html5doctor.com/drag-and-drop-to-server/
            // http://stackoverflow.com/questions/166221/how-can-i-upload-files-asynchronously-with-jquery

            var dragFileHolder = jF(".DragFileHolder");
            eval("dragFileHolder[0].ondragover = function() { thisObj.DragFileHolder_ondragover(this); return false; };");
            eval("dragFileHolder[0].ondragleave = function() { thisObj.DragFileHolder_ondragend(this); return false; };");
            eval("dragFileHolder[0].ondragend = function() { thisObj.DragFileHolder_ondragend(this); return false; };");
            eval("dragFileHolder[0].ondrop = function (e) { e.preventDefault && e.preventDefault(); thisObj.DragFileHolder_ondrop(event); return false; };");

            eval("jObj.find('.FileInput').change(function(){ thisObj.FileInput_Change(); });");
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if(this.ViewModel.IsInDemoMode)
            {
                jF(".Screen").hide();
                jF(".Screen0").show();
                return;
            }

            this.Model = null;
            jF(".DropText").html(jF(".DropText").attr("HtmlText"));
            jF(".ProgressControl").hide();
            jF(".Screen").hide();
            jF(".Screen1").show();
            jF(".Bttn1").hide();
            AjaxService.ASPdatabaseService.New(this, "Guid_Return").Guid__GetNewGuidString();
        }
        public void Guid_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.GuidKey = ajaxResponse.ReturnObj.As<string>();
            jF(".Bttn1").show();
        }




        //------------------------------------------------------------------------------------------ Events --
        public void Next1_Click()
        {
            jF(".Screen").hide();
            jF(".Screen2").show();

            jF(".ProgressControl").hide();
            jF(".SendingFile").hide();
            jF(".TwoOptionTable").show();
        }
        //----------------------------------------------------------------------------------------------------
        public void DragFileHolder_ondragover(HtmlElement elementObj)
        {
            jF(".DragFileHolder").addClass("DragFileHolder_Hover");
            jF(".DropText").html("&nbsp;");
        }
        public void DragFileHolder_ondragend(HtmlElement elementObj)
        {
            jF(".DropText").html(jF(".DropText").attr("HtmlText"));
            jF(".DragFileHolder").removeClass("DragFileHolder_Hover");
        }
        public void DragFileHolder_ondrop(Event evt)
        {
            jF(".DragFileHolder").removeClass("DragFileHolder_Hover");

            SharpKit.Html.fileapi.File file = null;
            eval(@"
                if(evt.dataTransfer.files != null && evt.dataTransfer.files.length > 0)
                    file = evt.dataTransfer.files[0];
                ");
            this.SendFileData(file, this.ViewModel.GuidKey);
        }
        //----------------------------------------------------------------------------------------------------
        public void FileInput_Change()
        {
            var file = jF(".FileInput")[0].As<HtmlInputElement>().files[0];
            this.SendFileData(file, this.ViewModel.GuidKey);
        }
        //----------------------------------------------------------------------------------------------------
        private void SendFileData(SharpKit.Html.fileapi.File file, string guidKey)
        {
            if (file == null)
            {
                alert("File was not attached.");
                return;
            }

            var arr = file.name.split(".");
            var ext = arr[arr.length - 1].toLowerCase().trim();
            if (ext != "xlsx")
            {
                alert("Please attach an Excel file in the .xlsx format.");
                return;
            }

            var progress = jF(".ProgressControl");
            var thisObj = this;
            eval(@"
            var formData = new FormData();
            formData.append('file', file);

            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'ASPdatabase.NET.aspx?Upload=Excel&Key=" + guidKey + @"&TableId=" + this.ViewModel.GridRequest.Id + @"');

            progress[0].value = 0;
            progress.show();
            xhr.upload.onprogress = function (event) 
            {
                if (event.lengthComputable) 
                {
                    var complete = (event.loaded / event.total * 100 | 0);
                    progress[0].value = progress.innerHTML = complete;
                }
            };
            xhr.onload = function () 
            {
                progress[0].value = progress.innerHTML = 100;
                if(xhr.status == 200)
                    thisObj.UploadComplete();
                else
                    alert('An error occurred during upload. \nxhr.status: ' + xhr.status);
            };
            xhr.send(formData);
            ");
            jF(".TwoOptionTable").hide();
            jF(".SendingFile").show();
        }
        //----------------------------------------------------------------------------------------------------
        public void UploadComplete()
        {
            jF(".Screen").hide();

            AjaxService.ASPdatabaseService.New(this, "GetUploadedExcelInfo_Return")
                .ImportExport__GetUploadedExcelInfo(this.ViewModel.GuidKey);
        }
        public void GetUploadedExcelInfo_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            this.Model = ajaxResponse.ReturnObj.As<ImportExcelInfo>();

            if(this.Model.Worksheets.Length < 1)
            {
                alert("No worksheets were found in this Excel file");
                this.Close();
            }
            else if(this.Model.Worksheets.Length > 1)
            {
                this.Populate_Screen_SelectWorksheet();
                jF(".Screen_SelectWorksheet").show();
                jF(".Bttn_Prev3").show();
            }
            else
            {
                this.Populate_Screen_SelectKeys();
                jF(".Bttn_Prev3").hide();
                jF(".Screen_SelectKeys").show();
                this.ViewModel.SelectedWorksheet = this.Model.Worksheets[0];
            }
        }
        //---------------------------------------------------------------------------------------------------- On Screen_SelectWorksheet
        public void Next2_Click()
        {
            this.Populate_Screen_SelectKeys();
            jF(".Screen").hide();
            jF(".Screen_SelectKeys").show();
        }
        //---------------------------------------------------------------------------------------------------- On Screen_SelectKeys
        public void Prev3_Click()
        {
            jF(".Screen").hide();
            jF(".Screen_SelectWorksheet").show();
        }
        public void Next3_Click()
        {
            jF(".Screen").hide();
            jF(".Screen_Mappings").show();
        }
        //---------------------------------------------------------------------------------------------------- On Screen_Mappings
        public void Prev4_Click()
        {
            jF(".Screen").hide();
            jF(".Screen_SelectKeys").show();
        }
        public void Next4_Click()
        {

        }

        //----------------------------------------------------------------------------------------------------
        private void Populate_Screen_SelectWorksheet()
        {
            jF(".Bttn_Next2").hide();
            var div = jF(".WorksheetListDiv").html("");
            for (int i = 0; i < this.Model.Worksheets.Length; i++)
            {
                var item = this.Model.Worksheets[i];
                div.append(st.New("<div class='WorksheetItem' Index='{0}'>{1}</div>")
                    .Format2(i.As<string>(), item.WorksheetName).TheString);
            }
            var thisObj = this;
            eval("div.find('.WorksheetItem').click(function(){ thisObj.WorksheetItem_Click(this); });");
        }
        public void WorksheetItem_Click(HtmlElement element)
        {
            var worksheetItem = J(element);
            int index = worksheetItem.attr("Index").As<int>();
            this.ViewModel.SelectedWorksheet = this.Model.Worksheets[index];

            jF(".WorksheetItem").removeClass("WorksheetItem_On");
            worksheetItem.addClass("WorksheetItem_On");
            jF(".Bttn_Next2").show();
        }
        //----------------------------------------------------------------------------------------------------
        private void Populate_Screen_SelectKeys()
        {
            var div = jF(".SelectKeysList").html("");
            for (int i = 0; i < this.Model.TableColumns.Length; i++)
            {
                var item = this.Model.TableColumns[i];
                string onClass = "";
                if (item.IsSelectedAsUniqueKey)
                    onClass = "KeyItem_On";
                div.append(st.New("<div class='KeyItem {0}' Index='{1}'>{2}</div>")
                    .Format3(onClass, i.As<string>(), item.ColumnName).TheString);
            }
            var thisObj = this;
            eval("div.find('.KeyItem').click(function(){ thisObj.KeyItem_Click(this); });");
        }
        public void KeyItem_Click(HtmlElement element)
        {
            var keyItem = J(element);
            int index = keyItem.attr("Index").As<int>();

            var column = this.Model.TableColumns[index];
            column.IsSelectedAsUniqueKey = !column.IsSelectedAsUniqueKey;

            if(column.IsSelectedAsUniqueKey)
            {
                keyItem.addClass("KeyItem_On");
            }
            else
            {
                keyItem.removeClass("KeyItem_On");
            }
        }
        //----------------------------------------------------------------------------------------------------
        private void Populate_Screen_Mappings()
        {

        }



        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ImportExportUI_UploadExcel { }


                .ImportExportUI_UploadExcel .Label_UploadExcel { position:relative; line-height: 2.1875em; background: #2d925c; color: #fff; 
                                                                 text-align: center; cursor:default; }
                .ImportExportUI_UploadExcel .Label_UploadExcel .Icon { position:absolute; height: 2.1875em; width: 2.5625em; background: #257e50;
                                                                       background: url('ASPdatabase.NET.aspx?IMG=Sprite1'); }
                .ImportExportUI_UploadExcel .Label_UploadExcel span { font-size: .9em; padding-left: 1.42em; }

                .ImportExportUI_UploadExcel .Bttn { cursor:pointer; background: #777; color: #fff; padding: 0em 1.5em; line-height: 2.1875em; }
                .ImportExportUI_UploadExcel .Bttn:hover { background: #444; }
                .ImportExportUI_UploadExcel .Bttn_Left { float:left; }
                .ImportExportUI_UploadExcel .Bttn_Right { float:right; }

                .ImportExportUI_UploadExcel .Screen { display:none; font-size: .8em; padding: 1em 0em; }
                .ImportExportUI_UploadExcel .Screen1 { }
                .ImportExportUI_UploadExcel .Screen2 { }



                .ImportExportUI_UploadExcel .Screen2 { }
                .ImportExportUI_UploadExcel .Screen2 table { width: 100%; height: 12em; }
                .ImportExportUI_UploadExcel .Screen2 table td { }
                .ImportExportUI_UploadExcel .Screen2 table .Td1 { width: 44%; }
                .ImportExportUI_UploadExcel .Screen2 table .Td2 { width: 3%; font-size: 1px; }
                .ImportExportUI_UploadExcel .Screen2 table .Td3 { width: 6%; text-align:center; vertical-align:middle; background: #b3e6cb; }
                .ImportExportUI_UploadExcel .Screen2 table .Td4 { width: 3%; font-size: 1px; }
                .ImportExportUI_UploadExcel .Screen2 table .Td5 { width: 44%; text-align:center; vertical-align:middle; background: #f3f3f3; }
                .ImportExportUI_UploadExcel .Screen2 .DragFileHolder { border: .5em dashed #ccc; text-align:center; line-height: 12em; }
                .ImportExportUI_UploadExcel .Screen2 .DragFileHolder .DropText { }
                .ImportExportUI_UploadExcel .Screen2 .DragFileHolder_Hover { border-color: #3f6852; background: #eee; background: url('ASPdatabase.NET.aspx?IMG=Sprite1') -45px -117px; }
                .ImportExportUI_UploadExcel .Screen2 .FileInput { width: 200px; }
                .ImportExportUI_UploadExcel .Screen2 progress { margin-top: 3em; margin-bottom: 3em; width: 100%; }

                .ImportExportUI_UploadExcel .WorksheetListDiv { max-height: 15em; overflow-y: auto; }
                .ImportExportUI_UploadExcel .WorksheetListDiv .WorksheetItem { background: #f4f4f4; border-bottom: .15em solid #fff; cursor:pointer; padding: .3em 1em; }
                .ImportExportUI_UploadExcel .WorksheetListDiv .WorksheetItem:hover { background: #ddd; color: #000; }
                .ImportExportUI_UploadExcel .WorksheetListDiv .WorksheetItem_On { background: #228fe1; color: #fff; }

                .ImportExportUI_UploadExcel .SelectKeysList { max-height: 15em; overflow-y: auto; }
                .ImportExportUI_UploadExcel .SelectKeysList .KeyItem { background: #f4f4f4; border-bottom: .15em solid #fff; cursor:pointer; padding: .3em 1em; }
                .ImportExportUI_UploadExcel .SelectKeysList .KeyItem:hover { background: #ddd; color: #000; }
                .ImportExportUI_UploadExcel .SelectKeysList .KeyItem_On { background: #228fe1; color: #fff; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Label_UploadExcel'>
                    <div class='Icon'></div>
                    <span>Upload Excel</span>
                </div>

                <div class='Screen Screen0'>
                    <br />
                    This feature is disabled in demo mode.
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                    <div class='clear'></div>
                </div>

                <div class='Screen Screen1'>
                    Please make sure Excel file is in .xlsx format.<br />
                    Older .xls format is not supported.<br />
                    <br />
                    The top row of Excel file should have the field names header for matching to this table.<br />
                    <br />
                    You can set custom mappings in one of the following steps.
                    <br />
                    <br />
                    <div class='Bttn Bttn_Right Bttn1' On_Click='Next1_Click'>Next &gt;&gt;</div>
                    <div class='clear'></div>
                </div>

                <div class='Screen Screen2'>
                    After your file is uploaded we'll show you some options before saving the Excel data.
                    <br />
                    <br />
                    <table class='TwoOptionTable'>
                        <tr>
                            <td class='Td1'>
                                <div class='DragFileHolder'>
                                    <div class='DropText' HtmlText='Drop .xlsx File Here'>&nbsp;</div>
                                </div>
                            </td>
                            <td class='Td2'>&nbsp;</td>
                            <td class='Td3'>or</td>
                            <td class='Td4'>&nbsp;</td>
                            <td class='Td5'>
                                <form enctype='multipart/form-data'>
                                    <input name='file' type='file' class='FileInput' />
                                </form>
                            </td>
                        </tr>
                    </table>

                    <div class='SendingFile'>
                        <br />
                        <br />
                        Sending Excel File . . .
                        <br />
                        <br />
                    </div>

                    <progress class='ProgressControl'></progress>
                    <div class='DivA'></div>
                </div>

                <div class='Screen Screen_SelectWorksheet'>
                    Select Excel Worksheet
                    <br />
                    <br />
                    <div class='WorksheetListDiv'></div>
                    <br />
                    <br />
                    <div class='Bttn Bttn_Next2 Bttn_Right' On_Click='Next2_Click'>Next &gt;&gt;</div>
                    <div class='clear'></div>
                </div>
                <div class='Screen Screen_SelectKeys'>
                    Select Unique Identifiers for Excel Import
                    <br />
                    <br />
                    <div class='SelectKeysList'></div>
                    <br />
                    <br />
                    <div class='Bttn Bttn_Left Bttn_Prev3' On_Click='Prev3_Click'>&lt;&lt; Previous</div>
                    <div class='Bttn Bttn_Right' On_Click='Next3_Click'>Next &gt;&gt;</div>
                    <div class='clear'></div>
                </div>
                <div class='Screen Screen_Mappings'>
                    Import Mappings
                    <br />
                    <br />
                    <br />
                    <div class='Bttn Bttn_Left' On_Click='Prev4_Click'>&lt;&lt; Previous</div>
                    <div class='Bttn Bttn_Right' On_Click='Next4_Click'>Next &gt;&gt;</div>
                    <div class='clear'></div>
                </div>
                <div class='Screen Screen_Saving'>
                    
                </div>
                <div class='Screen Screen_Complete'>
                    
                </div>
            ";
        }
    }
}
