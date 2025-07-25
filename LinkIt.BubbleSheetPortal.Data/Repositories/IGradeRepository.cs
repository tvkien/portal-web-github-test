using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IGradeRepository:IReadOnlyRepository<Grade>
    {
        List<Grade> GetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId);
        IQueryable<Grade> GetStateSubjectGradeByStateAndSubject(string stateCode, string subject);

        List<Grade> GetGradesByStateID(int stateId);
        List<Grade> ACTGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId);
        List<Grade> GetGradesForItemSetSaveTest(int userId, int districtId, int roleId);

        List<Grade> SATGetGradesByUserIdDistrictIdRoleId(int userId, int districtId, int roleId);

        List<Grade> StudentLookupGetGradesFilter(int userId, int districtId, int roleId);
        List<Grade> GetGradesFormBankByUserIdDistrictIdRoleId(int userId, int districtId, int roleId, bool isFromMultiDate, bool usingMultiDate);
        List<GradeOrder> GetGradeOrders(int districtId);
        List<Grade> GetGradesByUserId(int userId, int districtId, int roleId);
    }
}
