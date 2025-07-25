using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup
{
    public class AuthorGroupListViewModel
    {
        public bool IsDistrictAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsTeacher { get; set; }
        public int DistrictId { get; set; }
        public int StateId { get; set; }
        public int SchoolId { get; set; }
        public int CurrentUserId { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public int BankID { get; set; }

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