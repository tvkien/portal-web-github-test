using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pBloomsRepository : IReadOnlyRepository<QTI3pBlooms>
    {
        private readonly Table<QTI3pBloomEntity> table;

        public QTI3pBloomsRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pBloomEntity>();
        }

        public IQueryable<QTI3pBlooms> Select()
        {
            return table.Select(x => new QTI3pBlooms
                                {
                                    BloomsId = x.BloomsID,
                                    Name =  x.Name
                                });
        }
    }
}
