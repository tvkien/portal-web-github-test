using System;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOAuditTrailRepository : ISGOAuditTrailRepository
    {
        private readonly Table<SGOAuditTrailEntity> table;
        private readonly SGODataContext _context;

        public SGOAuditTrailRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = SGODataContext.Get(connectionString);
            table = SGODataContext.Get(connectionString).GetTable<SGOAuditTrailEntity>();
        }

        public IQueryable<SGOAuditTrailData> Select()
        {
            return table.Select(x => new SGOAuditTrailData
                                     {
                                         SGOID = x.SGOID,
                                         ChangedOn = x.ChangedOn,
                                         ActionDetail = x.ActionDetail,
                                         SGOAuditTrailID = x.SGOAuditTrailID,
                                         ChagedByUserID = x.ChagedByUserID,
                                         SGOActionTypeID = x.SGOActionTypeID
                                     });
        }

        public void Save(SGOAuditTrailData item)
        {
            var entity = table.FirstOrDefault(x => x.SGOAuditTrailID.Equals(item.SGOAuditTrailID));

            if (entity == null)
            {
                entity = new SGOAuditTrailEntity();
                table.InsertOnSubmit(entity);
            }

            item.ChangedOn = DateTime.UtcNow;
            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOID = entity.SGOID;
        }

        public void Delete(SGOAuditTrailData item)
        {
            var entity = table.FirstOrDefault(x => x.SGOAuditTrailID.Equals(item.SGOAuditTrailID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOAuditTrailData model, SGOAuditTrailEntity entity)
        {
            entity.ActionDetail = model.ActionDetail;
            entity.ChagedByUserID = model.ChagedByUserID;
            entity.ChangedOn = model.ChangedOn;
            entity.SGOActionTypeID = model.SGOActionTypeID;
            entity.SGOID = model.SGOID;
        }

        public IQueryable<SGOAuditTrailSearchItem> GetAuditTrailBySGOID(int sgoID)
        {
            var data = _context.SGOGetAuditTrailBySGOID(sgoID).ToList();
            var result = data.AsQueryable().Select(o => new SGOAuditTrailSearchItem
            {
                LastName = o.LastName,
                FirstName = o.FirstName,
                UserID = o.UserID,
                CreatedDate = o.CreatedDate,
                Details = o.Details,
                SourceOfData = o.SourceOfData,
                ID = o.ID,
                ActionType = o.ActionType
            });

            return result;
        }
    }
}