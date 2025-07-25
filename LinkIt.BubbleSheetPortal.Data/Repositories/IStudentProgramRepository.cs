using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IStudentProgramRepository : IRepository<StudentProgram>
    {
        IQueryable<StudentProgramManage> GetStudentPrograms();
        List<int> GetActiveStudentsOfProgram(int programId, string date);

        IQueryable<Student> GetUnassignedStudents(int programId, string studentCode, string firstName, string lastName,
            int districtId);
    }
}
