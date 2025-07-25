using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AuthorGroupBankRepository : IRepository<AuthorGroupBank>
    {
        private readonly Table<AuthorGroupBankEntity> table;

        public AuthorGroupBankRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<AuthorGroupBankEntity>();
        }

        public IQueryable<AuthorGroupBank> Select()
        {
            return table.Select(x => new AuthorGroupBank
            {
                BankID = x.BankID,
                AuthorGroupID = x.AuthorGroupID,
                AuthorGroupBankID = x.AuthorGroupBankID
            });
        }

        public void Save(AuthorGroupBank item)
        {
            var entity = table.FirstOrDefault(x => x.AuthorGroupID.Equals(item.AuthorGroupBankID));

            if (entity == null)
            {
                entity = new AuthorGroupBankEntity();
                table.InsertOnSubmit(entity);
            }

            entity.BankID = item.BankID;
            entity.AuthorGroupID = item.AuthorGroupID;

            table.Context.SubmitChanges();
            item.AuthorGroupID = entity.AuthorGroupID;
        }

        public void Delete(AuthorGroupBank item)
        {
            var entity = table.FirstOrDefault(x => x.AuthorGroupBankID.Equals(item.AuthorGroupBankID));
            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}