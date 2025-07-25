using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryStateRepository : IReadOnlyRepository<State>
    {
        private readonly List<State> table = new List<State>();

        public InMemoryStateRepository()
        {
            table = AddStates();
        }

        private List<State> AddStates()
        {
            return new List<State>
                       {
                           new State { Id = 1, Code = "LA", Name = "Louisiana" },
                           new State { Id = 2, Code = "AL", Name = "Alabama" },
                           new State { Id = 3, Code = "MS", Name = "Mississippi" },
                           new State { Id = 4, Code = "FL", Name = "Florida" },
                           new State { Id = 5, Code = "TX", Name = "Texas" },
                           new State { Id = 6, Code = "GA", Name = "Georgia" },
                           new State { Id = 7, Code = "CA" }
                       };
        }

        public IQueryable<State> Select()
        {
            return table.AsQueryable();
        }
    }
}