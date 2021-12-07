using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using OTAWebApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using OTAWebApp.Data;
using System.Net.Mime;
using Microsoft.EntityFrameworkCore;

namespace OTAWebApp.Controllers
{
    public class DownloadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DownloadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            StringValues DeviceID;
            StringValues ProjectName;
            StringValues BoardVendor;
            StringValues BoardModel;
            StringValues BoardLabel;
            StringValues SoftwareConf;
            StringValues SoftwareVersion;
            StringValues SoftwareGitHash;

            Request.Headers.TryGetValue("DeviceID", out DeviceID);    
            Request.Headers.TryGetValue("ProjectName", out ProjectName);
            Request.Headers.TryGetValue("BoardVendor", out BoardVendor);            
            Request.Headers.TryGetValue("BoardModel", out BoardModel);            
            Request.Headers.TryGetValue("BoardLabel", out BoardLabel);
            Request.Headers.TryGetValue("SoftwareConf", out SoftwareConf);
            Request.Headers.TryGetValue("SoftwareVersion", out SoftwareVersion);         
            Request.Headers.TryGetValue("SoftwareGitHash", out SoftwareGitHash);

            ProjectName = "embedded_project_template";
            SoftwareConf = "stm_nucleo-h743zi_main";

#if DOCKER
            const string FTPServerBaseUrl = "ftp://ftpd_server/";
#else
            const string FTPServerBaseUrl = "ftp://127.0.0.1/";
#endif
            //new
            //{
            //    SerialNumber = DeviceID,
            //    BoardVendor = BoardVendor,
            //    BoardModel = BoardModel,
            //    BoardLabel = BoardLabel,
            //    Software = ProjectName,
            //    SoftwareLabel = SoftwareConf,
            //    SoftwareVersion = SoftwareVersion,
            //    GitHash = SoftwareGitHash
            //}
            try
            {
                var project = _context.Project
                .Include(i => i.SoftwareTypes)
                .Where(i => i.Name.Equals(ProjectName))
                .Single();

                var softwareType = project.SoftwareTypes
                    .Where(i => i.Name.Equals(SoftwareConf))
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


        [HttpPost]
        public async Task<IActionResult> DownloadPost()
        {
            StringValues BoardId;
            StringValues CurrentVersion;

            Request.Headers.TryGetValue("BoardId", out BoardId);
            Request.Headers.TryGetValue("CurrentVersion", out CurrentVersion);


            const string FTPServerBaseUrl = "ftp://127.0.0.1/Test/test.txt";

            try
            {
                string route = FTPServerBaseUrl;
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

                    return File(stream.GetBuffer(), MediaTypeNames.Text.Plain, "test.txt");
                }
            }
            catch (WebException ex)
            {
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
        }
    }
}
