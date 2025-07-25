using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.LTI
{
    public interface ILTIRequestHistoryRepository
    {
        void UpdateStatus(string nonce, bool isCompleted);
    }
}
