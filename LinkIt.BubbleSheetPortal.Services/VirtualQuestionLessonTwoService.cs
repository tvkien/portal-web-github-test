using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualQuestionLessonTwoService
    {
        private readonly IVirtualQuestionLessonTwoRepository _repository;
        private readonly IRepository<LessonTwo> _lessonTwoRepository;

        public VirtualQuestionLessonTwoService(IVirtualQuestionLessonTwoRepository repository, IRepository<LessonTwo> lessonTwoRepository)
        {
            this._repository = repository;
            this._lessonTwoRepository = lessonTwoRepository;
        }

        public IQueryable<VirtualQuestionLessonTwo> Select()
        {
            return _repository.Select();
        }

        public void Save(VirtualQuestionLessonTwo virtualTestData)
        {
            _repository.Save(virtualTestData);
        }
        public void Delete(int virtualQuestionId, int lessonTwoId)
        {
            var item =
                _repository.Select().FirstOrDefault(
                    x => x.VirtualQuestionId == virtualQuestionId && x.LessonTwoId == lessonTwoId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public List<LessonTwo> GetMutualOthersOfVirtualQuestions(string virtualQuestionIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
            var itemOthers = _repository.Select().Where(x => idList.Contains(x.VirtualQuestionId)).Select(
                   x => new VirtualQuestionLessonTwo() { VirtualQuestionId = x.VirtualQuestionId, LessonTwoId = x.LessonTwoId, Name = x.Name }).ToList();
            var others = itemOthers.GroupBy(x => x.LessonTwoId).Select(g => g.First()).Select(x => new LessonTwo { LessonTwoID = x.LessonTwoId, Name = x.Name });
            List<LessonTwo> resutl = new List<LessonTwo>();
            foreach (var other in others)
            {
                var count = itemOthers.Where(x => x.LessonTwoId == other.LessonTwoID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(other);
                }
            }

            return resutl;
        }
        public SaveStatusDTO AssignOtherTagForVirtualQuestions(string virtualQuestionIdString, string name)
        {
            var result = new SaveStatusDTO();
            try
            {
                name = HttpUtility.UrlDecode(name);
                //if there's already a other tag, no need to insert
                var other = _lessonTwoRepository.Select().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                int otherId = 0;
                if (other == null)
                {
                    //insert topic
                    var newItem = new LessonTwo();
                    newItem.Name = name;
                    _lessonTwoRepository.Save(newItem);
                    otherId = newItem.LessonTwoID;
                }
                else
                {
                    otherId = other.LessonTwoID;
                }
                if (otherId > 0)
                {
                    result.Id = otherId;
                    List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
                    foreach (var virtualQuestionId in idList)
                    {
                        var item = _repository.Select().FirstOrDefault(x => x.VirtualQuestionId == virtualQuestionId && x.LessonTwoId == otherId);
                        if (item == null)
                        {
                            var tag = new VirtualQuestionLessonTwo();
                            tag.VirtualQuestionId = virtualQuestionId;
                            tag.LessonTwoId = otherId;

                            _repository.Save(tag);
                            if (tag.VirtualQuestionId == 0)
                            {
                                result.Error = "Error inserting tag to question.";
                                return result;
                            }
                        }
                    }
                    return result;
                }
                else
                {
                    result.Error = string.Format("Error inserting new tag.");
                }

            }
            catch (Exception ex)
            {
                result.Error = string.Format("Error inserting new tag: {0}", ex.Message);
            }
            return result;
        }
        public string RemoveOtherTagForVirtualQuestions(string virtualQuestionIdString, int lessonTwoId)
        {
            try
            {
                List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
                foreach (int id in idList)
                {
                    Delete(id, lessonTwoId);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("Error deleting tag: {0}", ex.Message);
            }
            return string.Empty;
        }

        public void CloneVirtualQuestionLessonTwo(List<CloneVirtualQuestion> cloneVirtualQuestions)
        {
            var oldVirtualQuestionIDs = cloneVirtualQuestions.Select(x => x.OldVirtualQuestionID);

            var olds = _repository.Select().Where(o => oldVirtualQuestionIDs.Contains(o.VirtualQuestionId)).ToList();

            if (olds.Count == 0)
            {
                return;
            }

            olds.ForEach(item =>
            {
                var newVirtualQuestionId = cloneVirtualQuestions.FirstOrDefault(x => x.OldVirtualQuestionID == item.VirtualQuestionId).NewVirtualQuestionID;

                item.VirtualQuestionLessonTwoId = 0;
                item.VirtualQuestionId = newVirtualQuestionId;
            });

            _repository.InsertMultipleRecord(olds);
        }
    }
}
