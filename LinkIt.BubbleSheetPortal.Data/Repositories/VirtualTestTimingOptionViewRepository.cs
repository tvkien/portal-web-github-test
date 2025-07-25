using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualTestTimingOptionViewRepository : IReadOnlyRepository<VirtualTestTimingOption>
    {
        private readonly Table<VirtualTestTimingOptionView> table;

        public VirtualTestTimingOptionViewRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = ExtractTestDataContext.Get(connectionString).GetTable<VirtualTestTimingOptionView>();
        }

        public IQueryable<VirtualTestTimingOption> Select()
        {
            return table.Select(x => new VirtualTestTimingOption
                                {
                                    DistrictID = x.DistrictID,
                                    TimingSettingID = x.TimingSettingID,
                                    TimingSettingName = x.TimingSettingName,
                                    Value = x.Value,
                                    VirtualTestTimingID = x.VirtualTestTimingID
                                });
        }
    }
}
