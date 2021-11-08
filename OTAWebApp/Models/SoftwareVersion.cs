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

        [Required]
        public uint Major { get; set; }

        [Required]
        public uint Minor { get; set; }
        
        [Required]
        public uint Patch { get; set; }

        [Required] [StringLength(50, MinimumLength= 1)]
        public string Label { get; set; }

        [Required] [StringLength(50, MinimumLength= 2)]
        public String Author { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
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
