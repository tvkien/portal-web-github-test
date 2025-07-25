using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class BubbleSheetListService
    {
        private readonly IReadOnlyRepository<BubbleSheetListItem> repository;
        private readonly IBubbleSheetListRepository iBubbleSheetListRepository;
        private readonly UserSchoolService userSchoolService;

        public BubbleSheetListService(IReadOnlyRepository<BubbleSheetListItem> repository, UserSchoolService userSchoolService, IBubbleSheetListRepository iBubbleSheetListRepository)
        {
            this.repository = repository;
            this.userSchoolService = userSchoolService;
            this.iBubbleSheetListRepository = iBubbleSheetListRepository;
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetList(User user)
        {
            switch (user.RoleId)
            {
                case (int)Permissions.Publisher:
                    return GetBubbleSheetListItemsForPublisher();
                case (int)Permissions.DistrictAdmin:
                    return GetBubbleSheetListByDistrictId(user.DistrictId);
                case (int)Permissions.SchoolAdmin:
                    return GetBubbleSheetListBySchoolId(GetSchoolsForSchoolAdmin(user));
                default:
                    return GetBubbleSheetListByUserId(user.Id).OrderBy(x => x.DateCreated);
            }
        }

        public BubbleSheetListItem GetBubbleSheetListItemByTicketAndUserId(string ticket, int userId)
        {
            return repository.Select().FirstOrDefault(x => x.Ticket.Equals(ticket) && x.CreatedByUserId.Equals(userId));
        }
        
        public IQueryable<BubbleSheetListItem> GetBubbleSheetListItemsForPublisher()
        {
            return repository.Select();
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetListByUserId(int userId)
        {
            //TODO: re-visit this in the future (17July2012, Antonio)
            if (userId > 0)
            {
                return repository.Select().Where(x => x.UserId.Equals(userId));
            }
            return repository.Select();
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetListByDistrictId(int? districtId)
        {
            return repository.Select().Where(x => x.DistrictId.Equals(districtId));
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetListBySchoolId(IEnumerable<UserSchool> userSchools)
        {
            return userSchools.SelectMany(userSchool => repository.Select().Where(x => x.SchoolId.Equals(userSchool.SchoolId))).AsQueryable();
        }

        private IEnumerable<UserSchool> GetSchoolsForSchoolAdmin(User user)
        {
            return userSchoolService.GetSchoolsUserHasAccessTo(user.Id).ToList();
        }

        //\
        /// <summary>
        /// Get List Bubblesheet By DistrictId, UserID & Role use Storeproceduce
        /// </summary>
        /// <param name="districtId"></param>
        /// <param name="schoolId"></param>
        /// <param name="userId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IQueryable<BubbleSheetListItem> GetBubbleSheetListProc(int districtId, int userId, int roleId, bool archived)
        {
            return iBubbleSheetListRepository.GetBubbleSheetListProc(districtId, userId, roleId, archived);
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetListProcV2(int districtId, int schoolId, int userId, int roleId, bool archived, DateTime dateFilter)
        {
            return iBubbleSheetListRepository.GetBubbleSheetListProcV2(districtId, schoolId, userId, roleId, archived, dateFilter);
        }

        public GetBubbleSheetReviewResponse GetBubbleSheetListProc_IncludedStatus(GetBubbleSheetReviewRequest request)
        {
            return iBubbleSheetListRepository.GetBubbleSheetListProc_IncludedStatus(request);
        }
    }
}