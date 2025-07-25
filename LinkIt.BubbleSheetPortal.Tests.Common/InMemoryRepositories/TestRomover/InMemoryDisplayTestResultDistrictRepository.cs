using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryDisplayTestResultDistrictRepository : IReadOnlyRepository<DisplayTestResultDistrict>
    {
        private readonly List<DisplayTestResultDistrict> table = new List<DisplayTestResultDistrict>();
        
        public InMemoryDisplayTestResultDistrictRepository()
        {
            table = AddInMemoryDisplayTestResultDistrictRepository();
        }

        private List<DisplayTestResultDistrict> AddInMemoryDisplayTestResultDistrictRepository()
        {
            return new List<DisplayTestResultDistrict>
                    {                           
                        new DisplayTestResultDistrict{AuthorUserId = 1,ClassId = 1,ClassNameCustom = "abc 1",DistrictId = 272,StudentCustom = "Tung 1",  SchoolId = 1,SchoolName = "Tuy Phong 1",StudentId = 1,UserId = 1, VirtualTestId = 1},
                        new DisplayTestResultDistrict{AuthorUserId = 2,ClassId = 2,ClassNameCustom = "abc 2",DistrictId = 272,StudentCustom = "Tung 2",  SchoolId = 2,SchoolName = "Tuy Phong 2",StudentId = 2,UserId = 2, VirtualTestId = 2},
                        new DisplayTestResultDistrict{AuthorUserId = 3,ClassId = 3,ClassNameCustom = "abc 3",DistrictId = 272,StudentCustom = "Tung 3",  SchoolId = 3,SchoolName = "Tuy Phong 3",StudentId = 3,UserId = 3, VirtualTestId = 3},
                        new DisplayTestResultDistrict{AuthorUserId = 4,ClassId = 4,ClassNameCustom = "abc 4",DistrictId = 272,StudentCustom = "Tung 4",  SchoolId = 4,SchoolName = "Tuy Phong 4",StudentId = 4,UserId = 4, VirtualTestId = 4},
                        new DisplayTestResultDistrict{AuthorUserId = 5,ClassId = 5,ClassNameCustom = "abc 5",DistrictId = 272,StudentCustom = "Tung 5",  SchoolId = 5,SchoolName = "Tuy Phong 5",StudentId = 5,UserId = 5, VirtualTestId = 5},
                        new DisplayTestResultDistrict{AuthorUserId = 1,ClassId = 5,ClassNameCustom = "abc 6",DistrictId = 272,StudentCustom = "Tung 1",  SchoolId = 1,SchoolName = "Tuy Phong 1",StudentId = 1,UserId = 5, VirtualTestId = 5},
                        new DisplayTestResultDistrict{AuthorUserId = 2,ClassId = 4,ClassNameCustom = "abc 7",DistrictId = 272,StudentCustom = "Tung 2",  SchoolId = 2,SchoolName = "Tuy Phong 2",StudentId = 2,UserId = 4, VirtualTestId = 4},
                        new DisplayTestResultDistrict{AuthorUserId = 3,ClassId = 3,ClassNameCustom = "abc 8",DistrictId = 272,StudentCustom = "Tung 3",  SchoolId = 3,SchoolName = "Tuy Phong 3",StudentId = 3,UserId = 3, VirtualTestId = 3},
                        new DisplayTestResultDistrict{AuthorUserId = 4,ClassId = 2,ClassNameCustom = "abc 9",DistrictId = 272,StudentCustom = "Tung 4",  SchoolId = 4,SchoolName = "Tuy Phong 4",StudentId = 4,UserId = 2, VirtualTestId = 2},
                        new DisplayTestResultDistrict{AuthorUserId = 5,ClassId = 1,ClassNameCustom = "abc 10",DistrictId = 272,StudentCustom = "Tung 5", SchoolId = 5,SchoolName = "Tuy Phong 5",StudentId = 5,UserId = 1, VirtualTestId = 1},
                    };
        }

        public IQueryable<DisplayTestResultDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}