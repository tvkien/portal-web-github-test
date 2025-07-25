using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class UploadFilesViewModel
    {
        public bool IsPublisherUploading { get; set; }
        public int StateId { get; set; }
        public int DistrictId { get; set; }

        public List<SelectListItem> AvailableStates { get; set; }
        public List<SelectListItem> AvailableDistricts { get; set; }

        public UploadFilesViewModel()
        {
            AvailableStates = new List<SelectListItem>();
            AvailableDistricts = new List<SelectListItem>();
        }
        public bool IsNetworkAdmin { get; set; }
        public List<int> ListDistricIds { get; set; }

        public string StrIds
        {
            get
            {
                var ids = string.Empty;
                if (this.ListDistricIds==null || !this.ListDistricIds.Any())
                {
                    return ids;
                }
                ids = this.ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
                return ids.TrimEnd(new[] { ',' });
            }
        }
    }
}