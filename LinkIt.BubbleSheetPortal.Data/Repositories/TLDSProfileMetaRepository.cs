using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSProfileMetaRepository : IRepository<TLDSProfileMeta>
    {
        private readonly Table<TLDSProfileMetaEntity> _table;
        private readonly TLDSContextDataContext _tldsContext;

        public TLDSProfileMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            _table = _tldsContext.GetTable<TLDSProfileMetaEntity>();
        }

        public IQueryable<TLDSProfileMeta> Select()
        {
            return _table.Select(x => new TLDSProfileMeta
            {
                TLDSProfileMetaID = x.TLDSProfileMetaID,
                TLDSProfileID = x.TLDSProfileID,
                MetaName = x.MetaName,
                MetaValue = x.MetaValue
            });
        }

        public void Save(TLDSProfileMeta item)
        {
            var entity = _table.FirstOrDefault(x => x.TLDSProfileID == item.TLDSProfileID && x.MetaName == item.MetaName);
            if (entity == null)
            {
                entity = new TLDSProfileMetaEntity();
                _table.InsertOnSubmit(entity);
            }

            entity.TLDSProfileID = item.TLDSProfileID;
            entity.MetaName = item.MetaName;
            entity.MetaValue = item.MetaValue;
            _table.Context.SubmitChanges();
            item.TLDSProfileMetaID = entity.TLDSProfileMetaID;
        }


        public void Delete(TLDSProfileMeta item)
        {
            var entity = _table.FirstOrDefault(x => x.TLDSProfileMetaID == item.TLDSProfileMetaID);

            if (entity != null)
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

       

    }
}
