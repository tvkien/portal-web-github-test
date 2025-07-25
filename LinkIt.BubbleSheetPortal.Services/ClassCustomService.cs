using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ClassCustomService
    {
        private readonly IReadOnlyRepository<ClassCustom> repository;

        public ClassCustomService(IReadOnlyRepository<ClassCustom> repository)
        {
            this.repository = repository;
        }

        public ClassCustom GetClassById(int classId)
        {
            return repository.Select().FirstOrDefault(x => x.Id == classId);
        }

        public IQueryable<ClassCustom> GetClassesBySchoolIdAndTermIdsAndUserIds(List<int> termIds, List<int> userIds, int schoolId)
        {
            return
                repository.Select()
                    .Where(
                        o => o.SchoolId.Equals(schoolId) && o.DistrictTermId != null && termIds.Contains(o.DistrictTermId.Value) && o.UserId != null && userIds.Contains(o.UserId.Value));
        }
    }
}
