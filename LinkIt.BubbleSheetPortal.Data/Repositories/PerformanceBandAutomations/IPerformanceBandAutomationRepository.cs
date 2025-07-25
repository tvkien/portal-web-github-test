using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.PerformanceBandAutomations;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.PerformanceBandAutomations
{
    public interface IPerformanceBandAutomationRepository
    {
        IEnumerable<GetTestTypeGradeAndSubjectForPBSResult> GetTestTypeGradeAndSubject(TestTypeGradeAndSubjectForPBSFilter criteria);
        IEnumerable<GetTestForPBSResult> GetTestForPBS(TestForPBSFilter criteria);
        IEnumerable<ApplySettingForPBSItemDto> ApplySettingForPBS(ApplySettingForPBSPayload payload);
        IEnumerable<GetPerformanceBandGroupsResult> GetPerformanceBandGroups(int districtId, int dataSetOriginID);
        IEnumerable<ApplySettingForPBSItemDto> RemoveSettingForPBS(ApplySettingForPBSPayload payload);
    }
}
