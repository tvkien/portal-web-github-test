using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassListRepository : IReadOnlyRepository<ClassList>
    {
        private readonly List<ClassList> table;

        public InMemoryClassListRepository()
        {
            table = AddClassLists();
        }

        private List<ClassList> AddClassLists()
        {
            return new List<ClassList>
                       {
                           new ClassList{ ClassId = 123, SchoolID = 1, UserId = 1},
                           new ClassList{ ClassId = 124, SchoolID = 1, UserId = 1},
                           new ClassList{ ClassId = 125, SchoolID = 1, UserId = 2},
                       };
        }

        public IQueryable<ClassList> Select()
        {
            return table.AsQueryable();
        }
    }
}
