using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;

namespace ASPdatabaseNET.DataObjects.DatabaseConnections
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class DatabaseConnectionResponse
    {
        public enum ResponseTypesEnum { NotSet, ConnectionsLists, SingleConnectionInfo, Test, Save, Delete };

        public ResponseTypesEnum ResponseType = ResponseTypesEnum.NotSet;
        public DatabaseShortInfo[] ActiveConnections;
        public DatabaseShortInfo[] HiddenConnections;
        public SQLObjects.ASPdb_Connection SingleConnectionInfo;

        public bool SaveSuccess = false;

        public bool SaveOnSuccess = false;
        public bool TestConnection_Passed = false;
        public string TestConnection_Message = "";

        public bool DeleteSuccess = false;
    }
}