using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassTestResultList
    {
        private string testName = string.Empty;

        public int? ClassId { get; set; }
        public int? TestCount { get; set; }

        public string TestName
        {
            get { return testName; }
            set { testName = value.ConvertNullToEmptyString(); }
        }
    }
}