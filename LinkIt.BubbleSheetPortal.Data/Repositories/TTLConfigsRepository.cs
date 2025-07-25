using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TTLConfigsRepository : IReadOnlyRepository<TTLConfigs>
    {
        private readonly Table<TTLConfigEntity> table;

        public TTLConfigsRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<TTLConfigEntity>();
        }

        public IQueryable<TTLConfigs> Select()
        {
            return table.Select(x => new TTLConfigs
            {
                 ID = x.ID,
                 DynamoTableName = x.DynamoTableName,
                 RetentionInDay = x.RetentionInDay
            });
        }
    }
}
