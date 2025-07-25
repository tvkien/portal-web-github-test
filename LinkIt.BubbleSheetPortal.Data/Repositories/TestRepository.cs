using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TestRepository : IReadOnlyRepository<Test>
    {
        private readonly Table<TestEntity> table;

        public TestRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<TestEntity>();
        }

        public IQueryable<Test> Select()
        {
            return table.Select(x => new Test
                {
                    Id = x.VirtualTestID,
                    Type = x.VirtualTestType,
                    BankId = x.BankID,
                    StateId = x.StateID,
                    Name = x.Name,
                    QuestionCount = x.QuestionEntities.Count,
                    VirtualTestSubTypeId = x.VirtualTestSubTypeID,
                    VirtualTestSourceId = x.VirtualTestSourceID,
                    IsTeacherLed = x.IsTeacherLed,
                    //TOOD: Task: [LNKT-30796] Phase 2
                    QuestionGroupCount = x.QuestionGroupEntities.Count,
                    NavigationMethodId = x.NavigationMethodID,
                    DataSetOriginID = x.DataSetOriginID,
                    DataSetCategoryID = x.DataSetCategoryID,
                    ParentTestID = x.ParentTestID,
                    OriginalTestID = x.OriginalTestID,
                    AchievementLevelSettingID = x.AchievementLevelSettingID,
                    AuthorUserID = x.AuthorUserID
            });
        }
    }
}
