using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiItemRefObjectService
    {
        private readonly IRepository<QtiItemRefObject> _repository;
        private readonly IQTIItemRepository _qTIItemRepository;

        public QtiItemRefObjectService(IRepository<QtiItemRefObject> repository, IQTIItemRepository qTIItemRepository)
        {
            _repository = repository;
            _qTIItemRepository = qTIItemRepository;
        }

        public IQueryable<QtiItemRefObject> GetAll()
        {
            return _repository.Select();
        }

        public void Save(QtiItemRefObject item)
        {
            _repository.Save(item);
        }

        public void Delete(int qtiItemRefObjectId)
        {
            _repository.Delete(new QtiItemRefObject { QtiItemRefObjectId = qtiItemRefObjectId });
        }

        public void Delete(QtiItemRefObject item)
        {
            _repository.Delete(item);
        }

        public void Assign(int qtiItemId, int qtiRefObjectId)
        {
            if (!_repository.Select().Any(x => x.QtiItemId == qtiItemId && x.QtiRefObjectId == qtiRefObjectId))
            {
                var newAssign = new QtiItemRefObject { QtiItemId = qtiItemId, QtiRefObjectId = qtiRefObjectId };
                _repository.Save(newAssign);
            }
        }

        public void Deassign(int qtiItemId, int qtiRefObjectId)
        {
            var assign =
                _repository.Select().FirstOrDefault(x => x.QtiItemId == qtiItemId && x.QtiRefObjectId == qtiRefObjectId);

            if (assign != null)
            {
                _repository.Delete(assign);
            }
        }

        public IList<int> GetRefObjectIdsByQtiItemIds(string qtiItemIds)
        {
            return _qTIItemRepository.GetRefObjectIdsByQtiItemIds(qtiItemIds);
        }
    }
}
