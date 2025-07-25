using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualSectionQuestionRepository : IRepository<VirtualSectionQuestion>
    {
        void InsertMultipleRecord(List<VirtualSectionQuestion> items);
    }
}
