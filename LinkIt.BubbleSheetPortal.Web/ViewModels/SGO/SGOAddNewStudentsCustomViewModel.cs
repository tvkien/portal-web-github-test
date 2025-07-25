using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class SGOAddNewStudentsCustomViewModel
    {
        public int SGOID { get; set; }
        public int DistrictID { get; set; }

        public bool IsPublisherOrNetworkAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }

        public int SGOStatusID { get; set; }
    }
}