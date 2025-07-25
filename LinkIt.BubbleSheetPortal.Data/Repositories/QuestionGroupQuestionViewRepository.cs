using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QuestionGroupQuestionViewRepository : IReadOnlyRepository<QuestionGroupQuestion>
    {
        private readonly Table<QuestionGroupQuestionView> view;

        public QuestionGroupQuestionViewRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            view = TestDataContext.Get(connectionString).GetTable<QuestionGroupQuestionView>();
        }
        public IQueryable<QuestionGroupQuestion> Select()
        {
            return view.Select(x => new QuestionGroupQuestion
            {
                QuestionGroupID = x.QuestionGroupID,
                VirtualQuestionID = x.VirtualQuestionID,
                VirtualSectionID = x.VirtualSectionID,
                VirtualTestID = x.VirtualTestID
            });
        }
    }
}
