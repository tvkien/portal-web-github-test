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
    public class ConfigurationRepository : IReadOnlyRepository<Configuration>
    {
        private readonly Table<ConfigurationEntity> table;

        public ConfigurationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ConfigurationEntity>();
        }

        public IQueryable<Configuration> Select()
        {
            return table.Select(x => new Configuration
                {
                     Name = x.Name,
                     Value = x.Value,
                     Type = x.Type ?? 1
                });
        }
    }
}
