using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SpecializedReportJobRepository : IRepository<SpecializedReportJob>
    {
        private readonly Table<SpecializedReportJobEntity> table;

        public SpecializedReportJobRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<SpecializedReportJobEntity>();
        }

        public IQueryable<SpecializedReportJob> Select()
        {
            return table.Select(x => new SpecializedReportJob
                                         {
                                             CreatedDate = x.CreatedDate,
                                             DownloadUrl = x.DownloadUrl,
                                             GeneratedItem = x.GeneratedItem,
                                             SpecializedReportJobId = x.SpecializedReportJobID,
                                             Status = x.Status,
                                             TotalItem = x.TotalItem,
                                             UserId = x.UserID,
                                             DistrictId = x.DistrictID??0
                                         });
        }

        public void Save(SpecializedReportJob item)
        {
            var entity = table.FirstOrDefault(x => x.SpecializedReportJobID.Equals(item.SpecializedReportJobId));

            if (entity.IsNull())
            {
                entity = new SpecializedReportJobEntity();
                table.InsertOnSubmit(entity);
            }

            BindEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.SpecializedReportJobId = entity.SpecializedReportJobID;
        }

        private void BindEntityToItem(SpecializedReportJobEntity entity, SpecializedReportJob item)
        {
            entity.CreatedDate = item.CreatedDate;
            entity.DownloadUrl = item.DownloadUrl;
            entity.GeneratedItem = item.GeneratedItem;
            entity.Status = item.Status;
            entity.TotalItem = item.TotalItem;
            entity.UserID = item.UserId;
            entity.DistrictID = item.DistrictId;
        }

        public void Delete(SpecializedReportJob item)
        {
            var entity = table.FirstOrDefault(x => x.SpecializedReportJobID.Equals(item.SpecializedReportJobId));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}