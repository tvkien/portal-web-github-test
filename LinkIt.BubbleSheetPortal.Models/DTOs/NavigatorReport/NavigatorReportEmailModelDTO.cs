using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportEmailModelDTO
    {
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> ArrEmailTo { get; set; }
        public List<string> ArrEmailCC { get; set; }
        public List<string> ArrEmailBCC { get; set; }

        public NavigatorReportEmailModelDTO()
        {
            ArrEmailTo = new List<string>();
            ArrEmailCC = new List<string>();
            ArrEmailBCC = new List<string>();
        }
    }
}
