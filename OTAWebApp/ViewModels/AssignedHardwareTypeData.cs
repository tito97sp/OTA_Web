using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.ViewModels
{
    public class AssignedHardwareTypeData
    {
        public int HardwareTypeId { get; set; }
        public string Name { get; set; }
        public bool Assigned { get; set; }
    }
}
