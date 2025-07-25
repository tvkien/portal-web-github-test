using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQtiBankRepository
    {
        IQueryable<GetQTIBankListResult> GetQtiBankList(int userId, int districtId, string bankName, string author,
                                                        string publishedTo);
        IQueryable<QtiBankPublishedDistrict> GetPublishedDistrict();
        IQueryable<QtiBankPublishedSchool> GetPublishedSchool();

        List<QtiBankCustom> LoadQTIBanks(int currentUserId, int roleId, int districtId,
            bool? hideTeacherBanks = null, bool? hideOtherPeopleBanks = null);
        List<QtiBank> LoadQTIBanksPersonal(int userId, int districtId);
        List<QtiBank> GetOwnerItemBanks(int userId);
        
        DateTime? GetQTIBankModifiedDate(int qtiBankId);
        IQueryable<QtiBank> Select();
        List<QtiBankSchool> GetQtiBankSchoolOfSchools(string schoolIdString);
        List<QtiBankSchool> GetQtiBankSchoolOfSchools(List<int> schoolIdList);
    }
}
