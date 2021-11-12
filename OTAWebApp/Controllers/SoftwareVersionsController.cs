using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OTAWebApp.Data;
using OTAWebApp.Models;
using OTAWebApp.ViewModels;

namespace OTAWebApp.Controllers
{
    public class SoftwareVersionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SoftwareVersionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: SoftwareVersions
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.SoftwareVersion.Include(s => s.SoftwareType);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: SoftwareVersions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareVersion = await _context.SoftwareVersion
                .Include(s => s.SoftwareType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareVersion == null)
            {
                return NotFound();
            }

            return View(softwareVersion);
        }

        // GET: SoftwareVersions/Create
        public IActionResult Create(int SoftwareTypeId)
        {
            var allHardwareTypes = _context.HardwareType.ToList();
            var hardwareTypes = new List<AssignedHardwareTypeData>();
            foreach (var hardwareType in allHardwareTypes) 
            {
                hardwareTypes.Add(new AssignedHardwareTypeData
                {
                    HardwareTypeId = hardwareType.Id,
                    Name = hardwareType.Name,
                    Assigned = false
                });
            }

            ViewBag.HardwareTypes = hardwareTypes;
            ViewBag.SoftwareTypeId = SoftwareTypeId;
            return View();
        }

        // POST: SoftwareVersions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SoftwareTypeId,Major,Minor,Patch,Label,Author")] SoftwareVersion softwareVersion, string[] selectedHardwareTypes)
        {

#if USE_FTP
            const string FTPServerBaseUrl = "ftp://ftpd_server/";

            IFormFileCollection files = Request.Form.Files;
            if (files.Count == 0 || files.Count > 1)
            {
                return new BadRequestResult();
            }

            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file.FileName);

                // Get the path => ~/User/SoftwareTypeName/
                string FTPServerPath = "";
                SoftwareType softwareType = _context.SoftwareType.Where(b => b.Id.Equals(softwareVersion.SoftwareTypeId)).FirstOrDefault();
                if (softwareType != null)   //Check if exists.
                {
                    FTPServerPath = softwareType.Name + "/";

                    // check if directory exists
                    try
                    {
                        string route = FTPServerBaseUrl + FTPServerPath;
                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(route);
                        //softwareVersion.FirmwarePath = route;

                        request.Method = WebRequestMethods.Ftp.MakeDirectory;

                        //Enter FTP Server credentials.
                        request.Credentials = new NetworkCredential("asanchez", "eskidefondo");
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.EnableSsl = false;

                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                        if (response.StatusCode != FtpStatusCode.PathnameCreated)
                        {
                            // Folder already created.
                        }

                        response.Close();
                    }
                    catch (WebException ex)
                    {
                        //return new BadRequestResult();
                        //throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                    }

                    // Get the file name => /Major_Minor_Patch_Label_UniqueGuid;
                    string FileName = softwareVersion.Major.ToString() + "_" + softwareVersion.Minor.ToString() + "_" + softwareVersion.Patch.ToString() + "_"
                        + softwareVersion.Label.ToString() + "_" + Guid.NewGuid().ToString() + fi.Extension;

                    MemoryStream fileStream = new MemoryStream();
                    file.CopyTo(fileStream);

                    try
                    {
                        string route = FTPServerBaseUrl + FTPServerPath + FileName;

                        FtpWebRequest request = (FtpWebRequest)WebRequest.Create(route);
                        softwareVersion.FirmwarePath = route;

                        request.Method = WebRequestMethods.Ftp.UploadFile;

                        //Enter FTP Server credentials.
                        request.Credentials = new NetworkCredential("asanchez", "eskidefondo");
                        request.ContentLength = fileStream.Length;
                        request.UsePassive = true;
                        request.UseBinary = true;
                        request.ServicePoint.ConnectionLimit = (int)fileStream.Length;
                        request.EnableSsl = false;

                        using (Stream requestStream = request.GetRequestStream())
                        {
                            requestStream.Write(fileStream.GetBuffer(), 0, (int)fileStream.Length);
                            requestStream.Close();
                        }

                        FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                        //lblMessage.Text += FileName + " uploaded.<br />";
                        response.Close();

                        //return new OkResult();
                    }
                    catch (WebException ex)
                    {
                        return new BadRequestResult();
                        throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
                    }
                }
                else
                {
                    return new NotFoundResult();
                }
            }
