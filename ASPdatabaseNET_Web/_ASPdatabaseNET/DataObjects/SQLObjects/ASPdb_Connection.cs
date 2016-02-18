using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DataObjects.SQLObjects
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class ASPdb_Connection
    {
        public enum Enum_ConnectionTypes { NotSet, SQLServer, SQLServerAzure, MySQL };
        public enum Enum_ParametersTypes { NotSet, WindowsIntegratedSecurity, SQLServerCredentials, ConnectionString };

        public int ConnectionId = -1;
        public int SiteId = 1;
        public string ConnectionName = "";
        public string ConnectionType = "NotSet";
        public string ParametersType = "NotSet";
        public bool Active = true;
        public DateTime? DateTimeCreated = null;
        public int CreatedByUserId = -1;

        public string Param_ServerAddress = "";
        public string Param_DatabaseName = "";
        public string Param_U = "";
        public string Param_P = "";
        public string Param_ConnectionString = "";

        //------------------------------------ Calculated Properties
        public string DateTimeCreated_String = "";
        public string CreatedByUsername = "";
        public string TableCount;

        [JsProperty(Export = false)]
        public Enum_ConnectionTypes E_ConnectionType
        {
            get
            {
                AjaxService.ASPdatabaseService.GetSetVal();
                if (this.ConnectionType == Enum_ConnectionTypes.SQLServer.ToString())
                    return Enum_ConnectionTypes.SQLServer;
                else if (this.ConnectionType == Enum_ConnectionTypes.SQLServerAzure.ToString())
                    return Enum_ConnectionTypes.SQLServerAzure;
                else if (this.ConnectionType == Enum_ConnectionTypes.MySQL.ToString())
                    return Enum_ConnectionTypes.MySQL;
                else
                    return Enum_ConnectionTypes.NotSet;
            }
        }
        [JsProperty(Export = false)]
        public Enum_ParametersTypes E_ParameterType
        {
            get
            {
                if (this.ParametersType == Enum_ParametersTypes.WindowsIntegratedSecurity.ToString())
                    return Enum_ParametersTypes.WindowsIntegratedSecurity;
                else if (this.ParametersType == Enum_ParametersTypes.SQLServerCredentials.ToString())
                    return Enum_ParametersTypes.SQLServerCredentials;
                else if (this.ParametersType == Enum_ParametersTypes.ConnectionString.ToString())
                    return Enum_ParametersTypes.ConnectionString;
                else
                    return Enum_ParametersTypes.NotSet;
            }
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public ASPdb.UniversalADO.DbEnums.ConnectionTypes Get_UniversalADO_ConnectionType()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            switch (this.E_ConnectionType)
            {
                case Enum_ConnectionTypes.SQLServer:
                    return ASPdb.UniversalADO.DbEnums.ConnectionTypes.SQLServer;
                case Enum_ConnectionTypes.SQLServerAzure:
                    return ASPdb.UniversalADO.DbEnums.ConnectionTypes.SQLServer;
                case Enum_ConnectionTypes.MySQL:
                    return ASPdb.UniversalADO.DbEnums.ConnectionTypes.MySQL;
                default:
                    return ASPdb.UniversalADO.DbEnums.ConnectionTypes.NotSet;
            }
        }



        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public bool IsPasswordIncluded()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                string temp = this.Param_P.Replace("#", "").Trim();    
                if (temp.Length > 0)
                    return true;
            }
            catch { }
            return false;
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        private string FetchPassword()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string rtn = "";
            if (this.ConnectionId < 0)
                return "";
            try
            {
                rtn = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(this.ConnectionId, false).Param_P;
            }
            catch { }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public string GetConnectionString()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string rtn = this.Param_ConnectionString;

            switch (this.E_ConnectionType)
            {
                case Enum_ConnectionTypes.SQLServer:
                    switch (this.E_ParameterType)
                    {
                        case Enum_ParametersTypes.WindowsIntegratedSecurity:
                            rtn = this.GetConnectionString__SQLServer__WindowsIntegratedSecurity();
                            break;
                        case Enum_ParametersTypes.SQLServerCredentials:
                            rtn = this.GetConnectionString__SQLServer__SQLServerCredentials();
                            break;
                    }
                    break;
                case Enum_ConnectionTypes.SQLServerAzure:
                    switch (this.E_ParameterType)
                    {
                        case Enum_ParametersTypes.WindowsIntegratedSecurity:
                            rtn = this.GetConnectionString__SQLServer__WindowsIntegratedSecurity();
                            break;
                        case Enum_ParametersTypes.SQLServerCredentials:
                            rtn = this.GetConnectionString__SQLServer__SQLServerCredentials();
                            break;
                    }
                    break;
                case Enum_ConnectionTypes.MySQL:
                    break;
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        private string GetConnectionString__SQLServer__WindowsIntegratedSecurity()
        {
            // Do Validation ! ! !

            return String.Format(
                @"Server={0}; 
                  Initial Catalog={1}; 
                  Integrated Security=True;",
                this.Param_ServerAddress, 
                this.Param_DatabaseName);
        }
        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        private string GetConnectionString__SQLServer__SQLServerCredentials()
        {
            // Do Validation ! ! !

            string pass = "";
            if (this.IsPasswordIncluded())
                pass = this.Param_P;
            else
                pass = this.FetchPassword();

            return String.Format(
                @"Server={0}; 
                  Database={1}; 
                  User Id={2}; 
                  Password={3};",
                this.Param_ServerAddress,
                this.Param_DatabaseName,
                this.Param_U,
                pass);
        }


        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public string GetValidationMessage()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var missingItems = new List<string>();
            
            if (this.E_ConnectionType == Enum_ConnectionTypes.NotSet)
                missingItems.Add("Database Type");
            if (this.E_ParameterType == Enum_ParametersTypes.NotSet)
                missingItems.Add("Connection Parameters Type");

            if (missingItems.Count < 1)
            {
                switch (this.E_ParameterType)
                {
                    case Enum_ParametersTypes.WindowsIntegratedSecurity:
                        if (String.IsNullOrWhiteSpace(this.Param_ServerAddress))
                            missingItems.Add("Server Address");
                        if (String.IsNullOrWhiteSpace(this.Param_DatabaseName))
                            missingItems.Add("Database Name");
                        break;
                    case Enum_ParametersTypes.SQLServerCredentials:
                        if (String.IsNullOrWhiteSpace(this.Param_ServerAddress))
                            missingItems.Add("Server Address");
                        if (String.IsNullOrWhiteSpace(this.Param_DatabaseName))
                            missingItems.Add("Database Name");
                        break;
                    case Enum_ParametersTypes.ConnectionString:
                        if (String.IsNullOrWhiteSpace(this.Param_ConnectionString))
                            missingItems.Add("Connection String");
                        break;
                }
            }
            string rtn = "";
            if (missingItems.Count > 0)
            {
                rtn += "<br />Please select value(s) for: <br /><br />";
                foreach (var item in missingItems)
                    rtn += "-- " + item + "<br /><br />";
            }
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public ASPdb_Connection GetDuplicate()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            return new ASPdb_Connection()
            {
                ConnectionId = this.ConnectionId,
                SiteId = this.SiteId,
                ConnectionName = this.ConnectionName,
                ConnectionType = this.ConnectionType,
                ParametersType = this.ParametersType,
                Active = this.Active,
                DateTimeCreated = this.DateTimeCreated,
                DateTimeCreated_String = this.DateTimeCreated_String,
                CreatedByUserId = this.CreatedByUserId,
                CreatedByUsername = this.CreatedByUsername,
                Param_ServerAddress = this.Param_ServerAddress,
                Param_DatabaseName = this.Param_DatabaseName,
                Param_U = this.Param_U,
                Param_P = this.Param_P,
                Param_ConnectionString = this.Param_ConnectionString,
                TableCount = this.TableCount
            };
        }
    }
}