using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryTermRepository : IReadOnlyRepository<Term>
    {
        private readonly List<Term> table;

        public InMemoryTermRepository()
        {
            table = AddTerms();
        }

        private List<Term> AddTerms()
        {
            return new List<Term>
                       {
                           new Term{ Id = 1, Name = "Term 1", TeacherId = 1},
                           new Term{ Id = 2, Name = "Term 2", TeacherId = 1},
                           new Term{ Id = 3, Name = "Term 3", TeacherId = 2},
                           new Term{ Id = 4, Name = "Term 4", TeacherId = 2},
                           new Term{ Id = 5, Name = "Term 5", TeacherId = 3},
                           new Term{ Id = 6, Name = "Term 6", TeacherId = 3},
                           new Term{ Id = 7, Name = "Term 7", TeacherId = 3},
                       };
        }

        public IQueryable<Term> Select()
        {
            return table.AsQueryable();
        }
    }
}
