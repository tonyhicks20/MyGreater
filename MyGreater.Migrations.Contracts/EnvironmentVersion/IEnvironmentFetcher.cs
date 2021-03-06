using System.Collections.Generic;

namespace MyGreater.Migration.Logic.DbEnvironments
{
    public interface IEnvironmentFetcher
    {
        List<DbEnvironment> GetEnvironments();
    }

    public class DbEnvironment
    {
        public string Name { get; set; }
        public int SortIndex { get; set; }
        public int VersionId { get; set; }
    }
}