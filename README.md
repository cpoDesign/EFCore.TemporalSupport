# EFCore.TemporalSupport
An implementation of temporal support for EF Core.

## Build version

## Badges

|Badge type| Current status|
| --- | --- |
|**Build status**| [![Build status](https://cpodesign.visualstudio.com/PB/_apis/build/status/EFCoreTemporalSupport%20-%20Packaging)](https://cpodesign.visualstudio.com/PB/_build/latest?definitionId=28) |
|**NuGet**| [![nuget](https://img.shields.io/nuget/v/EFCoreTemporalSupport.svg)](https://www.nuget.org/packages/EFCoreTemporalSupport)|
|**Open Cover**| [![Coverage Status](https://coveralls.io/repos/github/cpoDesign/APIFramework/badge.svg?branch=master)](https://coveralls.io/github/cpoDesign/APIFramework?branch=master)|
|**DepShield Badge**|[![DepShield Badge](https://depshield.sonatype.org/badges/cpoDesign/APIFramework/depshield.svg)](https://depshield.github.io)

# Guide
 The following package supports the following methods:
 * AddAsTemporalTable
 * RemoveAsTemporalTable

These are used to add Temporal support to a specific table.
Recommendation is to add a new migration file for instance:
```
    public partial class <InsertYourDate>_addtemporalsupport : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: <SchemaName>);
            migrationBuilder.AddAsTemporalTable(<TableName>, <SchemaName>);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(name: <SchemaName>);
            migrationBuilder.RemoveAsTemporalTable(this.TargetModel.FindEntityType(typeof(<ModelType).Name), <SchemaName>);
        }
    }
```
