using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.BubbleSheetReview;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetListRepository : IReadOnlyRepository<BubbleSheetListItem>, IBubbleSheetListRepository
    {
        private readonly Table<BubbleSheetListViewCache> table;
        private readonly BubbleSheetDataContext _bubbleSheetDataContext;
        private readonly ManageTestRepository _manageTestRepository;

        public BubbleSheetListRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetListViewCache>();
            _bubbleSheetDataContext = BubbleSheetDataContext.Get(connectionString);
            _manageTestRepository = new ManageTestRepository(conn);
        }

        public IQueryable<BubbleSheetListItem> Select()
        {
            return table.Select(x => new BubbleSheetListItem
            {
                Ticket = x.Ticket,
                BankName = x.BankName,
                GradeName = x.GradeName,
                TestName = x.TestName,
                SubjectName = x.SubjectName,
                ClassName = x.ClassName,
                TeacherName = x.TeacherName,
                CreatedByUserId = x.CreatedByUserID.GetValueOrDefault(),
                UserId = x.UserID,
                DateCreated = x.DateCreated ?? DateTime.Now.AddYears(-20),
                DateUpdated = x.DateUpdated,
                GradedCount = x.GradedCount ?? 0,
                TotalSheets = x.TotalCount ?? 0,
                UnmappedCount = x.UnmappedCount ?? 0,
                DistrictId = x.DistrictID ?? 0,
                SchoolId = x.SchoolID ?? 0,
                ClassId = x.ClassID,
                IsArchived = x.IsArchived,
                IsManualEntry = x.IsManualEntry
            });
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetListProc(int districtId, int userId, int roleId, bool archived)
        {
            return
                _bubbleSheetDataContext.BubbleSheetListProc(districtId, userId, roleId, archived)
                                       .ToList()
                                       .Select(x => new BubbleSheetListItem
                                       {
                                           Ticket = x.Ticket,
                                           BankName = x.BankName,
                                           GradeName = x.GradeName,
                                           TestName = x.TestName,
                                           SubjectName = x.SubjectName,
                                           ClassName = x.ClassName,
                                           TeacherName = x.TeacherName,
                                           CreatedByUserId = x.CreatedByUserID.GetValueOrDefault(),
                                           UserId = x.UserID,
                                           DateCreated = x.DateCreated ?? DateTime.Now.AddYears(-20),
                                           DateUpdated = x.DateUpdated,
                                           GradedCount = x.GradedCount ?? 0,
                                           TotalSheets = x.TotalCount ?? 0,
                                           UnmappedCount = x.UnmappedCount ?? 0,
                                           DistrictId = x.DistrictID ?? 0,
                                           SchoolId = x.SchoolID ?? 0,
                                           ClassId = x.ClassID,
                                           IsArchived = x.IsArchived,
                                           IsManualEntry = x.IsManualEntry,
                                           VirtualTestSubTypeID = x.VirtualTestSubTypeID
                                       }).AsQueryable();
        }

        public IQueryable<BubbleSheetListItem> GetBubbleSheetListProcV2(int districtId, int schoolId, int userId, int roleId, bool archived, DateTime createdDate)
        {
            return
                _bubbleSheetDataContext.BubbleSheetListProcV2(districtId, userId, roleId, archived, createdDate, schoolId)
                                       .ToList()
                                       .Select(x => new BubbleSheetListItem
                                       {
                                           Ticket = x.Ticket,
                                           BankID = x.BankID,
                                           BankName = x.BankName,
                                           GradeName = x.GradeName,
                                           TestName = x.TestName,
                                           SubjectName = x.SubjectName,
                                           ClassName = x.ClassName,
                                           TeacherName = x.TeacherName,
                                           GroupTeacherName = x.GroupTeacherName,
                                           ClassIds = x.ClassIDs,
                                           CreatedByUserId = x.CreatedByUserID.GetValueOrDefault(),
                                           UserId = x.UserID,
                                           DateCreated = x.DateCreated ?? DateTime.Now.AddYears(-20),
                                           DateUpdated = x.DateUpdated,
                                           GradedCount = x.GradedCount ?? 0,
                                           TotalSheets = x.TotalCount ?? 0,
                                           UnmappedCount = x.UnmappedCount ?? 0,
                                           DistrictId = x.DistrictID ?? 0,
                                           SchoolId = x.SchoolID ?? 0,
                                           ClassId = x.ClassID,
                                           IsArchived = x.IsArchived,
                                           IsManualEntry = x.IsManualEntry,
                                           VirtualTestSubTypeID = x.VirtualTestSubTypeID
                                       }).AsQueryable();
        }

        public GetBubbleSheetReviewResponse GetBubbleSheetListProc_IncludedStatus(GetBubbleSheetReviewRequest request)
        {
            var result = new GetBubbleSheetReviewResponse();

            var data = _bubbleSheetDataContext.BubbleSheetListProc_IncludedStatus(request.DistrictId, request.UserId, request.RoleId, request.Archived, request.CreatedDate,
                    request.SchoolId, request.TestName, request.GradeName, request.SubjectName, request.BankName, request.ClassName, request.TeacherName, request.GeneralSearch,
                    request.SortColumn, request.SortDirection, request.StartRow, request.PageSize)
                .ToList();

            result.TotalRecord = data?.FirstOrDefault()?.TotalRecord ?? 0;
            result.Data = data
                .Select(ConvertToBubbleSheetListItem()).ToList();
            ;

            return result;
        }

        private static Func<BubbleSheetListProc_IncludedStatusResult, BubbleSheetListItem> ConvertToBubbleSheetListItem()
        {
            return x => new BubbleSheetListItem
            {
                Ticket = x.Ticket,
                BankID = x.BankID,
                BankName = x.BankName,
                GradeName = x.GradeName,
                TestName = x.TestName,
                SubjectName = x.SubjectName,
                ClassName = x.ClassName,
                TeacherName = x.TeacherName,
                GroupTeacherName = x.GroupTeacherName,
                ClassIds = x.ClassIDs,
                CreatedByUserId = x.CreatedByUserID.GetValueOrDefault(),
                UserId = x.UserID,
                DateCreated = x.DateCreated ?? DateTime.Now.AddYears(-20),
                DateUpdated = x.DateUpdated,
                GradedCount = x.GradedCount ?? 0,
                TotalSheets = x.TotalCount ?? 0,
                UnmappedCount = x.UnmappedCount ?? 0,
                DistrictId = x.DistrictID ?? 0,
                SchoolId = x.SchoolID ?? 0,
                ClassId = x.ClassID,
                IsArchived = x.IsArchived,
                IsManualEntry = x.IsManualEntry,
                VirtualTestSubTypeID = x.VirtualTestSubTypeID,
                Fini = x.Fini,
                Review = x.Review,
                Ungraded = x.Ungraded

            };
        }
    }
}
