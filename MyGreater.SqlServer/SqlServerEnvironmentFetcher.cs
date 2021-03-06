using Dapper;
using MyGreater.Migration.Logic.DbEnvironments;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace MyGreater.SqlServer
{
    public class SqlServerEnvironmentFetcher : IEnvironmentFetcher
    {
        #region Private Fields

        private readonly DatabaseSettings _DbSettings;

        #endregion Private Fields

        #region Public Constructors

        public SqlServerEnvironmentFetcher(DatabaseSettings dbSettings)
        {
            _DbSettings = dbSettings;
        }

        #endregion Public Constructors

        #region Public Methods

        public List<DbEnvironment> GetEnvironments()
        {
            var dbEnvironments = GetDatabaseEnvironments();
            var configEnvironments = _DbSettings.ConfigEnvironments;

            var joinResults = from configEnv in configEnvironments
                              join dbEnv in dbEnvironments on configEnv.Name equals dbEnv.Name into tmp
                              from leftJoinResult in tmp.DefaultIfEmpty()
                              select new { ConfigEnvironment = configEnv, DatabaseEnvironment = leftJoinResult };

            foreach (var environmentHolder in joinResults)
            {
                if (environmentHolder.DatabaseEnvironment == null)
                    InsertEnvironment(environmentHolder.ConfigEnvironment);
                else
                    environmentHolder.ConfigEnvironment.VersionId = environmentHolder.DatabaseEnvironment.VersionId;
            }

            return joinResults.Select(tmp => tmp.ConfigEnvironment).ToList();
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckCreateTable()
        {
            using (SqlConnection connection = GetConnection())
            {
                string schemaName = "dbo";
                string tableName = "";
                string[] dbNameSplit = _DbSettings.TableName.Split('.');

                if (dbNameSplit.Length == 2)
                {
                    schemaName = dbNameSplit[0];
                    tableName = dbNameSplit[1];
                }
                else
                    tableName = _DbSettings.TableName;


                schemaName = schemaName.Replace("[", "").Replace("]", "");
                tableName = tableName.Replace("[", "").Replace("]", "");

                try
                {
                    connection.Execute($"CREATE SCHEMA {schemaName}");
                }
                catch
                {
                    //Schema will have been created already then
                }


                string sql = $@"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = N'{schemaName}' AND TABLE_NAME = N'{tableName}')
BEGIN
    CREATE TABLE {_DbSettings.TableName} ({_DbSettings.VersionIdColumn} INT,{_DbSettings.EnvironmentNameColumn} VARCHAR({_DbSettings.EnvironmentNameColumnSize}), PRIMARY KEY ({_DbSettings.EnvironmentNameColumn}))
END";

                connection.Execute(sql);
            }
        }

        private SqlConnection GetConnection()
        {
            var conn = new SqlConnection(_DbSettings.ConnectionString);
            conn.Open();
            return conn;
        }

        private List<DbEnvironment> GetDatabaseEnvironments()
        {
            CheckCreateTable();
            using (SqlConnection connection = GetConnection())
            {
                string vCol = $"{_DbSettings.VersionIdColumn} AS {nameof(DbEnvironment.VersionId)}";
                string eCol = $"{_DbSettings.EnvironmentNameColumn} AS {nameof(DbEnvironment.Name)}";
                var environments = connection.Query<DbEnvironment>($"SELECT {vCol}, {eCol} FROM {_DbSettings.TableName}");
                return environments.ToList();
            }
        }

        private void InsertEnvironment(DbEnvironment environment)
        {
            using (SqlConnection connection = GetConnection())
            {
                string st = $"INSERT INTO {_DbSettings.TableName} ({_DbSettings.EnvironmentNameColumn},{_DbSettings.VersionIdColumn})";
                st += $" VALUES ('{environment.Name}',{environment.VersionId})";
                connection.Execute(st);
            }
        }

        #endregion Private Methods
    }
}