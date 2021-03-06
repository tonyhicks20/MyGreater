using MyGreater.FolderScriptFetcher;
using MyGreater.Migration.Logic;
using MyGreater.Migration.Logic.Scripts;
using MyGreater.SqlServer;
using System;
using System.IO;

namespace MyGreater.NetCore.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            DatabaseSettings dbSettings = new DatabaseSettings("Dev=3;QA=2;Prod=1")
            {
                ConnectionString = "Server=localhost;Database=Dev;Trusted_Connection=True;",
                EnvironmentNameColumn = "[EnvironmentName]",
                EnvironmentNameColumnSize = 50,
                TableName = "[EnvironmentVersion]",
                VersionIdColumn = "[VersionId]"
            };
            var envFetcher = new SqlServerEnvironmentFetcher(dbSettings);
            var scriptCreator = new SqlServerScriptCreator(dbSettings);
            var scriptActionExecutor = new SqlServerActionExecutor(dbSettings);
            var scriptFetcher = new FolderConventionScriptFetcher(@"C:\Users\tony\Desktop\MigrationScripts");
            var scriptFormatter = new ScriptFormatter(scriptCreator);

            MigrationRunner runner = new MigrationRunner(envFetcher, scriptFetcher, scriptFormatter, scriptActionExecutor);
            string migrationScript = runner.GetMigrationScript(Constants.TransactionPolicy.PerVersion);
            File.WriteAllText(@"C:\Users\tony\Desktop\Output\Script.sql", migrationScript);
        }
    }
}
