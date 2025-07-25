using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RosterTypeRepository : IRepository<LinkIt.BubbleSheetPortal.Models.RosterType>
    {
        private readonly Table<RosterTypeEntity> table;        

        public RosterTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<RosterTypeEntity>();
        }

        public IQueryable<RosterType> Select()
        {
            return table.Select(x => new RosterType
            {
                RosterTypeId = x.RosterTypeID,
                RosterTypeName = x.RosterTypeName
            });
        }

        public void Save(RosterType item)
        {
            var entity = table.FirstOrDefault(x => x.RosterTypeID.Equals(item.RosterTypeId));

            if (entity == null)
            {
                entity = new RosterTypeEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.RosterTypeId = entity.RosterTypeID;
        }

        public void Delete(RosterType item)
        {
            var entity = table.FirstOrDefault(x => x.RosterTypeID.Equals(item.RosterTypeId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(RosterType item, RosterTypeEntity entity)
        {
            entity.RosterTypeName = item.RosterTypeName;            
        }        
    }
}
