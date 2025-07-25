using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTIItemStateStandardRepository : IRepository<QTIItemStateStandard>
    {
        List<StateStandardSubject> GetStandardSubjects(string stateCode);
        List<StateSubjectGrade> GetGradeByStateCodeAndSubject(string stateCode, string subject);
        List<State> GetStatesQTIItem(int? userId, int? districtId, int? userIdStateForUser);
        List<State> GetStatesQTI3pItem(int? qti3pSourceId, int? userId);
        List<StateStandardSubject> GetStateStandardSubjectsForItemLibraryFilter(string stateCode,int? userId,int? districtId);
        List<StateSubjectGrade> GetGradesByStateAndSubjectForItemLibraryFilter(string stateCode, string subject,int? userId,int? districtId);
        void InsertMultipleRecord(List<QTIItemStateStandard> items);
    }
}
