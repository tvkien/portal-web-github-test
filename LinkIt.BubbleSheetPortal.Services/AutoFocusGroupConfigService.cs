using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class AutoFocusGroupConfigService
    {
        private readonly IAutoFocusGroupConfigRepository _autoFocusGroupConfigRepository;
        public AutoFocusGroupConfigService(IAutoFocusGroupConfigRepository autoFocusGroupConfigRepository)
        {
            _autoFocusGroupConfigRepository = autoFocusGroupConfigRepository;
        }

        public AutoFocusGroupConfig GetConfigByDistrictID(int districtID)
        {
            return _autoFocusGroupConfigRepository.GetConfigByDistrictID(districtID);
        }

    }
}
