using MyGreater.Migration.Logic.DbEnvironments;
using MyGreater.Migration.Logic.EnvironmentVersion;
using MyGreater.Migration.Logic.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGreater.Migration.Logic
{
    public class MigrationRunner
    {
        #region Private Fields

        private Dictionary<string, DbEnvironment> _Environments;
        
        private readonly IScriptActionExecutor _ScriptAction;

        private readonly IScriptFetcher _ScriptFetcher;

        private readonly IScriptFormatter _ScriptFormatter;

        #endregion Private Fields

        #region Public Constructors

        public MigrationRunner(IEnvironmentFetcher environmentSource, IScriptFetcher scriptFetcher,
            IScriptFormatter scriptFormatter, IScriptActionExecutor scriptAction)
        {
            SetEnvironments(environmentSource);
            _ScriptFetcher = scriptFetcher;
            _ScriptFormatter = scriptFormatter;
            _ScriptAction = scriptAction;
        }

        #endregion Public Constructors

        #region Public Methods

        public string GetMigrationScript(Constants.TransactionPolicy transactionPolicy)
        {
            StringBuilder sb = new StringBuilder();
            foreach(DbEnvironment dbEnv in _Environments.Values.OrderBy(e => e.SortIndex))
            {
                sb.Append(GetMigrationScript(dbEnv, transactionPolicy));
            }
            return sb.ToString();
        }


        public string GetMigrationScript(DbEnvironment environment, Constants.TransactionPolicy transactionPolicy)
        {
            return GetMigrationScript(environment, _ScriptFetcher.GetMaxVersionId(environment.Name), transactionPolicy);
        }

        public string GetMigrationScript(DbEnvironment environment, int versionIdTo, Constants.TransactionPolicy transactionPolicy)
        {
            if (environment.VersionId > versionIdTo)
                throw new NotSupportedException("Executing scripts backwards in versionId's is not yet supported");

            var scripts = _ScriptFetcher.GetScripts(environment.Name, environment.VersionId + 1, versionIdTo);
            string scriptText = _ScriptFormatter.Amalgamate(scripts, transactionPolicy);
            return scriptText;
        }

        public void MigrateToVersion(DbEnvironment environment, Constants.TransactionPolicy transactionPolicy)
        {
            if (!_Environments.ContainsKey(environment.Name))
                throw new EnvironmentException($"The given environment : {environment.Name} was not in the set of valid environments : \n{string.Join("\n", _Environments.Keys)}");

            GetMigrationScript(environment, _ScriptFetcher.GetMaxVersionId(environment.Name), transactionPolicy);
        }

        #endregion Public Methods

        #region Private Methods

        private void SetEnvironments(IEnvironmentFetcher environmentSource)
        {
            var environments = environmentSource.GetEnvironments();
            //Ensure that all environment names and sort indexes are unique
            if (environments.GroupBy(env => env.Name).Any(grp => grp.Count() > 1))
                throw new EnvironmentException("Ensure that names for environments are unique");
            if (environments.GroupBy(env => env.SortIndex).Any(grp => grp.Count() > 1))
                throw new EnvironmentException("Ensure that sortindexes for environments are unique");

            _Environments = environments.ToDictionary(x => x.Name);
        }

        #endregion Private Methods
    }
}