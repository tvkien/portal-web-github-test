using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictRosterOptionRepository : IRepository<LinkIt.BubbleSheetPortal.Models.DistrictRosterOption>
    {
        private readonly Table<DistrictRosterOptionEntity> table;        

        public DistrictRosterOptionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<DistrictRosterOptionEntity>();
        }

        public IQueryable<DistrictRosterOption> Select()
        {
            return table.Select(x => new DistrictRosterOption
            {
                DisplayName = x.DisplayName,
                DisplayOrder = x.DisplayOrder,
                DistrictId = x.DistrictID,
                DistrictRosterOptionId = x.DistrictRosterOptionID,
                IsEnabled = x.IsEnabled,
                RosterTypeId = x.RosterTypeID
            });
        }

        public void Save(DistrictRosterOption item)
        {
            var entity = table.FirstOrDefault(x => x.DistrictRosterOptionID.Equals(item.DistrictRosterOptionId));

            if (entity == null)
            {
                entity = new DistrictRosterOptionEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(item, entity);
            table.Context.SubmitChanges();
            item.DistrictRosterOptionId = entity.DistrictRosterOptionID;
        }

        public void Delete(DistrictRosterOption item)
        {
            var entity = table.FirstOrDefault(x => x.DistrictRosterOptionID.Equals(item.DistrictRosterOptionId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(DistrictRosterOption item, DistrictRosterOptionEntity entity)
        {
            entity.DisplayName = item.DisplayName;
            entity.DisplayOrder = item.DisplayOrder;
            entity.DistrictID = item.DistrictId;
            entity.IsEnabled = item.IsEnabled;
            entity.RosterTypeID = item.RosterTypeId;

        }        
    }
}
