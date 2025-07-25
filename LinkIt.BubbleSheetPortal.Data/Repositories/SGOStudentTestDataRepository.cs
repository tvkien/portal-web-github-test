using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SGOStudentTestDataRepository : IReadOnlyRepository<SGOStudentTestData>
    {
         private readonly Table<SGOStudentTestDataView> table;

         public SGOStudentTestDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = SGODataContext.Get(connectionString).GetTable<SGOStudentTestDataView>();
        }

         public IQueryable<SGOStudentTestData> Select()
         {
             return table.Select(x => new SGOStudentTestData
             {
                 BankId = x.BankID,
                 BankName = x.BankName,
                 GradeId = x.GradeID,
                 GradeName = x.GradeName,
                 GradeOrder = x.GradeOrder,
                 StateId = x.StateID,
                 StateName = x.StateName,
                 StudentId = x.StudentID,
                 SubjectId = x.SubjectID,
                 SubjectName = x.SubjectName,
                 TestResultId = x.TestResultID,
                 VirtualTestId = x.VirtualTestID,
                 VirtualTestName = x.VirtualTestName,
                 AchievementLevelSettingId = x.AchievementLevelSettingID,
                 AchievementLevelSettingName = x.AchievementLevelSettingName,
                 VirtualTestSourceId = x.VirtualTestSourceID
             });
         }
    }
}
