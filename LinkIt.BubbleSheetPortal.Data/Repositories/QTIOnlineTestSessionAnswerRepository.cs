using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiOnlineTestSessionAnswerRepository : IRepository<QtiOnlineTestSessionAnswer>
    {
        private readonly Table<QTIOnlineTestSessionAnswerEntity> table;

        public QtiOnlineTestSessionAnswerRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<QTIOnlineTestSessionAnswerEntity>();
            Mapper.CreateMap<QtiOnlineTestSessionAnswer, QTIOnlineTestSessionAnswerEntity>(); 
        }

        public IQueryable<QtiOnlineTestSessionAnswer> Select()
        {
            return table.Select(x => new QtiOnlineTestSessionAnswer
                {
                    QtiOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                    QtiOnlineTestSessionID = x.QTIOnlineTestSessionID,
                    VirtualQuestionID = x.VirtualQuestionID,
                    AnswerChoice = x.AnswerChoice,
                    Answered = x.Answered,
                    AnswerText = x.AnswerText,
                    Timestamp = x.Timestamp,
                    ResponseIdentifier = x.ResponseIdentifier,
                    CrossedAnswer = x.CrossedAnswer,
                    Flag = x.Flag,
                    AnswerImage = x.AnswerImage,
                    HighlightPassage = x.HighlightPassage,
                    Status = x.Status,
                    QuestionOrder = x.QuestionOrder,
                    AnswerTemp = x.AnswerTemp,
                    AnswerOrder = x.AnswerOrder,
                    PointsEarned = x.PointsEarned,
                    HighlightQuestion = x.HighlightQuestion,
                    Overridden = x.Overridden
                });
        }

        public void Save(QtiOnlineTestSessionAnswer item)
        {
            var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionAnswerID.Equals(item.QtiOnlineTestSessionAnswerID));

            if (entity == null)
            {
                entity = new QTIOnlineTestSessionAnswerEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QtiOnlineTestSessionAnswerID = entity.QTIOnlineTestSessionID;
        }

        public void Delete(QtiOnlineTestSessionAnswer item)
        {
            var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionID.Equals(item.QtiOnlineTestSessionAnswerID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}
