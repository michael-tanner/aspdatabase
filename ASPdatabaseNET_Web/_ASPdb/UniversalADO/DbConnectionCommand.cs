using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data.SqlClient;

namespace ASPdb.UniversalADO
{
    //----------------------------------------------------------------------------------------------------////
    public class DbConnectionCommand : IDisposable
    {
        public DbEnums.ConnectionTypes ConnectionType = DbEnums.ConnectionTypes.NotSet;
        public string ConnectionString;
        public DbConnection Connection;
        public DbCommand Command;

        //----------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            try
            {
                if (this.Connection != null)
                    this.Connection.Dispose();
            }
            catch { }
            try
            {
                if (this.Command != null)
                    this.Command.Dispose();
            }
            catch { }
        }

        //----------------------------------------------------------------------------------------------------
        public void AddParameter(string parameterName, object value)
        {
            if (this.Command != null)
            {
                DbParameter parameter = null;
                switch (this.ConnectionType)
                {
                    case DbEnums.ConnectionTypes.SQLServer:
                        parameter = new SqlParameter(parameterName, value);
                        break;
                    case DbEnums.ConnectionTypes.MySQL:
                        break;
                    case DbEnums.ConnectionTypes.Oracle:
                        break;
                }
                this.Command.Parameters.Add(parameter);
            }
        }

        //----------------------------------------------------------------------------------------------------
        public DbReaderWrapper ExecuteReaderWrapper()
        {
            var reader = this.Command.ExecuteReader();
            var rtn = new DbReaderWrapper(reader);
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public int ExecuteNonQuery()
        {
            return this.Command.ExecuteNonQuery();
        }

    }
}