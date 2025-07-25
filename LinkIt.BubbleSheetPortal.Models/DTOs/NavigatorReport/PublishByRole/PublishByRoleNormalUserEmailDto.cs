using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport.PublishByRole
{
    public class PublishByRoleNormalUserEmailDto
    {
        public PublishUserInformationDto UserInfo { get; set; }
        public string[] EmailCC { get; set; }
        public PublishReportInformationDto[] Reports { get; set; }
    }
}
