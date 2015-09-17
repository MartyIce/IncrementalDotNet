using System.Collections.Generic;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain
{
    public class BuildFileExecutor
    {
        private IChangeFinder _changeFinder;
        private IBuildInfoGenerator _buildInfoGenerator;
        private IBuildFileCreator _buildFileCreator;

        public BuildFileExecutor(IChangeFinder changeFinder, IBuildInfoGenerator buildInfoGenerator, IBuildFileCreator buildFileCreator)
        {
            _changeFinder = changeFinder;
            _buildInfoGenerator = buildInfoGenerator;
            _buildFileCreator = buildFileCreator;
        }

        public void Execute()
        {
            List<ProjectInfo> pis = _changeFinder.FindProjectsWithRecentChanges(@"c:\temp");

            BuildInfo bi = _buildInfoGenerator.GenerateBuild(pis);

            _buildFileCreator.WriteBuildFile(bi);
        }

    }

}
