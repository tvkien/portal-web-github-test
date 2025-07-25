using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class S3PortalLinkRepository : IRepository<S3PortalLink>
    {
        private readonly Table<S3PortalLinkEntity> table;

        public S3PortalLinkRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ExtractTestDataContext.Get(connectionString).GetTable<S3PortalLinkEntity>();
            Mapper.CreateMap<S3PortalLink, S3PortalLinkEntity>();
        }

        public IQueryable<S3PortalLink> Select()
        {
            return table.Select(x => new S3PortalLink
            {
                S3PortalLinkId = x.S3PortalLinkID,
                ServiceType = x.ServiceType ?? 0,
                DistrictId = x.DistrictID ?? 0,
                DateCreated = x.DateCreated ,
                BucketName = x.BucketName,
                FilePath = x.FilePath,
                PortalKey = x.PortalKey
            });
        }

        public void Save(S3PortalLink item)
        {
            var entity = table.FirstOrDefault(x => x.S3PortalLinkID.Equals(item.S3PortalLinkId));

            if (entity.IsNull())
            {
                entity = new S3PortalLinkEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.S3PortalLinkId = entity.S3PortalLinkID;
        }

        public void Delete(S3PortalLink item)
        {
            var entity = table.FirstOrDefault(x => x.S3PortalLinkID.Equals(item.S3PortalLinkId));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
