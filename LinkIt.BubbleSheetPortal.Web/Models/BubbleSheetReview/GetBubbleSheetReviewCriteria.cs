using LinkIt.BubbleSheetPortal.Web.Models.DataTable;

namespace LinkIt.BubbleSheetPortal.Web.Models.BubbleSheetReview
{
    public class GetBubbleSheetReviewCriteria : GenericDataTableRequest
    {
        public bool archived { get; set; }
        public int? districtId { get; set; }
        public int? schoolId { get; set; }
        public int? duration { get; set; }

        public string GradeName { get; set; }
        public string SubjectName { get; set; }
        public string BankName { get; set; }
        public string ClassName { get; set; }
        public string TeacherName { get; set; }
        public string TestName { get; set; }
    }
}