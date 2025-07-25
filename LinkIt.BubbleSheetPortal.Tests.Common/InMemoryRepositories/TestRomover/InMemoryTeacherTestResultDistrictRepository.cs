using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.TestRomover
{
    public class InMemoryTeacherTestResultDistrictRepository : IReadOnlyRepository<TeacherTestResultDistrict>
    {
        private readonly List<TeacherTestResultDistrict> table = new List<TeacherTestResultDistrict>();

        public InMemoryTeacherTestResultDistrictRepository()
        {
            table = AddInMemoryTeacherTestResultDistrictRepository();
        }

        private List<TeacherTestResultDistrict> AddInMemoryTeacherTestResultDistrictRepository()
        {
            return new List<TeacherTestResultDistrict>
                    {                           
                        new TeacherTestResultDistrict {ClassId = 1, DistrictId = 272, SchoolId = 1, UserId = 1, UserName = "miller1", StudentId = 1, VirtualTestId = 1, VirtualTestSourceId = 3}   ,
                        new TeacherTestResultDistrict {ClassId = 2, DistrictId = 272, SchoolId = 2, UserId = 2, UserName = "miller2", StudentId = 2, VirtualTestId = 2, VirtualTestSourceId = 1}   ,
                        new TeacherTestResultDistrict {ClassId = 3, DistrictId = 272, SchoolId = 3, UserId = 3, UserName = "miller3", StudentId = 3, VirtualTestId = 3, VirtualTestSourceId = 3}   ,
                        new TeacherTestResultDistrict {ClassId = 4, DistrictId = 272, SchoolId = 4, UserId = 4, UserName = "miller4", StudentId = 4, VirtualTestId = 4, VirtualTestSourceId = 2}   ,
                        new TeacherTestResultDistrict {ClassId = 5, DistrictId = 272, SchoolId = 5, UserId = 5, UserName = "miller5", StudentId = 5, VirtualTestId = 5, VirtualTestSourceId = 3}   ,
                        new TeacherTestResultDistrict {ClassId = 6, DistrictId = 272, SchoolId = 6, UserId = 6, UserName = "miller6", StudentId = 6, VirtualTestId = 6, VirtualTestSourceId = 3}   ,
                        new TeacherTestResultDistrict {ClassId = 7, DistrictId = 272, SchoolId = 7, UserId = 7, UserName = "miller7", StudentId = 7, VirtualTestId = 7, VirtualTestSourceId = 3}   ,
                        new TeacherTestResultDistrict {ClassId = 8, DistrictId = 272, SchoolId = 8, UserId = 8, UserName = "miller8", StudentId = 8, VirtualTestId = 8, VirtualTestSourceId = 4}   ,
                        new TeacherTestResultDistrict {ClassId = 9, DistrictId = 272, SchoolId = 9, UserId = 9, UserName = "miller9", StudentId = 9, VirtualTestId = 9, VirtualTestSourceId = 3}   ,
                        new TeacherTestResultDistrict {ClassId = 10, DistrictId = 272, SchoolId = 10, UserId = 10, UserName = "miller10", StudentId = 10, VirtualTestId = 10, VirtualTestSourceId = 5}   
                    };
        }

        public IQueryable<TeacherTestResultDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
