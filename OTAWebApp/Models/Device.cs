using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.Models
{
    public class Device
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public String NickName { get; set; }
        public String SerialNumber { get; set; }
        
        public int HardwareTypeId { get; set; }
        public int SoftwareTypeId { get; set; }

        public virtual HardwareType HardwareType { get; set; }
        public virtual SoftwareType SoftwareType { get; set; }


    }
}
