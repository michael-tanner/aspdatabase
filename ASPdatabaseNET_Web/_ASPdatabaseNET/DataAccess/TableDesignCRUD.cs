using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.DataObjects.TableDesign;
using ASPdatabaseNET.DbInterfaces.TableObjects;

namespace ASPdatabaseNET.DataAccess
{
    //----------------------------------------------------------------------------------------------------////
    public class TableDesignCRUD
    {
        //----------------------------------------------------------------------------------------------------
        public static TableDesignResponse GetInfo_ForCreateNew(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new TableDesignResponse() { ResponseType = TableDesignResponse.ResponseTypesEnum.GetInfo_ForCreateNew };
            rtn.ConnectionId = connectionId;
            rtn.TableId = -1;

            var connectionInfo = DataAccess.DatabaseConnectionsCRUD.Get(connectionId, false);
            rtn.ConnectionName = connectionInfo.SingleConnectionInfo.ConnectionName;

            var schemaInfos = DbInterfaces.GenericInterface.Schemas__GetAll(connectionId);
            rtn.Schemas = (from s in schemaInfos orderby s.SchemaName select s.SchemaName).ToArray();

            rtn.AllTables_InDb = SQLObjectsCRUD.ASPdb_Table__GetAll(rtn.ConnectionId, true).ToArray();

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static TableDesignResponse GetInfo_ForModify(int tableId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new TableDesignResponse() { ResponseType = TableDesignResponse.ResponseTypesEnum.GetInfo_ForModify };
            rtn.ConnectionId = -1;
            rtn.TableId = tableId;

            rtn.TableStructure = DbInterfaces.SQLServerInterface.Tables__Get(tableId, true, false, false);
            if (rtn.TableStructure == null)
                throw new Exception("rtn.TableStructure is null");
            var connectionInfo = DataAccess.DatabaseConnectionsCRUD.Get(rtn.TableStructure.ConnectionId, false);

            rtn.ConnectionName = connectionInfo.SingleConnectionInfo.ConnectionName;
            rtn.TableStructure.ConnectionName = connectionInfo.SingleConnectionInfo.ConnectionName;
            rtn.ConnectionId = rtn.TableStructure.ConnectionId;
            rtn.Schema = rtn.TableStructure.Schema;
            rtn.TableName = rtn.TableStructure.TableName;

            rtn.AllTables_InDb = SQLObjectsCRUD.ASPdb_Table__GetAll(rtn.ConnectionId, true).ToArray();

            return rtn;
        }
    }
}