using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestVirtualTestCustomScoreService
    {
        private readonly IRepository<VirtualTestVirtualTestCustomScore> virtualTestCustomScoreSettingRepository;

        public VirtualTestVirtualTestCustomScoreService(IRepository<VirtualTestVirtualTestCustomScore> virtualTestCustomScoreSettingRepository)
        {
            this.virtualTestCustomScoreSettingRepository = virtualTestCustomScoreSettingRepository;
        }

        public IQueryable<VirtualTestVirtualTestCustomScore> Select()
        {
            return virtualTestCustomScoreSettingRepository.Select();
        }
        public VirtualTestVirtualTestCustomScore GetByVirtualTestId(int virtualTestId)
        {
            return virtualTestCustomScoreSettingRepository.Select().FirstOrDefault(x => x.VirtualTestId == virtualTestId);
        }
        public void Save(VirtualTestVirtualTestCustomScore obj)
        {
            virtualTestCustomScoreSettingRepository.Save(obj);
        }
        public void Delete(VirtualTestVirtualTestCustomScore obj)
        {
            virtualTestCustomScoreSettingRepository.Delete(obj);
        }
    }
}
