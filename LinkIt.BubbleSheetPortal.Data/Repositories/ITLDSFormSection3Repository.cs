using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.TLDS;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface ITLDSFormSection3Repository : IRepository<TLDSFormSection3>
    {
        bool UpdateTldsForm(TLDSFormSection3 item);

        bool SubmittedForm(TLDSFormSection3 item);
    }
}
