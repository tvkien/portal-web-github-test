using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class NotificationMessageRepository : INotificationMessageRepository
    {
        private readonly Table<NotificationMessageEntity> table;
        private readonly DbDataContext _dbContext;

        public NotificationMessageRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<NotificationMessageEntity>();
            _dbContext = DbDataContext.Get(connectionString);
        }

        public IQueryable<NotificationMessage> Select()
        {
            return table.Select(x => new NotificationMessage
            {
                AccessedDistrict = x.AccessedDistrict,
                HtmlContent = x.HtmlContent,
                NotificationMessageId = x.NotificationMessageID,
                PublishedTime = x.PublishedTime,
                Status = x.Status,
                ReceivingUserID = x.ReceivingUserID,
                NotificationType = x.NotificationType
            });
        }

        public void Save(NotificationMessage item)
        {
            var entity = table.FirstOrDefault(x => x.NotificationMessageID.Equals(item.NotificationMessageId));

            if (entity.IsNull())
            {
                entity = new NotificationMessageEntity();
                table.InsertOnSubmit(entity);
            }

            BindEntityToItem(entity, item);
            table.Context.SubmitChanges();
            item.NotificationMessageId = entity.NotificationMessageID;
        }

        private void BindEntityToItem(NotificationMessageEntity entity, NotificationMessage item)
        {
            entity.AccessedDistrict = item.AccessedDistrict;
            entity.HtmlContent = item.HtmlContent;
            entity.PublishedTime = item.PublishedTime;
            entity.Status = item.Status;
            entity.ReceivingUserID = item.ReceivingUserID;
            entity.NotificationType = item.NotificationType;
        }

        public void Delete(NotificationMessage item)
        {
            var entity = table.FirstOrDefault(x => x.NotificationMessageID.Equals(item.NotificationMessageId));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public IList<NotificationMessage> GetNotificationByDistrictId(int districtId, int userId)
        {
            var results = _dbContext
                .GetNotificationByDistrictId(districtId, userId)
                .Select(o => new NotificationMessage
                {
                    NotificationMessageId = o.NotificationMessageID,
                    Status = o.Status,
                    PublishedTime = o.PublishedTime,
                    HtmlContent = o.HtmlContent,
                    AccessedDistrict = o.AccessedDistrict,
                    ReceivingUserID = o.ReceivingUserID,
                    NotificationType = o.NotificationType
                })
                .ToList();

            return results;
        }
    }
}
