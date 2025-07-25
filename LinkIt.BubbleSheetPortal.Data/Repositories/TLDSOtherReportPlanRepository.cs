using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSOtherReportPlanRepository : IRepository<TLDSOtherReportPlan>
    {
        private readonly Table<TLDSOtherReportPlanEntity> table;

        public TLDSOtherReportPlanRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TLDSContextDataContext.Get(connectionString).GetTable<TLDSOtherReportPlanEntity>();
        }

        public IQueryable<TLDSOtherReportPlan> Select()
        {
            return table.Select(x => new TLDSOtherReportPlan
            {
                OtherReportPlanID = x.OtherReportPlanID,
                ProfileID = x.ProfileID,
                ReportName = x.ReportName,
                ReportDate = x.ReportDate,
                AttachmentURL = x.AttachmentURL,
                AvailableOnRequest = x.AvailableOnRequest
            });
        }

        public void Save(TLDSOtherReportPlan item)
        {
            var entity = table.FirstOrDefault(x => x.OtherReportPlanID.Equals(item.OtherReportPlanID));

            if (entity == null)
            {
                entity = new TLDSOtherReportPlanEntity();
                table.InsertOnSubmit(entity);
            }            
            entity.ProfileID = item.ProfileID;
            entity.ReportName = item.ReportName;
            entity.ReportDate = item.ReportDate;
            entity.AttachmentURL = item.AttachmentURL;
            entity.AvailableOnRequest = item.AvailableOnRequest;
          
            table.Context.SubmitChanges();
            item.OtherReportPlanID = entity.OtherReportPlanID;
        }

        public void Delete(TLDSOtherReportPlan item)
        {
            var entity = table.FirstOrDefault(x => x.OtherReportPlanID.Equals(item.OtherReportPlanID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
