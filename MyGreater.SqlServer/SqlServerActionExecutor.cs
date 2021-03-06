using MyGreater.Migration.Logic.Scripts;
using System.Data.SqlClient;
using Dapper;

namespace MyGreater.SqlServer
{
    public class SqlServerActionExecutor : IScriptActionExecutor
    {
        private DatabaseSettings _DbSettings;

        public SqlServerActionExecutor(DatabaseSettings dbSettings)
        {
            _DbSettings = dbSettings;
        }

        public bool Action(string script)
        {
            using (SqlConnection connection = new SqlConnection(_DbSettings.ConnectionString))
            {
                connection.Open();
                connection.Execute(script);
            }
            return true;
        }
    }
}