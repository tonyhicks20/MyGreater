using System.Collections.Generic;

namespace MyGreater.Migration.Logic.Scripts
{
    public interface IScriptFetcher
    {
        List<Script> GetScripts(string environmentName, int versionIdFrom, int versionIdTo);

        int GetMaxVersionId(string environment);
    }
}