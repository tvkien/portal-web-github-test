using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSDevelopmentOutcomeProfileRepository : IRepository<TLDSDevelopmentOutcomeProfile>
    {
        private readonly Table<TLDSDevelopmentOutcomeProfileEntity> table;

        public TLDSDevelopmentOutcomeProfileRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSDevelopmentOutcomeProfileEntity>();
        }

        public IQueryable<TLDSDevelopmentOutcomeProfile> Select()
        {
            return table.Select(x => new TLDSDevelopmentOutcomeProfile
            {
                DevelopmentOutcomeProfileID = x.DevelopmentOutcomeProfileID,
                ProfileID = x.ProfileID,
                DevelopmentOutcomeTypeID = x.DevelopmentOutcomeTypeID,
                DevelopmentOutcomeContent = x.DevelopmentOutcomeContent,
                StrategyContent = x.StrategyContent,
                OriginalFileName = x.OriginalFileName,
                S3FileName = x.S3FileName
            });
        }

        public void Save(TLDSDevelopmentOutcomeProfile item)
        {
            var entity = table.FirstOrDefault(x => x.DevelopmentOutcomeProfileID.Equals(item.DevelopmentOutcomeProfileID)
                                                || (x.ProfileID == item.ProfileID && x.DevelopmentOutcomeTypeID == item.DevelopmentOutcomeTypeID));

            if (entity == null)
            {
                entity = new TLDSDevelopmentOutcomeProfileEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.ProfileID = item.ProfileID;
            entity.DevelopmentOutcomeTypeID = item.DevelopmentOutcomeTypeID;
            entity.DevelopmentOutcomeContent = item.DevelopmentOutcomeContent;
            entity.StrategyContent = item.StrategyContent;
            entity.OriginalFileName = item.OriginalFileName;
            entity.S3FileName = item.S3FileName;
            table.Context.SubmitChanges();
            item.DevelopmentOutcomeProfileID = entity.DevelopmentOutcomeProfileID;
        }

        public void Delete(TLDSDevelopmentOutcomeProfile item)
        {
            var entity = table.FirstOrDefault(x => x.DevelopmentOutcomeProfileID.Equals(item.DevelopmentOutcomeProfileID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
