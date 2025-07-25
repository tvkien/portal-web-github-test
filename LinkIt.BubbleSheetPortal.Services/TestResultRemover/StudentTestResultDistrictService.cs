using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class StudentTestResultDistrictService
    {
        private readonly IReadOnlyRepository<StudentTestResultDistrict> repository;

        public StudentTestResultDistrictService(IReadOnlyRepository<StudentTestResultDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ListItem> GetStudentByFilter(int districtId, int virtualtestId, List<int> classIds, int schoolId, bool isTeacherOrSchoolAdmin, bool isRegrader)
        {
            var query = repository.Select().Where(x => x.DistrictId.Equals(districtId));
            if (virtualtestId > 0)
            {
                query = query.Where(o => o.VirtualTestId.Equals(virtualtestId));
            }
            if (classIds.Count > 0 || isTeacherOrSchoolAdmin)
            {
                query = query.Where(o => classIds.Contains( o.ClassId));
            } 
            if (schoolId > 0)
            {
                query = query.Where(o => o.SchoolId.Equals(schoolId));
            }
            if(isRegrader)
            {
                query = query.Where(o => o.VirtualTestSourceId != 3);
            }
            var lst = query.
                Select(o => new ListItem { Id = o.StudentId, Name = o.StudentCustom })
                .Distinct()
                .OrderBy(o => o.Name);
            return lst;
        }
    }
}
