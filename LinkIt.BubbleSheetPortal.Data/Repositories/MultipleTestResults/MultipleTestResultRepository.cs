using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.MultipleTestResults
{
    public class MultipleTestResultRepository : IMultipleTestResultRepository
    {
        readonly TestDataContext _testContext;

        public MultipleTestResultRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testContext = TestDataContext.Get(connectionString);
        }

        public IEnumerable<StudentAssignmentDto> GetStudentAssignment(int virtualTestId, List<int> studentIds, string classIds, string search, string sortColumns, int pageSize)
        {
            var studentIdsStr = string.Join(",", studentIds);
            var students = _testContext.GetStudentAssignments(virtualTestId, studentIdsStr, classIds, search, pageSize, sortColumns).Select(x => new StudentAssignmentDto
            {
                StudentId = x.StudentID ?? 0,
                FullName = x.FullName,
                Code = x.Code,
                StudentIds = x.StudentIDs,
                Type = x.Type,
                QTITestClassAssignmentID = x.QTITestClassAssignmentID ?? 0,
                AssignmentDate = x.AssignmentDate.GetValueOrDefault(),
                ResultDate = x.ResultDate
            });

            return students;
        }
    }
}
