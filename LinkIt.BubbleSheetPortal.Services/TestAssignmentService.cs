using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class TestAssignmentService
    {
        private readonly IRepository<TestAssignment> repository;
        private readonly IReadOnlyRepository<ClassCustomForGroup> classCustomForGroupRepository;

        public TestAssignmentService(IRepository<TestAssignment> repository, IReadOnlyRepository<ClassCustomForGroup> classCustomForGroupRepository)
        {
            this.repository = repository;
            this.classCustomForGroupRepository = classCustomForGroupRepository;
        }

        public IQueryable<TestAssignment> GetTestAssignments()
        {
            return repository.Select();
        }

        public IQueryable<TestAssignment> GetTestAssignmentByClassId(int classId)
        {
            return repository.Select().Where(en=>en.ClassId == classId);
        }

        public IQueryable<TestAssignment> GetTestAssignmentByClassIdList(List<int> listClassIds)
        {
            return repository.Select().Where(x => listClassIds.Contains(x.ClassId));
        }

        public ClassCustomForGroup GetClassById(int classId)
        {
            return classCustomForGroupRepository.Select().FirstOrDefault(x => x.Id == classId);
        }

        public IQueryable<ClassCustomForGroup> GetClassesBySchoolIdAndTermIdsAndUserIds(List<int> termIds, List<int> userIds, int schoolId)
        {
            return classCustomForGroupRepository.Select()
                    .Where(o => o.SchoolId.Equals(schoolId) && o.DistrictTermId != null && termIds.Contains(o.DistrictTermId.Value) && o.UserId != null && userIds.Contains(o.UserId.Value));
        }
    }
}
