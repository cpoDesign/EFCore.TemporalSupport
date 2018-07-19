using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EFCoreTemporalSupport
{
    /// <summary>
    /// EFCore Temporal Support, based on: https://blog.bennymichielsen.be/2017/11/07/auditing-with-ef-core-and-sql-server-part-1/
    /// </summary>
    public static class EnableTemporalDataOnTable
    {
        public static void AddAsTemporalTable(this MigrationBuilder migrationBuilder, IEntityType entityType, string temporalScheme, string temporalTableName)
        {
            var tableName = entityType.Relational().TableName;
            var schemaName = entityType.Relational().Schema ?? "dbo";
            migrationBuilder.Sql($@"
                    IF NOT EXISTS (SELECT * FROM sys.[tables] t INNER JOIN sys.schemas s ON s.schema_id = t.schema_id WHERE t.name = '{tableName}' AND temporal_type = 2 and s.name = '{schemaName}')
                    BEGIN
                        ALTER TABLE {schemaName}.{tableName}   
                        ADD  ValidFrom datetime2 (0) GENERATED ALWAYS AS ROW START HIDDEN constraint DF_{tableName}_ValidFrom DEFAULT DATEADD(second, -1, SYSUTCDATETIME())  
                            , ValidTo datetime2 (0)  GENERATED ALWAYS AS ROW END HIDDEN constraint DF_{tableName}_ValidTo DEFAULT '9999.12.31 23:59:59.99'  
                            , PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo);   
 
                        ALTER TABLE {schemaName}.{tableName}    
                        SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = {temporalScheme}.{temporalTableName})); 
                    END
                ");
        }

        public static void AddAsTemporalTable(this MigrationBuilder migrationBuilder, IEntityType entityType, string temporalScheme)
        {
            var tableName = string.Format("{0}History", entityType.Relational().TableName);
            AddAsTemporalTable(migrationBuilder, entityType, temporalScheme, tableName);
        }

        public static void RemoveAsTemporalTable(this MigrationBuilder migrationBuilder, IEntityType entityType, string temporalScheme, string temporalTableName)
        {
            var tableName = entityType.Relational().TableName;
            var schemaName = entityType.Relational().Schema ?? "dbo";
            string alterStatement = $@"ALTER TABLE {tableName} SET (SYSTEM_VERSIONING = OFF);";
            migrationBuilder.Sql(alterStatement);
            alterStatement = $@"ALTER TABLE {tableName} DROP PERIOD FOR SYSTEM_TIME";
            migrationBuilder.Sql(alterStatement);
            alterStatement = $@"ALTER TABLE {tableName} DROP CONSTRAINT DF_{tableName}_SysStart, DF_{tableName}_SysEnd";
            migrationBuilder.Sql(alterStatement);
            alterStatement = $@"ALTER TABLE {tableName} DROP COLUMN ValidFrom, COLUMN ValidTo";
            migrationBuilder.Sql(alterStatement);
            alterStatement = $@"DROP TABLE {temporalScheme}.{temporalTableName}";
            migrationBuilder.Sql(alterStatement);
        }

        public static void RemoveAsTemporalTable(this MigrationBuilder migrationBuilder, IEntityType entityType, string temporalScheme)
        {
            var tableName = string.Format("{0}History", entityType.Relational().TableName);
            RemoveAsTemporalTable(migrationBuilder, entityType, temporalScheme, tableName);
        }
    }
}
