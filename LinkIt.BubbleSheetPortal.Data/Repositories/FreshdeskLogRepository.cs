using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.UserGuide;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class FreshdeskLogRepository : IRepository<FreshdeskLogData>
    {
        private readonly Table<FreshdeskLogEntity> table;

        public FreshdeskLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = TestDataContext.Get(connectionString);
            table = dataContext.GetTable<FreshdeskLogEntity>();
        }

        public IQueryable<FreshdeskLogData> Select()
        {
            return table.Select(x => new FreshdeskLogData
                {
                    FreshdeskLogID = x.FreshdeskLogID,
                    UserID = x.UserID,
                    Email = x.Email,
                    LastLoginDate = x.LastLoginDate
                });
        }

        public void Save(FreshdeskLogData item)
        {
            var entity = table.FirstOrDefault(x => x.FreshdeskLogID.Equals(item.FreshdeskLogID));

            if (entity == null)
            {
                entity = new FreshdeskLogEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.FreshdeskLogID = entity.FreshdeskLogID;
        }

        public void Delete(FreshdeskLogData item)
        {
            throw new NotImplementedException();
        }

        private void MapModelToEntity(FreshdeskLogEntity entity, FreshdeskLogData item)
        {
            entity.UserID = item.UserID;
            entity.Email = item.Email;
            entity.LastLoginDate = item.LastLoginDate;
        }
    }
}