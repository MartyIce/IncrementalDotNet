using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization.Json;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain
{
    public class IncrementalBuildGenerator
    {
        private IChangeFinder _changeFinder;
        private IBuildInfoGenerator _buildInfoGenerator;
        private IBuildFileCreator _buildFileCreator;

        public IncrementalBuildGenerator(IChangeFinder changeFinder, IBuildInfoGenerator buildInfoGenerator, IBuildFileCreator buildFileCreator)
        {
            _changeFinder = changeFinder;
            _buildInfoGenerator = buildInfoGenerator;
            _buildFileCreator = buildFileCreator;
        }

        public void Execute()
        {
            bool buildAll = false;
            bool.TryParse(ConfigurationManager.AppSettings["buildAll"].ToString(), out buildAll);
            List<ProjectInfo> pis = _changeFinder.FindProjectsToBuild(ConfigurationManager.AppSettings["rootDir"], buildAll);

            BuildInfo bi = _buildInfoGenerator.GenerateBuild(pis, ConfigurationManager.AppSettings["buildFileRelativeOffset"]);

            _buildFileCreator.WriteBuildFile(bi);
        }

    }

}
