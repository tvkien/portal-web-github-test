using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.AuthorizeItemLib
{
    public class XLIFunctionSchoolRoleRepository : IReadOnlyRepository<XLIFunctionSchoolRoleMap>
    {
        private readonly Table<XLIFunctionSchoolRole> table;

        public XLIFunctionSchoolRoleRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<XLIFunctionSchoolRole>();
        }

        public IQueryable<XLIFunctionSchoolRoleMap> Select()
        {
            return table.Select(x =>
             new XLIFunctionSchoolRoleMap()
            {
                XLIFunctionSchoolRoleID = x.XLIFunctionSchoolID,
                XLIFunctionSchoolID = x.XLIFunctionSchoolID,
                RoleID = x.RoleID
            });
        }
    }
}
