using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IStudentGradeRepository
    {

        List<StudentGrade> GetGradesStudent(int districtId, int userId, int roleId);
        List<StudentProgram> GetProgramsStudent(int districtId, int userId, int roleId);

        List<StudentGenderGrade> GetStudentsAvailableByFilter(int districtId, string programIdList,
            string gradeIdList, bool showInactive, int userId, int roleId);
        List<StudentGenderGrade> ManageParentGetStudentsAvailableByFilter(int districtId, string programIdList,
           string gradeIdList, bool showInactive, int userId, int roleId);

        void AddManyStudentsToClass(int classId, string studentIdList);

    }
}
