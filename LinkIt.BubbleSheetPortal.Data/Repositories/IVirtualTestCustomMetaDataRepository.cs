using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Old.DataLocker;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualTestCustomMetaDataRepository : IRepository<VirtualTestCustomMetaData>
    {        
        void UpdateOrderForMetaData(int? virtualTestCustomScoreId, int? virtualTestCustomSubScoreId, string scoreTypeOrderXml);
        List<CustomMetaSettingMinMaxDto> GetCustomMetaSettings(int templateId);

        void DeleteMetaData(int templateId, int? subTemplateId = null);
    }

}
