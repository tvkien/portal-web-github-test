using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IDataLockerForStudentRepository
    {
        GetAttachmentForStudentResponse GetListAttachmentForStudents(GetDatalockerForStudentPaginationRequest request);
        QTITestClassAssignmentData GetQTITestClassAssignment(int virtualTestID, int classID, int type);
        List<int> GetStudentTestResultByVirtualTestAndClass(int virtualTestID, int classID);
        (List<TestResultScoreArtifact>, List<Guid?>) SaveStudentArtifacts(SaveStudentAttachmentsParameters model, int userId);
    }
}
