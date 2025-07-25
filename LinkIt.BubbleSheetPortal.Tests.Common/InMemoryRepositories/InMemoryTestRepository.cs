using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryTestRepository : IReadOnlyRepository<Test>
    {
        private readonly List<Test> table = new List<Test>();

        public InMemoryTestRepository()
        {
            table = AddTests();
        }

        private static List<Test> AddTests()
        {
            return new List<Test>
                       {
                           new Test{ Id = 1, Name = "Test 1", BankId = 1, QuestionCount = 10, StateId = 1, Type = 3},
                           new Test{ Id = 2, Name = "Test 2", BankId = 1, QuestionCount = 5, StateId = 1, Type = 3}
                       };
        }

        public IQueryable<Test> Select()
        {
            return table.AsQueryable();
            ;
        }
    }
}
