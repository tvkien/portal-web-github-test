using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTIItemAnswerScore
    {
        public int QTIItemAnswerScoreId { get; set; }
        public int QTIItemId { get; set; }
        public string ResponseIdentifier { get; set; }
        public string Answer { get; set; }
        public string Score { get; set; }
    }
}