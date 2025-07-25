using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IVirtualTestRepository : IRepository<VirtualTestData>
    {
        VirtualTestProperty GetVirtualTestProperties(int virtualTestId, int roleId, int districtId);
        IQueryable<VirtualSectionQuestionQtiItem> GetVirtualSectionQuestionQtiItem(int virtualTestId);
        IQueryable<VirtualQuestionWithCorrectAnswer> GetVirtualQuestionWithCorrectAnswer(int virtualTestId);
        int CanDeleteVirtualTestById(int virtualtestId);
        void DeleteVirtualTestById(int virtualTestId, int userId, int roleId, out string error);
        VirtualQuestionProperties GetVirtualQuestionProperties(int virtualQuestionId);
        void RemoveVirtualSection(int virtualSectionId, out string error);
        void ReassignVirtualQuestionOrder(int virtualTestID);
        void RemoveVirtualQuestion(int virtualQuestionId, out string error);
        bool CanRemoveVirtualQuestion(string virtualQuestionIds, out string error);
        IQueryable<ComplexVirtualQuestionAnswerScore> GetComplexVirtualQuestionAnswerScores(int virtualQuestionId);
        void UpdateComplexVirtualQuestionAnswerScores(int virtualQuestionId, string responseIdentifier, int score,
                                               int subPointsPossible, int pointsPossible, out string error);
        void ReassignVirtualSectionOrder(int virtualTestID);
        void ReassignVirtualSectionQuestionOrder(int virtualTestId,int virtualSectionId);
        int FixSectionDataForVirtualTest(int virtualTestId);
        void MoveVirtualSectionQuestion(int virtualTestId, int sourceIndex, int sourceSectionId, int targetIndex,
                                        int targetSectionId);
        void MoveVirtualSection(int virtualTestId, int sourceIndex, int targetIndex);
        VirtualTestData GetVirtualTestByID(int virtualTestID);
        IQueryable<VirtualQuestionS3Object> GetVirtualQuestionToCreateS3Object(int virtualTestId);
        List<GhostQuestion> GetGhostQuestions(int baseVirtualQuestionID);
        int? GetMaxQuestionOrder(int virtualTestID);
        void ReassignBaseVirtualSectionQuestionOrder(int virtualTestId, int? virtualSectionId);
        List<VirtualSectionQuestion> GetBaseQuestions(int virtualTestId, int virtualQuestionId);
        void MoveManyVirtualSectionQuestion(int virtualTestId, string virtualQuestionIdString, int sourceIndex, int sourceSectionId, int targetIndex,
                                        int targetSectionId, out string message);
        void UpdateBaseVirtualQuestionClone(int oldVirtualTestId, int newVirtualTestId);
        IQueryable<ChoiceVariableVirtualQuestionAnswerScore> GetChoiceVariableVirtualQuestionAnswerScores(
            int virtualQuestionId);
        void MoveManyVirtualQuestionGroup(int virtualTestId, string virtualQuestionIdString, int? sourceQuestionGroupId, int targetVirtualSectionId, int? targetQuestionGroupId, int targetIndex);
        AlgorithmicMaxPoint GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(int virtualQuestionId, int qtiItemId);
        void ClearQuestionLabelQuestionLNumber(int virtualTestId);
        List<VirtualTestOrder> GetVirtualTestOrders(int districtId);
        int CountOpendedQuestionPerTest(int virtualtestId);
        List<ListItem> VirtualTestWithOutTestResultForPublisher(int districtId);
        void DeleteVirtualQuestionBranchingByTestID(int virtualTestId);        
        void DeleteVirtualSectionBranchingByTestID(int virtualTestId);
        void DeleteVirtualQuestionBranchingAlgorithmByTestID(int virtualTestId);
        void ChangePositionVirtualSection(int virtualTestId, int sourceIndex, int targetIndex);
        List<ConstructedResponseQuestion> GetConstructedResponseQuestions(int virtualTestId);

        void ClonePBSForTestRetake(int oldTestId, int newTestId);
        GetTestByAdvanceFilter GetTestByAdvanceFilter(GetTestByAdvanceFilterRequest request);
        (string CategoriesName, IList<TestPropertyExportData> DataExport) GetTestPropertyToExportByVirtualTestIDs(int districtID, string strVirtualTestID);
        IList<TestLibraryExportData> GetTestLibraryToExportByDistrictID(int districtID);
        IList<ChoiceVariableVirtualQuestionAnswerScore> GetChoiceVariableVirtualQuestionAnswerScoresByVirtualQuestionIDs(string strVirtualQuestionID);
        IList<ComplexVirtualQuestionAnswerScore> GetComplexVirtualQuestionAnswerScoresByVirtualQuestionIDs(string strVirtualQuestionID);
    }
}
