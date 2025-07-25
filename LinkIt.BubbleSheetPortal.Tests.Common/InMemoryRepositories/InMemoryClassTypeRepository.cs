using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassTypeRepository : IReadOnlyRepository<ClassType>
    {
        private List<ClassType> table;

        public InMemoryClassTypeRepository()
        {
            table = AddClassType();
        }

        private List<ClassType> AddClassType()
        {
            return new List<ClassType>
                       {
                           new ClassType{ Id = 1, Name = "Reading"},
                           new ClassType{ Id = 2, Name = "Science"},
                           new ClassType{ Id = 3, Name = "Math"}
                       };
        }

        public IQueryable<ClassType> Select()
        {
            return table.AsQueryable();
        }
    }
}