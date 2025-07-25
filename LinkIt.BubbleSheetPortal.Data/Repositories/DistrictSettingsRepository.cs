using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictSettingsRepository : IRepository<DistrictSettings>
    {
        private readonly Table<DistrictSettingEntity> table;
        public DistrictSettingsRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<DistrictSettingEntity>();
            Mapper.CreateMap<DistrictSettings, DistrictSettingEntity>();
        }

        public IQueryable<DistrictSettings> Select()
        {
            return table.Select(x => new DistrictSettings()
                    {
                           DistrictSettingId = x.DistrictSettingID,
                           DistrictId = x.DistrictID,
                           DefaultARSettings = x.DefaultARSettings,
                           TestSettings = x.TestSettings
                    });
        }

        public void Save(DistrictSettings item)
        {
            var entity = table.FirstOrDefault(x => x.DistrictSettingID.Equals(item.DistrictSettingId));

            if (entity.IsNull())
            {
                entity = new DistrictSettingEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.DistrictSettingId = entity.DistrictSettingID;
        }

        public void Delete(DistrictSettings item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.DistrictSettingID.Equals(item.DistrictSettingId));
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }
    }
}
