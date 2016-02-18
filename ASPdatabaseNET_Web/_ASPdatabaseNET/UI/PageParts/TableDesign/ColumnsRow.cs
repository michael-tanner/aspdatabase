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
    public class ColumnsRow : MRBPattern<Column, TableDesign_ViewModel>
    {
        public JsEvent_BeforeAfter OnInsertBefore = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnColumnPositionChange = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnRequestRemove = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnGotoTab = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnColumnNameBlur = new JsEvent_BeforeAfter();
        public JsEvent_BeforeAfter OnUpOrDownArrow = new JsEvent_BeforeAfter();

        public Column Original_ColumnValues;

        public ColumnsRow_TypeSelector TypeSelector;

        public bool IsInt(int value)
        {
            if (isNaN(value) || (" " + value).As<JsString>().split(".").length > 1)
                return false;
            return true;
        }
        public int ColumnPosition_Property
        {
            get
            {
                int value = jF2(".Txt_ColumnPosition").val().As<JsNumber>();
                if (!this.IsInt(value))
                    this.ColumnPosition_Property = this.Model.OrdinalPosition;
                value = jF2(".Txt_ColumnPosition").val().As<JsNumber>();
                return value;
            }
            set
            {
                if (IsInt(value))
                {
                    jF2(".Txt_ColumnPosition").val(value.As<string>());
                    if(this.Model != null)
                        this.Model.OrdinalPosition = value;

                    jRoot[0].style.zIndex = (10000 - value).As<string>(); ;
                }
            }
        }
        public string ColumnName_Property
        {
            get
            {
                return jF2(".Txt_ColumnName").val().As<string>();
            }
            set
            {
                jF2(".Txt_ColumnName").val(value);
            }
        }
        public string DataType_Property
        {
            get
            {
                return jF2(".Txt_DataType").val().As<string>();
            }
            set
            {
                jF2(".Txt_DataType").val(value);
            }
        }
        public bool AllowNulls_Property
        {
            get
            {
                if (jF2(".CheckBox_AllowNulls").attr("checked") == "checked")
                    return true;
                else
                    return false;
            }
            set
            {
                if (value)
                    jF2(".CheckBox_AllowNulls").attr("checked", "checked");
                else
                    jF2(".CheckBox_AllowNulls").attr("checked", false);
            }
        }
        public string DefaultValue_Property
        {
            get
            {
                return jF2(".Txt_DefaultValue").val().As<string>();
            }
            set
            {
                jF2(".Txt_DefaultValue").val(value);
            }
        }

        //------------------------------------------------------------------------------------- Constructor --
        /// <summary>Instantiate:No</summary>
        public ColumnsRow(Column model, TableDesign_ViewModel viewModel)
        {
            this.Model = model;
            this.ViewModel = viewModel;
        }
        //----------------------------------------------------------------------------------------------------
        public static ColumnsRow Get_UIObject(HtmlElement htmlElement)
        {
            ColumnsRow rtn = null;
            try
            {
                eval("rtn = htmlElement.UIObject;");
            }
            catch { }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public void Instantiate_Sub()
        {
            this.jRoot = J("<li class='ColumnsRow jRoot'>");
            this.jRoot.append(this.GetHtmlRoot());
            var tmpJRoot = this.jRoot;
            var thisObj = this;
            eval("tmpJRoot[0].UIObject = thisObj;");

            this.Original_ColumnValues = Column.Clone(this.Model);

            if (this.ViewModel.IsCreateNew)
            {
                jF2(".PK_CheckBox").show();
                jF2(".Txt_ColumnPosition").val(this.Model.OrdinalPosition.As<string>());
                jF2(".Txt_ColumnName").val("");
                jF2(".Txt_DataType").val("");
                jF2(".Txt_DefaultValue").val("");
            }
            else
            {
                if (this.Model.IsPrimaryKey)
                    jF2(".PK_Selected").show();
                else 
                    jF2(".PK_Empty").show();

                this.AllowNulls_Property = this.Model.AllowNulls;
                this.DefaultValue_Property = this.Model.DefaultValue;

                string str_FK_Counts = this.IfHas_ForeignKey_ReturnCounts();
                if (str_FK_Counts != null)
                    jF2(".FK_Icon").show().attr("title", str_FK_Counts);

                string str_IX_Counts = this.IfHas_Index_ReturnCount();
                if (str_IX_Counts != null)
                    jF2(".IX_Icon").show().attr("title", str_IX_Counts);
            }

            this.TypeSelector = new ColumnsRow_TypeSelector();
            this.TypeSelector.ViewModel = this.ViewModel;
            this.TypeSelector.Instantiate();
            this.TypeSelector.OnChange.After.AddHandler(this, "TypeSelector_OnChange", 0);
            this.TypeSelector.Close();
            jF(".Holder_TypeSelector").append(this.TypeSelector.jRoot);


            this.BindUI();
        }
        //----------------------------------------------------------------------------------------------------
        private void ConnectEvents_Sub()
        {
            Evt.Attach_ToElement("mousedown", this, jRoot[0], "OnMouseDown", null);
            Evt.Attach_ToElement("mouseup", this, jRoot[0], "OnMouseUp", null);
            //Evt.Attach_ToElement("mouseleave", this, jRoot[0], "OnMouseUp", null);

            Evt.Attach_ToElement("blur", this, jF2(".Txt_ColumnPosition")[0], "ColumnPosition_Blur", null);
            Evt.Attach_ToElement("blur", this, jF2(".Txt_ColumnName")[0], "ColumnName_Blur", null);
            Evt.Attach_ToElement("focus", this, jF2(".Txt_DataType")[0], "Txt_DataType_Focus", null);
            Evt.Attach_ToElement("blur", this, jF2(".Txt_DataType")[0], "Txt_DataType_Blur", null);
            Evt.Attach_ToElement("blur", this, jF2(".CheckBox_AllowNulls")[0], "AnyField_Blur", null);
            Evt.Attach_ToElement("click", this, jF2(".CheckBox_AllowNulls")[0], "AnyField_Blur", null);
            Evt.Attach_ToElement("blur", this, jF2(".Txt_DefaultValue")[0], "AnyField_Blur", null);

            var thisObj = this;
            var jRootObj = this.jRoot;
            eval("jRootObj.find('.Txt_ColumnPosition').keydown(function(event) { thisObj.OnKeyDown_Generic(event, '.Txt_ColumnPosition') });");
            eval("jRootObj.find('.Txt_ColumnName').keydown(function(event) { thisObj.OnKeyDown_Generic(event, '.Txt_ColumnName') });");
            eval("jRootObj.find('.Txt_DataType').keydown(function(event) { thisObj.OnKeyDown_Generic(event, '.Txt_DataType'); thisObj.Txt_DataType_Changed(); });");
            //eval("jRootObj.find('.CheckBox_AllowNulls').keydown(function(event) { thisObj.OnKeyDown_Generic(event, '.CheckBox_AllowNulls') });");
            eval("jRootObj.find('.Txt_DefaultValue').keydown(function(event) { thisObj.OnKeyDown_Generic(event, '.Txt_DefaultValue') });");
        }
        //----------------------------------------------------------------------------------------------------
        public string IfHas_ForeignKey_ReturnCounts()
        {
            int counter_PK = 0;
            int counter_FK = 0;
            if (this.Model == null || this.ViewModel == null || this.ViewModel.TableStructure == null)
                return null;

            var pk_List = this.ViewModel.TableStructure.ForeignKeys_PK;
            if(pk_List != null)
                for (int i = 0; i < pk_List.Length; i++)
                {
                    var cols = pk_List[i].Columns;
                    for (int j = 0; j < cols.Length; j++)
                        if (cols[j].PrimaryKey_ColumnName == this.Model.ColumnName)
                            counter_PK++;
                }

            var fk_List = this.ViewModel.TableStructure.ForeignKeys_FK;
            if(fk_List != null)
                for (int i = 0; i < fk_List.Length; i++)
                {
                    var cols = fk_List[i].Columns;
                    for (int j = 0; j < cols.Length; j++)
                        if (cols[j].ForeignKey_ColumnName == this.Model.ColumnName)
                            counter_FK++;
                }

            if (counter_PK + counter_FK == 0)
                return null;
            else
                return "PK Count: " + counter_PK + "\nFK Count: " + counter_FK + "";
        }
        public string IfHas_Index_ReturnCount()
        {
            int counter = 0;
            if (this.Model == null || this.ViewModel == null || this.ViewModel.TableStructure == null || this.ViewModel.TableStructure.Indexes == null)
                return null;

            var ix_List = this.ViewModel.TableStructure.Indexes;
            for (int i = 0; i < ix_List.Length; i++)
            {
                var cols = ix_List[i].Columns;
                for (int j = 0; j < cols.Length; j++)
                    if (cols[j].ColumnName == this.Model.ColumnName)
                        counter++;
            }
            if (counter == 0)
                return null;
            else
                return "Index Count: " + counter + "";
        }


        //----------------------------------------------------------------------------------------------------
        public bool HasValidContent()
        {
            var rtn = true;


            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public Column SaveToModel()
        {
            this.Model.OrdinalPosition = this.ColumnPosition_Property;
            this.Model.ColumnName = this.ColumnName_Property;
            this.Model.DataType = this.DataType_Property; // jF2(".Txt_DataType").val().As<string>();
            this.Model.AllowNulls = this.AllowNulls_Property; // (jF2(".CheckBox_AllowNulls").attr("checked") == "checked");




            int seed = 1, increment = 1;
            if (((JsString)this.Model.DataType).toLowerCase().indexOf("identity") > -1) // int - Identity(0,0)
            {
                try
                {
                    var arr1 = ((JsString)this.Model.DataType).split("(");
                    var arr2 = arr1[1].split(")")[0].split(",");
                    seed = arr2[0].trim().As<JsNumber>();
                    increment = arr2[1].trim().As<JsNumber>();
                }
                catch { }
                if (((JsString)this.Model.DataType).toLowerCase().indexOf("bigint") > -1)
                    this.Model.DataType = "id64";
                else
                    this.Model.DataType = "id";
            }
            if (this.Model.DataType == "id")
            {
                this.Model.DataType = "int";
                this.Model.IsIdentity = true;
                this.Model.Identity = new Identity();
                this.Model.Identity.ColumnName = this.Model.ColumnName;
                this.Model.Identity.Seed = seed;
                this.Model.Identity.Increment = increment;
            }
            else if (this.Model.DataType == "id64")
            {
                this.Model.DataType = "bigint";
                this.Model.IsIdentity = true;
                this.Model.Identity = new Identity();
                this.Model.Identity.ColumnName = this.Model.ColumnName;
                this.Model.Identity.Seed = seed;
                this.Model.Identity.Increment = increment;
            }

            this.Model.DefaultValue = null;
            if (this.DefaultValue_Property.Length > 0)
                this.Model.DefaultValue = this.DefaultValue_Property;

            return this.Model;
        }

        //------------------------------------------------------------------------------------------ Events --
        public void PK_CheckBox_Click()
        {
            this.Model.IsPrimaryKey = true;
            jF2(".PK_CheckBox").hide();
            jF2(".PK_Selected").show();
            jF2(".Txt_ColumnName").focus();
        }
        public void IconClici_PK()
        {
            if (this.ViewModel.IsCreateNew)
            {
                this.Model.IsPrimaryKey = false;
                jF2(".PK_Selected").hide();
                jF2(".PK_CheckBox").show();
                jF2(".PK_CheckBoxInput").attr("checked", false);
                jF2(".Txt_ColumnName").focus();
            }
            else
                this.OnGotoTab.After.Fire1("PK");
        }
        public void IconClici_FK()
        {
            this.OnGotoTab.After.Fire1("FK");
        }
        public void IconClici_IX()
        {
            this.OnGotoTab.After.Fire1("IX");
        }
        //----------------------------------------------------------------------------------------------------
        public void BttnInsert_Click()
        {
            this.OnInsertBefore.After.Fire1(this);
        }
        public void BttnDelete_Click_1()
        {
            if (this.Model.ChangeAction == Column.ChangeActionTypes.Add)
                this.OnRequestRemove.After.Fire1(this);
            else
            {
                this.Model.ChangeAction = Column.ChangeActionTypes.Delete;
                jF2(".DeleteBttn").hide();
                jF2(".DeleteBttn_Clicked").show();
                jF2(".IconsWrapper").hide();
                this.OnChange.After.Fire();
            }
        }
        public void BttnDelete_Click_2()
        {
            this.Model.ChangeAction = Column.ChangeActionTypes.Update;
            jF2(".DeleteBttn_Clicked").hide();
            jF2(".DeleteBttn").show();
            jF2(".IconsWrapper").show();
        }
        //----------------------------------------------------------------------------------------------------
        public void OnMouseDown()
        {
            jRoot.addClass("ColumnsRow_MouseDown");
            J(".GlobalClass_ColumnsRow_InsertBttn").addClass("BttnInsert_Faded");
            jF2(".LeftPadding").hide();
            //jF2(".BttnInsert").hide();
            jF("input").blur();
        }
        public void OnMouseUp()
        {
            jRoot.removeClass("ColumnsRow_MouseDown");
            J(".GlobalClass_ColumnsRow_InsertBttn").removeClass("BttnInsert_Faded");
            jF2(".LeftPadding").show();
            //jF2(".BttnInsert").show();
        }
        public void ColumnPosition_Blur()
        {
            if (this.ColumnPosition_Property == this.Model.OrdinalPosition)
                return;
            else
                OnColumnPositionChange.After.Fire1(this);
        }
        public void ColumnName_Blur()
        {
            this.OnColumnNameBlur.After.Fire();
            this.AnyField_Blur();
        }
        public void AnyField_Blur()
        {
            bool somethingChanged = false;
            if (this.ColumnName_Property != this.Original_ColumnValues.ColumnName)
                somethingChanged = true;
            else if (this.DataType_Property != this.Original_ColumnValues.DataType)
                somethingChanged = true;
            else if (this.AllowNulls_Property != this.Original_ColumnValues.AllowNulls)
                somethingChanged = true;
            else if (this.DefaultValue_Property != this.Original_ColumnValues.DefaultValue)
            {
                if (this.DefaultValue_Property == "" && this.Original_ColumnValues.DefaultValue == null)
                { }
                else { somethingChanged = true; }
            }

            if (somethingChanged)
                this.OnChange.After.Fire();
        }
        //----------------------------------------------------------------------------------------------------
        public void OnKeyDown_Generic(Event evt, string className)
        {
            switch (evt.keyCode)
            {
                case 27: // Esc
                    jF2(className).blur();
                    break;
                case 37: // Left
                    break;
                case 38: // Up
                    this.OnUpOrDownArrow.After.Fire2(this.ColumnPosition_Property - 2, className);
                    break;
                case 39: // Right
                    break;
                case 40: // Down
                    this.OnUpOrDownArrow.After.Fire2(this.ColumnPosition_Property, className);
                    break;
                case 13: // Enter
                    break;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public void PutFocusOn(string className)
        {
            jF2(className).focus();
        }

        
        //----------------------------------------------------------------------------------------------------
        public void Txt_DataType_Focus()
        {
            this.TypeSelector.Open();
        }
        //----------------------------------------------------------------------------------------------------
        public void Txt_DataType_Blur()
        {
            this.AnyField_Blur();

            this.TypeSelector.DelayClose();
        }
        //----------------------------------------------------------------------------------------------------
        public void Txt_DataType_Changed()
        {

        }
        //----------------------------------------------------------------------------------------------------
        public void TypeSelector_OnChange()
        {
            this.DataType_Property = this.TypeSelector.Model;
            this.AnyField_Blur();
            if (this.TypeSelector.IsOpen)
                this.TypeSelector.Close();
        }


        //-------------------------------------------------------------------------------------- CSS & HTML --
        public new static string GetCssTree()
        {
            string rtn = "";
            rtn += ColumnsRow_TypeSelector.GetCssTree();
            rtn += GetCssRoot();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public new static string GetCssRoot()
        {
            return @"
                .ColumnsRow { position:relative; line-height: 28px; border-bottom: 1px solid #d6d6d6; padding-left: 52px; background: #fff; }

                .ColumnsRow .BttnInsert { position: absolute; left: 0px; top: 0px;
                                          line-height: 15px; width: 47px; 
                                          border-radius: 7px;  border: 1px solid #6f7074;
                                          font-size: .65em; text-align:center; background: #d7d8dc; color: #6f7074; cursor:pointer; z-index: 100; }
                .ColumnsRow .BttnInsert:hover { background: #333; color: #fff; border-color: #333; }
                .ColumnsRow .BttnInsert_Faded { opacity: .3; }


                .ColumnsRow .LeftPadding { position:absolute; top: 0px; left: 0px; height: 30px; width:52px; background: #979aa5; z-index: 50; }

                .ColumnsRow .PositionHandle { position:relative; float:left; width: 85px; color: #507bb3; cursor:pointer; }
                .ColumnsRow .PositionHandle:hover { }
                .ColumnsRow .PositionHandle .SixDots { width: 42px; text-align:center; }
                .ColumnsRow .PositionHandle .Txt_ColumnPosition { position:absolute; top: 4px; right: 11px;
                                                                  width:32px; line-height: 18px; height: 18px; color: #6d6d6d; padding: 0px; 
                                                                  border: 1px solid #e1e1e1; text-align: center; font-size: .75em; }

                .ColumnsRow .PK_IconHolder { float:left; width: 55px; height: 28px; border-right: 1px solid #e8e8e8; text-align:center; font-size: .7em; }
                .ColumnsRow .PK_IconHolder .PK_Empty { display:none; }
                .ColumnsRow .PK_IconHolder .PK_Selected { display:none; line-height: 21px; height: 21px; width: 23px; margin:auto; margin-top: 4px; margin-bottom: 0px; 
                                                          background: #f26b0b; color: #fff; border-radius: 2px; cursor:pointer; }
                .ColumnsRow .PK_IconHolder .PK_CheckBox { display:none; line-height: 21px; height: 21px; width: 27px; margin:auto; padding-top: 6px; }
                .ColumnsRow .PK_IconHolder .PK_CheckBox .PK_CheckBoxInput { }

                .ColumnsRow .Div_ColumnName { float:left; width: 195px; border-right: 1px solid #e8e8e8; }
                .ColumnsRow .Div_ColumnName .Txt_ColumnName { width: 185px; line-height: 26px; height: 26px; padding: 0px 5px; font-size: .8em; }

                .ColumnsRow .Div_DataType { position:relative; float:left; width: 154px; border-right: 1px solid #e8e8e8; }
                .ColumnsRow .Div_DataType .Txt_DataType { width: 144px; line-height: 26px; height: 26px; padding: 0px 5px; font-size: .8em; }
                .ColumnsRow .Div_DataType .Holder_TypeSelector { position:absolute; }

                .ColumnsRow .Div_AllowNulls { float:left; width: 55px; border-right: 1px solid #e8e8e8; text-align: center; }
                .ColumnsRow .Div_AllowNulls .CheckBox_AllowNulls {  }

                .ColumnsRow .Div_DefaultValue { float:left; width: 190px; border-right: 1px solid #e8e8e8; }
                .ColumnsRow .Div_DefaultValue .Txt_DefaultValue { width: 180px; line-height: 26px; height: 26px; padding: 0px 5px; font-size: .8em; }

                .ColumnsRow .RightSideIcons { position:relative; float:right; font-size: .7em; text-align:center; }
                .ColumnsRow .RightSideIcons .FK_Icon { display:none; position:absolute; top: 3px; right: 97px;
                                                       line-height: 21px; height: 21px; width: 23px; 
                                                       background: #0b94da; color: #fff; border-radius: 2px; cursor:pointer; }
                .ColumnsRow .RightSideIcons .IX_Icon { display:none; position:absolute; top: 3px; right: 68px; 
                                                       line-height: 21px; height: 21px; width: 23px; 
                                                       background: #6cc5aa; color: #fff; border-radius: 2px; cursor:pointer; }
                .ColumnsRow .RightSideIcons .DeleteBttn { position:absolute; top: 7px; right: 5px; line-height: 13px; width: 52px; cursor:pointer;  
                                                          color: #5d5d5d; border: 1px solid #ccc; border-radius: 7px; font-size: .9em; }
                .ColumnsRow .RightSideIcons .DeleteBttn:hover { background: #194e8d; color: #fff; border-color: #194e8d; }
                .ColumnsRow .RightSideIcons .DeleteBttn_Clicked { display:none; position:absolute; top: 7px; right: 5px; z-index:500;
                                                                  line-height: 13px; width: 110px; cursor:pointer; background: #194e8d;
                                                                  color: #fff; border: 1px solid #194e8d; border-radius: 7px; font-size: .9em; }
                .ColumnsRow .RightSideIcons .DeleteBttn_Clicked:hover { background: #111; }



                .ColumnsRow_MouseDown { background:#c6d8f1; margin-left: 0px; padding-left: 52px; }
                .ColumnsRow_MouseDown input { background:#c6d8f1; }
            ";
        }
        //----------------------------------------------------------------------------------------------------
        public new string GetHtmlRoot()
        {
            return @"
                <div class='BttnInsert GlobalClass_ColumnsRow_InsertBttn' On_Click='BttnInsert_Click'>Insert</div>
                <div class='LeftPadding'>&nbsp;</div>
                
                <div class='PositionHandle'>
                    <div class='SixDots'>:::</div>
                    <input type='text' class='Txt_ColumnPosition' ModelKey='OrdinalPosition' />
                </div>

                <div class='PK_IconHolder'>
                    <div class='PK_Empty'>&nbsp;</div>
                    <div class='PK_Selected' On_Click='IconClici_PK'>PK</div>
                    <div class='PK_CheckBox' On_Click='PK_CheckBox_Click'>
                        <input type='checkbox' class='PK_CheckBoxInput' On_Click='PK_CheckBox_Click' />
                    </div>
                </div>

                <div class='Div_ColumnName'>
                    <input type='text' class='Txt_ColumnName' ModelKey='ColumnName' />
                </div>

                <div class='Div_DataType'>
                    <input type='text' class='Txt_DataType' ModelKey='DataType' />
                    <div class='Holder_TypeSelector'></div>
                </div>

                <div class='Div_AllowNulls'>
                    <input type='checkbox' class='CheckBox_AllowNulls' />
                </div>

                <div class='Div_DefaultValue'>
                    <input type='text' class='Txt_DefaultValue' />
                </div>

                <div class='RightSideIcons'>
                    <div class='IconsWrapper'>
                        <div class='FK_Icon' On_Click='IconClici_FK'>FK</div>
                        <div class='IX_Icon' On_Click='IconClici_IX'>IX</div>
                    </div>
                    <div class='DeleteBttn' On_Click='BttnDelete_Click_1'>Delete</div>      
                    <div class='DeleteBttn_Clicked' On_Click='BttnDelete_Click_2'>Delete On Save</div>      
                </div>

                <div class='clear'></div>
            ";
        }

    }
}