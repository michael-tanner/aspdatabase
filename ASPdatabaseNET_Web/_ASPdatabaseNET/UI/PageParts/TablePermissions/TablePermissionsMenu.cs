using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.UI.PageParts.TablePermissions
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TablePermissionsMenu : MRBPattern<string, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public TablePermissionsMenu()
        {
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='TablePermissionsMenu jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();
        }

        //------------------------------------------------------------------------------------------ Events --
        public void TabClick_UserGroups()
        {
            jF2(".Tab").removeClass("Selected");
            jF2(".Tab_UserGroups").addClass("Selected");
            
            jF2(".MenuBox").hide();
            jF2(".MenuBox_UG").show();
        }
        public void TabClick_ImpliedPermissions()
        {
            jF2(".Tab").removeClass("Selected");
            jF2(".Tab_ImpliedPermissions").addClass("Selected");

            jF2(".MenuBox").hide();
            jF2(".MenuBox_IP").show();
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
            .TablePermissionsMenu { }
            .TablePermissionsMenu .Tabs .Tab { float: left; color: #14498f; font-size: .8em; text-align:center; 
                                            line-height: 19px; padding-bottom: 3px; cursor: pointer; }
            .TablePermissionsMenu .Tabs .Tab:hover { background: #e0e0e0; }
            .TablePermissionsMenu .Tabs .Tab_UserGroups { width: 68px;
                                                    border-top: 1px solid #e0e0e0; 
                                                    border-left: 1px solid #e0e0e0;
                                                    border-bottom: 1px solid #14498f; }
            .TablePermissionsMenu .Tabs .Tab_ImpliedPermissions { width: 144px; 
                                                            border-top: 1px solid #e0e0e0; 
                                                            border-right: 1px solid #e0e0e0;
                                                            border-bottom: 1px solid #14498f; }
            .TablePermissionsMenu .Tabs .Selected { border: 1px solid #14498f; border-bottom-color: #fff; }

            .TablePermissionsMenu .MenuBox { border: 1px solid #14498f; border-top-width: 0px; padding: 10px 0px; }

            .TablePermissionsMenu .MenuBox .Item { line-height: 24px; color: #45494f; font-size: .8em; padding-left: 8px; cursor: pointer; }
            .TablePermissionsMenu .MenuBox .Item:hover { background: #ddd; color: #000; }
            .TablePermissionsMenu .MenuBox .Selected { background: #5a6169; color: #fff; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Tabs'>
                    <div class='Tab Tab_UserGroups Selected' On_Click='TabClick_UserGroups'>User<br />Groups</div>
                    <div class='Tab Tab_ImpliedPermissions' On_Click='TabClick_ImpliedPermissions'>Implied Permissions<br />(by Username)</div>
                </div>
                <div class='clear'></div>
                <div class='MenuBox MenuBox_UG'>
                    <div class='Item'>Group Name 01</div>
                    <div class='Item'>Group Name 02</div>
                    <div class='Item Selected'>Group Name 03</div>
                    <div class='Item'>Group Name 04</div>
                    <div class='Item'>Group Name 05</div>
                    <div class='Item'>Group Name 06</div>
                </div>
                <div class='MenuBox MenuBox_IP hide'>
                    <div class='Item'>Username 01</div>
                    <div class='Item'>Username 02</div>
                    <div class='Item'>Username 03</div>
                    <div class='Item'>Username 04</div>
                    <div class='Item'>Username 05</div>
                    <div class='Item'>Username 06</div>
                    <div class='Item'>Username 07</div>
                    <div class='Item Selected'>Username 08</div>
                    <div class='Item'>Username 09</div>
                    <div class='Item'>Username 10</div>
                </div>
            ";
        }
    }
}