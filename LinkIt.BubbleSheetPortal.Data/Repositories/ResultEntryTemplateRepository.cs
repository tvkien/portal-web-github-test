using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ResultEntryTemplateRepository: IResultEntryTemplateRepository
    {
        private DataLockerContextDataContext _dataLockerContext;
        private SGODataContext _sgoDataContext;
        public ResultEntryTemplateRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _sgoDataContext = SGODataContext.Get(connectionString);
            _dataLockerContext = DataLockerContextDataContext.Get(connectionString);
        }
        public IQueryable<ResultEntryTemplateModel> GetTemplates(int userId, int roleId, int districtId, bool archived)
        {
            string strNow = DateTime.UtcNow.ToShortDateString();
            DateTime dtNow = DateTime.UtcNow;

            IQueryable<int> publishedTemplateIds = GetVirtualTestCustomScoreDistrictShareByDistrictId(districtId).Select(o => o.VirtualTestCustomScoreID);

            IQueryable<ResultEntryTemplateModel> lst;
            if ((roleId == (int)Permissions.DistrictAdmin) || (roleId == (int)Permissions.Publisher))
            {
                 lst = _dataLockerContext.VirtualTestCustomScoreViews.Where(o => (o.DistrictID == districtId || publishedTemplateIds.Contains(o.VirtualTestCustomScoreID)) && (archived ? (o.Archived.Equals(0)|| o.Archived.Equals(1)) : o.Archived.Equals(0)))
                    .Select(o => new ResultEntryTemplateModel()
                    {
                        UpdatedDate = o.UpdatedDate,
                        Name = o.Name,
                        VirtualTestCustomScoreID = o.VirtualTestCustomScoreID,
                        CreatedDate = o.CreatedDate,
                        NameFirst = o.NameFirst,
                        NameLast = o.NameLast,
                        TotalVirtualTestAssociated = o.TotalVirtualTestAssociated,
                        PublishedDistricts = GetPublishedDistrictNamesForTemplate(o.VirtualTestCustomScoreID),
                        IsPublished = publishedTemplateIds.Contains(o.VirtualTestCustomScoreID),
                        IsMultiDate = o.IsMultiDate ?? false,
                        Archived = o.Archived
                    });               
            }
            else
            {
                lst = _dataLockerContext.VirtualTestCustomScoreViews.Where(o => (o.AuthorUserID == userId || publishedTemplateIds.Contains(o.VirtualTestCustomScoreID)) && (archived ? (o.Archived.Equals(0) || o.Archived.Equals(1)) : o.Archived.Equals(0)))
                    .Select(o => new ResultEntryTemplateModel()
                    {
                        UpdatedDate = o.UpdatedDate,
                        Name = o.Name,
                        VirtualTestCustomScoreID = o.VirtualTestCustomScoreID,
                        CreatedDate = o.CreatedDate,
                        NameFirst = o.NameFirst,
                        NameLast = o.NameLast,
                        TotalVirtualTestAssociated = o.TotalVirtualTestAssociated,
                        PublishedDistricts = GetPublishedDistrictNamesForTemplate(o.VirtualTestCustomScoreID),
                        IsPublished = publishedTemplateIds.Contains(o.VirtualTestCustomScoreID),
                        IsMultiDate = o.IsMultiDate ?? false,
                        Archived = o.Archived
                    });
               
            }
            return lst;
        }

        public void AddVirtualTestCustomScoreDistrictShares(PublishTemplateDistrictModel templateDistrict)
        {
            var virtualTestCustomScoreDistrictShare = new VirtualTestCustomScoreDistrictShareEntity
            {
                VirtualTestCustomScoreDistrictShareID = templateDistrict.VirtualTestCustomScoreDistrictShareID,
                VirtualTestCustomScoreID = templateDistrict.VirtualTestCustomScoreID,
                DistrictID = templateDistrict.DistrictId,
                CreatedUserID = templateDistrict.CreatedUserId,
                CreatedDate = templateDistrict.CreatedDate
            };

            _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.InsertOnSubmit(virtualTestCustomScoreDistrictShare);
            _dataLockerContext.SubmitChanges();
        }

        public List<District> GetPublishedDistrictsForTemplate(int templateId)
        {
            var districtEntities = _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.Where(vt => vt.VirtualTestCustomScoreID == templateId).Select(vt => vt.DTLDistrictEntity);

            return districtEntities.Select(dt => new District { Id = dt.DistrictID, Name = dt.Name }).ToList();
        }

        public IQueryable<PublishTemplateDistrictModel> GetVirtualTestCustomScoreDistrictShare(int templateId)
        {
            return _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.Where(vt => vt.VirtualTestCustomScoreID == templateId)
                .Select(
                    vt => new PublishTemplateDistrictModel
                    {
                        VirtualTestCustomScoreDistrictShareID = vt.VirtualTestCustomScoreDistrictShareID,
                        VirtualTestCustomScoreID = vt.VirtualTestCustomScoreID,
                        DistrictId = vt.DistrictID,
                        CreatedDate = vt.CreatedDate,
                        CreatedUserId = vt.CreatedUserID,
                        DistrictName = vt.DTLDistrictEntity.Name
                    }
                );
        }

        public IQueryable<PublishTemplateDistrictModel> GetVirtualTestCustomScoreDistrictShareByDistrictId(int districtId)
        {
            return _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.Where(vt => vt.DistrictID == districtId)
                .Select(
                    vt => new PublishTemplateDistrictModel
                    {
                        VirtualTestCustomScoreDistrictShareID = vt.VirtualTestCustomScoreDistrictShareID,
                        VirtualTestCustomScoreID = vt.VirtualTestCustomScoreID,
                        DistrictId = vt.DistrictID,
                        CreatedDate = vt.CreatedDate,
                        CreatedUserId = vt.CreatedUserID
                    }
                );
        }

        public PublishTemplateDistrictModel GetVirtualTestCustomScoreDistrictShareById(int templateDistrictId)
        {
            VirtualTestCustomScoreDistrictShareEntity virtualTestCustomScoreDistrict = _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.FirstOrDefault(vt => vt.VirtualTestCustomScoreDistrictShareID == templateDistrictId);
            var publishTemplateDistrict = new PublishTemplateDistrictModel
            {
                VirtualTestCustomScoreDistrictShareID = virtualTestCustomScoreDistrict.VirtualTestCustomScoreDistrictShareID,
                VirtualTestCustomScoreID = virtualTestCustomScoreDistrict.VirtualTestCustomScoreID,
                DistrictId = virtualTestCustomScoreDistrict.DistrictID,
                CreatedDate = virtualTestCustomScoreDistrict.CreatedDate,
                CreatedUserId = virtualTestCustomScoreDistrict.CreatedUserID
            };

            return publishTemplateDistrict;
        }
        public void DeleteVirtualTestCustomScoreDistrictShare(PublishTemplateDistrictModel templateDistrict)
        {
            var existTemplateDistrict = _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.FirstOrDefault(vt => vt.VirtualTestCustomScoreDistrictShareID == templateDistrict.VirtualTestCustomScoreDistrictShareID);

            _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.DeleteOnSubmit(existTemplateDistrict);
            _dataLockerContext.SubmitChanges();
        }

        public List<string> GetPublishedDistrictNamesForTemplate(int templateId)
        {
            var districts = _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.Where(vt => vt.VirtualTestCustomScoreID == templateId).Select(x => x.DTLDistrictEntity.Name).ToList();
            return districts;
        }
        public List<int> GetPublishedDistrictIdsForTemplate(int templateId)
        {
            var districts = _dataLockerContext.VirtualTestCustomScoreDistrictShareEntities.Where(vt => vt.VirtualTestCustomScoreID == templateId).Select(x => x.DTLDistrictEntity.DistrictID).ToList();
            return districts;
        }
    }
}

