using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class GenderRepository : IReadOnlyRepository<Gender>
    {
        private readonly Table<GenderEntity> table;

        public GenderRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<GenderEntity>();
        }

        public IQueryable<Gender> Select()
        {
            return table.Select(x => new Gender
                {
                    GenderID = x.GenderID,
                    Name = x.Name,
                    Code = x.Code
                });
        }
    }
}