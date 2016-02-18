using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.Pages;

namespace ASPdatabaseNET.UI.PagesFramework
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class PageIdentifier : jQueryContext
    {
        /*  There are 3 places below where PageType settings are set:  */
        /*      enum PageTypes                                         */
        /*      GetAllPageTypesInts()                                  */
        /*      CreateAndGetPageObj()                                  */

        public enum PageTypes /* Explicitly set enum numbers since they will be used as part of the URL */
        {
            StartPage = 0,
            Settings = 20
            //Connections_Add = 21,
            //Tables = 25,
            //Sync = 35,
            //Settings = 99,
            //Account = 98
        }
        public PageTypes PageType;
        public string PageParam1 = "";
        public string PageParam2 = "";
        public string PageParam3 = "";
        public string PageParam4 = "";
        public string PageParam5 = "";

        //------------------------------------------------------------------------------------------ static --
        public static PageIdentifier GetFromUrlHash()
        {
            var rtn = new PageIdentifier(PageTypes.StartPage);
            var arr = window.location.hash.replace("#", "").split("-");

            if (arr.length > 0) // clean & build this out later
                if (arr[0].toLowerCase() == "manageassets")
                    window.location.href = "#00-ManageAssets";
            
            var pageTypeInt = IntStatic.Parse(arr[0], 0);
            var allEnumInts = GetAllPageTypesInts();
            bool pageTypeIsValid = false;
            for (int i = 0; i < allEnumInts.Length; i++)
                if (allEnumInts[i] == pageTypeInt)
                    pageTypeIsValid = true;

            rtn.PageType = PageTypes.StartPage;
            if (pageTypeIsValid)
            {
                PageTypes tmpPageType = PageTypes.StartPage;
                eval("tmpPageType = pageTypeInt;");
                rtn.PageType = tmpPageType;
                //rtn.PageType = pageTypeInt.As<PageTypes>();
            }
            else
                window.location.hash = "";

            if (arr.length > 1) rtn.PageParam1 = arr[1];
            if (arr.length > 2) rtn.PageParam2 = arr[2];
            if (arr.length > 3) rtn.PageParam3 = arr[3];
            if (arr.length > 4) rtn.PageParam4 = arr[4];
            if (arr.length > 5) rtn.PageParam5 = arr[5];

            return rtn;
        }
        //------------------------------------------------------------------------------------------ static --
        public static int[] GetAllPageTypesInts()
        {
            int i = 0;
            var rtn = (new JsArray()).As<int[]>(); // new int[0];
            rtn[i++] = PageTypes.StartPage.As<JsNumber>();
            rtn[i++] = PageTypes.Settings.As<JsNumber>();
            return rtn;
        }


        //------------------------------------------------------------------------------------- constructor --
        public PageIdentifier(PageTypes pageType)
        {
            this.PageType = pageType;
        }

        //----------------------------------------------------------------------------------------------------
        public string ToHash()
        {
            string rtn = "";

            string tmpPageStr = this.PageType.As<string>();
            if (("" + tmpPageStr).Length < 2)
                rtn += "#0" + tmpPageStr;
            else
                rtn += "#" + tmpPageStr;

            bool maxFound = false;
            int maxParams = 5;
            if (!maxFound)
            {
                if (("" + this.PageParam5).Length > 0) { maxFound = true; } else { maxParams = 4; }
            }
            if (!maxFound)
            {
                if (("" + this.PageParam4).Length > 0) { maxFound = true; } else { maxParams = 3; }
            }
            if (!maxFound)
            {
                if (("" + this.PageParam3).Length > 0) { maxFound = true; } else { maxParams = 2; }
            }
            if (!maxFound)
            {
                if (("" + this.PageParam2).Length > 0) { maxFound = true; } else { maxParams = 1; }
            }
            if (!maxFound)
            {
                if (("" + this.PageParam1).Length > 0) { maxFound = true; } else { maxParams = 0; }
            }


            if (("" + this.PageParam1).Length < 1) this.PageParam1 = "00";
            if (("" + this.PageParam2).Length < 1) this.PageParam2 = "00";
            if (("" + this.PageParam3).Length < 1) this.PageParam3 = "00";
            if (("" + this.PageParam4).Length < 1) this.PageParam4 = "00";
            if (("" + this.PageParam5).Length < 1) this.PageParam5 = "00";


            if (maxParams >= 1) rtn += "-" + this.PageParam1;
            if (maxParams >= 2) rtn += "-" + this.PageParam2;
            if (maxParams >= 3) rtn += "-" + this.PageParam3;
            if (maxParams >= 4) rtn += "-" + this.PageParam4;
            if (maxParams >= 5) rtn += "-" + this.PageParam5;

            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public IPage CreateAndGetPageObj()
        {
            IPage rtn = null;
            switch (this.PageType)
            {
                case PageTypes.StartPage:
                    rtn = new EverythingPage();
                    break;
                case PageTypes.Settings:
                    rtn = new OtherSamplePage();
                    break;
            }
            if (rtn != null)
            {
                rtn.PageId = this; // set params inside here
                rtn.Instantiate();
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public void Launch()
        {
            BasePage.GetFromDocument().LaunchPage(this);
        }
    }
}