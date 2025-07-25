using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBankRepository : IRepository<Bank>
    {
        BankProperty GetBankProperty(int bankId);

        bool CanDeleteBankByID(int bankId);

        void DeleteBankByID(int bankId);
        bool CheckBankLock(int bankId, int userId, int districtId);

        PrintTestOptions CheckPermissionPrintQuestionAndAnswerKey(int bankId, int userId, int districtId);

        List<BankOrder> GetBankOrders(int districtId);
    }
}
