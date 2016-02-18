using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdb.UniversalADO
{
    public class CoreDbConfig
    {
        //----------------------------------------------------------------------------------------------------
        public static DbEnums.ConnectionTypes ConnectionType
        {
            get
            {
                return DbEnums.ConnectionTypes.SQLServer;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static string ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings["ASPdb_AppData"].ConnectionString;
            }
        }

    }
}