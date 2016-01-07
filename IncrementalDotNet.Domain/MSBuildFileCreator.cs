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
        private ILogger _logger;

        public MSBuildFileCreator(ILogger logger)
        {
            _logger = logger;
        }

        public void WriteBuildFile(BuildInfo bi)
        {
            buildMasterXml bmx = new buildMasterXml(bi);
            string outputDir = ConfigurationManager.AppSettings["outputDir"];
            WriteFile(outputDir + @"autobuild.master.xml", bmx.TransformText(), null, false);
            foreach (var ps in bi.ProjectSetsToBuild)
            {
                buildBatchTemplate bbt = new buildBatchTemplate(bi, ps);
                WriteFile(outputDir + @"autobuild." + ps.BuildKeyName + ".xml", bbt.TransformText(), ps, false);
            }
        }
        private void WriteFile(string outputLocation, string classText, ProjectSet ps, bool appendText = false)
        {
            _logger.WriteLine("Generating build file: " + outputLocation);
            if (ps != null)
            {
                ps.ProjectInfos.ForEach(pi =>
                {
                    _logger.WriteLine("     " + pi.AssemblyName);
                });
            }
            using (StreamWriter sw = new StreamWriter(outputLocation, appendText))
            {
                sw.Write(classText);
            }
        }
    }
}