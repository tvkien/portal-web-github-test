using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictDecodeRepository : IReadOnlyRepository<DistrictDecode>
    {
        private readonly Table<DistrictDecodeEntity> table;

        public DistrictDecodeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<DistrictDecodeEntity>();
        }
        public IQueryable<DistrictDecode> Select()
        {
            return table.Select(x => new DistrictDecode
                                     {
                                         DistrictDecodeID = x.DistrictDeCodeID,
                                         DistrictID = x.DistrictID,
                                         Label = x.Label,
                                         Value = x.Value
                                     });
        }
    }
}
