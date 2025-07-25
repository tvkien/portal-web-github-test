using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemorySchoolTestResultDistrictRepository : IReadOnlyRepository<SchoolTestResultDistrict>
    {
        private readonly List<SchoolTestResultDistrict> table = new List<SchoolTestResultDistrict>();

        public InMemorySchoolTestResultDistrictRepository()
        {
            table = AddInMemorySchoolTestResultDistrictRepository();
        }

        private List<SchoolTestResultDistrict> AddInMemorySchoolTestResultDistrictRepository()
        {
            return new List<SchoolTestResultDistrict>
                    {                           
                        new SchoolTestResultDistrict{ClassId = 1, DistrictId = 272, Name = "Tackan Elementary 1", SchoolId = 1, StudentId = 10, UserId = 1, VirtualTestId = 10, VirtualTestSourceId = 3}    ,
                        new SchoolTestResultDistrict{ClassId = 2, DistrictId = 272, Name = "Tackan Elementary 2", SchoolId = 2, StudentId = 9, UserId = 2, VirtualTestId = 9, VirtualTestSourceId = 1}    ,
                        new SchoolTestResultDistrict{ClassId = 3, DistrictId = 272, Name = "Tackan Elementary 3", SchoolId = 3, StudentId = 8, UserId = 3, VirtualTestId = 8, VirtualTestSourceId = 3}    ,
                        new SchoolTestResultDistrict{ClassId = 4, DistrictId = 272, Name = "Tackan Elementary 4", SchoolId = 4, StudentId = 7, UserId = 4, VirtualTestId = 7, VirtualTestSourceId = 2}    ,
                        new SchoolTestResultDistrict{ClassId = 5, DistrictId = 272, Name = "Tackan Elementary 5", SchoolId = 5, StudentId = 6, UserId = 5, VirtualTestId = 6, VirtualTestSourceId = 3}    ,
                        new SchoolTestResultDistrict{ClassId = 6, DistrictId = 272, Name = "Tackan Elementary 6", SchoolId = 6, StudentId = 5, UserId = 6, VirtualTestId = 5, VirtualTestSourceId = 3}    ,
                        new SchoolTestResultDistrict{ClassId = 7, DistrictId = 272, Name = "Tackan Elementary 7", SchoolId = 7, StudentId = 4, UserId = 7, VirtualTestId = 4, VirtualTestSourceId = 3}    ,
                        new SchoolTestResultDistrict{ClassId = 8, DistrictId = 272, Name = "Tackan Elementary 8", SchoolId = 8, StudentId = 3, UserId = 8, VirtualTestId = 3, VirtualTestSourceId = 4}    ,
                        new SchoolTestResultDistrict{ClassId = 9, DistrictId = 272, Name = "Tackan Elementary 9", SchoolId = 9, StudentId = 2, UserId = 9, VirtualTestId = 2, VirtualTestSourceId = 3}    ,
                        new SchoolTestResultDistrict{ClassId = 10, DistrictId = 272, Name = "Tackan Elementary 10", SchoolId = 10, StudentId = 1, UserId = 10, VirtualTestId = 1, VirtualTestSourceId = 5}    
                    };
        }

        public IQueryable<SchoolTestResultDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
