using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIRefObjectHistoryService
    {
        private readonly IRepository<QtiRefObjectHistory> _repository;

        public QTIRefObjectHistoryService(
            IRepository<QtiRefObjectHistory> repository)
        {
            _repository = repository;
        }

        public QtiRefObjectHistory GetById(int qtiRefObjectHistoryId)
        {
            return _repository.Select().FirstOrDefault(en => en.QTIRefObjectHistoryId == qtiRefObjectHistoryId);
        }

        public IList<QtiRefObjectHistory> GetListByQtiRefObjectId(int qtiRefObjectId, int numberOfRows)
        {
            return _repository.Select()
                .Where(en => en.QTIRefObjectId == qtiRefObjectId)
                .OrderByDescending(en => en.ChangedDate)
                .Take(numberOfRows)
                .ToArray();
        }

        public void Save(QtiRefObjectHistory item)
        {
            _repository.Save(item);
        }

        public int Count(Func<QtiRefObjectHistory, bool> predicate)
        {
            return _repository.Select().Count(predicate);
        }
    }
}
