using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AlgorithmicScoreService
    {
        private readonly IAlgorithmQTIItemGradingRepository _algorithmicQtiItemRepository;
        private readonly IReadOnlyRepository<AlgorithmicVirtualQuestionGrading> _algorithmicVirtualQuestionRepository;
        public AlgorithmicScoreService(IAlgorithmQTIItemGradingRepository algorithmicQtiItemRepository,
            IReadOnlyRepository<AlgorithmicVirtualQuestionGrading> algorithmicVirtualQuestionRepository)
        {
            _algorithmicQtiItemRepository = algorithmicQtiItemRepository;
            _algorithmicVirtualQuestionRepository = algorithmicVirtualQuestionRepository;
        }

        public void AlgorithmicSaveExpression(int qtiItemId, int virtualQuestionId, string expressionXML, int userId)
        {
            _algorithmicQtiItemRepository.AlgorithmicSaveExpression(qtiItemId, virtualQuestionId, expressionXML, userId);
        }

        public List<AlgorithmicExpression> GetListExpression(int qtiItemId, int? virtualQuestionId)
        {
            if (virtualQuestionId.HasValue && virtualQuestionId.Value > 0)
            {
                var result =
                    _algorithmicVirtualQuestionRepository.Select()
                        .Where(x => x.VirtualQuestionID == virtualQuestionId.Value)
                        .Select(x => new AlgorithmicExpression()
                        {
                            VirtualQuestionAlgorithmID = x.AlgorithmID,
                            Expression = x.Expression,
                            Order = x.Order.Value,
                            PointEarned = x.PointsEarned,
                            Rules = x.Rules
                        }).ToList();
                if (result.Any())
                    return result;
            }

            return _algorithmicQtiItemRepository.Select()
                        .Where(x => x.QTIItemID == qtiItemId)
                        .Select(x => new AlgorithmicExpression()
                        {
                            QtiItemAlgorithmID = x.AlgorithmID,
                            Expression = x.Expression,
                            Order = x.Order.Value,
                            PointEarned = x.PointsEarned,
                            Rules = x.Rules
                        }).ToList();
        }
    }
}