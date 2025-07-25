using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public  class DistrictRepository : IReadOnlyRepository<District>
    {
        private readonly Table<DistrictEntity> table;

        public DistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<DistrictEntity>();
        }

        public IQueryable<District> Select()
        {
            return table.Select(district => new District
            {
                    Id = district.DistrictID,
                    StateId = district.StateID,
                    Name = district.Name,
                    DistrictGroupId = district.DistrictGroupID ?? 0,
                    LICode = district.LICode
                });
        }
        
    }
}