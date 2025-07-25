using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class WhatNewRepository : IRepository<WhatNewData>
    {
        private readonly Table<FileLinksEntity> _table;
        private readonly UserDataContext _context;


        public WhatNewRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = UserDataContext.Get(connectionString).GetTable<FileLinksEntity>();
            _context = UserDataContext.Get(connectionString);
        }

        public void Delete(WhatNewData item)
        {
            throw new NotImplementedException();
        }
        public void Save(WhatNewData item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<WhatNewData> Select()
        {
            var result = _table.Select(o => new WhatNewData
            {
                DownloadId = o.DownloadID,
                FilePath = o.FilePath,
            });
            return result;
        }
    }
}
