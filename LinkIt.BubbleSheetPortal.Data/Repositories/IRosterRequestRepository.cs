using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IRosterRequestRepository
    {
        IQueryable<Request> GetRequestsByUserID(int userId, int districtId);
    }
}
