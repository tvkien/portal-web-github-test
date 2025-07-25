using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassTypeRepository : IReadOnlyRepository<ClassType>
    {
        private readonly Table<ClassTypeEntity> table;

        public ClassTypeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassTypeEntity>();
        }

        public IQueryable<ClassType> Select()
        {
            return table.Select(x => new ClassType
                                         {
                                             Id = x.ClassTypeID,
                                             Name = x.Name
                                         });
        }
    }
}
