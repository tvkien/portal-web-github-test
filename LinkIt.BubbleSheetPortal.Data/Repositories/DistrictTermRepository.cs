using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using AutoMapper;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictTermRepository : IRepository<DistrictTerm>
    {
        private readonly Table<DistrictTermEntity> table;

        public DistrictTermRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<DistrictTermEntity>();
            Mapper.CreateMap<DistrictTerm, DistrictTermEntity>();
        }

        public IQueryable<DistrictTerm> Select()
        {
            return table.Select(x => new DistrictTerm()
            {
                DistrictTermID = x.DistrictTermID,
                Name = x.Name,
                DistrictID = x.DistrictID,
                DateStart = x.DateStart,
                DateEnd = x.DateEnd,
                Code = x.Code,
                CreatedByUserID = x.CreatedByUserID,
                UpdatedByUserID = x.UpdatedByUserID,
                DateCreated = x.DateCreated,
                DateUpdated = x.DateUpdated,
                ModifiedUser = x.ModifiedUser,
                ModifiedBy = x.ModifiedBy,
                SISID = x.SISID
            });
        }

        public void Save(DistrictTerm item)
        {
            var entity = table.FirstOrDefault(x => x.DistrictTermID == item.DistrictTermID);

            if (entity.IsNull())
            {
                entity = new DistrictTermEntity();
                table.InsertOnSubmit(entity);
            }
            else
            {
                item.CreatedByUserID = entity.CreatedByUserID;
                item.DateCreated = entity.DateCreated;
            }

            Mapper.Map(item, entity);

            table.Context.SubmitChanges();
            item.DistrictTermID = entity.DistrictTermID;
        }

        public void Delete(DistrictTerm item)
        {
            var entity = table.FirstOrDefault(x => x.DistrictTermID == item.DistrictTermID);

            if (!entity.IsNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
