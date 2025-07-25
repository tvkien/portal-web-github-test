using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IStudentParentRepository
    {
        IQueryable<GetStudentParentListByDistrictIDResult> GetStudentParentListByDistrictID(int? districtID);
        IQueryable<GetClassesWithDetailByStudentIDResult> GetClassesWithDetailByStudentID(int studentID);
        IQueryable<GetParentsByStudentIDResult> GetParentsByStudentID(int studentID);
        IQueryable<GetStudentsByParentIDResult> GetStudentsByParentID(int parentID);
        IQueryable<GetNotAssignParentsOfStudentResult> GetNotAssignParentsOfStudent(int studentID);
        bool IsParentHasAnyAssociation(int studentID);
    }
}
