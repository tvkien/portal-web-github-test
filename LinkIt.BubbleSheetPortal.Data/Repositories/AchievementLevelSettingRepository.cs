using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AchievementLevelSettingRepository : IReadOnlyRepository<AchievementLevelSetting>
    {
        private readonly Table<AchievementLevelSettingEntity> table;

        public AchievementLevelSettingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<AchievementLevelSettingEntity>();
        }

        public IQueryable<AchievementLevelSetting> Select()
        {
            return table.Select(x => new AchievementLevelSetting
            {
                AchievementLevelSettingID = x.AchievementLevelSettingID,
                ValueString = x.ValueString,
                LabelString = x.LabelString,
                Name = x.Name,
                GradeValueString = x.GradedValueString
            });
        }
    }
}