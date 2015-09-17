using System.Collections.Generic;

namespace IncrementalDotNet.Domain.Contracts
{
    public class BuildInfo
    {
        public BuildInfo()
        {
            ProjectSetsToBuild = new List<ProjectSet>();
        }

        public List<ProjectSet> ProjectSetsToBuild { get; set; }
    }
}