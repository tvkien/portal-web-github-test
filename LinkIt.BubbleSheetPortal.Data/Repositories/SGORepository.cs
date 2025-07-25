using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGORepository : ISGORepository
    {
        private readonly Table<SGOEntity> table;
        private readonly SGODataContext _context;

        public SGORepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = SGODataContext.Get(connectionString);
            table = SGODataContext.Get(connectionString).GetTable<SGOEntity>();
        }

        public IQueryable<SGOObject> Select()
        {
            return table.Select(x => new SGOObject
            {
                ApproverUserID = x.ApproverUserID,
                CreatedDate = x.CreatedDate,
                Name = x.Name,
                OwnerUserID = x.OwnerUserID,
                SGOID = x.SGOID,
                SGOStatusID = x.SGOStatusID,
                TargetScoreType = x.TargetScoreType,
                UpdatedDate = x.UpdatedDate,
                StartDate = x.StartDate,
                EndDate = x.EndDate,
                SchoolIDs = x.SchoolIDs,
                ClassIDs = x.ClassIDs,
                GradeIDs = x.GradeIDs,
                Version = x.Version,
                IsArchive = x.IsArchive.GetValueOrDefault(),
                DistrictID = x.DistrictID.GetValueOrDefault(),
                Feedback = x.Feedback,
                AdminComment = x.AdminComment,
                TeacherComment = x.TeacherComment,
                EducatorComment = x.EducatorComment,
                GenerateResultDate = x.GenerateResultsDate,
                Type = x.Type,
                RationaleUnstructuredScoring = x.RationaleUnstructuredScoring,
                AttachUnstructuredScoringUrl = x.AttachUnstructuredScoringUrl,
                AttachUnstructuredProgressUrl = x.AttachUnstructuredProgressUrl,
                TotalTeacherSGOScoreCustom = x.TotalTeacherSGOScoreCustom,
                PreparationApprovedDate = x.PreparationApprovedDate,
                SGONote = x.SGONote
            });
        }

        public void Save(SGOObject item)
        {
            var entity = table.FirstOrDefault(x => x.SGOID.Equals(item.SGOID));

            if (entity == null)
            {
                entity = new SGOEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOID = entity.SGOID;
        }
        
        public void Delete(SGOObject item)
        {
            var entity = table.FirstOrDefault(x => x.SGOID.Equals(item.SGOID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void SGOSaveStudentPopulate(SGOPopulateStudentFilter objFilter)
        {
            _context.SGOSaveStudentPopulate(objFilter.SGOId, objFilter.GenderIds, objFilter.RaceIds, objFilter.ProgramIds, objFilter.TermIds, objFilter.ClassIds);
        }

        public List<ListItemExtra> GetStudentSelectedBySogId(int sgoId)
        {
            return _context.SGOGetStudentSelectedBySGOID(sgoId).Select(o => new ListItemExtra()
            {
                Id = o.StudentID,
                Name = o.Name,
                ExtracId = o.ClassID
            }).ToList();
        }

        private void MapModelToEntity(SGOObject model, SGOEntity entity)
        {
            entity.ApproverUserID = model.ApproverUserID;
            entity.CreatedDate = model.CreatedDate;
            entity.Name = model.Name;
            entity.OwnerUserID = model.OwnerUserID;
            entity.SGOStatusID = model.SGOStatusID;
            entity.TargetScoreType = model.TargetScoreType;
            entity.UpdatedDate = model.UpdatedDate;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.SchoolIDs = model.SchoolIDs;
            entity.ClassIDs = model.ClassIDs;
            entity.GradeIDs = model.GradeIDs;
            entity.Version = model.Version;
            entity.IsArchive = model.IsArchive;
            entity.DistrictID = model.DistrictID;
            entity.Feedback = model.Feedback;
            entity.AdminComment = model.AdminComment;
            entity.TeacherComment = model.TeacherComment;
            entity.EducatorComment = model.EducatorComment;
            entity.GenerateResultsDate = model.GenerateResultDate;
            entity.Type = model.Type;
            entity.RationaleUnstructuredScoring = model.RationaleUnstructuredScoring;
            entity.AttachUnstructuredScoringUrl = model.AttachUnstructuredScoringUrl;
            entity.AttachUnstructuredProgressUrl = model.AttachUnstructuredProgressUrl;
            entity.TotalTeacherSGOScoreCustom = model.TotalTeacherSGOScoreCustom;
            entity.PreparationApprovedDate = model.PreparationApprovedDate;
            entity.SGONote = model.SGONote;
        }

        public void SGODeleteSGOGroupWithOut9988BySGOId(int sgoId)
        {
            _context.SGODeleteSGOGroupBySGOIDWithOut9899(sgoId);
        }


        public void SGOSaveStudent(int sgoId, string studentDataPointXML)
        {
            var vResult = GetElement(studentDataPointXML);
            _context.SGOSaveStudent(sgoId, vResult);
        }
        private XElement GetElement(string xml)
        {
            return XElement.Parse(xml);
        }

        public List<SGOStudentScoreInDataPoint> GetStudentScorePreAssessmentLinkIt(int dataPointID)
        {
            return _context.SGOGetStudentScorePreAssessmentLinkIt(dataPointID).ToList()
                .Select(x => new SGOStudentScoreInDataPoint
                {
                    AchievementLevel = x.AchievementLevel.GetValueOrDefault(-1),
                    ScoreLexile = x.ScoreLexile.GetValueOrDefault(-1),
                    ScoreRaw = x.ScoreRaw.GetValueOrDefault(-1),
                    ScoreScaled = x.ScoreScaled.GetValueOrDefault(-1),
                    StudentID = x.StudentID,
                    TotalPointPossible = x.PointsPossible.GetValueOrDefault(-1),
                    TotalQuestion = x.QuestionCount.GetValueOrDefault(-1),
                    ScorePercent = x.PointsPossible.GetValueOrDefault(0) != 0 ? x.ScoreRaw.GetValueOrDefault(0)*100/x.PointsPossible.GetValueOrDefault(0) : 0,
                    ScorePercentage = x.ScorePercentage.GetValueOrDefault(-1),
                    ScoreIndex = x.ScoreIndex.GetValueOrDefault(-1),
                    ScoreCustomN_1 = x.ScoreCustomN_1.GetValueOrDefault(-1),
                    ScoreCustomN_2 = x.ScoreCustomN_2.GetValueOrDefault(-1),
                    ScoreCustomN_3 = x.ScoreCustomN_3.GetValueOrDefault(-1),
                    ScoreCustomN_4 = x.ScoreCustomN_4.GetValueOrDefault(-1),

                }).ToList();
        }

        public List<SGOStudentScoreInDataPoint> GetStudentScorePreAssessmentExternal(int dataPointID)
        {
            return _context.SGOGetStudentScorePreAssessmentExternal(dataPointID).ToList()
                .Select(x => new SGOStudentScoreInDataPoint
                {
                    AchievementLevel = x.AchievementLevel.GetValueOrDefault(-1),
                    ScoreLexile = x.ScoreLexile.GetValueOrDefault(-1),
                    ScoreRaw = x.ScoreRaw.GetValueOrDefault(-1),
                    ScoreScaled = x.ScoreScaled.GetValueOrDefault(-1),
                    StudentID = x.StudentID,
                    TotalPointPossible = x.PointsPossible.GetValueOrDefault(-1),
                    TotalQuestion = -1,
                    ScorePercent = x.PointsPossible.GetValueOrDefault(0) != 0 ? x.ScoreRaw.GetValueOrDefault(0) * 100 / x.PointsPossible.GetValueOrDefault(0) : 0
                }).ToList();
        }

        public List<SGOStudentScoreInDataPoint> GetStudentScorePreAssessmentLegacy(int dataPointID)
        {
            return _context.SGOGetStudentScorePreAssessmentLegacy(dataPointID).ToList()
                .Select(x => new SGOStudentScoreInDataPoint
                {
                    AchievementLevel = x.AchievementLevel.GetValueOrDefault(-1),
                    ScoreLexile = x.ScoreLexile.GetValueOrDefault(-1),
                    ScoreRaw = x.ScoreRaw.GetValueOrDefault(-1),
                    ScoreScaled = x.ScoreScaled.GetValueOrDefault(-1),
                    ScorePercent = x.ScorePercent.GetValueOrDefault(-1),
                    StudentID = x.StudentID,
                    TotalPointPossible = x.PointsPossible.GetValueOrDefault(-1),
                    TotalQuestion = -1,
                    ScorePercentage = x.ScorePercentage.GetValueOrDefault(-1),
                    ScoreIndex = x.ScoreIndex.GetValueOrDefault(-1),
                    ScoreCustomN_1 = x.ScoreCustomN_1.GetValueOrDefault(-1),
                    ScoreCustomN_2 = x.ScoreCustomN_2.GetValueOrDefault(-1),
                    ScoreCustomN_3 = x.ScoreCustomN_3.GetValueOrDefault(-1),
                    ScoreCustomN_4 = x.ScoreCustomN_4.GetValueOrDefault(-1),
                }).ToList();
        }

        public void MoveStudentHasNoGroupToDefaultGroup(int SGOID)
        {
            _context.SGOMoveStudentHasNoGroupToDefaultGroup(SGOID);
        }

        public IQueryable<SGOCustomNew> GetSGOCustom(int districtId, int userId, int pageIndex, int pageSize, ref int? totalRecords,
            string sortColumns, string searchbox, int userRoleId, bool? isArchived, bool? isActive, int? schoolId, int? teacherId, int? reviewerId
            , int? districtTermId, string sgoStatusIds, DateTime? InstructionPeriodFrom, DateTime? InstructionPeriodTo)
        {
            //TODO: archived
            return _context.SGOGetListSGO(districtId, userId, pageIndex, pageSize, ref totalRecords, sortColumns, searchbox, isArchived, userRoleId
                ,isActive, schoolId, teacherId, reviewerId, districtTermId, sgoStatusIds, InstructionPeriodFrom, InstructionPeriodTo)
                .Select(o => new SGOCustomNew()
                {
                    ID = o.SGOID,
                    Name = o.NAME,
                    Teacher = o.Teacher,
                    School = o.School,
                    Grade = o.GradeIDs,
                    Course = o.Course,
                    TotalStudent = o.TotalStudent,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    CreatedDate = o.CreatedDate,
                    EffectiveStatus = o.EffectiveStatus,
                    EffectiveStatusDate = o.EffectiveStatusDate,
                    Version = o.Version.GetValueOrDefault(),
                    IsArchived = o.IsArchived.GetValueOrDefault(),
                    OwnerUserID = o.OwnerUserID.GetValueOrDefault(),
                    ApproverUserID = o.ApproverUserID.GetValueOrDefault(),
                    ApproverName = o.ApproverName
                }).AsQueryable();
        }

        public SGOCustomReport GetSGOCustomById(int sgoId)
        {
            //TODO: archived
            return _context.SGOGetSGOCustomByID(sgoId)
                .Select(o => new SGOCustomReport()
                {
                    ID = o.SGOID,
                    Name = o.Name,
                    Teacher = o.Teacher,
                    School = o.School,
                    Grade = o.GradeIDs,
                    Course = o.Course,
                    TotalStudent = o.TotalStudent,
                    StartDate = o.StartDate,
                    EndDate = o.EndDate,
                    CreatedDate = o.CreatedDate,
                    EffectiveStatus = o.EffectiveStatus,
                    EffectiveStatusDate = o.EffectiveStatusDate,
                    Version = o.Version,
                    IsArchived = o.IsArchive.GetValueOrDefault(),
                    OwnerUserID = o.OwnerUserID,
                    ApproverUserID = o.ApproverUserID.GetValueOrDefault(),
                    ApproverName = o.ApproverName,
                    Feedback = o.Feedback,
                    TeacherComment = o.TeacherComment,
                    AdminComment = o.AdminComment,
                    EducatorComment = o.EducatorComment,
                    DistrictTerms = o.DistrictTerms
                }).FirstOrDefault();
        }

        public void AssignStudentToDataPointBand(int sgoDataPointBandID, string studentIDs)
        {
            _context.SGOAssignStudentToDataPointBand(sgoDataPointBandID, studentIDs);
        }

        public void AssignStudentToGroup(int sgoID, int sgoGroupID, string studentIDs)
        {
            _context.SGOAssignStudentToGroup(sgoID, sgoGroupID, studentIDs);
        }

        public List<SGOAutoGroupStudentData> AutoAssignStudentToGroup(int sgoID, bool includeUpdate, int scoreType)
        {
            return _context.SGOAutoAssignStudentToGroup(sgoID, includeUpdate, scoreType).ToList()
                .Select(x => new SGOAutoGroupStudentData
                {
                    SGOGroupID = x.SGOGroupID.GetValueOrDefault(),
                    AveragePercentScore = x.AveragePercentScore.GetValueOrDefault(),
                    MaxPercentage = x.MaxPercentage.GetValueOrDefault(),
                    MinPercentage = x.MinPercentage.GetValueOrDefault(),
                    SGOStudentID = x.SGOStudentID,
                    StudentID = x.StudentID
                }).ToList();
        }

        public void UpdateGroup(int sgoID, string groupNames)
        {
            _context.SGOUpdateGroup(sgoID, groupNames);
        }

        public void DeleteDataPointBand(string dataPointBandIDs)
        {
            _context.SGODeleteDataPointBand(dataPointBandIDs);
        }

        public IQueryable<ListItem> SGOGetDistictTerm(SGOPopulateStudentFilter obj)
        {
            return _context.SGOGetDistrctTerm(obj.DistrictId, obj.UserId, obj.UserRoleId)
                .Select(o => new ListItem()
                {
                    Id = o.DistricttermID.GetValueOrDefault(),
                    Name = o.NAME
                }).AsQueryable();
        }

        public IQueryable<ListItem> SGOGetGender(SGOPopulateStudentFilter obj)
        {
            return _context.SGOGetGender(obj.DistrictId, obj.UserId, obj.UserRoleId)
                .Select(o => new ListItem()
                {
                    Id = o.GenderID,
                    Name = o.NAME
                }).AsQueryable();
        }

        public IQueryable<ListItem> SGOGetRace(SGOPopulateStudentFilter obj)
        {
            return _context.SGOGetRace(obj.DistrictId, obj.UserId, obj.UserRoleId)
                .Select(o => new ListItem()
                {
                    Id = o.RaceID,
                    Name = o.NAME
                }).AsQueryable();
        }

        public IQueryable<ListItem> SGOGetProgram(SGOPopulateStudentFilter obj)
        {
            return _context.SGOGetProgram(obj.DistrictId, obj.UserId, obj.UserRoleId)
                .Select(o => new ListItem()
                {
                    Id = o.ProgramID,
                    Name = o.NAME
                }).AsQueryable();
        }

        public IQueryable<ListItem> SGOGetClasses(SGOPopulateStudentFilter obj)
        {
            return _context.SGOGetClasses(obj.DistrictId, obj.UserId, obj.UserRoleId, obj.TermIds)
                .Select(o => new ListItem()
                {
                    Id = o.ClassID,
                    Name = o.NAME
                }).AsQueryable();
        }

        public IQueryable<ListItemExtra> SGOGetStudents(SGOPopulateStudentFilter obj)
        {
            return _context.SGOGetStudents(obj.DistrictId, obj.UserId, obj.UserRoleId, obj.ClassIds, obj.GenderIds, obj.RaceIds, obj.ProgramIds)
                .Select(o => new ListItemExtra()
                {
                    Id = o.StudentID,
                    Name = o.Name,
                    ExtracId = o.ClassID
                }).AsQueryable();
        }

        public void PopulateSchoolIdsAndClassIdsBySgoId(int sgoId)
        {
            _context.SGOPopulateColumnSchoolIDsAndClassIDsBySGOId(sgoId);
        }

        public IQueryable<SGOStepObject> GetCompletedList(int sgoId)
        {
            return _context.SGOGetCompletedList(sgoId).Select(o => new SGOStepObject()
            {
                Step = o.StepId.GetValueOrDefault(),
                StatusCompleted = o.IsCompleted.GetValueOrDefault()
            }).AsQueryable();
        }

        public List<SGOScoreAchievmentData> GetScoreToCreateDefaultBandForHistoricalTest(int dataPointId)
        {
            return _context.SGOGetScoreToCreateDefaultBandForHistoricalTest(dataPointId)
                .ToList()
                .Select(x => new SGOScoreAchievmentData
                {
                    AchievementLabelString = x.LabelString ?? string.Empty,
                    AchievementLevel = x.AchievementLevel.GetValueOrDefault(),
                    AchievementValueString = x.valueString ?? string.Empty,
                    ScorePercentage = x.ScorePercentage.GetValueOrDefault(),
                    ScoreRaw = x.ScoreRaw.GetValueOrDefault(),
                    ScoreScaled = x.ScoreScaled.GetValueOrDefault(),
                    ScorePercent = x.ScorePercent.GetValueOrDefault(),
                    ScoreIndex = x.ScoreIndex.GetValueOrDefault(),
                    ScoreLexile = x.ScoreLexile.GetValueOrDefault(),
                    ScoreCustomN_1 = x.ScoreCustomN_1.GetValueOrDefault(),
                    ScoreCustomN_2 = x.ScoreCustomN_2.GetValueOrDefault(),
                    ScoreCustomN_3 = x.ScoreCustomN_3.GetValueOrDefault(),
                    ScoreCustomN_4 = x.ScoreCustomN_4.GetValueOrDefault(),

                }).ToList();
        }

        public int SGOAuthorizeRevision(int sgoId, int userId, int statusId)
        {
            var obj = _context.SGOAuthorizeRevision(sgoId, userId, statusId).FirstOrDefault();
            if (obj != null)
                return obj.NewSGOID.GetValueOrDefault();
            return 0;
        }
        public void SGORelatedDataPoint(int oldDataPointId, int newSgoId)
        {
            _context.SGOCloneRelatedDataPoint(oldDataPointId, newSgoId);
        }

        public List<User> SGOGetAdminsOfUser(int districtID, int userID, int roleID)
        {
            var data = _context.SGOGetAdminsOfUser(districtID, roleID, userID).ToList();
            var result = data.Select(o => new User
            {
                Id = o.UserID.Value,
                LastName = o.LastName,
                FirstName = o.FirstName
            }).ToList();

            return result;
        }

        public List<SGOCalculateScoreResult> GetSGOCalculateScoreResult(int sgoId, int sgoDataPointId)
        {
            return _context.SGOCalculateScoreResult(sgoId, sgoDataPointId).Select(x => new SGOCalculateScoreResult
            {
                Name = x.Name,
                PercentStudentAtTargetScore = x.PercentStudentAtTargetScore,
                SGOGroupID = x.SGOGroupID.GetValueOrDefault(),
                TeacherScore = x.TeacherScore,
                Weight = x.Weight,
                WeightedScore = x.WeightedScore
            }).ToList();
        }

        public bool CheckScoreSGOResults(int sgoId, int datapointId)
        {
            if (sgoId > 0 && datapointId > 0)
            {
                //TODO: call store check exist TestResult of student.
                var v = _context.SGOCheckExistTestResultByDataPoint(sgoId, datapointId).ToList();
                if(v.First() != null && v.First().TotalTestResults > 0)
                    return true;
            }
            return false;
        }      

        public bool CheckScoreTestResultStudent(int sgoId, int datapointId, int virtualTestId)
        {
            var result = _context.SGOCheckScoreTestResult(sgoId, datapointId, virtualTestId).ToList();
            if (result.FirstOrDefault() != null && result.FirstOrDefault().IsPostOrPreAssessmentHaveResult.HasValue)
                return result.FirstOrDefault().IsPostOrPreAssessmentHaveResult.Value;

            return false;
        }
        public bool IsExistStudentHasScoreNull(int sgoId, int datapointId)
        {
            var result = _context.SGOCheckExistStudentNoScore(sgoId, datapointId).FirstOrDefault();
            if (result != null)
                return result.IsExistStudentNoScore ?? false;

            return false;
        }
        public List<SGODataPoint> GetDataPointHasNoBand(int sgoId)
        {
            var lstCustomDataPoint = _context.SGOGetDataPointHasNoBand(sgoId).Select(o => new SGODataPoint()
            {
                SGODataPointID = o.SGODataPointID,
                Name = o.Name,
                Type = o.Type,
                ScoreType = o.ScoreType
            }).ToList();
            return lstCustomDataPoint;
        }
        
         public void PopulateDefaultAttainmentGroup(int sgoId)
        {
            _context.SGOPopulateDefaultAttainmentGroup(sgoId);
        }

        public List<SGOScoringDetail> GetSgoScoringDetail(int sgoId, int? sgoDataPointId)
        {
            return _context.SGOScoringDetail(sgoId, sgoDataPointId)
                .Select(x => new SGOScoringDetail
                {
                    AchievedScore = x.AchievedScore,
                    AchievedTarget = x.AchievedTarget,
                    BasedScore = x.BasedScore,
                    FirstName = x.FirstName,
                    GroupName = x.GroupName,
                    GroupOrder = x.GroupOrder,
                    LastName = x.LastName,
                    PostScore = x.PostScore,
                    SgoStudentId = x.SGOStudentID,
                    StudentId = x.StudentID
                }).ToList();
        }
         public List<ListItemExtra> GetAllStudentsBySogId(int sgoId)
        {
            return _context.SGOGetAllStudentBySGOID(sgoId).Select(o => new ListItemExtra()
            {
                Id = o.StudentID,
                Name = o.Name,
                ExtracId = o.ClassID
            }).ToList();
        }


        public List<SGOStudentScoreInDataPoint> GetStudentScorePreCustomAssessmentLinkIt(int dataPointId, int? virtualTestCustomSubScoreId = 0)
        {
            return _context.SGOGetStudentScoreCustomScoreAssessmentLinkIt(dataPointId, virtualTestCustomSubScoreId).ToList()
                .Select(x => new SGOStudentScoreInDataPoint
                {
                    StudentID = x.StudentID.GetValueOrDefault(),
                    ScoreText = x.ScoreText
                }).ToList();
        }

        public List<SGOReportDataPoint> GetSGOReportDataPoint(int sgoId)
        {

            return _context.SGOReportGetSGODataPoint(sgoId)
                .Select(x => new SGOReportDataPoint
                {
                    GradeName = x.GradeName,
                    Name = x.Name,
                    RationaleGuidance = x.RationaleGuidance,
                    SgoDataPointId = x.SGODataPointID,
                    SubjectName = x.SubjectName,
                    TypeName = x.TypeName,
                    Type = x.Type,
                    ImprovementBasedDataPoint = x.ImprovementBasedDataPoint,
                    ScoreType = x.ScoreType,
                    ScoreTypeName = x.ScoreTypeName,
                    VirtualTestId = x.VirtualTestId
                }).ToList();
        }

        public List<SGOReportDataPointFilter> GetSGOReportDataPointFilter(int sgoId)
        {
            return _context.SGOReportGetDataPointFilter(sgoId)
                .Select(x => new SGOReportDataPointFilter
                {
                    FilterId = x.FilterID,
                    FilterName = x.FilterName,
                    FilterType = x.FilterType,
                    SgoDataPointId = x.SGODataPointID
                }).ToList();
        }

        public IQueryable<SGOExportData> SGOGetFinalAdministrativeSignoffSGO(int userId, DateTime? from, DateTime? to, bool? isArchire, bool? isActive, string sgoStatusIDs)
        {
            return _context.SGOGetFinalAdministrativeSignoffSGO(userId,from, to,isArchire,isActive, sgoStatusIDs)
                .Select(x => new SGOExportData
                             {
                                 Name = x.Name,
                                 StateId = x.StateID,
                                 FirstName = x.NameFirst,
                                 LastName = x.NameLast,
                                 LocalCode = x.Code,
                                 FinalSignoffDate = x.FinalSignoffDate,
                                 SISID = x.SISID,
                                 SchoolName = x.SchoolName,
                                 TargetScore = x.SGOScore,
                                 UserID = x.UserID,
                                 TargetScoreType = x.TargetScoreType,
                                 ScoreCustom = x.TotalTeacherSGOScoreCustom,
                                 Type = x.Type
                             }).AsQueryable();
        }

        public bool GetAccessPermission(int districtId, int userId, int sgoId)
        {
            var checkPermission = _context.SGOGetAccessPermission(districtId, userId, sgoId).FirstOrDefault();
            if (checkPermission != null)
                return checkPermission.HasAccess > 0;

            return false;
        }

        public List<SGOLoggingData> GetFullDataForLogging(int sgoId)
        {
            return _context.SGOGetFullDataForLogging(sgoId)
                .Select(x => new SGOLoggingData
                {
                    TableName = x.TableName,
                    XMLContent = x.XMLContent
                }).ToList();
        }

        public List<SGOGetCandidateClass> GetCandidateClassForReplacement(int sgoId)
        {
            return null;

            //var candidateClasses = _context.SGOGetCandidateClassForReplacement(sgoId)
            //    .Select(x => new SGOGetCandidateClass
            //    {
            //        CandidateClassId = x.CandidateClassID,
            //        CandidateClassName = x.CandidateClassName,
            //        RemovedClassId = x.RemovedClassID
            //    }).ToList().ToList();

            //return candidateClasses;
        }

        public void ApplyCandidateClassForReplacement(int sgoId, int removedClassId, int candidateClassId)
        {
            //_context.SGOApplyCandidateClassForReplacement(sgoId, removedClassId, candidateClassId);
        }
    }
}
