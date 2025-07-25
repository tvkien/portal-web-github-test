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
    public class PassageGradeQtiRepository : IReadOnlyRepository<PassageGradeQti>
    {
        private readonly Table<PassageGradeQtiView> table;

        public PassageGradeQtiRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<PassageGradeQtiView>();
        }

        public IQueryable<PassageGradeQti> Select()
        {
            return table.Select(x => new PassageGradeQti
                                {
                                    GradeId = x.GradeID,
                                    Name = x.Name,
                                    Order = x.Order
                                });
        }
    }
}
