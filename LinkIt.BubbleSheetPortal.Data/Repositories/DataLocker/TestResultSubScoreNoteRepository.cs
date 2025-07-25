using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DataLocker
{
    public class TestResultSubScoreNoteRepository : IRepository<TestResultSubScoreNote>
    {
        private readonly Table<TestResultSubScoreNoteEntity> table;
        private readonly DataLockerContextDataContext dbContext;

        public TestResultSubScoreNoteRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DataLockerContextDataContext.Get(connectionString).GetTable<TestResultSubScoreNoteEntity>();
        }

        public void Save(TestResultSubScoreNote item)
        {
            throw new NotImplementedException();
        }

        public void Delete(TestResultSubScoreNote item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TestResultSubScoreNote> Select()
        {
            return table.Select(x => new TestResultSubScoreNote
            {
                Name = x.Name,
                Note = x.Note,
                NoteKey = x.NoteKey,
                TestResultSubScoreID = x.TestResultSubScoreID,
                TestResultSubScoreNoteID = x.TestResultSubScoreNotesID
            });
        }
    }
}
