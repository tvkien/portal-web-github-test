using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictSettingsService
    {
        private readonly IRepository<DistrictSettings> _districtSettingsRepository;

        public DistrictSettingsService(IRepository<DistrictSettings> districtSettingsRepository)
        {
            _districtSettingsRepository = districtSettingsRepository;
        }

        public DistrictSettings GetDistrictSettingByDistrictId(int districtId)
        {
           return _districtSettingsRepository.Select().FirstOrDefault(o => o.DistrictId == districtId);
        }

        /// <summary>
        /// Check exist Record with districtId
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool SaveDistrictSettings(DistrictSettings item)
        {
            if (item != null)
            {
                var obj = _districtSettingsRepository.Select().FirstOrDefault(o => o.DistrictId == item.DistrictId);
                if (obj != null)
                {
                    item.DistrictSettingId = obj.DistrictSettingId;
                }
                _districtSettingsRepository.Save(item);
            }
            return false;
        }
    }
}
