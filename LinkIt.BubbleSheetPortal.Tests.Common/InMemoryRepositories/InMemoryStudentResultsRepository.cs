using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStudentResultsRepository : IReadOnlyRepository<BubbleSheetStudentResults>
    {
        private readonly List<BubbleSheetStudentResults> table;

        public InMemoryStudentResultsRepository()
        {
            table = AddBubbleSheetStudentResults();
        }

        private List<BubbleSheetStudentResults> AddBubbleSheetStudentResults()
        {
            return new List<BubbleSheetStudentResults>
                       {
                           new BubbleSheetStudentResults{Ticket = "Ticket3", Status = "Error", StudentId = 4, StudentName = "Student1"},
                           new BubbleSheetStudentResults{Ticket = "Ticket4", Status = "Valid", StudentId = 5, StudentName = "Student2"},
                           new BubbleSheetStudentResults{Ticket = "Ticket3", Status = "Invalid", StudentId = 7, StudentName = "Student3"},
                           new BubbleSheetStudentResults{Ticket = "Ticket5", Status = "Missing", StudentId = 8, StudentName = "Student4"},
                           new BubbleSheetStudentResults{Ticket = "Ticket6", Status = "Invalid", StudentId = 6, StudentName = "Student5"},
                           new BubbleSheetStudentResults{Ticket = "Ticket1", ClassId = 123, Status = "Valid", StudentId = 7, StudentName = "Student6"},
                           new BubbleSheetStudentResults{Ticket = "Ticket1", ClassId = 123, Status = "Missing", StudentId = 8, StudentName = "Student7"},
                           new BubbleSheetStudentResults{Ticket = "Ticket1", ClassId = 123, Status = "Valid", StudentId = 9, StudentName = "Student8"},
                           new BubbleSheetStudentResults{Ticket = "ticket", ClassId = 123, Status = "Valid", StudentId = 9, StudentName = "Student7"},
                           new BubbleSheetStudentResults{Ticket = "ticket", ClassId = 123, Status = "Valid", StudentId = 9, StudentName = "Student6"},
                           new BubbleSheetStudentResults{Ticket = "ticket", ClassId = 123, Status = "Valid", StudentId = 9, StudentName = "Student5"},
                           new BubbleSheetStudentResults{Ticket = "ticket", ClassId = 123, Status = "Valid", StudentId = 9, StudentName = "Student4"}
                       };
        }

        public IQueryable<BubbleSheetStudentResults> Select()
        {
            return table.AsQueryable();
        }
    }
}