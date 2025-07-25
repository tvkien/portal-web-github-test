using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IMasterStandardRepository : IReadOnlyRepository<MasterStandard>
    {
        IList<StateSubjectGrade> GetStateSubjectGradeByStateAndSubject(string state, string subject);

        IList<StateStandardSubject> GetStateStandardsByStateCode(string stateCode);

        IList<MasterStandard> GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevelCC(string state, string subject,
            string grade);

        IList<MasterStandard> GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevel(string state, string subject,
            string grade);

        IList<MasterStandard> GetETSStateStandardsByParentGUID(string parentGUID);
        IList<MasterStandard> GetStateStandardsForItemLibraryFilterTopLevel(string state, string subject, string grade, int? userId, int? districtId);
        IList<MasterStandard> GetStateStandardsForItemLibraryFilterTopLevelCC(string state, string subject, string grade, int? userId, int? districtId);
        IList<MasterStandard> GetStateStandardsForItem3pLibraryFilterTopLevel(string state, string subject, string grade, int? qti3pSourceId);
        IList<MasterStandard> GetStateStandardsForItem3pLibraryFilterTopLevelCC(string state, string subject, string grade, int? qti3pSourceId);

        IList<MasterStandard> GetStateStandardsNextLevelForItemLibraryFilter(string guid, string state, string subject, string grade, int? userId, int? districtId);

        IList<MasterStandard> GetStateStandardsNextLevelForItem3pLibraryFilter(string guid, string state,string subject, string grade, int? qti3pSourceId);
        IQueryable<Qti3pItemStandardXml> GetQti3pItemStandardXml();
    }
}