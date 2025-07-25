using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryGenericBubbleSheetRepository : IReadOnlyRepository<GenericBubbleSheet>
    {
        private readonly IEnumerable<GenericBubbleSheet> table;

        public InMemoryGenericBubbleSheetRepository()
        {
            table = AddGenericBubbleSheets();
        }

        private IEnumerable<GenericBubbleSheet> AddGenericBubbleSheets()
        {
            return new List<GenericBubbleSheet>
                {
                    new GenericBubbleSheet{ BubbleSheetFileId = 1, BubbleSheetId = 10, ClassID = 100, FirstName = "First", LastName = "Last", OutputFileName = "OutputFileName", InputFileName = "InputFileName", StudentID = 101, Ticket = "Ticket1" },
                    new GenericBubbleSheet{ BubbleSheetFileId = 2, BubbleSheetId = 11, ClassID = 101, FirstName = "First", LastName = "Last", OutputFileName = "OutputFileName", InputFileName = "InputFileName", StudentID = 102, Ticket = "Ticket1" },
                    new GenericBubbleSheet{ BubbleSheetFileId = 3, BubbleSheetId = 12, ClassID = 102, FirstName = "First", LastName = "Last", OutputFileName = "OutputFileName", InputFileName = "InputFileName", StudentID = 103, Ticket = "Ticket1" },
                    new GenericBubbleSheet{ BubbleSheetFileId = 4, BubbleSheetId = 13, ClassID = 103, FirstName = "First", LastName = "Last", OutputFileName = "OutputFileName", InputFileName = "InputFileName", StudentID = 104, Ticket = "Ticket1" },
                    new GenericBubbleSheet{ BubbleSheetFileId = 5, BubbleSheetId = 14, ClassID = 104, FirstName = "First", LastName = "Last", OutputFileName = "OutputFileName", InputFileName = "InputFileName", StudentID = 105, Ticket = "Ticket1" },
                };
        }

        public IQueryable<GenericBubbleSheet> Select()
        {
            return table.AsQueryable();
        }
    }
}