using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.UI.PageParts.Users.Objs;

namespace ASPdatabaseNET.UI.PageParts.Users
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Panel_GroupUI : MRBPattern<GroupInfo, UsersViewModel>
    {
        public Panel_GroupUI_ConnectionItem[] ConnectionItems;
        public Panel_GroupUI_PermissionItem[] PermissionItems;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public Panel_GroupUI()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='Panel_GroupUI jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            jF(".Left").hide();
            jF(".Right").hide();
            jF(".TablePermissionsTable").hide();
            jF(".Loading").show();
            AjaxService.ASPdatabaseService.New(this, "GetGroup_Return").Users__GetGroup(this.ViewModel.CurrentId);
        }
        public void GetGroup_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<GroupInfo>();
            this.BindUI();

            this.Model.Active = !this.Model.Active;
            this.ActiveBttn_Click();

            var holder_ConnectionItems = jF(".Holder_ConnectionItems").html("");
            this.ConnectionItems = new Panel_GroupUI_ConnectionItem[0];
            if(this.Model.Permissions != null)
                for (int i = 0; i < this.Model.Permissions.Length; i++)
                {
                    this.ConnectionItems[i] = new Panel_GroupUI_ConnectionItem();
                    this.ConnectionItems[i].ViewModel = this.ViewModel;
                    this.ConnectionItems[i].Model = this.Model.Permissions[i];
                    this.ConnectionItems[i].Instantiate();
                    this.ConnectionItems[i].OnChange.After.AddHandler(this, "ConnectionItem_Click", 1);
                    holder_ConnectionItems.append(this.ConnectionItems[i].jRoot);
                }

            jF(".Holder_PermissionItems").html("");
            this.PermissionItems = new Panel_GroupUI_PermissionItem[0];

            jF(".Loading").hide();
            jF(".Left").show();
            jF(".Right").show();
            jF(".TablePermissionsTable").show();
            if (this.Model.GroupId < 0)
                jF(".GroupId").html("");
            UI.PagesFramework.BasePage.WindowResized();
        }






        //------------------------------------------------------------------------------------------ Events --
        public void SaveBttn_Click()
        {
            this.Model.GroupName = jF(".T_GroupName").val().As<string>();

            AjaxService.ASPdatabaseService.New(this, "SaveGroup_Return").Users__SaveGroup(this.Model);
        }
        public void SaveGroup_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.CurrentId = ajaxResponse.ReturnObj.As<JsNumber>();
            this.OnChange.After.Fire();
            alert("Saved");
        }
        //----------------------------------------------------------------------------------------------------
        public void DeleteBttn_Click()
        {
            if (prompt("Are you certain? \n\nIf so, type \"delete group\"") == "delete group")
            {
                AjaxService.ASPdatabaseService.New(this, "DeleteGroup_Return").Users__DeleteGroup(this.Model.GroupId);
            }
        }
        public void DeleteGroup_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.ViewModel.CurrentId = -1;
            this.Model.GroupId = -1;
            this.Close();
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void CancelBttn_Click()
        {
            this.Close();
        }
        //----------------------------------------------------------------------------------------------------
        public void ActiveBttn_Click()
        {
            this.Model.Active = !this.Model.Active;
            if (this.Model.Active)
            {
                jF(".ActiveBttn").addClass("ActiveBttn_On");
                jF(".C_Active").attr("checked", true);
            }
            else
            {
                jF(".ActiveBttn").removeClass("ActiveBttn_On");
                jF(".C_Active").removeAttr("checked");
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void ConnectionItem_Click(Panel_GroupUI_ConnectionItem connectionItem)
        {
            for (int i = 0; i < this.ConnectionItems.Length; i++)
                this.ConnectionItems[i].RemoveClass__Item_On();

            Permission connectionRoot = null;
            for (int i = 0; i < this.Model.Permissions.Length; i++)
                if (this.Model.Permissions[i].ConnectionId == connectionItem.Model.ConnectionId)
                    connectionRoot = this.Model.Permissions[i];

            var holder_PermissionItems = jF(".Holder_PermissionItems").html("");
            this.PermissionItems = new Panel_GroupUI_PermissionItem[0];
            if (connectionRoot != null)
            {
                int i = 0;
                this.PermissionItems[i] = new Panel_GroupUI_PermissionItem();
                this.PermissionItems[i].Model = connectionRoot;
                i++;
                if(connectionRoot.SubPermissions != null)
                    for (int j = 0; j < connectionRoot.SubPermissions.Length; j++)
                    {
                        this.PermissionItems[i] = new Panel_GroupUI_PermissionItem();
                        this.PermissionItems[i].Model = connectionRoot.SubPermissions[j];
                        i++;
                        if (connectionRoot.SubPermissions[j].SubPermissions != null)
                            for (int k = 0; k < connectionRoot.SubPermissions[j].SubPermissions.Length; k++)
                            {
                                this.PermissionItems[i] = new Panel_GroupUI_PermissionItem();
                                this.PermissionItems[i].Model = connectionRoot.SubPermissions[j].SubPermissions[k];
                                i++;
                            }
                    }
            }
            for (int i = 0; i < this.PermissionItems.Length; i++)
            {
                this.PermissionItems[i].ViewModel = this.ViewModel;
                this.PermissionItems[i].Instantiate();
                holder_PermissionItems.append(this.PermissionItems[i].jRoot);
            }
            holder_PermissionItems.scrollTop(0);
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += Panel_GroupUI_ConnectionItem.GetCssTree();
            rtn += Panel_GroupUI_PermissionItem.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .Panel_GroupUI { }

                .Panel_GroupUI .MainForm { margin-top: 2.1875em; padding: 0em 1.4em; background: #f8f8f8; border-left: 1px solid #fff; }
                .Panel_GroupUI .MainForm .Loading {  line-height: 3.25em; text-align: center; color: #777; }
                .Panel_GroupUI .MainForm .Left {  display:none; float:left; width: 13em; }
                .Panel_GroupUI .MainForm .Right { display:none; float:right; width: 16em; }
                
                .Panel_GroupUI .MainForm .ActiveBttn { font-size: .8em; position:relative; height: 2.265625em; color: #a1a1a1; cursor:pointer; }
                .Panel_GroupUI .MainForm .ActiveBttn:hover { background: #6a6e73; color: #fff; }
                .Panel_GroupUI .MainForm .ActiveBttn_On { background: #eb640a; color: #fff; }
                .Panel_GroupUI .MainForm .ActiveBttn input { position:absolute; top: 0.625em; left: 1.25em; }
                .Panel_GroupUI .MainForm .ActiveBttn span { position:absolute; top: 0.46875em; left: 3.75em; }

                .Panel_GroupUI .MainForm .Label0 { font-size: .7em; line-height: 1.3em; color: #a1a1a1; }
                .Panel_GroupUI .MainForm .Label1 { font-size: .7em; line-height: 2.25em; color: #a1a1a1; }
                .Panel_GroupUI .MainForm .Label1 span { color: #1161ca; }
                
                .Panel_GroupUI .MainForm .Label2 { font-size: .8em; line-height: 1.484375em; color: #a1a1a1; margin-top: 1.2em; }
                
                .Panel_GroupUI .MainForm input[type=text] { font-size: .9em; width: 13.944444em; border: 1px solid #3d7bcc; background: #fff; color: #3d7bcc; margin-bottom: 1.1em; line-height: 1.5em; padding: .25em; }
                
                .Panel_GroupUI .MainForm .BttnsTable { width: 100%; }
                .Panel_GroupUI .MainForm .BttnsTable td { font-size: .8em; width: 33%; text-align:center; line-height: 2.265625em; color: #fff; cursor:pointer; }
                .Panel_GroupUI .MainForm .BttnsTable .SaveBttn { background: #14498f; }
                .Panel_GroupUI .MainForm .BttnsTable .SaveBttn:hover { background: #6a6e73; }
                .Panel_GroupUI .MainForm .BttnsTable .Bttn1 { background: #3d7bcc; border-left: 1px solid #fff; }
                .Panel_GroupUI .MainForm .BttnsTable .Bttn1:hover { background: #6a6e73; }

                .Panel_GroupUI .MainForm .TablePermissionsTable { width: 100%; }
                .Panel_GroupUI .MainForm .TablePermissionsTable td { }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td1 { font-size: .8em; color: #a1a1a1; }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td2 { font-size: .8em; color: #a1a1a1; padding: 0em 2em 0em .8em; }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td3 { background: #fff; border-top: 1px solid #aac5e9; width: 10.75em; border-right: 0.25em solid #f8f8f8; }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td4 { background: #fff; border-top: 1px solid #aac5e9; }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td2 .RightLabel { float: right; font-size: .7em; width: 4.1em; text-align:center; color: #333; }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td3 .Holder_ConnectionItems { min-height: 8em; }
                .Panel_GroupUI .MainForm .TablePermissionsTable .Td4 .Holder_PermissionItems { }
                .Panel_GroupUI .MainForm .MarginBottom { line-height: 0.6875em; }
                
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='MainForm'>
                    <div class='Loading'>Loading . . .</div>
                    <div class='Left'>
                        <div class='ActiveBttn' On_Click='ActiveBttn_Click'>
                            <input type='checkbox' class='C_Active' />
                            <span>Active</span>
                            &nbsp;
                        </div>
                        <div class='Label2'>Group Name</div>
                        <input type='text' class='T_GroupName' ModelKey='GroupName' />
                    </div>
                    <div class='Right'>
                        <table class='BttnsTable'><tr>
                            <td class='SaveBttn'   On_Click='SaveBttn_Click'  >Save</td>
                            <td class='Bttn1'      On_Click='DeleteBttn_Click'>Delete</td>
                            <td class='Bttn1'      On_Click='CancelBttn_Click'>Cancel</td>
                        </tr></table>
                        <div class='Label0'>&nbsp;</div>
                        <div class='Label1'>GroupId : <span class='GroupId' ModelKey='GroupId'></span></div>
                        <div class='Label1'>Time Created : <span ModelKey='TimeCreated_Str'></span></div>
                    </div>
                    <div class='clear'></div>
                    <table class='TablePermissionsTable'>
                        <tr>
                            <td class='Td1'>Connections</td>
                            <td class='Td2'>
                                Permissions
                                <div class='RightLabel'>Delete</div>
                                <div class='RightLabel'>Insert</div>
                                <div class='RightLabel'>Edit</div>
                                <div class='RightLabel'>View</div>
                                <div class='clear'></div>
                            </td>
                        </tr>
                        <tr>
                            <td class='Td3'>
                                <div class='Holder_ConnectionItems'></div>
                            </td>
                            <td class='Td4'>
                                <div class='Holder_PermissionItems AutoResize'></div>
                            </td>
                        </tr>
                    </table>
                    <div class='MarginBottom'>&nbsp;</div>
                </div>
            ";
        }
    }
}
