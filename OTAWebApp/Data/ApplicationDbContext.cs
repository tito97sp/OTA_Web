using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using OTAWebApp.Models;

namespace OTAWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<OTAWebApp.Models.SoftwareType> SoftwareType { get; set; }
        public DbSet<OTAWebApp.Models.SoftwareVersion> SoftwareVersion { get; set; }
        public DbSet<OTAWebApp.Models.Device> Device { get; set; }
        public DbSet<OTAWebApp.Models.HardwareType> HardwareType { get; set; }
        public DbSet<OTAWebApp.Models.Project> Project { get; set; }

        
    }
}
