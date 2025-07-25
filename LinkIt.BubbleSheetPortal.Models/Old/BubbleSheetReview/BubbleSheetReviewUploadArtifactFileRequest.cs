using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetReview
{
    public class BubbleSheetReviewUploadArtifactFileRequest
    {
        public int DistrictID { get; set; }
        public int BubbleSheetID { get; set; }
        public string Ticket { get; set; }
        public int StudentID { get; set; }
        public HttpPostedFileBase PostedFile { get; set; }
    }
}
