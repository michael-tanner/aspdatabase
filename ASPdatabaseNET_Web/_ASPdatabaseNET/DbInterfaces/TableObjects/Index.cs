using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.DbInterfaces.TableObjects
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class Index
    {
        public int ConnectionId;
        public string Schema;
        public string TableName;
        public string IndexName;
        public bool IsUnique = false;
        public IndexColumn[] Columns;

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Validate(Enums.ValidationTypes validationType)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (String.IsNullOrEmpty(this.Schema))
                throw new Exception("Schema is empty.");
            if (String.IsNullOrEmpty(this.TableName))
                throw new Exception("TableName is empty.");
            ASPdb.Framework.Validation.ValidateTextForSql1(this.Schema, true);
            ASPdb.Framework.Validation.ValidateTextForSql1(this.TableName, true);


            if (validationType == Enums.ValidationTypes.NotSet
                || validationType == Enums.ValidationTypes.Select
                || validationType == Enums.ValidationTypes.Create)
            {
                if (this.Columns == null || this.Columns.Length < 1)
                    throw new Exception("No columns specified.");
            }

            if (validationType == Enums.ValidationTypes.Update
                || validationType == Enums.ValidationTypes.Drop)
            {
                if (String.IsNullOrEmpty(this.IndexName))
                    throw new Exception("IndexName value not provided.");
                ASPdb.Framework.Validation.ValidateTextForSql1(this.IndexName, true);
            }
        }
    }

    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class IndexColumn
    {
        public enum E_SortTypes { Ascending, Descending };

        public int ColumnId;
        public string ColumnName;
        public E_SortTypes SortDirection = E_SortTypes.Ascending;

    }
}