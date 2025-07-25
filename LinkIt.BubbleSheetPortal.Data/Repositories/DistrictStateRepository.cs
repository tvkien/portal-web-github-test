using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictStateRepository : IReadOnlyRepository<DistrictState>
    {
        private readonly Table<DistrictStateView> table;

        public DistrictStateRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<DistrictStateView>();
        }

        public IQueryable<DistrictState> Select()
        {
            return table.Select(x => new DistrictState
            {
                DistrictId = x.DistrictID,
                DistrictNameCustom = x.DistrictNameCustom
            });
        }
    }
}
