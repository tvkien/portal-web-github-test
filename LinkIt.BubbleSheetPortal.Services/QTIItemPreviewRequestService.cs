using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.MultiPartExpression;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIItemPreviewRequestService
    {
        private readonly IRepository<QTIItemPreviewRequest> _repository;
        private readonly IRepository<MultiPartExpressionPreview> _multiPartExpressionPreviewRepository;

        public QTIItemPreviewRequestService(IRepository<QTIItemPreviewRequest> repository,
            IRepository<MultiPartExpressionPreview> multiPartExpressionPreviewRepository)
        {
            _repository = repository;
            _multiPartExpressionPreviewRepository = multiPartExpressionPreviewRepository;
        }

        public IQueryable<QTIItemPreviewRequest> Select()
        {
            return _repository.Select();
        }

        public void Save(QTIItemPreviewRequest item)
        {
            _repository.Save(item);
        }

        public IQueryable<MultiPartExpressionPreview> GetMultiPartExpressions()
        {
            return _multiPartExpressionPreviewRepository.Select();
        }

        public void SaveMultiPartExpression(List<MultiPartExpressionPreview> multiPartExpressions, string qtiItemPreviewRequestId)
        {
            foreach (var multiPartExpression in multiPartExpressions)
            {
                multiPartExpression.QTIItemPreviewRequestID = qtiItemPreviewRequestId;
                _multiPartExpressionPreviewRepository.Save(multiPartExpression); 
            }
        }
    }
}
