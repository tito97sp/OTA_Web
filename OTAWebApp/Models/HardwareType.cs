using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.Models
{
    public class HardwareType
    {
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
    }
}

