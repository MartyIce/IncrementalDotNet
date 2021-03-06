﻿using System;
using System.Collections.Generic;
using IncrementalDotNet.Domain;
using IncrementalDotNet.Domain.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IncrementalDotNet.Test
{
    [TestClass]
    public class BuildFileExecutor
    {
        [TestMethod]
        public void TestExecute()
        {
            var changeFinder = new MockChangeFinder();
            var buildInfoGenerator = new MockBuildInfoGenerator();
            var buildFileCreator = new MockBuildFileCreator();
            Domain.IncrementalBuildGenerator bfe = new Domain.IncrementalBuildGenerator(changeFinder, buildInfoGenerator, buildFileCreator);
            bfe.Execute();

            Assert.AreEqual(changeFinder.GeneratedPis, buildInfoGenerator.PassedPis);
        }

        public class MockBuildInfoGenerator : IBuildInfoGenerator
        {
            public List<ProjectInfo> PassedPis { get; set; }
            public BuildInfo GeneratedBi { get; set; }
            public BuildInfo GenerateBuild(List<ProjectInfo> pis, string relativePathOffset)
            {
                PassedPis = pis;
                GeneratedBi = new BuildInfo("");
                return GeneratedBi;
            }
        }

        public class MockChangeFinder : IChangeFinder
        {
            public List<ProjectInfo> GeneratedPis { get; set; }
            public List<ProjectInfo> FindProjectsToBuild(string rootDirectory, bool buildAll)
            {
                GeneratedPis = new List<ProjectInfo>();
                return GeneratedPis;
            }

        }
    }

    public class MockBuildFileCreator : IBuildFileCreator
    {
        public void WriteBuildFile(BuildInfo bi)
        {
            
        }
    }
}
