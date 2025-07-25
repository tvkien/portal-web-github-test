using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.Requests;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RosterRequestService
    {
        private readonly IRosterRequestRepository _rosterRequestRepository;

        public RosterRequestService(IRosterRequestRepository rosterRequestRepository)
        {
            _rosterRequestRepository = rosterRequestRepository;
        }

        public IQueryable<Request> GetRequestsByUserId(int userId, int districtId)
        {
            return _rosterRequestRepository.GetRequestsByUserID(userId, districtId).Where(x => x.UserId.Equals(userId) && !x.IsDeleted);
        }

    }
}
