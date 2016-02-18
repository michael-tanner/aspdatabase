using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.DataObjects.ManageAssets;

namespace ASPdatabaseNET.DataObjects.SQLObjects
{
    //----------------------------------------------------------------------------------------------------////
    public class ASPdb_View
    {
        public int ViewId = -1;
        public int ConnectionId;
        public string Schema;
        public string ViewName;
        public bool Hide;

        public string UniqueNameKey // ex: dbo.customerorders
        {
            get
            {
                AjaxService.ASPdatabaseService.GetSetVal();
                return this.Schema.ToLower().Trim() + "." + this.ViewName.ToLower().Trim();
            }
        }

        public bool UseSquareBrackets_Schema
        {
            get { return ASPdb.Framework.Validation.InSqlText_DoesNameNeedSquareBrackets(this.Schema); }
        }
        public bool UseSquareBrackets_ViewName
        {
            get { return ASPdb.Framework.Validation.InSqlText_DoesNameNeedSquareBrackets(this.ViewName); }
        }

        //------------------------------------------------------------------------------------- constructor --
        public ASPdb_View()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
        }
        //------------------------------------------------------------------------------------- constructor --
        public ASPdb_View(int connectionId, string schema, string viewName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            this.ConnectionId = connectionId;
            this.Schema = schema;
            this.ViewName = viewName;
        }

        //----------------------------------------------------------------------------------------------------
        public BasicAssetInfo ToBasicAssetInfo()
        {
            return new BasicAssetInfo()
            {
                Active = !this.Hide,
                AssetType = BasicAssetInfo.E_AssetTypes.SqlView,
                ConnectionId = this.ConnectionId,
                GenericId = this.ViewId,
                Schema = this.Schema,
                GenericName = this.ViewName,
                UseSquareBrackets_Schema = this.UseSquareBrackets_Schema,
                UseSquareBrackets_GenericName = this.UseSquareBrackets_ViewName
            };
        }

    }
}