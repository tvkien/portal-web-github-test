using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using System;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog
{
    public class AnswerLogRepository : IAnswerLogRepository
    {
        private readonly Table<AnswerLogEntity> table;

        public AnswerLogRepository(IConnectionString conn)
        {
            var connectionString = conn.GetAdminReportingLogConnectionString();
            table = TestResultLogDataContext.Get(connectionString).GetTable<AnswerLogEntity>();
            Mapper.CreateMap<AnswerLog, AnswerLogEntity>();
        }

        public IQueryable<AnswerLog> Select()
        {
            return table.Select(x => new AnswerLog
                    {
                        AnswerLogID = x.AnswerLogID,
                        AnswerID = x.AnswerID,
                        PointsEarned = x.PointsEarned,
                        PointsPossible = x.PointsPossible,
                        WasAnswered = x.WasAnswered,
                        TestResultID = x.TestResultID,
                        VirtualQuestionID = x.VirtualQuestionID,
                        AnswerLetter = x.AnswerLetter,
                        Blocked = x.Blocked,
                        AnswerText = x.AnswerText,
                        BubbleSheetErrorType = x.BubbleSheetErrorType,
                        ResponseIdentifier = x.ResponseIdentifier,
                        AnswerImage = x.AnswerImage,
                        HighlightQuestion = x.HighlightQuestion,
                        HighlightPassage = x.HighlightPassage,
                        Overridden = x.Overridden,
                        UpdatedBy = x.UpdatedBy,
                        UpdatedDate = x.UpdatedDate
                    });
        }

        public void Save(AnswerLog item)
        {
            try
            {
                var entity = table.FirstOrDefault(x => x.AnswerLogID.Equals(item.AnswerLogID));

                if (entity.IsNull())
                {
                    entity = new AnswerLogEntity();
                    table.InsertOnSubmit(entity);
                }

                Mapper.Map(item, entity);
                table.Context.SubmitChanges();
                item.AnswerLogID = entity.AnswerLogID;
            }
            catch (Exception ex) { }
        }

        public void Save(IList<AnswerLog> answerLogs)
        {
            try
            {
                foreach (var item in answerLogs)
                {
                    AnswerLogEntity entity;
                    if (item.AnswerLogID.Equals(0))
                    {
                        entity = new AnswerLogEntity();
                        table.InsertOnSubmit(entity);
                    }
                    else
                    {
                        entity = table.FirstOrDefault(x => x.AnswerLogID.Equals(item.AnswerLogID));
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

        public void Delete(AnswerLog item)
        {
            if (item.IsNotNull())
            {
                //TODO:
            }
        }        
    }
}
