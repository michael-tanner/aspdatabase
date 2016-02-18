using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpKit.JavaScript;
using SharpKit.jQuery;
using SharpKit.Html;
using ASPdb.FrameworkUI;

namespace ASPdatabaseNET.DbInterfaces.TableObjects
{
    //----------------------------------------------------------------------------------------------------////
    [JsType(JsMode.Prototype, Config.SharpKitConfig.SharpKit_BuildPath)]
    public class TableStructure
    {
        public int ConnectionId;
        public string ConnectionName;
        public int TableId;
        public string Schema;
        public string TableName;

        public bool StructureIsAView;

        public Column[] Columns;
        public PrimaryKey PrimaryKey;
        public ForeignKey[] ForeignKeys_FK;
        public ForeignKey[] ForeignKeys_PK;
        public Index[] Indexes;

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void Validate(Enums.ValidationTypes validationType)
        {
        }

        //----------------------------------------------------------------------------------------------------
        [JsMethod(Export = false)]
        public void SetTableName(string tableName)
        {
            this.TableName = tableName;
        }

    }
}