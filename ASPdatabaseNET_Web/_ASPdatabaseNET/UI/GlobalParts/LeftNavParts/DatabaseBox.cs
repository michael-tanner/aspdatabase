using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DataObjects.Nav;

namespace ASPdatabaseNET.UI.GlobalParts.LeftNavParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class DatabaseBox : MRBPattern<NavDatabaseInfo, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public DatabaseBox()
        {
        }

        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            jRoot = J("<div class='DatabaseBox jRoot'>");
            jRoot.append(this.GetHtmlRoot());

            if(this.Model.IsOpen)
                jF2(".DatabaseSubBox").show();

            var holder = jF2(".DatabaseSubBox");
            if (this.Model.Section_Tables != null)
            {
                var section = new SectionBox();
                section.Model = this.Model.Section_Tables;
                section.Instantiate();
                section.OnChange.After.AddHandler(this, "SectionBox_OnChange", 0);
                holder.append(section.jRoot);
            }
            //if (this.Model.Section_AppViews != null)
            //{
            //    var section = new SectionBox();
            //    section.Model = this.Model.Section_AppViews;
            //    section.Instantiate();
            //    section.OnChange.After.AddHandler(this, "SectionBox_OnChange", 0);
            //    holder.append(section.jRoot);
            //}
            if (this.Model.Section_Views != null)
            {
                var section = new SectionBox();
                section.Model = this.Model.Section_Views;
                section.Instantiate();
                section.OnChange.After.AddHandler(this, "SectionBox_OnChange", 0);
                holder.append(section.jRoot);
            }

            this.BindUI();
        }


        //----------------------------------------------------------------------------------------------------
        public void ConnectionNameBttn_Click()
        {
            if (this.Model.IsOpen)
                jF2(".DatabaseSubBox").hide();
            else
                jF2(".DatabaseSubBox").show();
            this.Model.IsOpen = !this.Model.IsOpen;
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void SectionBox_OnChange()
        {
            this.OnChange.After.Fire();
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
            .DatabaseBox { font-size: .9em; }
            .DatabaseBox .ConnectionNameBttn { background: #173a67; color: #fff; line-height: 2.222em; margin-bottom: 1px; padding-left: 1.52777em; border-top-left-radius:1.4em; }
            .DatabaseBox .ConnectionNameBttn:hover { background: #000; cursor: pointer; }
            .DatabaseBox .DatabaseSubBox { display: none; margin-left: 1.52777em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
            <div class='ConnectionNameBttn' On_Click='ConnectionNameBttn_Click' ModelKey='ConnectionName'></div>
            <div class='DatabaseSubBox'>
            </div>
            ";
        }
    }
}