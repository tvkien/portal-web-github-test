using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class StudentTestResultDistrict
    {
        private string studentCustom = string.Empty;

        public int StudentId { get; set; }
        public int DistrictId { get; set; }
        public int ClassId { get; set; }
        public int VirtualTestId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public int VirtualTestSourceId { get; set; }

        public string StudentCustom
        {
            get { return studentCustom; }
            set { studentCustom = value.ConvertNullToEmptyString(); }
        }
    }
}