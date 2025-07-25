using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualQuestionLessonOneRepository : IRepository<VirtualQuestionLessonOne>
    {
        void InsertMultipleRecord(List<VirtualQuestionLessonOne> items);
    }
}
