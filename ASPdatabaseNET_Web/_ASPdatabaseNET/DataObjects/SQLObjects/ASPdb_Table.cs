using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.DataObjects.ManageAssets;

namespace ASPdatabaseNET.DataObjects.SQLObjects
{
    public class ASPdb_Table
    {
        public int TableId = -1;
        public int ConnectionId;
        public string Schema;
        public string TableName;
        public bool Hide;

        public string UniqueNameKey // ex: dbo.customerorders
        {
            get
            {
                return this.Schema.ToLower().Trim() + "." + this.TableName.ToLower().Trim();
            }
        }

        public bool UseSquareBrackets_Schema
        {
            get { return ASPdb.Framework.Validation.InSqlText_DoesNameNeedSquareBrackets(this.Schema); }
        }
        public bool UseSquareBrackets_TableName
        {
            get { return ASPdb.Framework.Validation.InSqlText_DoesNameNeedSquareBrackets(this.TableName); }
        }


        //------------------------------------------------------------------------------------- constructor --
        public ASPdb_Table()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
        }
        //------------------------------------------------------------------------------------- constructor --
        public ASPdb_Table(int connectionId, string schema, string tableName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            this.ConnectionId = connectionId;
            this.Schema = schema;
            this.TableName = tableName;
        }

        //----------------------------------------------------------------------------------------------------
        public BasicAssetInfo ToBasicAssetInfo()
        {
            return new BasicAssetInfo()
            {
                Active = !this.Hide,
                AssetType = BasicAssetInfo.E_AssetTypes.Table,
                ConnectionId = this.ConnectionId,
                GenericId = this.TableId,
                Schema = this.Schema,
                GenericName = this.TableName,
                UseSquareBrackets_Schema = this.UseSquareBrackets_Schema,
                UseSquareBrackets_GenericName = this.UseSquareBrackets_TableName
            };
        }

    }
}