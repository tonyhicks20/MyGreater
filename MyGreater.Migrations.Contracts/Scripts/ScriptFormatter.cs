using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyGreater.Migration.Logic.Scripts
{
    public class ScriptFormatter : IScriptFormatter
    {
        private readonly IScriptCreator _ScriptCreator;
        private readonly string _DestinationEnvironment;

        public ScriptFormatter(IScriptCreator scriptCreator, string destinationEnvironment)
        {
            _ScriptCreator = scriptCreator;
            _DestinationEnvironment = destinationEnvironment;
        }

        public string Amalgamate(List<Script> scriptsToRun, Constants.TransactionPolicy transactionPolicy, bool orderByDesc = false)
        {
            StringBuilder _ScriptText = new StringBuilder();
            if(scriptsToRun.Any())
                _ScriptText.AppendLine(_ScriptCreator.GetEnvironmentVariable(_DestinationEnvironment));

            if (transactionPolicy == Constants.TransactionPolicy.EntireMigration)
                _ScriptText.Append(_ScriptCreator.StartTransactionScript());

            var orderedScripts = orderByDesc ?
                    scriptsToRun.OrderByDescending(s => s.VersionId) :
                    scriptsToRun.OrderBy(s => s.VersionId);

            foreach (Script script in orderedScripts)
            {
                _ScriptText.Append(FormatScript(script.Text, script.VersionId, script.EnvironmentName, transactionPolicy));
            }

            if (transactionPolicy == Constants.TransactionPolicy.EntireMigration)
            {
                _ScriptText.Append(_ScriptCreator.EndTransactionScript());
                _ScriptText.Append(_ScriptCreator.GetScriptDelimeter());
            }

            return _ScriptText.ToString();
        }

        public string FormatScript(string scriptText, int versionId, string scriptEnvironment,Constants.TransactionPolicy transactionPolicy)
        {
            //Add the environment variable before custom formatting the script
            string editedScript = _ScriptCreator.FormatScript(scriptText);

            StringBuilder final = new StringBuilder();

            if (transactionPolicy == Constants.TransactionPolicy.PerVersion)
                final.Append(_ScriptCreator.StartTransactionScript());

            //Add the formatted portion within the transaction
            final.Append(editedScript);
            final.Append(_ScriptCreator.UpdateVersionNumber(versionId,scriptEnvironment));

            if (transactionPolicy == Constants.TransactionPolicy.PerVersion)
            {
                final.Append(_ScriptCreator.EndTransactionScript());
                final.Append(_ScriptCreator.GetScriptDelimeter());
            }

            return final.ToString();
        }
    }
}