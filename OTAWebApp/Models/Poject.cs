using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OTAWebApp.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }

        public String Name { get; set; }
        public String Description { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastModificationDate { get; set; }

        public virtual ICollection<SoftwareType> SoftwareTypes { get; set; }

        public void update_LastModification() 
        {
            LastModificationDate = DateTime.Now;
        }

    }
}
