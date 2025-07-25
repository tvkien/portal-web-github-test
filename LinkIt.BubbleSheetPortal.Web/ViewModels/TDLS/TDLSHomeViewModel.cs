using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.TDLS
{
    public class TDLSHomeViewModel
    {
        public bool IsPublisher { get; set; }

        public bool IsNetworkAdmin { get; set; }

        public bool IsDistrictAdmin { get; set; }

        public bool IsSchoolAdmin { get; set; }

        public int CurrentDistrictId { get; set; }
        public List<int> ListDistricIds { get; set; }

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

        public string Directions { get; set; }

    }
}