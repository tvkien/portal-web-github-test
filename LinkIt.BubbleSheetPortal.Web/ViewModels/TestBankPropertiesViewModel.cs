using LinkIt.BubbleSheetPortal.Web.Models.RestrictionDTO;
using System;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class TestBankPropertiesViewModel
    {
        public string TestBankName { get; set; }
        public int TestBankId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string GradeName { get; set; }
        public int GradeId { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string AuthorGroup { get; set; }
        public int AuthoGroupId { get; set; }

        public string DistrictPublished { get; set; }
        public int DistrictPublishedId { get; set; }

        public string SchoolPublished { get; set; }
        public int SchoolPublishedId { get; set; }

        public int StateId { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsDistrictAdmin { get; set; }
        public bool IsNetworkAdmin { get; set; }
        public bool PublishedToDistrictDistrictAdminOnly { get; set; }
        public bool Archived { get; set; }
        public RestrictionAccessModel RestrictionAccessList { get; set; }
        public TestBankPropertiesViewModel()
        {
            RestrictionAccessList = new RestrictionAccessModel();
        }
    }
}
