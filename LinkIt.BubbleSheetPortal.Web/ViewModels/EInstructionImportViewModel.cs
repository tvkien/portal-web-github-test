using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class EInstructionImportViewModel
    {
        public bool IsAdmin { get; set; }
        public bool CanSelectTeachers { get; set; }
        public bool IsSchoolAdmin { get; set; }
        public bool IsPublisher { get; set; }

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