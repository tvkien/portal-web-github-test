using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetGenericMapRepository : IRepository<BubbleSheetGenericMap>
    {
        private readonly Table<BubbleSheetGenericMapEntity> table;

        public BubbleSheetGenericMapRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetGenericMapEntity>();
        }

        public IQueryable<BubbleSheetGenericMap> Select()
        {
            return table.Select(x => new BubbleSheetGenericMap()
            {
                 BubbleSheetGenericMapID = x.BubbleSheetGenericMapID,
                 GenericBubbleSheetID = x.GenericBubbleSheetID,
                 StudentBubbleSheetID = x.StudentBubbleSheetID,
                 TestIndex = x.TestIndex
            });
        }

        public void Save(BubbleSheetGenericMap item)
        {
            var entity = table.FirstOrDefault(x => x.BubbleSheetGenericMapID.Equals(item.BubbleSheetGenericMapID));
            if (entity.IsNull())
            {
                entity = new BubbleSheetGenericMapEntity();
                table.InsertOnSubmit(entity);
            }
            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.BubbleSheetGenericMapID = entity.BubbleSheetGenericMapID;
        }

        public void Delete(BubbleSheetGenericMap item)
        {
            throw new NotImplementedException();
        }

        private void MapModelToEntity(BubbleSheetGenericMap item, BubbleSheetGenericMapEntity entity)
        {
            entity.GenericBubbleSheetID = item.GenericBubbleSheetID;
            entity.StudentBubbleSheetID = item.StudentBubbleSheetID;
            entity.TestIndex = item.TestIndex;
        }
    }
}
