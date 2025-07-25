using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictConfigurationRepository : IReadOnlyRepository<DistrictConfiguration>
    {
        private readonly Table<DistrictConfigurationEntity> table;

        public DistrictConfigurationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<DistrictConfigurationEntity>();
        }

        public IQueryable<DistrictConfiguration> Select()
        {
            return table.Select(x => new DistrictConfiguration
            {
                Name = x.Name,
                Value = x.Value,
                DistrictId = x.DistrictID
            });
        }
    }
}
