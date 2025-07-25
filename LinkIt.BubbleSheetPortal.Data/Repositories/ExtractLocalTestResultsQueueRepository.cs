using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ExtractLocalTestResultsQueueRepository : IRepository<ExtractLocalTestResultsQueue>
    {
        private readonly Table<ExtractLocalTestResultsQueueEntity> table;
        public ExtractLocalTestResultsQueueRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ExtractTestDataContext.Get(connectionString).GetTable<ExtractLocalTestResultsQueueEntity>();
            Mapper.CreateMap<ExtractLocalTestResultsQueue, ExtractLocalTestResultsQueueEntity>();
        }

        public IQueryable<ExtractLocalTestResultsQueue> Select()
        {
            return table.Select(x => new ExtractLocalTestResultsQueue
            {
              ExtractLocalTestResultsQueueId = x.ExtractLocalTestResultsQueueID,
              DistrictId = x.DistrictID,
              UserId = x.UserID,
              ExportTemplates = x.ExportTemplates,
              ListIDsInput = x.ListIDsInput, 
              Status = x.Status,
              CreatedDate = x.CreatedDate,
              ProcessingDate = x.ProcessingDate,
              ProcessingTime = x.ProcessingTime,
              EndProcessingDate = x.EndProcessingDate,
              ExtractTestResultParamID = x.ExtractTestResultParamID
            });
        }

        public void Save(ExtractLocalTestResultsQueue item)
        {
            var entity = table.FirstOrDefault(x => x.ExtractLocalTestResultsQueueID.Equals(item.ExtractLocalTestResultsQueueId));

            if (entity.IsNull())
            {
                entity = new ExtractLocalTestResultsQueueEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.ExtractLocalTestResultsQueueId = entity.ExtractLocalTestResultsQueueID;
        }

        public void Delete(ExtractLocalTestResultsQueue item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.ExtractLocalTestResultsQueueID.Equals(item.ExtractLocalTestResultsQueueId));
                if (entity != null)
                {
                    entity.Status = 0;
                    table.Context.SubmitChanges();
                }
            }
        }
    }
}
