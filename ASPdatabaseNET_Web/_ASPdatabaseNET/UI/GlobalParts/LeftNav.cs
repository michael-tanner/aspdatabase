using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdb.Ajax;
using ASPdatabaseNET.AjaxService;
using ASPdatabaseNET.DataObjects.Nav;
using ASPdatabaseNET.UI.GlobalParts.LeftNavParts;

namespace ASPdatabaseNET.UI.GlobalParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class LeftNav : MRBPattern<NavSiteInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public LeftNav()
        {
            this.Instantiate();
        }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='LeftNav jRoot'>");
            jRoot.append(this.GetHtmlRoot());

            var thisObj = this;
            var databaseBoxesHolder = jF(".DatabaseBoxesHolder");
            eval("databaseBoxesHolder.scroll(function(){ thisObj.Scroll(); });");

            ASPdatabaseService.New(this, "GetSiteNav_Return").Global__GetSiteNav(false);
        }
        //----------------------------------------------------------------------------------------------------
        public void GetSiteNav_Return(AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            this.Model = ajaxResponse.ReturnObj.As<NavSiteInfo>();
            this.Set_IsOpens_FromLocalStorage();

            if (this.Model.IsInDemoMode)
                J(".TopBarHolder .LogoLink").html("ASPdatabase.NET <span style='font-size: .6em;'> ... (Click Here to Learn More)</span>");

            var holder = jF2(".DatabaseBoxesHolder");
            holder.html("");
            for (int i = 0; i < this.Model.Databases.Length; i++)
            {
                var databaseBox = new DatabaseBox();
                databaseBox.Model = this.Model.Databases[i];
                databaseBox.Instantiate();
                databaseBox.OnChange.After.AddHandler(this, "DatabaseBox_OnChange", 0);
                holder.append(databaseBox.jRoot);
            }
            jRoot.show();
            this.CheckLocalStorage_Scroll();
        }

        //----------------------------------------------------------------------------------------------------
        public void Set_IsOpens_FromLocalStorage()
        {
            try
            {
                string json = localStorage.getItem("ASPdatabaseNET.UI.GlobalParts.LeftNav");
                var localSiteNav = (new ASPdb.Ajax.AjaxHelper()).FromJson<NavSiteInfo>(json);
                if (localSiteNav == null)
                    return;

                var dict = new object[0];
                for (int i = 0; i < localSiteNav.Databases.Length; i++)
                {
                    var navDB = localSiteNav.Databases[i];
                    string key = "C_" + navDB.ConnectionId;
                    eval("dict[key] = navDB;");
                }
                for (int i = 0; i < this.Model.Databases.Length; i++)
                {
                    NavDatabaseInfo navDB1 = this.Model.Databases[i];
                    NavDatabaseInfo navDB2 = null;
                    string key = "C_" + navDB1.ConnectionId;
                    try { eval("navDB2 = dict[key];"); }
                    catch { }
                    if (navDB2 != null)
                    {
                        navDB1.IsOpen = navDB2.IsOpen;
                        if (navDB1.Section_Tables != null && navDB2.Section_Tables != null)
                            navDB1.Section_Tables.IsOpen = navDB2.Section_Tables.IsOpen;
                        //if (navDB1.Section_AppViews != null && navDB2.Section_AppViews != null)
                        //    navDB1.Section_AppViews.IsOpen = navDB2.Section_AppViews.IsOpen;
                        if (navDB1.Section_Views != null && navDB2.Section_Views != null)
                            navDB1.Section_Views.IsOpen = navDB2.Section_Views.IsOpen;
                    }
                }
            }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        public void DatabaseBox_OnChange()
        {
            try
            {
                string json = (new ASPdb.Ajax.AjaxHelper()).ToJson(this.Model);
                localStorage.setItem("ASPdatabaseNET.UI.GlobalParts.LeftNav", json);
            }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        public void Scroll()
        {
            int scrollTop = jF(".DatabaseBoxesHolder").scrollTop().As<JsNumber>();
            try { localStorage.setItem("ASPdatabaseNET.UI.GlobalParts.LeftNav.ScrollTop", scrollTop.As<string>()); }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        public void CheckLocalStorage_Scroll()
        {
            try
            {
                int scrollTop = localStorage.getItem("ASPdatabaseNET.UI.GlobalParts.LeftNav.ScrollTop").As<JsNumber>();
                jF(".DatabaseBoxesHolder").scrollTop(scrollTop);
            }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        public void DatabasesLabelBox_Click()
        {
            jRoot.hide();
            ASPdatabaseService.New(this, "GetSiteNav_Return").Global__GetSiteNav(true);
        }

        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += DatabaseBox.GetCssTree();
            rtn += SectionBox.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
            .LeftNav { background: #e4e4e4;  }
            .LeftNav .DatabasesLabelBox { line-height: 2.8409em; padding-left: 1.25em; color: #000; font-size: 1.1em; color: #555; cursor:pointer; }
            .LeftNav .DatabasesLabelBox:hover { background: #ccc; color: #000; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='DatabasesLabelBox' On_Click='DatabasesLabelBox_Click' title='Refresh Menu'>
                Connections
            </div>
            <div class='DatabaseBoxesHolder AutoResize'></div>
            ";
        }
    }
}