using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        List<District> GetSharedProviders(int? districtId);
        void UpdateLessonPath(int lessonId, int lessonFileTypeId, string lessonPath);
        void UpdateGuidePath(int lessonId,string guidePath);
        void DeleteLessonGrade(int lessonId, int gradeId);
        void DeleteLessonGrade(int lessonId);
        List<int> GetAssignedGradeIdList(int lessonId);
        void AssignGradeToLesson(int lessonId, int gradeId);
        void DeleteLessonStateStandard(int lessonId, int masterStandardId);
        void DeleteLessonStateStandard(int lessonId);
        List<int> GetAssignedMasterStandardIdList(int lessonId);
        void AssignMasterStandardToLesson(int lessonId, int masterStandardId);
        void UpdateLesson(int lessonId, int lessonProviderId, int contentTypeId, string lessonName,
                                 string description, int subjectId, string keywords);
    }
}
