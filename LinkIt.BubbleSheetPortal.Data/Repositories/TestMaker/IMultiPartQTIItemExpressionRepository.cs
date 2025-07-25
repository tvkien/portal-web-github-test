using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestMaker
{
    public interface IMultiPartQtiItemExpressionRepository: IReadOnlyRepository<MultiPartQtiItemExpressionDto>
    {
        void SaveExpression(int qtiItemId, int virtualQuestionId, string expressionXML, int userId);

        void DuplicateExpression(int oldQtiItemId, int newQtiItemId, int userId);
    }
}
