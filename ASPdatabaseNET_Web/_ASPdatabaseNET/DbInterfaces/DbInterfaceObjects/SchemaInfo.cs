using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.DbInterfaces.DbInterfaceObjects
{
    public class SchemaInfo
    {
        public int SchemaId;
        public int PrincipalId; // Owner's Schema Id
        public string CatalogName;
        public string SchemaName;
        public string SchemaOwner;
    }
}