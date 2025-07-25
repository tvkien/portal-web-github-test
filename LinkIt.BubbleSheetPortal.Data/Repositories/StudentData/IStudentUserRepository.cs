using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup;
using System.Collections.Generic;
using System.Data;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.StudentData
{
    public interface IStudentUserRepository : IRepository<StudentUser>
    {
        int GetUserIDViaStudentUser(int studentId);
        int GetStudentIDViaStudentUser(int userId);
        List<CalculatedMetaData> GetCalculatedMetaData(DataTable calculatedTable, DataTable studentTable);
    }
}
