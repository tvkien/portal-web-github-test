using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class CEESchoolRepository : IRepository<CEESchool>
    {
        private readonly Table<CEESchoolEntity> table;

        public CEESchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<CEESchoolEntity>();
            Mapper.CreateMap<CEESchool, CEESchoolEntity>();
        }

        public IQueryable<CEESchool> Select()
        {
            return table.Select(x => new CEESchool
                {
                    SchoolId = x.SchoolID,
                    CEESchoolId = x.CEESchoolID,                   
                    Name = x.Name,                    
                    LocationCode = x.LocationCode,
                    StateCode = x.StateCode
                });
        }

        public void Save(CEESchool item)
        {
            var entity = table.FirstOrDefault(x => x.CEESchoolID.Equals(item.CEESchoolId));

            if (entity == null)
            {
                entity = new CEESchoolEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.CEESchoolId = entity.CEESchoolID;
        }

        public void Delete(CEESchool item)
        {
            var entity = table.FirstOrDefault(x => x.CEESchoolID.Equals(item.CEESchoolId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}