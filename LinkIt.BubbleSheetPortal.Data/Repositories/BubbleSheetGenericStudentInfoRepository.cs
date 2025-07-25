using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class BubbleSheetGenericStudentInfoRepository : IReadOnlyRepository<BubbleSheetGenericStudentInfo>
    {
        private readonly Table<BubbleSheetGenericStudentInfoEntity> table;

        public BubbleSheetGenericStudentInfoRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = BubbleSheetDataContext.Get(connectionString).GetTable<BubbleSheetGenericStudentInfoEntity>();
        }
        public IQueryable<BubbleSheetGenericStudentInfo> Select()
        {
            return table.Select(x => new BubbleSheetGenericStudentInfo()
                    {
                        BubbleSheetFileId = x.BubbleSheetFileID,
                        BubbleSheetGenericStudentInfoId = x.BubbleSheetGenericStudentInfoID,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        StudentCode = x.StudentCode
                    });
        }
    }
}
