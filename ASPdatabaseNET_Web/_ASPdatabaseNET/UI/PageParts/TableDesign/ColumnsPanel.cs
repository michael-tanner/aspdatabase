using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DbInterfaces.TableObjects;
using ASPdatabaseNET.DataObjects.TableDesign;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ColumnsPanel : MRBPattern<TableStructure, TableDesign_ViewModel>
    {
        public ColumnsRow[] ColumnsRows;
        public JsEvent_BeforeAfter OnHasPendingChanges = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnGotoTab = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnSaved = new JsEvent_BeforeAfter();
        public int TotalCoumnsInUI_Min = 50;
        public int TotalCoumnsInUI_MinEmpty = 10;


        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:Yes</summary>
        public ColumnsPanel(TableDesign_ViewModel viewModel)
        {
            this.ViewModel = viewModel;
            this.Instantiate();
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<div class='ColumnsPanel jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            this.jRoot.hide();

        }
        //----------------------------------------------------------------------------------------------------
        private void Open_Sub()
        {
            var saveBttn = jF2(".SaveBttn");
            var holder_ColumnRows = jF2(".Holder_ColumnRows");
            holder_ColumnRows.html("").scrollTop();

            if (this.ViewModel.IsCreateNew)
                jF(".PK_OnlyNew").show();
            else
                jF(".PK_OnlyNew").hide();

            saveBttn.hide();
            this.ColumnsRows = new ColumnsRow[0];
            if (!this.ViewModel.IsCreateNew)
            {
                for (int i = 0; i < this.Model.Columns.Length; i++)
                {
                    this.ColumnsRows[i] = new ColumnsRow(this.Model.Columns[i], this.ViewModel);
                    this.ColumnsRows[i].Model.ColumnName_Original = this.ColumnsRows[i].Model.ColumnName;
                    this.ColumnsRows[i].Model.ChangeAction = Column.ChangeActionTypes.Update;
                    this.Instantiate_AndAttachEvents(this.ColumnsRows[i]);
                    holder_ColumnRows.append(this.ColumnsRows[i].jRoot);
                }
                saveBttn.show().removeClass("SaveBttn_Active");
            }
            this.Build_ColumnsRows();


            var thisObj = this;
            eval("holder_ColumnRows.sortable({ update: function(event, ui) { thisObj.PositionChanged_ViaMouse(event, ui); } });");

            this.BindUI();
        }

        //----------------------------------------------------------------------------------------------------
        public void EnableSaveBttn()
        {
            if (!jF2(".SaveBttn").hasClass("SaveBttn_Active"))
                jF2(".SaveBttn").addClass("SaveBttn_Active");
            this.OnHasPendingChanges.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public TableStructure SaveToModel()
        {
            this.Model.Columns = new Column[0];
            for (int i = 0; i < this.ColumnsRows.Length; i++)
            {
                this.Model.Columns[i] = this.ColumnsRows[i].SaveToModel();
            }

            return this.Model;
        }
       
        
        //----------------------------------------------------------------------------------------------------
        private ColumnsRow AddEmptyColumnToUI(int columnPosition, bool insertAtBottom, jQuery or_InsertBeforeThisElement)
        {
            var rtn = new ColumnsRow(new Column(), this.ViewModel);
            rtn.Model.OrdinalPosition = columnPosition;
            rtn.Model.ChangeAction = Column.ChangeActionTypes.Add;
            this.Instantiate_AndAttachEvents(rtn);

            if (insertAtBottom)
                jF2(".Holder_ColumnRows").append(rtn.jRoot);
            else
                rtn.jRoot.insertBefore(or_InsertBeforeThisElement);

            return rtn;
        }
        private void Instantiate_AndAttachEvents(ColumnsRow columnsRow)
        {
            columnsRow.Instantiate();
            columnsRow.OnInsertBefore.After.AddHandler(this, "OnInsertBefore", 1);
            columnsRow.OnRequestRemove.After.AddHandler(this, "OnRequestRemove", 1);
            columnsRow.OnColumnPositionChange.After.AddHandler(this, "PositionChanged_ViaTextBox", 1);
            columnsRow.OnGotoTab.After.AddHandler(this, "OnGotoTab_Method", 1);
            columnsRow.OnColumnNameBlur.After.AddHandler(this, "OnColumnNameBlur_Method", 0);
            columnsRow.OnChange.After.AddHandler(this, "OnColumnsRowChange", 0);
            columnsRow.OnUpOrDownArrow.After.AddHandler(this, "OnColumn_UpOrDownArrow", 2);
        }
        private void Build_ColumnsRows()
        {
            int insertCounter1 = this.TotalCoumnsInUI_Min - jF2(".ColumnsRow").length;
            if (insertCounter1 > 0)
                for (int i = 0; i < insertCounter1; i++)
                    this.AddEmptyColumnToUI(0, true, null);

            int insertCounter2 = this.TotalCoumnsInUI_MinEmpty - this.HowManyBottomColumnsAreEmpty();
            if (insertCounter2 > 0)
                for (int i = 0; i < insertCounter2; i++)
                    this.AddEmptyColumnToUI(0, true, null);

            this.ColumnsRows = new ColumnsRow[0];
            var elements = jF2(".ColumnsRow");
            for (int i = 0; i < elements.length; i++)
            {
                this.ColumnsRows[i] = ColumnsRow.Get_UIObject(elements[i]);
                this.ColumnsRows[i].ColumnPosition_Property = i + 1;
            }
        }
        private int HowManyBottomColumnsAreEmpty()
        {
            int rtn = 0;
            var elements = jF2(".ColumnsRow");
            for (int i = elements.length - 1; i >= 0; i--)
            {
                var columnsRow = ColumnsRow.Get_UIObject(elements[i]);
                if (JsStr.S(columnsRow.ColumnName_Property).Trim().String.Length < 1)
                    rtn++;
                else
                    i = -1; // end loop;
            }
            return rtn;
        }



        //------------------------------------------------------------------------------------------ Events --
        public void OnInsertBefore(ColumnsRow columnsRow)
        {
            var new_ColumnsRow = this.AddEmptyColumnToUI(columnsRow.ColumnPosition_Property, false, columnsRow.jRoot);

            this.Build_ColumnsRows();

            this.EnableSaveBttn();
        }
        public void OnRequestRemove(ColumnsRow columnsRow)
        {
            columnsRow.jRoot.detach();
            this.Build_ColumnsRows();
            this.EnableSaveBttn();
        }
        public void OnGotoTab_Method(string tabType)
        {
            this.OnGotoTab.After.Fire1(tabType);
        }
        public void OnColumnNameBlur_Method()
        {
            this.Build_ColumnsRows();
        }
        public void OnColumnsRowChange()
        {
            this.EnableSaveBttn();
        }
        public void OnColumn_UpOrDownArrow(int indexRow, string classNameToFocus)
        {
            if (indexRow >= 0 && indexRow < this.ColumnsRows.Length)
                this.ColumnsRows[indexRow].PutFocusOn(classNameToFocus);
        }

        public void PositionChanged_ViaMouse(object jQ_Event, object ui)
        {
            var elements = jF2(".ColumnsRow");
            for (int i = 0; i < elements.length; i++)
            {
                int new_ColumnPosition = i + 1;
                var columnsRow = ColumnsRow.Get_UIObject(elements[i]);
                if (columnsRow != null)
                {
                    columnsRow.ColumnPosition_Property = new_ColumnPosition;
                }
            }
            this.Build_ColumnsRows();
            this.EnableSaveBttn();
        }
        public void PositionChanged_ViaTextBox(ColumnsRow columnsRow)
        {
            columnsRow.jRoot.detach();
            int old_Position = columnsRow.Model.OrdinalPosition;
            int new_Position = columnsRow.ColumnPosition_Property;

            if (new_Position < 1)
                new_Position = 1;
            if (new_Position > old_Position)
                new_Position++;

            int index_ToInsertBefore = new_Position - 1;
            if (index_ToInsertBefore < this.ColumnsRows.Length)
                columnsRow.jRoot.insertBefore(this.ColumnsRows[index_ToInsertBefore].jRoot);
            else
            {
                columnsRow.ColumnPosition_Property = this.ColumnsRows.Length + 1;
                columnsRow.jRoot.insertAfter(this.ColumnsRows[this.ColumnsRows.Length - 1].jRoot);
            }
            this.Build_ColumnsRows();
            this.EnableSaveBttn();
        }
        //----------------------------------------------------------------------------------------------------
        public void SaveBttn_Click()
        {
            if (!jF2(".SaveBttn").hasClass("SaveBttn_Active"))
                return;

            this.Model.Columns = new Column[0];
            for (int i = 0; i < this.ColumnsRows.Length; i++)
            {
                this.ColumnsRows[i].SaveToModel();
                this.Model.Columns[i] = this.ColumnsRows[i].Model;
            }
            ASPdatabaseNET.AjaxService.ASPdatabaseService.New(this, "Save_AjaxReturn").TableDesign__UpdateTable(this.Model);
        }
        public void Save_AjaxReturn(ASPdb.Ajax.AjaxResponse ajaxResponse)
        {
            if (UI.Errors.ExceptionHandler.Check(ajaxResponse))
                return;

            this.OnSaved.After.Fire1("Columns");
        }

        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += GetCssRoot();
            rtn += ColumnsRow.GetCssTree();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ColumnsPanel { position:relative; background: #979aa5; }
                .ColumnsPanel .TopCorrectiveLine { position:absolute; top: -1px; width: 100%; height: 3px; background: #979aa5; }

                .ColumnsPanel .SaveBttn { display:block; position:absolute; background: #979aa5; color: #b9bcca; border: 1px solid #c9ccd7;
                                          top: 9px; right: 8px; line-height: 35px; padding: 0px 33px; cursor: default; z-index: 400; }

                .ColumnsPanel .SaveBttn_Active { background: #14498f; color: #fff; cursor: pointer; border-color: #fff; }
                .ColumnsPanel .SaveBttn_Active:hover { background: #e14738; }

                .ColumnsPanel .MainEditBox_Padding { padding: 27px 8px 20px 5px; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox { background: #fff; position:relative; }

                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels { position:absolute; top: 0px; right: 0px; width: 881px; z-index: 300; overflow: visible;
                                                                              color: #0c5eca; background: #fff; border-bottom: 2px solid #747474; font-size: .9em; font-weight: bold; line-height: 35px; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .Tip_PrimaryKey { position:absolute; font-size: .75em; background: #f26b0b; color: #fff; border-radius: .6em;
                                                                                              left: 6.75em; top: -1em;
                                                                                              font-weight: normal; line-height: 1.5em; padding: 0em .7em; z-index: 2; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .PK_Line1 { position:absolute; left: 7.7em; top: 0em; background: #f26b0b; width: 0.13888em; height: 2.3em; z-index: 1; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .PK_Line1  .PK_Line2 { position:absolute; bottom: 0.069444em; left: -0.069444em; width: 0.2777em; height: 0.138888em; background: #f26b0b; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .HeadLabel1 { width: auto; position:absolute; top: 2px; left: 22px;  font-size: .75em; text-align:center; line-height: 15px; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .HeadLabel2 { width: auto; position:absolute; top: 0px; left: 155px; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .HeadLabel3 { width: auto; position:absolute; top: 0px; left: 350px; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .HeadLabel4 { width: auto; position:absolute; top: 2px; left: 504px; font-size: .75em; text-align:center; line-height: 15px; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .HeadLabels .HeadLabel5 { width: auto; position:absolute; top: 0px; left: 567px; }



                .ColumnsPanel .MainEditBox_Padding .MainEditBox .Spacer1 { height: 37px; background: #979aa5; }
                .ColumnsPanel .MainEditBox_Padding .MainEditBox .Holder_ColumnRows { padding-top: 0px; overflow-y: scroll; height: 200px; background: #979aa5; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='TopCorrectiveLine'></div>
                <div class='SaveBttn SaveBttn_Active' On_Click='SaveBttn_Click'>Save Changes</div>
                <div class='MainEditBox_Padding'>
                    <div class='MainEditBox'>
                        <div class='HeadLabels'>&nbsp;
                            <div class='Tip_PrimaryKey PK_OnlyNew'>Primary Key</div>
                            <div class='PK_Line1 PK_OnlyNew'><div class='PK_Line2'><span></span></div></div>

                            <div class='HeadLabel1'>Column<br />Position</div>
                            <div class='HeadLabel2'>Column Name</div>
                            <div class='HeadLabel3'>Data Type</div>
                            <div class='HeadLabel4'>Allow<br />Nulls</div>
                            <div class='HeadLabel5'>Default Value</div>
                            <div class='clear'></div>
                        </div>
                        <div class='Spacer1'></div>
                        <ul class='Holder_ColumnRows AutoResize' AutoResize_BottomSpace='20'></ul>
                    </div>
                </div>
            ";
        }
    }
}