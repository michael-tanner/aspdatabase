using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET;
using ASPdatabaseNET.DataObjects.ManageAssets;
using ASPdatabaseNET.DbInterfaces;

namespace ASPdatabaseNET.DataAccess
{
    //----------------------------------------------------------------------------------------------------////
    public class ManageAssetsCRUD
    {
        //----------------------------------------------------------------------------------------------------
        public static ManageAssetResponse GetAssetsLists(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new AssetsLists_PopulatorHelper(connectionId, true);
            var aspdb_Connection = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId);
            if (!aspdb_Connection.Active)
                throw new Exception("This connection is inactive.");

            rtn.ConnectionName = aspdb_Connection.ConnectionName;


            //-----------------------------------------------------------
            var aspdb_Tables = SQLObjectsCRUD.ASPdb_Table__GetAll(connectionId, true);
            foreach (var item in aspdb_Tables)
            {
                var basicAssetInfo = item.ToBasicAssetInfo();
                if (basicAssetInfo.Active)
                    rtn.Tables_Active.Add(basicAssetInfo);
                else
                    rtn.Tables_Hidden.Add(basicAssetInfo);
            }


            //-----------------------------------------------------------
            var aspdb_Views = SQLObjectsCRUD.ASPdb_View__GetAll(connectionId);
            foreach (var item in aspdb_Views)
            {
                var basicAssetInfo = item.ToBasicAssetInfo();
                if (basicAssetInfo.Active)
                    rtn.SqlViews_Active.Add(basicAssetInfo);
                else
                    rtn.SqlViews_Hidden.Add(basicAssetInfo);
            }


            //-----------------------------------------------------------
            var schemaInfos = GenericInterface.Schemas__GetAll(connectionId);
            foreach (var item in schemaInfos)
            {
                var basicAssetInfo = new BasicAssetInfo(true, BasicAssetInfo.E_AssetTypes.Schema);
                basicAssetInfo.ConnectionId = connectionId;
                basicAssetInfo.GenericId = item.SchemaId;
                basicAssetInfo.Schema = item.SchemaName;
                rtn.Schemas.Add(basicAssetInfo);
            }


            return new ManageAssetResponse()
            {
                ResponseType = ManageAssetResponse.ResponseTypesEnum.GetAssetsLists,
                AssetsListsInfo = rtn.ToAssetsLists()
            };
        }
    }
}