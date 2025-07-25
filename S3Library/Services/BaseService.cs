using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using S3Library.Entities;

namespace S3Library.Services
{
    public class BaseService : IDisposable
    {
        protected readonly S3PortalLinkContext _context;
        public BaseService(string connectionString)
        {
            _context = new S3PortalLinkContext(connectionString);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
