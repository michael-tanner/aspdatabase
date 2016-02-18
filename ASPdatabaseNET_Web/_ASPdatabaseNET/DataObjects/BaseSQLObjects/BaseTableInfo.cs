using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.DataObjects.BaseSQLObjects
{
    public class BaseTableInfo
    {
        public string SchemaId;
        public string SchemaName;
        public string TableName;
        public string ObjectId;
        public DateTime? CreateDate;
        public DateTime? ModifyDate;
    }
}