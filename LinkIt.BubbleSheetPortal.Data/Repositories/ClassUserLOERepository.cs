using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassUserLOERepository : IReadOnlyRepository<ClassUserLOE>
    {
        private readonly Table<ClassUserLOEEntity> table;

        public ClassUserLOERepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassUserLOEEntity>();
        }

        public IQueryable<ClassUserLOE> Select()
        {
            return table.Select(x => new ClassUserLOE
                                         {
                                             Id = x.ClassUserLOEID,
                                             Name = x.Name
                                         });
        }
    }
}