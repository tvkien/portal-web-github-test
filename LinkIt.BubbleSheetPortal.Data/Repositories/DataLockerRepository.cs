using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using System.Collections.Generic;
using System.Xml.Linq;
using LinkIt.BubbleSheetPortal.Models;
using System;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using System.Data;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class DataLockerRepository : IDataLockerRepository
    {
        DataLockerContextDataContext _context;
        public DataLockerRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = DataLockerContextDataContext.Get(connectionString);
            _context.CommandTimeout = 36000;
        }
        public DataSet GetPBSScoreMetaData(int districtID, string virtualTestIDs)
        {
            var parameters = new List<(string, string, SqlDbType, object, ParameterDirection)>();
            parameters.Add(("DistrictID", "", SqlDbType.Int, districtID, ParameterDirection.Input));
            parameters.Add(("VirtualTestIDs", "", SqlDbType.VarChar, virtualTestIDs, ParameterDirection.Input));
            return _context.QueryMutipleTable<DataSet>(new SqlParameterRequest()
            {
                StoredName = "AR_GetPBSForTestResult",
                Parameters = parameters
            });
        }
        public bool HasAssociatedAutoSave(int virtualTestCustomScoreId)
        {
            var virtualTest_VirtualTestCustomScores = _context.VirtualTest_VirtualTestCustomScoreDLEntities.AsQueryable();
            var autoSaveResult = _context.DTLAutoSaveResultDataEntities.AsQueryable();
            return virtualTest_VirtualTestCustomScores.Join(autoSaveResult, vTCS => vTCS.VirtualTestID, aTSR => aTSR.VirtualTestID, (vTCS, aTSR) => new { vTCS, aTSR })
                .Where(w => w.vTCS.VirtualTestCustomScoreID == virtualTestCustomScoreId).Any();
        }
        public bool DTLTemplateHasAssociatedTestResult(int virtualTestCustomScoreId)
        {
            var found = _context.DTLTemplateHasAssociatedTestResult(virtualTestCustomScoreId);
            return found == 1;
        }

        public void DTLDeleteSubscore(int virtualTestCustomSubScoreId)
        {
            _context.DTLDeleteSubscore(virtualTestCustomSubScoreId);
        }

        public void DTLDeleteTemplate(int virtualTestCustomScoreId)
        {
            _context.DTLDeleteTemplate(virtualTestCustomScoreId);
        }

        public void DTLArchiveTemplate(int virtualTestCustomScoreId, bool archived)
        {
            _context.DTLArchiveTemplate(virtualTestCustomScoreId, archived);
        }

        public List<DTLStudentAndTestResultScore> GetStudentAndTestResultScore(int virtualTestId, int classId, string studentIds)
        {
            return _context.DTLGetStudentAndTestResultScore(virtualTestId, classId, studentIds)
                .Select(x => new DTLStudentAndTestResultScore
                {
                    AchievementLevel = x.AchievementLevel,
                    ClassID = x.ClassID ?? classId,
                    Code = x.Code,
                    AltCode = x.AltCode,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    ResultDate = x.ResultDate,
                    ScoreCustomA_1 = x.ScoreCustomA_1,
                    ScoreCustomA_2 = x.ScoreCustomA_2,
                    ScoreCustomA_3 = x.ScoreCustomA_3,
                    ScoreCustomA_4 = x.ScoreCustomA_4,
                    ScoreCustomN_1 = x.ScoreCustomN_1,
                    ScoreCustomN_2 = x.ScoreCustomN_2,
                    ScoreCustomN_3 = x.ScoreCustomN_3,
                    ScoreCustomN_4 = x.ScoreCustomN_4,
                    ScorePercent = x.ScorePercent,
                    ScorePercentage = x.ScorePercentage,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentID = x.StudentID,
                    TestResultID = x.TestResultID,
                    TestResultScoreID = x.TestResultScoreID,
                    VirtualTestID = x.VirtualTestID ?? virtualTestId
                }).ToList();
        }

        public List<DTLStudentAndTestResultScore> GetStudentAndTestResultScoreMultiple(int virtualTestId, int classId, string studentIds, DateTime? entryResultDate = null)
        {
            return _context.DTLGetStudentAndTestResultScoreMultiple(virtualTestId, classId, studentIds, entryResultDate)
                .Select(x => new DTLStudentAndTestResultScore
                {
                    AchievementLevel = x.AchievementLevel,
                    ClassID = x.ClassID ?? classId,
                    Code = x.Code,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    ResultDate = x.ResultDate,
                    ScoreCustomA_1 = x.ScoreCustomA_1,
                    ScoreCustomA_2 = x.ScoreCustomA_2,
                    ScoreCustomA_3 = x.ScoreCustomA_3,
                    ScoreCustomA_4 = x.ScoreCustomA_4,
                    ScoreCustomN_1 = x.ScoreCustomN_1,
                    ScoreCustomN_2 = x.ScoreCustomN_2,
                    ScoreCustomN_3 = x.ScoreCustomN_3,
                    ScoreCustomN_4 = x.ScoreCustomN_4,
                    ScorePercent = x.ScorePercent,
                    ScorePercentage = x.ScorePercentage,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentID = x.StudentID,
                    TestResultID = x.TestResultID,
                    TestResultScoreID = x.TestResultScoreID,
                    VirtualTestID = x.VirtualTestID
                }).ToList();
        }

        public List<DTLStudentAndTestResultSubScore> GetStudentAndTestResultSubScore(int virtualTestId, int classId, string studentIds)
        {
            return _context.DTLGetStudentAndTestResultSubScore(virtualTestId, classId, studentIds)
                .Select(x => new DTLStudentAndTestResultSubScore
                {
                    ClassID = x.ClassID,
                    Code = x.Code,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    MiddleName = x.MiddleName,
                    ResultDate = x.ResultDate,
                    Name = x.Name,
                    ScoreCustomA_1 = x.ScoreCustomA_1,
                    ScoreCustomA_2 = x.ScoreCustomA_2,
                    ScoreCustomA_3 = x.ScoreCustomA_3,
                    ScoreCustomA_4 = x.ScoreCustomA_4,
                    ScoreCustomN_1 = x.ScoreCustomN_1,
                    ScoreCustomN_2 = x.ScoreCustomN_2,
                    ScoreCustomN_3 = x.ScoreCustomN_3,
                    ScoreCustomN_4 = x.ScoreCustomN_4,
                    ScorePercent = x.ScorePercent,
                    ScorePercentage = x.ScorePercentage,
                    ScoreRaw = x.ScoreRaw,
                    ScoreScaled = x.ScoreScaled,
                    StudentID = x.StudentID,
                    TestResultID = x.TestResultID,
                    TestResultScoreID = x.TestResultScoreID,
                    VirtualTestID = x.VirtualTestID,
                    TestResultScoreSubID = x.TestResultSubScoreID
                }).ToList();
        }

        public List<DTLStudentAndTestResultSubScore> GetStudentAndTestResultSubScoreMultiple(int virtualTestId, int classId, string studentIds, DateTime entryResultDate)
        {
            return _context.DTLGetStudentAndTestResultSubScoreMultiple(virtualTestId, classId, studentIds, entryResultDate)
              .Select(x => new DTLStudentAndTestResultSubScore
              {
                  ClassID = x.ClassID,
                  Code = x.Code,
                  FirstName = x.FirstName,
                  LastName = x.LastName,
                  MiddleName = x.MiddleName,
                  ResultDate = x.ResultDate,
                  Name = x.Name,
                  ScoreCustomA_1 = x.ScoreCustomA_1,
                  ScoreCustomA_2 = x.ScoreCustomA_2,
                  ScoreCustomA_3 = x.ScoreCustomA_3,
                  ScoreCustomA_4 = x.ScoreCustomA_4,
                  ScoreCustomN_1 = x.ScoreCustomN_1,
                  ScoreCustomN_2 = x.ScoreCustomN_2,
                  ScoreCustomN_3 = x.ScoreCustomN_3,
                  ScoreCustomN_4 = x.ScoreCustomN_4,
                  ScorePercent = x.ScorePercent,
                  ScorePercentage = x.ScorePercentage,
                  ScoreRaw = x.ScoreRaw,
                  ScoreScaled = x.ScoreScaled,
                  StudentID = x.StudentID,
                  TestResultID = x.TestResultID,
                  TestResultScoreID = x.TestResultScoreID,
                  VirtualTestID = x.VirtualTestID,
                  TestResultScoreSubID = x.TestResultSubScoreID
              }).ToList();
        }

        public void SaveEntryResults(string testresultsXML, string testresultscoresXML, string testresultsubscoresXML, string testresultscoreNotesXML, string testresultSubScoreNotesXML, string testresultscoreUploadFileXML, string testresultsubScoreUploadFileXML, string testresultidDelete)
        {
            _context.DTLSaveEntryResults(GetElement(testresultsXML), GetElement(testresultscoresXML), GetElement(testresultsubscoresXML), GetElement(testresultscoreNotesXML), GetElement(testresultSubScoreNotesXML), GetElement(testresultscoreUploadFileXML), GetElement(testresultsubScoreUploadFileXML), testresultidDelete);
        }

        public void SaveEntryResultsMultiple(string testresultsXML, string testresultscoresXML, string testresultsubscoresXML, string testresultscoreNotesXML, string testresultSubScoreNotesXML, string testresultscoreUploadFileXML, string testresultsubScoreUploadFileXML, string testresultidDelete)
        {
            _context.DTLSaveEntryResultsMultiple(GetElement(testresultsXML), GetElement(testresultscoresXML), GetElement(testresultsubscoresXML), GetElement(testresultscoreNotesXML), GetElement(testresultSubScoreNotesXML), GetElement(testresultscoreUploadFileXML), GetElement(testresultsubScoreUploadFileXML), testresultidDelete);
        }

        private XElement GetElement(string xml)
        {
            return XElement.Parse(xml);
        }

        public void DeleteVirtualTestLegacyById(int virtualTestId, int userId, int roleId, out string error)
        {
            error = string.Empty;
            _context.DeleteVirtualTestLegacyByID(virtualTestId, userId, roleId, ref error);
        }

        public int CopyTemplateByID(int templateId, int userId, string templateName)
        {
            var result = _context.DTLCopyTemplateByID(templateId, userId, templateName).ToList();
            if (result.Count > 0)
                return result.FirstOrDefault().TemplateIDNew.GetValueOrDefault();

            return 0;
        }

        public void DeleteAllAutoSaveData(int virtualTestId, int classId)
        {
            _context.DTLDeleteAllAutoSaveData(virtualTestId, classId);
        }
        public void DeleteAutoSaveMultiDate(int virtualTestId, int classId)
        {
            _context.DTLDeleteAllAutoSaveData(virtualTestId, classId);
        }
        public void DeleteAllAutoSaveDataBaseDate(int virtualTestId, int classId, DateTime dateSave)
        {
            _context.DTLDeleteAllAutoSaveDataBaseDate(virtualTestId, classId, dateSave);
        }
        public List<DTLFormModel> GetFormsByBankID(int bankId, bool isFromMultiDate, bool usingMultiDate)
        {
            return _context.GetFormDataLockerByBankID(bankId, isFromMultiDate, usingMultiDate).Select(x => new DTLFormModel() { Id = x.VirtualTestID, Name = x.TestName, IsMultiDate = x.IsMultiDate ?? false }).ToList();
        }

        public void AutoSaveResultDataBaseDate(DTLAutoSaveResultData data)
        {
            if (data == null) return;
            var entity = _context.DTLAutoSaveResultDataEntities.FirstOrDefault(o => o.UserID == data.UserId && o.ClassID == data.ClassId
                                                                                && o.VirtualTestID == data.VirtualTestId
                                                                                && o.ResultDate == data.ResultDate);
            if (entity == null)
            {
                entity = new DTLAutoSaveResultDataEntity() { CreatedDate = DateTime.UtcNow };
                _context.DTLAutoSaveResultDataEntities.InsertOnSubmit(entity);
            }

            Map(data, entity);
            _context.SubmitChanges();
        }

        internal void Map(DTLAutoSaveResultData data, DTLAutoSaveResultDataEntity entity)
        {
            if (data == null || entity == null) return;
            entity.VirtualTestID = data.VirtualTestId;
            entity.UserID = data.UserId;
            entity.ClassID = data.ClassId;
            entity.StudentTestResultScoresJson = data.StudentTestResultScoresJson;
            entity.StudentTestResultSubScoresJson = data.StudentTestResultSubScoresJson;
            entity.ActualTestResultScoresJson = data.ActualTestResultScoresJson;
            entity.UpdatedDate = DateTime.UtcNow;
            entity.ResultDate = data.ResultDate;
        }
    }
}
