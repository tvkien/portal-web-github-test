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
    public class APIFunctionRepository : IReadOnlyRepository<APIFunction>
    {
         private readonly Table<APIFunctionEntity> table;

         public APIFunctionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<APIFunctionEntity>();
        }

        public IQueryable<APIFunction> Select()
        {
            return table.Select(x => new APIFunction
                                {
                                    APIFunctionId = x.APIFunctionID,
                                    HTTPAction = x.HttpAction,
                                    Status = x.Status,
                                    URI = x.URI,
                                    Version = x.Version
                                });
        }
    }
}
