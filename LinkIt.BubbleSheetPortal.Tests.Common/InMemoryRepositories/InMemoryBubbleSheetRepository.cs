using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetRepository : IBubbleSheetRepository
    {
        private readonly List<BubbleSheet> table = new List<BubbleSheet>();
        private static int index = 16;

        public InMemoryBubbleSheetRepository()
        {
            table = AddBubbleSheets();
        }

        private static List<BubbleSheet> AddBubbleSheets()
        {
            return new List<BubbleSheet>
                       {
                           new BubbleSheet{ Id = 12, BubbleSheetCode = "code", BubbleSize = "Small", ClassId = 123, CreatedByUserId = 699, DistrictTermId = 123, SchoolId = 12, StudentId = 12512, TeacherId = 145, Ticket = "ticket", UserId = 256, TestId = 14225, IsArchived = false},
                           new BubbleSheet{ Id = 13, BubbleSheetCode = "code", BubbleSize = "Medium", ClassId = 123, CreatedByUserId = 699, DistrictTermId = 123, SchoolId = 12, StudentId = 12515, TeacherId = 145, Ticket = "ticket", UserId = 256, TestId = 14225, IsArchived = false},
                           new BubbleSheet{ Id = 14, BubbleSheetCode = "code", BubbleSize = "Small", ClassId = 123, CreatedByUserId = 699, DistrictTermId = 123, SchoolId = 13, StudentId = 12517, TeacherId = 145, Ticket = "ticket", UserId = 256, TestId = 14225, IsArchived = false},
                           new BubbleSheet{ Id = 15, BubbleSheetCode = "code", BubbleSize = "Small", ClassId = 123, CreatedByUserId = 699, DistrictTermId = 123, SchoolId = 16, StudentId = 12513, TeacherId = 145, Ticket = "ticket", UserId = 256, TestId = 14225, IsArchived = false},
                           new BubbleSheet{ Id = 17, BubbleSheetCode = "code", BubbleSize = "Small", ClassId = 123, CreatedByUserId = 699, DistrictTermId = 123, SchoolId = 16, StudentId = 12513, TeacherId = 145, Ticket = "ticket", UserId = 256, TestId = 14225, IsArchived = false},
                           new BubbleSheet{ Id = 17, BubbleSheetCode = "code2", BubbleSize = "Small", ClassId = 124, CreatedByUserId = 699, DistrictTermId = 124, SchoolId = 18, StudentId = 0, TeacherId = 200, Ticket = "ticket2", IsGenericSheet = true, UserId = 55, TestId = 156, IsArchived = false},
                           new BubbleSheet{ Id = 125, BubbleSheetCode = "code2", BubbleSize = "Small", ClassId = 125, CreatedByUserId = 699, DistrictTermId = 124, SchoolId = 18, StudentId = 0, TeacherId = 200, Ticket = "ticket2", IsGenericSheet = true, UserId = 55, TestId = 156, IsArchived = false},
                           new BubbleSheet{ Id = 125, BubbleSheetCode = "code2", BubbleSize = "Small", ClassId = 125, CreatedByUserId = 699, DistrictTermId = 124, SchoolId = 18, StudentId = 0, TeacherId = 200, Ticket = "ticket4", IsGenericSheet = true, UserId = 55, TestId = 156, IsArchived = false},
                           new BubbleSheet{ Id = 125, BubbleSheetCode = "code2", BubbleSize = "Small", ClassId = 125, CreatedByUserId = 699, DistrictTermId = 124, SchoolId = 18, StudentId = 0, TeacherId = 200, Ticket = "ticket5", IsGenericSheet = false, UserId = 55, TestId = 156, IsArchived = false},
                           new BubbleSheet{ Id = 250, Ticket = "open-ended-ticket" }
                       };
        }

        public IQueryable<BubbleSheet> Select()
        {
            return table.AsQueryable();
        }

        public void Save(BubbleSheet item)
        {
            item.Id = index++;
            table.Add(item);
        }

        public void Delete(BubbleSheet item)
        {
            table.Remove(item);
        }

        public void ToggleArchiveBubbleSheets(IEnumerable<BubbleSheet> bubbleSheets)
        {
            
        }

        public void UpdateBubbleSheetsWithTicket(IEnumerable<BubbleSheet> bubbleSheets, string ticket)
        {
            foreach (var entity in bubbleSheets.Select(bubbleSheet => table.FirstOrDefault(x => x.Id == bubbleSheet.Id)))
            {
                entity.Ticket = ticket;
            }
        }

        public void Save(IList<BubbleSheet> listBubbleSheets)
        {
            foreach (var item in listBubbleSheets)
            {
                item.Id = index++;
                table.Add(item);
            }
        }
    }
}
