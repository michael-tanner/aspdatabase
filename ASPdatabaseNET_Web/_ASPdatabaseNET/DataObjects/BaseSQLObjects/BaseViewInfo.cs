using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.DataObjects.BaseSQLObjects
{
    public class BaseViewInfo
    {
        public string SchemaId;
        public string SchemaName;
        public string ViewName;
        public string ObjectId;
        public DateTime? CreateDate;
        public DateTime? ModifyDate;
    }
}