using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestMaker;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services.TestMaker
{
    public class MultiPartExpressionService
    {
        private readonly IMultiPartQtiItemExpressionRepository _multiPartQtiItemExpressionRepository;
        private readonly IReadOnlyRepository<MultiPartVirtualQuestionExpressionDto> _multiPartVirtualQuestionExpressionRepository;

        public MultiPartExpressionService(IMultiPartQtiItemExpressionRepository multiPartQtiItemExpressionRepository,
                                           IReadOnlyRepository<MultiPartVirtualQuestionExpressionDto> multiPartVirtualQuestionExpressionRepository)
        {
            _multiPartQtiItemExpressionRepository = multiPartQtiItemExpressionRepository;
            _multiPartVirtualQuestionExpressionRepository = multiPartVirtualQuestionExpressionRepository;
        }

        public void SaveExpression(int qtiItemId, int virtualQuestionId, string expressionXML, int userId)
        {
            _multiPartQtiItemExpressionRepository.SaveExpression(qtiItemId, virtualQuestionId, expressionXML, userId);
        }

        public List<MultiPartExpressionDto> GetListExpression(int qtiItemId, int? virtualQuestionId)
        {
            if (virtualQuestionId.HasValue && virtualQuestionId.Value > 0)
            {
                var result =
                    _multiPartVirtualQuestionExpressionRepository.Select()
                        .Where(x => x.VirtualQuestionId == virtualQuestionId.Value)
                        .Select(x => new MultiPartExpressionDto()
                        {
                            MultiPartVirtualQuestionExpressionId = x.MultiPartVirtualQuestionExpressionId,
                            Expression = x.Expression,
                            EnableElements = x.EnableElements,
                            Order = x.Order.Value,
                            Rules = x.Rules
                        }).ToList();

                return result;
            }

            return _multiPartQtiItemExpressionRepository.Select()
                        .Where(x => x.QtiItemId == qtiItemId)
                        .Select(x => new MultiPartExpressionDto()
                        {
                            MultiPartQTIItemExpressionId = x.MultiPartQtiItemExpressionId,
                            Expression = x.Expression,
                            EnableElements = x.EnableElements,
                            Order = x.Order.Value,
                            Rules = x.Rules
                        }).ToList();
        }
    }
}
