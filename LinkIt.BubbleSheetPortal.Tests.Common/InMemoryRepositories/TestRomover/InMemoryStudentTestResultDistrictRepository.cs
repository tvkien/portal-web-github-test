using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryStudentTestResultDistrictRepository : IReadOnlyRepository<StudentTestResultDistrict>
    {
        private readonly List<StudentTestResultDistrict> table = new List<StudentTestResultDistrict>();

        public InMemoryStudentTestResultDistrictRepository()
        {
            table = AddInMemoryStudentTestResultDistrictRepository();
        }

        private List<StudentTestResultDistrict> AddInMemoryStudentTestResultDistrictRepository()
        {
            return new List<StudentTestResultDistrict>
                    {                           
                        new StudentTestResultDistrict{ClassId = 1, DistrictId = 272, SchoolId = 10, StudentCustom = "Ryan Lili 1", StudentId = 1,UserId = 10,VirtualTestId = 1,VirtualTestSourceId = 3}    ,
                        new StudentTestResultDistrict{ClassId = 2, DistrictId = 272, SchoolId = 9, StudentCustom = "Ryan Lili 2", StudentId = 2,UserId = 9,VirtualTestId = 2,VirtualTestSourceId = 1}    ,
                        new StudentTestResultDistrict{ClassId = 3, DistrictId = 272, SchoolId = 8, StudentCustom = "Ryan Lili 3", StudentId = 3,UserId = 8,VirtualTestId = 3,VirtualTestSourceId = 3}    ,
                        new StudentTestResultDistrict{ClassId = 4, DistrictId = 272, SchoolId = 7, StudentCustom = "Ryan Lili 4", StudentId = 4,UserId = 7,VirtualTestId = 4,VirtualTestSourceId = 2}    ,
                        new StudentTestResultDistrict{ClassId = 5, DistrictId = 272, SchoolId = 6, StudentCustom = "Ryan Lili 5", StudentId = 5,UserId = 6,VirtualTestId = 5,VirtualTestSourceId = 3}    ,
                        new StudentTestResultDistrict{ClassId = 6, DistrictId = 272, SchoolId = 5, StudentCustom = "Ryan Lili 6", StudentId = 6,UserId = 5,VirtualTestId = 6,VirtualTestSourceId = 3}    ,
                        new StudentTestResultDistrict{ClassId = 7, DistrictId = 272, SchoolId = 4, StudentCustom = "Ryan Lili 7", StudentId = 7,UserId = 4,VirtualTestId = 7,VirtualTestSourceId = 3}    ,
                        new StudentTestResultDistrict{ClassId = 8, DistrictId = 272, SchoolId = 3, StudentCustom = "Ryan Lili 8", StudentId = 8,UserId = 3,VirtualTestId = 8,VirtualTestSourceId = 4}    ,
                        new StudentTestResultDistrict{ClassId = 9, DistrictId = 272, SchoolId = 2, StudentCustom = "Ryan Lili 9", StudentId = 9,UserId = 2,VirtualTestId = 9,VirtualTestSourceId = 3}    ,
                        new StudentTestResultDistrict{ClassId = 10, DistrictId = 272, SchoolId = 1, StudentCustom = "Ryan Lili 10", StudentId = 10,UserId = 1,VirtualTestId = 10,VirtualTestSourceId = 5}    
                    };
        }

        public IQueryable<StudentTestResultDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
