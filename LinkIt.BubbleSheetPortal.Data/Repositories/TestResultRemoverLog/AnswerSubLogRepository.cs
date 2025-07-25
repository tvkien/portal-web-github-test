using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public class AnswerSubLogRepository : IAnswerSubLogRepository
    {
        private readonly Table<AnswerSubLogEntity> table;

        public AnswerSubLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<AnswerSubLogEntity>();
            Mapper.CreateMap<AnswerSubLog, AnswerSubLogEntity>();
        }

        public IQueryable<AnswerSubLog> Select()
        {
            return table.Select(x => new AnswerSubLog
            {
                AnswerSubLogID = x.AnswerSubLogID,
                AnswerSubID = x.AnswerSubID,
                AnswerID = x.AnswerID,
                VirtualQuestionSubID = x.VirtualQuestionSubID,
                PointsEarned = x.PointsEarned,
                PointsPossible = x.PointsPossible,
                AnswerLetter = x.AnswerLetter,
                AnswerText = x.AnswerText,
                ResponseIdentifier = x.ResponseIdentifier,
                Overridden = x.Overridden,
                UpdatedBy = x.UpdatedBy,
                UpdatedDate = x.UpdatedDate
            });
        }

        public void Save(AnswerSubLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.AnswerSubLogID.Equals(item.AnswerSubLogID));

                if (entity == null)
                {
                    entity = new AnswerSubLogEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.AnswerSubLogID = entity.AnswerSubLogID;
            }
            catch (Exception ex) { }
        }

        public void Save(IList<AnswerSubLog> answerSubLogs)
        {
            try
            {
                foreach (var item in answerSubLogs)
                {
                    AnswerSubLogEntity entity;
                    if (item.AnswerSubLogID.Equals(0))
                    {
                        entity = new AnswerSubLogEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.AnswerSubLogID.Equals(item.AnswerSubLogID));
                    }
                    if (entity != null)
                    {
                        Mapper.Map(item, entity);
                    }
                }
                table.Context.SubmitChanges();
            }
            catch (Exception ex) { }
        }
        public void Delete(AnswerSubLog item)
        {
            throw new NotImplementedException();
        }
    }
}
