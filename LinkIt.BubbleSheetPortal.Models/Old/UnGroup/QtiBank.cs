using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiBank
    {
        public int QtiBankId { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public int AccessId { get; set; }
        public int? SchoolId { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int? AuthorGroupId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}