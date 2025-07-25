using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LessonRepository : ILessonRepository
    {
        private readonly Table<LessonViewEntity> tableLessonView;
        private readonly Table<LessonEntity> table;
        private readonly Table<LessonGradeEntity> tableLessonGrade;
        private readonly Table<LessonStateStandardEntity> tableLessonStateStandard;
        private readonly LearningLibraryDataContext _learningLibraryDataContext;

        public LessonRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            tableLessonView = LearningLibraryDataContext.Get(connectionString).GetTable<LessonViewEntity>();
            table = LearningLibraryDataContext.Get(connectionString).GetTable<LessonEntity>();
            tableLessonGrade = LearningLibraryDataContext.Get(connectionString).GetTable<LessonGradeEntity>();
            tableLessonStateStandard = LearningLibraryDataContext.Get(connectionString).GetTable<LessonStateStandardEntity>();
            _learningLibraryDataContext = LearningLibraryDataContext.Get(connectionString);
        }

        public IQueryable<Lesson> Select()
        {
            return tableLessonView.Select(x => new Lesson
                                         {
                                             LessonId = x.LessonID,
                                             LessonName = x.LessonName,
                                             Grade = x.Grade,
                                             //DateCreated = x.DateCreated,
                                             LessonPath = x.LessonPath,
                                             GuidePath = x.GuidePath,
                                             LessonContentTypeId = x.LessonContentTypeID,
                                             LessonType = x.LessonType,
                                             SubjectName = x.SubjectName,
                                             LessonProviderId = x.LessonProviderID,
                                             Provider = x.Provider,
                                             ProviderThumbnail = x.ProviderThumbnail,
                                             SubjectId = x.SubjectID,
                                             Keywords = x.Keywords,
                                             StandardGUIDString = x.StandardGUIDString,
                                             StandardDescriptionString = x.StandardDescriptionString,
                                             StandardSubjectString = x.StandardSubjectString,
                                             LessonFileTypeId = x.LessonFileTypeID,
                                             Description = x.Description,
                                             StandardNumberString = x.StandardNumberString,
                                             GradeOrderString = x.GradeOrderString,
                                             tUserId = x.tUserID
                                         });
        }
        public List<District> GetSharedProviders(int? districtId)
        {
            var result = _learningLibraryDataContext.GetSharedProviders(districtId).Select(x => new District { Id = x.DistrictID,Name = x.Name});
            return result.ToList();
        }
        public void Save(Lesson item)
        {
            var entity = table.FirstOrDefault(x => x.LessonID.Equals(item.LessonId));

            if (entity.IsNull())
            {
                entity = new LessonEntity();
                entity.DateCreated = System.DateTime.UtcNow;
                table.InsertOnSubmit(entity);
            }
            entity.LessonProviderID = item.LessonProviderId;
            entity.Name = item.LessonName;
            entity.LessonFileTypeID = item.LessonFileTypeId;
            entity.SubjectID = item.SubjectId;
            entity.Description = string.IsNullOrEmpty(item.Description)?string.Empty:item.Description;
            entity.Keywords = string.IsNullOrEmpty(item.Keywords) ? string.Empty : item.Keywords;
            entity.LessonPath = item.LessonPath;
            entity.GuidePath = item.GuidePath;
            entity.LessonContentTypeID = item.LessonContentTypeId.Value;
            entity.tUserID = item.tUserId;
            entity.DateUpdated = System.DateTime.UtcNow;
            table.Context.SubmitChanges();
            item.LessonId = entity.LessonID;
        }

        public void Delete(Lesson item)
        {
            var entity = table.FirstOrDefault(x => x.LessonID.Equals(item.LessonId));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

       public void UpdateLessonPath(int lessonId, int lessonFileTypeId, string lessonPath)
       {
           _learningLibraryDataContext.UpdateLessonPath(lessonId, lessonFileTypeId, lessonPath);
       }
       public void UpdateGuidePath(int lessonId, string guidePath)
       {
           _learningLibraryDataContext.UpdateGuidePath(lessonId, guidePath);
       }
       public void UpdateLesson( int lessonId, int lessonProviderId, int contentTypeId, string lessonName, string description,int subjectId, string keywords)
       {
           _learningLibraryDataContext.UpdateLesson(lessonId,lessonProviderId,contentTypeId,lessonName,description,subjectId,keywords);
       }


       public void DeleteLessonGrade(int lessonId, int gradeId)
       {
           _learningLibraryDataContext.DeleteLessonGrade(lessonId, gradeId);
       }
       public void DeleteLessonGrade(int lessonId)
       {
           _learningLibraryDataContext.DeleteLessonGrade(lessonId,null);
       }
       public List<int> GetAssignedGradeIdList(int lessonId)
       {
           return tableLessonGrade.Where(x => x.LessonID == lessonId).Select(x => x.GradeID).ToList();
       }
       public void AssignGradeToLesson(int lessonId, int gradeId)
       {
           var entity = tableLessonGrade.FirstOrDefault(x => x.LessonID.Equals(lessonId) && x.GradeID.Equals(gradeId));

           if (entity.IsNull())
           {
               entity = new LessonGradeEntity();
               tableLessonGrade.InsertOnSubmit(entity);
           }
           entity.LessonID = lessonId;
           entity.GradeID = gradeId;
           tableLessonGrade.Context.SubmitChanges();
       }
       public void DeleteLessonStateStandard(int lessonId, int masterStandardId)
       {
           _learningLibraryDataContext.DeleteLessonStateStandard(lessonId, masterStandardId);
       }
       public void DeleteLessonStateStandard(int lessonId)
       {
           _learningLibraryDataContext.DeleteLessonStateStandard(lessonId, null);
       }
       public List<int> GetAssignedMasterStandardIdList(int lessonId)
       {
           return tableLessonStateStandard.Where(x => x.LessonID == lessonId).Select(x => x.StateStandardID).ToList();
       }
       public void AssignMasterStandardToLesson(int lessonId, int masterStandardId)
       {
           var entity = tableLessonStateStandard.FirstOrDefault(x => x.LessonID.Equals(lessonId) && x.StateStandardID.Equals(masterStandardId));

           if (entity.IsNull())
           {
               entity = new LessonStateStandardEntity();
               tableLessonStateStandard.InsertOnSubmit(entity);
           }
           entity.LessonID = lessonId;
           entity.StateStandardID = masterStandardId;
           tableLessonStateStandard.Context.SubmitChanges();
       }

    }
}