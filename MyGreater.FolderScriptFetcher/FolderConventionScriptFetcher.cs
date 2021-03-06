using MyGreater.Migration.Logic.Scripts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyGreater.FolderScriptFetcher
{
    public class FolderConventionScriptFetcher : IScriptFetcher
    {
        #region Private Fields

        private List<Script> _AllScripts;
        private readonly string _BaseDirectoryName;

        #endregion Private Fields

        #region Public Constructors

        public FolderConventionScriptFetcher(string baseDirectoryName)
        {
            _BaseDirectoryName = baseDirectoryName;
        }

        #endregion Public Constructors

        #region Public Methods

        public int GetMaxVersionId(string environment)
        {
            CheckCreateScripts();
            var environmentScripts = _AllScripts.Where(e => e.EnvironmentName.Equals(environment));
            return environmentScripts.Any() ? environmentScripts.Max(e => e.VersionId) : 0;
        }

        public List<Script> GetScripts(string environmentName, int versionIdFrom, int versionIdTo)
        {
            CheckCreateScripts();
            return _AllScripts.Where
                (e =>
                     e.EnvironmentName == environmentName &&
                     e.VersionId >= versionIdFrom &&
                     e.VersionId <= versionIdTo
                ).ToList();
        }

        #endregion Public Methods

        #region Private Methods

        private void CheckCreateScripts()
        {
            if (_AllScripts == null)
                _AllScripts = GetScriptsFromDirectory(_BaseDirectoryName);
        }

        private List<Script> GetScriptsFromDirectory(string baseFolderName)
        {
            string[] folderNames = Directory.GetDirectories(baseFolderName);
            List<Script> scripts = new List<Script>();
            foreach (string folderName in folderNames)
            {
                foreach (string fileName in Directory.GetFiles(folderName))
                {
                    scripts.Add
                    (
                        new Script
                        {
                            EnvironmentName = Path.GetFileName(folderName),
                            VersionId = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileName).Split('-')[0].Trim()),
                            Text = File.ReadAllText(fileName)
                        }
                    );
                }
            }
            return scripts;
        }

        #endregion Private Methods
    }
}