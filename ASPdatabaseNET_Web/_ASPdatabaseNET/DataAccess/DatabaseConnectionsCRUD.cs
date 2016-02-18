using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.DataObjects.DatabaseConnections;
using ASPdatabaseNET.DataObjects.SQLObjects;
using ASPdb.UniversalADO;

namespace ASPdatabaseNET.DataAccess
{
    //----------------------------------------------------------------------------------------------------////
    public class DatabaseConnectionsCRUD
    {
        //----------------------------------------------------------------------------------------------------
        public static DatabaseConnectionResponse GetList()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (!Subscription.SubscriptionAppState.ValidateActiveSubscribers())
                throw new Exception("Validation Error");

            var rtn = new DatabaseConnectionResponse() { ResponseType = DatabaseConnectionResponse.ResponseTypesEnum.ConnectionsLists };
            var list_Active = new List<DatabaseShortInfo>();
            var list_Hidden = new List<DatabaseShortInfo>();
            try
            {
                var all = DataAccess.SQLObjectsCRUD.ASPdb_Connection__GetAll(false);
                foreach (var item in all)
                {
                    var shortInfo = new DatabaseShortInfo() { ConnectionId = item.ConnectionId, ConnectionName = item.ConnectionName };
                    if (item.Active) list_Active.Add(shortInfo);
                    else list_Hidden.Add(shortInfo);
                }
            }
            catch { }
            rtn.ActiveConnections = list_Active.ToArray();
            rtn.HiddenConnections = list_Hidden.ToArray();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static DatabaseConnectionResponse Get(int connectionId, bool removeInfoForDemo)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId);
            rtn = rtn.GetDuplicate();

            string tmpP = "##########";
            if(!String.IsNullOrEmpty(rtn.Param_P))
                for (int i = 0; i < rtn.Param_P.Length; i++)
                    tmpP += "#";
            rtn.Param_P = tmpP;

            rtn.DateTimeCreated_String = "...";
            if (rtn.DateTimeCreated.HasValue)
                rtn.DateTimeCreated_String = rtn.DateTimeCreated.Value.ToString();
            rtn.DateTimeCreated = null;

            if(String.IsNullOrWhiteSpace(rtn.CreatedByUsername))
                rtn.CreatedByUsername = "...";

            rtn.TableCount = "...";


            var rtn2 = new DatabaseConnectionResponse()
            {
                ResponseType = DatabaseConnectionResponse.ResponseTypesEnum.SingleConnectionInfo,
                SingleConnectionInfo = rtn
            };
            if(removeInfoForDemo)
            {
                rtn2.SingleConnectionInfo.Param_ConnectionString = "##########";
                rtn2.SingleConnectionInfo.Param_ServerAddress = "##########";
                rtn2.SingleConnectionInfo.Param_DatabaseName = "##########";
                rtn2.SingleConnectionInfo.Param_P = "##########";
                rtn2.SingleConnectionInfo.Param_U = "##########";
            }
            return rtn2;
        }
        //----------------------------------------------------------------------------------------------------
        public static DatabaseConnectionResponse Test(ASPdb_Connection aspdb_Connection, bool saveOnSuccess)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new DatabaseConnectionResponse() { ResponseType = DatabaseConnectionResponse.ResponseTypesEnum.Test };
            rtn.SaveOnSuccess = saveOnSuccess;
            rtn.TestConnection_Passed = false;

            try
            {
                string validationString = aspdb_Connection.GetValidationMessage();
                if (validationString != "")
                    throw new Exception(validationString);

                string connectionString = aspdb_Connection.GetConnectionString();
                var tableItems = SQLObjectsCRUD.ASPdb_Table__GetRawList_NoCache(aspdb_Connection);

                string s = "";
                
                s += String.Format("{0} Tables Found<br /><br />", tableItems.Count);
                if(tableItems.Count == 1)
                    s = String.Format("{0} Table Found<br /><br />", tableItems.Count);

                foreach (var item in tableItems)
                    s += String.Format("<div class='TableItem1'><span>{0}.</span>{1}</div>", item.SchemaName, item.TableName);

                rtn.TestConnection_Passed = true;
                rtn.TestConnection_Message = s;
            }
            catch (Exception exc)
            {
                rtn.TestConnection_Passed = false;
                rtn.TestConnection_Message += exc.Message; // +"<hr />" + exc.StackTrace;
            }



            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static DatabaseConnectionResponse Save(ASPdb_Connection connectionInfo)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            connectionInfo.ConnectionName = connectionInfo.ConnectionName.Trim();

            SQLObjectsCRUD.ASPdb_Connection__Save(connectionInfo);

            return new DatabaseConnectionResponse() { ResponseType = DatabaseConnectionResponse.ResponseTypesEnum.Save, SaveSuccess = true };
        }   
        //----------------------------------------------------------------------------------------------------
        public static DatabaseConnectionResponse Delete(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new DatabaseConnectionResponse() { ResponseType = DatabaseConnectionResponse.ResponseTypesEnum.Delete };

            // check user permissions
            // check SiteId

            // what to do with related tables ... such as [" + Config.SystemProperties.AppSchema + @"].[ASPdb_Tables]

            SQLObjectsCRUD.ASPdb_Connection__Delete(connectionId);


            rtn.DeleteSuccess = true;
            return rtn;
        }
    }
}