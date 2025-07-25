using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BankOrder
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int Order { get; set; }
    }
}