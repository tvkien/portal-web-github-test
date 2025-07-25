using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTITestClassAssignmentRepository
    {
        void SaveMutipleRecord(List<QTITestClassAssignmentData> items);
        CheckMatchEmailDto CheckMatchEmail(string email, int districtId, int virtualTestId, int termId, int assignmentType);
        bool CanActiveForRetake(int qtiTestClassAssignmentID);
        void InsertMultipleRecord(List<QTITestClassAssignmentData> items);
    }
}
