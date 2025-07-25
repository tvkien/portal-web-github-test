using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIItemLessonTwoService
    {
        private readonly IRepository<QTIItemLessonTwo> _repository;

        public QTIItemLessonTwoService(IRepository<QTIItemLessonTwo> repository)
        {
            this._repository = repository;
        }

        public IQueryable<QTIItemLessonTwo> GetAll()
        {
            return _repository.Select();
        }

        public void Save(QTIItemLessonTwo item)
        {
            if (item != null)
            {
                _repository.Save(item);
            }
        }


        public void Delete(int QTIItemLessonTwoID)
        {
            var item = _repository.Select().FirstOrDefault(x => x.QTIItemLessonTwoID == QTIItemLessonTwoID);
            if(item!=null)
            {
                _repository.Delete(item);
            }
        }
        public void Delete(int qtiItemId, int lessonTwoId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.QTIItemID == qtiItemId && x.LessonTwoID == lessonTwoId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public List<LessonTwo> GetMutualOthersOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(qtiItemIdString);
            var itemOthers =
               _repository.Select().Where(x => idList.Contains(x.QTIItemID)).Select(
                   x => new QTIItemLessonTwo { QTIItemID = x.QTIItemID, LessonTwoID = x.LessonTwoID, Name = x.Name }).ToList();
            var others = itemOthers.GroupBy(x => x.LessonTwoID).Select(g => g.First()).Select(x => new LessonTwo { LessonTwoID = x.LessonTwoID, Name = x.Name });
            List<LessonTwo> resutl = new List<LessonTwo>();
            foreach (var other in others)
            {
                var count = itemOthers.Where(x => x.LessonTwoID == other.LessonTwoID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(other);
                }
            }

            return resutl;
        }
       
    }
}
