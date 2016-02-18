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
    public class ImportExportUI : MRBPattern<string, ImportExportViewModel>
    {
        public ImportExportUI_UploadExcel UploadExcel;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ImportExportUI()
        {
            this.ViewModel = new ImportExportViewModel();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ImportExportUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.UploadExcel = new ImportExportUI_UploadExcel();
            this.UploadExcel.ViewModel = this.ViewModel;
            this.UploadExcel.Instantiate();
            this.UploadExcel.Close();
            this.UploadExcel.OnClose.After.AddHandler(this, "UploadExcel_Close", 0);
            jF(".Holder_UploadExcel").append(this.UploadExcel.jRoot);
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            if (this.UploadExcel.IsOpen)
                this.UploadExcel.Close();
            
            //jF(".Div_PA").hide().delay(100).fadeIn(250);

            int shiftRight = J(".Bttn_ViewOptions").outerWidth();
            jF(".Div_PA")[0].style.right = shiftRight + "px";
        }






        //------------------------------------------------------------------------------------------ Events --
        public void CloseClick1()
        {
            if (!this.UploadExcel.IsOpen)
                this.Close();
        }
        public void CloseClick2()
        {
            if (this.UploadExcel.IsOpen)
                this.UploadExcel.Close();
            else
                this.Close();
        }
        //----------------------------------------------------------------------------------------------------
        public void UploadExcel_Click()
        {
            jF(".MainBttns").hide();
            this.UploadExcel.Open();
        }
        public void UploadExcel_Close()
        {
            jF(".MainBttns").show();
        }
        public void DownloadExcel_Click()
        {
            var json = (new ASPdb.Ajax.AjaxHelper()).ToJson(this.ViewModel.GridRequest);
            string requestString = "";
            eval("requestString = CryptoJS.enc.Base64.stringify(CryptoJS.enc.Utf8.parse(json));");
            requestString = encodeURI(requestString);

            string randomString = ASPdb.Security.AESLogic.RandomBase64(30, 99);
            eval("randomString = CryptoJS.enc.Base64.stringify(CryptoJS.enc.Utf8.parse(randomString));");
            randomString = encodeURI(randomString);

            string url = "ASPdatabase.NET.aspx?Download=Excel&Key=" + requestString + "&Tag=" + randomString;
            window.location = url.As<Location>();

            this.Close();
        }





        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += ImportExportUI_UploadExcel.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ImportExportUI {  }
                .ImportExportUI .Div_BG { background: #464f5b; opacity: 0.65; position:fixed; width: 100%; height: 8000px; top: 0px; left: 0px; }

                .ImportExportUI .Div_PR { position:relative; }
                .ImportExportUI .Div_PR .Div_PA { position:absolute; top: 1px; right: 0px; background: #fff; color: #093a79; 
                                                  width_next: 30em; width: 18em; min-height: 8.4em; box-shadow: 0.125em 0.125em .2em #333; padding: .5em 1.2em; }

                .ImportExportUI .Div_PR .Div_PA .Head { float:left; font-size: .9em; padding: .2em .4em; }
                .ImportExportUI .Div_PR .Div_PA .CloseBttn { float:right; font-size: .9em; cursor:pointer; padding: .2em .4em; }
                .ImportExportUI .Div_PR .Div_PA .CloseBttn:hover { background: #093a79; color: #fff; }

                .ImportExportUI .Div_PR .Div_PA .Spacer1 { height: 2.5em; }


                .ImportExportUI .Div_PR .Div_PA .Bttn_UploadExcel { float:left; position:relative; line-height: 2.1875em; background: #2d925c; color: #fff; 
                                                                    width: 14em; text-align: center; cursor:pointer; }
                .ImportExportUI .Div_PR .Div_PA .Bttn_UploadExcel .Icon { position:absolute; height: 2.1875em; width: 2.5625em; background: #257e50;
                                                                          background: url('ASPdatabase.NET.aspx?IMG=Sprite1'); }
                .ImportExportUI .Div_PR .Div_PA .Bttn_UploadExcel span { font-size: .9em; padding-left: 1.42em; }
                .ImportExportUI .Div_PR .Div_PA .Bttn_UploadExcel:hover { background: #c8ecda; color: #135533; }


                .ImportExportUI .Div_PR .Div_PA .Bttn_DownloadExcel { float:right; position:relative; line-height: 2.1875em; background: #6247a0; color: #fff; 
                                                                    width: 14em; text-align: center; cursor:pointer; }
                .ImportExportUI .Div_PR .Div_PA .Bttn_DownloadExcel .Icon { position:absolute; height: 2.1875em; width: 2.5625em;
                                                                          background: url('ASPdatabase.NET.aspx?IMG=Sprite1') no-repeat 0px -35px; }
                .ImportExportUI .Div_PR .Div_PA .Bttn_DownloadExcel span { font-size: .9em; padding-left: 1.42em; }
                .ImportExportUI .Div_PR .Div_PA .Bttn_DownloadExcel:hover { background: #ede5fa; color: #352065; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Div_BG NoSelect' On_Click='CloseClick1'>&nbsp;</div>
                <div class='Div_PR'>
                    <div class='Div_PA'>
                        <div class='Head'>Export</div>
                        <div class='CloseBttn' On_Click='CloseClick2'>Close</div>
                        <div class='clear'></div>
                        <div class='Spacer1'></div>

                        <div class='MainBttns'>
                            <div class='Bttn_UploadExcel hide' On_Click='UploadExcel_Click'>
                                <div class='Icon'></div>
                                <span>Upload Excel</span>
                            </div>
                            <div class='Bttn_DownloadExcel' On_Click='DownloadExcel_Click'>
                                <div class='Icon'></div>
                                <span>Download Excel</span>
                            </div>
                            <div class='clear'></div>
                        </div>

                        <div class='Holder_UploadExcel'></div>

                    </div>
                </div>
            ";
        }
    }
}
