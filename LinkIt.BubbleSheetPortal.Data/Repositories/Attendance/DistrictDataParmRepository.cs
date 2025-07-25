using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Attendance
{
    public class DistrictDataParmRepository : IReadOnlyRepository<DistrictDataParmDTO>
    {
        private readonly Table<DistrictDataParmEntity> table;

        public DistrictDataParmRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<DistrictDataParmEntity>();
        }

        public IQueryable<DistrictDataParmDTO> Select()
        {
            return table.Select(x => new DistrictDataParmDTO
            {
                DataSetCategoryID = x.DataSetCategoryID,
                DataSetOriginID = x.DataSetOriginID,
                DistrictDataParmID = x.DistrictDataParmID,
                DistrictID = x.DistrictID.GetValueOrDefault(),
                ImportType = x.ImportType,
                JSONDataConfig = x.JSONDataConfig
            });
        }
    }
}
