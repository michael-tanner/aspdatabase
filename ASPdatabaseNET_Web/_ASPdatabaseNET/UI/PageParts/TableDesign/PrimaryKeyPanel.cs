using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DbInterfaces.TableObjects;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class PrimaryKeyPanel : MRBPattern<PrimaryKey, TableDesign_ViewModel>
    {
        public bool KeyDoesNotExist = true;
        public bool InEditMode = false;
        public PrimaryKeyRow[] PrimaryKeyRows;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public PrimaryKeyPanel(TableDesign_ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='PrimaryKeyPanel jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.jRoot.hide();

            this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            AjaxService.ASPdatabaseService.New(this, "PrimaryKey__Get_Return").TableDesign__PrimaryKey__Get(this.ViewModel.TableId);
        }
        public void PrimaryKey__Get_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.Model = ajaxResponse.ReturnObj.As<PrimaryKey>();
            this.KeyDoesNotExist = (this.Model == null);
            if (this.KeyDoesNotExist)
            {
                this.Model = new PrimaryKey();
                this.Model.ConnectionId = this.ViewModel.ConnectionId;
                this.Model.TableId = this.ViewModel.TableStructure.TableId;
                this.Model.Schema = this.ViewModel.TableStructure.Schema;
                this.Model.TableName = this.ViewModel.TableStructure.TableName;
                this.Model.Columns = new PrimaryKeyColumn[0];
                jF(".BttnPK_Remove").hide();
            }
            else
                jF(".BttnPK_Remove").show();


            this.PrimaryKeyRows = new PrimaryKeyRow[0];
            var holder_PrimaryKeyFields = jF(".Holder_PrimaryKeyFields").html("");
            this.Model.Columns[this.Model.Columns.Length] = new PrimaryKeyColumn();
            for (int i = 0; i < this.Model.Columns.Length; i++)
            {
                this.PrimaryKeyRows[i] = new PrimaryKeyRow();
                this.PrimaryKeyRows[i].ViewModel = this.ViewModel;
                this.PrimaryKeyRows[i].Model = this.Model.Columns[i];
                this.PrimaryKeyRows[i].Instantiate();
                this.PrimaryKeyRows[i].OnChange.After.AddHandler(this, "SubItem_Changed", 0);
                holder_PrimaryKeyFields.append(this.PrimaryKeyRows[i].jRoot);
            }
            this.SubItem_Changed();

            this.InEditMode = false;
            this.RefreshBttns();
        }
        //----------------------------------------------------------------------------------------------------
        public void RefreshBttns()
        {
            if(this.InEditMode)
                jF(".BttnPK_Toggleable").removeClass("BttnPK_Off");
            else
                jF(".BttnPK_Toggleable").addClass("BttnPK_Off");
        }



        //------------------------------------------------------------------------------------------ Events --
        public void Save_Click()
        {
            if (!this.InEditMode)
                return;

            int j = 0;
            this.Model.Columns = new PrimaryKeyColumn[0];
            for (int i = 0; i < this.PrimaryKeyRows.Length; i++)
            {
                var column = this.PrimaryKeyRows[i].GetModel();
                if(column != null)
                    this.Model.Columns[j++] = column;
            }
            if (this.KeyDoesNotExist)
                AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__PrimaryKey__Create(this.Model);
            else
                AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__PrimaryKey__Update(this.Model);
        }
        public void Save_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;
            this.OnChange.After.Fire();
        }


        public void Cancel_Click()
        {
            if (!this.InEditMode)
                return;
            this.Open();
        }
        public void Remove_Click()
        {
            if (confirm("Are you sure?"))
                AjaxService.ASPdatabaseService.New(this, "Save_Return").TableDesign__PrimaryKey__Delete(this.Model);
        }
        //----------------------------------------------------------------------------------------------------
        public void SubItem_Changed()
        {
            this.InEditMode = true;
            this.RefreshBttns();

            int lastIndex = this.PrimaryKeyRows.Length - 1;
            bool addEmptyItem = this.PrimaryKeyRows[lastIndex].HasValue();
            if(addEmptyItem)
            {
                int i = lastIndex + 1;
                this.PrimaryKeyRows[i] = new PrimaryKeyRow();
                this.PrimaryKeyRows[i].ViewModel = this.ViewModel;
                this.PrimaryKeyRows[i].Model = new PrimaryKeyColumn();
                this.PrimaryKeyRows[i].Instantiate();
                this.PrimaryKeyRows[i].OnChange.After.AddHandler(this, "SubItem_Changed", 0);
                jF(".Holder_PrimaryKeyFields").append(this.PrimaryKeyRows[i].jRoot);
            }

            int skipIndex = -1;
            for (int i = 0; i < this.PrimaryKeyRows.Length; i++) 
            {
                if (this.PrimaryKeyRows[i].IsIdentity)
                    skipIndex = i;
            }
            if(skipIndex > -1)
            {
                for (int i = 0; i < this.PrimaryKeyRows.Length; i++)
                    if (i != skipIndex)
                        this.PrimaryKeyRows[i].HideIdentityUI();
            }
            else
            {
                for (int i = 0; i < this.PrimaryKeyRows.Length; i++)
                    this.PrimaryKeyRows[i].ShowIdentityUI();
            }
            

        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += PrimaryKeyRow.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .PrimaryKeyPanel { width: 100%; position:relative; padding-top: 37px; }
                .PrimaryKeyPanel .TopColorBar { position:absolute; top: -1px; width: inherit; min-width: 200px; height: 8px; background: #f26b0b; margin-bottom: 35px; }

                .PrimaryKeyPanel .BttnsBar2 { }
                .PrimaryKeyPanel .BttnsBar2 .BttnPK { font-size: .85em; float:left; background: #14498f; color: #fff; cursor: pointer; padding: .3em 1.3em; margin-right: 1.2em; }
                .PrimaryKeyPanel .BttnsBar2 .BttnPK:hover { background: #333; }
                .PrimaryKeyPanel .BttnsBar2 .BttnPK_Off { background: #e5e5ea; cursor: default; }
                .PrimaryKeyPanel .BttnsBar2 .BttnPK_Off:hover { background: #e5e5ea; }

                .PrimaryKeyPanel .Label2 { margin: 1.5em 1.375em .25em; color: #14498f; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopColorBar'></div>
                <div class='BttnsBar2 NoSelect'>
                    <div class='BttnPK BttnPK_Toggleable BttnPK_Off' On_Click='Save_Click'>Save</div>
                    <div class='BttnPK BttnPK_Toggleable BttnPK_Off' On_Click='Cancel_Click'>Cancel Changes</div>
                    <div class='BttnPK BttnPK_Remove' On_Click='Remove_Click'>Remove Primary Key</div>
                    <div class='clear'></div>
                </div>

                <div class='Label2'>Primary Key Fields</div>
                <table class='Holder_PrimaryKeyFields'></table>
            ";
        }
    }
}