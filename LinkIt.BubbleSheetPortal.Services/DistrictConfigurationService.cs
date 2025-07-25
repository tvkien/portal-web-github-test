using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictConfigurationService
    {
        private readonly IReadOnlyRepository<DistrictConfiguration> repository;

        public DistrictConfigurationService(IReadOnlyRepository<DistrictConfiguration> repository)
        {
            this.repository = repository;
        }

        public DistrictConfiguration GetDistrictConfigurationByKey(int districtId, string key)
        {
            // Return default configuration value (value of districtId = 0) if this district does not have this key
            return repository.Select()
                .Where(o => (o.DistrictId == districtId || o.DistrictId == 0) && o.Name.Equals(key))
                .OrderByDescending(o => o.DistrictId)
                .FirstOrDefault();
        }

        public List<DistrictConfiguration> GetDistrictConfigurationListByKey(int districtId, string key)
        {
            return repository.Select()
                .Where(o => o.DistrictId == 0 && o.Name.Equals(key)).ToList();
        }
    }
}
