using System;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly Table<AnswerEntity> table;
        private readonly TestDataContext _testDataContext;

        public AnswerRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<AnswerEntity>();
            _testDataContext = TestDataContext.Get(connectionString);
            Mapper.CreateMap<Answer, AnswerEntity>();
        }

        public IQueryable<Answer> Select()
        {
            return table.Select(x => new Answer
                    {
                        AnswerID = x.AnswerID,
                        PointsEarned = x.PointsEarned,
                        PointsPossible = x.PointsPossible,
                        WasAnswered = x.WasAnswered,
                        TestResultID = x.TestResultID,
                        VirtualQuestionID = x.VirtualQuestionID,
                        AnswerLetter = x.AnswerLetter,
                        Blocked = x.Blocked,
                        AnswerText = x.AnswerText,
                        BubbleSheetErrorType = x.BubbleSheetErrorType
                    });
        }

        public void Save(Answer item)
        {
            var entity = table.FirstOrDefault(x => x.AnswerID.Equals(item.AnswerID));

            if (entity.IsNull())
            {
                entity = new AnswerEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            table.Context.SubmitChanges();
            item.AnswerID = entity.AnswerID;
        }

        public void Delete(Answer item)
        {
            if (item.IsNotNull())
            {
                //TODO:
            }
        }

        public bool RegradeTestByTestResultId(int testResultId)
        {
            if (testResultId > 0)
            {
                try
                {
                    _testDataContext.TestResult_Regrade(testResultId);
                    return true;
                }
                catch (Exception exception)
                {
                    return false;
                }
            }
            return false;
        }

        public bool PurgeTest(int virtualTestId)
        {
            if (virtualTestId > 0)
            {
                try
                {
                    _testDataContext.PurgeTestByVirtualTestId(virtualTestId);
                    return true;
                }
                catch (Exception exception)
                {
                    return false;
                }
            }
            return false;
        }
    }
}
