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
                    else if (FindMatch(line, "<OutputPath>", ref match))
                    {
                        string matcher = "..\\";
                        string outputPath = match.Replace("</OutputPath>", "").Trim();
                        if (outputPath.StartsWith("..") && !outputPath.Replace(matcher, "").Contains(matcher))
                            outputPath = matcher + outputPath;
                        if (outputPath.StartsWith(".."))
                            outputPath = Path.GetFullPath(fi.Directory.FullName + outputPath);
                        pi.OutputPath = outputPath;
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
 
    }
}