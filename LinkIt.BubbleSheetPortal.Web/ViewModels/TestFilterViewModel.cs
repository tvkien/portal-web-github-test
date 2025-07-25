using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestFilterViewModel
    {
        public bool IsPublisher { get; set; }
        public bool IsTeacher { get; set; }
        public int UserID { get; set; }
        public int DistrictId { get; set; }
        public bool IsNetworkAdmin { get; set; }

        public List<int> ListDistricIds { get; set; }
        public TestFilterViewModel()
        {
            IsPublisher = false;
            IsTeacher = false;
            IsNetworkAdmin = false;
            UserID = 0;
            DistrictId = 0;
            ListDistricIds = new List<int>();
        }
        public string StrIds
        {
            get
            {
                var ids = string.Empty;
                if (!this.ListDistricIds.Any())
                {
                    return ids;
                }
                ids = this.ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
                return ids.TrimEnd(new[] { ',' });
            }
        }
    }
}
