using System;
using System.Collections.Generic;
using System.IO;

namespace IncrementalDotNet.Domain.Contracts
{
    
    public class ProjectInfo
    {
        public ProjectInfo(string path)
        {
            Path = path;
            References = new List<string>();
            Files = new List<string>();
        }

        public string Path { get; set; }
        public List<string> References { get; set; }
        public string AssemblyName { get; set; }
        public List<string> Files { get; set; }
        public string OutputPath { get; set; }

        public string BuildKeyName
        {
            get { return AssemblyName.Replace(" ", "").Replace(".", "").Trim(); }
        }

        public bool HasFilesNewerThanAssembly()
        {
            var assemblyFileInfo = new FileInfo(OutputPath + AssemblyName + ".dll");
            DateTime assemblyUpdate = assemblyFileInfo.LastWriteTime;
            bool retVal = false;
            foreach (string file in Files)
            {
                if ((new FileInfo(file)).LastWriteTime > assemblyUpdate)
                {
                    retVal = true;
                    break;
                }
            }
            return retVal;
        }
    }
}