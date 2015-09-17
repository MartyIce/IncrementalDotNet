using System;
using System.Collections.Generic;
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
        public List<ProjectInfo> FindProjectsWithRecentChanges(string rootDirectory)
        {
            List<ProjectInfo> retVal = new List<ProjectInfo>();

            List<string> paths = new List<string>();

            List<string> excludePaths = new List<string>()
            {
                "\\UI\\Html",
                "\\Olap",
                "\\StressTesting",
                "\\Windows",
                "\\Test\\Client",
            };

            FindAllProjects(rootDirectory, paths, excludePaths);

            List<string> excludeList = new List<string>() 
            {
                "DocumentationRoot.csproj",
                "Documentation.csproj",
            };

            paths.ForEach(path =>
            {
                if (!excludeList.Any(x => path.EndsWith(x)))
                    retVal.Add(CompileProjectInfo(path));
            });

            return retVal;
        }

        private ProjectInfo CompileProjectInfo(string path)
        {
            ProjectInfo pi = new ProjectInfo(path);
            FileInfo fi = new FileInfo(path);
            using (StreamReader sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    string match = null;
                    if (FindMatch(line, "<AssemblyName>", ref match))
                    {
                        string assemblyName = match.Replace("</AssemblyName>", "").Trim();
                        pi.AssemblyName = assemblyName;
                    }
                    else if (FindMatch(line, "<HintPath>", ref match))
                    {
                        string filePath = match.Replace("</HintPath>", "").Trim();
                        if (filePath.StartsWith(".."))
                            filePath = Path.GetFullPath(fi.Directory.FullName + filePath);
                        pi.References.Add(filePath);
                    }
                    else if (FindMatch(line, "<Compile Include=\"", ref match))
                    {
                        if (!match.Contains("<!--"))
                        {
                            string includedFile = FindAndReplace(match, "\" />");
                            includedFile = FindAndReplace(includedFile, "\">");
                            if (!includedFile.StartsWith(".\\"))
                                includedFile = ".\\" + includedFile;
                            includedFile = Path.GetFullPath(fi.Directory.FullName + includedFile);
                            pi.Files.Add(includedFile);
                        }
                    }
                    else if (FindMatch(line, "<Reference Include=\"", ref match))
                    {
                        if (!match.Contains("<!--"))
                        {

                            string includedFile = match;
                            if(includedFile.Contains(","))
                                includedFile = includedFile.Substring(0, match.IndexOf(","));
                            else
                            {
                                includedFile = FindAndReplace(match, "\" />");
                                includedFile = FindAndReplace(includedFile, "\">");
                            }
                            pi.References.Add(includedFile);
                        }
                    }
                }
            }

            return pi;
        }

        private string FindAndReplace(string line, string match)
        {
            if (line.Contains(match))
                return line.Replace(match, "").Trim();
            else
                return line;
        }
        private bool FindMatch(string line, string match, ref string retVal)
        {
            if (line.Contains(match))
            {
                retVal = line.Replace(match, "").Trim();
                return true;
            }
            else
                return false;
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
                    else
                    {
                        Console.WriteLine("Skipping: " + d);
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