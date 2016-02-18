using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPdatabaseNET.Memory
{
    //----------------------------------------------------------------------------------------------------////
    public class AppCache
    {
        public static string AppKey = "ASPdatabaseNET.Memory.AppCache";
        //------------------------------------------------------------------------------------------ static --
        public static AppCache Get()
        {
            AppCache rtn = null;
            try
            {
                rtn = (AppCache)HttpContext.Current.Application[AppKey];
            }
            catch { }
            if(rtn == null)
            {
                rtn = new AppCache() { CacheCreatedTime = DateTime.Now };
                HttpContext.Current.Application[AppKey] = rtn;
            }
            return rtn;
        }
        //------------------------------------------------------------------------------------------ static --
        public static AppCache Reset()
        {
            HttpContext.Current.Application[AppKey] = null;
            return Get();
        }



        //----------------------------------------------------------------------------- instance properties --
        public DateTime CacheCreatedTime;
        public List<DataObjects.SQLObjects.ASPdb_Connection> ASPdb_Database_List;

        public Dictionary<int, CacheHolder_Tables> ASPdb_Table_Dictionary1; // Key is ConnectionId
        public Dictionary<int, DataObjects.SQLObjects.ASPdb_Table> ASPdb_Table_Dictionary2; // Key is TableId

        public Dictionary<int, CacheHolder_Views> ASPdb_View_Dictionary1; // Key is ConnectionId
        public Dictionary<int, DataObjects.SQLObjects.ASPdb_View> ASPdb_View_Dictionary2; // Key is TableId


        public Dictionary<string, object> AnyData;


        //------------------------------------------------------------------------------------- constructor --
        public AppCache()
        {
            this.AnyData = new Dictionary<string, object>();
        }


        //----------------------------------------------------------------------------------------------------
        public object Get_AnyData(string key, bool useCache, bool resetCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            object rtn = null;
            if(useCache)
                if (this.AnyData.ContainsKey(key))
                {
                    if (resetCache)
                        this.AnyData.Remove(key);
                    else
                        rtn = this.AnyData[key];
                }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public void Set_AnyData(string key, object obj)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            if (this.AnyData.ContainsKey(key))
                this.AnyData[key] = obj;
            else
                this.AnyData.Add(key, obj);
        }





        //------------------------------------------------------------------------------------------------////
        public class CacheHolder_Tables
        {
            public int ConnectionId;
            public List<ASPdatabaseNET.DataObjects.SQLObjects.ASPdb_Table> ASPdb_Table_List;
        }
        //------------------------------------------------------------------------------------------------////
        public class CacheHolder_Views
        {
            public int ConnectionId;
            public List<ASPdatabaseNET.DataObjects.SQLObjects.ASPdb_View> ASPdb_View_List;
        }
    }
}