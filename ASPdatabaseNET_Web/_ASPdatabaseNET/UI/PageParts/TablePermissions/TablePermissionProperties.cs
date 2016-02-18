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
    public class TablePermissionProperties : MRBPattern<string, string>
    {
        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public TablePermissionProperties()
        {
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='TablePermissionProperties jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());

            this.BindUI();
        }

        //------------------------------------------------------------------------------------------ Events --




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
            .TablePermissionProperties { }
            .TablePermissionProperties .ArrowBox { border: 1px solid #bebfc1; background: #e6e7e9; padding: 7px; color: #5a6169; margin-bottom: 40px; }
            .TablePermissionProperties .ArrowBox .Label1 { padding-left: 35px; font-size: .6em; }
            .TablePermissionProperties .ArrowBox .CenterBox { width: 445px; }
            .TablePermissionProperties .ArrowBox .CenterBox .Label2 { text-align: right; margin: 2px 0px; }
            .TablePermissionProperties .ArrowBox .CenterBox .Label3 { font-size: .7em; text-align: right; }

            .TablePermissionProperties .Left2a { float: left; margin-left: 55px; width: 212px; font-size: .9em; line-height: 24px; }
            .TablePermissionProperties .Left2b { float: left; width: 255px; font-size: .9em; line-height: 24px; }
            .TablePermissionProperties .Label4 { font-size: 1.1em; }

            .TablePermissionProperties .DivSpace1 { height: 14px; }

            .TablePermissionProperties .BottomSectionLine { border: 1px solid #bebfc1; border-top-width: 0px; 
                                                            background: #e6e7e9; height: 5px; margin: 17px 0px; }

            .TablePermissionProperties .Label5 { margin-left: 17px; color: #b1bdcb; font-size: .9em; }
            .TablePermissionProperties .Label6 { margin-left: 17px; color: #3e444a; font-size: .9em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='ArrowBox'>
                    <div class='Label1'>Permissions Between:</div>
                    <div class='CenterBox'>
                        <div class='Label2'>Table Name   <------------->   Group Name 03</div>
                        <div class='Label3'>(has 7 users)</div>
                    </div>
                </div>

                <div class='Left2a'>
                    <div class='Label4'>Access Level</div>

                    <input type='radio' name='Radio_AccessLevel' value='NoAccess' />
                    No Access
                    <br />
                    <input type='radio' name='Radio_AccessLevel' value='View' checked='checked' />
                    View
                    <br />
                    <input type='radio' name='Radio_AccessLevel' value='Edit' />
                    Edit
                    <br />
                    <input type='radio' name='Radio_AccessLevel' value='FullAdmin' />
                    Full Admin
                </div>

                <div class='Left2b'>
                    <div class='Label4'>Options</div>

                    <input type='checkbox' />
                    Can Download
                    <br />

                    <input type='checkbox' />
                    Can Create Custom Views
                    
                    <div class='DivSpace1'></div>

                    <input type='checkbox' />
                    Can Import
                    <br />

                    <input type='checkbox' />
                    Can Create New Records
                    <br />

                    <input type='checkbox' />
                    Can Delete
                </div>

                <div class='clear'></div>
                <div class='BottomSectionLine'></div>

                <div class='Label5'>Implied Settings</div>
                <div class='Label6'>
                    Users in Group Name 03 can do the following:
                    <br /><br />
	                &nbsp;&nbsp;&nbsp;&nbsp;    - View All Table Data <br />
	                &nbsp;&nbsp;&nbsp;&nbsp;    - Create Custom Views<br />
	                &nbsp;&nbsp;&nbsp;&nbsp;    - Export/Download Data
                </div>

                <div class='BottomSectionLine'></div>
            ";
        }
    }
}