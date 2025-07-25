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
    public class CriteriaGradeRepository : IReadOnlyRepository<CriteriaGrade>
    {
         private readonly Table<CriteriaGradeView> table;

         public CriteriaGradeRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<CriteriaGradeView>();
        }

         public IQueryable<CriteriaGrade> Select()
        {
            return table.Select(x => new CriteriaGrade
                                {
                                    GradeId = x.GradeID,
                                    Name = x.Name,
                                    Order = x.Order
                                });
        }
    }
}
