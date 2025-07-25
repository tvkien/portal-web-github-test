using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class NextApplicableStudent 
    {
        public int StudentID { get; set; }
        public bool IsLastStudent { get; set; }

        public int VirtualQuestionID { get; set; }
    }
}