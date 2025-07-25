using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Xml.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIOnlineTestSessionRepository : IQTIOnlineTestSessionRepository
    {
        private readonly Table<QTIOnlineTestSessionEntity> table;
        private readonly StudentDataContext _studentContext;
        private readonly TestDataContext _context;

        public QTIOnlineTestSessionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<QTIOnlineTestSessionEntity>();
            Mapper.CreateMap<QTIOnlineTestSession, QTIOnlineTestSessionEntity>();

            _studentContext = StudentDataContext.Get(connectionString);
            _context = TestDataContext.Get(connectionString);
        }

        public IQueryable<QTIOnlineTestSession> Select()
        {
            return table.Select(x => new QTIOnlineTestSession
            {
                AssignmentGUId = x.AssignmentGUID,
                QTIOnlineTestSessionId = x.QTIOnlineTestSessionID,
                SessionQuestionOrder = x.SessionQuestionOrder,
                StartDate = x.StartDate,
                StatusId = x.StatusID,
                StudentId = x.StudentID,
                TimeOver = x.TimeOver,
                VirtualTestId = x.VirtualTestID
            });
        }

        public void Save(QTIOnlineTestSession item)
        {
            var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionID.Equals(item.QTIOnlineTestSessionId));

            if (entity == null)
            {
                entity = new QTIOnlineTestSessionEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTIOnlineTestSessionId = entity.QTIOnlineTestSessionID;
        }

        public void Delete(QTIOnlineTestSession item)
        {
            var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionID.Equals(item.QTIOnlineTestSessionId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void ReopenTest(int qtiOnlineTestSession, string imageIndexs)
        {
            var imageElement = XElement.Parse(imageIndexs);
            _studentContext.ReopenCompletedOnlineTestSession(qtiOnlineTestSession, imageElement);
        }

        public bool CanReopenTest(int qtiOnlineTestSessionID)
        {
            var singleResult = _studentContext.CanReopenCompletedOnlineTestSession(qtiOnlineTestSessionID);
            if (singleResult == null) return false;

            var data = singleResult.FirstOrDefault();
            if (data == null) return false;

            var result = data.Result.HasValue && data.Result.Value;

            return result;
        }

        public IQueryable<GetTestStudentSessionExportResponse> GetTestStudentSessionsExport(GetTestStudentSessionExportRequest request)
        {
            int? totalRecord = 0;

            var data = _context.GetTestStudentSessionsExport(request.AssignDate, request.OnlyShowPedingReview, request.ShowActiveClassTestAssignment,
                request.UserID, request.DistrictID, request.QtiTestClassAssignmentId, request.SchoolID, request.GradeName, request.SubjectName, request.BankName,
                request.ClassName, request.TeacherName, request.StudentName, request.TestName, request.AssignmentCodes, request.GeneralSearch, request.SortColumn,
                request.SortDirection, request.StartRow, request.PageSize, ref totalRecord).ToList();

            return data.AsQueryable().Select(x => new GetTestStudentSessionExportResponse
            {
                QTITestClassAssignmentId = x.QTITestClassAssignmentID ?? 0,
                AssignmentDate = x.AssignmentDate,
                ClassName = x.ClassName,
                ClassSection = x.ClassSection,
                CourseNumber = x.CourseNumber,
                Grade = x.GradeName,
                SchoolCode = x.SchoolCode,
                SchoolName = x.SchoolName,
                Term = x.Term,
                TestCode = x.TestCode,
                UserCode = x.UserCode,
                UserName = x.UserName,
                TestName = x.TestName,
                Students = x.Students,
                Started = x.Started ?? 0,
                NotStarted = x.NotStarted ?? 0,
                WaitingForReview = x.WaitingForReview ?? 0
            });
        }

        /// <summary>
        /// submitType: 0 Student submit, 1 teacher submit
        /// Return auto grading queue
        /// </summary>
        public SubmitOnlineTestResult SubmitOnlineTest(int qtiOnlineTestSessionID, bool timeOver, string token, int submitType, int? requestUserID)
        {
            var result = _context.SubmitOnlineTest(qtiOnlineTestSessionID, timeOver, token, submitType, requestUserID).FirstOrDefault();
            return result;
        }

        public bool HasExistTestInProgress(int virtualTestId)
        {
            var inprogressStatus = new List<int> {
                (int)QTIOnlineTestSessionStatusEnum.Created,
                (int)QTIOnlineTestSessionStatusEnum.InProgress,
                (int)QTIOnlineTestSessionStatusEnum.Paused,
                (int)QTIOnlineTestSessionStatusEnum.PendingReview
            };
            var query =
                from s in _context.GetTable<QTIOnlineTestSessionEntity>()
                join c in _context.QTITestClassAssignmentEntities on s.AssignmentGUID equals c.AssignmentGUID
                where s.VirtualTestID == virtualTestId && inprogressStatus.Contains(s.StatusID) && c.Type != (int)AssignmentType.TeacherPreview
                select c;
            return query.Any();
        }
    }
}
