using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITLDSProfileRepository : IRepository<TLDSProfile>
    {
        List<TLDSProfileFilterModel> FilterTLDSProfile(int currentUserId, int? districtId,
            int? createdUserId, int? submittedSchoolID, int? tldsProfileId, int? enrollmentYear, int? tldsGroupID);
        List<TLDSProfileFilterModel> GetTLDSProfilesForSchoolAdmin(int currentUserId, int? districtId,
            int? createdUserId, int? submittedSchoolID, int? tldsProfileId, bool? showArchived, int? enrollmentYear);

        List<LookupStudent> TLDSStudentLookup(LookupStudentCustom obj, int pageIndex, int pageSize,
            ref int? totalRecords, string sortColumns);

        void DeleteProfile(int curentUserId, int districtId, int profileId);
        void RejectProfile(int currentUserId, int districtId, int profileId, string rejectedReason);
        List<ListItem> GetGradesForFilter(int currentUserId, int? districtId, int roleId);

        List<TLDSProfileFilterModel> GetTLDSProfilesForAssiciateToGroup(int currentUserId, int? districtId, int? createdUserId, int? submittedSchoolID, int? tldsProfileId, int? enrollmentYear);
    }
}
