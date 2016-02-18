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
    public class TablePermissionsMainUI : MRBPattern<string, string>
    {
        public TablePermissionsMenu TablePermissionsMenu;
        public TablePermissionProperties TablePermissionProperties;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public TablePermissionsMainUI()
        {
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='TableDesignMainUI MainUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.TablePermissionsMenu = new TablePermissionsMenu();
            this.TablePermissionProperties = new TablePermissionProperties();
            jF2(".Left").append(this.TablePermissionsMenu.jRoot);
            jF2(".Right").append(this.TablePermissionProperties.jRoot);

            this.BindUI();
        }

        //------------------------------------------------------------------------------------------ Events --




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += TablePermissionsMenu.GetCssTree();
            rtn += TablePermissionProperties.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .TableDesignMainUI { position: relative; }

                .TableDesignMainUI .BttnsBar { position: absolute; top: 32px; width: 760px; }
                .TableDesignMainUI .BttnsBar .Bttn { float: right; background: #14498f; color: #fff; line-height: 40px; padding: 0px 25px; 
                                                     cursor: pointer; margin-left: 5px; }
                .TableDesignMainUI .BttnsBar .Bttn:hover { background: #333; }
                .TableDesignMainUI .BttnsBar .Inactive { background: #ddd; cursor: default; }
                .TableDesignMainUI .BttnsBar .Inactive:hover { background: #ddd; }

                .TableDesignMainUI .TableNameLabel { margin-top: 15px; font-size: 1.05em; color: #555; }
                .TableDesignMainUI .TableNameLabel span { background: #f8f8f8; padding: 1px 3px; border: 1px solid #eee; }

                .TableDesignMainUI .Left { float: left; width: 215px; margin-top: 25px; }
                .TableDesignMainUI .Right { float: right; width: 525px; margin-top: 25px; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='Head'>Permissions</div>
                <div class='BttnsBar'>
                    <div class='Bttn'>Cancel</div>
                    <div class='Bttn Inactive'>Save</div>
                </div>
                <div class='TableNameLabel'>
                    For Table: <span>[Connection Name]</span>.<span>[" + Config.SystemProperties.AppSchema + @"]</span>.<span>[Table Name]</span>
                </div>
                <div class='Main'>
                    <div class='Left'></div>
                    <div class='Right'></div>
                    <div class='clear'></div>
                </div>
            ";
        }
    }
}