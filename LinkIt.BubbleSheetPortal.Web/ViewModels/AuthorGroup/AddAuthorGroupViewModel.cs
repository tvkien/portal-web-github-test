using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup
{
    public class AddAuthorGroupViewModel : ValidatableEntity<AddAuthorGroupViewModel>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? StateId { get; set; }
        public int? DistrictId { get; set; }
        public int? SchoolId { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }
        public IEnumerable<SelectListItem> Districts { get; set; }
        public IEnumerable<SelectListItem> Schools { get; set; }

        public AddAuthorGroupViewModel()
        {
            States = new List<SelectListItem>();
            Districts = new List<SelectListItem>();
            Schools = new List<SelectListItem>();
        }

        public bool IsDistrictAdmin { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsTeacher { get; set; }
        public int UserDistrictId { get; set; }
        public int UserSchoolId { get; set; }
        public int UserStateId { get; set; }
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