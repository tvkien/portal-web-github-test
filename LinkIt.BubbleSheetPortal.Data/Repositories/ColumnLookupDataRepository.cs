using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ColumnLookupDataRepository : IReadOnlyRepository<ColumnLookupData>
    {
        private readonly Table<ColumnLookupDataEntity> table;

        public ColumnLookupDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<ColumnLookupDataEntity>();
        }

        public IQueryable<ColumnLookupData> Select()
        {
            return table.Select(c => new ColumnLookupData
                {
                    LookupDataID = c.LookupDataID,
                    ColumnID = c.ColumnID,
                    Data = c.Data
                });
        }
    }
}