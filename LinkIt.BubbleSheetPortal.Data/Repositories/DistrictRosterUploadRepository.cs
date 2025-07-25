using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DistrictRosterUploadRepository : IReadOnlyRepository<DistrictRosterUpload>
    {
        private readonly Table<DistrictRosterUploadView> table;

        public DistrictRosterUploadRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<DistrictRosterUploadView>();
        }

        public IQueryable<DistrictRosterUpload> Select()
        {
            return table.Select(x => new DistrictRosterUpload
                {
                    DistrictId = x.DistrictID,
                    UploadTypeId = x.UploadTypeID
                });
        }
    }
}