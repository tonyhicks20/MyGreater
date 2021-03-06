using System.Collections.Generic;

namespace MyGreater.Migration.Logic.Scripts
{
    public interface IScriptFormatter
    {
        string Amalgamate(List<Script> scriptsToRun, Constants.TransactionPolicy transactionPolicy, bool orderByDesc = false);

        string FormatScript(string scriptText, int versionId, string scriptEnvironment,Constants.TransactionPolicy transactionPolicy);
    }
}