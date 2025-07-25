using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AchievementLevelSettingService
    {
        private readonly IReadOnlyRepository<RequestParameterDistrict> requestParameterDistrictRepository;
        private readonly IReadOnlyRepository<AchievementLevelSetting> achievementLevelSettingRepository;

        public AchievementLevelSettingService(IReadOnlyRepository<RequestParameterDistrict> requestParameterDistrictRepository, IReadOnlyRepository<AchievementLevelSetting> achievementLevelSettingRepository)
        {
            this.requestParameterDistrictRepository = requestParameterDistrictRepository;
            this.achievementLevelSettingRepository = achievementLevelSettingRepository;
        }

        public IQueryable<AchievementLevelSetting> GetAchievementByDistrictID(int districtID)
        {
            var requestParameterDistrictQuery = requestParameterDistrictRepository.Select().Where(x => x.DistrictID.Equals(districtID)).Distinct();
            List<int> parameterValueList = new List<int>();
            foreach (var rpd in requestParameterDistrictQuery)
            {
                int value;
                int.TryParse(rpd.Value, out value);
                if (value != 0)
                {
                    parameterValueList.Add(value);
                }
            }

            return achievementLevelSettingRepository.Select().Where(a => parameterValueList.Contains(a.AchievementLevelSettingID)).OrderBy(a => a.Name).ThenBy(a => a.LabelString);
        }

        public IQueryable<AchievementLevelSetting> GetByIds(List<int> ids)
        {
            return achievementLevelSettingRepository.Select().Where(x => ids.Contains(x.AchievementLevelSettingID));
        }

        public AchievementLevelSetting GetByName(string name)
        {
            return achievementLevelSettingRepository.Select().FirstOrDefault(x => x.Name == name);
        }

        public IQueryable<AchievementLevelSetting> GetAllAchievementLevelSettings()
        {
            return achievementLevelSettingRepository.Select();
        }
    }
}
