using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DbInterfaces.TableObjects
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ForeignKey
    {
        public enum E_CascadeOptions { NoAction, Cascade, SetNull, SetDefault }; // -- as stored in SQL Server: No Action, Cascade, Set Null, Set Default
        public enum E_RelationshipSides { NotSet, ForeignKeySide, PrimaryKeySide };


        public int ConnectionId;
        public E_RelationshipSides RelationshipSide = E_RelationshipSides.NotSet;
        public string ConstraintName;

        public string ForeignKey_Schema;
        public string ForeignKey_TableName;

        public string PrimaryKey_Schema;
        public string PrimaryKey_TableName;

        public ForeignKeyColumn[] Columns;
        public E_CascadeOptions DeleteRule = E_CascadeOptions.NoAction;
        public E_CascadeOptions UpdateRule = E_CascadeOptions.NoAction;

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Validate(Enums.ValidationTypes validationType)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (validationType == Enums.ValidationTypes.Create)
            {
                if (String.IsNullOrEmpty(this.ForeignKey_Schema))
                    throw new Exception("ForeignKey_Schema is empty.");
                if (String.IsNullOrEmpty(this.ForeignKey_TableName))
                    throw new Exception("ForeignKey_TableName is empty.");
                if (String.IsNullOrEmpty(this.PrimaryKey_Schema))
                    throw new Exception("PrimaryKey_Schema is empty.");
                if (String.IsNullOrEmpty(this.PrimaryKey_TableName))
                    throw new Exception("PrimaryKey_TableName is empty.");
                if (this.Columns == null || this.Columns.Length < 1)
                    throw new Exception("No columns specified.");
                ASPdb.Framework.Validation.ValidateTextForSql1(this.ForeignKey_Schema, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.ForeignKey_TableName, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.PrimaryKey_Schema, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.PrimaryKey_TableName, true);
            }
            else if (validationType == Enums.ValidationTypes.Drop)
            {
                if (String.IsNullOrEmpty(this.ForeignKey_Schema))
                    throw new Exception("ForeignKey_Schema is empty.");
                if (String.IsNullOrEmpty(this.ForeignKey_TableName))
                    throw new Exception("ForeignKey_TableName is empty.");
                if (String.IsNullOrEmpty(this.ConstraintName))
                    throw new Exception("ForeignKey value not provided.");
                ASPdb.Framework.Validation.ValidateTextForSql1(this.ForeignKey_Schema, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.ForeignKey_TableName, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.ConstraintName, true);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ForeignKeyColumn
    {
        public string ForeignKey_ColumnName = "";
        public string PrimaryKey_ColumnName = "";
        public int OrdinalPosition = 0;
    }
}