using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolStudentRegistrationService
    {
        private readonly IRepository<SchoolStudentRegistration> repository;

        public SchoolStudentRegistrationService(IRepository<SchoolStudentRegistration> repository)
        {
            this.repository = repository;
        }

        public SchoolStudentRegistration GetSchoolById(int schoolId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(schoolId));
        }

        public IQueryable<SchoolStudentRegistration> GetSchoolsByDistrictId(int districtId)
        {
            return repository.Select().Where(x => x.DistrictId.Equals(districtId)).OrderBy(x => x.Name).ThenBy(x => x.Code).ThenBy(x => x.StateCode);
        }

        public IQueryable<SchoolStudentRegistration> GetSchoolsByDistrictIdAndLocationCode(int districtId, string locationCode)
        {
            return repository.Select().Where(x => x.DistrictId.Equals(districtId) && x.LocationCode == locationCode);
        }

        public void Save(SchoolStudentRegistration item)
        {
            repository.Save(item);
        }

        public void Delete(SchoolStudentRegistration item)
        {
            repository.Delete(item);
        }

        public SchoolStudentRegistration GetSchoolByNames(string schoolName, int districtId)
        {
            var query = repository.Select().Where(o => o.DistrictId == districtId && o.Name.Equals(schoolName));
            if (query.Any())
                return query.FirstOrDefault();
            return null;
        }

        public IQueryable<SchoolStudentRegistration> GetAll()
        {
            return repository.Select();
        }
    }
}