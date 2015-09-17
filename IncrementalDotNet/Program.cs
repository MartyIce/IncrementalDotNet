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
            BuildFileExecutor bfe = new BuildFileExecutor(new ChangeFinder(), new BuildInfoGenerator(), new MSBuildFileCreator());
            bfe.Execute();

        }
    }

}
