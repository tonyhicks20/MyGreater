namespace MyGreater.Migration.Logic.Scripts
{
    public interface IScriptCreator
    {
        /// <summary>
        /// Creates the script that ends the transaction
        /// </summary>
        /// <returns></returns>
        string EndTransactionScript();

        /// <summary>
        /// Creates the script that ends the transaction
        /// </summary>
        /// <returns></returns>
        string FormatScript(string scriptText);

        /// <summary>
        /// This "environment" variable will allow the script writer to generate conditional code based on the environment.
        /// </summary>
        /// <returns></returns>
        string GetEnvironmentVariable(string environmentName);

        /// <summary>
        /// Allows any calling application to split up the amalgamated script file
        /// </summary>
        /// <returns></returns>
        string GetScriptDelimeter();

        /// <summary>
        /// Creates the script to start a transaction
        /// </summary>
        /// <returns></returns>
        string StartTransactionScript();

        /// <summary>
        /// Creates the script to updates the version on the database
        /// </summary>
        /// <param name="versionId"></param>
        string UpdateVersionNumber(int versionId, string environmentName);
    }
}