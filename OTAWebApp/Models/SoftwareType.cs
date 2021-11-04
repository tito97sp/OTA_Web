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
    
    public class SoftwareType
    {
        [Key]
        public int Id { get; set; }
        public String Name { get; set; }
        public DateTime Date { get; set; }
        public String Description { get; set; }

        public virtual ICollection<SoftwareVersion> SoftwareVersions { get; set; }

    }
}
