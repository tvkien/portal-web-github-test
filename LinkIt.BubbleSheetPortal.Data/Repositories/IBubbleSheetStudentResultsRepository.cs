using System.Collections.Generic;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBubbleSheetStudentResultsRepository : IReadOnlyRepository<BubbleSheetStudentResults>
    {
        List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicket(string ticket, int classID);
        List<BubbleSheetStudentResults> GetBubbleSheetStudentStatus(string studentIdList, string ticket, int classID);
        List<BubbleSheetStudentResults> GetStudentsOfBubbleSheetByTicketNoStatus(string ticket, int classID);
        List<string> GetBubbleSheetStudentStatusV2(string ticket, int classID);
        BubbleSheetFinalStatus BuildBBSFinalStatus(string status);
    }
}
