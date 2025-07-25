using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class LocalizeResourceRepository : IReadOnlyRepository<LocalizeResource>
    {
        private readonly Table<LocalizeResourceEntity> table;

        public LocalizeResourceRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<LocalizeResourceEntity>();
        }

        public IQueryable<LocalizeResource> Select()
        {
            return table.Select(x => new LocalizeResource
            {
                DistrictID = x.DistrictID,
                Key = x.Key,
                Label = x.Label,
                LocalizeResourceID = x.LocalizeResourceID
            });
          
        }
    }
}