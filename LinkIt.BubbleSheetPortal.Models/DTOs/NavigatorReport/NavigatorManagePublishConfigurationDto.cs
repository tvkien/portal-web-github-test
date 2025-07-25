using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorManagePublishConfigurationDto
    {
        public NavigatorConfigurationDTO NavigatorConfiguration { get; set; }
        public List<StudentGrade> StudentGrade { get; set; }
        public List<StudentProgram> StudentProgram { get; set; }
    }
}
