using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ISubjectRepository : IReadOnlyRepository<Subject>
    {
        List<Subject> GetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole);

        List<Subject> ACTGetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole);

        List<Subject> GetSubjectsForItemSetSaveTest(int gradeId, int districtId, int userId, int userRole);

        List<Subject> GetSubjectSByGradeIdAndAuthor(SearchBankCriteria criteria);

        List<Subject> GetSubjectsByGradeIdAndAuthorOfAllTestType(SearchBankCriteria criteria);
        List<Subject> SATGetSubjectSByGradeId(int gradeId, int districtId, int userId, int userRole);

        List<SubjectOrder> GetSubjectsFormBankByGradeId(int gradeId, int districtId, int userId, int userRole, bool isFromMultiDate, bool usingMultiDate);
        IQueryable<SubjectOrderDistrict> GetSubjectOrders(int districtId);

        Subject GetSubjectByShortName(string shortName, int stateId, string subjectName, string gradeName);
    }
}
