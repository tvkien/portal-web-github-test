using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSUserMetaRepository : IRepository<TLDSUserMeta>
    {
        private readonly Table<TLDSUserMetaEntity> table;

        public TLDSUserMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<TLDSUserMetaEntity>();
        }

        public IQueryable<TLDSUserMeta> Select()
        {
            return table.Select(x => new TLDSUserMeta
            {
                MetaValue = x.MetaValue,
                UserID = x.UserID,
                TLDSUserMetaID = x.TLDSUserMetaID
            });
        }

        public void Save(TLDSUserMeta item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSUserMetaID.Equals(item.TLDSUserMetaID));

            if (entity.IsNull())
            {
                entity = new TLDSUserMetaEntity();
                table.InsertOnSubmit(entity);
            }

            BindEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.TLDSUserMetaID = entity.TLDSUserMetaID;
        }

        private void BindEntityToItem(TLDSUserMetaEntity entity, TLDSUserMeta item)
        {
            entity.MetaValue = item.MetaValue;
            entity.UserID = item.UserID;
        }

        public void Delete(TLDSUserMeta item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSUserMetaID.Equals(item.TLDSUserMetaID));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
