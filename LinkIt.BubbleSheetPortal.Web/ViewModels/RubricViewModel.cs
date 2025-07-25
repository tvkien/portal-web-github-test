using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class RubricViewModel
    {
        public bool IsAdmin { get; set; }
        public bool IsSchoolAdminOrTeacher { get; set; }
        public bool IsPublisher { get; set; }
        public int CurrentDistrictId { get; set; }
        public bool IsNetworkAdmin { get; set; }

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
    }
}