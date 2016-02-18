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
    public class PrimaryKey
    {
        public int ConnectionId;
        public int TableId;
        public string Schema;
        public string TableName;
        public string ConstraintName;
        public PrimaryKeyColumn[] Columns;

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Validate(Enums.ValidationTypes validationType)
        {
            if (validationType == Enums.ValidationTypes.Create)
            {
                if (String.IsNullOrEmpty(this.Schema))
                    throw new Exception("Schema is empty.");
                if (String.IsNullOrEmpty(this.TableName))
                    throw new Exception("TableName is empty.");
                if (this.Columns == null || this.Columns.Length < 1)
                    throw new Exception("No columns specified.");
                ASPdb.Framework.Validation.ValidateTextForSql1(this.Schema, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.TableName, true);
            }
            else if (validationType == Enums.ValidationTypes.Drop)
            {
                if (String.IsNullOrEmpty(this.Schema))
                    throw new Exception("Schema value not provided.");
                if (String.IsNullOrEmpty(this.TableName))
                    throw new Exception("TableName value not provided.");
                if (String.IsNullOrEmpty(this.ConstraintName))
                    throw new Exception("ConstraintName value not provided.");
                ASPdb.Framework.Validation.ValidateTextForSql1(this.Schema, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.TableName, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(this.ConstraintName, true);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class PrimaryKeyColumn
    {
        public string ColumnName = "";
        public int OrdinalPosition = -1;

        public Identity Identity;
    }
}