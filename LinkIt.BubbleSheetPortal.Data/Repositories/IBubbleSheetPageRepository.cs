using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBubbleSheetPageRepository: IReadOnlyRepository<BubbleSheetPage>
    {
        IEnumerable<string> SearchSheetPageIdText(string id, int take);
    }
}
