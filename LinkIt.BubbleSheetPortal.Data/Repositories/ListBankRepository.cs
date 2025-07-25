using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ListBankRepository : IReadOnlyRepository<ListBank>
    {
         private readonly Table<ListBankView> table;

         public ListBankRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<ListBankView>();
        }

         public IQueryable<ListBank> Select()
        {
            return table.Select(x => new ListBank
                {
                    BankId = x.BankID,
                    Name = x.Name,
                    BankAccessId = x.BankAccessID ?? 0,
                    CreateByUserId = x.CreatedByUserID ?? 0,
                    BankDistrictId = x.BankDistrictID ?? 0,
                    DistrictId = x.DistrictID ?? 0,
                    BankDistrictAccessId = x.BankDistrictAccessID?? 0,
                    CreateBankDistrictId = x.CreatebankDistrictId ?? 0,
                    SubjectId = x.SubjectID,
                    SubjectName = x.SubjectName,
                    GradeId = x.GradeID,
                    GradeName = x.GradeName,
                    Hide = x.Hide??false,
                    Archived = x.Archived??false
                });
        }
    }
}
