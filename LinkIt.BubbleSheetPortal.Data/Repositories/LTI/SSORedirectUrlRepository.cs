using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.LTI;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.LTI
{
    public class SSORedirectUrlRepository : IRepository<SSORedirectUrl>
    {
        private readonly Table<SSORedirectUrlEntity> _table;

        public SSORedirectUrlRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = DbDataContext.Get(connectionString).GetTable<SSORedirectUrlEntity>();
        }

        public void Delete(SSORedirectUrl item)
        {
            throw new NotImplementedException();
        }

        public void Save(SSORedirectUrl item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<SSORedirectUrl> Select()
        {
            return _table.Select(o => new SSORedirectUrl
            {
                SSORedirectUrlId = o.SSORedirectUrlId,
                SSOInformationId = o.SSOInformationId,
                RedirectUrl = o.RedirectUrl,
                RoleId = o.RoleId.GetValueOrDefault(),
                Type = o.Type,
                XLIModuleCode = o.XLIModuleCode
            });
        }
    }
}
