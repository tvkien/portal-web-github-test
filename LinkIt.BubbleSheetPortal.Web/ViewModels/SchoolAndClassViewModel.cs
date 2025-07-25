using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class SchoolAndClassViewModel
    {
        public int RoleId { get; set; }
        public int DefaultDistrictId { get; set; }
        public string DefaultDistrictName { get; set; }
        public int UserId { get; set; }
        public string UserLastName { get; set; }
        public int CurrentSelectedDistrictId { get; set; }
        public int CurrentSelectedSchoolId { get; set; }
        public int CurrentSelectedTeacherId { get; set; }
        public int CurrentSelectedClassId { get; set; }

        public bool IsNetworkAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsDistrictAdmin { get; set; }

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