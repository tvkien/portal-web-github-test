using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.DistrictReferenceData
{
    public class InMemoryAchievementLevelSettingRepository : IReadOnlyRepository<AchievementLevelSetting>
    {
        private readonly List<AchievementLevelSetting> table;

        public InMemoryAchievementLevelSettingRepository()
        {
            table = AddAchievementLevelSettings();
        }

        private List<AchievementLevelSetting> AddAchievementLevelSettings()
        {
            return new List<AchievementLevelSetting>()
            {
                new AchievementLevelSetting{ AchievementLevelSettingID = 1, GradeValueString = "3;2;1", LabelString = "Advanced;Proficient;Basic;NA", Name = "NJPass", ValueString = "3;2;1;0" },
                new AchievementLevelSetting{ AchievementLevelSettingID = 2, GradeValueString = "1;2;3;4", LabelString = "Advanced;Proficient;Partial;NA", Name = "NJASK", ValueString = "1;2;3;4" },
                new AchievementLevelSetting{ AchievementLevelSettingID = 3, GradeValueString = "4;3;2;1;0", LabelString = "Advanced;Proficient;Basic;Below Basic;NA", Name = "Default", ValueString = "4;3;2;1;0" },
                new AchievementLevelSetting{ AchievementLevelSettingID = 4, GradeValueString = "5;4;3;2;1;9", LabelString = "Advanced;Proficient;Basic;Below basic;Far below basic;Did not attempt", Name = "STAR", ValueString = "5;4;3;2;1;9" },
                new AchievementLevelSetting{ AchievementLevelSettingID = 5, GradeValueString = "1;2;3;4", LabelString = "P;A;B;U", Name = "INHS1", ValueString = "1;2;3;4" }
            };
        }

        public IQueryable<AchievementLevelSetting> Select()
        {
            return table.AsQueryable();
        }
    }
}
