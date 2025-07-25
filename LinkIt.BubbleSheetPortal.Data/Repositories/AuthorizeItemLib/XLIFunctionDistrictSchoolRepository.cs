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
    public class XLIFunctionDistrictSchoolRepository : IReadOnlyRepository<XLIFunctionDistrictSchoolMap>
    {
         private readonly Table<XLIFunctionDistrictSchool> table;

         public XLIFunctionDistrictSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<XLIFunctionDistrictSchool>();
        }

         public IQueryable<XLIFunctionDistrictSchoolMap> Select()
        {
            return table.Select(x =>
             new XLIFunctionDistrictSchoolMap()
            {
                XLIFunctionDistrictSchoolID = x.XLIFunctionDistrictSchoolID,
                XLIFunctionID = x.XLIFunctionID,
                DistrictID = x.DistrictID,
                Restrict   =x.Restrict
                
            });
        }
    }
}
