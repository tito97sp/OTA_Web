using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using OTAWebApp.Models;

namespace OTAWebApp.Dal
{
    public class OTAInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<OTAContext>
    {
        protected override void Seed(OTAContext context)
        {
            var softwaretypes = new List<SoftwareType>
            {
            new SoftwareType{ Name = "Nuttx_Firmwre", Date = DateTime.Parse("2002-09-01"), Description = "Firmware for nuttx"},
            new SoftwareType{ Name = "Nuttx_Bootloader", Date = DateTime.Parse("2006-03-04"), Description = "Firmware for test OTA bootloader"},
            new SoftwareType{ Name = "Linux_Firmwre", Date = DateTime.Parse("2008-12-01"), Description = "Firmware for linux"},
            };

            softwaretypes.ForEach(s => context.SoftwareTypes.Add(s));
            context.SaveChanges();

            var softwareversions = new List<SoftwareVersion>
            {
            new SoftwareVersion{SoftwareTypeId=1,Major=1,Minor=1, Patch=0,Label= "alpha", Author = "asanchez", Date= DateTime.Parse("2002-09-01")},
            };
            softwareversions.ForEach(s => context.SoftwareVersions.Add(s));
            context.SaveChanges();
        }
    }
}