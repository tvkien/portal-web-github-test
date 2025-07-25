using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictTermClassService
    {
        private readonly IReadOnlyRepository<DistrictTermClass> repository;

        public DistrictTermClassService(IReadOnlyRepository<DistrictTermClass> repository)
        {
            this.repository = repository;
        }

        public DistrictTermClass GetDistrictTermClassById(int userId, int schoolId, int districtId)
        {
            return repository.Select().FirstOrDefault(o => o.UserId == userId && o.SchoolId == schoolId && o.DistrictId == districtId);
        }

        public bool SchoolHaveActiveClass(int userId, int schoolId, int districtId)
        {
            var districtTermClass = repository.Select().FirstOrDefault(o => o.UserId == userId && o.SchoolId == schoolId && o.DistrictId == districtId);
            if (districtTermClass.IsNotNull())
            {
                DateTime today = DateTime.Now.Date;
                return districtTermClass.DateStart.Date <= today && today <= districtTermClass.DateEnd;
            }
            return false;
        }

        public bool SchoolHaveClassNotStartYet(int userId, int schoolId, int districtId)
        {
            var districtTermClass = repository.Select().FirstOrDefault(o => o.UserId == userId && o.SchoolId == schoolId && o.DistrictId == districtId);
            if (districtTermClass.IsNotNull())
            {
                DateTime today = DateTime.Now.Date;
                return today < districtTermClass.DateStart.Date;
            }
            return false;
        }
        public IQueryable<DistrictTermClass> GetAll()
        {
            return repository.Select();
        }
    }
}