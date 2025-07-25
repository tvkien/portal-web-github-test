using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QtiOnlineTestSessionAnswerSubRepository : IRepository<QtiOnlineTestSessionAnswerSubData>
    {
        private readonly Table<QTIOnlineTestSessionAnswerSubEntity> table;

        public QtiOnlineTestSessionAnswerSubRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<QTIOnlineTestSessionAnswerSubEntity>();
            Mapper.CreateMap<QtiOnlineTestSessionAnswerSubData, QTIOnlineTestSessionAnswerSubEntity>(); 
        }

        public IQueryable<QtiOnlineTestSessionAnswerSubData> Select()
        {
            return table.Select(x => new QtiOnlineTestSessionAnswerSubData
            {
                QtiOnlineTestSessionAnswerID = x.QTIOnlineTestSessionAnswerID,
                QTIOnlineTestSessionAnswerSubID = x.QTIOnlineTestSessionAnswerSubID,
                VirtualQuestionSubID = x.VirtualQuestionSubID,
                AnswerChoice = x.AnswerChoice,
                Answered = x.Answered,
                AnswerText = x.AnswerText,
                Timestamp = x.Timestamp,
                ResponseIdentifier = x.ResponseIdentifier,
                CrossedAnswer = x.CrossedAnswer,
                AnswerTemp = x.AnswerTemp,
                Overridden = x.Overridden,
                PointsEarned = x.PointsEarned,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            });
        }

        public void Save(QtiOnlineTestSessionAnswerSubData item)
        {
            var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionAnswerSubID.Equals(item.QTIOnlineTestSessionAnswerSubID));

            if (entity == null)
            {
                entity = new QTIOnlineTestSessionAnswerSubEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.QTIOnlineTestSessionAnswerSubID = entity.QTIOnlineTestSessionAnswerSubID;
        }

        public void Delete(QtiOnlineTestSessionAnswerSubData item)
        {
            var entity = table.FirstOrDefault(x => x.QTIOnlineTestSessionAnswerSubID.Equals(item.QTIOnlineTestSessionAnswerSubID));

            if (entity != null)
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }
    }
}