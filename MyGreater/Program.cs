using MyGreater.FolderScriptFetcher;
using MyGreater.Migration.Logic;
using MyGreater.Migration.Logic.Scripts;
using MyGreater.SqlServer;
using System;
using System.Configuration;
using System.IO;

namespace MyGreater
{
#pragma warning disable S1118 // Utility classes should not have public constructors
    internal class Program
#pragma warning restore S1118 // Utility classes should not have public constructors
    {
        private static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MigrationDestination"].ConnectionString;
            if(string.IsNullOrWhiteSpace(connectionString))
                throw new ConfigurationErrorsException("Please specify a value for the connectionstring : MigrationDestination");

            string tableName = GetApplicationSetting("VersionTableName");
            string scriptSourceFolder = GetApplicationSetting("ScriptSourceFolder");
            string scriptDestination = GetApplicationSetting("MigrationScriptFile");
            string environmentOrders = GetApplicationSetting("EnvironmentOrders");
            string currentEnvironment = GetApplicationSetting("CurrentEnvironment");


            DatabaseSettings dbSettings = new DatabaseSettings(environmentOrders)
            {
                ConnectionString = connectionString,
                TableName = tableName,
                EnvironmentNameColumn = "[EnvironmentName]",
                EnvironmentNameColumnSize = 50,
                VersionIdColumn = "[VersionId]"
            };


            var envFetcher = new SqlServerEnvironmentFetcher(dbSettings);
            var scriptCreator = new SqlServerScriptCreator(dbSettings);
            var scriptActionExecutor = new SqlServerActionExecutor(dbSettings);
            var scriptFetcher = new FolderConventionScriptFetcher(scriptSourceFolder);
            var scriptFormatter = new ScriptFormatter(scriptCreator, currentEnvironment);

            MigrationRunner runner = new MigrationRunner(envFetcher, scriptFetcher, scriptFormatter, scriptActionExecutor);
            string migrationScript = runner.GetMigrationScript(Constants.TransactionPolicy.PerVersion);
            File.WriteAllText(scriptDestination, migrationScript);
        }

        private static string GetApplicationSetting(string appSettingName)
        {
            string appSetting = ConfigurationManager.AppSettings.Get(appSettingName);
            if (appSetting == null)
                throw new ConfigurationErrorsException("Please specify a value for the appSetting : " + appSettingName);
            return ConfigurationManager.AppSettings.Get(appSettingName);
        }
    }
}