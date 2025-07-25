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
    public class XLIFunctionRepository : IReadOnlyRepository<XLIFunctionMap>
    {
        private readonly Table<XLIFunction> table;

        public XLIFunctionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<XLIFunction>();
        }

        public IQueryable<XLIFunctionMap> Select()
        {
            return table.Select(x =>
             new XLIFunctionMap()
            {
                XLIFunctionID =x.XLIFunctionID,
                Name =x.Name,
                Description =x.Description         

            });
        }
    }
}
