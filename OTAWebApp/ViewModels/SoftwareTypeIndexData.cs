using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OTAWebApp.Models;

namespace OTAWebApp.ViewModels
{
    public class SoftwareTypeIndexData
    {
        public IEnumerable<SoftwareType> SoftwareTypes { get; set; }
        public IEnumerable<SoftwareVersion> SoftwareVersions { get; set; }
    }
}
