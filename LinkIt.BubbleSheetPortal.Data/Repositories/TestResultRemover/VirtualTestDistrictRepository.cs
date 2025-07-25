using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.GenesisGradeBook;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class VirtualTestDistrictRepository : IVirtualTestDistrictRepository
    {
        private readonly Table<VirtualTestDistrictView> _table;
        private readonly TestDataContext _testDataContext;

        public VirtualTestDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<VirtualTestDistrictView>();
            _testDataContext = TestDataContext.Get(connectionString);
            _testDataContext.CommandTimeout = 300;
        }

        public IQueryable<VirtualTestDistrict> Select()
        {
            return _table.Select(x => new VirtualTestDistrict
            {
                VirtualTestId = x.VirtualTestID,
                DistrictId = x.DistrictID,
                Name = x.TestNameCustom,
                ClassId = x.ClassID ?? 0,
                VirtualTestSourceId = x.VirtualTestSourceID
            });
        }

        /// <summary>
        /// Get VirtualTest by District & UserRole with TestName is custom
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public IQueryable<VirtualTestDistrict> GetVirtualTestDistricts(int districtId, int userId, int roleId, int? schoolId, int? teacherId, int? classId, int? studentId, int isRegrader)
        {
            return _testDataContext.VirtualTestDistrictProc(districtId, userId, roleId, schoolId, classId, teacherId, studentId, isRegrader)
                .Select(x => new VirtualTestDistrict()
                {
                    Name = x.TestNameCustom,
                    VirtualTestId = x.VirtualTestID
                }).AsQueryable();
        }
        /// <summary>
        /// Get VirtualTest by District & UserRole with TestName is custom
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="isRegrader"></param>
        /// <returns></returns>
        public IQueryable<VirtualTestDistrict> GetVirtualTestDistrictsForRegrader(int districtId, int userId, int roleId, int schoolId, int? termId)
        {
            return _testDataContext.GetVirtualTestsForTestRegrader(districtId, userId, roleId, schoolId, termId)
                .Select(x => new VirtualTestDistrict()
                {
                    Name = x.TestNameCustom,
                    VirtualTestId = x.VirtualTestID
                }).AsQueryable();
        }

        /// <summary>
        /// Get VirtualTest with Term is Valid
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public IQueryable<VirtualTestDistrict> GetVirtualTestValidTermByDistrict(int districtId, int userId, int roleId, int? schoolId, int? teacherId, int? classId, int? studentId)
        {
            return _testDataContext.VirtualTestValidTermByDistrictProc(districtId, userId, roleId, schoolId, classId, teacherId, studentId)
                .Select(x => new VirtualTestDistrict()
                {
                    Name = x.TestNameCustom,
                    VirtualTestId = x.VirtualTestID
                }).AsQueryable();
        }

        /// <summary>
        /// Get Class by District & UserRole with ClassName is custom
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="virtualtestId"></param>
        /// <param name="studentId"></param>
        /// <param name="SchoolId"></param>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        public IQueryable<ClassDistrict> GetClassDistrictByRole(int districtId, int userId, int roleId,
            int? virtualtestId, int? studentId, int? SchoolId, int? teacherId, int isRegrader)
        {
            return _testDataContext.ClassDistrictProc(districtId, userId, roleId, virtualtestId, studentId, SchoolId, teacherId, isRegrader)
                .Select(o => new ClassDistrict()
                {
                    ClassId = o.ClassID,
                    DistrictId = o.DistrictID,
                    Name = o.ClassNameCustom
                }).AsQueryable();
        }
        public IQueryable<ClassDistrict> GetClassDistrictByRoleHasTestResult(int districtId, int userId, int roleId, int schoolId, int termId)
        {
            return _testDataContext.GetClassesForTestRegrader(districtId, userId, roleId, schoolId, termId)
                .Select(o => new ClassDistrict()
                {
                    ClassId = o.ClassID,
                    Name = o.Name
                }).AsQueryable();
        }

        public IQueryable<ClassDistrict> GetClassValidTermByDistrictRole(int districtId, int userId, int roleId,
            int? virtualtestId, int? studentId, int? schoolId, int? teacherId)
        {
            return _testDataContext.GetClassByDistrictSchoolTestResult(districtId, userId, roleId, virtualtestId, studentId, schoolId, teacherId)
                .Select(o => new ClassDistrict()
                {
                    ClassId = o.ClassID,
                    DistrictId = o.DistrictID,
                    Name = o.ClassNameCustom
                }).AsQueryable();
        }

        /// <summary>
        /// Get Student By District & User Role with custom StudentName
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public IQueryable<StudentTestResultDistrict> GetStudentDistrictByRole(int districtId, int userId, int roleId, int? schoolId, int? teacherId, int? classId, int? virtualTestId, int isRegrader)
        {
            return _testDataContext.StudentDistrictProc(districtId, userId, roleId, virtualTestId, classId, schoolId, teacherId, isRegrader)
               .Select(o => new StudentTestResultDistrict()
               {
                   StudentId = o.StudentID,
                   DistrictId = o.DistrictID,
                   StudentCustom = o.StudentCustom
               }).AsQueryable();
        }

        public IQueryable<StudentTestResultDistrict> GetStudentValidTermByDistrictAndRole(int districtId, int userId, int roleId,
            int? schoolId, int? teacherId, int? classId, int? virtualTestId)
        {
            return _testDataContext.StudentValidTermByDistrictProc(districtId, userId, roleId, virtualTestId, classId, schoolId, teacherId)
               .Select(o => new StudentTestResultDistrict()
               {
                   StudentId = o.StudentID,
                   DistrictId = districtId,
                   StudentCustom = o.StudentCustom
               }).AsQueryable();
        }

        /// <summary>
        /// Get School have testresult & user role by filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="teacherId"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        /// 
        public IQueryable<SchoolTestResultDistrict> GetSchoolDistrictByRole(int districtId, int userId, int roleId,
            int? teacherId, int? classId, int? studentId, int? virtualTestId, int isRegrader)
        {
            return _testDataContext.SchoolDitrictProc(districtId, userId, roleId, teacherId, classId, studentId, virtualTestId, isRegrader)
              .Select(o => new SchoolTestResultDistrict()
              {
                  SchoolId = o.SchoolID,
                  Name = o.Name
              }).AsQueryable();
        }
        public IQueryable<SchoolTestResultDistrict> GetSchoolDistrictByRoleReGrader(int districtId, int userId, int roleId
            )
        {
            return _testDataContext.GetSchoolsForTestRegrader(districtId, userId, roleId)
              .Select(o => new SchoolTestResultDistrict()
              {
                  SchoolId = (int)o.SchoolID,
                  Name = o.Name
              }).AsQueryable();
        }

        public IQueryable<SchoolTestResultDistrict> GetSchoolValidTermByDistrictAndRole(int districtId, int userId, int roleId,
             int? teacherId, int? classId, int? studentId, int? virtualTestId)
        {
            return _testDataContext.SchoolValidTermByDitrictProc(districtId, userId, roleId, teacherId, classId, studentId, virtualTestId)
              .Select(o => new SchoolTestResultDistrict()
              {
                  SchoolId = o.SchoolID,
                  Name = o.Name
              }).AsQueryable();
        }

        public IQueryable<SchoolTestResultDistrict> GetSchoolValidTermByDistrictForRemoveTestResults(int districtId, int userId, int roleId,
             int? teacherId, int? classId, int? studentId, int? virtualTestId)
        {
            return _testDataContext.SchoolValidTermByDitrictTestResultProc(districtId, userId, roleId, teacherId, classId, studentId, virtualTestId)
              .Select(o => new SchoolTestResultDistrict()
              {
                  SchoolId = o.SchoolID,
                  Name = o.Name
              }).AsQueryable();
        }
        /// <summary>
        /// Get Primary Teacher have test result & User role by filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolid"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public IQueryable<TeacherTestResultDistrict> GetPrimaryTeacherDistrictByRole(int districtId, int userId,
            int roleId, int? schoolid, int? classId, int? studentId, int? virtualTestId, int isRegrader)
        {
            return _testDataContext.PrimaryTeacherDitrictProc(districtId, userId, roleId, schoolid, classId, studentId, virtualTestId, isRegrader)
             .Select(o => new TeacherTestResultDistrict()
             {
                 UserId = o.UserID,
                 UserName = o.UserName
             }).AsQueryable();
        }
        /// <summary>
        /// Get Primary Teacher have test result & User role by filter
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <param name="schoolid"></param>
        /// <param name="classId"></param>
        /// <param name="studentId"></param>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public IQueryable<TeacherTestResultDistrict> GetPrimaryTeacherValidTermByDistrictAndRole(int districtId, int userId, int roleId,
            int? schoolid, int? classId, int? studentId, int? virtualTestId)
        {
            return _testDataContext.PrimaryTeacherValidTermByDitrictProc(districtId, userId, roleId, schoolid, classId, studentId, virtualTestId)
             .Select(o => new TeacherTestResultDistrict()
             {
                 UserId = o.UserID,
                 UserName = o.UserName
             }).AsQueryable();
        }

        public IQueryable<DisplayTestResultDistrict> GetTestResutDistrictProcByRole(int districtId, int userId,
            int roleId, int? schoolid, string teacherName, int? classId, string studentName, int? virtualTestId,
            int termId, string testPeriod, int pageIndex, int pageSize, ref int? totalRecords, string sortColumns,
            int isRegrader, string generalSearch)
        {
            return _testDataContext.DisplayTestResultReGraderProc(districtId, userId, roleId, schoolid, teacherName,
                classId, studentName, virtualTestId, termId, testPeriod, pageIndex, pageSize, ref totalRecords, sortColumns, isRegrader, generalSearch)
                .Select(o => new DisplayTestResultDistrict()
                {
                    TestResultId = o.TestResultID,
                    TestName = o.TestNameCustom,
                    SchoolName = o.SchoolName,
                    TeacherCustom = o.TeacherCustom,
                    ClassNameCustom = o.ClassNameCustom,
                    StudentCustom = o.StudentCustom,
                    ResultDate = o.ResultDate,
                    StudentId = o.StudentID
                }).AsQueryable();
        }

        public IQueryable<TermDistrict> GetTermDistrictByRole(int districtId, int userId, int roleId, int virtualTestId,
            int studentId, int classId, int schoolId, int teacherId, int isRegrader)
        {
            return _testDataContext.TermDistrictProc(districtId, userId, roleId, virtualTestId, studentId, classId, schoolId, teacherId, isRegrader)
               .Select(o => new TermDistrict()
               {
                   DistrictTermId = o.DistrictTermID,
                   Name = o.DistrictTermName
               }).AsQueryable();
        }

        public IQueryable<TermDistrict> GetDistrictTermValidFilterByRole(int districtId, int userId, int roleId, int virtualTestId,
            int studentId, int classId, int schoolId, int teacherId)
        {
            return _testDataContext.TermDistrictValidProc(districtId, userId, roleId, virtualTestId, studentId, classId, schoolId, teacherId)
               .Select(o => new TermDistrict()
               {
                   DistrictTermId = o.DistrictTermID,
                   Name = o.DistrictTermName
               }).AsQueryable();
        }
        public IQueryable<TermDistrict> GetTermDistrictByRoleForRegrader(int districtId, int userId, int roleId, int? classId, int schoolId)
        {
            return _testDataContext.GetDistrictTermsForTestRegrader(districtId, userId, roleId, schoolId, classId)
              .Select(o => new TermDistrict()
              {
                  DistrictTermId = o.DistrictTermID,
                  Name = o.DistrictTermName
              }).AsQueryable();
        }

        public IQueryable<TermDistrict> GetFullDistrictTermValidFilterByRole(int districtId, int userId, int roleId, int virtualTestId,
            int studentId, int classId, int schoolId, int teacherId)
        {
            return _testDataContext.FullTermDistrictValidProc(districtId, userId, roleId, virtualTestId, studentId, classId, schoolId, teacherId)
               .Select(o => new TermDistrict()
               {
                   DistrictTermId = o.DistrictTermID,
                   Name = o.DistrictTermName,
                   Active = o.Active
               }).AsQueryable();
        }

        public IQueryable<TermDistrict> GetFullDistrictTermValidFilterByRoleV2(int districtId, int userId, int roleId, int virtualTestId,
            int studentId, int classId, string schoolIds, int teacherId)
        {
            return _testDataContext.FullTermDistrictValidV2Proc(districtId, userId, roleId, virtualTestId, studentId, classId, schoolIds, teacherId)
               .Select(o => new TermDistrict()
               {
                   DistrictTermId = o.DistrictTermID,
                   Name = o.DistrictTermName,
                   Active = o.Active
               }).AsQueryable();
        }

        public IQueryable<TestResultLog> GetTestResultDetails(string testresultIds)
        {
            return _testDataContext.GetTestResultDetails(testresultIds)
               .Select(x => new TestResultLog()
               {
                   TestResultID = x.TestResultID,
                   VirtualTestID = x.VirtualTestID,
                   StudentID = x.StudentID,
                   TeacherID = x.TeacherID,
                   SchoolID = x.SchoolID,
                   ResultDate = x.ResultDate,
                   CreatedDate = x.CreatedDate,
                   UpdatedDate = x.UpdatedDate,
                   TermID = x.TermID,
                   ClassID = x.ClassID,
                   GradedByID = x.GradedByID,
                   ScoreType = x.ScoreType,
                   ScoreValue = x.ScoreValue,
                   SubmitType = x.SubmitType,
                   DistrictTermID = x.DistrictTermID,
                   UserID = x.UserID,
                   OriginalUserID = x.OriginalUserID,
                   LegacyBatchID = x.LegacyBatchID,
                   BubbleSheetID = x.BubbleSheetID,
                   QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                   DistrictID = x.DistrictID,
                   DistrictName = x.DistrictName,
                   DistrictTermName = x.DistrictTermName,
                   SchoolName = x.SchoolName,
                   UserName = x.Username,
                   ClassName = x.ClassName,
                   StudentFirst = x.FirstName,
                   StudentLast = x.LastName,
                   TestName = x.TestName
               }).AsQueryable();
        }

        public IQueryable<TestResultScoreLog> GetTestResultScores(string listTestresultIds)
        {
            return _testDataContext.GetTestResultScoreForLogByTestResultID(listTestresultIds)
               .Select(x => new TestResultScoreLog()
               {
                   TestResultScoreID = x.TestResultScoreID,
                   TestResultID = x.TestResultID,
                   TookTest = x.TookTest,
                   ScorePercent = x.ScorePercent,
                   ScorePercentage = x.ScorePercentage,
                   ScoreRaw = x.ScoreRaw,
                   ScoreScaled = x.ScoreScaled,
                   ScoreLexile = x.ScoreLexile,
                   ScoreIndex = x.ScoreIndex,
                   UsePercent = x.UsePercent,
                   UsePercentage = x.UsePercentage,
                   UseRaw = x.UseRaw,
                   UseScaled = x.UseScaled,
                   UseLexile = x.UseLexile,
                   UseIndex = x.UseIndex,
                   PointsPossible = x.PointsPossible,
                   AchievementLevel = x.AchievementLevel,
                   UseGradeLevelEquiv = x.UseGradeLevelEquiv,
                   ScoreGradeLevelEquiv = x.ScoreGradeLevelEquiv,
                   Name = x.Name,
                   MetStandard = x.MetStandard,
                   AchieveLevelName = x.AchieveLevelName,
                   ScoreCustomN_1 = x.ScoreCustomN_1,
                   ScoreCustomN_2 = x.ScoreCustomN_2,
                   ScoreCustomN_3 = x.ScoreCustomN_3,
                   ScoreCustomN_4 = x.ScoreCustomN_4,
                   ScoreCustomA_1 = x.ScoreCustomA_1,
                   ScoreCustomA_2 = x.ScoreCustomA_2,
                   ScoreCustomA_3 = x.ScoreCustomA_3,
                   ScoreCustomA_4 = x.ScoreCustomA_4
               }).AsQueryable();
        }

        public IQueryable<TestResultSubScoreLog> GetTestResultSubScores(string listTestResultScoreId)
        {
            return _testDataContext.GetTestResultSubScoreForLog(listTestResultScoreId)
               .Select(x => new TestResultSubScoreLog()
               {
                   TestResultSubScoreID = x.TestResultSubScoreID,
                   TestResultScoreID = x.TestResultScoreID,
                   ScorePercent = x.ScorePercent,
                   ScorePercentage = x.ScorePercentage,
                   ScoreRaw = x.ScoreRaw,
                   ScoreScaled = x.ScoreScaled,
                   ScoreLexile = x.ScoreLexile,
                   UsePercent = x.UsePercent,
                   UsePercentage = x.UsePercentage,
                   UseRaw = x.UseRaw,
                   UseScaled = x.UseScaled,
                   UseLexile = x.UseLexile,
                   PointsPossible = x.PointsPossible,
                   AchievementLevel = x.AchievementLevel,
                   UseGradeLevelEquiv = x.UseGradeLevelEquiv,
                   ScoreGradeLevelEquiv = x.ScoreGradeLevelEquiv,
                   Name = x.Name,
                   MetStandard = x.MetStandard,
                   ScoreCustomN_1 = x.ScoreCustomN_1,
                   ScoreCustomN_2 = x.ScoreCustomN_2,
                   ScoreCustomN_3 = x.ScoreCustomN_3,
                   ScoreCustomN_4 = x.ScoreCustomN_4,
                   ScoreCustomA_1 = x.ScoreCustomA_1,
                   ScoreCustomA_2 = x.ScoreCustomA_2,
                   ScoreCustomA_3 = x.ScoreCustomA_3,
                   ScoreCustomA_4 = x.ScoreCustomA_4
               }).AsQueryable();
        }

        public IQueryable<TestResultProgramLog> GetTestResultProgram(string listTestresultIds)
        {
            return _testDataContext.GetTestResultProgramsByTestResultIDs(listTestresultIds)
               .Select(x => new TestResultProgramLog()
               {
                   TestResultProgramID = x.TestResultProgramID,
                   TestResultID = x.TestResultID,
                   ProgramID = x.ProgramID
               }).AsQueryable();
        }

        public IQueryable<AnswerLog> GetAnswersByTestResultId(string listTestresultIds)
        {
            return _testDataContext.GetAnswerForLogByTestResultID(listTestresultIds)
               .Select(x => new AnswerLog()
               {
                   AnswerID = x.AnswerID,
                   PointsEarned = x.PointsEarned,
                   PointsPossible = x.PointsPossible,
                   WasAnswered = x.WasAnswered,
                   TestResultID = x.TestResultID,
                   VirtualQuestionID = x.VirtualQuestionID,
                   AnswerLetter = x.AnswerLetter,
                   Blocked = x.Blocked,
                   AnswerText = x.AnswerText,
                   BubbleSheetErrorType = x.BubbleSheetErrorType,
                   ResponseIdentifier = x.ResponseIdentifier,
                   AnswerImage = x.AnswerImage,
                   HighlightQuestion = x.HighlightQuestion,
                   HighlightPassage = x.HighlightPassage,
                   Overridden = x.Overridden,
                   UpdatedBy = x.UpdatedBy,
                   UpdatedDate = x.UpdatedDate
               }).AsQueryable();
        }

        public IQueryable<AnswerSubLog> GetAnswerSubsByAnswerId(string listAnswerId)
        {
            return _testDataContext.GetAnswerSubForLogByAnswerID(listAnswerId)
               .Select(x => new AnswerSubLog()
               {
                   AnswerSubID = x.AnswerSubID,
                   AnswerID = x.AnswerID,
                   VirtualQuestionSubID = x.VirtualQuestionSubID,
                   PointsEarned = x.PointsEarned,
                   PointsPossible = x.PointsPossible,
                   AnswerLetter = x.AnswerLetter,
                   AnswerText = x.AnswerText,
                   ResponseIdentifier = x.ResponseIdentifier,
                   Overridden = x.Overridden,
                   UpdatedBy = x.UpdatedBy,
                   UpdatedDate = x.UpdatedDate
               }).AsQueryable();
        }


        public IQueryable<DisplayTestResultDistrictCustom> GetTestResutRetaggedProcByRole(TestResultDataModel model, ref int? totalRecords)
        {

            return _testDataContext.DisplayTestResultRetaggedProc(model.DistrictId, model.UserId, model.RoleId, model.Schoolid, model.TeacherName,
                model.ClassId, model.StudentName, model.VirtualTestId, model.TermId, model.IsShowExported, model.PageIndex, model.PageSize,
                ref totalRecords, model.SortColumns, model.IsStudentInformationSystem, model.DateCompare, model.GeneralSearch)
                .Select(o => new DisplayTestResultDistrictCustom()
                {
                    TestResultId = o.TestResultID,
                    TestName = o.TestNameCustom,
                    SchoolName = o.SchoolName,
                    TeacherCustom = o.TeacherCustom,
                    ClassNameCustom = o.ClassNameCustom,
                    StudentCustom = o.StudentCustom,
                    IsExported = o.IsExported,
                    ResultDate = o.ResultDate.GetValueOrDefault()
                }).AsQueryable();

        }

        public IQueryable<DisplayTestResultDistrictCustom> GetRemoveTestResutProcByRole(RemoveTestResultsDetail input, ref int? totalRecords)
        {

            return _testDataContext.DisplayRemoveTestResultProc(input.DistrictId, input.UserId, input.RoleId, input.SchoolId, input.TeacherName, input.ClassId, input.StudentName, input.VirtualTestId, input.TermId,
                                input.DateCompare, input.StartIndex, input.PageSize,ref totalRecords, input.SortColumns, 0, input.GeneralSearch)
                .Select(o => new DisplayTestResultDistrictCustom()
                {
                    TestResultId = o.TestResultID,
                    TestName = o.TestNameCustom,
                    SchoolName = o.SchoolName,
                    TeacherCustom = o.TeacherCustom,
                    ClassNameCustom = o.ClassNameCustom,
                    StudentCustom = o.StudentCustom,
                    ResultDate = o.ResultDate.GetValueOrDefault()
                }).AsQueryable();

        }

        public IQueryable<DisplayTestResultDistrictCustomV2> GetRemoveTestResutProcByRoleV2(RemoveTestResultsDetailV2 input)
        {
            return _testDataContext.DisplayRemoveTestResultV2Proc(input.DistrictId, input.UserId, input.RoleId, input.SchoolIds, input.CategoryIds, input.GradeIds, input.SubjectNames, input.TermId,
                input.ClassId, input.TeacherName, input.StudentName, input.FromResultDate, input.ToResultDate, input.FromCreatedDate, input.ToCreatedDate, input.FromUpdatedDate, input.ToUpdatedDate,
                                input.VirtualTestName, input.StartIndex, input.PageSize, input.SortColumns, input.GeneralSearch, input.HasStudentGeneralSearch)
                .Select(o => new DisplayTestResultDistrictCustomV2()
                {
                    TestResultID = o.TestResultID,
                    VirtualTestName = o.VirtualTestName,
                    StudentName = o.StudentName,
                    ClassTermName = o.ClassTermName,
                    ResultDate = o.ResultDate.GetValueOrDefault(),
                    CategoryName = o.CategoryName,
                    GradeName = o.GradeName,
                    SubjectName = o.SubjectName,
                    TotalRecords = o.TotalRecords,
                    TotalVirtualTests = o.TotalVirtualTests,
                    TotalStudents = o.TotalStudents
                }).AsQueryable();
        }

        public IQueryable<DisplayVirtualTestDistrictCustomV2> GetRemoveVirtualTestProcByRoleV2(RemoveVirtualTestDetailV2 input, ref int? totalRecords, ref int? totalStudents, ref int? totalTestResults)
        {
            
            return _testDataContext.DisplayRemoveVirtualTestV2Proc(input.DistrictId, input.UserId, input.RoleId, input.SchoolIds, input.CategoryIds, input.GradeIds, input.SubjectNames, input.TermId,
                input.ClassId, input.TeacherName, input.StudentName, input.FromResultDate, input.ToResultDate, input.FromCreatedDate, input.ToCreatedDate, input.FromUpdatedDate, input.ToUpdatedDate,
                                input.VirtualTestName, input.StartIndex, input.PageSize, ref totalRecords, ref totalStudents, ref totalTestResults, input.SortColumns, input.GeneralSearch)
                .Select(o => new DisplayVirtualTestDistrictCustomV2()
                {
                    VirtualTestID = o.VirtualTestID,
                    VirtualTestName = o.VirtualTestName,
                    CategoryName = o.CategoryName,
                    GradeName = o.GradeName,
                    SubjectName = o.SubjectName,
                    ResultCount = o.ResultCount,
                    TestResultIDList = o.TestResultIDList,
                    StudentNameList = o.StudentNameList,
                    StudentIDList = o.StudentIDList,
                }).AsQueryable();

        }

        public bool? IsExistAutoGradingQueueBeingGraded(string testResultIds)
        {
            return _testDataContext.IsExistGradingQueue(testResultIds).Select(x => x.IsExist).FirstOrDefault();
        }

        public IQueryable<ListItem> GetTestBySchoolAndDistrictTerm(int districtId, int schoolId, int districtTermId, int userId, int roleId)
        {
            return _testDataContext.GetTestBySchoolAndDistrictTerm(districtId, schoolId, districtTermId, userId, roleId)
                .Select(x => new ListItem() {Id= x.VirtualTestID, Name = x.TestNameCustom}).AsQueryable(); ;
        }

        public IQueryable<ListItem> GetAllSubjects()
        {
            return _testDataContext.SubjectEntities.GroupBy(g => g.Name).Select(g =>
            new ListItem
            {
                Id = g.Min(s => s.SubjectID),
                Name = g.Key
            });
        }
    }
}
