using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.SSO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SSOInformationRepository : ISSOInformationRepository
    {
        private readonly Table<SSOInformationEntity> table;

       public SSOInformationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<SSOInformationEntity>();
        }

        public SSOInformation GetByTpe(string type)
        {
            var entity = table.FirstOrDefault(m => m.Type.Equals(type));
            SSOInformation ssoInformation = null;

            if (entity != null)
            {
                ssoInformation = new SSOInformation
                {
                    Auth0ClientId = entity.Auth0ClientId,
                    Auth0ClientSecret = entity.Auth0ClientSecret,
                    DefaultConnection = entity.DefaultConnection,
                    SSOInformationID = entity.SSOInformationID,
                    UrlLandingPage = entity.UrlLandingPage,
                    UrlLogoutPage = entity.UrlLogoutPage
                };
            }

            return ssoInformation;
        }

        public void Delete(SSOInformation item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(SSOInformation item)
        {
            throw new System.NotImplementedException();
        }

        public IQueryable<SSOInformation> Select()
        {
            throw new System.NotImplementedException();
        }
    }
}
