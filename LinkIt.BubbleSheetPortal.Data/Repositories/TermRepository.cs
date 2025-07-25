using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TermRepository : IReadOnlyRepository<Term>
    {
        private readonly Table<TermEntity> table;

        public TermRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<TermEntity>();
        }

        public IQueryable<Term> Select()
        {
            return table.Select(x => new Term
                {
                    Id = x.TermID,
                    Name = x.Name,
                    TeacherId = x.TeacherID
                });
        }
    }
}