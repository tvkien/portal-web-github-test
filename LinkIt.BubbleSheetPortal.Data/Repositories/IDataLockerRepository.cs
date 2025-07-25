using System;
using System.Collections.Generic;
using System.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IDataLockerRepository
    {
        bool DTLTemplateHasAssociatedTestResult(int virtualTestCustomScoreId);
        void DTLDeleteSubscore(int virtualTestCustomSubScoreId);
        void DTLDeleteTemplate(int virtualTestCustomScoreId);
        void DTLArchiveTemplate(int virtualTestCustomScoreId, bool archived);

        List<DTLStudentAndTestResultScore> GetStudentAndTestResultScore(int virtualTestId, int classId, string studentIds);
        List<DTLStudentAndTestResultScore> GetStudentAndTestResultScoreMultiple(int virtualTestId, int classId, string studentIds, DateTime? entryResultDate = null);
        List<DTLStudentAndTestResultSubScore> GetStudentAndTestResultSubScore(int virtualTestId, int classId, string studentIds);
        List<DTLStudentAndTestResultSubScore> GetStudentAndTestResultSubScoreMultiple(int virtualTestId, int classId, string studentIds, DateTime entryResultDate);
        void DeleteVirtualTestLegacyById(int virtualTestId, int userId, int roleId, out string error);
        void SaveEntryResults(string testresultsXML, string testresultscoresXML, string testresultsubscoresXML, string testresultscoreNotesXML, string testresultSubScoreNotesXML, string testresultscoreUploadFileXML, string testresultsubScoreUploadFileXML, string testresultidDelete);
        void SaveEntryResultsMultiple(string testresultsXML, string testresultscoresXML, string testresultsubscoresXML, string testresultscoreNotesXML, string testresultSubScoreNotesXML, string testresultscoreUploadFileXML, string testresultsubScoreUploadFileXML, string testresultidDelete);
        void DeleteAllAutoSaveData(int virtualTestId, int classId);
        void DeleteAllAutoSaveDataBaseDate(int virtualTestId, int classId, DateTime dateSave);
        int CopyTemplateByID(int templateId, int userId, string templateName);
        List<DTLFormModel> GetFormsByBankID(int bankId, bool isFromMultiDate, bool usingMultiDate);
        void AutoSaveResultDataBaseDate(DTLAutoSaveResultData data);
        bool HasAssociatedAutoSave(int virtualTestCustomScoreId);
        DataSet GetPBSScoreMetaData(int districtID, string VirtualTestIDs);
    }
}
