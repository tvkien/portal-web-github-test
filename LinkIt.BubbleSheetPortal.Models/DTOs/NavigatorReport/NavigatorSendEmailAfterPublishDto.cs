using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorSendEmailAfterPublishDto
    {
        private string customNote;

        public PublishResultDto PublishResult { get; set; }
        public bool AlsoSendEmail { get; set; }
        public string[] EmailCC { get; set; }
        public string ExcludeUserIds { get; set; }
        public int RoleId { get; set; }
        public int? DistrictId { get; set; }
        public int UserId { get; set; }
        public string CustomNote
        {
            get => customNote; set
            {
                customNote = (!string.IsNullOrEmpty(value) ?
                                                    Regex.Replace(value, @"\r\n?|\n|\\r\\n?|\\n", "<br>")
                                                    : string.Empty).Replace("\\", "");
            }
        }
        public string[] EmailTo { get; set; }
        public bool IgnoreEmailToCheck { get; set; }
        public string GeneralUrl { get; set; }
    }
}
