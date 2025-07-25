using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    internal class DistrictMetaRepository : IReadOnlyRepository<DistrictMeta>
    {
        private readonly Table<DistrictMetaEntity> table;

        public DistrictMetaRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = UserDataContext.Get(connectionString).GetTable<DistrictMetaEntity>();
        }

        public IQueryable<DistrictMeta> Select()
        {
            return table.Select(districtMeta => new DistrictMeta
            {
                DistrictMetaID = districtMeta.DistrictMetaID,
                DistrictID = districtMeta.DistrictID,
                Name = districtMeta.Name,
                Data = districtMeta.Data,
            });
        }
    }
}
