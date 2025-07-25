using System.Collections.Generic;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IBubbleSheetRepository : IRepository<BubbleSheet>
    {
        void ToggleArchiveBubbleSheets(IEnumerable<BubbleSheet> bubbleSheets);
        void UpdateBubbleSheetsWithTicket(IEnumerable<BubbleSheet> bubbleSheets, string ticket);
        void Save(IList<BubbleSheet> listBubbleSheets);
    }
}