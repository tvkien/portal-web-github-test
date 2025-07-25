using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualQuestionRepository : IRepository<VirtualQuestionData>
    {
        void InsertMultipleRecord(List<VirtualQuestionData> items);
    }
}
