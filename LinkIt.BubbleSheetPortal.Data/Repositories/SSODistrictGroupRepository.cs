using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinkIt.BubbleSheetPortal.Models.SSO;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SSODistrictGroupRepository : ISSODistrictGroupRepository
    {
        private readonly Table<SSODistrictGroupEntity> table;
        private readonly UserDataContext _userDataContext;

        public SSODistrictGroupRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<SSODistrictGroupEntity>();
            _userDataContext = UserDataContext.Get(connectionString);
        }

        public void Delete(SSODistrictGroup item)
        {

        }

        public void Save(SSODistrictGroup item)
        {

        }

        public IQueryable<SSODistrictGroup> Select()
        {
            return null;
        }

        public SSODistrictGroup GetByDistrictID(int districtID, string type = "auth0")
        {
            var obj = table.Where(m=>m.DistrictID == districtID && m.SSOInformationEntity.Type == type).Select(m=> new SSODistrictGroup
            { 
                DistrictID = m.DistrictID,
                District = new Models.District
                {
                    Id = m.DistrictID,
                    LICode = m.DistrictEntity.LICode,
                    Name = m.DistrictEntity.Name
                },
                SSODistrictGroupID = m.SSODistrictGroupID,
                SSOInformationID = m.SSOInformationID,
                SSOInformation = new SSOInformation
                {
                    SSOInformationID = m.SSOInformationID,
                    Auth0ClientId = m.SSOInformationEntity.Auth0ClientId,
                    Auth0ClientSecret = m.SSOInformationEntity.Auth0ClientSecret,
                    UrlLandingPage = m.SSOInformationEntity.UrlLandingPage,
                    UrlLogoutPage = m.SSOInformationEntity.UrlLogoutPage,
                    DefaultConnection = m.SSOInformationEntity.DefaultConnection

                },
            }).FirstOrDefault();

            return obj;
        }

        public SSODistrictGroup GetByTenantID(int tenantID, string type = "auth0")
        {
            var obj = table.Where(m => m.TenantID == tenantID && m.SSOInformationEntity.Type == type).Select(m => new SSODistrictGroup
            {
                DistrictID = m.DistrictID,
                District = new Models.District
                {
                    Id = m.DistrictID,
                    LICode = m.DistrictEntity.LICode,
                    Name = m.DistrictEntity.Name
                },
                SSODistrictGroupID = m.SSODistrictGroupID,
                SSOInformationID = m.SSOInformationID,
                SSOInformation = new SSOInformation
                {
                    SSOInformationID = m.SSOInformationID,
                    Auth0ClientId = m.SSOInformationEntity.Auth0ClientId,
                    Auth0ClientSecret = m.SSOInformationEntity.Auth0ClientSecret,
                    UrlLandingPage = m.SSOInformationEntity.UrlLandingPage,
                    UrlLogoutPage = m.SSOInformationEntity.UrlLogoutPage,
                    DefaultConnection = m.SSOInformationEntity.DefaultConnection

                },
            }).FirstOrDefault();

            return obj;
        }

        public List<Models.District> GetListDistrictSameSSO(string subdomain)
        {
            var districts = new List<Models.District>();

            var ssoInfoId = table.Where(m => m.DistrictEntity != null && m.DistrictEntity.LICode.ToLower() == subdomain.ToLower())
                .Select(m=>m.SSOInformationID).FirstOrDefault();

            if(ssoInfoId > 0)
            {
                districts = table.Where(m => m.SSOInformationID == ssoInfoId).Select(m => new Models.District
                {
                    Id = m.DistrictID,
                    Name = m.DistrictEntity.Name,
                    LICode = m.DistrictEntity.LICode
                }).ToList();
            }

            return districts;
        }
    }
}
