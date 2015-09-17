using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IncrementalDotNet.Domain.Contracts;

namespace IncrementalDotNet.Domain.Templates
{
    public partial class buildBatchTemplate
    {
        public BuildInfo BuildInfo { get; set; }
        public ProjectSet CurrentProjectSet { get; set; }

        public buildBatchTemplate(BuildInfo bi ,ProjectSet model)
        {
            BuildInfo = bi;
            CurrentProjectSet = model;
        }    
    }
    public partial class buildMasterXml
    {
        public BuildInfo BuildInfo { get; set; }

        public buildMasterXml(BuildInfo model)
        {
            BuildInfo = model;
        }
    }
}
