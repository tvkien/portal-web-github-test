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
    public class S3PermissionRepository : IRepository<S3Permission>
    {
        private readonly Table<S3PermissionEntity> table;
        public S3PermissionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ExtractTestDataContext.Get(connectionString).GetTable<S3PermissionEntity>();
            Mapper.CreateMap<S3Permission, S3PermissionEntity>();
        }

        public IQueryable<S3Permission> Select()
        {
            return table.Select(x => new S3Permission()
                    {
                        Id = x.ID,
                        CreateDate = x.CreateDate,
                        S3LinkitPortalId = x.S3PortalLinkID,
                        UserId = x.UserID
                    });
        }

        public void Save(S3Permission item)
        {
            var entity = table.FirstOrDefault(x => x.ID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new S3PermissionEntity();
                table.InsertOnSubmit(entity);
            }
            entity.CreateDate = item.CreateDate;
            entity.S3PortalLinkID = item.S3LinkitPortalId;
            entity.UserID = item.UserId;
            table.Context.SubmitChanges();
            item.Id = entity.ID;
        }

        public void Delete(S3Permission item)
        {
            if (item.IsNotNull())
            {
                var entity = table.FirstOrDefault(x => x.ID.Equals(item.Id));
                if (entity != null)
                {
                    table.DeleteOnSubmit(entity);
                    table.Context.SubmitChanges();
                }
            }
        }
    }
}
