using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class APIPermissionRepository : IReadOnlyRepository<APIPermission>
    {
         private readonly Table<APIPermissionEntity> table;

         public APIPermissionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<APIPermissionEntity>();
        }

         public IQueryable<APIPermission> Select()
        {
            return table.Select(x => new APIPermission
                                {
                                    APIPermissionId = x.APIPermissionID,
                                    APIAccountTypeId = x.APIAccountTypeID ?? 1,
                                    APIFunctionId = x.APIFunctionID,
                                    CreateDate = x.CreatedDate,
                                    DistrictId = x.DistrictID ?? 0,
                                    IsAllow = x.IsAllow,
                                    TargetId = x.TargetID,
                                    UpdatedDate = x.UpdatedDate
                                });
        }
    }
}
