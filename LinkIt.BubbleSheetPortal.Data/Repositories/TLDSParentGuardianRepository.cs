using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSParentGuardianRepository : IRepository<TLDSParentGuardian>
    {
        private readonly Table<TLDSParentGuardianEntity> table;

        public TLDSParentGuardianRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSParentGuardianEntity>();
        }

        public IQueryable<TLDSParentGuardian> Select()
        {
            return table.Select(x => new TLDSParentGuardian
            {
                TLDSParentGuardianID = x.TLDSParentGuardianID,
                TLDSProfileID = x.TLDSProfileID,
                ParentGuardianName = x.ParentGuardianName,
                ParentGuardianRelationship = x.ParentGuardianRelationship,
                ParentGuardianPhone = x.ParentGuardianPhone,
                ParentGuardianEmail = x.ParentGuardianEmail
            });
        }

        public void Save(TLDSParentGuardian item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSParentGuardianID.Equals(item.TLDSParentGuardianID));

            if (entity == null)
            {
                entity = new TLDSParentGuardianEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.TLDSProfileID = item.TLDSProfileID;
            entity.ParentGuardianName = item.ParentGuardianName;
            entity.ParentGuardianRelationship = item.ParentGuardianRelationship;
            entity.ParentGuardianPhone = item.ParentGuardianPhone;
            entity.ParentGuardianEmail = item.ParentGuardianEmail;

            table.Context.SubmitChanges();
            item.TLDSParentGuardianID = entity.TLDSParentGuardianID;
        }

        public void Delete(TLDSParentGuardian item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSParentGuardianID.Equals(item.TLDSParentGuardianID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
