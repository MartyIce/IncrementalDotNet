using IncrementalDotNet.Domain;
using NUnit.Framework;

namespace IncrementalDotNet.Test
{
    [TestFixture]
    public class ChangeFinder
    {
        private class Logger : ILogger
        {
            public void WriteLine(string text)
            {
                throw new System.NotImplementedException();
            }
        }

        [Test]
        public void Test()
        {
            Domain.ChangeFinder cf = new Domain.ChangeFinder(new MSBuildParser(), new Logger());
            string rootDirectory = @"C:\Temp";
            cf.FindProjectsToBuild(rootDirectory, false);
        }
    }
}