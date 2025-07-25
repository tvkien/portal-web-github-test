using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class DisplayTestResultDistrictService
    {
        private readonly IReadOnlyRepository<DisplayTestResultDistrict> repository;

        public DisplayTestResultDistrictService(IReadOnlyRepository<DisplayTestResultDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<DisplayTestResultDistrict> GetTestResultToView(int districtId, int virtualTestId, int schoolId, List< int> classIds, int studentId)
        {
            var query = repository.Select().Where(x => x.DistrictId.Equals(districtId));
            if (virtualTestId > 0)
            {
                query = query.Where(o => o.VirtualTestId == virtualTestId);
            }
            if (schoolId > 0)
            {
                query = query.Where(o => o.SchoolId == schoolId);
            } 
            if (classIds.Count > 0)
            {
                query = query.Where(o => classIds.Contains(o.ClassId));
            }
            if (studentId > 0)
            {
                query = query.Where(o => o.StudentId == studentId);
            } 
            return query.OrderBy(o=>o.TestName)
                .ThenBy(o=>o.SchoolName)
                .ThenBy(o=>o.TeacherCustom)
                .ThenBy(o=>o.ClassNameCustom)
                .ThenBy(o=>o.StudentCustom);
        }
    }
}