#endif
            softwareVersion.FirmwarePath = "route";
            softwareVersion.Date = DateTime.Now;
            
            if (selectedHardwareTypes == null)
            {
                softwareVersion.SupportedHardware = new List<HardwareType>();
            }
            foreach (var hardwareId in selectedHardwareTypes) 
            {
                softwareVersion.SupportedHardware = new List<HardwareType>();
                softwareVersion.SupportedHardware.Add(_context.HardwareType.Where(i => i.Id.ToString() == hardwareId).FirstOrDefault());
            }

            if (ModelState.IsValid)
            {
                _context.Add(softwareVersion);
                await _context.SaveChangesAsync();
                ViewData.Add("SoftwareTypeId", softwareVersion.SoftwareTypeId);
                return RedirectToAction("Details", "SoftwareTypes", new { id = softwareVersion.SoftwareTypeId });
            }
            //return View(softwareVersion);

            ViewData.Add("SoftwareTypeId", softwareVersion.SoftwareTypeId);
            return RedirectToAction("Details", "SoftwareTypes", new { id = softwareVersion.SoftwareTypeId });
        }

        // GET: SoftwareVersions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareVersion = _context.SoftwareVersion
                .Include(i => i.SupportedHardware)
                .Where(i => i.Id == id)
                .Single();

            if (softwareVersion == null)
            {
                return NotFound();
            }

            var allHardwareTypes = _context.HardwareType;
            var supportedHardware = new HashSet<int>(softwareVersion.SupportedHardware.Select(c => c.Id));
            var hardwareTypes = new List<AssignedHardwareTypeData>();
            foreach (var hardwareType in allHardwareTypes)
            {
                hardwareTypes.Add(new AssignedHardwareTypeData
                {
                     HardwareTypeId = hardwareType.Id,
                     Name = hardwareType.Name,
                     Assigned = supportedHardware.Contains(hardwareType.Id)
                });
            }
            ViewBag.HardwareTypes = hardwareTypes;
        
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id", softwareVersion.SoftwareTypeId);
            return View(softwareVersion);
        }

        // POST: SoftwareVersions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SoftwareTypeId,Major,Minor,Patch,Label,Author")] SoftwareVersion softwareVersion, string[] selectedHardwareTypes)
        {
            if (id != softwareVersion.Id)
            {
                return NotFound();
            }

            var SoftwareVersionToUpdate = _context.SoftwareVersion
               .Include(i => i.SupportedHardware)
               .Where(i => i.Id == id)
               .Single();

            if (selectedHardwareTypes != null)
            {
                if (selectedHardwareTypes == null)
                {
                    SoftwareVersionToUpdate.SupportedHardware = new List<HardwareType>();
                }

                var selectedHardwareTypesHS = new HashSet<string>(selectedHardwareTypes);
                var softwareHardwareTypes = new HashSet<int>(SoftwareVersionToUpdate.SupportedHardware.Select(c => c.Id));

                foreach (var hardwareType in _context.HardwareType)
                {
                    if (selectedHardwareTypesHS.Contains(hardwareType.Id.ToString()))
                    {
                        if (!softwareHardwareTypes.Contains(hardwareType.Id))
                        {
                            SoftwareVersionToUpdate.SupportedHardware.Add(hardwareType);
                        }
                    }
                    else
                    {
                        if (softwareHardwareTypes.Contains(hardwareType.Id))
                        {
                            SoftwareVersionToUpdate.SupportedHardware.Remove(hardwareType);
                        }
                    }
                }
            }

            //softwareVersion.SupportedHardware = SoftwareVersionToUpdate.SupportedHardware;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(SoftwareVersionToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SoftwareVersionExists(softwareVersion.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id", softwareVersion.SoftwareTypeId);
            return View(softwareVersion);
        }

        // GET: SoftwareVersions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var softwareVersion = await _context.SoftwareVersion
                .Include(s => s.SoftwareType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (softwareVersion == null)
            {
                return NotFound();
            }

            return View(softwareVersion);
        }

        // POST: SoftwareVersions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var softwareVersion = await _context.SoftwareVersion.FindAsync(id);
#if USE_FTP
            try
            {
                string route = softwareVersion.FirmwarePath;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(route);
                request.Method = WebRequestMethods.Ftp.DeleteFile;

                //Enter FTP Server credentials.
                request.Credentials = new NetworkCredential("asanchez", "eskidefondo");
                request.UsePassive = true;
                request.UseBinary = true;
                request.EnableSsl = false;

                FtpWebResponse response = (FtpWebResponse)request.GetResponse();

                if (response.StatusCode != FtpStatusCode.FileActionOK)
                {
                    //return RedirectToAction(nameof(Index));
                }

                response.Close();
            }
            catch (WebException ex)
            {
                //return new BadRequestResult();
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
#endif
            _context.SoftwareVersion.Remove(softwareVersion);
            await _context.SaveChangesAsync();
            ViewData.Add("SoftwareTypeId", softwareVersion.SoftwareTypeId);
            return RedirectToAction("Details", "SoftwareTypes", new { id = softwareVersion.SoftwareTypeId });
            //return View(softwareVersion);
        }

        private bool SoftwareVersionExists(int id)
        {
            return _context.SoftwareVersion.Any(e => e.Id == id);
        }
    }
}
