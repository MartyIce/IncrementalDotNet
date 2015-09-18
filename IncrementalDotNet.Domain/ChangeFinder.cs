using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain
{
    public interface IChangeFinder
    {
        List<ProjectInfo> FindProjectsWithRecentChanges(string rootDirectory);
    }

    public class ChangeFinder : IChangeFinder
    {
        private readonly IMSBuildParser _buildParser;

        public ChangeFinder(IMSBuildParser buildParser)
        {
            _buildParser = buildParser;
        }

        public List<ProjectInfo> FindProjectsWithRecentChanges(string rootDirectory)
        {
            var projects = FindProjects(rootDirectory);
            projects.ForEach(p => Console.WriteLine(p.Path));

            return projects.FindAll(p => p.HasFilesNewerThanAssembly()).ToList();
        }

        private List<ProjectInfo> FindProjects(string rootDirectory)
        {
            List<ProjectInfo> retVal = new List<ProjectInfo>();

            List<string> paths = new List<string>();

            var excludedPaths = new List<string>(ConfigurationManager.AppSettings["excludedDirectories"].Split(';'));
            FindAllProjects(rootDirectory, paths, excludedPaths);

            var excludedProjects = new List<string>(ConfigurationManager.AppSettings["excludedProjects"].Split(';'));
            paths.ForEach(path =>
            {
                if (!excludedProjects.Any(x => path.EndsWith(x)))
                    retVal.Add(_buildParser.CompileProjectInfo(path));
            });

            return retVal;
        }

        static void FindAllProjects(string sDir, List<string> projs, List<string> excludePaths)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    if (!excludePaths.Any(x => d.ToLower().Contains(x.ToLower())))
                    {
                        foreach (string f in Directory.GetFiles(d, "*.csproj"))
                        {
                            projs.Add(f);
                        }
                        FindAllProjects(d, projs, excludePaths);
                    }
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }
    }
}