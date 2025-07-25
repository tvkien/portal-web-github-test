using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualQuestionItemTagRepository : IRepository<VirtualQuestionItemTag>
    {
        void InsertMultipleRecord(List<VirtualQuestionItemTag> items);
    }
}
