#define USE_FTP

using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace OTAWebApp.Utils
{
    public class CommonTools
    {
        public static string DateFormated(DateTime DateTime)
        {
            if (DateTime.Now.Subtract(DateTime).Days < 1)
            {
                return DateTime.ToString("HH:mm");
            }
            else if (DateTime.Now.Subtract(DateTime).Days >= 1 && DateTime.Now.Subtract(DateTime).Days < 2)
            {
                return DateTime.Now.Subtract(DateTime).Days.ToString() + " day ago";
            }
            else if (DateTime.Now.Subtract(DateTime).Days >= 2 && DateTime.Now.Subtract(DateTime).Days < 7)
            {
                return DateTime.Now.Subtract(DateTime).Days.ToString() + " day ago";
            }
            else
            {
                return DateTime.Date.ToString("dd:MM:yyyy");
            }
        }

    }

    public class FTPUtils
    {
        public static void FTP_GetFile()
        {
        }

        public static string FTP_AddFile(IFormFile file, string savePath, string saveName)
        {
#if USE_FTP
#if DOCKER
            const string FTPServerBaseUrl = "ftp://ftpd_server/";
#else
            const string FTPServerBaseUrl = "ftp://127.0.0.1/";
#endif

            string[] broken_savepath = savePath.Split('/');

            // check if project directory exists
            string route = FTPServerBaseUrl;
            try
            {
                foreach (var sub_str in broken_savepath)
                {
                    route = route += (sub_str + "/");

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
            }
            catch (WebException ex)
            {
                // directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
            }

            // Get the file name => /Major_Minor_Patch_Label_UniqueGuid;
            //string FileName = softwareVersion.Major.ToString() + "_" 
            //                            + softwareVersion.Minor.ToString() + "_"
            //                            + softwareVersion.Patch.ToString() + "_"
            //                            + softwareVersion.Label.ToString() + "_" 
            //                            + Guid.NewGuid().ToString() 
            //                            + fi.Extension;

            MemoryStream fileStream = new MemoryStream();
            file.CopyTo(fileStream);

            try
            {
                route = FTPServerBaseUrl + savePath + saveName;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(route);

                //Enter FTP Server credentials.
                request.Method = WebRequestMethods.Ftp.UploadFile;
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
                response.Close();

                return route;
            }
            catch (WebException ex)
            {
                //return new BadRequestResult();
                throw new Exception((ex.Response as FtpWebResponse).StatusDescription);
            }
#else
            softwareVersion.FirmwarePath = "route";
#endif
        }

        public static void FTP_DeleteFile(string filePath)
        {
#if USE_FTP
            try
            {
                string route = filePath;

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
                }

                response.Close();
            }
            catch (Exception)
            {
                throw;
            }
#endif
        }
    }
}
