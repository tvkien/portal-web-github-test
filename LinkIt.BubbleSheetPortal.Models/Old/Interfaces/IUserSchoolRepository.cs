using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Models.Interfaces
{
    public interface IUserSchoolRepository<T> : IRepository<T> where T : class
    {
        IQueryable<T> SelectFromSchoolManagementView();
        IQueryable<T> SelectFromTeacherListView();
        IQueryable<T> GetTeacherBySchooolIdProc(int schoolId, int userId, int roleId, int districtId);
        IQueryable<T> GetTeacherSchoolByTermProc(int schoolId, bool isTeacherHasTerm, int userId, int roleId, string validUserSchoolRoleId, bool isIncludeDistrictAdmin = false);
    }
}
