using IncrementalDotNet.Domain;
using NUnit.Framework;

namespace IncrementalDotNet.Test
{
    [TestFixture]
    public class ChangeFinder
    {
        [Test]
        public void Test()
        {
            Domain.ChangeFinder cf = new Domain.ChangeFinder(new MSBuildParser());
            string rootDirectory = @"C:\Temp";
            cf.FindProjectsWithRecentChanges(rootDirectory);
        }
    }
}