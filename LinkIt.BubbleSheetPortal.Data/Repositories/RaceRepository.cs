using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RaceRepository : IRepository<Race>
    {
        private readonly Table<RaceEntity> table;

        public RaceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<RaceEntity>();
            Mapper.CreateMap<Race, RaceEntity>();
        }

        public IQueryable<Race> Select()
        {
            return table.Select(x => new Race
                {
                    Id = x.RaceID,
                    Name = x.Name,
                    DistrictID = x.DistrictID,
                    Code = x.Code,
                    AltCode = x.AltCode
                });
        }

        public void Save(Race item)
        {
            var entity = table.FirstOrDefault(x => x.RaceID.Equals(item.Id));

            if (entity == null)
            {
                entity = new RaceEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.RaceID;
        }

        public void Delete(Race item)
        {
            var entity = table.FirstOrDefault(x => x.RaceID.Equals(item.Id));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}