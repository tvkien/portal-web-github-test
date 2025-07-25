using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.ManagePublishing
{
    public class ManagePublishingViewModel
    {
        public NavigatorConfigurationDTO NavigatorConfiguration { get; set; }
        public List<ListItem> Programs { get; set; }
        public List<ListItem> Grades { get; set; }
    }
}
