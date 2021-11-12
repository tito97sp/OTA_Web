using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.Models
{
    public class SytemCompatiblity
    {
        [Key]
        public int Id { get; set; }
        public int HardwareTypeId { get; set; }
        public int SoftwareTypeId { get; set; }
        public int SoftwareVersionId { get; set; }
        public virtual HardwareType HardwareType { get; set; }
        public virtual SoftwareType SoftwareType { get; set; }
        public virtual SoftwareVersion SoftwareVersion { get; set; }
    }
}
