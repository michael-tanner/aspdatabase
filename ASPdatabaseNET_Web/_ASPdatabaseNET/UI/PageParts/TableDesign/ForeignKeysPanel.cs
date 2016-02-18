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
    public class ForeignKeysPanel : MRBPattern<ForeignKeysPair, TableDesign_ViewModel>
    {
        public ForeignKeysEdit ForeignKeysEdit_NewIn;
        public ForeignKeysEdit ForeignKeysEdit_NewOut;
        public ForeignKeysItem[] ForeignKeysItems_Inbound;
        public ForeignKeysItem[] ForeignKeysItems_Outbound;

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public ForeignKeysPanel(TableDesign_ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ForeignKeysPanel jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.jRoot.hide();
        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            UI.PagesFramework.BasePage.WindowResized();

            this.ForeignKeysEdit_NewIn = new ForeignKeysEdit(true, ForeignKeysEdit.RelationshipSides.Inbound, this.ViewModel);
            this.ForeignKeysEdit_NewIn.Instantiate();
            this.ForeignKeysEdit_NewIn.OnClose.After.AddHandler(this, "Item_OnEdit_Exit", 0);
            this.ForeignKeysEdit_NewIn.OnChange.After.AddHandler(this, "SubItem_Saved", 0);
            this.ForeignKeysEdit_NewIn.Close();
            jF(".Holder_Inbound_New").html("").append(this.ForeignKeysEdit_NewIn.jRoot);

            this.ForeignKeysEdit_NewOut = new ForeignKeysEdit(true, ForeignKeysEdit.RelationshipSides.Outbound, this.ViewModel);
            this.ForeignKeysEdit_NewOut.Instantiate();
            this.ForeignKeysEdit_NewOut.OnClose.After.AddHandler(this, "Item_OnEdit_Exit", 0);
            this.ForeignKeysEdit_NewOut.OnChange.After.AddHandler(this, "SubItem_Saved", 0);
            this.ForeignKeysEdit_NewOut.Close();
            jF(".Holder_Outbound_New").html("").append(this.ForeignKeysEdit_NewOut.jRoot);


            jF2(".Holder_InboundItems").html("");
            jF2(".Holder_OutboundItems").html("");
            jF2(".Inbound_Count").html("(0)");
            jF2(".Outbound_Count").html("(0)");

            var minTableStructure = this.ViewModel.Get_Minified_TableStructure();

            AjaxService.ASPdatabaseService.New(this, "GetKeys_Return").TableDesign__ForeignKeys__Get(minTableStructure);
        }
        public void GetKeys_Return(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (ajaxResponse.Error != null) { alert("Error: " + ajaxResponse.Error.Message); return; }

            this.Model = ajaxResponse.ReturnObj.As<ForeignKeysPair>();
            var holder_InboundItems = jF2(".Holder_InboundItems");
            var holder_OutboundItems = jF2(".Holder_OutboundItems");
            jF2(".Inbound_Count").html("(" + this.Model.InboundKeys.Length + ")");
            jF2(".Outbound_Count").html("(" + this.Model.OutboundKeys.Length + ")");

            this.ForeignKeysItems_Inbound = new ForeignKeysItem[0];
            this.ForeignKeysItems_Outbound = new ForeignKeysItem[0];

            for (int i = 0; i < this.Model.InboundKeys.Length; i++)
            {
                this.ForeignKeysItems_Inbound[i] = new ForeignKeysItem();
                this.ForeignKeysItems_Inbound[i].ViewModel = this.ViewModel;
                this.ForeignKeysItems_Inbound[i].Model = this.Model.InboundKeys[i];
                this.ForeignKeysItems_Inbound[i].Instantiate();
                this.ForeignKeysItems_Inbound[i].OnEdit_Enter.After.AddHandler(this, "Item_OnEdit_Enter", 0);
                this.ForeignKeysItems_Inbound[i].OnEdit_Exit.After.AddHandler(this, "Item_OnEdit_Exit", 0);
                this.ForeignKeysItems_Inbound[i].OnChange.After.AddHandler(this, "SubItem_Saved", 0);
                holder_InboundItems.append(this.ForeignKeysItems_Inbound[i].jRoot);
            }
            for (int i = 0; i < this.Model.OutboundKeys.Length; i++)
            {
                this.ForeignKeysItems_Outbound[i] = new ForeignKeysItem();
                this.ForeignKeysItems_Outbound[i].ViewModel = this.ViewModel;
                this.ForeignKeysItems_Outbound[i].Model = this.Model.OutboundKeys[i];
                this.ForeignKeysItems_Outbound[i].Instantiate();
                this.ForeignKeysItems_Outbound[i].OnEdit_Enter.After.AddHandler(this, "Item_OnEdit_Enter", 0);
                this.ForeignKeysItems_Outbound[i].OnEdit_Exit.After.AddHandler(this, "Item_OnEdit_Exit", 0);
                this.ForeignKeysItems_Outbound[i].OnChange.After.AddHandler(this, "SubItem_Saved", 0);
                holder_OutboundItems.append(this.ForeignKeysItems_Outbound[i].jRoot);
            }
        }


        //------------------------------------------------------------------------------------------ Events --
        public void NewInboundRelationship_Click()
        {
            this.Item_OnEdit_Enter();
            this.ForeignKeysEdit_NewIn.Open();
        }
        public void NewOutboundRelationship_Click()
        {
            this.Item_OnEdit_Enter();
            this.ForeignKeysEdit_NewOut.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void Item_OnEdit_Enter()
        {
            if (this.ForeignKeysItems_Inbound != null)
                for (int i = 0; i < this.ForeignKeysItems_Inbound.Length; i++)
                    this.ForeignKeysItems_Inbound[i].DisableBttns();
            if (this.ForeignKeysItems_Outbound != null)
                for (int i = 0; i < this.ForeignKeysItems_Outbound.Length; i++)
                    this.ForeignKeysItems_Outbound[i].DisableBttns();
            jF2(".NewInboundBttn").hide();
            jF2(".NewOutboundBttn").hide();
        }
        //----------------------------------------------------------------------------------------------------
        public void Item_OnEdit_Exit()
        {
            if (this.ForeignKeysItems_Inbound != null)
                for (int i = 0; i < this.ForeignKeysItems_Inbound.Length; i++)
                    this.ForeignKeysItems_Inbound[i].EnableBttns();
            if (this.ForeignKeysItems_Outbound != null)
                for (int i = 0; i < this.ForeignKeysItems_Outbound.Length; i++)
                    this.ForeignKeysItems_Outbound[i].EnableBttns();
            jF2(".NewInboundBttn").show();
            jF2(".NewOutboundBttn").show();
        }
        //----------------------------------------------------------------------------------------------------
        public void SubItem_Saved()
        {
            this.OnChange.After.Fire();
        }




        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += ForeignKeysEdit.GetCssTree();
            rtn += ForeignKeysItem.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ForeignKeysPanel { width: 100%; position:relative; padding-top: 17px; }
                .ForeignKeysPanel .TopColorBar { position:absolute; top: -1px; width: inherit; min-width: 200px; height: 8px; background: #0b94da; margin-bottom: 15px; }

                .ForeignKeysPanel .FKTable { width: 100%; }
                .ForeignKeysPanel .FKTable .Td1 { width: 50%; padding-right: 2px; }
                .ForeignKeysPanel .FKTable .Td2 { width: 7px; min-width: 7px; max-width: 7px; background: #e8e8ed; font-size: 1px; }
                .ForeignKeysPanel .FKTable .Td3 { width: 50%; padding-left: 2px; }

                .ForeignKeysPanel .AboveScrollSection { padding-bottom: 14px; padding-left: 10px; }
                .ForeignKeysPanel .AboveScrollSection .Head1 { font-size: 1.1em; margin-bottom: 11px; }
                .ForeignKeysPanel .AboveScrollSection .Head1 span { font-size: 1.4em; }
                .ForeignKeysPanel .ScrollSectionDiv {  }
                .ForeignKeysPanel .ScrollSectionDiv .Head2 { text-align: center; background: #f3f3f3; color: #999; padding: 2px 0px; margin-bottom:8px; }

                .ForeignKeysPanel .NewBttnHolderDiv { line-height: 22px; min-height: 22px; max-height: 22px; }
                .ForeignKeysPanel .Bttn { float:left; cursor:pointer; font-size: .8em; background: #14498f; color: #fff; padding: 0px 12px; }
                .ForeignKeysPanel .Bttn:hover { background: #333; }

                .ForeignKeysPanel .Inbound_Count  { color: #bbb; }
                .ForeignKeysPanel .Outbound_Count { color: #bbb; }

                .ForeignKeysPanel .Holder_InboundItems { }
                .ForeignKeysPanel .Holder_OutboundItems { }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopColorBar'></div>
                <table class='FKTable'>
                    <tr>
                        <td class='Td1'>
                            <div class='AboveScrollSection'>
                                <div class='Head1'>Relationships <span>TO</span> This Table <span class='Inbound_Count'>(0)</span></div>
                                <div class='NewBttnHolderDiv'>
                                    <div class='Bttn NewInboundBttn' On_Click='NewInboundRelationship_Click'>+ New Inbound Relationship</div>
                                </div>
                                <div class='clear'></div>
                            </div>
                            <div class='ScrollSectionDiv AutoResize'>
                                <div class='Head2 hide'>(Foreign Key Table) --&gt; (Primary Key Table)</div>
                                <div class='Holder_Inbound_New'></div>
                                <div class='Holder_InboundItems'></div>
                            </div>
                        </td>
                        <td class='Td2'>&nbsp;</td>
                        <td class='Td3'>
                            <div class='AboveScrollSection'>
                                <div class='Head1'>Relationships <span>FROM</span> This Table <span class='Outbound_Count'>(0)</span></div>
                                <div class='NewBttnHolderDiv'>
                                    <div class='Bttn NewOutboundBttn' On_Click='NewOutboundRelationship_Click'>+ New Outbound Relationship</div>
                                </div>
                                <div class='clear'></div>
                            </div>
                            <div class='ScrollSectionDiv AutoResize'>
                                <div class='Head2 hide'>(Foreign Key Table) --&gt; (Primary Key Table)</div>
                                <div class='Holder_Outbound_New'></div>
                                <div class='Holder_OutboundItems'></div>
                            </div>
                        </td>
                    </tr>
                </table>
            ";
        }
    }
}