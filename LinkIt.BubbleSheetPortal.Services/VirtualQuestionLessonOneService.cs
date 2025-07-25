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
    public class VirtualQuestionLessonOneService
    {
        private readonly IVirtualQuestionLessonOneRepository _repository;
        private readonly IRepository<LessonOne> _lessonOneRepository;

        public VirtualQuestionLessonOneService(IVirtualQuestionLessonOneRepository repository, IRepository<LessonOne> lessonOneRepository)
        {
            this._repository = repository;
            this._lessonOneRepository = lessonOneRepository;
        }

        public IQueryable<VirtualQuestionLessonOne> Select()
        {
            return _repository.Select();
        }

        public void Save(VirtualQuestionLessonOne virtualTestData)
        {
            _repository.Save(virtualTestData);
        }
        public void Delete(int virtualQuestionId, int lessonOneId)
        {
            var item =
                _repository.Select().FirstOrDefault(
                    x => x.VirtualQuestionId == virtualQuestionId && x.LessonOneId == lessonOneId);
            if (item != null)
            {
                _repository.Delete(item);
            }
        }
        public List<LessonOne> GetMutualSkillsVirtualQuestions(string virtualQuestionIdString)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
            var itemSkills = _repository.Select().Where(x => idList.Contains(x.VirtualQuestionId)).Select(
                   x => new VirtualQuestionLessonOne() { VirtualQuestionId = x.VirtualQuestionId, LessonOneId = x.LessonOneId, Name = x.Name }).ToList();
            var skills = itemSkills.GroupBy(x => x.LessonOneId).Select(g => g.First()).Select(x => new LessonOne { LessonOneID = x.LessonOneId, Name = x.Name });
            List<LessonOne> resutl = new List<LessonOne>();
            foreach (var skill in skills)
            {
                var count = itemSkills.Where(x => x.LessonOneId == skill.LessonOneID).Count();
                if (count == idList.Count)//the mutal topic
                {
                    resutl.Add(skill);
                }
            }
            return resutl;
        }
        public SaveStatusDTO AssignSkillTagForVirtualQuestions(string virtualQuestionIdString, string name)
        {
            var result = new SaveStatusDTO();
            try
            {
                //if there's already a skill tag, no need to insert
                name = HttpUtility.UrlDecode(name);
                var skill = _lessonOneRepository.Select().Where(x => x.Name.ToLower().Equals(name.ToLower())).FirstOrDefault();
                int skillId = 0;
                if (skill == null)
                {
                    //insert topic
                    var newItem = new LessonOne();
                    newItem.Name = name;
                    _lessonOneRepository.Save(newItem);
                    skillId = newItem.LessonOneID;
                }
                else
                {
                    skillId = skill.LessonOneID;
                }
                if (skillId > 0)
                {
                    result.Id = skillId;
                    List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
                    foreach (var virtualQuestionId in idList)
                    {
                        var item = _repository.Select().FirstOrDefault(
                            x => x.VirtualQuestionId == virtualQuestionId && x.LessonOneId == skillId);
                        if (item == null)
                        {
                            var tag = new VirtualQuestionLessonOne();
                            tag.VirtualQuestionId = virtualQuestionId;
                            tag.LessonOneId = skillId;

                            _repository.Save(tag);
                            if (tag.VirtualQuestionLessonOneId == 0)
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
                    result.Error = "Error inserting new tag.";

                }

            }
            catch (Exception ex)
            {
                result.Error = string.Format("Error inserting new tag: {0}", ex.Message);
            }
            return result;
        }
        public string RemoveSkillTagForVirtualQuestions(string virtualQuestionIdString, int lessonOneId)
        {
            try
            {
                List<int> idList = ServiceUtil.GetIdListFromIdString(virtualQuestionIdString);
                foreach (int id in idList)
                {
                    Delete(id, lessonOneId);
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return string.Format("Error deleting tag: {0}", ex.Message);
            }

        }

        public void CloneVirtualQuestionLessonOne(List<CloneVirtualQuestion> cloneVirtualQuestions)
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

                item.VirtualQuestionLessonOneId = 0;
                item.VirtualQuestionId = newVirtualQuestionId;
            });

            _repository.InsertMultipleRecord(olds);
        }
    }
}
