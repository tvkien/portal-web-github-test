using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Web.Helpers;

namespace LinkIt.BubbleSheetPortal.Web.Controllers
{
    public class AssetController : Controller
    {
        [HttpGet]
        public ActionResult GetViewReferenceImg(string imgPath)
        {
            if (string.IsNullOrEmpty(imgPath))
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }
           
            imgPath = Util.CorrectImgSrc(imgPath);
           
            var testItemMediaPath = string.Empty;
            if (string.IsNullOrWhiteSpace(testItemMediaPath))
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }

            imgPath = imgPath.Replace("/", "\\");
            if (imgPath.StartsWith("\\")) imgPath = imgPath.Substring(1);
            var roFilePath = Path.Combine(testItemMediaPath, imgPath);
            if (!System.IO.File.Exists(roFilePath))
            {
                return new FileContentResult(new byte[0], "image/jpeg");
            }

            var extension = Path.GetExtension(roFilePath);
            var contentType = Util.GetContentType(extension);

            var byteArray = System.IO.File.ReadAllBytes(roFilePath);
            return new FileContentResult(byteArray, contentType);
        }

        [HttpGet]
        public ActionResult GetViewReferenceImgFullPath(string imgPath)
        {
            if (string.IsNullOrWhiteSpace(imgPath)) return new FileContentResult(new byte[0], "image/jpeg");
            //if (IsImgRelativePath(imgPath)) return GetViewReferenceImg(imgPath);
            if (!System.IO.File.Exists(imgPath)) return new FileContentResult(new byte[0], "image/jpeg");

            var byteArray = System.IO.File.ReadAllBytes(imgPath);
            return new FileContentResult(byteArray, "image/jpeg");
        }
        private bool IsImgRelativePath(string path)
        {
            var startOfImgRelativePaths = new List<string> { "RO", "/RO", "ItemSet", "/ItemSet" };
            var result =
                startOfImgRelativePaths.Any(
                    startOfImgRelativePath =>
                        path.StartsWith(startOfImgRelativePath, StringComparison.OrdinalIgnoreCase));
            return result;
        }
    }
}