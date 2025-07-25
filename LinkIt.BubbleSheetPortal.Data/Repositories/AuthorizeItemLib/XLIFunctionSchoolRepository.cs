using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.AuthorizeItemLib
{
    public class XLIFunctionSchoolRepository : IReadOnlyRepository<XLIFunctionSchoolMap>
    {
        private readonly Table<XLIFunctionSchool> table;

        public XLIFunctionSchoolRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<XLIFunctionSchool>();
        }

        public IQueryable<XLIFunctionSchoolMap> Select()
        {
            return table.Select(x =>
             new XLIFunctionSchoolMap()
            {
                XLIFunctionSchoolID = x.XLIFunctionSchoolID,
                XLIFunctionID = x.XLIFunctionID,
                SchoolID = x.SchoolID,
                AllRole = x.AllRole               
            });
        }
    }
}
