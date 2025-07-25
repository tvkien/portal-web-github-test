using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IItemTagRepository
    {
        IQueryable<ItemTag> Select();
        IList<string> GetSuggestTags(int districtId, string textToSearch);
    }
}
