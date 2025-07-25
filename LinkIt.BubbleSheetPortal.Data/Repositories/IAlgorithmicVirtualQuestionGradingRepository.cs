using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAlgorithmicVirtualQuestionGradingRepository : IRepository<AlgorithmicVirtualQuestionGrading>
    {
        void InsertMultipleRecord(List<AlgorithmicVirtualQuestionGrading> items);
    }
}
