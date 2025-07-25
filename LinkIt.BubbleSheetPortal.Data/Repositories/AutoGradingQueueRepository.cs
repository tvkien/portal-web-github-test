using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AutoGradingQueueRepository : IAutoGradingQueueRepository
    {
        private readonly Table<AutoGradingQueueEntity> table;
        public AutoGradingQueueRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<AutoGradingQueueEntity>();
            Mapper.CreateMap<AutoGradingQueueData, AutoGradingQueueEntity>();
        }

        public IQueryable<AutoGradingQueueData> Select()
        {
            return table.Select(x => new AutoGradingQueueData
            {
                AutoGradingQueueID = x.AutoGradingQueueID,
                CreatedDate = x.CreatedDate,
                ForceGrading = x.ForceGrading,
                GradedAnswerCount = x.GradedAnswerCount,
                ProcessingDate = x.ProcessingDate,
                ProcessingTime = x.ProcessingTime,
                QTIOnlineTestSessionID = x.QTIOnlineTestSessionID,
                Status = x.Status,
                RequestUserId = x.RequestUserId,
                Type = x.Type,
                IsAwaitingRetry = x.IsAwaitingRetry
            });
        }

        public void Save(AutoGradingQueueData item)
        {
            var entity = table.FirstOrDefault(x => x.AutoGradingQueueID.Equals(item.AutoGradingQueueID));

            if (entity.IsNull())
            {
                entity = new AutoGradingQueueEntity();
                table.InsertOnSubmit(entity);
            }

            Map(item, entity);
            table.Context.SubmitChanges();
            item.AutoGradingQueueID = entity.AutoGradingQueueID;
        }

        private void Map(AutoGradingQueueData item, AutoGradingQueueEntity entity)
        {
            entity.AutoGradingQueueID = item.AutoGradingQueueID;
            entity.CreatedDate = item.CreatedDate;
            entity.ForceGrading = item.ForceGrading;
            entity.GradedAnswerCount = item.GradedAnswerCount;
            entity.ProcessingDate = item.ProcessingDate;
            entity.ProcessingTime = item.ProcessingTime;
            entity.QTIOnlineTestSessionID = item.QTIOnlineTestSessionID;
            entity.Status = item.Status;
            entity.RequestUserId = item.RequestUserId;
            entity.Type = item.Type;
        }

        public void Delete(AutoGradingQueueData item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.AutoGradingQueueID.Equals(item.AutoGradingQueueID));
                if (entity != null)
                {
                    entity.Status = 0;
                    table.Context.SubmitChanges();
                }
            }
        }

        public AutoGradingQueueData GetAutoGradingQueueByQTOnlineTestSessionID(int qTIOnlineTestSessionID)
        {
            return Select()
                    .Where(x => x.QTIOnlineTestSessionID == qTIOnlineTestSessionID)
                    .OrderByDescending(o => o.AutoGradingQueueID).FirstOrDefault();
        }

        public IEnumerable<AutoGradingQueueData> GetAutoGradingQueueByQTOnlineTestSessionID(IEnumerable<int> qTIOnlineTestSessionIds)
        {
            return Select()
                    .FilterOnLargeSet(entry => entry, (subSet) =>
                    {

                        return entry => subSet.Contains(entry.QTIOnlineTestSessionID);
                    }, qTIOnlineTestSessionIds)
                   .OrderByDescending(o => o.AutoGradingQueueID)
                   .ToArray();
        }
    }
}
