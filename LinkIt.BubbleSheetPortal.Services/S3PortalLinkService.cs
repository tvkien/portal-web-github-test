using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class S3PortalLinkService
    {
        private readonly IRepository<S3PortalLink> repository;

        public S3PortalLinkService(IRepository<S3PortalLink> repository)
        {
            this.repository = repository;
        }

        public S3PortalLink GetPortalLinkByKey(string portalKey)
        {
            return repository.Select().FirstOrDefault(o => o.PortalKey.Equals(portalKey));
        }

        public S3PortalLink Save(S3PortalLink obj)
        {
            if (obj != null)
            {
                repository.Save(obj);
            }
            return obj;
        }
    }
}
