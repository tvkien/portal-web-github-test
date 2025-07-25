using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class S3PermissionService
    {
        private readonly IRepository<S3Permission> repository;

        public S3PermissionService(IRepository<S3Permission>  repository)
        {
            this.repository = repository;
        }

        public S3Permission GetS3PermissionById(int s3LinkitPortalId, int userId)
        {
            return repository.Select().FirstOrDefault(o => o.S3LinkitPortalId == s3LinkitPortalId && o.UserId == userId);
        }
        public S3Permission GetS3PermissionById(int s3LinkitPortalId)
        {
            return repository.Select().FirstOrDefault(o => o.S3LinkitPortalId == s3LinkitPortalId);
        }
        public bool SaveS3Permission(int s3PortalLinkitId, int userId)
        {
            var obj = new S3Permission()
                {
                    CreateDate = DateTime.UtcNow,
                    S3LinkitPortalId = s3PortalLinkitId,
                    UserId = userId
                };
            repository.Save(obj);
            return true;
        }
    }
}
