using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pDOKRepository : IReadOnlyRepository<QTI3pDOK>
    {
        private readonly Table<QTI3pDOKEntity> table;

        public QTI3pDOKRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pDOKEntity>();
        }

        public IQueryable<QTI3pDOK> Select()
        {
            return table.Select(x => new QTI3pDOK
                                {
                                    QTI3pDOKID = x.QTI3pDOKID,
                                    Name =  x.Name,
                                    Code = x.Code
                                });
        }
    }
}
