using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class SchoolTestResultDistrictService
    {
        private readonly IReadOnlyRepository<SchoolTestResultDistrict> repository;

        public SchoolTestResultDistrictService(IReadOnlyRepository<SchoolTestResultDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ListItem> GetSchoolByFilter(int districtId, int virtualtestId, List< int> classIds, int studentId, bool isTeacherOrSchoolAdmin, bool isRegrader)
        {
            var query = repository.Select().Where(x => x.DistrictId.Equals(districtId));
            if(isRegrader)
            {
                query = query.Where(o => o.VirtualTestSourceId != 3);
            }
            if (virtualtestId > 0)
            {
                query = query.Where(o => o.VirtualTestId.Equals(virtualtestId));
            }
             
            if (classIds.Count > 0 || isTeacherOrSchoolAdmin)
            {
                query = query.Where(o =>classIds.Contains(o.ClassId) );
            }
            if (studentId > 0)
            {
                query = query.Where(o => o.StudentId.Equals(studentId));
            }
            return query.OrderBy(o => o.Name)
                .Select(o => new ListItem { Id = o.SchoolId, Name = o.Name })
                .Distinct(); 
        }
    }
}
