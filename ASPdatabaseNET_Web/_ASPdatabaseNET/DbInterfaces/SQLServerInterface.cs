using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using ASPdb.UniversalADO;
using ASPdatabaseNET.DbInterfaces.DbInterfaceObjects;
using ASPdatabaseNET.DbInterfaces.TableObjects;
using ASPdatabaseNET.DataObjects.TableDesign;

namespace ASPdatabaseNET.DbInterfaces
{
    //----------------------------------------------------------------------------------------------------////
    public class SQLServerInterface
    {
        //----------------------------------------------------------------------------------------------------
        public static SchemaInfo[] Schemas__GetAll(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new List<SchemaInfo>();
            string connectionString = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId).GetConnectionString();
            string sql = @"
                SELECT t1.Schema_Id, t1.principal_id,
                t2.Catalog_Name, t2.Schema_Name, t2.Schema_Owner FROM sys.schemas t1
                Inner Join INFORMATION_SCHEMA.SCHEMATA t2 on t1.name = t2.SCHEMA_NAME 
                where t2.SCHEMA_OWNER = 'dbo'
                order by t1.name
                ";
            using(DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionString, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        var schemaInfo = new SchemaInfo()
                        {
                            SchemaId = reader.Get("Schema_Id", -1),
                            PrincipalId = reader.Get("Principal_Id", -1),
                            CatalogName = reader.Get("Catalog_Name", ""),
                            SchemaName = reader.Get("Schema_Name", ""),
                            SchemaOwner = reader.Get("Schema_Owner", "")
                        };
                        rtn.Add(schemaInfo);
                    }
                }
            }
            return rtn.ToArray();
        }
        //----------------------------------------------------------------------------------------------------
        public static void Schemas__SaveNew(int connectionId, string newSchemaName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            ASPdb.Framework.Validation.ValidateTextForSql1(newSchemaName, true);

            string connectionString = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId).GetConnectionString();
            string sql = String.Format("Create Schema [{0}]", newSchemaName);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionString, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Schemas__Delete(int connectionId, string schemaName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            ASPdb.Framework.Validation.ValidateTextForSql1(schemaName, true);

            string connectionString = DataAccess.SQLObjectsCRUD.ASPdb_Connection__Get(connectionId).GetConnectionString();
            string sql = String.Format("Drop Schema [{0}]", schemaName);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionString, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }


        
        ////----------------------------------------------------------------------------------------------------
        //public static Column[] Tables__Get2(int tableId)
        //{
        //    var rtn = Tables__Get(tableId);
        //    if (rtn != null)
        //        return rtn.Columns;
        //    else
        //        return null;
        //}


        //----------------------------------------------------------------------------------------------------
        public static TableStructure Views__Get(int viewId, bool useCache, bool resetCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string key = "SQLServerInterface.Views__Get." + viewId;
            var cache = Memory.AppCache.Get();

            var rtn = (TableStructure)cache.Get_AnyData(key, useCache, resetCache);
            if (rtn == null)
            {
                rtn = Views__Get(viewId);
                cache.Set_AnyData(key, rtn);
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static TableStructure Views__Get(int viewId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_View = DataAccess.SQLObjectsCRUD.ASPdb_View__Get(viewId);
            if (aspdb_View == null)
                return null;
            var rtn = new TableStructure()
            {
                ConnectionId = aspdb_View.ConnectionId,
                TableId = aspdb_View.ViewId,
                Schema = aspdb_View.Schema,
                TableName = aspdb_View.ViewName,
                StructureIsAView = true
            };

            rtn.Columns = Tables__Get__GetColumns(rtn);
            return rtn;
        }









        
        //----------------------------------------------------------------------------------------------------
        public static TableStructure Tables__Get(int tableId, bool includeForeignKeysAndIndexes, bool useCache, bool resetCache)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string key = "SQLServerInterface.Tables__Get." + tableId + "." + includeForeignKeysAndIndexes;
            var cache = Memory.AppCache.Get();

            var rtn = (TableStructure)cache.Get_AnyData(key, useCache, resetCache);
            if (rtn == null)
            {
                rtn = Tables__Get(tableId, includeForeignKeysAndIndexes);
                cache.Set_AnyData(key, rtn);
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static TableStructure Tables__Get(int tableId, bool includeForeignKeysAndIndexes)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var aspdb_Table = DataAccess.SQLObjectsCRUD.ASPdb_Table__Get(tableId);
            if (aspdb_Table == null)
                return null;
            var rtn = new TableStructure()
            {
                ConnectionId = aspdb_Table.ConnectionId,
                TableId = aspdb_Table.TableId,
                Schema = aspdb_Table.Schema,
                TableName = aspdb_Table.TableName,
                StructureIsAView = false
            };

            rtn.Columns = Tables__Get__GetColumns(rtn);
            rtn.PrimaryKey = PrimaryKey__Get(rtn);

            if(includeForeignKeysAndIndexes)
            {
                ForeignKey[] fkSide_ForeignKeys, pkSide_ForeignKeys;
                ForeignKey__Get(rtn, out fkSide_ForeignKeys, out pkSide_ForeignKeys);
                rtn.ForeignKeys_FK = fkSide_ForeignKeys;
                rtn.ForeignKeys_PK = pkSide_ForeignKeys;

                rtn.Indexes = Index__Get(rtn);
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        private static Column[] Tables__Get__GetColumns(TableStructure tableStructure)
        {
            var rtn = new List<Column>();
            string sql = SQLServer_SQLBuilders.BuildSql__Column_SelectAll(tableStructure);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        var column = new Column()
                        {
                            ConnectionId = tableStructure.ConnectionId,
                            Schema = reader.Get("Schema", ""),
                            TableName = reader.Get("TableName", ""),
                            OrdinalPosition = reader.Get("OrdinalPosition", 0),
                            ColumnName = reader.Get("ColumnName", ""),
                            DataType_Name = reader.Get("DataType", ""),
                            MaxLength = reader.Get("MaxLength", 0),
                            Precision = reader.Get("Precision", 0),
                            Scale = reader.Get("Scale", 0),
                            AllowNulls = reader.Get("AllowNulls", "NO").ToUpper() == "YES",
                            DefaultValue = reader.GetString_OrNullDefault("DefaultValue"),
                            IsPrimaryKey = reader.Get("IsPrimaryKey", 0) == 1,
                            IsIdentity = reader.Get("IsIdentity", 0) == 1,
                            Identity = null
                        };
                        if (column.IsIdentity)
                            column.Identity = new Identity()
                            {
                                ConnectionId = tableStructure.ConnectionId,
                                Schema = reader.Get("Schema", ""),
                                TableName = reader.Get("TableName", ""),
                                ColumnName = reader.Get("ColumnName", ""),
                                Seed = reader.Get("Identity_Seed", 0),
                                Increment = reader.Get("Identity_Increment", 0),
                                CurrentIdentity = reader.Get("Identity_CurrentIdentity", 0)
                            };
                        column.Populate__DataType_FullString();
                        column.Fix_DefaultValue_ForDisplay();
                        rtn.Add(column);
                    }
                }
            }
            return rtn.ToArray();
        }



        //----------------------------------------------------------------------------------------------------
        public static TableDesignResponse Table__Create(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new TableDesignResponse() { ResponseType = TableDesignResponse.ResponseTypesEnum.CreateTable, 
                ConnectionId = tableStructure.ConnectionId, Schema = tableStructure.Schema, TableName = tableStructure.TableName };

            tableStructure.Columns = (from c in tableStructure.Columns
                                      where c.ColumnName.Length > 0
                                      && c.DataType.Length > 0
                                      orderby c.OrdinalPosition
                                      select c).ToArray();

            var primaryKey = new PrimaryKey() { ConnectionId = tableStructure.ConnectionId, TableId = tableStructure.TableId, Schema = tableStructure.Schema, TableName = tableStructure.TableName };
            var tmp_PrimaryKeyColumns = new List<PrimaryKeyColumn>();
            int i = 1;
            foreach (var column in tableStructure.Columns)
            {
                if (column.IsPrimaryKey)
                    tmp_PrimaryKeyColumns.Add(new PrimaryKeyColumn() { ColumnName = column.ColumnName, OrdinalPosition = i++ });
                column.Fix_DefaultValue_ForSaving();
            }
            primaryKey.Columns = tmp_PrimaryKeyColumns.ToArray();
  
            string sql = SQLServer_SQLBuilders.BuildSql__Table_Create(tableStructure);
            ASPdb.Framework.Debug.WriteLine(sql);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
            PrimaryKey__Create(primaryKey, false);

            rtn.TableId = -1;
            Memory.AppCache.Reset();
            var allTables = DataAccess.SQLObjectsCRUD.ASPdb_Table__GetAll(tableStructure.ConnectionId, true);
            foreach (var table in allTables)
                if (table.Schema.ToLower() == rtn.Schema.ToLower() 
                    && table.TableName.ToLower() == rtn.TableName.ToLower())
                    rtn.TableId = table.TableId;

            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static TableDesignResponse Table__Update(TableStructure newTableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new TableDesignResponse() { ResponseType = TableDesignResponse.ResponseTypesEnum.UpdateTable };
            var currentTableStructure = Tables__Get(newTableStructure.TableId, true);

            var currentColumnsDict = new Dictionary<string, Column>();
            foreach(var column in currentTableStructure.Columns)
                if(!currentColumnsDict.ContainsKey(column.ColumnName.ToLower()))
                    currentColumnsDict.Add(column.ColumnName.ToLower(), column);


            var columnsList_All = new List<Column>();
            var columnsList_Delete = new List<Column>();
            var columnsList_Add = new List<Column>();
            var columnsList_Update = new List<Column>();
            bool reorder_Needed = false;
            int tmpOrdinalPos = 1;

            //var cols = (from c in newTableStructure.Columns orderby c.OrdinalPosition, c.ColumnName select c)

            //--------------------
            foreach (var column in (from c in newTableStructure.Columns orderby c.OrdinalPosition, c.ColumnName select c))
                if (column.HasValidContent())
                {
                    column.Validate(Enums.ValidationTypes.NotSet);
                    column.ConnectionId = currentTableStructure.ConnectionId;
                    column.Schema = currentTableStructure.Schema;
                    column.TableName = currentTableStructure.TableName;
                    column.OrdinalPosition = tmpOrdinalPos++;
                    column.Fix_DefaultValue_ForSaving();

                    columnsList_All.Add(column);
                    switch (column.ChangeAction)
                    {
                        case Column.ChangeActionTypes.Delete:
                            columnsList_Delete.Add(column);
                            break;
                        case Column.ChangeActionTypes.Update:
                            if (Table__Update__IsColumnDifferent(column, currentColumnsDict))
                                columnsList_Update.Add(column);
                            break;
                        case Column.ChangeActionTypes.Add:
                            columnsList_Add.Add(column);
                            reorder_Needed = true;
                            break;
                    }
                }
            newTableStructure.Columns = columnsList_All.ToArray();

            //--------------------
            foreach (var column in columnsList_Delete)
                Column__Delete(column);

            foreach (var column in columnsList_Update)
                Column__Update(column);

            foreach (var column in columnsList_Add)
                Column__Create(column);

            //--------------------
            if (!reorder_Needed)
                foreach (var column in newTableStructure.Columns)
                    if (currentColumnsDict.ContainsKey(column.ColumnName_Original.ToLower()))
                    {
                        var columnCurrent = currentColumnsDict[column.ColumnName_Original.ToLower()];
                        if (columnCurrent.OrdinalPosition != column.OrdinalPosition)
                            reorder_Needed = true;
                    }
            //--------------------
            if (reorder_Needed)
            {
                string sql = SQLServer_SQLBuilders.BuildSql__Table_UpdateWithRecreate(newTableStructure, currentTableStructure);
                ASPdb.Framework.Debug.WriteLine("------------------------------");
                ASPdb.Framework.Debug.WriteLine("SQL: " + sql);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(currentTableStructure.ConnectionId, sql))
                {
                    command.Command.ExecuteNonQuery();
                }
            }

            Memory.AppCache.Reset();
            var tableStructure_ResetCache = DbInterfaces.SQLServerInterface.Tables__Get(newTableStructure.TableId, false, true, true);

            return rtn;
        }
        private static bool Table__Update__IsColumnDifferent(Column column, Dictionary<string, Column> currentColumnsDict)
        {
            if (!currentColumnsDict.ContainsKey(column.ColumnName_Original.ToLower()))
                return true;

            var columnOrig = currentColumnsDict[column.ColumnName_Original.ToLower()];
            return !column.IsEqual_ForUpdating(columnOrig);
        }
        //----------------------------------------------------------------------------------------------------
        public static TableDesignResponse Table__Delete(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new TableDesignResponse() { ResponseType = TableDesignResponse.ResponseTypesEnum.DeleteTable };

            string sql = SQLServer_SQLBuilders.BuildSql__Table_Drop(tableStructure);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
            Memory.AppCache.Reset();
            return rtn;
        }




        //----------------------------------------------------------------------------------------------------
        public static void Column__Create(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__Column_Create(column);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(column.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Column__Update(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string defaultConstraintName = null;
            var defaultConstraint = DefaultConstraint__Get(column);
            if (defaultConstraint != null)
                defaultConstraintName = defaultConstraint.DefaultConstraintName;

            string sql = SQLServer_SQLBuilders.BuildSql__Column_Update(column, defaultConstraintName);
            ASPdb.Framework.Debug.WriteLine("SQL: " + sql);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(column.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Column__Delete(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string defaultConstraintName = null;
            var defaultConstraint = DefaultConstraint__Get(column);
            if (defaultConstraint != null)
                defaultConstraintName = defaultConstraint.DefaultConstraintName;

            string sql = SQLServer_SQLBuilders.BuildSql__Column_Drop(column, defaultConstraintName);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(column.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }



        
        //----------------------------------------------------------------------------------------------------
        public static PrimaryKey PrimaryKey__Get(int tableId)
        {
            var tableStructure = DbInterfaces.SQLServerInterface.Tables__Get(tableId, false, true, false);
            return PrimaryKey__Get(tableStructure);
        }
        //----------------------------------------------------------------------------------------------------
        public static PrimaryKey PrimaryKey__Get(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            PrimaryKey rtn = null;


            var colDict = new Dictionary<string, Column>();
            foreach (var item in tableStructure.Columns)
                if (!colDict.ContainsKey(item.ColumnName.ToLower()))
                    colDict.Add(item.ColumnName.ToLower(), item);

            var primaryKeyColumns_List = new List<PrimaryKeyColumn>();
            string sql = SQLServer_SQLBuilders.BuildSql__PrimaryKey_Select(tableStructure);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        if (rtn == null)
                            rtn = new PrimaryKey()
                            {
                                ConnectionId = tableStructure.ConnectionId,
                                TableId = tableStructure.TableId,
                                Schema = reader.Get("Schema", ""),
                                TableName = reader.Get("TableName", ""),
                                ConstraintName = reader.Get("ConstraintName", "")
                            };
                        var primaryKeyItem = new PrimaryKeyColumn()
                        {
                            ColumnName = reader.Get("ColumnName", ""),
                            OrdinalPosition = reader.Get("OrdinalPosition", 0)
                        };
                        if (colDict.ContainsKey(primaryKeyItem.ColumnName.ToLower()))
                            primaryKeyItem.Identity = colDict[primaryKeyItem.ColumnName.ToLower()].Identity;
                        primaryKeyColumns_List.Add(primaryKeyItem);
                    }
                }
            }
            if(rtn != null)
                rtn.Columns = primaryKeyColumns_List.ToArray();
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static void PrimaryKey__Create(PrimaryKey primaryKey, bool seeIfIdentityChanged)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var allConstraints = Constraint__GetAll_InConnection(primaryKey.ConnectionId);
            string sql = SQLServer_SQLBuilders.BuildSql__PrimaryKey_Create(primaryKey, true, allConstraints, true);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(primaryKey.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
            if (seeIfIdentityChanged)
                PrimaryKey__SeeIfIdentityChanged(primaryKey);
            var tableStructure_ResetCache = DbInterfaces.SQLServerInterface.Tables__Get(primaryKey.TableId, false, true, true);
        }
        //----------------------------------------------------------------------------------------------------
        public static void PrimaryKey__Update(PrimaryKey primaryKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__PrimaryKey_Update(primaryKey);
            ASPdb.Framework.Debug.WriteLine(sql);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(primaryKey.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
            PrimaryKey__SeeIfIdentityChanged(primaryKey);
            var tableStructure_ResetCache = DbInterfaces.SQLServerInterface.Tables__Get(primaryKey.TableId, false, true, true);
        }
        //----------------------------------------------------------------------------------------------------
        public static void PrimaryKey__Delete(PrimaryKey primaryKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__PrimaryKey_Drop(primaryKey);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(primaryKey.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
            var tableStructure_ResetCache = DbInterfaces.SQLServerInterface.Tables__Get(primaryKey.TableId, false, true, true);
        }
        //----------------------------------------------------------------------------------------------------
        private static void PrimaryKey__SeeIfIdentityChanged(PrimaryKey primaryKey)
        {
            bool saveIsNeeded = false;
            var currentTableStructure = Tables__Get(primaryKey.TableId, true);
            Identity currentIdentity = null;
            foreach (var col in currentTableStructure.Columns)
                if (col.Identity != null)
                    currentIdentity = col.Identity;
            Identity newIdentity = null;
            foreach (var col in primaryKey.Columns)
                if (col.Identity != null)
                    newIdentity = col.Identity;

            if (currentIdentity == null && newIdentity == null)
                saveIsNeeded = false;
            else if (currentIdentity != null && newIdentity == null)
                saveIsNeeded = true;
            else if (currentIdentity == null && newIdentity != null)
                saveIsNeeded = true;
            else
            {
                if (currentIdentity.ColumnName.ToLower() != newIdentity.ColumnName.ToLower())
                    saveIsNeeded = true;
                else if (currentIdentity.Seed != newIdentity.Seed)
                    saveIsNeeded = true;
                else if (currentIdentity.Increment != newIdentity.Increment)
                    saveIsNeeded = true;
            }

            if (saveIsNeeded)
            {
                var newTableStructure = Tables__Get(primaryKey.TableId, true);
                foreach(var column in newTableStructure.Columns)
                {
                    column.IsIdentity = false;
                    column.Identity = null;
                    if (newIdentity != null && column.ColumnName.ToLower() == newIdentity.ColumnName.ToLower())
                    {
                        column.IsIdentity = true;
                        column.Identity = newIdentity;
                    }
                }
                string sql = SQLServer_SQLBuilders.BuildSql__Table_UpdateWithRecreate(newTableStructure, currentTableStructure);
                ASPdb.Framework.Debug.WriteLine(sql);
                using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(currentTableStructure.ConnectionId, sql))
                {
                    command.Command.ExecuteNonQuery();
                }
            }
        }


        
        
        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Key == lowercase ConstraintName.
        /// Value == ConstraintName.
        /// </summary>
        public static Dictionary<string, string> Constraint__GetAll_InConnection(int connectionId)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new Dictionary<string, string>();
            string sql = SQLServer_SQLBuilders.BuildSql__Constraint_SelectAll();
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        string constraintName = reader.Get("ConstraintName", "");
                        string constraintName_L = constraintName.ToLower();
                        if (!rtn.ContainsKey(constraintName_L))
                            rtn.Add(constraintName_L, constraintName);
                    }
                }
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static DefaultConstraint DefaultConstraint__Get(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var dict = new Dictionary<string, DefaultConstraint>();
            string sql = SQLServer_SQLBuilders.BuildSql__DefaultConstraint_Select(column);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(column.ConnectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        string columnName = reader.Get("ColumnName", "");
                        string columnName_L = columnName.ToLower();

                        var defaultConstraint = new DefaultConstraint()
                        {
                            ConnectionId = column.ConnectionId,
                            Schema = column.Schema,
                            TableName = column.TableName,
                            ColumnName = reader.Get("ColumnName", ""),
                            DefaultConstraintName = reader.Get("DefaultConstraintName", ""),
                            DefaultDefinition = reader.Get("DefaultDefinition", "")
                        };

                        if (!dict.ContainsKey(columnName_L))
                            dict.Add(columnName_L, defaultConstraint);
                    }
                }
            }
            if(dict.ContainsKey(column.ColumnName.ToLower()))
                return dict[column.ColumnName.ToLower()];
            else
                return null;
        }




        //----------------------------------------------------------------------------------------------------
        public static void Identity__Create(Identity identity)
        {
            // http://stackoverflow.com/questions/1049210/adding-an-identity-to-an-existing-column
        }
        //----------------------------------------------------------------------------------------------------
        public static void Identity__Update(Identity identity)
        {
        }
        //----------------------------------------------------------------------------------------------------
        public static void Identity__Delete(Identity identity)
        {
        }



        
        //----------------------------------------------------------------------------------------------------
        public static ForeignKeysPair ForeignKey__Get(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new ForeignKeysPair();
            ForeignKey[] fkSide_ForeignKeys;
            ForeignKey[] pkSide_ForeignKeys;
            ForeignKey__Get(tableStructure, out fkSide_ForeignKeys, out pkSide_ForeignKeys);
            rtn.InboundKeys = pkSide_ForeignKeys;
            rtn.OutboundKeys = fkSide_ForeignKeys;
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static void ForeignKey__Get(TableStructure tableStructure, out ForeignKey[] fkSide_ForeignKeys, out ForeignKey[] pkSide_ForeignKeys)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__ForeignKey_Select(tableStructure);

            var fkSide_ForeignKeysDict = new Dictionary<string, ForeignKey>();
            var pkSide_ForeignKeysDict = new Dictionary<string, ForeignKey>();
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        string relationshipSide = reader.Get("RelationshipSide", "").ToUpper();
                        string constraintName = reader.Get("ConstraintName", "");
                        string constraintName_L = constraintName.ToLower();
                        int ordinalPosition = reader.Get("OrdinalPosition", 0);
                        string fk_Schema = reader.Get("FK_Schema", "");
                        string fk_Table = reader.Get("FK_Table", "");
                        string fk_Column = reader.Get("FK_Column", "");
                        string pk_Schema = reader.Get("PK_Schema", "");
                        string pk_Table = reader.Get("PK_Table", "");
                        string pk_Column = reader.Get("PK_Column", "");
                        string updateRule = reader.Get("UpdateRule", "");
                        string deleteRule = reader.Get("DeleteRule", "");

                        var dict = fkSide_ForeignKeysDict;
                        var eRelationshipSide = ForeignKey.E_RelationshipSides.ForeignKeySide;
                        if (relationshipSide == "PK")
                        {
                            dict = pkSide_ForeignKeysDict;
                            eRelationshipSide = ForeignKey.E_RelationshipSides.PrimaryKeySide;
                        }

                        ForeignKey foreignKey;
                        if (dict.ContainsKey(constraintName_L))
                            foreignKey = dict[constraintName_L];
                        else
                        {
                            dict.Add(constraintName_L, new ForeignKey());
                            foreignKey = dict[constraintName_L];
                            foreignKey.ConnectionId = tableStructure.ConnectionId;
                            foreignKey.ConstraintName = constraintName;
                            foreignKey.RelationshipSide = eRelationshipSide;

                            foreignKey.ForeignKey_Schema = fk_Schema;
                            foreignKey.ForeignKey_TableName = fk_Table;

                            foreignKey.PrimaryKey_Schema = pk_Schema;
                            foreignKey.PrimaryKey_TableName = pk_Table;

                            foreignKey.DeleteRule = ForeignKey__Get__GetCascadeOption(deleteRule);
                            foreignKey.UpdateRule = ForeignKey__Get__GetCascadeOption(updateRule);

                            foreignKey.Columns = new ForeignKeyColumn[0];
                        }
                        var columnsList = foreignKey.Columns.ToList();
                        var foreignKeyColumn = new ForeignKeyColumn()
                        {
                            ForeignKey_ColumnName = fk_Column,
                            PrimaryKey_ColumnName = pk_Column,
                            OrdinalPosition = ordinalPosition
                        };
                        columnsList.Add(foreignKeyColumn);
                        foreignKey.Columns = columnsList.ToArray();
                    }
                }
            }
            fkSide_ForeignKeys = fkSide_ForeignKeysDict.Values.ToArray();
            pkSide_ForeignKeys = pkSide_ForeignKeysDict.Values.ToArray();
        }
        //----------------------------------------------------------------------------------------------------
        public static void ForeignKey__Create(ForeignKey foreignKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var allConstraints = Constraint__GetAll_InConnection(foreignKey.ConnectionId);
            string sql = SQLServer_SQLBuilders.BuildSql__ForeignKey_Create(foreignKey, true, allConstraints);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(foreignKey.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void ForeignKey__Update(ForeignKey foreignKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__ForeignKey_Update(foreignKey);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(foreignKey.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void ForeignKey__Delete(ForeignKey foreignKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__ForeignKey_Drop(foreignKey);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(foreignKey.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        private static ForeignKey.E_CascadeOptions ForeignKey__Get__GetCascadeOption(string strCascadeOption)
        {
            switch (strCascadeOption.ToLower())
            {
                case "no action":
                    return ForeignKey.E_CascadeOptions.NoAction;
                case "cascade":
                    return ForeignKey.E_CascadeOptions.Cascade;
                case "set null":
                    return ForeignKey.E_CascadeOptions.SetNull;
                case "set default":
                    return ForeignKey.E_CascadeOptions.SetDefault;
            }
            return ForeignKey.E_CascadeOptions.NoAction;
        }




        //----------------------------------------------------------------------------------------------------
        /// <summary>
        /// Key in return Dictionary is lowercase index name.
        /// Does not include sub columns infos.
        /// </summary>
        private static Dictionary<string, Index> Index__GetAll_InConnection(int connectionId)
        {
            var rtn = new Dictionary<string, Index>();
            string sql = SQLServer_SQLBuilders.BuildSql__Index_SelectAll();
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(connectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        var index = new Index()
                        {
                            ConnectionId = connectionId,
                            Schema = reader.Get("Schema", ""),
                            TableName = reader.Get("TableName", ""),
                            IndexName = reader.Get("IndexName", ""),
                            Columns = null,
                            IsUnique = reader.Get("IsUnique", false)
                        };
                        string key_IndexName = index.IndexName.ToLower();
                        if (!rtn.ContainsKey(key_IndexName))
                            rtn.Add(key_IndexName, index);
                    }
                }
            }
            return rtn;
        }
        //----------------------------------------------------------------------------------------------------
        public static Index[] Index__Get(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var rtn = new Dictionary<string, Index>(); // key : indexName_L
            string sql = SQLServer_SQLBuilders.BuildSql__Index_Select(tableStructure);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(tableStructure.ConnectionId, sql))
            {
                using (DbReaderWrapper reader = command.ExecuteReaderWrapper())
                {
                    while (reader.Read())
                    {
                        string schema = reader.Get("Schema", "");
                        string tableName = reader.Get("TableName", "");
                        string indexName = reader.Get("IndexName", "");
                        string indexName_L = indexName.ToLower();
                        bool isUnique = reader.Get("IsUnique", false);

                        int columnId = reader.Get("ColumnId", 0);
                        string columnName = reader.Get("ColumnName", "");
                        bool isDescending = reader.Get("IsDescending", false);

                        Index index;
                        if (rtn.ContainsKey(indexName_L))
                            index = rtn[indexName_L];
                        else
                        {
                            index = new Index();
                            rtn.Add(indexName_L, index);
                            index.ConnectionId = tableStructure.ConnectionId;
                            index.Schema = schema;
                            index.TableName = tableName;
                            index.IndexName = indexName;
                            index.IsUnique = isUnique;
                            index.Columns = new IndexColumn[0];
                        }
                        var indexColumnsList = index.Columns.ToList();
                        var indexColumn = new IndexColumn()
                        {
                            ColumnId = columnId,
                            ColumnName = columnName,
                            SortDirection = IndexColumn.E_SortTypes.Ascending
                        };
                        if (isDescending)
                            indexColumn.SortDirection = IndexColumn.E_SortTypes.Descending;
                        indexColumnsList.Add(indexColumn);
                        index.Columns = indexColumnsList.ToArray();
                    }
                }
            }
            return rtn.Values.ToArray();
        }
        //----------------------------------------------------------------------------------------------------
        public static void Index__Create(Index index)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            var allIndexes = Index__GetAll_InConnection(index.ConnectionId);
            string sql = SQLServer_SQLBuilders.BuildSql__Index_Create(index, true, allIndexes);
            using(DbConnectionCommand command = UniversalADO.OpenConnectionCommand(index.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Index__Update(Index index)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__Index_Update(index);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(index.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }
        //----------------------------------------------------------------------------------------------------
        public static void Index__Delete(Index index)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = SQLServer_SQLBuilders.BuildSql__Index_Drop(index);
            using (DbConnectionCommand command = UniversalADO.OpenConnectionCommand(index.ConnectionId, sql))
            {
                command.Command.ExecuteNonQuery();
            }
        }


    }
}