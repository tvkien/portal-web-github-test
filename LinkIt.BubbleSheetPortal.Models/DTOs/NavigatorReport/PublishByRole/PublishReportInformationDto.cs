using LinkIt.BubbleSheetPortal.Common;
using System.Collections.Generic;
using System.IO;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole
{
    public class PublishReportInformationDto
    {
        public int NavigatorReportId { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string Breadcrumb { get; set; }
        public string FileType
        {
            get
            {
                string extension = Path.GetExtension(this.ReportName)?.ToLower();
                if (Constanst.EXTENSION_MAP_TO_FILE_TYPE.TryGetValue(extension, out string fileType))
                {
                    return fileType;
                }
                return "file extension invalid";
            }
            set
            {
                ;
            }
        }
    }

}
