using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class RosterRequestRepository : IRosterRequestRepository
    {

        private readonly DbDataContext _dataContext;

        public RosterRequestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _dataContext = DbDataContext.Get(connectionString);
        }

        public IQueryable<Request> GetRequestsByUserID(int userId, int districtId)
        {
            var query = _dataContext.GetRequestsByUserID(userId, districtId);

            var result = query.Select(x => new Request
            {
                Id = x.RequestID,
                DataRequestType = (DataRequestType)x.DataRequestTypeID,
                DistrictId = x.DistrictID ?? 0,
                UserId = x.UserID,
                EmailAddress = x.Email,
                ImportedFileName = x.ImportedFileName,
                RequestTime = x.RequestTime,
                RequestMode = x.Mode != null ? (RequestMode)x.Mode : RequestMode.Validation,
                RequestStatus = x.Status != null ? (RequestStatus)x.Status : RequestStatus.Pending,
                IsDeleted = x.IsDeleted,
                HasBeenMoved = x.HasBeenMoved,
                HasEmailContent = x.HasEmailContent.Equals(true),
                RosterType = x.RosterType,
                DistrictName = x.DistrictName,
            }).AsQueryable();

            return result;
        }
    }
}
