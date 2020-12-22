using System.ComponentModel;

namespace Nuke.Common.Tools.FluentMigrator
{
    public enum Processor
    {
        DB2,
        [Description("DB2 iSeries")]
        DB2iSeries,
        DotConnectOracle,
        DotConnectOracle12c,
        Firebird,
        Hana,
        MySql4,
        MySql5,
        Oracle,
        Oracle12c,
        Oracle12cManaged,
        OracleManaged,
        Postgres,
        Postgres92,
        PostgreSQL10_0,
        PostgreSQL11_0,
        Redshift,
        SqlAnywhere16,
        SQLite,
        SqlServer2000,
        SqlServer2005,
        SqlServer2008,
        SqlServer2012,
        SqlServer2014,
        SqlServer2016,
        SqlServerCe
    }
}