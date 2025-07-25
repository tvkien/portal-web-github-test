using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class TermDistrict
    {
        private string name = string.Empty;

        public int SchoolId { get; set; }
        public int DistrictId { get; set; }
        public int VirtualTestId { get; set; }
        public int ClassId { get; set; }   
        public int StudentId { get; set; }
        public int TeacherId { get; set; }
        public int UserId { get; set; }
        public int DistrictTermId { get; set; }
        public int Active { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }

    public class TermDistrictWithStatus
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Active { get; set; }
    }
}