using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class VirtualTestTestResultDistrictService
    {
        private readonly IReadOnlyRepository<VirtualTestTestResultDistrict> repository;

        public VirtualTestTestResultDistrictService(IReadOnlyRepository<VirtualTestTestResultDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ListItem> GetVirtualTestByFilter(int districtId, List<int> classIds, int studentId, int schoolId, bool isTeacherOrSchoolAdmin, bool isRegrader)
        {
            var query = repository.Select().Where(x => x.DistrictId.Equals(districtId));
            if(isRegrader)
            {
                query = query.Where(o => o.VirtualTestSourceId != 3);
            }
            if (classIds.Count > 0 || isTeacherOrSchoolAdmin)
            {
                query = query.Where(o => classIds.Contains( o.ClassId));
            }
            if (studentId > 0)
            {
                query = query.Where(o => o.StudentId.Equals(studentId));
            }
            if (schoolId > 0)
            {
                query = query.Where(o => o.SchoolId.Equals(schoolId));
            }
             
            return query.OrderBy(o => o.Name)
                .Select(o => new ListItem { Id = o.VirtualTestId, Name = o.Name })
                .Distinct();
        }
    }
}
