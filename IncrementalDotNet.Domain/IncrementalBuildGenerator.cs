using System.Collections.Generic;
using System.Configuration;
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
            List<ProjectInfo> pis = _changeFinder.FindProjectsWithRecentChanges(ConfigurationManager.AppSettings["rootDir"]);

            BuildInfo bi = _buildInfoGenerator.GenerateBuild(pis);

            _buildFileCreator.WriteBuildFile(bi);
        }

    }

}
