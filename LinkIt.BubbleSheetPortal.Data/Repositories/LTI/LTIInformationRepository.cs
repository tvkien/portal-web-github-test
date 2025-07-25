using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.LTI
{
    public class LTIInformationRepository: IRepository<LTIInformation>
    {
        private readonly Table<LTIInformationEntity> _table;

        public LTIInformationRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = DbDataContext.Get(connectionString).GetTable<LTIInformationEntity>();
            Mapper.CreateMap<LTIInformation, LTIInformationEntity>();
        }

        public void Delete(LTIInformation item)
        {
            throw new NotImplementedException();
        }

        public void Save(LTIInformation item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<LTIInformation> Select()
        {
            return _table.Select(o => new LTIInformation
            {
                LTIInformationID = o.LTIInformationID,
                PlatformID = o.PlatformID,
                ClientID = o.ClientID,
                DeploymentID = o.DeploymentID,
                AuthorizationServerID = o.AuthorizationServerID,
                AuthenticationRequestURL = o.AuthenticationRequestURL,
                AccessTokenURL = o.AccessTokenURL,
                PublicKey = o.PublicKey,
                DistrictID = o.DistrictID
            });
        }
    }
}
