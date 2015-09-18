using System.Configuration;
using System.IO;
using IncrementalDotNet.Domain.Contracts;
using IncrementalDotNet.Domain.Templates;

namespace IncrementalDotNet.Domain
{
    public interface IBuildFileCreator
    {
        void WriteBuildFile(BuildInfo bi);
    }

    public class MSBuildFileCreator : IBuildFileCreator
    {
        public void WriteBuildFile(BuildInfo bi)
        {
            buildMasterXml bmx = new buildMasterXml(bi);
            string outputDir = ConfigurationManager.AppSettings["outputDir"];
            WriteFile(outputDir + @"autobuild.master.xml", bmx.TransformText());
            foreach (var ps in bi.ProjectSetsToBuild)
            {
                buildBatchTemplate bbt = new buildBatchTemplate(bi, ps);
                WriteFile(outputDir + @"autobuild." + ps.BuildKeyName + ".xml", bbt.TransformText());
            }
        }
        private static void WriteFile(string outputLocation, string classText, bool appendText = false)
        {
            using (StreamWriter sw = new StreamWriter(outputLocation, appendText))
            {
                sw.Write(classText);
            }
        }
    }
}