using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAlgorithmQTIItemGradingRepository : IRepository<AlgorithmQTIItemGrading>
    {
        void AlgorithmicSaveExpression(int qtiItemId, int virtualQuestionId, string expressionXML, int userId);
    }
}
