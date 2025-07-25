using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiBankList
    {
        public int QtiBankId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int AccessId { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? AuthorGroupId { get; set; }
        public string Author { get; set; }
        public string QtiGroupSet { get; set; }
        public string DistrictNames { get; set; } 
        public string SchoolNames { get; set; } 
    }
}