using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LessonService
    {
        private readonly ILessonRepository repository;
        

        public LessonService(ILessonRepository repository, IUserManageRepository userManageRepository)
        {
            this.repository = repository;
        }

        public IQueryable<Lesson> GetLessons()
        {
            return repository.Select();
        }
        public List<District> GetSharedProviders(int? districtId)
        {
            return repository.GetSharedProviders(districtId);
        }
        public void Save(Lesson item)
        {
            repository.Save(item);
            
        }
        public void DeleteLesson(Lesson item)
        {
            repository.Delete(item);
        }
        public void UpdateLessonPath(int lessonId,int lessonFileTypeId,string lessonPath)
        {
            repository.UpdateLessonPath(lessonId,lessonFileTypeId,lessonPath);

        }
        public void UpdateGuidePath(int lessonId, string guidePath)
        {
            repository.UpdateGuidePath(lessonId, guidePath);
        }
        public void UpdateLesson(int lessonId, int lessonProviderId, int contentTypeId, string lessonName,
                                 string description, int subjectId, string keywords)
        {
            repository.UpdateLesson(lessonId,lessonProviderId,contentTypeId,lessonName,description,subjectId,keywords);
        }
        public void DeleteLessonGrade(int lessonId, int gradeId)
        {
            repository.DeleteLessonGrade(lessonId, gradeId);
        }
        public void DeleteLessonGrade(int lessonId)
        {
            repository.DeleteLessonGrade(lessonId);
        }
        public List<int> GetAssignedGradeIdList(int lessonId)
        {
            return repository.GetAssignedGradeIdList(lessonId);
        }
        public void AssignGradeToLesson(int lessonId, int gradeId)
        {
            repository.AssignGradeToLesson(lessonId, gradeId);
        }
        public void DeleteLessonStateStandard(int lessonId, int masterStandardId)
        {
            repository.DeleteLessonStateStandard(lessonId, masterStandardId);
        }
        public void DeleteLessonStateStandard(int lessonId)
        {
            repository.DeleteLessonStateStandard(lessonId);
        }
        public List<int> GetAssignedMasterStandardIdList(int lessonId)
        {
            return repository.GetAssignedMasterStandardIdList(lessonId);
        }
        public void AssignMasterStandardToLesson(int lessonId, int masterStandardId)
        {
            repository.AssignMasterStandardToLesson(lessonId, masterStandardId);
        }

    }
}