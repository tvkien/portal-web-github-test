using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PrintingGroupRepository : IPrintingGroupRepository
    {
        private readonly Table<PrintingGroupEntity> table;
        private readonly DbDataContext dbDataContext;

        public PrintingGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            dbDataContext = DbDataContext.Get(connectionString);
            table = DbDataContext.Get(connectionString).GetTable<PrintingGroupEntity>();
            Mapper.CreateMap<PrintingGroup, PrintingGroupEntity>();
        }

        public IQueryable<PrintingGroup> Select()
        {
            return table.Select(x => new PrintingGroup
                {
                    Id = x.GroupID,
                    Name = x.Name,
                    DistrictId = x.DistrictID ?? 0,
                    CreatedUserId = x.CreatedUserID ?? 0,
                    IsActive = x.IsActive
                });
        }

        public void Save(PrintingGroup item)
        {
            var entity = table.FirstOrDefault(x => x.GroupID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new PrintingGroupEntity();
                table.InsertOnSubmit(entity);
            }

            BindItemToEntity(item, entity);
            table.Context.SubmitChanges();
            item.Id = entity.GroupID;
        }

        public void Delete(PrintingGroup item)
        {
            var entity = table.FirstOrDefault(x => x.GroupID.Equals(item.Id));

            if (entity.IsNotNull())
            {
                entity.IsActive = false;
                table.Context.SubmitChanges();
            }
        }

        private void BindItemToEntity(PrintingGroup item, PrintingGroupEntity groupEntity)
        {
            groupEntity.IsActive = item.IsActive;
            groupEntity.CreatedUserID = item.CreatedUserId;
            groupEntity.DistrictID = item.DistrictId;
            groupEntity.Name = item.Name;
        }

        public List<int> GetListgroupIdsByUserId(int districtId, int userId)
        {
            return dbDataContext.GetListGroupIDsByUserId(districtId, userId).Select(o=>o.GroupID).ToList();
        }
    }
}