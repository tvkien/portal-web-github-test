using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportDetailPanelDTO
    {
        public NavigatorReportFileDetailDTO FileDetail { get; set; }
        public ReportDetailCountOnlyDTO ReportDetailCountOnly { get; set; }
        public List<NavigatorSchoolFolderDetailDTO> SchoolFolderDetail { get; set; }
        public bool HideManageAccessButton { get; set; }
        public bool KeywordMandatory { get; set; }
    }
}
