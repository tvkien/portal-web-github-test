using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIItemLessonOneService
    {
        private readonly IRepository<QTIItemLessonOne> _repository;

        public QTIItemLessonOneService(IRepository<QTIItemLessonOne> repository)
        {
            this._repository = repository;
        }

        public IQueryable<QTIItemLessonOne> GetAll()
        {
            return _repository.Select();
        }

        public void Save(QTIItemLessonOne item)
        {
            if (item != null)
            {
                _repository.Save(item);
            }
        }


        public void Delete(int QTIItemLessonOneID)
        {
            var item = _repository.Select().FirstOrDefault(x => x.QTIItemLessonOneID == QTIItemLessonOneID);
            if(item!=null)
            {
                _repository.Delete(item);
            }
        }
        public void Delete(int qtiItemId,int lessonOneId)
        {
            var item = _repository.Select().FirstOrDefault(x => x.QTIItemID == qtiItemId && x.LessonOneID == lessonOneId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public List<LessonOne> GetMutualSkillsOfManyQtiItems(string qtiItemIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(qtiItemIdString);
            var itemSkills =
               _repository.Select().Where(x => idList.Contains(x.QTIItemID)).Select(
                   x => new QTIItemLessonOne { QTIItemID = x.QTIItemID, LessonOneID = x.LessonOneID, Name = x.Name }).ToList();
            var skills = itemSkills.GroupBy(x => x.LessonOneID).Select(g => g.First()).Select(x => new LessonOne { LessonOneID = x.LessonOneID, Name = x.Name });
            List<LessonOne> resutl = new List<LessonOne>();
            foreach (var skill in skills)
            {
                var count = itemSkills.Where(x => x.LessonOneID == skill.LessonOneID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(skill);
                }
            }
            return resutl;
        }
      
    }
}
