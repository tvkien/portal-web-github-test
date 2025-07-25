using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryVirtualTestWithOutTestResultRepository : IReadOnlyRepository<VirtualTestWithOutTestResult>
    {
        private readonly List<VirtualTestWithOutTestResult> table = new List<VirtualTestWithOutTestResult>();

        public InMemoryVirtualTestWithOutTestResultRepository()
        {
            table = AddInMemoryVirtualTestWithOutTestResultRepository();
        }

        private List<VirtualTestWithOutTestResult> AddInMemoryVirtualTestWithOutTestResultRepository()
        {
            return new List<VirtualTestWithOutTestResult>
                    {                           
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 3", VirtualTestId = 2253},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 4", VirtualTestId = 2254},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 5", VirtualTestId = 2255},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 6", VirtualTestId = 2256},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 7", VirtualTestId = 2257},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 8", VirtualTestId = 2258},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 9", VirtualTestId = 2259},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 0", VirtualTestId = 2250},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 1", VirtualTestId = 2251},
                        new VirtualTestWithOutTestResult {AuthorUserId = 856, DistrictId = 272, Name = "GK 0708DRA 01 Fall 2", VirtualTestId = 2252},
                    };
        }

        public IQueryable<VirtualTestWithOutTestResult> Select()
        {
            return table.AsQueryable();
        }
    }
}
