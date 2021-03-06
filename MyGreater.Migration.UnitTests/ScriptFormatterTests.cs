using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyGreater.Migration.Logic.Scripts;

namespace MyGreater.Migration.UnitTests
{
    [TestClass]
    public class ScriptFormatterTests
    {
        [TestMethod]
        public void ShouldCreateTransactions()
        {
            var mock = new Mock<IScriptCreator>();
            mock.Setup(x => x.StartTransactionScript()).Returns("StartOfTransaction~");
            mock.Setup(x => x.EndTransactionScript()).Returns("~EndOfTransaction");
            mock.Setup(x => x.FormatScript(It.IsAny<string>())).Returns<string>(parm => $"#{parm}#");
            var formatter = new ScriptFormatter(mock.Object, "Test");
            string scriptText = "ScriptText";

            string formattedScript = formatter.FormatScript(scriptText, 1, "", Logic.Constants.TransactionPolicy.PerVersion);
            Assert.AreEqual($"StartOfTransaction~#{scriptText}#~EndOfTransaction", formattedScript);
        }

        [TestMethod]
        public void ShouldNotCreateTransactions()
        {
            var mock = new Mock<IScriptCreator>();
            mock.Setup(x => x.StartTransactionScript()).Returns("StartOfTransaction~");
            mock.Setup(x => x.EndTransactionScript()).Returns("~EndOfTransaction");
            mock.Setup(x => x.FormatScript(It.IsAny<string>())).Returns<string>(parm => $"#{parm}#");
            var formatter = new ScriptFormatter(mock.Object,"Test");
            string scriptText = "ScriptText";

            string formattedScript = formatter.FormatScript(scriptText, 1, "", Logic.Constants.TransactionPolicy.EntireMigration);
            Assert.AreEqual($"#{scriptText}#", formattedScript);
        }

        [TestMethod]
        public void ShouldFormatScriptCorrectly()
        {
            var mock = new Mock<IScriptCreator>();
            mock.Setup(x => x.StartTransactionScript()).Returns("StartOfTransaction~");
            mock.Setup(x => x.EndTransactionScript()).Returns("~EndOfTransaction;");
            mock.Setup(x => x.FormatScript(It.IsAny<string>())).Returns<string>(scriptTxt => $"#{scriptTxt}#");
            mock.Setup(x => x.GetEnvironmentVariable(It.IsAny<string>())).Returns<string>(envName => "EnvironmentVariable=" + envName + ";");
            mock.Setup(x => x.UpdateVersionNumber(It.IsAny<int>(),It.IsAny<string>())).Returns<int,string>((versionId,envId) => "VersionId=" + versionId);
            mock.Setup(x => x.GetScriptDelimeter()).Returns("!!");
            var formatter = new ScriptFormatter(mock.Object,"Test");

            string formattedScript = formatter.FormatScript("ScriptText1", 1, "Dev", Logic.Constants.TransactionPolicy.PerVersion);
            Assert.AreEqual($"StartOfTransaction~#EnvironmentVariable=Test;ScriptText1#VersionId=1~EndOfTransaction;!!", formattedScript);

            formattedScript = formatter.FormatScript("SomeOtherScriptText", 2, "QA", Logic.Constants.TransactionPolicy.PerVersion);
            Assert.AreEqual($"StartOfTransaction~#EnvironmentVariable=Test;SomeOtherScriptText#VersionId=2~EndOfTransaction;!!", formattedScript);
        }
    }
}