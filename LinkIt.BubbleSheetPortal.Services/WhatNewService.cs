using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class WhatNewService
    {
        private readonly IRepository<WhatNewData> _whatNewRepository;
        public WhatNewService(IRepository<WhatNewData> whatNewRepository)
        {
            _whatNewRepository = whatNewRepository;
        }

        public WhatNewData GetDownloadFile(Guid downloadId)
        {
            return _whatNewRepository.Select().FirstOrDefault(o => o.DownloadId == downloadId);
        }
    }
}
