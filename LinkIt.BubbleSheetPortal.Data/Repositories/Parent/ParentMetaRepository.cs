using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Parent
{
    public class ParentMetaRepository : IRepository<ParentMetaDto>
    {
        private readonly Table<ParentMetaEntity> table;

        public ParentMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ParentMetaEntity>();
        }

        public IQueryable<ParentMetaDto> Select()
        {
            return table.Select(x => new ParentMetaDto
            {
                ParentMetaID = x.ParentMetaID, 
                ParentID = x.ParentID,
                Name = x.Name,
                Data = x.Data
            });
        }

        public void Save(ParentMetaDto item)
        {
            var entity = table.FirstOrDefault(x => x.ParentMetaID.Equals(item.ParentMetaID));
            if (entity == null)
            {
                entity = new ParentMetaEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.ParentMetaID = entity.ParentMetaID;
        }

        public void Delete(ParentMetaDto item)
        {
            var entity = table.FirstOrDefault(x => x.ParentMetaID.Equals(item.ParentMetaID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(ParentMetaDto item, ParentMetaEntity entity)
        {
            entity.ParentID = item.ParentID;
            entity.Name = item.Name;
            entity.Data = string.IsNullOrEmpty(item.Data) ? string.Empty : item.Data.Trim();
        }
    }
}
