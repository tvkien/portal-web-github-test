using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestResultUnitOfWorkRepository : IUnitOfWorkRepository<TestResult>
    {
        private readonly Table<TestResultEntity> table;
        private readonly TestDataContext dbContext;

        public TestResultUnitOfWorkRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<TestResultEntity>();
            dbContext = TestDataContext.Get(connectionString);
            Mapper.CreateMap<TestResult, TestResultEntity>();
        }

        public IQueryable<TestResult> Select()
        {
            return table.Select(x => new TestResult
                                         {
                                             TestResultId = x.TestResultID,
                                             VirtualTestId = x.VirtualTestID,
                                             StudentId = x.StudentID,
                                             TeacherId = x.TeacherID ?? 0,
                                             SchoolId = x.SchoolID ?? 0,
                                             ResultDate = x.ResultDate,
                                             CreatedDate = x.CreatedDate,
                                             UpdatedDate = x.UpdatedDate,
                                             TermId = x.TermID ?? 0,
                                             ClassId = x.ClassID ?? 0,
                                             GradedById = x.GradedByID,
                                             ScoreType = x.ScoreType,
                                             ScoreValue = x.ScoreValue ?? 0,
                                             TRData = x.TRData,
                                             SubmitType = x.SubmitType ?? 0,
                                             DistrictTermId = x.DistrictTermID ?? 0,
                                             UserId = x.UserID ?? 0,
                                             OriginalUserId = x.OriginalUserID ?? 0,
                                             UIN = x.UIN,
                                             LegacyBatchId = x.LegacyBatchID ?? 0,
                                             BubbleSheetId = x.BubbleSheetID ?? 0
                                         });
        }

        public void SaveOnSubmit(TestResult item)
        {
            var entity = table.FirstOrDefault(x => x.TestResultID.Equals(item.TestResultId));

            if (entity == null)
            {
                entity = new TestResultEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
        }

        public void DeleteOnSubmit(TestResult item)
        {
            dbContext.DeleteTestResultAndSubItems(item.TestResultId);
        }

        public void SaveChanges()
        {
            table.Context.SubmitChanges();
        }
    }
}
