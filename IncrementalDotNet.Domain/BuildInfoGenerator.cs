using System.Collections.Generic;
using System.Linq;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain
{
    public interface IBuildInfoGenerator
    {
        BuildInfo GenerateBuild(List<ProjectInfo> pis);
    }

    public class BuildInfoGenerator : IBuildInfoGenerator
    {
        public BuildInfo GenerateBuild(List<ProjectInfo> pis)
        {
            BuildInfo bi = new BuildInfo();
            List<ProjectInfo> accountedProjects = new List<ProjectInfo>();

            // First find projects with no references in anything else in this list.
            int counter = 0;
            ProjectSet lastPs = null;
            while (accountedProjects.Count != pis.Count)
            {
                var nextBatch = FindNextBatch(pis, accountedProjects);
                var newPs = new ProjectSet(nextBatch, string.Format("Batch {0}", counter++));
                if (lastPs != null)
                    newPs.DependsOn = lastPs;
                lastPs = newPs;
                bi.ProjectSetsToBuild.Add(newPs);
                accountedProjects.AddRange(nextBatch);

            }

            return bi;
        }

        private List<ProjectInfo> FindNextBatch(List<ProjectInfo> pis, List<ProjectInfo> accountedProjects)
        {
            List<ProjectInfo> retVal = new List<ProjectInfo>();

            foreach (var pi in pis)
            {
                if (!accountedProjects.Contains(pi))
                {
                    bool unreferenced = true;
                    foreach (var reference in pi.References)
                    {
                        string refToMatch = reference.EndsWith(".dll") ? reference.Replace(".dll", "") : reference;

                        if (pis.Any(x => refToMatch.EndsWith(x.AssemblyName)) &&
                            !accountedProjects.Any(x => refToMatch.EndsWith(x.AssemblyName)))
                        {
                            unreferenced = false;
                            break;
                        }
                    }
                    if (unreferenced)
                        retVal.Add(pi);
                }                
            }


            return retVal;
        }
    }
}