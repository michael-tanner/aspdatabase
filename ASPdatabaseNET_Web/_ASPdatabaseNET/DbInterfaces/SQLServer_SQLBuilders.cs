using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ASPdb.Framework;
using ASPdatabaseNET.DbInterfaces.TableObjects;

namespace ASPdatabaseNET.DbInterfaces
{
    //----------------------------------------------------------------------------------------------------////
    public class SQLServer_SQLBuilders
    {
        //----------------------------------------------------------------------------------------------------
        //public static string BuildSql__Table_Select(TableStructure tableStructure)
        //{
        //    tableStructure.Validate(Enums.ValidationTypes.Select);
        //    string sql = "";


        //    return sql;
        //}
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Table_Create(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            tableStructure.Validate(Enums.ValidationTypes.Create);
            string sql = "";

            tableStructure.Columns = (from c in tableStructure.Columns
                                      orderby c.OrdinalPosition
                                      select c).ToArray();
            string sql_Columns = "";
            foreach (var column in tableStructure.Columns)
            {
                //------------------------------ 
                if (sql_Columns != "")
                    sql_Columns += ", \n                   ";

                //------------------------------ Null / Not Null
                string strNull = " NOT NULL";
                if (column.AllowNulls)
                    strNull = " NULL";

                //------------------------------ Default
                string strDefault = "";
                if (column.DefaultValue != null)
                {
                    if (column.IsNumericType_GetAndSet() && !column.DefaultValue.EndsWith(")"))
                        column.DefaultValue = "(" + column.DefaultValue + ")";

                    string dValue = column.DefaultValue;
                    if (dValue.StartsWith("'") && !dValue.StartsWith("''")) dValue = dValue.Substring(1, dValue.Length - 1);
                    if (dValue.EndsWith("'") && !dValue.EndsWith("''")) dValue = dValue.Substring(0, dValue.Length - 1);
                    dValue = dValue.Replace("'", "''");

                    if (dValue.EndsWith(")"))
                        strDefault = " DEFAULT" + dValue + "";
                    else
                        strDefault = " DEFAULT(N'" + dValue + "')";
                }




                //------------------------------ Identity
                string strIdentity = "";
                if (column.IsIdentity && column.Identity != null)
                {
                    strNull = " NOT NULL";
                    strDefault = "";
                    strIdentity = String.Format(" IDENTITY({0},{1})", column.Identity.Seed, column.Identity.Increment);
                }

                //------------------------------ 
                sql_Columns += String.Format(" [{0}] {1}{2}{3}{4}", column.ColumnName, column.DataType, strIdentity, strNull, strDefault);
            }
            sql = String.Format(@"
                CREATE TABLE [{0}].[{1}]
                (  {2}  );
                ",
                 tableStructure.Schema, tableStructure.TableName,
                 sql_Columns);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Table_Drop(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            tableStructure.Validate(Enums.ValidationTypes.Drop);

            string sql = String.Format(@" DROP TABLE [{0}].[{1}]; ", tableStructure.Schema, tableStructure.TableName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Table_UpdateWithRecreate(TableStructure newTable, TableStructure oldTable)
        {
            AjaxService.ASPdatabaseService.GetSetVal();

            //tableStructure.Validate(Enums.ValidationTypes.Update);

            // ------------------------ ENSURE COLUMNS MATCH

            /* Steps
             *  1) Foreach FK_Side Foreign Key:
             *      a) BEGIN TRANSACTION
             *      b) Drop Foreign Keys --> oldTable
             *      c) ALTER TABLE [Schema].[TableName] SET (LOCK_ESCALATION = TABLE)
             *      d) COMMIT
             * 
             *  2) BEGIN TRANSACTION
             *  3) CREATE TABLE --> Temp
             *  4) ALTER TABLE [Schema].[Temp] SET (LOCK_ESCALATION = TABLE)
             *  5) SET IDENTITY_INSERT [Schema].[Temp] ON
             *  6) IF EXISTS(SELECT * FROM [Schema].[TableName]) ... EXEC('INSERT INTO ... [Schema].[Temp]
             *  7) SET IDENTITY_INSERT [Schema].[Temp] OFF
             * 
             *  8) DROP PK_Side Foreign Keys
             *  9) DROP TABLE [Schema].[TableName]
             * 10) EXECUTE sp_rename N'Schema.Temp', N'TableName', 'OBJECT' 
             * 
             * 11) Add Primary Key
             * 12) Add Indexes
             * 13) Add Foreign Keys
             * 14) COMMIT
            */
            string sql = "";

            string schema = oldTable.Schema;
            string tableName = oldTable.TableName;
            string tempTableName = "ASPdb__TEMP__" + oldTable.TableName + "__1";

            bool newTableHasIdentity = false;
            foreach (var column in newTable.Columns)
                if (column.Identity != null)
                    newTableHasIdentity = true;



            newTable.SetTableName(tempTableName); // ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ? ?

            sql += String.Format(@"
            BEGIN TRANSACTION T0

                SET QUOTED_IDENTIFIER ON
                SET ARITHABORT ON
                SET NUMERIC_ROUNDABORT OFF
                SET CONCAT_NULL_YIELDS_NULL ON
                SET ANSI_NULLS ON
                SET ANSI_PADDING ON
                SET ANSI_WARNINGS ON
                SET XACT_ABORT ON
                ");

            sql += String.Format(@"
                IF OBJECT_ID('{0}.{1}', 'U') IS NOT NULL
                    DROP TABLE [{0}].[{1}]
                ", schema, tempTableName);

            if(oldTable.ForeignKeys_FK != null)
                foreach (var foreignKey in oldTable.ForeignKeys_FK)
                {
                    sql += String.Format(@"
                ALTER TABLE [{0}].[{1}]
	                DROP CONSTRAINT [{2}]
                ALTER TABLE [{3}].[{4}] SET (LOCK_ESCALATION = TABLE)
                ",
                    schema,
                    tableName,
                    foreignKey.ConstraintName,
                    foreignKey.PrimaryKey_Schema,
                    foreignKey.PrimaryKey_TableName);
                }


            sql += BuildSql__Table_Create(newTable);

            var tmpPrimaryKey = new PrimaryKey() { ConnectionId = newTable.ConnectionId, Schema = newTable.Schema, TableId = newTable.TableId, TableName = newTable.TableName };
            tmpPrimaryKey.Columns = new PrimaryKeyColumn[oldTable.PrimaryKey.Columns.Length];
            for (int i = 0; i < oldTable.PrimaryKey.Columns.Length; i++)
                tmpPrimaryKey.Columns[i] = oldTable.PrimaryKey.Columns[i];
            string tmpConstraintName = oldTable.PrimaryKey.ConstraintName;
            if (!tmpConstraintName.Contains("__"))
                tmpConstraintName += "__1";
            else
            {
                string rightPart = tmpConstraintName.Split(new string[] { "__" }, StringSplitOptions.None).Last();
                int numb = -1;
                Int32.TryParse(rightPart, out numb);
                if(numb < 0)
                    tmpConstraintName += "__1";
                else
                    tmpConstraintName = tmpConstraintName.Substring(0, tmpConstraintName.Length - rightPart.Length) + (numb + 1).ToString();
            }
            tmpPrimaryKey.ConstraintName = tmpConstraintName;
            if (oldTable.PrimaryKey != null)
                sql += BuildSql__PrimaryKey_Create(tmpPrimaryKey, false, null, true) + @"
                ";


            string[] arr_Columns = (from c in newTable.Columns select c.ColumnName).ToArray();
            string sql_ColumnsCommaList = Str.Join(arr_Columns, "\n                                    [", "], \n                                    [", "]");
            sql += String.Format(@"
                ALTER TABLE [{0}].[{1}] SET (LOCK_ESCALATION = TABLE)
                ", schema, tempTableName);

            if (newTableHasIdentity)
                sql += String.Format(@"
                SET IDENTITY_INSERT [{0}].[{1}] ON
                ", schema, tempTableName);

            sql += String.Format(@"
                IF EXISTS(SELECT * FROM [{0}].[{1}])
	                 EXEC('INSERT INTO [{0}].[{2}] ( {3} )
		                SELECT {3}
		                FROM [{0}].[{1}] WITH (HOLDLOCK, TABLOCKX)')
                ", schema, tableName, tempTableName, sql_ColumnsCommaList);

            if (newTableHasIdentity)
                sql += String.Format(@"
                SET IDENTITY_INSERT [{0}].[{1}] OFF
                ", schema, tempTableName);

            
            if(oldTable.ForeignKeys_PK != null)
                foreach (var foreignKey in oldTable.ForeignKeys_PK)
                {
                    sql += String.Format(@"
                        ALTER TABLE [{0}].[{1}]
	                        DROP CONSTRAINT [{2}]
                        ", foreignKey.ForeignKey_Schema, foreignKey.ForeignKey_TableName, foreignKey.ConstraintName);
                }

            sql += String.Format(@"
                DROP TABLE [{0}].[{1}]

                EXECUTE sp_rename N'{0}.{2}', N'{1}', 'OBJECT' 

                ", schema, tableName, tempTableName);

//            if (oldTable.PrimaryKey != null)
//                sql += BuildSql__PrimaryKey_Create(oldTable.PrimaryKey, false, null) + @"
//                ";

            if(oldTable.Indexes != null)
                foreach (var index in oldTable.Indexes)
                    sql += @"
                " + BuildSql__Index_Create(index, false, null) + @"
                
                ";

            if (oldTable.ForeignKeys_FK != null)
                foreach (var foreignKey in oldTable.ForeignKeys_FK)
                    sql += BuildSql__ForeignKey_Create(foreignKey, false, null) + @"
                    ";

            if (oldTable.ForeignKeys_PK != null)
                foreach (var foreignKey in oldTable.ForeignKeys_PK)
                    sql += String.Format(@"
                        {0}

                        ALTER TABLE [{1}].[{2}] SET (LOCK_ESCALATION = TABLE)
                    ", 
                     BuildSql__ForeignKey_Create(foreignKey, false, null), 
                     foreignKey.ForeignKey_Schema, 
                     foreignKey.ForeignKey_TableName);

            sql += @"
            COMMIT TRANSACTION T0
            ";

            return sql;
        }




        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Column_SelectAll(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();

            tableStructure.Validate(Enums.ValidationTypes.Select);

            string sql = String.Format(@"
                SELECT 
	                col.TABLE_SCHEMA as [Schema],
	                col.TABLE_NAME as [TableName],
	                col.ORDINAL_POSITION as [OrdinalPosition],
	                col.COLUMN_NAME as [ColumnName],
	                col.DATA_TYPE as [DataType],
	                col.CHARACTER_MAXIMUM_LENGTH as [MaxLength],
	                col.NUMERIC_PRECISION as [Precision],
	                col.NUMERIC_SCALE as [Scale],
	                col.IS_NULLABLE as [AllowNulls],
	                col.COLUMN_DEFAULT as [DefaultValue],
                    CASE WHEN CCU.COLUMN_NAME IS NULL THEN 0 ELSE 1 END AS [IsPrimaryKey],
	                COLUMNPROPERTY(object_id('[' + col.Table_Schema + '].[' + col.Table_Name + ']'), COL.COLUMN_NAME, 'IsIdentity') AS [IsIdentity],
	                IDENT_SEED('[' + col.Table_Schema + '].[' + col.Table_Name + ']') AS Identity_Seed,
	                IDENT_INCR('[' + col.Table_Schema + '].[' + col.Table_Name + ']') AS Identity_Increment,
	                IDENT_CURRENT('[' + col.Table_Schema + '].[' + col.Table_Name + ']') AS Identity_CurrentIdentity
                FROM 
                    INFORMATION_SCHEMA.COLUMNS as col
                left JOIN
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS as tc
                    ON col.TABLE_SCHEMA = tc.TABLE_SCHEMA AND col.TABLE_NAME = tc.TABLE_NAME AND tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                left JOIN
                    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE as ccu 
                    ON col.TABLE_SCHEMA = tc.TABLE_SCHEMA AND col.COLUMN_NAME = ccu.COLUMN_NAME AND tc.CONSTRAINT_NAME = ccu.CONSTRAINT_NAME 
                where col.[TABLE_SCHEMA] = '{0}' and col.[TABLE_NAME] = '{1}'
                order by
                    col.TABLE_SCHEMA, col.TABLE_NAME, col.ORDINAL_POSITION
                ",
                 tableStructure.Schema,
                 tableStructure.TableName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Column_Create(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            column.Validate(Enums.ValidationTypes.Create);


            //------------------------------ Null / Not Null
            string strNull = " NOT NULL";
            if (column.AllowNulls)
                strNull = " NULL";

            //------------------------------ Default
            string strDefault = "";
            if (column.DefaultValue != null)
            {
                ASPdb.Framework.Debug.WriteLine("column.DataType:                  " + column.DataType);
                ASPdb.Framework.Debug.WriteLine("column.DataType_Name:             " + column.DataType_Name);
                ASPdb.Framework.Debug.WriteLine("column.IsNumericType_GetAndSet(): " + column.IsNumericType_GetAndSet());
                ASPdb.Framework.Debug.WriteLine("");

                ASPdb.Framework.Debug.WriteLine("column.DefaultValue: " + column.DefaultValue);
                if (column.IsNumericType_GetAndSet() && !column.DefaultValue.EndsWith(")"))
                    column.DefaultValue = "(" + column.DefaultValue.Replace("'", "") + ")";
                ASPdb.Framework.Debug.WriteLine("column.DefaultValue: " + column.DefaultValue);
                ASPdb.Framework.Debug.WriteLine("");

                if (column.DefaultValue.EndsWith(")"))
                    strDefault = " DEFAULT" + column.DefaultValue + "";
                else
                    strDefault = " DEFAULT(N'" + column.DefaultValue + "')";
            }

            string sql = String.Format(@"
                ALTER TABLE [{0}].[{1}]
                ADD [{2}] {3} {4} {5}
                ", column.Schema, column.TableName
                 , column.ColumnName, column.DataType, strNull, strDefault);

            ASPdb.Framework.Debug.WriteLine(sql);
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Column_Drop(Column column, string defaultConstraintName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            column.Validate(Enums.ValidationTypes.Drop);
            string sql = "";

            if (defaultConstraintName != null)
                sql += String.Format("ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]",
                    column.Schema, column.TableName, defaultConstraintName);

            sql += String.Format(@"
                ALTER TABLE [{0}].[{1}]
                DROP COLUMN [{2}] 
                ", column.Schema, column.TableName
                 , column.ColumnName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Column_Update(Column column, string defaultConstraintName)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            column.Validate(Enums.ValidationTypes.Update);

            //------------------------------ Null / Not Null
            string strNull = " NOT NULL";
            if (column.AllowNulls)
                strNull = " NULL";

            //------------------------------ Default
            string strDefault = "";
            if (column.DefaultValue != null)
            {
                if (column.IsNumericType_GetAndSet() && !column.DefaultValue.EndsWith(")"))
                    column.DefaultValue = "(" + column.DefaultValue + ")";

                if (column.DefaultValue.EndsWith(")"))
                    strDefault = " DEFAULT" + column.DefaultValue + "";
                else
                    strDefault = " DEFAULT(N'" + column.DefaultValue + "')";
            }

            string sql = @"
            BEGIN TRANSACTION
            ";

            if (defaultConstraintName != null)
                sql += String.Format(@"
                ALTER TABLE [{0}].[{1}]
                DROP CONSTRAINT [{2}]
                ", column.Schema, column.TableName, defaultConstraintName);

            sql += String.Format(@"
                ALTER TABLE [{0}].[{1}]
                ALTER COLUMN [{2}] {3} {4};
                ", column.Schema, column.TableName
                 , column.ColumnName_Original, column.DataType, strNull);

            if (column.DefaultValue != null)
                sql += String.Format(@"
                ALTER TABLE [{0}].[{1}] 
                ADD {2} FOR [{3}];
                ", column.Schema, column.TableName
                 , strDefault, column.ColumnName_Original);

            if (!String.IsNullOrEmpty(column.ColumnName) && column.ColumnName != column.ColumnName_Original)
                sql += String.Format("EXEC sp_RENAME '{0}.{1}.{2}' , '{3}', 'COLUMN'",
                    column.Schema, column.TableName, column.ColumnName_Original, column.ColumnName);

            sql += @"
            COMMIT
            ";
            return sql;
        }




        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__PrimaryKey_Select(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            tableStructure.Validate(Enums.ValidationTypes.Select);

            string sql = String.Format(@"
                SELECT 
	                [ConstraintName] = kcu.CONSTRAINT_NAME,
	                [TableCatalog] = kcu.TABLE_CATALOG,
	                [Schema] = kcu.TABLE_SCHEMA,
	                [TableName] = kcu.TABLE_NAME,
	                [ColumnName] = kcu.COLUMN_NAME,
	                [OrdinalPosition] = kcu.ORDINAL_POSITION,
					[ConstraintType] = tc.CONSTRAINT_TYPE
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE as kcu
				Inner Join INFORMATION_SCHEMA.TABLE_CONSTRAINTS as tc ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
                    and kcu.TABLE_SCHEMA = '{0}' and kcu.Table_Name = '{1}'
                Order By kcu.ORDINAL_POSITION;
                ",
                 tableStructure.Schema,
                 tableStructure.TableName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__PrimaryKey_Create(PrimaryKey primaryKey, bool autoFixName, Dictionary<string, string> allConstraints, bool clustered)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            primaryKey.Validate(Enums.ValidationTypes.Create);

            if (autoFixName && allConstraints != null)
            {
                if (String.IsNullOrEmpty(primaryKey.ConstraintName))
                    primaryKey.ConstraintName = String.Format("PK_{0}_{1}", primaryKey.Schema, primaryKey.TableName);
                if (allConstraints.ContainsKey(primaryKey.ConstraintName.ToLower()))
                {
                    int loopMax = 10000;
                    for (int i = 1; i < loopMax; i++)
                    {
                        string tryName = primaryKey.ConstraintName + "_" + i.ToString("0000");
                        if (!allConstraints.ContainsKey(tryName.ToLower()))
                        {
                            primaryKey.ConstraintName = tryName;
                            i = loopMax + 1;
                        }
                    }
                }
            }
            else if (String.IsNullOrEmpty(primaryKey.ConstraintName))
                throw new Exception("ConstraintName value not provided.");
            ASPdb.Framework.Validation.ValidateTextForSql1(primaryKey.ConstraintName, true);


            primaryKey.Columns = (from c in primaryKey.Columns
                                  orderby c.OrdinalPosition
                                  select c).ToArray();
            string sql_Columns = "";
            foreach (var col in primaryKey.Columns)
            {
                ASPdb.Framework.Validation.ValidateTextForSql1(col.ColumnName, true);
                if (sql_Columns != "")
                    sql_Columns += ", ";
                sql_Columns += " [" + col.ColumnName + "]";
            }
            string sql_Clustered = "";
            if (clustered) sql_Clustered = " CLUSTERED ";

            string sql = String.Format(@"
                ALTER TABLE [{0}].[{1}]
                ADD CONSTRAINT [{2}] 
                PRIMARY KEY {3} ({4});
                ",
                primaryKey.Schema, primaryKey.TableName,
                primaryKey.ConstraintName,
                sql_Clustered, sql_Columns);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__PrimaryKey_Drop(PrimaryKey primaryKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            primaryKey.Validate(Enums.ValidationTypes.Drop);

            string sql = String.Format(" ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]; ", primaryKey.Schema, primaryKey.TableName, primaryKey.ConstraintName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__PrimaryKey_Update(PrimaryKey primaryKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = String.Format(@"
                BEGIN TRAN T1;
                    {0}
                    {1}
                COMMIT TRAN T1;
            ",
             BuildSql__PrimaryKey_Drop(primaryKey),
             BuildSql__PrimaryKey_Create(primaryKey, false, null, true));

            return sql;
        }




        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Constraint_SelectAll()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = @"
                SELECT 
	                distinct
	                [ConstraintName] = CONSTRAINT_NAME,
	                [TableCatalog] = TABLE_CATALOG,
	                [Schema] = TABLE_SCHEMA,
	                [TableName] = TABLE_NAME
                FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                --WHERE OBJECTPROPERTY(OBJECT_ID(constraint_name), 'IsPrimaryKey') = 1
                Order By CONSTRAINT_NAME
                ";

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__DefaultConstraint_SelectAll()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = @"
                SELECT
	                [ColumnName] = all_columns.name,
                    [DefaultConstraintName] = default_constraints.name, 
                    [DefaultDefinition] = default_constraints.definition
                FROM sys.all_columns
                INNER JOIN
                    sys.tables ON all_columns.object_id = tables.object_id
                INNER JOIN 
                    sys.schemas ON tables.schema_id = schemas.schema_id
                INNER JOIN
                    sys.default_constraints ON all_columns.default_object_id = default_constraints.object_id
                Order By 
	                schemas.name, tables.name, all_columns.name, default_constraints.name
                ";
            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__DefaultConstraint_Select(Column column)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = String.Format(@"
                SELECT
	                [ColumnName] = all_columns.name,
                    [DefaultConstraintName] = default_constraints.name, 
                    [DefaultDefinition] = default_constraints.definition
                FROM sys.all_columns
                INNER JOIN
                    sys.tables ON all_columns.object_id = tables.object_id
                INNER JOIN 
                    sys.schemas ON tables.schema_id = schemas.schema_id
                INNER JOIN
                    sys.default_constraints ON all_columns.default_object_id = default_constraints.object_id
                WHERE 
                    schemas.name = '{0}' AND tables.name = '{1}'
                Order By 
	                all_columns.name, default_constraints.name
                ", column.Schema, column.TableName);
            return sql;
        }





        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__ForeignKey_Select(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            tableStructure.Validate(Enums.ValidationTypes.Select);

            string sql = String.Format(@"
	                (select
		                [RelationshipSide] = 'FK',
		                [ConstraintName] = RC.CONSTRAINT_NAME,
		                [OrdinalPosition] = CU.ORDINAL_POSITION,
		                [FK_Schema] = FK.TABLE_SCHEMA,
		                [FK_Table] = FK.TABLE_NAME,
		                [FK_Column] = CU.COLUMN_NAME,
		                [PK_Schema] = PK.TABLE_SCHEMA,
		                [PK_Table] = PK.TABLE_NAME,
		                [PK_Column] = PT.COLUMN_NAME,
		                [UpdateRule] = RC.UPDATE_RULE,
		                [DeleteRule] = RC.DELETE_RULE
	                from
		                INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
	                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
		                on RC.CONSTRAINT_NAME = FK.CONSTRAINT_NAME	
	                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
		                on RC.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
	                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU
		                on RC.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
	                INNER JOIN (SELECT t1.TABLE_SCHEMA, t1.TABLE_NAME, t2.COLUMN_NAME, t2.ORDINAL_POSITION
				                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS t1
				                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE t2
					                on t1.CONSTRAINT_NAME = t2.CONSTRAINT_NAME
				                WHERE t1.CONSTRAINT_TYPE = 'PRIMARY KEY'
			                   ) PT
		                on 
		                PT.TABLE_SCHEMA = PK.TABLE_SCHEMA and
		                PT.TABLE_NAME = PK.TABLE_NAME and 
		                PT.ORDINAL_POSITION = CU.ORDINAL_POSITION
	                where 
		                FK.TABLE_SCHEMA = '{0}' and FK.TABLE_NAME = '{1}')
                UNION
	                (select
		                [RelationshipSide] = 'PK',
		                [ConstraintName] = RC.CONSTRAINT_NAME,
		                [OrdinalPosition] = CU.ORDINAL_POSITION,
		                [FK_Schema] = FK.TABLE_SCHEMA,
		                [FK_Table] = FK.TABLE_NAME,
		                [FK_Column] = CU.COLUMN_NAME,
		                [PK_Schema] = PK.TABLE_SCHEMA,
		                [PK_Table] = PK.TABLE_NAME,
		                [PK_Column] = PT.COLUMN_NAME,
		                [UpdateRule] = RC.UPDATE_RULE,
		                [DeleteRule] = RC.DELETE_RULE
	                from
		                INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS RC
	                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK
		                on RC.CONSTRAINT_NAME = FK.CONSTRAINT_NAME	
	                INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK
		                on RC.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME
	                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU
		                on RC.CONSTRAINT_NAME = CU.CONSTRAINT_NAME
	                INNER JOIN (SELECT t1.TABLE_SCHEMA, t1.TABLE_NAME, t2.COLUMN_NAME, t2.ORDINAL_POSITION
				                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS t1
				                INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE t2
					                on t1.CONSTRAINT_NAME = t2.CONSTRAINT_NAME
				                WHERE t1.CONSTRAINT_TYPE = 'PRIMARY KEY'
			                   ) PT
		                on 
		                PT.TABLE_SCHEMA = PK.TABLE_SCHEMA and
		                PT.TABLE_NAME = PK.TABLE_NAME and 
		                PT.ORDINAL_POSITION = CU.ORDINAL_POSITION
	                where 
		                PK.TABLE_SCHEMA = '{0}' and PK.TABLE_NAME = '{1}')
                Order By
	                RelationshipSide, ConstraintName, OrdinalPosition;
                ",
                 tableStructure.Schema,
                 tableStructure.TableName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__ForeignKey_Create(ForeignKey foreignKey, bool autoFixName, Dictionary<string, string> allConstraints)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            foreignKey.Validate(Enums.ValidationTypes.Create);

            string optionalSQL_ConstraintName = "";
            string sql_FK_Columns = "";
            string sql_PK_Columns = "";
            string sql_DeleteRule = "";
            string sql_UpdateRule = "";

            //----------//----------//----------//
            if (autoFixName && allConstraints != null)
            {
                if (String.IsNullOrEmpty(foreignKey.ConstraintName))
                    foreignKey.ConstraintName = String.Format("FK_{0}_{1}", foreignKey.ForeignKey_Schema, foreignKey.ForeignKey_TableName);
                if (allConstraints.ContainsKey(foreignKey.ConstraintName.ToLower()))
                {
                    int loopMax = 10000;
                    for (int i = 1; i < loopMax; i++)
                    {
                        string tryName = foreignKey.ConstraintName + "_" + i.ToString("0000");
                        if (!allConstraints.ContainsKey(tryName.ToLower()))
                        {
                            foreignKey.ConstraintName = tryName;
                            i = loopMax + 1;
                        }
                    }
                }
            }
            //----------//----------//----------//
            if (!String.IsNullOrEmpty(foreignKey.ConstraintName))
            {
                ASPdb.Framework.Validation.ValidateTextForSql1(foreignKey.ConstraintName, true);
                optionalSQL_ConstraintName = "CONSTRAINT " + foreignKey.ConstraintName;
            }
            //----------//----------//----------//
            foreignKey.Columns = (from c in foreignKey.Columns
                                  orderby c.OrdinalPosition
                                  select c).ToArray();
            foreach (var col in foreignKey.Columns)
            {
                if (sql_FK_Columns != "")
                {
                    sql_FK_Columns += ", ";
                    sql_PK_Columns += ", ";
                }
                ASPdb.Framework.Validation.ValidateTextForSql1(col.ForeignKey_ColumnName, true);
                ASPdb.Framework.Validation.ValidateTextForSql1(col.PrimaryKey_ColumnName, true);
                sql_FK_Columns += "[" + col.ForeignKey_ColumnName + "]";
                sql_PK_Columns += "[" + col.PrimaryKey_ColumnName + "]";
            }
            //----------//----------//----------//
            switch (foreignKey.DeleteRule)
            {
                case ForeignKey.E_CascadeOptions.NoAction: sql_DeleteRule = "ON DELETE No Action"; break;
                case ForeignKey.E_CascadeOptions.Cascade: sql_DeleteRule = "ON DELETE Cascade"; break;
                case ForeignKey.E_CascadeOptions.SetDefault: sql_DeleteRule = "ON DELETE Set Default"; break;
                case ForeignKey.E_CascadeOptions.SetNull: sql_DeleteRule = "ON DELETE Set Null"; break;
            }
            //----------//----------//----------//
            switch (foreignKey.UpdateRule)
            {
                case ForeignKey.E_CascadeOptions.NoAction: sql_UpdateRule = "ON UPDATE No Action"; break;
                case ForeignKey.E_CascadeOptions.Cascade: sql_UpdateRule = "ON UPDATE Cascade"; break;
                case ForeignKey.E_CascadeOptions.SetDefault: sql_UpdateRule = "ON UPDATE Set Default"; break;
                case ForeignKey.E_CascadeOptions.SetNull: sql_UpdateRule = "ON UPDATE Set Null"; break;
            }
            //----------//----------//----------//
            //----------//----------//----------//
            string sql = String.Format(@"
                ALTER TABLE [{0}].[{1}]
                ADD
                {2}
                FOREIGN KEY
                ({3})
                REFERENCES [{4}].[{5}]({6})
                {7}
                {8};
            ",
             foreignKey.ForeignKey_Schema, foreignKey.ForeignKey_TableName,
             optionalSQL_ConstraintName,
             sql_FK_Columns,
             foreignKey.PrimaryKey_Schema, foreignKey.PrimaryKey_TableName, sql_PK_Columns,
             sql_DeleteRule,
             sql_UpdateRule);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__ForeignKey_Drop(ForeignKey foreignKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            foreignKey.Validate(Enums.ValidationTypes.Drop);

            string sql = String.Format(" ALTER TABLE [{0}].[{1}] DROP CONSTRAINT [{2}]; ", foreignKey.ForeignKey_Schema, foreignKey.ForeignKey_TableName, foreignKey.ConstraintName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__ForeignKey_Update(ForeignKey foreignKey)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = String.Format(@"
                BEGIN TRAN T1;
                    {0}
                    {1}
                COMMIT TRAN T1;
            ",
             BuildSql__ForeignKey_Drop(foreignKey),
             BuildSql__ForeignKey_Create(foreignKey, false, null));

            return sql;
        }




        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Index_SelectAll()
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = @"
                SELECT
                    [Schema] = SC.name,
                    [TableName] = T.name,
                    [IndexName] = IND.name,
                    [IsUnique] = IND.is_unique,
	                [IsPrimaryKey] = IND.is_primary_key
                FROM 
                     sys.indexes IND 
                INNER JOIN 
                     sys.tables T 
	                 on IND.object_id = T.object_id 
                INNER JOIN
	                 sys.schemas  SC 
	                 on T.schema_id = SC.schema_id
                ORDER BY 
                     SC.name, T.name, IND.name;
                ";

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Index_Select(TableStructure tableStructure)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            tableStructure.Validate(Enums.ValidationTypes.Select);

            string sql = String.Format(@"
                SELECT
                    [Schema] = SC.name,
                    [TableName] = T.name,
                    [IndexName] = IND.name,
                    [IsUnique] = IND.is_unique,
                    [ColumnId] = IC.index_column_id,
                    [ColumnName] = COL.name,
                    [IsDescending] = IC.is_descending_key
                FROM 
                     sys.indexes IND 
                INNER JOIN 
                     sys.index_columns IC 
	                 on IND.object_id = IC.object_id and IND.index_id = IC.index_id 
                INNER JOIN 
                     sys.columns COL 
	                 on IC.object_id = COL.object_id and IC.column_id = COL.column_id 
                INNER JOIN 
                     sys.tables T 
	                 on IND.object_id = T.object_id 
                INNER JOIN
	                 sys.schemas  SC 
	                 on T.schema_id = SC.schema_id
                WHERE
	                IND.is_primary_key = 0 
                    and SC.name = '{0}' 
	                and T.name = '{1}'
                ORDER BY 
                     SC.name, T.name, IND.name, IC.index_column_id;
                ", 
                 tableStructure.Schema, tableStructure.TableName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Index_Create(Index index, bool autoFixName, Dictionary<string, Index> allIndexes)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            index.Validate(Enums.ValidationTypes.Create);


            if (autoFixName && allIndexes != null)
            {
                if (String.IsNullOrEmpty(index.IndexName))
                    index.IndexName = String.Format("IX_{0}_{1}", index.Schema, index.TableName);

                int loopMax = 10000;
                for (int i = 1; i < loopMax; i++)
                {
                    string tryName = index.IndexName + "_" + i.ToString("0000");
                    if (!allIndexes.ContainsKey(tryName.ToLower()))
                    {
                        index.IndexName = tryName;
                        i = loopMax + 1;
                    }
                }
            }
            else if (String.IsNullOrEmpty(index.IndexName))
                throw new Exception("IndexName value not provided.");
            ASPdb.Framework.Validation.ValidateTextForSql1(index.IndexName, true);




            index.Columns = (from c in index.Columns
                             orderby c.ColumnId
                             select c).ToArray();
            string unique = "";
            if (index.IsUnique)
                unique = "UNIQUE";
            string sql = String.Format(@"CREATE {0} INDEX [{1}] ON [{2}].[{3}] (",
                unique, index.IndexName, index.Schema, index.TableName);
            for (int i = 0; i < index.Columns.Length; i++)
            {
                ASPdb.Framework.Validation.ValidateTextForSql1(index.Columns[i].ColumnName, true);
                if (i > 0)
                    sql += ", ";
                sql += String.Format(" [{0}]", index.Columns[i].ColumnName);
                if (index.Columns[i].SortDirection == IndexColumn.E_SortTypes.Descending)
                    sql += " desc";
            }
            sql += ") ";

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Index_Drop(Index index)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            index.Validate(Enums.ValidationTypes.Drop);

            string sql = String.Format(" DROP INDEX [{0}].[{1}].[{2}]; ", index.Schema, index.TableName, index.IndexName);

            return sql;
        }
        //----------------------------------------------------------------------------------------------------
        public static string BuildSql__Index_Update(Index index)
        {
            AjaxService.ASPdatabaseService.GetSetVal();
            string sql = String.Format(@"
                BEGIN TRAN T1;
                    {0}
                    {1}
                COMMIT TRAN T1;
                ", 
                 BuildSql__Index_Drop(index),
                 BuildSql__Index_Create(index, false, null));

            return sql;
        }

    }
}