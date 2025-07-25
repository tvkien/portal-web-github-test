using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Services.TestResultRemover
{
    public class ClassTestResultDistrictService
    {
        private readonly IReadOnlyRepository<ClassTestResultDistrict> repository;

        public ClassTestResultDistrictService(IReadOnlyRepository<ClassTestResultDistrict> repository)
        {
            this.repository = repository;
        }

        public IQueryable<ClassTestResultDistrict> GetClassByFilter(int districtId, int virtualtestId, int studentId, int schoolId, List<int> classIds, bool isTeacherOrSchoolAdmin, bool isRegrader)
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
             
            if (studentId > 0)
            {
                query = query.Where(o => o.StudentId.Equals(studentId));
            }
            if (classIds.Count > 0 || isTeacherOrSchoolAdmin)
            {
                query = query.Where(o=> classIds.Contains(o.ClassId));
            }
            var lst = query;
                //Select(o => new ListItem { Id = o.ClassId, Name = o.Name})
                //.Distinct()
                //.OrderBy(o => o.Name);
            return lst;
        }
    }
}
