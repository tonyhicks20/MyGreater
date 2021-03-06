using MyGreater.Migration.Logic.Scripts;
using System;

namespace MyGreater.SqlServer
{
    public class SqlServerScriptCreator : IScriptCreator
    {
        private DatabaseSettings _DbSettings;

        public SqlServerScriptCreator(DatabaseSettings dbSettings)
        {
            _DbSettings = dbSettings;
        }

        public string EndTransactionScript()
        {
            return "";
//@"
//    COMMIT;
//END TRY
//BEGIN CATCH
//    ROLLBACK;
//    THROW;
//END CATCH
//";
        }

        public string FormatScript(string scriptText)
        {
            return scriptText;// Environment.NewLine + $"exec sp_executesql N'{scriptText.Replace("'", "''")}';";
        }

        public string GetEnvironmentVariable(string environmentName)
        {
            return Environment.NewLine + 
            $":setvar EnvironmentName \"{environmentName}\"" + Environment.NewLine +
            ":setvar __IsSqlCmdEnabled \"True\" " + Environment.NewLine +
                "GO " + Environment.NewLine +
            " IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True' " + Environment.NewLine +
            " BEGIN " + Environment.NewLine +
            " PRINT N'SQLCMD mode must be enabled to successfully execute this script.'; " + Environment.NewLine +
            " SET NOEXEC ON; " + Environment.NewLine +
            " END " + Environment.NewLine +
            " ELSE " + Environment.NewLine +
            "   SET NOEXEC OFF;" + Environment.NewLine +
            " GO ";
        }

        public string GetScriptDelimeter()
        {
            return Environment.NewLine + "--~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#~#" + Environment.NewLine;
        }

        public string StartTransactionScript()
        {
            return "";// "\nBEGIN TRY\nBEGIN TRANSACTION\n";
        }

        public string UpdateVersionNumber(int versionId, string environmentName)
        {
            return Environment.NewLine + $"UPDATE {_DbSettings.TableName} SET VersionId = {versionId} WHERE EnvironmentName = '{environmentName}'";
        }
    }
}