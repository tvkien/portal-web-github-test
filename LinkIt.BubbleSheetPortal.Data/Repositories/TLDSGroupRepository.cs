using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSGroupRepository : ITLDSGroupRepository
    {
        private readonly Table<TLDSGroupEntity> table;
        private readonly TLDSContextDataContext _tldsContext;
        public TLDSGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            table = _tldsContext.GetTable<TLDSGroupEntity>();
        }

        public void Delete(TLDSGroupDTO item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(TLDSGroupDTO item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSGroupID.Equals(item.TLDSGroupID));
            if (entity == null)
            {
                entity = new TLDSGroupEntity();
                table.InsertOnSubmit(entity);
            }
            entity.TLDSUserMetaID = item.TLDSUserMetaID;
            entity.GroupName = item.GroupName;
            entity.Status = item.Status;
            table.Context.SubmitChanges();
            item.TLDSGroupID = entity.TLDSGroupID;
        }

        public IQueryable<TLDSGroupDTO> Select()
        {
            return table.Select(x => new TLDSGroupDTO
            {
                TLDSGroupID = x.TLDSGroupID,
                TLDSUserMetaID = x.TLDSUserMetaID,
                GroupName = x.GroupName,
                Status = x.Status
            });
        }

        public List<TLDSGroupDTO> GetAlllByTldsUserMetaID(int tldsUserMetaID)
        {
            return _tldsContext.TLDSGroup_GetAll(tldsUserMetaID)
                               .Select(x => new TLDSGroupDTO
                               {
                                   TLDSGroupID = x.TLDSGroupID,
                                   GroupName = x.GroupName,
                                   NumberOfProfile = x.NumberOfProfile.Value,
                                   Status = x.Status
                               })
                               .ToList();
        }

        public bool DeactiveTLDSGroup(int tldsGroupId)
        {
            var query = table.FirstOrDefault(x => x.TLDSGroupID == tldsGroupId);
            if (query != null)
            {
                query.Status = false;
                table.Context.SubmitChanges();                
                return true;
            }
            return false;
        }

        public bool ActiveTLDSGroup(int tldsGroupId)
        {
            var query = table.FirstOrDefault(x => x.TLDSGroupID == tldsGroupId);
            if (query != null)
            {
                query.Status = true;
                table.Context.SubmitChanges();                
                return true;
            }
            return false;
        }
    }
}
