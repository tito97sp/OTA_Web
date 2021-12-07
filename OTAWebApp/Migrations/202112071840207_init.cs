namespace OTAWebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Device",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NickName = c.String(),
                        SerialNumber = c.String(),
                        BoardVendor = c.String(),
                        BoardModel = c.String(),
                        BoardLabel = c.String(),
                        Software = c.String(),
                        SoftwareLabel = c.String(),
                        SoftwareVersion = c.String(),
                        GitHash = c.String(),
                        FirstSeen = c.DateTime(nullable: false),
                        LastSeen = c.DateTime(nullable: false),
                        Notes = c.String(),
                        SoftwareTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareType", t => t.SoftwareTypeId, cascadeDelete: true)
                .Index(t => t.SoftwareTypeId);
            
            CreateTable(
                "dbo.SoftwareType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProjectId = c.Int(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Project", t => t.ProjectId, cascadeDelete: true)
                .Index(t => t.ProjectId);
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        CreationDate = c.DateTime(nullable: false),
                        LastModificationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.SoftwareVersion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SoftwareTypeId = c.Int(nullable: false),
                        Major = c.Int(nullable: false),
                        Minor = c.Int(nullable: false),
                        Patch = c.Int(nullable: false),
                        Label = c.String(nullable: false, maxLength: 50),
                        Author = c.String(nullable: false, maxLength: 50),
                        Date = c.DateTime(nullable: false),
                        FirmwarePath = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareType", t => t.SoftwareTypeId, cascadeDelete: true)
                .Index(t => t.SoftwareTypeId);
            
            CreateTable(
                "dbo.HardwareType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        SoftwareVersion_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SoftwareVersion", t => t.SoftwareVersion_Id)
                .Index(t => t.SoftwareVersion_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Device", "SoftwareTypeId", "dbo.SoftwareType");
            DropForeignKey("dbo.HardwareType", "SoftwareVersion_Id", "dbo.SoftwareVersion");
            DropForeignKey("dbo.SoftwareVersion", "SoftwareTypeId", "dbo.SoftwareType");
            DropForeignKey("dbo.SoftwareType", "ProjectId", "dbo.Project");
            DropIndex("dbo.HardwareType", new[] { "SoftwareVersion_Id" });
            DropIndex("dbo.SoftwareVersion", new[] { "SoftwareTypeId" });
            DropIndex("dbo.SoftwareType", new[] { "ProjectId" });
            DropIndex("dbo.Device", new[] { "SoftwareTypeId" });
            DropTable("dbo.HardwareType");
            DropTable("dbo.SoftwareVersion");
            DropTable("dbo.Project");
            DropTable("dbo.SoftwareType");
            DropTable("dbo.Device");
        }
    }
}
