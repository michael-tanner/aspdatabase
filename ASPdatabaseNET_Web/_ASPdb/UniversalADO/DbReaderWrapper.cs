using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Common;

namespace ASPdb.UniversalADO
{
    //----------------------------------------------------------------------------------------------------////
    public class DbReaderWrapper : IDisposable
    {
        public DbDataReader Reader;

        //------------------------------------------------------------------------------------- constructor --
        public DbReaderWrapper()
        {
        }
        //------------------------------------------------------------------------------------- constructor --
        public DbReaderWrapper(DbDataReader reader)
        {
            this.Reader = reader;
        }
        //----------------------------------------------------------------------------------------------------
        public void Dispose()
        {
            try
            {
                if (this.Reader != null)
                    this.Reader.Dispose();
            }
            catch { }
        }
        //----------------------------------------------------------------------------------------------------
        public bool Read()
        {
            return this.Reader.Read();
        }


        //----------------------------------------------------------------------------------------------------
        public int FieldCount
        {
            get
            {
                return this.Reader.FieldCount;
            }
        }
        //----------------------------------------------------------------------------------------------------
        public string GetName(int ordinal)
        {
            return this.Reader.GetName(ordinal);
        }
        //----------------------------------------------------------------------------------------------------
        public Type GetFieldType(int ordinal)
        {
            return this.Reader.GetFieldType(ordinal);
        }
        //----------------------------------------------------------------------------------------------------
        public string GetDataTypeName(int ordinal)
        {
            return this.Reader.GetDataTypeName(ordinal);
        }




        //----------------------------------------------------------------------------------------------------
        public string Get(int ordinal, string defaultValue)
        {
            try
            {
                return this.Reader[ordinal].ToString();
            }
            catch { return defaultValue; }
        }

        //----------------------------------------------------------------------------------------------------
        /// <summary>Read [int] from Reader</summary>
        public int Get(string fieldName, int defaultValue)
        {
            try
            {
                return Int32.Parse(this.Reader[fieldName].ToString());
            }
            catch { }
            return defaultValue;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Read [bool] from Reader</summary>
        public bool Get(string fieldName, bool defaultValue)
        {
            try
            {
                return Boolean.Parse(this.Reader[fieldName].ToString());
            }
            catch { }
            return defaultValue;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Read [string] from Reader</summary>
        public string Get(string fieldName, string defaultValue)
        {
            try
            {
                return this.Reader[fieldName].ToString();
            }
            catch { }
            return defaultValue;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Read [DateTime] from Reader</summary>
        public DateTime Get(string fieldName, DateTime defaultValue)
        {
            try
            {
                return DateTime.Parse(this.Reader[fieldName].ToString());
            }
            catch { }
            return defaultValue;
        }
        //----------------------------------------------------------------------------------------------------
        /// <summary>Read [DateTime?] from Reader</summary>
        public DateTime? Get(string fieldName, DateTime? defaultValue)
        {
            try
            {
                string value = this.Reader[fieldName].ToString();
                if(value != null && value.Length > 0)
                    return DateTime.Parse(value);
            }
            catch { }
            return defaultValue;
        }




        //----------------------------------------------------------------------------------------------------
        /// <summary>Read [string] from Reader</summary>
        public string GetString_OrNullDefault(string fieldName)
        {
            try
            {
                if (this.Reader[fieldName] != DBNull.Value)
                    return this.Reader[fieldName].ToString();
            }
            catch { }
            return null;
        }


    }
}