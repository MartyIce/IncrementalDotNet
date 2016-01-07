using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IncrementalDotNet.Domain;

namespace IncrementalDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = new Logger();
            IncrementalBuildGenerator ibg = new IncrementalBuildGenerator(new ChangeFinder(new MSBuildParser(), logger), new BuildInfoGenerator(), new MSBuildFileCreator(logger));
            ibg.Execute();

        }
    }

    internal class Logger : ILogger
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }

}
