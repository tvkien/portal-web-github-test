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
    public class ClassExtraRepository : IReadOnlyRepository<ClassExtra>
    {
        private readonly Table<ClassUserView> table;

        public ClassExtraRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<ClassUserView>();
        }

        public IQueryable<ClassExtra> Select()
        {
            return table.Select(x => new ClassExtra
            {
                ClassId = x.ClassID,
                Name = x.Name,
                DistrictTermId = x.DistrictTermID,
                UserId = x.UserID,
                SchoolId = x.SchoolID,
                UserIdClassUser = x.UserIDClassUser,
                ClassUserLOEId = x.ClassUserLOEID
            });
        }
    }
}
