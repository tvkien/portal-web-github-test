using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RequestEmailNotificationRepository : IReadOnlyRepository<RequestEmailNotification>
    {
        private readonly Table<RequestEmailNotificationEntity> table;

        public RequestEmailNotificationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<RequestEmailNotificationEntity>();
        }

        public IQueryable<RequestEmailNotification> Select()
        {
            return table.Select(x => new RequestEmailNotification
                {
                    Id = x.ID,
                    RequestId = x.RequestID,
                    EmailContent = x.EmailContent
                });
        }
    }
}