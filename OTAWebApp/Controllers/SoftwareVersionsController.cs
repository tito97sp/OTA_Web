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
            ViewBag.SoftwareTypeId = SoftwareTypeId;
            return View();
        }

        // POST: SoftwareVersions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SoftwareTypeId,Major,Minor,Patch,Label,Author")] SoftwareVersion softwareVersion)
        {
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

            softwareVersion.Date = DateTime.Now;

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

            var softwareVersion = await _context.SoftwareVersion.FindAsync(id);
            if (softwareVersion == null)
            {
                return NotFound();
            }
            ViewData["SoftwareTypeId"] = new SelectList(_context.SoftwareType, "Id", "Id", softwareVersion.SoftwareTypeId);
            return View(softwareVersion);
        }

        // POST: SoftwareVersions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SoftwareTypeId,Major,Minor,Patch,Label,Author,Date,FirmwarePath")] SoftwareVersion softwareVersion)
        {
            if (id != softwareVersion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(softwareVersion);
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
