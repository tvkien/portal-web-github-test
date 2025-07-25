using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class APIPermissionService
    {
        private readonly IReadOnlyRepository<APIPermission> repository;

        public APIPermissionService(IReadOnlyRepository<APIPermission> repository)
        {
            this.repository = repository;
        }

        public List<int> GetAPIPermissionByTaget(int tagerId)
        {
            return repository.Select().Where(o => o.TargetId == tagerId && o.IsAllow ).Select(o => o.APIFunctionId).ToList();
        }
    }
}
