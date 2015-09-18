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
            IncrementalBuildGenerator ibg = new IncrementalBuildGenerator(new ChangeFinder(new MSBuildParser()), new BuildInfoGenerator(), new MSBuildFileCreator());
            ibg.Execute();

        }
    }

}
