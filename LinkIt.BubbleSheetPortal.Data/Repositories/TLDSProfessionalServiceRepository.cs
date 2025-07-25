using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSProfessionalServiceRepository : IRepository<TLDSProfessionalService>
    {
        private readonly Table<TLDSProfessionalServiceEntity> table;

        public TLDSProfessionalServiceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSProfessionalServiceEntity>();
        }

        public IQueryable<TLDSProfessionalService> Select()
        {
            return table.Select(x => new TLDSProfessionalService
            {
                ProfessionalServiceID = x.ProfessionalServiceID,
                ProfileID = x.ProfileID,
                Name = x.Name,
                Address = x.Address,
                ContactPerson = x.ContactPerson,
                Position = x.Position,
                Phone = x.Phone,
                Email = x.Email,
                WrittenReportAvailable = x.WrittenReportAvailable,
                ReportForwardedToSchoolDate = x.ReportForwardedToSchoolDate,
                Attached = x.Attached,
                AvailableUponRequested = x.AvailableUponRequested
            });
        }

        public void Save(TLDSProfessionalService item)
        {
            var entity = table.FirstOrDefault(x => x.ProfessionalServiceID.Equals(item.ProfessionalServiceID));

            if (entity == null)
            {
                entity = new TLDSProfessionalServiceEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.ProfileID = item.ProfileID;
            entity.Address = item.Address;
            entity.Attached = item.Attached;
            entity.AvailableUponRequested = item.AvailableUponRequested;
            entity.ContactPerson = item.ContactPerson;
            entity.Email = item.Email;
            entity.Name = item.Name;
            entity.Phone = item.Phone;
            entity.WrittenReportAvailable = item.WrittenReportAvailable;
            entity.Position = item.Position;
            entity.ReportForwardedToSchoolDate = item.ReportForwardedToSchoolDate;
            table.Context.SubmitChanges();
            item.ProfessionalServiceID = entity.ProfessionalServiceID;
        }

        public void Delete(TLDSProfessionalService item)
        {
            var entity = table.FirstOrDefault(x => x.ProfessionalServiceID.Equals(item.ProfessionalServiceID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
