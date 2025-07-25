using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XLIAreaDistrictModuleService
    {
        private readonly IXLIAreaDistrictModuleRepository xLIAreaDistrictModuleRepository;

        public XLIAreaDistrictModuleService(IXLIAreaDistrictModuleRepository xLIAreaDistrictModuleRepository,
            IVirtualTestRepository virtualTestRepository)
        {
            this.xLIAreaDistrictModuleRepository = xLIAreaDistrictModuleRepository;
        }
        public bool CheckDistrictRoleAccessModule(int userid, int moduleid)
        {
            return xLIAreaDistrictModuleRepository.CheckDistrictRoleAccessModule(userid, moduleid);
        }
    }
}
