using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class UserDistrictSectorRepository : IReadOnlyRepository<UserDistrictSector>
    {
        private readonly Table<UserDistrictSectorView> table;

        public UserDistrictSectorRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<UserDistrictSectorView>();
        }

        public IQueryable<UserDistrictSector> Select()
        {
            return table.Select(x => new UserDistrictSector
                                {
                                     UserID = x.UserID,
                                     DistrictID = x.DistrictID,
                                     LICode = x.LICode,
                                     Code = x.Code,
                                     Sector = x.Sector,
                                     Email = x.Email
                                });
        }
    }
}
