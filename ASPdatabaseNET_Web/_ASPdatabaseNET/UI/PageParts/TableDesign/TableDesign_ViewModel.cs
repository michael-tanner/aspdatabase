using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;
using ASPdatabaseNET.DbInterfaces.TableObjects;
using ASPdatabaseNET.DataObjects.TableDesign;
using ASPdatabaseNET.DataObjects.SQLObjects;

namespace ASPdatabaseNET.UI.PageParts.TableDesign
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TableDesign_ViewModel
    {
        public bool IsCreateNew = false;
        public int ConnectionId = -1;
        public int TableId = -1;
        public TableStructure TableStructure;

        public ASPdb_Table[] AllTables_InDb;
        public TableStructure TableStructure_TempOtherTable;

        //----------------------------------------------------------------------------------------------------
        public void Set(TableDesignResponse tableDesignResponse)
        {
            this.ConnectionId = tableDesignResponse.ConnectionId;
            this.TableId = tableDesignResponse.TableId;
            this.TableStructure = tableDesignResponse.TableStructure;
            this.AllTables_InDb = tableDesignResponse.AllTables_InDb;
        }
        //----------------------------------------------------------------------------------------------------
        public TableStructure Get_Minified_TableStructure()
        {
            var rtn = new TableStructure();
            rtn.ConnectionId = this.ConnectionId;
            rtn.TableId = this.TableId;
            rtn.Schema = this.TableStructure.Schema;
            rtn.TableName = this.TableStructure.TableName;
            return rtn;
        }


        //----------------------------------------------------------------------------------------------------
        public ASPdb_Table GetTableInfo(int tableId)
        {
            if (this.AllTables_InDb != null)
                for (int i = 0; i < this.AllTables_InDb.Length; i++)
                    if (this.AllTables_InDb[i].TableId == tableId)
                        return this.AllTables_InDb[i];
            return null;
        }
        //----------------------------------------------------------------------------------------------------
        public int GetTableId(string schema, string tableName)
        {
            if (this.AllTables_InDb != null)
                for (int i = 0; i < this.AllTables_InDb.Length; i++)
                {
                    var table = this.AllTables_InDb[i];
                    if (table.Schema == schema && table.TableName == tableName)
                        return table.TableId;
                }
            return -1;
        }
    }
}