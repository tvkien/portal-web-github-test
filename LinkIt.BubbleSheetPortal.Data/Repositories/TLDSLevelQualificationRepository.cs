using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSLevelQualificationRepository : IRepository<TLDSLevelQualification>
    {
        private readonly Table<TLDSLevelQualificationEntity> table;

        public TLDSLevelQualificationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSLevelQualificationEntity>();
        }

        public IQueryable<TLDSLevelQualification> Select()
        {
            return table.Select(x => new TLDSLevelQualification
            {
                TLDSLevelQualificationID = x.TLDSLevelQualificationID,
                Name = x.Name,
            });
        }

        public void Save(TLDSLevelQualification item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSLevelQualificationID.Equals(item.TLDSLevelQualificationID));

            if (entity.IsNull())
            {
                entity = new TLDSLevelQualificationEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.Name = item.Name;
            table.Context.SubmitChanges();
            item.TLDSLevelQualificationID = entity.TLDSLevelQualificationID;
        }

       

        public void Delete(TLDSLevelQualification item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSLevelQualificationID.Equals(item.TLDSLevelQualificationID));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
