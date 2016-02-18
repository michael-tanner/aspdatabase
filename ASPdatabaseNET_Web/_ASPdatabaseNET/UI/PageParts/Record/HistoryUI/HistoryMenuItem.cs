using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Record.Objs_History;

namespace ASPdatabaseNET.UI.PageParts.Record.HistoryUI
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class HistoryMenuItem : MRBPattern<HistoryRecord, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public HistoryMenuItem()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J(this.GetHtmlRoot());

            jF(".Td_Revision").html(this.Model.Revision + "");
            jF(".Td_TimeLapsed").html(this.Model.TimeLapsedString);
            jF(".Td_TimeSaved").html(this.Model.TimeSaved_String);
            jF(".Td_ByUsername").html(this.Model.ByUsername);
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Click()
        {
            this.OnChange.After.Fire1(this);
            jRoot.addClass("HistoryMenuItem_On");
        }
        //----------------------------------------------------------------------------------------------------
        public void Deselect()
        {
            jRoot.removeClass("HistoryMenuItem_On");
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
            .HistoryMenuItem { font-size: .7em; line-height: 1.9em; cursor:pointer; }
            .HistoryMenuItem:hover { background: #ccc; }
            .HistoryMenuItem_On { background: #0a598c; color: #fff; }
            .HistoryMenuItem_On:hover { background: #444; color: #fff; }
            .HistoryMenuItem td { padding: 0em .3em 0em .5em; }
            .HistoryMenuItem .Td_Revision { }
            .HistoryMenuItem .Td_TimeSaved { width: 40%; }
            .HistoryMenuItem .Td_TimeLapsed { width: 27%; }
            .HistoryMenuItem .Td_ByUsername { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetTemplate()
        {
            return @"
<css>

<html>
-tr #HistoryMenuItem
--td #Td_Revision   On_Click='Click'
--td #Td_TimeSaved  On_Click='Click'
--td #Td_TimeLapsed On_Click='Click'
--td #Td_ByUsername On_Click='Click'
            ";
        }
    }
}
