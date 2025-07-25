using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSAdditionalInformationRepository : IRepository<TLDSAdditionalInformation>
    {
        private readonly Table<TLDSAdditionalInformationEntity> table;

        public TLDSAdditionalInformationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSAdditionalInformationEntity>();
        }

        public IQueryable<TLDSAdditionalInformation> Select()
        {
            return table.Select(x => new TLDSAdditionalInformation
            {
                AdditionalInformationID = x.AdditionalInformationID,
                ProfileID = x.ProfileID,
                AreasOfNote = x.AreasOfNote,
                StrategiesForEnhancedSupport = x.StrategiesForEnhancedSupport,
                DateCreated = x.DateCreated
            });
        }

        public void Save(TLDSAdditionalInformation item)
        {
            var entity = table.FirstOrDefault(x => x.AdditionalInformationID.Equals(item.AdditionalInformationID));

            if (entity == null)
            {
                entity = new TLDSAdditionalInformationEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.ProfileID = item.ProfileID;
            entity.AreasOfNote = item.AreasOfNote;
            entity.StrategiesForEnhancedSupport = item.StrategiesForEnhancedSupport;
            entity.DateCreated = item.DateCreated;
            
            table.Context.SubmitChanges();
            item.AdditionalInformationID = entity.AdditionalInformationID;
        }

        public void Delete(TLDSAdditionalInformation item)
        {
            var entity = table.FirstOrDefault(x => x.AdditionalInformationID.Equals(item.AdditionalInformationID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
