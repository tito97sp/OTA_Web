using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.Models
{
    public class SoftwareVersion
    {
        [Key]
        public int Id { get; set; }
        public int SoftwareTypeId {get; set;}

        public uint Major { get; set; }
        public uint Minor { get; set; }
        public uint Patch { get; set; }
        public string Label { get; set; }
        public String Author { get; set; }
        public DateTime Date { get; set; }
        public string FirmwarePath { get; set; }

        [Display(Name = "Full Version")]
        public string FullVersion
        {
            get { return Major + "." + Minor + "." + Patch + "." + Label; }
        }

        public virtual SoftwareType SoftwareType { get; set; }
       
    }
}
