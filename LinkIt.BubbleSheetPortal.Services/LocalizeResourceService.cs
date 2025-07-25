using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class LocalizeResourceService
    {
        private readonly IReadOnlyRepository<LocalizeResource> _repository;

        public LocalizeResourceService(IReadOnlyRepository<LocalizeResource> repository)
        {
            _repository = repository;
        }

        public string GetLabelByKey(int districtID, string key)
        {
            var resources = _repository.Select().Where(x => (x.DistrictID == districtID || x.DistrictID.HasValue == false) && x.Key == key);
            if(resources != null && resources.Count() > 0)
            {
                var localeByDistrict = resources.FirstOrDefault(x => x.DistrictID.HasValue && x.DistrictID.Value == districtID);
                if(localeByDistrict == null)
                {
                    localeByDistrict = resources.FirstOrDefault(x => x.DistrictID.HasValue == false);
                }

                return localeByDistrict.Label;
            }

            return key;
        }
    }
}
