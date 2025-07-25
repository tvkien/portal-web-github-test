using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IAutoFocusGroupConfigRepository : IReadOnlyRepository<AutoFocusGroupConfig>
    {
        AutoFocusGroupConfig GetConfigByDistrictID(int districtID);
    }
}
