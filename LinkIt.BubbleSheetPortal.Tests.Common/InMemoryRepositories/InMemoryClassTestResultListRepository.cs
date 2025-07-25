using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassTestResultListRepository : IReadOnlyRepository<ClassTestResultList>
    {
        private List<ClassTestResultList> table;

        public InMemoryClassTestResultListRepository()
        {
            table = AddClassTestResultList();
        }

        private List<ClassTestResultList> AddClassTestResultList()
        {
            return new List<ClassTestResultList>
                       {
                           new ClassTestResultList{ ClassId = 1, TestCount = 2, TestName = "Some Test" },
                           new ClassTestResultList{ ClassId = 1, TestCount = 2, TestName = "Some Test" },
                           new ClassTestResultList{ ClassId = 1, TestCount = 2, TestName = "Some Test" }
                       };
        }

        public IQueryable<ClassTestResultList> Select()
        {
            return table.AsQueryable();
        }
    }
}