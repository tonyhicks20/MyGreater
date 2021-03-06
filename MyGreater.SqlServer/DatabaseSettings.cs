using MyGreater.Migration.Logic.DbEnvironments;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MyGreater.SqlServer
{
    public class DatabaseSettings
    {
        public DatabaseSettings(string delimitedString)
        {
            ConfigEnvironments = GetConfigEnvironments(delimitedString);
        }

        public string TableName { get; set; }
        public string VersionIdColumn { get; set; }
        public string EnvironmentNameColumn { get; set; }
        public int EnvironmentNameColumnSize { get; set; }
        public string ConnectionString { get; set; }
        public List<DbEnvironment> ConfigEnvironments { get; set; }

        /// <summary>
        /// These contain the SortIndex, not the VersionId
        /// </summary>
        /// <returns></returns>
        private List<DbEnvironment> GetConfigEnvironments(string delimited)
        {
            //Dev=3;QA=2;Prod=1
            return delimited.Split(';').Select(env =>
            {
                string[] kv = env.Split('=');
                return new DbEnvironment
                {
                    Name = kv[0],
                    SortIndex = Convert.ToInt32(kv[1])
                };
            }).ToList();
        }
    }
}