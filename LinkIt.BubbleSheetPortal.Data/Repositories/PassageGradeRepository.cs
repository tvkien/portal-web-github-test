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
    public class PassageGradeRepository : IReadOnlyRepository<PassageGrade>
    {
        private readonly Table<PassageGradeView> table;

        public PassageGradeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<PassageGradeView>();
        }

        public IQueryable<PassageGrade> Select()
        {
            return table.Select(x => new PassageGrade
                                {
                                    GradeId = x.GradeID,
                                    Name = x.Name,
                                    Order = x.Order
                                });
        }
    }
}
