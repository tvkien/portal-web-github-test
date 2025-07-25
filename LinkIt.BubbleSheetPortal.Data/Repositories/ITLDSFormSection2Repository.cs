using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITLDSFormSection2Repository : IRepository<TLDSFormSection2>
    {
        bool UpdateTldsForm(TLDSFormSection2 item);

        bool SubmittedForm(TLDSFormSection2 item);
    }
}
