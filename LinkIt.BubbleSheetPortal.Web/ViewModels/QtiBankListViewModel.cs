using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class QtiBankListViewModel
    {
        public int QtiBankId { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string QtiGroupSet { get; set; }
        public string DistrictNames { get; set; } 
        public string SchoolNames { get; set; } 
    }

    public class QtiBankListViewModelV2
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string QtiGroupSet { get; set; }
        public string DistrictNames { get; set; }
        public string SchoolNames { get; set; }
        public int QtiBankId { get; set; }
    }
}
