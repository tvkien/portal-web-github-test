using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class VirtualTestWithOutTestResult
    {
        private string name = string.Empty;

        public int VirtualTestId { get; set; }
        public int AuthorUserId { get; set; }
        public int DistrictId { get; set; }
        public int BankId { get; set; }
        public int? ParentTestID { get; set; }
        public int? OriginalTestID { get; set; }

        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }
    }
}
