using System;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AblesVirtualTestMappingRepository : IReadOnlyRepository<AblesVirtualTestMapping>
    {
        private readonly Table<AblesVirtualTestMappingEntity> table;

        public AblesVirtualTestMappingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<AblesVirtualTestMappingEntity>();
        }

        public IQueryable<AblesVirtualTestMapping> Select()
        {
            return table.Select(x => new AblesVirtualTestMapping
            {
                AblesTestName = x.AblesTestName,
                Round = x.Round,
                VirtualTestID = x.VirtualTestID,
                ValueMapping = x.ValueMapping,
                DistrictID = x.DistrictID,
                IsASD = x.IsASD ?? false
            });
        }
    }
}
