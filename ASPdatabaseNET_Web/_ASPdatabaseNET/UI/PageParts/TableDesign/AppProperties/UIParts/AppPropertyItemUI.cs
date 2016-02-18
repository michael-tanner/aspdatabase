using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs;

namespace ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.UIParts
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class AppPropertyItemUI : MRBPattern<AppPropertiesItem, TableDesign_ViewModel>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public AppPropertyItemUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='AppPropertyItemUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();
            ASPdatabaseNET.UI.PagesFramework.BasePage.WindowResized();
        }






        //------------------------------------------------------------------------------------------ Events --
        public void Click()
        {
            this.OnChange.After.Fire1(this.Model);
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
                .AppPropertyItemUI { border-bottom: 1px solid #888; cursor:pointer; }
                .AppPropertyItemUI:hover { background: #ccc; }
                .AppPropertyItemUI td { padding: 0em 1.5em; line-height: 1.8em; }
                .AppPropertyItemUI td .Smaller { font-size: .75em; color: #999; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td On_Click='Click'><div ModelKey='Index'             class=''       ></div></td>
                <td On_Click='Click'><div ModelKey='ColumnName'        class=''       ></div></td>
                <td On_Click='Click'><div ModelKey='DataType_Name'     class='Smaller'></div></td>
                <td On_Click='Click'><div ModelKey='AppColumnType_Str' class='Smaller'></div></td>
                <td On_Click='Click'><div ModelKey='AdditionalInfo'    class='Smaller'></div></td>
            ";
        }
    }
}
