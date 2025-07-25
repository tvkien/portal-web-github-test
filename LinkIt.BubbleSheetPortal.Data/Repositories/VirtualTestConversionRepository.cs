using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestConversionRepository : IReadOnlyRepository<VirtualTestConversion>
    {
        private readonly Table<VirtualTestConversionEntity> table;

        public VirtualTestConversionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<VirtualTestConversionEntity>();
        }
        public IQueryable<VirtualTestConversion> Select()
        {
            return table.Select(x => new VirtualTestConversion()
                                     {
                                         ConversionSetID = x.ConversionSetID,
                                         Name = x.Name,
                                         TargetIDs = x.TargetIDs,
                                         TargetType = x.TargetType,
                                         VirtualTestConversionID = x.VirtualTestConversionID,
                                         VirtualTestID = x.VirtualTestID
                                     });
        }
    }
}