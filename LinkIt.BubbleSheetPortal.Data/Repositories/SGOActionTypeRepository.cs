using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOActionTypeRepository : ISGOActionTypeRepository
    {
        private readonly Table<SGOActionTypeEntity> table;
        private readonly SGODataContext _context;

        public SGOActionTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = SGODataContext.Get(connectionString);
            table = SGODataContext.Get(connectionString).GetTable<SGOActionTypeEntity>();
        }

        public IQueryable<SGOActionTypeData> Select()
        {
            return table.Select(x => new SGOActionTypeData()
                                     {
                                        Name = x.Name,
                                        SGOActionTypeID = x.SGOActionTypeID
                                     });
        }

        public void Save(SGOActionTypeData item)
        {
            var entity = table.FirstOrDefault(x => x.SGOActionTypeID.Equals(item.SGOActionTypeID));

            if (entity == null)
            {
                entity = new SGOActionTypeEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);

            table.Context.SubmitChanges();
            item.SGOActionTypeID = entity.SGOActionTypeID;
        }

        public void Delete(SGOActionTypeData item)
        {
            var entity = table.FirstOrDefault(x => x.SGOActionTypeID.Equals(item.SGOActionTypeID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(SGOActionTypeData model, SGOActionTypeEntity entity)
        {
            entity.Name = model.Name;
        }
    }
}