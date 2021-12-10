using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace OTAWebApp.Models
{
    public class SoftwareVersion
    {
        [Key]
        public int Id { get; set; }
        public int SoftwareTypeId {get; set;}

        [Required]
        public int Major { get; set; }

        [Required]
        public int Minor { get; set; }
        
        [Required]
        public int Patch { get; set; }

        [Required] [StringLength(50, MinimumLength= 1)]
        public string Label { get; set; }

        [Required] [StringLength(50, MinimumLength= 2)]
        public String Author { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string FirmwarePath { get; set; } = "not set";

        [Display(Name = "Full Version")]
        public string FullVersion
        {
            get { return Major + "." + Minor + "." + Patch + "." + Label; }
        }

        public virtual SoftwareType SoftwareType { get; set; }
        public virtual ICollection<HardwareType> SupportedHardware { get; set; }

        public string createUniqueId() 
        {
            var uniqueId = Major.ToString() + "_"
                            + Minor.ToString() + "_"
                            + Patch.ToString() + "_"
                            + Label.ToString() + "_"
                            + Guid.NewGuid().ToString();
            return uniqueId;
        }
        public bool isValid()
        {
            Type t = this.GetType();

            var properties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (prop.Name.Equals("SoftwareType") 
                    || prop.Name.Equals("SupportedHardware"))
                { 
                }
                else {
                    var value = prop.GetValue(this, null);

                    if (value == null /*&& (string)value != ""*/)
                    {
                        return false;
                    } 
                }
            }

            return true;
        }

    }
}
