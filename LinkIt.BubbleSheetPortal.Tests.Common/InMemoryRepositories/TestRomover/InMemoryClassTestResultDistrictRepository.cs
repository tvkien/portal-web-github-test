using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryClassTestResultDistrictRepository : IReadOnlyRepository<ClassTestResultDistrict>
    {
        private readonly List<ClassTestResultDistrict> table = new List<ClassTestResultDistrict>();

        public InMemoryClassTestResultDistrictRepository()
        {
            table = AddInMemoryClassTestResultDistrictRepository();
        }

        private List<ClassTestResultDistrict> AddInMemoryClassTestResultDistrictRepository()
        {
            return new List<ClassTestResultDistrict>
                    {                           
                        new ClassTestResultDistrict{ClassId = 1, DistrictId = 272, Name = "PLTW 7 9(A-B) 10", SchoolId = 1, UserId = 10, StudentId = 1, UserName = "Monaco0",VirtualTestId = 1, VirtualTestSourceId = 3}    ,
                        new ClassTestResultDistrict{ClassId = 2, DistrictId = 272, Name = "PLTW 7 9(A-B) 9", SchoolId = 2, UserId = 9, StudentId = 2, UserName = "Monaco9",VirtualTestId = 2, VirtualTestSourceId = 1}    ,
                        new ClassTestResultDistrict{ClassId = 3, DistrictId = 272, Name = "PLTW 7 9(A-B) 8", SchoolId = 3, UserId = 8, StudentId = 3, UserName = "Monaco8",VirtualTestId = 3, VirtualTestSourceId = 3}    ,
                        new ClassTestResultDistrict{ClassId = 4, DistrictId = 272, Name = "PLTW 7 9(A-B) 7", SchoolId = 4, UserId = 7, StudentId = 4, UserName = "Monaco7",VirtualTestId = 4, VirtualTestSourceId = 2}    ,
                        new ClassTestResultDistrict{ClassId = 5, DistrictId = 272, Name = "PLTW 7 9(A-B) 6", SchoolId = 5, UserId = 6, StudentId = 5, UserName = "Monaco6",VirtualTestId = 5, VirtualTestSourceId = 3}    ,
                        new ClassTestResultDistrict{ClassId = 6, DistrictId = 272, Name = "PLTW 7 9(A-B) 5", SchoolId = 6, UserId = 5, StudentId = 6, UserName = "Monaco5",VirtualTestId = 6, VirtualTestSourceId = 3}    ,
                        new ClassTestResultDistrict{ClassId = 7, DistrictId = 272, Name = "PLTW 7 9(A-B) 4", SchoolId = 7, UserId = 4, StudentId = 7, UserName = "Monaco4",VirtualTestId = 7, VirtualTestSourceId = 3}    ,
                        new ClassTestResultDistrict{ClassId = 8, DistrictId = 272, Name = "PLTW 7 9(A-B) 3", SchoolId = 8, UserId = 3, StudentId = 8, UserName = "Monaco3",VirtualTestId = 8, VirtualTestSourceId = 4}    ,
                        new ClassTestResultDistrict{ClassId = 9, DistrictId = 272, Name = "PLTW 7 9(A-B) 2", SchoolId = 9, UserId = 2, StudentId = 9, UserName = "Monaco2",VirtualTestId = 9, VirtualTestSourceId = 3}    ,
                        new ClassTestResultDistrict{ClassId = 10, DistrictId = 272, Name = "PLTW 7 9(A-B) 1", SchoolId = 10, UserId = 1, StudentId = 10, UserName = "Monaco1",VirtualTestId = 10, VirtualTestSourceId = 5}    ,
                    };
        }

        public IQueryable<ClassTestResultDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
