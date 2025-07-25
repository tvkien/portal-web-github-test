using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class VirtualTestTestResultDistrict
    {
        private string name = string.Empty;

        public int VirtualTestId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int DistrictId { get; set; }
        public int SchoolId { get; set; }
        public int UserId { get; set; }
        public int VirtualTestSourceId { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}