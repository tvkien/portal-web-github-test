using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using LinkIt.BubbleSheetPortal.Models.DTOs.TLDS;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITLDSGroupRepository : IRepository<TLDSGroupDTO>
    {
        List<TLDSGroupDTO> GetAlllByTldsUserMetaID(int tldsUserMetaID);

        bool DeactiveTLDSGroup(int tldsGroupId);

        bool ActiveTLDSGroup(int tldsGroupId);
    }
}
