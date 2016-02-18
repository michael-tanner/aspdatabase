using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.DbInterfaces.TableObjects
{
    public class DefaultConstraint
    {
        public int ConnectionId;
        public string Schema;
        public string TableName;

        public string ColumnName;
        public string DefaultConstraintName;
        public string DefaultDefinition;
    }
}