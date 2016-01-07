using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain
{
    public interface IChangeFinder
    {
        List<ProjectInfo> FindProjectsToBuild(string rootDirectory, bool buildAll = false);
    }

    public class ChangeFinder : IChangeFinder
    {
        private readonly IMSBuildParser _buildParser;
        private readonly ILogger _logger;

        public ChangeFinder(IMSBuildParser buildParser, ILogger logger)
        {
            _buildParser = buildParser;
            _logger = logger;
        }

        public List<ProjectInfo> FindProjectsToBuild(string rootDirectory, bool buildAll = false)
        {
            var projects = FindProjects(rootDirectory, buildAll);
            _logger.WriteLine("Projects found:");
            projects.ForEach(p => _logger.WriteLine(p.Path));

            if (buildAll)
                return projects;
            else
            {
                List<ProjectInfo> allToBuild = new List<ProjectInfo>();
                allToBuild = projects.FindAll(p => HasFilesNewerThanAssembly(p)).ToList();
                int lastCount = -1;
                while (lastCount != allToBuild.Count)
                {
                    lastCount = allToBuild.Count;
                    bool added = false;
                    foreach (var toBuild in allToBuild)
                    {
                        foreach (var allProj in projects)
                        {
                            if (!allToBuild.Contains(allProj))
                            {
                                if (allProj.References.Contains(toBuild.AssemblyName))
                                {
                                    allToBuild.Add(allProj);
                                    added = true;
                                    break;
                                }
                            }
                        }
                        if (added)
                            break;
                    }
//                    foreach (var reference in pi.References)
//                    {
//                        string refToMatch = reference.EndsWith(".dll") ? reference.Replace(".dll", "") : reference;
//
//                        bool assemblyInOurTree = pis.Any(x => refToMatch.EndsWith(x.AssemblyName));
//                        bool assemblyAlreadyAccounted = accountedProjects.Any(x => refToMatch.EndsWith(x.AssemblyName));
//                        if (assemblyInOurTree && !assemblyAlreadyAccounted)
//                        {
//                            unreferenced = false;
//                            break;
//                        }
//                    }

                }
                return allToBuild;
            }
        }
        public bool HasFilesNewerThanAssembly(ProjectInfo projectInfo)
        {
            bool retVal = false;
            string assemblyName = projectInfo.OutputPath + projectInfo.AssemblyName + ".dll";
            var assemblyFileInfo = new FileInfo(assemblyName);

            _logger.WriteLine("");
            _logger.WriteLine("Checking assembly: " + assemblyName);
            if (assemblyFileInfo.Exists)
            {
                DateTime assemblyUpdate = assemblyFileInfo.LastWriteTime;
                foreach (string file in projectInfo.Files)
                {
                    if ((new FileInfo(file)).LastWriteTime > assemblyUpdate)
                    {
                        _logger.WriteLine("File changed: " + file);
                        retVal = true;
                        break;
                    }
                }
                if(!retVal)
                    _logger.WriteLine("No newer files found");
            }
            else
            {
                _logger.WriteLine("Assembly doesn't exist");                
            }
            _logger.WriteLine("");
            return retVal;
        }

        private List<ProjectInfo> FindProjects(string rootDirectory, bool buildAll)
        {
            List<ProjectInfo> retVal = new List<ProjectInfo>();

            List<string> paths = new List<string>();

            var excludedPaths = new List<string>(ConfigurationManager.AppSettings["excludedDirectories"].Split(';'));
            excludedPaths.RemoveAll(x => string.IsNullOrEmpty(x));
            FindAllProjects(rootDirectory, paths, excludedPaths);

            var excludedProjects = new List<string>(ConfigurationManager.AppSettings["excludedProjects"].Split(';'));
            excludedProjects.RemoveAll(x => string.IsNullOrEmpty(x));

            var excludedNamespaces = new List<string>(ConfigurationManager.AppSettings["excludedNamespaces"].Split(';'));
            excludedNamespaces.RemoveAll(x => string.IsNullOrEmpty(x));
            if (buildAll)
                excludedNamespaces.Clear();

            paths.ForEach(path =>
            {
                if (!excludedProjects.Any(x => path.EndsWith(x)) && !excludedNamespaces.Any(en => path.Contains(en)))
                    retVal.Add(_buildParser.CompileProjectInfo(path));
            });

            return retVal;
        }

        void FindAllProjects(string sDir, List<string> projs, List<string> excludePaths)
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
                _logger.WriteLine(excpt.Message);
            }
        }
    }
}