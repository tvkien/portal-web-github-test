using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryGenderRepository : IReadOnlyRepository<Gender>
    {
        private List<Gender> table;

        public InMemoryGenderRepository()
        {
            table = AddGender();
        }

        private List<Gender> AddGender()
        {
            return new List<Gender>
                       {
                           new Gender{ GenderID = 1, Name = "Male", Code = 'M' }, 
                           new Gender{ GenderID = 1, Name = "Female", Code = 'F' }
                       };
        }

        public IQueryable<Gender> Select()
        {
            return table.AsQueryable();
        }
    }
}