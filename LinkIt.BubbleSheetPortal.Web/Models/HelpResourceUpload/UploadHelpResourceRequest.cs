using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.HelpResourceUpload
{
    public class UploadHelpResourceRequest
    {
        public HttpPostedFileBase PostedFile { get; set; }
        public int? HelpResourceCategoryID { get; set; }
        public int? HelpResourceTypeID { get; set; }
        public int? HelpResourceFileTypeID { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public string KeyWords { get; set; }
        public string HelpResourceLink { get; set; }
        public int? HelpResourceID { get; set; }
        public string HelpResourceLinkOrFile { get; set; }
    }
}