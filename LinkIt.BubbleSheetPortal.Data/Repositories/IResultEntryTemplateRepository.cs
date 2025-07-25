using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DataLocker;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IResultEntryTemplateRepository
    {
        List<int> GetPublishedDistrictIdsForTemplate(int templateId);
        IQueryable<ResultEntryTemplateModel> GetTemplates(int userId, int roleId, int districtId, bool archived);
        void AddVirtualTestCustomScoreDistrictShares(PublishTemplateDistrictModel templateDistrict);
        List<Models.District> GetPublishedDistrictsForTemplate(int templateId);
        IQueryable<PublishTemplateDistrictModel> GetVirtualTestCustomScoreDistrictShare(int templateId);
        PublishTemplateDistrictModel GetVirtualTestCustomScoreDistrictShareById(int templateDistrictId);
        void DeleteVirtualTestCustomScoreDistrictShare(PublishTemplateDistrictModel templateDistrict);
        IQueryable<PublishTemplateDistrictModel> GetVirtualTestCustomScoreDistrictShareByDistrictId(int districtId);
    }
}
