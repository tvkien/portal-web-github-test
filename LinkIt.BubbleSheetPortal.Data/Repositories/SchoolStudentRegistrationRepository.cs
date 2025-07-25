using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolStudentRegistrationRepository : IRepository<SchoolStudentRegistration>
    {
        private readonly Table<SchoolStudentRegistrationEntity> table;

        public SchoolStudentRegistrationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<SchoolStudentRegistrationEntity>();
            Mapper.CreateMap<SchoolStudentRegistration, SchoolStudentRegistrationEntity>();
        }

        public IQueryable<SchoolStudentRegistration> Select()
        {
            return table.Select(x => new SchoolStudentRegistration
                {
                    Id = x.SchoolID,
                    DistrictId = x.DistrictID,
                    Name = x.Name,
                    Code = x.Code,
                    StateCode = x.StateCode,
                    Status = x.Status,
                    StateId = x.StateID,
                    LocationCode = x.LocationCode
                });
        }

        public void Save(SchoolStudentRegistration item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolID.Equals(item.Id));

            if (entity == null)
            {
                entity = new SchoolStudentRegistrationEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.SchoolID;
        }

        public void Delete(SchoolStudentRegistration item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolID.Equals(item.Id));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}