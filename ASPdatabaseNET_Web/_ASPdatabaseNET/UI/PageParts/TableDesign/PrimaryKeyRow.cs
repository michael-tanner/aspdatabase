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
    public class PrimaryKeyRow : MRBPattern<PrimaryKeyColumn, TableDesign_ViewModel>
    {
        public Column ColumnInfo;
        public bool IsIdentity = false;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public PrimaryKeyRow()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<tr class='PrimaryKeyRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.BindUI();

            if (this.Model.Identity != null)
            {
                this.IsIdentity = true;
                jF(".Txt_Increment").val(this.Model.Identity.Increment.As<string>());
                jF(".Txt_Seed").val(this.Model.Identity.Seed.As<string>());
            }

            var thisObj = this;
            var jRootObj = jRoot;
            eval("jRootObj.find('.Txt_Seed').on('input', function(){ thisObj.IdentityPropertyChanged(); });");
            eval("jRootObj.find('.Txt_Increment').on('input', function(){ thisObj.IdentityPropertyChanged(); });");
    
            this.UpdateUI();
        }

        //----------------------------------------------------------------------------------------------------
        public void IdentityPropertyChanged()
        {
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void UpdateUI()
        {
            this.ColumnInfo = null;
            var allColumns = this.ViewModel.TableStructure.Columns;
            for (int i = 0; i < allColumns.Length; i++)
                if (allColumns[i].ColumnName.As<JsString>().toLowerCase() == this.Model.ColumnName.As<JsString>().toLowerCase())
                {
                    this.ColumnInfo = allColumns[i];
                    i = allColumns.Length + 1;
                }


            var select = jF(".Select_PrimaryKeyFields").html("<option value=''></option>");
            for (int i = 0; i < allColumns.Length; i++)
            {
                string tmp = allColumns[i].ColumnName;
                select.append("<option value='" + tmp + "'>" + tmp + "</option>");
            }
            select.val(this.Model.ColumnName);

            string dataType = "";
            if (this.ColumnInfo != null)
                dataType = this.ColumnInfo.DataType;
            jF(".Txt_DataType").html(dataType);

            this.HideIdentityUI();
            if(dataType == "int" || dataType == "bigint")
            {
                jF(".Div_IsIdentity").show();
                if (this.IsIdentity)
                {
                    jF(".Div_IsIdentity").addClass("Div_IsIdentity_On");
                    jF(".Checkbox_IsIdentity").attr("checked", true);
                    jF(".Div_IdentityProperties").show();
                }
                else
                {
                    jF(".Div_IsIdentity").removeClass("Div_IsIdentity_On");
                    jF(".Checkbox_IsIdentity").removeAttr("checked");
                    jF(".Div_IdentityProperties").hide();
                }
            }
        }


        //----------------------------------------------------------------------------------------------------
        public bool HasValue()
        {
            var value = jF(".Select_PrimaryKeyFields").val().As<string>();
            if (value != null && value != "")
                return true;

            return false;
        }
        //----------------------------------------------------------------------------------------------------
        public void HideIdentityUI()
        {
            jF(".Div_IsIdentity").hide();
            jF(".Div_IdentityProperties").hide();
        }
        public void ShowIdentityUI()
        {
            if (this.ColumnInfo != null)
                if(this.ColumnInfo.DataType == "int")
                {
                    this.IsIdentity = false;
                    this.UpdateUI();
                }
        }
        //----------------------------------------------------------------------------------------------------
        public PrimaryKeyColumn GetModel()
        {
            if (!this.HasValue())
                return null;
            var rtn = new PrimaryKeyColumn();
            rtn.ColumnName = jF(".Select_PrimaryKeyFields").val().As<string>();
            rtn.Identity = null;
            if(this.IsIdentity)
            {
                int increment = 1;
                int seed = 1;
                try
                {
                    increment = 1 * jF(".Txt_Increment").val().As<JsNumber>();
                    seed = 1 * jF(".Txt_Seed").val().As<JsNumber>();
                }
                catch { }
                rtn.Identity = new Identity();
                rtn.Identity.ConnectionId = this.ViewModel.ConnectionId;
                rtn.Identity.Schema = this.ViewModel.TableStructure.Schema;
                rtn.Identity.TableName = this.ViewModel.TableStructure.TableName;
                rtn.Identity.ColumnName = rtn.ColumnName;
                rtn.Identity.Increment = increment;
                rtn.Identity.Seed = seed;
            }
            return rtn;
        }



        //------------------------------------------------------------------------------------------ Events --
        public void Select_PrimaryKeyFields_Change()
        {
            this.Model.ColumnName = jF(".Select_PrimaryKeyFields").val().As<string>();
            this.UpdateUI();
            this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void Div_IsIdentity_Click()
        {
            this.IsIdentity = !this.IsIdentity;
            this.UpdateUI();
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
                .PrimaryKeyRow { }
                .PrimaryKeyRow td { line-height: 3em; background: #e5e5ea; border-bottom: .3em solid #fff; padding-left: 1em; }
                .PrimaryKeyRow .PKTD1 { width: 14.5em; }
                .PrimaryKeyRow .PKTD2 { font-size: .8em; width: 8em; padding-top: .3em; color: #777; }
                .PrimaryKeyRow .PKTD3 { width: 6em; }
                .PrimaryKeyRow .PKTD4 { width: 15em; font-size: .8em; line-height: 1.5em; padding-right: 1em; }

                .PrimaryKeyRow .Select_PrimaryKeyFields { font-size: .85em; width: 16em; }

                .PrimaryKeyRow .Div_IsIdentity { font-size: .8em; margin-top: 0.5em; padding: 0em .7em; line-height: 2.65625em; cursor:pointer; }
                .PrimaryKeyRow .Div_IsIdentity:hover { background: #b4b4b4; color: #fff; }
                .PrimaryKeyRow .Div_IsIdentity_On { background: #859dc0; color: #fff; }
                .PrimaryKeyRow .Div_IsIdentity .Checkbox_IsIdentity { }

                .PrimaryKeyRow .Div_IdentityProperties { margin-top: .2em; }
                .PrimaryKeyRow .Div_IdentityProperties input { width: 3em; border: 1px solid #859dc0; text-align:center; margin: .2em 0em; padding: 0em; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <td class='PKTD1'>
                    <select class='Select_PrimaryKeyFields' On_Change='Select_PrimaryKeyFields_Change'></select>
                </td>
                <td class='PKTD2 Txt_DataType'>
                </td>
                <td class='PKTD3'>
                    <div class='Div_IsIdentity' On_Click='Div_IsIdentity_Click'>
                        <input type='checkbox' class='Checkbox_IsIdentity' />
                        Identity
                    </div>
                </td>
                <td class='PKTD4'>
                    <div class='Div_IdentityProperties'>
                        <input type='text' class='Txt_Seed' value='1' /> Identity Seed
                        <br />
                        <input type='text' class='Txt_Increment' value='1' /> Identity Increment
                        <br />
                    </div>
                </td>
            ";
        }
    }
}
