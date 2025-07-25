using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class PasswordQuestionRepository : IReadOnlyRepository<PasswordResetQuestion>
    {
        private readonly Table<PasswordResetQuestionEntity> table;

        public PasswordQuestionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<PasswordResetQuestionEntity>();
        }

        public IQueryable<PasswordResetQuestion> Select()
        {
            return table.Select(x => new PasswordResetQuestion
                {
                    Id = x.PasswordQuestionID,
                    Question = x.Text,
                    Type = x.Type
                });
        }
    }
}