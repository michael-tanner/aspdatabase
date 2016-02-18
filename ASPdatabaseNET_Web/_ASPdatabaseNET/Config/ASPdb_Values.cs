using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdb.UniversalADO;

namespace ASPdatabaseNET.Config
{
    //----------------------------------------------------------------------------------------------------////
    public class ASPdb_Values
    {
        //----------------------------------------------------------------------------------------------------
        public static ASPdb_Value[] Get(string key)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new List<ASPdb_Value>();
            try
            {
                string sql = String.Format("select * from [{0}].[ASPdb_Values] where [Key] = @Key order by [ValueId]",
                    Config.SystemProperties.AppSchema);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@Key", key);
                    using(DbReaderWrapper reader = command.ExecuteReaderWrapper())
                    {
                        while(reader.Read())
                            rtn.Add(new ASPdb_Value()
                            {
                                ValueId = reader.Get("ValueId", -1),
                                Key = reader.Get("Key", ""),
                                Value = reader.Get("Value", "")
                            });
                    }
                }
            }
            catch { }
            return rtn.ToArray();
        }
        //----------------------------------------------------------------------------------------------------
        public static string GetFirstValue(string key, string defaultValue)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                return Get(key)[0].Value;
            }
            catch { return defaultValue; }
        }


        //----------------------------------------------------------------------------------------------------
        public static void Set(string key, string value)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (Get(key).Length < 1)
            {
                Add(key, value);
                return;
            }

            string sql = "update [" + Config.SystemProperties.AppSchema + "].[ASPdb_Values] set [Key] = @Key, [Value] = @Value where [Key] = @Key";
            using(DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@Key", key);
                command.AddParameter("@Value", value);
                command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Add(string key, string value)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = "insert into [" + Config.SystemProperties.AppSchema + "].[ASPdb_Values] ([Key], [Value]) values (@Key, @Value) ";
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@Key", key);
                command.AddParameter("@Value", value);
                command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Remove(string key)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            try
            {
                string sql = String.Format("delete from [{0}].[ASPdb_Values] where [Key] = @Key",
                    Config.SystemProperties.AppSchema);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@Key", key);
                    command.ExecuteNonQuery();
                }
            }
            catch { }
        }
    }
    //----------------------------------------------------------------------------------------------------////
    public class ASPdb_Value
    {
        public int ValueId;
        public string Key;
        public string Value;
    }
}