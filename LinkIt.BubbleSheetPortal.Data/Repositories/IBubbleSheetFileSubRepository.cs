using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBubbleSheetFileSubRepository : IReadOnlyRepository<BubbleSheetFileSub>
    {
        void Save(BubbleSheetFileSub item);
        void Delete(BubbleSheetFileSub item);
    }
}
