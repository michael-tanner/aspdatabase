using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;

namespace ASPdb.UniversalADO
{
    //----------------------------------------------------------------------------------------------------////
    public class UniversalADO
    {
        //------------------------------------------------------------------------------------------ static --
        public static DbConnection OpenConnection()
        {
            return OpenConnection(CoreDbConfig.ConnectionString);
        }
        //------------------------------------------------------------------------------------------ static --
        public static DbConnection OpenConnection(string connectionString)
        {
            DbConnection rtn = null;
            var connectionType = CoreDbConfig.ConnectionType;


            //if (connectionString.Contains("Database=world"))
            //    connectionType = DbEnums.ConnectionTypes.MySQL;


            switch (connectionType)
            {
                case DbEnums.ConnectionTypes.SQLServer:
                    rtn = new SqlConnection(connectionString);
                    break;
                case DbEnums.ConnectionTypes.MySQL:
                    //rtn = new MySqlConnection(connectionString);
                    break;
                case DbEnums.ConnectionTypes.Oracle:
                    break;
                case DbEnums.ConnectionTypes.DB2:
                    break;
                case DbEnums.ConnectionTypes.PostgreSQL:
                    break;
                case DbEnums.ConnectionTypes.Access:
                    break;
                case DbEnums.ConnectionTypes.Excel:
                    break;
            }
            rtn.Open();
            return rtn;
        }

        //------------------------------------------------------------------------------------------ static --
        public static DbCommand GetACommand(string sql, DbConnection connection, DbEnums.ConnectionTypes connectionType)
        {
            DbCommand rtn = null;

            switch (connectionType)
            {
                case DbEnums.ConnectionTypes.SQLServer:
                    rtn = new SqlCommand(sql, (SqlConnection)connection);
                    break;
                case DbEnums.ConnectionTypes.MySQL:
                    //rtn = new MySqlCommand(sql, (MySqlConnection)connection);
                    break;
                case DbEnums.ConnectionTypes.Oracle:
                    break;
                case DbEnums.ConnectionTypes.DB2:
                    break;
                case DbEnums.ConnectionTypes.PostgreSQL:
                    break;
                case DbEnums.ConnectionTypes.Access:
                    break;
                case DbEnums.ConnectionTypes.Excel:
                    break;
            }
            return rtn;
        }

        //------------------------------------------------------------------------------------------ static --
        public static DbConnectionCommand GetConnectionCommand(DbConnection connection, string sql)
        {
            var rtn = new DbConnectionCommand()
            {
                ConnectionType = CoreDbConfig.ConnectionType,
                ConnectionString = connection.ConnectionString,
                Connection = connection
            };
            rtn.Command = GetACommand(sql, rtn.Connection, rtn.ConnectionType);
            return rtn;
        }
        //------------------------------------------------------------------------------------------ static --
        public static DbConnectionCommand OpenConnectionCommand(string sql)
        {
            var rtn = new DbConnectionCommand()
            {
                ConnectionType = CoreDbConfig.ConnectionType,
                ConnectionString = CoreDbConfig.ConnectionString,
                Connection = OpenConnection()
            };
            rtn.Command = GetACommand(sql, rtn.Connection, rtn.ConnectionType);
            return rtn;
        }
        //------------------------------------------------------------------------------------------ static --
        public static DbConnectionCommand OpenConnectionCommand(string connectionString, string sql)
        {
            var rtn = new DbConnectionCommand()
            {
                ConnectionType = CoreDbConfig.ConnectionType,
                ConnectionString = CoreDbConfig.ConnectionString,
                Connection = OpenConnection(connectionString)
            };

            //if (connectionString.Contains("Database=world"))
            //    rtn.ConnectionType = DbEnums.ConnectionTypes.MySQL;

            rtn.Command = GetACommand(sql, rtn.Connection, rtn.ConnectionType);
            return rtn;
        }
        //------------------------------------------------------------------------------------------ static --
        public static DbConnectionCommand OpenConnectionCommand(int connectionId, string sql)
        {
            //string connectionString = ASPdatabaseNET.DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId).Param_ConnectionString; // this breaks the one-way dependency of ASPdb

            var connectionInfo = ASPdatabaseNET.DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId);
            string connectionString = connectionInfo.GetConnectionString();

            var rtn = new DbConnectionCommand()
            {
                //ConnectionType = CoreDbConfig.ConnectionType,
                //ConnectionType = connectionInfo.E_ConnectionType,
                ConnectionType = connectionInfo.Get_UniversalADO_ConnectionType(),
                ConnectionString = connectionString,
                Connection = OpenConnection(connectionString)
            };
            rtn.Command = GetACommand(sql, rtn.Connection, rtn.ConnectionType);
            return rtn;
        }

    }
}