using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AutoFocusGroupConfigRepository : IAutoFocusGroupConfigRepository
    {
        private readonly Table<AutoFocusGroupConfigEntity> table;
        public AutoFocusGroupConfigRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<AutoFocusGroupConfigEntity>();
        }

        public AutoFocusGroupConfig GetConfigByDistrictID(int districtID)
        {
            var result = table.Where(o => o.DistrictID == districtID).Select(x => new AutoFocusGroupConfig
            {
                AutoFocusGroupConfigID = x.AutoFocusGroupConfigID,
                DistrictID = x.DistrictID,
                AutoGroupType = x.AutoGroupType,
                JSONConfig = x.JSONConfig,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                ScheduledTime = x.ScheduledTime,
                LastRunDate = x.LastRunDate,
                Run = x.Run
            }).FirstOrDefault();

            return result;
        }

        public IQueryable<AutoFocusGroupConfig> Select()
        {
            return table.Select(x => new AutoFocusGroupConfig
            {
                AutoFocusGroupConfigID = x.AutoFocusGroupConfigID,
                DistrictID = x.DistrictID,
                AutoGroupType = x.AutoGroupType,
                JSONConfig = x.JSONConfig,
                CreatedDate = x.CreatedDate,
                UpdatedDate = x.UpdatedDate,
                ScheduledTime = x.ScheduledTime,
                LastRunDate = x.LastRunDate,
                Run = x.Run
            });
        }
    }
}
