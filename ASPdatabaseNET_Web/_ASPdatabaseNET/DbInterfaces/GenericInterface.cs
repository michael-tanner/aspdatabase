using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.DbInterfaces.DbInterfaceObjects;

namespace ASPdatabaseNET.DbInterfaces
{
    //----------------------------------------------------------------------------------------------------////
    public class GenericInterface
    {
        //----------------------------------------------------------------------------------------------------
        public static SchemaInfo[] Schemas__GetAll(int connectionId)
        {
            return SQLServerInterface.Schemas__GetAll(connectionId);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Schemas__SaveNew(int connectionId, string newSchemaName)
        {
            SQLServerInterface.Schemas__SaveNew(connectionId, newSchemaName);
        }
        //----------------------------------------------------------------------------------------------------
        public static void Schemas__Delete(int connectionId, string schemaName)
        {
            SQLServerInterface.Schemas__Delete(connectionId, schemaName);
        }


    }
}