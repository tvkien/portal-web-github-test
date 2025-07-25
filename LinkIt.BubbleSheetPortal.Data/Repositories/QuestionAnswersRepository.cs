using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories 
{
    public class QuestionAnswersRepository : IReadOnlyRepository<QuestionOptions>
    {
        private readonly Table<QuestionAnswersEntity> table;

        public QuestionAnswersRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<QuestionAnswersEntity>();
            Mapper.CreateMap<QuestionOptions, QuestionAnswersEntity>();
        }

        public IQueryable<QuestionOptions> Select()
        {
            return table.Select(x => new QuestionOptions
                {
                    TestId = x.TestId,
                    VirtualQuestionId = x.VirtualQuestionID,
                    ProblemNumber = x.ProblemNumber,
                    AnswerIdentifiers = x.AnswerIdentifiers,
                    IsOpenEndedQuestion = x.QTISchemaID.Equals(10),
                    PointsPossible = x.PointsPossible,
                    QtiSchemaId = x.QTISchemaID,
                    IsGhostQuestion = x.BaseVirtualQuestionID.HasValue,
                    QuestionGroupID = x.QuestionGroupID,
                    VirtualSectionID = x.VirtualSectionID,
            });
        }
    }
}
