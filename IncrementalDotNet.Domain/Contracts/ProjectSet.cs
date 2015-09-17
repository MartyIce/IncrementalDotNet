using System.Collections.Generic;

namespace IncrementalDotNet.Domain.Contracts
{
    public class ProjectSet
    {
        public ProjectSet(List<ProjectInfo> projectInfos, string name)
        {
            ProjectInfos = projectInfos;
            Name = name;
        }

        public List<ProjectInfo> ProjectInfos { get; set; }
        public string Name { get; set; }
        public ProjectSet DependsOn { get; set; }

        public string BuildKeyName
        {
            get { return Name.Replace(" ", "").Replace(".", "").Trim(); }
        }
    }
}