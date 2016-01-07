using System.Collections.Generic;

namespace IncrementalDotNet.Domain.Contracts
{
    public class BuildInfo
    {
        public string BuildFileRelativeOffset { get; set; }
        public List<ProjectSet> ProjectSetsToBuild { get; set; }

        public BuildInfo(string buildFileRelativeOffset)
        {
            BuildFileRelativeOffset = buildFileRelativeOffset;
            ProjectSetsToBuild = new List<ProjectSet>();
        }

    }
}