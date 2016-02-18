using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Objs;
using ASPdb.UniversalADO;
using ASPdb.Framework;

namespace ASPdatabaseNET.UI.PageParts.TableDesign.AppProperties.Backend
{
    //----------------------------------------------------------------------------------------------------////
    public class AppPropertiesLogic
    {
        //----------------------------------------------------------------------------------------------------
        public static AppPropertiesInfo Get(int tableId, bool useCache)
        {
            AppPropertiesInfo rtn = null;
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(tableId, false, useCache, false);
            try
            {
                string sql = String.Format(@"
                    select * 
                    from [{0}].[ASPdb_Tables] 
                    where [TableId] = @TableId", Config.SystemProperties.AppSchema);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
                {
                    command.AddParameter("@TableId", tableId);
                    using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                        if (reader.Read())
                            rtn = ASPdb.Ajax.AjaxHelper.New.FromJson<AppPropertiesInfo>(reader.Get("AppProperties", ""));
                }
            }
            catch { }
            if (rtn == null)
                rtn = new AppPropertiesInfo() { Columns = new AppPropertiesItem[0] };

            var dictJson = new DictStringKey<AppPropertiesItem>();
            var dictOut = new DictStringKey<AppPropertiesItem>();
            foreach (var item in tableStructure.Columns)
                if (dictOut.DoesNot_ContainKey(item.ColumnName))
                    dictOut.Insert(item.ColumnName, new AppPropertiesItem()
                    {
                        Index = item.OrdinalPosition,
                        ColumnName = item.ColumnName.Trim(),
                        DataType_Name = item.DataType_Name,
                        IsPrimaryKey = item.IsPrimaryKey,
                        IsIdentity = item.IsIdentity,
                        AppColumnType = AppPropertiesItem.AppColumnTypes.Default,
                        AdditionalInfo = ""
                    });
            foreach (var item in rtn.Columns)
                dictJson.Insert(item.ColumnName, item);
            foreach (var key in dictOut.TheDictionary.Keys)
                if (dictJson.ContainsKey(key))
                {
                    var item1 = dictOut.Get(key);
                    var item2 = dictJson.Get(key);
                    item1.AppColumnType = item2.AppColumnType;
                }

            var dropdownDict = new DictStringKey<DropdownList>();
            if (rtn.DropdownListItems != null)
                foreach (var item in rtn.DropdownListItems)
                    dropdownDict.Insert(item.ColumnName, item);

            foreach(var item in dictOut.TheDictionary.Values)
            {
                item.AppColumnType_Str = item.AppColumnType.ToString();
                if (dropdownDict.ContainsKey(item.ColumnName) && item.AppColumnType == AppPropertiesItem.AppColumnTypes.DropdownList)
                    item.AdditionalInfo = "(" + dropdownDict.Get(item.ColumnName).Items.Length + ")";
            }

            rtn.Columns = (from o in dictOut.TheDictionary.Values orderby o.Index select o).ToArray();
            return rtn;
        }

        //----------------------------------------------------------------------------------------------------
        public static AppPropertiesInfo Save(int tableId, AppPropertiesInfo appPropertiesInfo)
        {
            string json = ASPdb.Ajax.AjaxHelper.New.ToJson(appPropertiesInfo);
            string sql = String.Format(@"
                update [{0}].[ASPdb_Tables]
                set [AppProperties] = @AppProperties
                where [TableId] = @TableId
                ", Config.SystemProperties.AppSchema);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(sql))
            {
                command.AddParameter("@AppProperties", json);
                command.AddParameter("@TableId", tableId);
                command.ExecuteNonQuery();
            }
            return Get(tableId, false);
        }
    }
}