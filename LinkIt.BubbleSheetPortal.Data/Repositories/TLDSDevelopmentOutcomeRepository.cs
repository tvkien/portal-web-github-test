using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSDevelopmentOutcomeRepository : IRepository<TLDSDevelopmentOutcome>
    {
        private readonly Table<TLDSDevelopmentOutcomeEntity> table;

        public TLDSDevelopmentOutcomeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSDevelopmentOutcomeEntity>();
        }

        public IQueryable<TLDSDevelopmentOutcome> Select()
        {
            return table.Select(x => new TLDSDevelopmentOutcome
            {
                DevelopmentOutcomeID = x.DevelopmentOutcomeID,
                Name = x.Name,
                DevelopmentOutcomeTypeID = x.DevelopmentOutcomeTypeID,
            });
        }

        public void Save(TLDSDevelopmentOutcome item)
        {
            var entity = table.FirstOrDefault(x => x.DevelopmentOutcomeID.Equals(item.DevelopmentOutcomeID));

            if (entity == null)
            {
                entity = new TLDSDevelopmentOutcomeEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.Name = item.Name;
            entity.DevelopmentOutcomeTypeID = item.DevelopmentOutcomeTypeID;
            table.Context.SubmitChanges();
            item.DevelopmentOutcomeID = entity.DevelopmentOutcomeID;
        }

        public void Delete(TLDSDevelopmentOutcome item)
        {
            var entity = table.FirstOrDefault(x => x.DevelopmentOutcomeID.Equals(item.DevelopmentOutcomeID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
