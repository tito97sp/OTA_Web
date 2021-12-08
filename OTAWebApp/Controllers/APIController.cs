using Microsoft.AspNetCore.Mvc;
using OTAWebApp.Data;
using OTAWebApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace OTAWebApp.Controllers
{
    namespace Api_v1
    {
        [ApiController]
        [ApiVersion("1.0")]
        [Route("api/{version:apiVersion}/Downloads")]
        public class DownloadsV1Controller : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            public DownloadsV1Controller(ApplicationDbContext context)
            {
                _context = context;
            }

            enum DeviceType 
            {
                Test,
                Development, 
                Production
            }
            public class DeviceConnection
            {
                [FromHeader]
                public String DeviceID { get; set; }
                [FromHeader]
                public String ProjectName { get; set; }
                [FromHeader]
                public String BoardVendor { get; set; }
                [FromHeader]
                public String BoardModel { get; set; }
                [FromHeader]
                public String BoardLabel { get; set; }
                [FromHeader]
                public String SoftwareConf { get; set; }
                [FromHeader]
                public String SoftwareVersion { get; set; }
                [FromHeader]
                public String SoftwareGitHash { get; set; }
            }

            
            // GET: api/<DownloadsController>
            [HttpGet]
            public async Task<IActionResult> Get([FromHeader] DeviceConnection DeviceConnection)
            {
                // Manage the device.
                try
                {
                    // check if previously registered
                    var device = _context.Device
                    .Where(i => i.SerialNumber.Equals(DeviceConnection.DeviceID))
                    .Single();

                    device.LastSeen = DateTime.Now;

                    if (ModelState.IsValid) 
                    {
                        _context.Update(device);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (Exception ex)
                {
                    if (ex is ArgumentNullException || ex is InvalidOperationException) 
                    {
                        // add device to database.
                        var device = new Device();

                        if (DeviceConnection.DeviceID == null) 
                        {
                            goto no_add_device;
                        }
                        if(DeviceConnection.ProjectName == null ||
                            !_context.Project.Any(i => i.Name.Equals(DeviceConnection.ProjectName))) 
                        {
                            goto no_add_device;
                        }
                        if(DeviceConnection.SoftwareConf == null || 
                            !_context.SoftwareType.Any(i => i.Name.Equals(DeviceConnection.SoftwareConf)))
                        {
                            goto no_add_device;
                        }

                        device.BoardLabel = DeviceConnection.BoardLabel;
                        device.BoardModel = DeviceConnection.BoardModel;
                        device.BoardVendor = DeviceConnection.BoardVendor;
                        device.GitHash = DeviceConnection.SoftwareGitHash;
                        device.SerialNumber = DeviceConnection.DeviceID;
                        device.SoftwareVersion = DeviceConnection.SoftwareVersion;
                        device.SoftwareLabel = DeviceConnection.SoftwareConf;
                        device.FirstSeen = DateTime.Now;
                        device.LastSeen = DateTime.Now;
                        device.SoftwareTypeId = _context.SoftwareType.Where(i => i.Name.Equals(DeviceConnection.SoftwareConf)).Single().Id;

                        if (ModelState.IsValid)
                        {
                            _context.Add(device);
                            await _context.SaveChangesAsync();
                        }

                    }
                }
                    
                no_add_device:

                // Look and return the correct version to update.
                try
                {
                    // TODO: Improve db access and robustness

                    var project = _context.Project
                        .Include(i => i.SoftwareTypes)
                        .Where(i => i.Name.Equals(DeviceConnection.ProjectName))
                        .Single();

                    var softwareType = _context.SoftwareType
                        .Where(i => i.Project.Name.Equals(DeviceConnection.ProjectName))
                        .Where(i => i.Name.Equals(DeviceConnection.SoftwareConf))
                        .Single();

                    var softwareVersion = _context.SoftwareVersion
                        .Include(i => i.SoftwareType)
                        .Where(i => i.SoftwareTypeId.Equals(softwareType.Id))
                        .OrderByDescending(i => i.Date)
                        .First();

                    string fileName = softwareVersion.Major + "_"
                                        + softwareVersion.Minor + "_"
                                        + softwareVersion.Patch + "_"
                                        + softwareVersion.Label + ".bin";

                    string route = softwareVersion.FirmwarePath;
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create(route);

                    request.Method = WebRequestMethods.Ftp.DownloadFile;

                    //Enter FTP Server credentials.
                    request.Credentials = new NetworkCredential("asanchez", "eskidefondo");
                    request.UsePassive = true;
                    request.UseBinary = true;
                    request.EnableSsl = false;

                    FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                    if (response.StatusCode != FtpStatusCode.FileActionOK)
                    {
                        // Folder already created.
                    }

                    using (MemoryStream stream = new MemoryStream())
                    {
                        response.GetResponseStream().CopyTo(stream);
                        response.Close();


                        return File(stream.GetBuffer(), MediaTypeNames.Text.Plain, fileName);
                    }
                }
                catch (WebException ex)
                {
                    throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                }
                catch (Exception ex)
                {
                    if (ex is ArgumentNullException || ex is InvalidOperationException)
                    {
                        return new NoContentResult();
                    }
                }
                
                return new NoContentResult();
            }
        }

        [ApiController]
        [ApiVersion("1.0")]
        [Route("api/{version:apiVersion}/Versions")]
        public class VersionsV1Controller : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            public VersionsV1Controller(ApplicationDbContext context)
            {
                _context = context;
            }

            // ReadVersion: api/<VersionsController>
            [HttpGet("read")]
            public IActionResult ReadVersion()
            {
                return new NoContentResult();
            }

            // CreateVersion: api/<VersionsController>
            [HttpPost("create")]
            public IActionResult CreateVersion()
            {
                return new NoContentResult();
            }

            // UpdateVersion: api/<VersionsController>
            [HttpPut("update")]
            public IActionResult UpdateVersion()
            {
                return new NoContentResult();
            }

            // UpdateVersion: api/<VersionsController>
            [HttpDelete("delete")]
            public IActionResult DeleteVersion()
            {
                return new NoContentResult();
            }

        }

        [ApiController]
        [ApiVersion("1.0")]
        [Route("api/{version:apiVersion}/Projects")]
        public class ProjectV1Controller : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            public ProjectV1Controller(ApplicationDbContext context)
            {
                _context = context;
            }

            // ReadProject: api/<ProjectsController>
            [HttpGet("read")]
            public IActionResult ReadProject()
            {
                return new NoContentResult();
            }

            // CreateProject: api/<ProjectsController>
            [HttpPost("create")]
            public IActionResult CreateProject()
            {
                return new NoContentResult();
            }

            // UpdateProject: api/<ProjectsController>
            [HttpPut("update")]
            public IActionResult UpdateProject()
            {
                return new NoContentResult();
            }

            // UpdateProject: api/<ProjectsController>
            [HttpDelete("delete")]
            public IActionResult DeleteProject()
            {
                return new NoContentResult();
            }

        }

        [ApiController]
        [ApiVersion("1.0")]
        [Route("api/{version:apiVersion}/Types")]
        public class TypesV1Controller : ControllerBase
        {
            private readonly ApplicationDbContext _context;
            public TypesV1Controller(ApplicationDbContext context)
            {
                _context = context;
            }

            // ReadType: api/<TypesController>
            [HttpGet("read")]
            public IActionResult ReadType()
            {
                return new NoContentResult();
            }

            // CreateType: api/<TypesController>
            [HttpPost("create")]
            public IActionResult CreateType()
            {
                return new NoContentResult();
            }

            // UpdateType: api/<TypesController>
            [HttpPut("update")]
            public IActionResult UpdateType()
            {
                return new NoContentResult();
            }

            // UpdateType: api/<TypesController>
            [HttpDelete("delete")]
            public IActionResult DeleteType()
            {
                return new NoContentResult();
            }

        }
    }
}
