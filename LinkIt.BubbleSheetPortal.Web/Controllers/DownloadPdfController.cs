using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    [AjaxAwareAuthorize]
    public class DownloadPdfController : BaseController
    {
        private const string DownloadPdfFolderKeySettings = "DownloadPdfFolderPath";

        private readonly DownloadPdfService _downloadPdfService;
        public DownloadPdfController(DownloadPdfService downloadPdfService)
        {
            _downloadPdfService = downloadPdfService;
        }

        public ActionResult Index(Guid? pdfID)
        {
            if (pdfID == null) return ErrorDownload();
            var pdf = _downloadPdfService.GetDownloadPdf(pdfID.Value);
            if (!string.IsNullOrEmpty(pdf.FilePath) &&
                           (pdf.FilePath.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || pdf.FilePath.StartsWith("https://", StringComparison.OrdinalIgnoreCase)))
            {
                WebClient webClient = new WebClient();
                byte[] documentData = webClient.DownloadData(pdf.FilePath);

                string fileName = string.Empty;

                try
                {
                    fileName = System.IO.Path.GetFileName(pdf.FilePath);
                }
                catch (Exception)
                {
                    fileName = string.Empty;
                }

                if (string.IsNullOrEmpty(fileName))
                    fileName = pdfID.Value.ToString();

                return File(documentData, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                var canDownload = _downloadPdfService.CanDownload(pdf, CurrentUser.Id);
                if (!canDownload) return ErrorDownload();

                var pdfFolderPath = DownloadPdfFolderPath();
                var filePath = GetDownloadPdfAbsoluteFilePath(pdf.FilePath, pdfFolderPath);

                if (string.IsNullOrWhiteSpace(filePath)) return ErrorDownload();

                if (Path.GetExtension(filePath) == ".zip")
                {
                    var type = "application/zip";
                    var fileName = Path.GetFileName(pdf.FilePath);
                    return File(filePath, type, fileName);
                }
                else
                {
                    var type = "application/pdf";
                    return File(filePath, type);
                }
            }
        }

        private ActionResult ErrorDownload()
        {
            return Content("<script language='javascript' type='text/javascript'>alert('File is invalid.');</script>");
        }

        public string GetDownloadPdfAbsoluteFilePath(string filePath, string folderPath)
        {
            if (filePath == null) return null;
            var absoluteFilePath = string.Format("{0}\\{1}", folderPath, filePath);
            absoluteFilePath = RemoveDublicateSlash(absoluteFilePath);

            if (!System.IO.File.Exists(absoluteFilePath)) return null;

            return absoluteFilePath;
        }

        public string DownloadPdfFolderPath()
        {
            var result = System.Configuration.ConfigurationManager.AppSettings[DownloadPdfFolderKeySettings];
            return result;
        }

        public string RemoveDublicateSlash(string path)
        {
            if (path == null) return null;
            path = path.Trim().Replace("/", "\\");

            var result = string.Empty;
            if (path.StartsWith("\\\\")) result = "\\\\";

            var dilimiters = new char[] { '\\' };
            var splitBySlash = path.Split(dilimiters, StringSplitOptions.RemoveEmptyEntries);

            result = result + string.Join("\\", splitBySlash);

            return result;
        }
    }
}
