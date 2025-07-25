using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System;
using System.Data.Linq;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DataLocker
{
    public class TestResultScoreNoteRepository : IRepository<TestResultScoreNote>
    {
        private readonly Table<TestResultScoreNoteEntity> table;
        private readonly DataLockerContextDataContext dbContext;

        public TestResultScoreNoteRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DataLockerContextDataContext.Get(connectionString).GetTable<TestResultScoreNoteEntity>();
        }

        public void Save(TestResultScoreNote item)
        {
            throw new NotImplementedException();
        }

        public void Delete(TestResultScoreNote item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TestResultScoreNote> Select()
        {
            return table.Select(x => new TestResultScoreNote
            {
                Name = x.Name,
                Note = x.Note,
                NoteKey = x.NoteKey,
                TestResultScoreID = x.TestResultScoreID,
                TestResultScoreNoteID = x.TestResultScoreNotesID
            });
        }
    }
}
