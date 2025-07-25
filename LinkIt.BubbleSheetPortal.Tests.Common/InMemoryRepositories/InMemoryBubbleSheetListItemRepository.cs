using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetListItemRepository : IReadOnlyRepository<BubbleSheetListItem>
    {
        private List<BubbleSheetListItem> table;

        public InMemoryBubbleSheetListItemRepository()
        {
            table = AddBubbleSheetListItems();
        }

        private List<BubbleSheetListItem> AddBubbleSheetListItems()
        {
            return new List<BubbleSheetListItem>
                       {
                           new BubbleSheetListItem{ Ticket = "Ticket1", CreatedByUserId = 11, UserId = 11, DistrictId = 1, SchoolId = 1, GradeName = "FirstGrade" },
                           new BubbleSheetListItem{ Ticket = "Ticket3", CreatedByUserId = 14, UserId = 14, DistrictId = 1, SchoolId = 1, GradeName = "FirstGrade" },
                           new BubbleSheetListItem{ Ticket = "Ticket2", CreatedByUserId = 10, UserId = 10, DistrictId = 1, SchoolId = 2, GradeName = "SecondGrade" },
                           new BubbleSheetListItem{ Ticket = "Ticket5", CreatedByUserId = 10, UserId = 10, DistrictId = 1, SchoolId = 3, GradeName = "ThirdGrade" },
                           new BubbleSheetListItem{ Ticket = "Ticket6", CreatedByUserId = 10, UserId = 10, DistrictId = 2, SchoolId = 4, GradeName = "ThirdGrade" },
                           new BubbleSheetListItem{ Ticket = "ticket", CreatedByUserId = 10, UserId = 10, DistrictId = 4, SchoolId = 4, GradeName = "ThirdGrade" },
                           new BubbleSheetListItem{ Ticket = "Ticket3", CreatedByUserId = 10, UserId = 10, DistrictId = 4, SchoolId = 4, GradeName = "ThirdGrade" }
                       };
        }

        public IQueryable<BubbleSheetListItem> Select()
        {
            return table.AsQueryable();
        }
    }
}