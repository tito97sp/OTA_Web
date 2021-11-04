using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Threading.Tasks;
using OTAWebApp.Models;

namespace OTAWebApp.Dal
{
    public class OTAContext : DbContext
    {
        public OTAContext() : base("OTAContext")
        {
        }

        public DbSet<SoftwareType> SoftwareTypes { get; set; }
        public DbSet<SoftwareVersion> SoftwareVersions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        

    }

}
