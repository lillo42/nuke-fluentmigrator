using System;
using System.ComponentModel;
using System.Reflection;
using Nuke.Common.Tooling;

namespace Nuke.Common.Tools.FluentMigrator
{
    public static  partial class FluentMigratorTask
    {
        public class FluentMigratorListMigrationsSetting : FluentMigratorSetting
        {
            protected override string Command => "list migrations";
        }
        
        #region Rollback
        public abstract class FluentMigratorRollbackSetting : FluentMigratorSetting
        {
            /// <summary>
            /// Overrides the transaction behavior of migrations, so that all migrations to be executed will run in one transaction. 
            /// </summary>
            public TransactionMode TransactionMode { get; internal set; } = TransactionMode.Migration;
            
            protected override Arguments ConfigureProcessArguments(Arguments arguments)
            {
                base.ConfigureProcessArguments(arguments);

                arguments
                    .Add("--transaction-mode", TransactionMode);
                
                return arguments;
            }
        }
        
        public class FluentMigratorRollbackAllSetting : FluentMigratorRollbackSetting
        {
            protected override string Command => "rollback all";
        }
        
        public class FluentMigratorRollbackBySetting : FluentMigratorRollbackSetting
        {
            protected override string Command => $"rollback by {Step}";
            
            /// <summary>
            /// The step
            /// </summary>
            public int Step { get; internal set; }
        }
        
        public class FluentMigratorRollbackToSetting : FluentMigratorRollbackSetting
        {
            protected override string Command => $"rollback to {Version}";

            /// <summary>
            /// The version 
            /// </summary>
            public long Version { get; internal set; }
        }
        #endregion

        #region Migrate

        public abstract class FluentMigratorMigrateSetting : FluentMigratorSetting
        {
            /// <summary>
            /// Overrides the transaction behavior of migrations, so that all migrations to be executed will run in one transaction. 
            /// </summary>
            public TransactionMode TransactionMode { get; internal set; } = TransactionMode.Migration;
            
            /// <summary>
            /// The specific version to migrate to (inclusive).
            /// </summary>
            public virtual long Target { get; internal set; }
            
            protected override Arguments ConfigureProcessArguments(Arguments arguments)
            {
                base.ConfigureProcessArguments(arguments);

                arguments
                    .Add("--transaction-mode", TransactionMode);
                return arguments;
            }
        }
        
        public class FluentMigratorMigrateUpSetting : FluentMigratorMigrateSetting
        {
            protected override string Command => "migrate up";

            /// <summary>
            /// The specific version to migrate to (inclusive).
            /// </summary>
            public override long Target { get; internal set; }

            protected override Arguments ConfigureProcessArguments(Arguments arguments)
            {
                base.ConfigureProcessArguments(arguments);

                arguments.Add("--target {value}", Target);
                return arguments;
            }
        }
        
        public class FluentMigratorMigrateDownSetting : FluentMigratorMigrateSetting
        {
            protected override string Command => "migrate down";
            
            /// <summary>
            /// The specific version to revert to (exclusive).
            /// </summary>
            public override long Target { get; internal set; }

            /// <summary>
            /// Whether comments should be stripped from SQL text prior to executing migration on server. Default is true; false will become the default in 4.x.
            /// </summary>
            public bool Strip { get; internal set; } = true;

            protected override Arguments ConfigureProcessArguments(Arguments arguments)
            {
                base.ConfigureProcessArguments(arguments);

                arguments
                    .Add("--target {value}", Target)
                    .Add("--strip {value}", Strip);
                return arguments;
            }
        }

        #endregion

        public abstract class FluentMigratorSetting : ToolSettings
        {
            /// <summary>
            ///   Path to the Fluent Migrator executable.
            /// </summary>
            public override string ProcessToolPath => base.ProcessToolPath ?? FluentMigratorPath;

            public override Action<OutputType, string> ProcessCustomLogger => FluentMigratorLogger;

            /// <summary>
            /// The connection string itself to the server and database you want to execute your migrations against.
            /// </summary>
            public string ConnectionString { get; internal set; }

            /// <summary>
            /// Indicates that migrations will be generated without consulting a target database.
            /// </summary>
            public bool NoConnectionString => string.IsNullOrEmpty(ConnectionString);

            /// <summary>
            /// The namespace contains the migrations you want to run. Default is all migrations found within the Target Assembly will be run.
            /// </summary>
            public string Namespace { get; internal set; }

            /// <summary>
            /// The assemblies containing the migrations you want to execute.
            /// </summary>
            public string Assembly { get; internal set; }

            /// <summary>
            /// Only output the SQL generated by the migration - do not execute it. Default is false.
            /// </summary>
            public bool Preview { get; internal set; }

            /// <summary>
            /// The profile to run after executing migrations.
            /// </summary>
            public string Profile { get; internal set; }

            /// <summary>
            /// Overrides the default database command timeout of 30 seconds.
            /// </summary>
            public int? Timeout { get; internal set; }

            /// <summary>
            /// Output generated SQL to a file. Default is no output. A filename may be specified, otherwise [targetAssemblyName].sql is the default.
            /// </summary>
            public string Output { get; internal set; }

            /// <summary>
            /// Whether migrations in nested namespaces should be included. Used in conjunction with the namespace option.
            /// </summary>
            public bool Nested { get; internal set; }
            
            /// <summary>
            /// The specific version to start migrating from. Only used when NoConnection is true. Default is 0.
            /// </summary>
            public int? StartVersions { get; internal set; }

            /// <summary>
            /// The directory to load SQL scripts specified by migrations from.
            /// </summary>
            public string WorkingDirectory { get; internal set; }
            
            /// <summary>
            /// Filters the migrations to be run by tag.
            /// </summary>
            public string Tag { get; internal set; }
            
            /// <summary>
            /// Allows execution of migrations marked as breaking changes.
            /// </summary>
            public bool AllowBreakingChanges { get; internal set; }

            /// <summary>
            /// The kind of database you are migrating against.
            /// </summary>
            public Processor Processor { get; internal set; } = Processor.Postgres;
            
            /// <summary>
            ///   Show verbose output.
            /// </summary>
            public bool? Verbose { get; internal set; }
            
            protected abstract string Command { get; }

            protected override Arguments ConfigureProcessArguments(Arguments arguments)
            {
                var processor = Processor.ToString();
                if (Processor.GetType().GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    processor = attribute.Description;
                }
                
                arguments
                    .Add($"fm {Command}")
                    .Add("--connection {value}", ConnectionString)
                    .Add("--no-connection", NoConnectionString)
                    .Add("--preview", Preview)
                    .Add("--profile {value}", Profile)
                    .Add("--verbose", Verbose)
                    .Add("--timeout {value}", Timeout)
                    .Add("--output {value}", Output)
                    .Add("--assembly {value}", Assembly)
                    .Add("--namespace {value}", Namespace)
                    .Add("--nested", Nested)
                    .Add("--working-directory {value}", WorkingDirectory)
                    .Add("--tag {value}", Tag)
                    .Add("--allow-breaking-changes", AllowBreakingChanges)
                    .Add("--processor {value}", processor);

                
                if (StartVersions.HasValue && NoConnectionString)
                {
                    arguments.Add("--start-version {value}", StartVersions);
                }
                
                return arguments;
            }
        }
    }
}