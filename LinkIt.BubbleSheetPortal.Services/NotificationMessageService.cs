using System;
using System.Linq;
using System.Net.Sockets;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class NotificationMessageService
    {
        private readonly INotificationMessageRepository repository;

        public NotificationMessageService(INotificationMessageRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<NotificationMessage> Select()
        {
            return repository.Select();
        }

        public IList<NotificationMessage> GetByDistrictId(int districtId, int userId)
        {
            var notificationMessages = repository.GetNotificationByDistrictId(districtId, userId);

            return notificationMessages;
        }

        public NotificationMessage GetByNotificationType(int userId, string notificationType)
        {
            return repository.Select().FirstOrDefault(x => x.ReceivingUserID == userId && x.NotificationType.Equals(notificationType));
        }

        public void SaveNotification(NotificationMessage item)
        {
            repository.Save(item);
        }
    }
}
