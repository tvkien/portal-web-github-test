using System.Linq;
using LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using AutoMapper;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.Enum;
using System.Transactions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentPreference;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Common;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.StudentPreferences
{
    public class StudentPreferenceRepository : IStudentPreferenceRepository
    {
        private readonly Table<StudentPreferencesEntity> _table;
        private readonly Table<StudentPreferenceDetailEntity> _tableDetail;
        private readonly TestDataContext _context;
        private readonly RestrictionDataContext _dbContext;

        public StudentPreferenceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = TestDataContext.Get(connectionString);
            _table = _context.GetTable<StudentPreferencesEntity>();
            _tableDetail = _context.GetTable<StudentPreferenceDetailEntity>();
            _dbContext = RestrictionDataContext.Get(connectionString);
        }

        public StudentPreferencesEntity GetByLevel(string level, int levelID, int? virtualTestID, int? dataSetCategoryID)
        {
            return GetByLevelIds(level, new int[] { levelID }, virtualTestID, dataSetCategoryID).FirstOrDefault();
        }
        public IQueryable<StudentPreferencesEntity> GetByLevelIds(string level, int[] levelIds, int? virtualTestID, int? dataSetCategoryID)
        {
            if (virtualTestID > 0) dataSetCategoryID = 0;
            var preferences = _table
                .Where(m => m.Level == level
            && levelIds.Contains(m.LevelID)
            && m.VirtualTestID.GetValueOrDefault() == virtualTestID.GetValueOrDefault()
            && m.DataSetCategoryID.GetValueOrDefault() == dataSetCategoryID.GetValueOrDefault());
            return preferences;
        }

        public IList<TestTypeDto> GetListTestType(int districtID, int userID, int roleID, int schoolID = 0)
        {
            var result = _context.GetTestTypeByUserId(districtID, userID, roleID, schoolID)
                .Select(m => new TestTypeDto
                {
                    DataSetOriginID = m.DataSetOriginID.GetValueOrDefault(),
                    DataSetCategoryID = m.DataSetCategoryID.GetValueOrDefault(),
                    DataSetCategoryName = m.DataSetCategoryName
                }).ToList();

            return result;
        }

        public void SetStudentPreference(StudentPreferenceDto model)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                   new TransactionOptions
                                   {
                                       IsolationLevel = IsolationLevel.ReadCommitted,
                                       Timeout = TransactionManager.MaximumTimeout
                                   }))
            {
                var preference = _table.FirstOrDefault(m => m.Level == model.Level && m.LevelID == model.LevelID && m.VirtualTestID.GetValueOrDefault() == model.VirtualTestID.GetValueOrDefault() && m.DataSetCategoryID.GetValueOrDefault() == model.DataSetCategoryID.GetValueOrDefault());

                if (preference == null)
                {
                    preference = new StudentPreferencesEntity();
                    Mapper.Map(model, preference);
                    _table.InsertOnSubmit(preference);
                }
                else
                {
                    preference.LastModifiedBy = model.LastModifiedBy;
                    preference.LastModifiedDate = model.LastModifiedDate;
                    preference.VirtualTestID = model.VirtualTestID;
                    preference.DataSetCategoryID = model.DataSetCategoryID;
                }

                _table.Context.SubmitChanges();

                model.Details.ForEach(m => m.StudentPreferenceID = preference.StudentPreferenceID);

                foreach (var item in model.Details)
                {
                    var detail = _tableDetail.FirstOrDefault(x => x.StudentPreferenceID == preference.StudentPreferenceID && x.Name == item.Name);

                    if (detail == null)
                    {
                        detail = new StudentPreferenceDetailEntity();
                        if (preference.DataSetCategoryID.HasValue)
                            detail.Locked = true;
                        _tableDetail.InsertOnSubmit(detail);
                    }

                    if ((preference.DataSetCategoryID.HasValue && preference.DataSetCategoryID.Value > 0) && !item.Value)
                        item.Locked = true;
                    Mapper.Map(item, detail);
                }

                _tableDetail.Context.SubmitChanges();
                scope.Complete();
            }
        }

        public void SetStudentsPreference(List<StudentPreferenceDto> models)
        {
            using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
                                     new TransactionOptions
                                     {
                                         IsolationLevel = IsolationLevel.ReadCommitted,
                                         Timeout = TransactionManager.MaximumTimeout
                                     }))
            {
                var studentPreferencesEntities = _table.Where(x => models.Select(y => y.Level).Contains(x.Level)
                                                                   && models.Select(y => y.LevelID).Contains(x.LevelID)
                                                                   && models.Select(y => y.VirtualTestID.GetValueOrDefault()).Contains(x.VirtualTestID.GetValueOrDefault())
                                                                   && models.Select(y => y.DataSetCategoryID.GetValueOrDefault()).Contains(x.DataSetCategoryID.GetValueOrDefault())).ToList();
                /* Filter again to prevent */
                studentPreferencesEntities = studentPreferencesEntities.Where(x => models.Any(m => m.Level == x.Level
                                                                                                && m.LevelID == x.LevelID
                                                                                                && m.VirtualTestID.GetValueOrDefault() == x.VirtualTestID.GetValueOrDefault()
                                                                                                && m.DataSetCategoryID.GetValueOrDefault() == x.DataSetCategoryID.GetValueOrDefault())).ToList();
                if (studentPreferencesEntities.Count > 0)
                {
                    #region Update
                    /* Update Entities  */
                    studentPreferencesEntities.ForEach(x =>
                    {
                        var model = models.FirstOrDefault(m => m.Level == x.Level
                                                            && m.LevelID == x.LevelID
                                                            && m.VirtualTestID.GetValueOrDefault() == x.VirtualTestID.GetValueOrDefault()
                                                            && m.DataSetCategoryID.GetValueOrDefault() == x.DataSetCategoryID.GetValueOrDefault());
                        if (model != null)
                        {
                            x.LastModifiedBy = model.LastModifiedBy;
                            x.LastModifiedDate = model.LastModifiedDate;
                            x.VirtualTestID = model.VirtualTestID;
                            x.DataSetCategoryID = model.DataSetCategoryID;
                        }
                    });
                    _table.Context.SubmitChanges();

                    /* Get update models */
                    var updateModels = models.Where(x => studentPreferencesEntities.Any(y => y.Level == x.Level
                                                                                            && y.LevelID == x.LevelID
                                                                                            && y.VirtualTestID.GetValueOrDefault() == x.VirtualTestID.GetValueOrDefault()
                                                                                            && y.DataSetCategoryID.GetValueOrDefault() == x.DataSetCategoryID.GetValueOrDefault())).ToList();
                    var updateStudentPreferenceDetailsDto = new List<StudentPreferenceDetailDto>();
                    updateModels.ForEach(x =>
                    {
                        var preference = studentPreferencesEntities.FirstOrDefault(m => m.Level == x.Level
                                                                                    && m.LevelID == x.LevelID
                                                                                    && m.VirtualTestID.GetValueOrDefault() == x.VirtualTestID.GetValueOrDefault()
                                                                                    && m.DataSetCategoryID.GetValueOrDefault() == x.DataSetCategoryID.GetValueOrDefault());

                        if (x.Details != null && x.Details.Count > 0)
                        {
                            var details = x.Details.Select(y => new StudentPreferenceDetailDto
                            {
                                StudentPreferenceID = preference.StudentPreferenceID,
                                Locked = ((preference.DataSetCategoryID.HasValue &&
                                           preference.DataSetCategoryID.Value > 0) && !y.Value) ? true : y.Locked,
                                Name = y.Name,
                                IsConflict = y.IsConflict,
                                IsDisabled = y.IsDisabled,
                                IsDisabledByType = y.IsDisabledByType,
                                IsMissing = y.IsMissing,
                                IsNotShow = y.IsNotShow,
                                Priority = y.Priority,
                                Value = y.Value
                            });
                            updateStudentPreferenceDetailsDto.AddRange(details);
                        }
                    });

                    /* Avoid Exception: The incoming request has too many parameters. The server supports a maximum of 2100 parameters  */
                    if (updateStudentPreferenceDetailsDto.Count >= 500)
                    {
                        int numberOfPage = (int)Math.Ceiling((double)updateStudentPreferenceDetailsDto.Count() / 500);
                        for (int i = 0; i < numberOfPage; i++)
                        {
                            var details = updateStudentPreferenceDetailsDto.Skip(i * 500).Take(500).ToList();
                            UpdateStudentPreferenceDetails(details);
                        }
                    }
                    else
                        UpdateStudentPreferenceDetails(updateStudentPreferenceDetailsDto);

                    #endregion

                    #region Insert 

                    var addNewModels = models.Except(updateModels).ToList();
                    if (addNewModels.Count > 0)
                        AddStudentPreferencesAndStudentPreferenceDetails(addNewModels);

                    #endregion
                }
                else
                    AddStudentPreferencesAndStudentPreferenceDetails(models);

                scope.Complete();
            }
        }

        private void AddStudentPreferencesAndStudentPreferenceDetails(List<StudentPreferenceDto> models)
        {
            var studentPreferencesEntities = new List<StudentPreferencesEntity>();
            Mapper.Map(models, studentPreferencesEntities);
            _table.InsertAllOnSubmit(studentPreferencesEntities);
            _table.Context.SubmitChanges();
            var studentPreferenceDetailsEntities = new List<StudentPreferenceDetailEntity>();
            foreach (var model in models)
            {
                var preference = studentPreferencesEntities.FirstOrDefault(m => m.Level == model.Level
                                                                            && m.LevelID == model.LevelID
                                                                            && m.VirtualTestID.GetValueOrDefault() == model.VirtualTestID.GetValueOrDefault()
                                                                            && m.DataSetCategoryID.GetValueOrDefault() == model.DataSetCategoryID.GetValueOrDefault());
                if (model.Details != null && model.Details.Count > 0)
                {
                    var details = model.Details.Select(x =>
                    {
                        var newEntity = Mapper.Map<StudentPreferenceDetailEntity>(x);
                        newEntity.StudentPreferenceID = preference.StudentPreferenceID;
                        if ((preference.DataSetCategoryID.HasValue && preference.DataSetCategoryID.Value > 0) && !x.Value)
                            newEntity.Locked = true;

                        return newEntity;
                    });

                    studentPreferenceDetailsEntities.AddRange(details);
                }
            }

            _tableDetail.InsertAllOnSubmit(studentPreferenceDetailsEntities);
            _tableDetail.Context.SubmitChanges();
        }

        private void UpdateStudentPreferenceDetails(List<StudentPreferenceDetailDto> models)
        {
            var studentPreferenceIds = models.Select(studentPreferenceDetail => studentPreferenceDetail.StudentPreferenceID).Distinct().ToArray();
            var studentPreferenceDetailNames = models.Select(studentPreferenceDetail => studentPreferenceDetail.Name)
                .Distinct().ToArray();
            var updateStudentPreferenceDetailEntities = _tableDetail.Where(x => studentPreferenceIds.Contains(x.StudentPreferenceID)
                                                                                && studentPreferenceDetailNames.Contains(x.Name)).ToList();
            /* Filter again to prevent */
            updateStudentPreferenceDetailEntities = updateStudentPreferenceDetailEntities.Where(x => models.Any(y => y.StudentPreferenceID == x.StudentPreferenceID && y.Name == x.Name)).ToList();
            updateStudentPreferenceDetailEntities.ForEach(x =>
            {
                var model = models.FirstOrDefault(y => y.StudentPreferenceID == x.StudentPreferenceID && y.Name == x.Name);
                x = Mapper.Map(model, x);
            });

            var updateModels = models.Where(x => updateStudentPreferenceDetailEntities.Any(y => y.StudentPreferenceID == x.StudentPreferenceID
                                                                                    && y.Name == x.Name)).ToList();
            var addNewModels = models.Except(updateModels).ToList();
            if (addNewModels.Count > 0)
            {
                var studentPreferenceDetailsEntities = Mapper.Map<List<StudentPreferenceDetailEntity>>(addNewModels);
                _tableDetail.InsertAllOnSubmit(studentPreferenceDetailsEntities);
            }

            _tableDetail.Context.SubmitChanges();
        }


        //TODO [TestType] Note for after finish tab 2
        public TestForStudentPreferenceResponseDto GetTestForStudentPreferences(StudentPreferenceRequestDto criteria)
        {
            var result = new TestForStudentPreferenceResponseDto();
            int? totalRecord = 0;
            var data = _context.GetTestForStudentPreferences(
                string.IsNullOrEmpty(criteria.Level) ? PreferenceLevel.CLASS : criteria.Level,
                criteria.DistrictID,
                criteria.SchoolID,
                criteria.UserID,
                criteria.RoleID,
                criteria.VirtualTestTypeIds,
                criteria.GradeIDs,
                criteria.SubjectIDs,
                criteria.Visibilities,
                criteria.StartRow,
                criteria.PageSize,
                criteria.GeneralSearch,
                criteria.SortColumn,
                criteria.SortDirection,
                ref totalRecord,
                false,
                string.Empty,
                criteria.ClassIds,
                criteria.ExcludeTestTypes)
            .Select(m => new TestForStudentPreferenceDto
            {
                VirtualTestID = m.VirtualTestID,
                TestName = m.TestName,
                DataSetCategory = m.DataSetCategory,
                DataSetCategoryID = m.DataSetCategoryID,
                Subject = m.Subject,
                Grade = m.Grade,
                ResultCount = m.ResultCount,
                ModifiedDate = m.ModifiedDate,
                ModifiedBy = m.ModifiedBy,
            }).ToList();

            result.Data = data;
            result.TotalRecord = totalRecord ?? 0;

            return result;
        }

        public List<SubjectGradeDto> GetSubjectGradeByUserID(int districtId, int userId, int roleId, int schoolId)
        {
            var data = _context.GetSubjectGradeByUserID(districtId, userId, roleId, schoolId)
                .ToArray()
                .Select(m => new SubjectGradeDto
                {
                    SubjectID = m.SubjectID.GetValueOrDefault(),
                    GradeID = m.GradeId.GetValueOrDefault(),
                    SubjectName = m.SubjectName,
                    GradeName = m.GradeName,
                    GradeOrder = m.GradeOrder.GetValueOrDefault()
                }).ToList();

            return data;
        }

        public IEnumerable<GetAvailableTestTypeGradeAndSubjectForStudentPreferenceResult> GetAvailableTestTypeGradeAndSubjectForStudentPreference(SearchBankCriteria criteria)
        {
            var result =  _context.GetAvailableTestTypeGradeAndSubjectForStudentPreference(criteria.Level, criteria.DistrictId, criteria.SchoolId, criteria.UserId, criteria.UserRole, criteria.ClassIds).ToArray();
            if(criteria.UserRole == (int)RoleEnum.SchoolAdmin || criteria.UserRole == (int)RoleEnum.Teacher)
            {
                var restrictedIds = _dbContext.XLITestRestrictionModuleRoles.Where(x => x.PublishedLevelID == criteria.DistrictId && x.RoleID == criteria.UserRole && x.RestrictedObjectName == Constanst.CATEGORY && x.XLITestRestrictionModuleID == 9).Select(x => x.RestrictedObjectID).ToList();
                if (restrictedIds.Any())
                {
                    result = result.Where(x => !restrictedIds.Contains(x.DataSetCategoryID)).ToArray();
                }
            }
            return result;
        }

        public IEnumerable<ClassDto> GetAssociatedClassesThatHasTestResult(int userId, int? districtId, int schoolId, int roleId, int? virtualTestId = null)
        {
            var classTestResults = _context.GetAssociatedClassesThatHasTestResult_Multiple(districtId, schoolId, userId, roleId);
            var virtualTestIdClassIds = classTestResults.GetResult<VirtualTestIdClassIdDto>().ToArray();
            var classes = classTestResults.GetResult<ClassDto>().ToArray();

            if (virtualTestId.HasValue)
            {
                var validClassIds = virtualTestIdClassIds
                    .Where(c => c.VirtualTestId == virtualTestId.Value)
                    .Select(c => c.ClassId)
                    .ToArray();

                return classes
                    .Where(c => validClassIds.Contains(c.ClassId))
                    .ToArray();
            }
            return classes.ToArray();
        }

        public IEnumerable<DataSetCategoryDTO> GetDataSetCategories(GetDatasetCatogoriesParams catogoriesParams)
        {
            return _context.GetDataSetCategoryForStudentPreference(catogoriesParams.DistrictId, catogoriesParams.UserId, catogoriesParams.RoleId, catogoriesParams.SchoolId)
                            .Select(x => new DataSetCategoryDTO()
                            {
                                DataSetCategoryID = x.DataSetCategoryID.GetValueOrDefault(),
                                DataSetCategoryName = x.DataSetCategoryName,
                                DistrictID = x.DistrictID,
                                DistrictName = x.DistrictName,
                                DisplayName = x.DisplayName
                            });
        }
    }
}
