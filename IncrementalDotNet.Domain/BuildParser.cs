using System.IO;
using System.Text.RegularExpressions;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain
{
    public interface IMSBuildParser
    {
        ProjectInfo CompileProjectInfo(string path);
    }

    public class MSBuildParser : IMSBuildParser
    {
        public ProjectInfo CompileProjectInfo(string path)
        {
            ProjectInfo pi = new ProjectInfo(path);
            FileInfo fi = new FileInfo(path);
            bool inDebug = false;
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
                    else if (FindMatch(line, "<PropertyGroup", ref match))
                    {
                        inDebug = line.Contains("'Debug|AnyCPU'");
                    }
                    else if (FindMatch(line, "<HintPath>", ref match))
                    {
                        string filePath = match.Replace("</HintPath>", "").Trim();
                        if (filePath.StartsWith(".."))
                            filePath = Path.GetFullPath(fi.Directory.FullName + filePath);
                        pi.References.Add(filePath);
                    }
                    else if (inDebug && FindMatch(line, "<OutputPath>", ref match))
                    {
                        string matcher = "..\\";
                        string outputPath = match.Replace("</OutputPath>", "").Trim();
                        if (outputPath.StartsWith("..") && !outputPath.Replace(matcher, "").Contains(matcher))
                            outputPath = matcher + outputPath;
                        if (outputPath.StartsWith(".."))
                            outputPath = Path.GetFullPath(fi.Directory.FullName + outputPath);
                        else if(!outputPath.Contains(":"))
                            outputPath = Path.GetFullPath(fi.Directory.FullName + "\\" + outputPath);
                        pi.OutputPath = outputPath;
                    }
                    else if (!IsCommented(match))
                    {
                        if (FindMatch(line, "<Compile Include=\"", ref match))
                        {
                            if (!IsCommented(match))
                                AddIncludedFile(match, fi, pi);
                        }
                        else if (FindMatch(line, "<Reference Include=\"", ref match))
                        {
                            if (!IsCommented(match))
                            {
                                string includedFile = match;
                                if (includedFile.Contains(","))
                                    includedFile = includedFile.Substring(0, match.IndexOf(","));
                                else
                                {
                                    includedFile = FindAndReplace(match, "\" />");
                                    includedFile = FindAndReplace(includedFile, "\">");
                                }
                                pi.References.Add(includedFile);
                            }
                        }
                        else if (FindMatch(line, "<ProjectReference Include=\"", ref match))
                        {
                            if (!IsCommented(match))
                            {
                                string includedFile = match;
                                includedFile = FindAndReplace(match, "\" />");
                                includedFile = FindAndReplace(includedFile, "\">");
                                var projectFile =
                                    Path.GetFullPath(Path.Combine(path.Substring(0, path.LastIndexOf("\\") + 1),
                                        includedFile));

                                string projAssembly = null;
                                using (StreamReader psr = new StreamReader(projectFile))
                                {
                                    while (!psr.EndOfStream)
                                    {
                                        string projLine = psr.ReadLine();
                                        if (FindMatch(projLine, "<AssemblyName>", ref projAssembly))
                                        {
                                            projAssembly = projAssembly.Replace("</AssemblyName>", "").Trim();
                                            break;
                                        }
                                    }
                                }
                                if (projAssembly != null)
                                    pi.References.Add(projAssembly);
                            }
                        }
                        else if (FindMatch(line, "<None Include=\"", ref match))
                        {
                            if (!IsCommented(match))
                                AddIncludedFile(match, fi, pi);
                        }
                        else if (FindMatch(line, "<EmbeddedResource Include=\"", ref match))
                        {
                            if (!IsCommented(match))
                                AddIncludedFile(match, fi, pi);
                        }
                    }
                }
            }

            return pi;
        }

        private void AddIncludedFile(string match, FileInfo fi, ProjectInfo pi)
        {
            string includedFile = FindAndReplace(match, "\" />");
            includedFile = FindAndReplace(includedFile, "\">");
            if (!includedFile.StartsWith(".\\"))
                includedFile = ".\\" + includedFile;
            includedFile = Path.GetFullPath(fi.Directory.FullName + includedFile);
            pi.Files.Add(includedFile);
        }

        private static bool IsCommented(string match)
        {
            return match != null && match.Contains("<!--");
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
 
    }
}