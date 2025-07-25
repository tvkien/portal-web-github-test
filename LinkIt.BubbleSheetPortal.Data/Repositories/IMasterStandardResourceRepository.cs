using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IMasterStandardResourceRepository : IReadOnlyRepository<MasterStandard>
    {
        IList<StateSubjectGrade> GetStateSubjectGradeByStateAndSubject(string state, string subject);

        IList<StateStandardSubject> GetStateStandardsByStateCode(string stateCode);

        IList<MasterStandard> GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevelCC(string state, string subject,
            string grade);

        IList<MasterStandard> GetMasterStandardsByStateCodeAndSubjectAndGradesTopLevel(string state, string subject,
            string grade);

        IList<MasterStandard> GetETSStateStandardsByParentGUID(string parentGUID);
    }
}