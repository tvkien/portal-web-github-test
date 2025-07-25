using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualQuestionLessonTwoRepository : IRepository<VirtualQuestionLessonTwo>
    {
        void InsertMultipleRecord(List<VirtualQuestionLessonTwo> items);
    }
}
