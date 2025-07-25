using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryTestResultUnitOfWorkRepository : IUnitOfWorkRepository<TestResult>
    {
        public static readonly List<TestResult> Data = GetData();
        public static readonly List<TestResult> PendingAdd = new List<TestResult>();
        public static readonly List<TestResult> PendingUpdate = new List<TestResult>();
        public static readonly List<TestResult> PendingRemove = new List<TestResult>();
 
        public static void Clear()
        {
            Data.Clear();
            Data.AddRange(GetData());
            PendingAdd.Clear();
            PendingUpdate.Clear();
            PendingRemove.Clear();
        }

        public IQueryable<TestResult> Select()
        {
            return Data.AsReadOnly().AsQueryable();
        }

        public void SaveOnSubmit(TestResult item)
        {
            var entity = Data.FirstOrDefault(x => x.TestResultId == item.TestResultId);
            if (entity == null)
            {
                PendingAdd.Add(item);
            }
            else
            {
                PendingUpdate.Add(item);
            }
        }

        public void DeleteOnSubmit(TestResult item)
        {
            PendingRemove.Add(item);
        }

        public void SaveChanges()
        {
            foreach (var testResult in PendingAdd)
            {
                Data.Add(testResult);
            }

            foreach (var testResult in PendingUpdate)
            {
                var entity = Data.Single(x => x.TestResultId == testResult.TestResultId);
                Data.Remove(entity);
                Data.Add(testResult);
            }

            foreach (var testResult in PendingRemove)
            {
                var entity = Data.Single(x => x.TestResultId == testResult.TestResultId);
                Data.Remove(entity);
            }
        }

        private static List<TestResult> GetData()
        {
            return new List<TestResult>
            {
                new TestResult { ClassId = 1, StudentId = 10 }
            };
        }
    }
}