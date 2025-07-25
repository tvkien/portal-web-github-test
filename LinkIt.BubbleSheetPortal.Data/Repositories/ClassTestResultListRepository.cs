using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassTestResultListRepository : IReadOnlyRepository<ClassTestResultList>
    {
        private readonly Table<ClassTestResultListView> table;

        public ClassTestResultListRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassTestResultListView>();
        }

        public IQueryable<ClassTestResultList> Select()
        {
            return table.Select(x => new ClassTestResultList
                {
                    ClassId = x.ClassID,
                    TestCount = x.TestCount,
                    TestName = x.TestName
                });
        }
    }
}