using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ReportType
    {
        public int ReportTypeId { get; set; }
        public string Name { get; set; }
        public int ReportOrder { get; set; }
        public string DisplayName { get; set; }
    }
}
