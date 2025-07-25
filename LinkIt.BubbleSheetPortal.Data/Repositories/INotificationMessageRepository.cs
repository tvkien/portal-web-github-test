using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface INotificationMessageRepository : IRepository<NotificationMessage>
    {
        IList<NotificationMessage> GetNotificationByDistrictId(int districtId, int userId);
    }
}
