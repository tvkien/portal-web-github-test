using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetReviewDetailsRepository : IReadOnlyRepository<BubbleSheetReviewDetails>
    {
        private readonly List<BubbleSheetReviewDetails> table;

        public InMemoryBubbleSheetReviewDetailsRepository()
        {
            table = AddBubbleSheetReviewDetails();
        }

        private List<BubbleSheetReviewDetails> AddBubbleSheetReviewDetails()
        {
            return new List<BubbleSheetReviewDetails>
                       {
                           new BubbleSheetReviewDetails { StudentName = "Student 1", StudentId = 10, Ticket = "Ticket1", CreatedByUserId = 5 },
                           new BubbleSheetReviewDetails { StudentName = "Student 2", BubbleSheetFileId = 1, StudentId = 10, Ticket = "Ticket2", CreatedByUserId = 2 },
                           new BubbleSheetReviewDetails { StudentName = "Student 3", StudentId = 10, Ticket = "Ticket3", CreatedByUserId = 10 },
                           new BubbleSheetReviewDetails { StudentName = "Student 4", StudentId = 10, Ticket = "Ticket4", CreatedByUserId = 4 },
                           new BubbleSheetReviewDetails { StudentName = "Student 5", BubbleSheetFileId = 2, StudentId = 10, Ticket = "Ticket2", CreatedByUserId = 6, UploadedDate = DateTime.Today },
                           new BubbleSheetReviewDetails { StudentName = "Student 6", StudentId = 10, Ticket = "Ticket4", CreatedByUserId = 3 },
                           new BubbleSheetReviewDetails { StudentName = "Student 7", StudentId = 11, Ticket = "Ticket1", CreatedByUserId = 6 },
                           new BubbleSheetReviewDetails { StudentName = "Student 8", StudentId = 11, Ticket = "Ticket7", CreatedByUserId = 5 },
                           new BubbleSheetReviewDetails { StudentName = "Student 9", StudentId = 11, Ticket = "Ticket3", CreatedByUserId = 10 },
                           new BubbleSheetReviewDetails { StudentName = "Open Ended", StudentId = 250, Ticket = "open-ended-ticket", BubbleSheetId = 250, ClassId = 250, CreatedByUserId = 10 },
                       };
        }

        public IQueryable<BubbleSheetReviewDetails> Select()
        {
            return table.AsQueryable();
        }
    }
}