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
    public class Column
    {
        public enum ChangeActionTypes { NotSet, Add, Delete, Update };

        public int ConnectionId;
        public string Schema;
        public string TableName;

        public int OrdinalPosition;
        public string ColumnName = "";
        public string ColumnName_Original = "";

        public string DataType = "";
        public string DataType_Name = "";
        public int MaxLength;
        public int Precision;
        public int Scale;
        public bool IsNumericType;

        public bool AllowNulls = true;
        public string DefaultValue = null;

        public bool IsPrimaryKey;
        public bool IsIdentity;
        public Identity Identity;

        public ChangeActionTypes ChangeAction = ChangeActionTypes.NotSet;


        //----------------------------------------------------------------------------------------------------
        public void Populate__DataType_FullString()
        {
            this.IsNumericType_GetAndSet();
            string value = this.DataType_Name;
            if (value == null)
                value = "";

            switch (value.ToLower())
            {
                case "nvarchar":
                    if(this.MaxLength == -1)
                        value = String.Format("nvarchar(MAX)");
                    else
                        value = String.Format("nvarchar({0})", this.MaxLength);
                    break;
                case "decimal":
                    value = String.Format("decimal({0}, {1})", this.Precision, this.Scale);
                    break;
                case "numeric":
                    value = String.Format("numeric({0}, {1})", this.Precision, this.Scale);
                    break;
            }

            this.DataType = value;
        }
        //----------------------------------------------------------------------------------------------------
        public bool IsNumericType_GetAndSet()
        {
            this.IsNumericType = false;
            string value = this.DataType_Name_GetAndSet();
            if (value == null)
                value = "";

            // bigint   binary(50)   bit   char(10)   date   datetime   datetime2(7)  datetimeoffset(7)   decimal(18, 0)   float   geography   geometry
            // hierarchyid   image   int   money   nchar(10)   ntext   numeric(18, 0)   nvarchar(50)   nvarchar(MAX)   real   smalldatetime   
            // smallint   smallmoney   sql_variant   text   time(7)   timestamp   tinyint   uniqueidentifier   varbinary(50)   varbinary(MAX)  
            // varchar(50)   varchar(MAX)   xml

            var numericTypes = new string[] { "bigint", "binary", "bit", "decimal", "float", "int", "money", "numeric", "real", "smallint", "smallmoney", "tinyint" };
            if (numericTypes.Contains(value.ToLower().Trim()))
                this.IsNumericType = true;

            return this.IsNumericType;
        }
        //----------------------------------------------------------------------------------------------------
        public string DataType_Name_GetAndSet()
        {
            if(String.IsNullOrEmpty(this.DataType_Name))
            {
                this.DataType_Name = this.DataType.Split(new char[] { '(' }).First().Trim();
            }
            return this.DataType_Name;
        }


        //----------------------------------------------------------------------------------------------------
        public static Column Clone(Column column)
        {
            var rtn = new Column();
            rtn.ConnectionId = column.ConnectionId;
            rtn.Schema = column.Schema;
            rtn.TableName = column.TableName;
            rtn.OrdinalPosition = column.OrdinalPosition;
            rtn.ColumnName = column.ColumnName;
            rtn.ColumnName_Original = column.ColumnName_Original;
            rtn.DataType = column.DataType;
            rtn.DataType_Name = column.DataType_Name;
            rtn.MaxLength = column.MaxLength;
            rtn.Precision = column.Precision;
            rtn.Scale = column.Scale;
            rtn.AllowNulls = column.AllowNulls;
            rtn.DefaultValue = column.DefaultValue;
            rtn.IsPrimaryKey = column.IsPrimaryKey;
            rtn.IsIdentity = column.IsIdentity;
            if (column.Identity != null)
                rtn.Identity = Identity.Clone(column.Identity);
            rtn.ChangeAction = column.ChangeAction;

            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public bool HasValidContent()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (String.IsNullOrWhiteSpace(this.ColumnName))
                return false;
            if (String.IsNullOrWhiteSpace(this.DataType))
                return false;

            return true;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public bool IsEqual_ForUpdating(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (this.ColumnName != column.ColumnName) return false;
            if (this.DataType != column.DataType) return false;
            if (this.AllowNulls != column.AllowNulls) return false;
            if (this.DefaultValue != column.DefaultValue) return false;

            return true;
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Validate(Enums.ValidationTypes validationType)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            this.ColumnName = this.ColumnName.Trim();
            this.DataType = this.DataType.Trim();
            if(this.DefaultValue != null)
                this.DefaultValue = this.DefaultValue.Trim();
        }


        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Fix_DefaultValue_ForDisplay()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string value = this.DefaultValue;
            if (value == null || value.Length < 1)
                return;

            if (value.StartsWith("(N"))
                value = value.Substring(2, value.Length - 3);
            else if (value.StartsWith("(("))
                value = value.Substring(2, value.Length - 4);
            else if (value.StartsWith("("))
                value = value.Substring(1, value.Length - 2);

            if (value != "''")
                value = value.Replace("''", "'");

            this.DefaultValue = value;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Fix_DefaultValue_ForSaving()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string value = this.DefaultValue;

            if (value == null || value.Length < 1)
            {
                value = null;
            }
            else if (this.IsNumericType)
            {
                string tmp = value.Trim().Replace(".", "");
                int i;
                if (Int32.TryParse(tmp, out i))
                {
                    if (!value.StartsWith("("))
                        value = "(" + value;
                    if (!value.EndsWith(")"))
                        value = value + ")";
                }
                else
                {
                    value = null;
                }
            }
            else if (value.Trim() == "''")
            {
                value = "";
            }
            else
            {
                if (value.Contains("'"))
                    value = value.Trim();

                if (value.StartsWith("'"))
                    value = value.Substring(1, value.Length - 1);
                if (value.EndsWith("'"))
                    value = value.Substring(0, value.Length - 1);

                value = value.Replace("'", "''");
            }

            this.DefaultValue = value;
        }



    }
}