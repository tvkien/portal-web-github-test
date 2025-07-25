using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryRequestEmailNotificationRepository : IReadOnlyRepository<RequestEmailNotification>
    {
        private readonly List<RequestEmailNotification> table = new List<RequestEmailNotification>();

        public InMemoryRequestEmailNotificationRepository()
        {
            table = AddRequestEmailNotifications();
        }

        private List<RequestEmailNotification> AddRequestEmailNotifications()
        {
            return new List<RequestEmailNotification>
                {
                    new RequestEmailNotification { Id = 1, RequestId = 1, EmailContent = "Email Content 1" },
                    new RequestEmailNotification { Id = 2, RequestId = 2, EmailContent = "Email Content 2" },
                    new RequestEmailNotification { Id = 3, RequestId = 3, EmailContent = "Email Content 3" }
                };  
        }

        public IQueryable<RequestEmailNotification> Select()
        {
            return table.AsQueryable();
        }
    }
}
