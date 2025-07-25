using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class APIAccountRepository : IReadOnlyRepository<APIAccount>
    {
        private readonly Table<APIAccountEntity> table;

        public APIAccountRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<APIAccountEntity>();
        }

        public IQueryable<APIAccount> Select()
        {
            return table.Select(x => new APIAccount
                                {
                                    APIAccountID = x.APIAccountID,
                                    ClientAccessKeyID = x.ClientAccessKeyID,
                                    CreatedDate = x.CreatedDate,
                                    UpdatedDate = x.UpdatedDate,
                                    Status = x.Status,
                                    Type = x.Type,
                                    LinkitPrivateKey = x.LinkitPrivateKey,
                                    LinkitPublicKey = x.LinkitPublicKey,
                                    APIAccountTypeID = x.APIAccountTypeID,
                                    TargetID = x.TargetID
                                });
        }
    }
}
