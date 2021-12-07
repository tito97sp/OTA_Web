using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTAWebApp.Models;

namespace OTAWebApp.ViewModels
{
    public class ProjectTypeIndexData
    {
        public Project Project { get; set; }
        public IEnumerable<SoftwareType> SoftwareTypes { get; set; }

    }
}
