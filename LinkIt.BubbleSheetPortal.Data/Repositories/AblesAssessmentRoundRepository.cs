using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AblesAssessmentRoundRepository : IReadOnlyRepository<AblesAssessmentRound>
    {
        private readonly Table<AblesAssessmentRoundEntity> table;
        private readonly TestDataContext _testContext;

        public AblesAssessmentRoundRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testContext = TestDataContext.Get(connectionString);
            _testContext.CommandTimeout = 300; //increase timeout value for dbcontext in report module

            table = _testContext.GetTable<AblesAssessmentRoundEntity>();
        }

        public IQueryable<AblesAssessmentRound> Select() => table.Select(x => new AblesAssessmentRound()
        {
            AssessmentRoundId = x.AssessmentRoundID,
            DateStart = x.DateStart,
            DateEnd = x.DateEnd,
            DistrictId = x.DistrictID,
            Round = x.Round,
            RoundIndex = x.RoundIndex ?? 0,
            Name = x.Name
        });
    }
}