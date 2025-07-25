using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSEarlyABLESReportRepository : IRepository<TLDSEarlyABLESReport>
    {
        private readonly Table<TLDSEarlyABLESReportEntity> table;

        public TLDSEarlyABLESReportRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSEarlyABLESReportEntity>();
        }

        public IQueryable<TLDSEarlyABLESReport> Select()
        {
            return table.Select(x => new TLDSEarlyABLESReport
            {
                EarlyABLESReportId = x.EarlyABLESReportId,
                ProfileId = x.ProfileId,
                ReportName = x.ReportName,
                LearningReadinessReportCompleted = x.LearningReadinessReportCompleted,
                ReportDate = x.ReportDate,
                AvailableOnRequest = x.AvailableOnRequest
            });
        }

        public void Save(TLDSEarlyABLESReport item)
        {
            var entity = table.FirstOrDefault(x => x.EarlyABLESReportId.Equals(item.EarlyABLESReportId));

            if (entity == null)
            {
                entity = new TLDSEarlyABLESReportEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.ProfileId = item.ProfileId;
            entity.ReportName = item.ReportName;
            entity.LearningReadinessReportCompleted = item.LearningReadinessReportCompleted;
            entity.ReportDate = item.ReportDate;
            entity.AvailableOnRequest = item.AvailableOnRequest;
            
            table.Context.SubmitChanges();
            item.EarlyABLESReportId = entity.EarlyABLESReportId;
        }

        public void Delete(TLDSEarlyABLESReport item)
        {
            var entity = table.FirstOrDefault(x => x.EarlyABLESReportId.Equals(item.EarlyABLESReportId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
