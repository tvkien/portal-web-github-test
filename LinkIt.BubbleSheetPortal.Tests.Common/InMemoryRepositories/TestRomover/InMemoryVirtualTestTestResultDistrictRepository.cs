using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryVirtualTestTestResultDistrictRepository : IReadOnlyRepository<VirtualTestTestResultDistrict>
    {
        private readonly List<VirtualTestTestResultDistrict> table = new List<VirtualTestTestResultDistrict>();

        public InMemoryVirtualTestTestResultDistrictRepository()
        {
            table = AddInMemoryVirtualTestTestResultDistrictRepository();
        }

        private List<VirtualTestTestResultDistrict> AddInMemoryVirtualTestTestResultDistrictRepository()
        {
            return new List<VirtualTestTestResultDistrict>
                    {                           
                        new VirtualTestTestResultDistrict {ClassId = 1, DistrictId = 272, Name = "GK 0708DRA 03 Spring 1", SchoolId = 1, StudentId = 1, VirtualTestId = 5, UserId = 6, VirtualTestSourceId = 3},   
                        new VirtualTestTestResultDistrict {ClassId = 2, DistrictId = 272, Name = "GK 0708DRA 03 Spring 2", SchoolId = 3, StudentId = 2, VirtualTestId = 1, UserId = 7, VirtualTestSourceId = 1},
                        new VirtualTestTestResultDistrict {ClassId = 3, DistrictId = 272, Name = "GK 0708DRA 03 Spring 3", SchoolId = 2, StudentId = 3, VirtualTestId = 5, UserId = 8, VirtualTestSourceId = 3},
                        new VirtualTestTestResultDistrict {ClassId = 4, DistrictId = 272, Name = "GK 0708DRA 03 Spring 4", SchoolId = 3, StudentId = 4, VirtualTestId = 2, UserId = 9, VirtualTestSourceId = 2},
                        new VirtualTestTestResultDistrict {ClassId = 5, DistrictId = 272, Name = "GK 0708DRA 03 Spring 5", SchoolId = 3, StudentId = 5, VirtualTestId = 5, UserId = 10, VirtualTestSourceId = 3},
                        new VirtualTestTestResultDistrict {ClassId = 6, DistrictId = 272, Name = "GK 0708DRA 03 Spring 6", SchoolId = 3, StudentId = 6, VirtualTestId = 3, UserId = 11, VirtualTestSourceId = 4},
                        new VirtualTestTestResultDistrict {ClassId = 7, DistrictId = 272, Name = "GK 0708DRA 03 Spring 7", SchoolId = 4, StudentId = 7, VirtualTestId = 5, UserId = 12, VirtualTestSourceId = 3},
                        new VirtualTestTestResultDistrict {ClassId = 8, DistrictId = 272, Name = "GK 0708DRA 03 Spring 8", SchoolId = 3, StudentId = 8, VirtualTestId = 4, UserId = 13, VirtualTestSourceId = 5},
                        new VirtualTestTestResultDistrict {ClassId = 9, DistrictId = 272, Name = "GK 0708DRA 03 Spring 9", SchoolId = 5, StudentId = 9, VirtualTestId = 5, UserId = 14, VirtualTestSourceId = 3},
                        new VirtualTestTestResultDistrict {ClassId = 10, DistrictId = 272, Name = "GK 0708DRA 03 Spring 10", SchoolId = 3, StudentId = 10, VirtualTestId = 5, UserId = 15, VirtualTestSourceId = 6}
                        
                    };
        }

        public IQueryable<VirtualTestTestResultDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
