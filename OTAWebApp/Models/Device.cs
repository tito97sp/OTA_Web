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

        public String NickName { get; set; }
        public String SerialNumber { get; set; }
        public String BoardVendor { get; set; }
        public String BoardModel { get; set; }
        public String BoardLabel { get; set; }
        public String Software { get; set; }
        public String SoftwareLabel { get; set; }
        public String SoftwareVersion { get; set; }
        public String GitHash { get; set; }

        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }

        public String Notes { get; set; }

        public int SoftwareTypeId { get; set; }

        public virtual SoftwareType SoftwareType { get; set; }

    }
}
