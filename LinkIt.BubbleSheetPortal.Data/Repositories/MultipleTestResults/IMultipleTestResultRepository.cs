using LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.MultipleTestResults
{
    public interface IMultipleTestResultRepository
    {
        IEnumerable<StudentAssignmentDto> GetStudentAssignment(int virtualTestId, List<int> studentIds, string classIds, string search, string sortColumns, int pageSize);
    }
}
