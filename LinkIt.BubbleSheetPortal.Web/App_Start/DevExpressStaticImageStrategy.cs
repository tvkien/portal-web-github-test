using System;
using System.Configuration;
using System.IO;
using DevExpress.Web.ASPxClasses;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    // images are cached within the predefined public folder. The method returns an image url.
    public class DevExpressStaticImageStrategy : RuntimeStorageStrategy
    {
        private readonly IS3Service _s3Service;

        public DevExpressStaticImageStrategy(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        public override string GetResourceUrl(ASPxWebControlBase control, byte[] content, string mimeType,
            string contentDisposition)
        {
            string fileExtension = "";
            if (mimeType.StartsWith("image/"))
                fileExtension = "." + mimeType.Substring("image/".Length);

            string fileName = GetControlUniqueName(control) + fileExtension;
            string buketName = LinkitConfigurationManager.GetS3Settings().ACTReportBucket;
            string folder = ConfigurationManager.AppSettings["ACTReportFolder"];

            try
            {
                string s3FileName = string.Format("{0}/{1}", folder, fileName);
                var s3Result = _s3Service.UploadRubricFile(buketName, s3FileName, new MemoryStream(content));
                if (s3Result.IsSuccess)
                {
                    //var s3Url = string.Format("https://s3.amazonaws.com/{0}/{1}/{2}", buketName, folder, fileName);
                    var s3Url = _s3Service.GetPublicUrl(buketName, s3FileName);
                    return s3Url;
                }
            }
            catch (Exception)
            {
            }

            return "";
        }
    }
}
