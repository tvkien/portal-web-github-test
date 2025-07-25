using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Common;
using AutoMapper;
using System.Data;
using LinkIt.BubbleSheetPortal.Models.Old.DataLocker;
using Newtonsoft.Json.Linq;
using LinkIt.BubbleSheetPortal.Common.JsonExtension;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DataLockerService
    {
        private IRepository<VirtualTestCustomScore> _virtualTestCustomScoreRepository;
        private IVirtualTestCustomMetaDataRepository _virtualTestCustomMetaDataRepository;
        private IRepository<VirtualTestCustomSubScore> _virtualTestCustomSubScoreRepository;
        private IDataLockerRepository _dataLockerRepository;
        private IRepository<VirtualTestVirtualTestCustomScore> _virtualTestVirtualTestCustomScore;
        IReadOnlyRepository<TestResultScoreUploadFile> _testResultScoreUploadFileRepository;
        private IRepository<TestResultScoreNote> _testResultScoreNoteRepository;
        private IRepository<TestResultSubScoreNote> _testResultSubScoreNoteRepository;
        private IRepository<DTLAutoSaveResultData> _autoSaveRepository;
        private IRepository<TestResult> _testResultRepository;
        private IRepository<ClassStudent> _classStudentRepository;
        private IDocumentManagement _documentManagementService;
        private DistrictDecodeService _districtDecodeService;

        public DataLockerService(IRepository<VirtualTestCustomScore> virtualTestCustomScoreRepository,
            IVirtualTestCustomMetaDataRepository virtualTestCustomMetaDataRepository,
            IRepository<VirtualTestCustomSubScore> virtualTestCustomSubScoreRepository,
            IDataLockerRepository dataLockerRepository,
            IRepository<VirtualTestVirtualTestCustomScore> virtualTestVirtualTestCustomScore,
            IReadOnlyRepository<TestResultScoreUploadFile> testResultScoreUploadFileRepository,
            IRepository<TestResultScoreNote> testResultScoreNoteRepository,
            IRepository<TestResultSubScoreNote> testResultSubScoreNoteRepository,
            IRepository<DTLAutoSaveResultData> autoSaveRepository,
            IRepository<TestResult> testResultRepository,
            IRepository<ClassStudent> classStudentRepository,
            IDocumentManagement documentManagementService,
            DistrictDecodeService districtDecodeService
            )
        {
            _virtualTestCustomScoreRepository = virtualTestCustomScoreRepository;
            _virtualTestCustomMetaDataRepository = virtualTestCustomMetaDataRepository;
            _virtualTestCustomSubScoreRepository = virtualTestCustomSubScoreRepository;
            _dataLockerRepository = dataLockerRepository;
            _virtualTestVirtualTestCustomScore = virtualTestVirtualTestCustomScore;
            _testResultScoreUploadFileRepository = testResultScoreUploadFileRepository;
            _testResultScoreNoteRepository = testResultScoreNoteRepository;
            _testResultSubScoreNoteRepository = testResultSubScoreNoteRepository;
            _autoSaveRepository = autoSaveRepository;
            _testResultRepository = testResultRepository;
            _classStudentRepository = classStudentRepository;
            _documentManagementService = documentManagementService;
            _districtDecodeService = districtDecodeService;
        }
        public List<Dictionary<string, string>> GetPBSColorForScoreResult(List<PerformanceBandSettingScoreModel> pBSs, DTLStudentAndTestResultScore score, DTLStudentAndTestResultSubScore subScore, bool isSubScore)
        {
            var scoreInfos = new List<Dictionary<string, string>>();
            if (pBSs != null && pBSs.Count > 0)
            {
                if (isSubScore)
                {
                    score = new DTLStudentAndTestResultScore();
                    score.ScoreRaw = subScore.ScoreRaw;
                    score.ScoreScaled = subScore.ScoreScaled;
                    score.ScorePercent = subScore.ScorePercent;
                    score.ScorePercentage = subScore.ScorePercentage;
                    score.ScoreCustomN_1 = subScore.ScoreCustomN_1;
                    score.ScoreCustomN_2 = subScore.ScoreCustomN_2;
                    score.ScoreCustomN_3 = subScore.ScoreCustomN_3;
                    score.ScoreCustomN_4 = subScore.ScoreCustomN_4;
                }
                foreach (var pbs in pBSs)
                {
                    if (score.ScoreRaw.HasValue && pbs.ScoreType == Constanst.ScoreRaw)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScoreRaw, GetPBSForItemScore(pbs, score.ScoreRaw.Value) } });
                    }
                    if (score.ScoreScaled.HasValue && pbs.ScoreType == Constanst.ScoreScaled)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScoreScaled, GetPBSForItemScore(pbs, score.ScoreScaled.Value) } });
                    }
                    if (score.ScorePercent.HasValue && pbs.ScoreType == Constanst.ScorePercent)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScorePercent, GetPBSForItemScore(pbs, score.ScorePercent.Value) } });
                    }
                    if (score.ScorePercentage.HasValue && pbs.ScoreType == Constanst.ScorePercentage)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScorePercentage, GetPBSForItemScore(pbs, score.ScorePercentage.Value) } });
                    }
                    if (score.ScoreCustomN_1.HasValue && pbs.ScoreType == Constanst.ScoreCustomN_1)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScoreCustomN_1, GetPBSForItemScore(pbs, score.ScoreCustomN_1.Value) } });
                    }
                    if (score.ScoreCustomN_2.HasValue && pbs.ScoreType == Constanst.ScoreCustomN_2)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScoreCustomN_2, GetPBSForItemScore(pbs, score.ScoreCustomN_2.Value) } });
                    }
                    if (score.ScoreCustomN_3.HasValue && pbs.ScoreType == Constanst.ScoreCustomN_3)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScoreCustomN_3, GetPBSForItemScore(pbs, score.ScoreCustomN_3.Value) } });
                    }
                    if (score.ScoreCustomN_4.HasValue && pbs.ScoreType == Constanst.ScoreCustomN_4)
                    {
                        scoreInfos.Add(new Dictionary<string, string>() { { Constanst.ScoreCustomN_4, GetPBSForItemScore(pbs, score.ScoreCustomN_4.Value) } });
                    }
                }
            }
            return scoreInfos;
        }
        private string GetPBSForItemScore(PerformanceBandSettingScoreModel pbs, decimal score)
        {
            var data = "";
            if (!string.IsNullOrEmpty(pbs.Cutoffs) && !string.IsNullOrEmpty(pbs.Color))
            {
                var arrCutoff = pbs.Cutoffs.Split(',');
                var colorList = pbs.Color.Split(';');
                var scoreIndex = arrCutoff.Select(s => Convert.ToDecimal(s)).ToList().FindIndex(x => score >= x);
                if (scoreIndex == -1)
                    scoreIndex = arrCutoff.Count();
                data  = colorList[scoreIndex];
            }
            return data;
        }
        public DataSet GetPBSScoreMetaData(int districtID, string virtualTestIDs)
        {
            return _dataLockerRepository.GetPBSScoreMetaData(districtID, virtualTestIDs);
        }

        public bool HasAssociatedAutoSave(int virtualTestCustomScoreId)
        {
            return _dataLockerRepository.HasAssociatedAutoSave(virtualTestCustomScoreId);
        }

        public List<DTLStudentAndTestResultScore> GetStudentAndTestResultScore(int virtualTestId, int classId, string studentIds)
        {
            var testResultScores = _dataLockerRepository.GetStudentAndTestResultScore(virtualTestId, classId, studentIds).ToList();
            var testResultScoreIDs = testResultScores.Where(x => x.TestResultScoreID.HasValue).Select(x => x.TestResultScoreID.Value).ToList();
            if (testResultScoreIDs.Count > 0)
            {
                var testResultScoreNotes = _testResultScoreNoteRepository.Select()
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultScoreID);
                    }, testResultScoreIDs)
                    .ToList();

                var testResultScoreArtifacts = _testResultScoreUploadFileRepository.Select()
                    .Where(c => c.TestResultScoreID.HasValue)
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultScoreID.Value);
                    }, testResultScoreIDs)
                    .ToList();

                foreach (var testResultScore in testResultScores)
                {
                    testResultScore.Artifacts = testResultScoreArtifacts.Where(x => x.TestResultScoreID == testResultScore.TestResultScoreID)
                        .Select(x => new TestResultScoreArtifact
                        {
                            Name = x.FileName,
                            IsLink = x.IsUrl,
                            Url = x.IsUrl == true ? x.FileName : "",
                            UploadDate = x.UploadDate,
                            TagValue = x.Tag,
                            DocumentGuid = x.DocumentGuid,
                            CreatedBy = x.CreatedBy
                        })
                        .ToList();

                    testResultScore.Notes = testResultScoreNotes.Where(x => x.TestResultScoreID == testResultScore.TestResultScoreID).ToList();
                }
            }

            var otherResultStudentIds = GetStudentHasResultInOtherClass(virtualTestId, classId, studentIds);
            testResultScores.ForEach(x => x.HasOtherScore = otherResultStudentIds.Contains(x.StudentID));

            return testResultScores;
        }

        public List<DTLStudentAndTestResultScore> GetStudentAndTestResultScoreMultiple(int virtualTestId, int classId, string studentIds, DateTime entryResultDate)
        {
            var testResultScores = _dataLockerRepository.GetStudentAndTestResultScoreMultiple(virtualTestId, classId, studentIds, entryResultDate).ToList();
            var testResultScoreIDs = testResultScores.Where(x => x.TestResultScoreID.HasValue).Select(x => x.TestResultScoreID.Value).ToList();
            if (testResultScoreIDs.Count > 0)
            {
                var testResultScoreNotes = _testResultScoreNoteRepository
                    .Select()
                    .FilterOnLargeSet(entity => entity, (subSet) =>
                    {
                        return entry => subSet.Contains(entry.TestResultScoreID);
                    }, testResultScoreIDs)
                    .ToList();

                var testResultScoreArtifacts = _testResultScoreUploadFileRepository
                    .Select()
                    .Where(x => x.TestResultScoreID.HasValue)
                    .FilterOnLargeSet
                    (entity => entity, (subSet) =>
                    {
                        return entry => subSet.Contains(entry.TestResultScoreID.Value);
                    }, testResultScoreIDs)
                    .ToList();

                foreach (var testResultScore in testResultScores)
                {
                    testResultScore.Artifacts = testResultScoreArtifacts.Where(x => x.TestResultScoreID == testResultScore.TestResultScoreID)
                        .Select(x => new TestResultScoreArtifact
                        {
                            Name = x.FileName,
                            IsLink = x.IsUrl,
                            Url = x.IsUrl ? x.FileName : "",
                            UploadDate = x.UploadDate,
                            TagValue = x.Tag,
                            DocumentGuid = x.DocumentGuid,
                            CreatedBy = x.CreatedBy
                        })
                        .ToList();

                    testResultScore.Notes = testResultScoreNotes.Where(x => x.TestResultScoreID == testResultScore.TestResultScoreID).ToList();
                }
            }

            var otherResultStudentIds = GetStudentHasResultInOtherClass(virtualTestId, classId);
            testResultScores.ForEach(x => x.HasOtherScore = otherResultStudentIds.Contains(x.StudentID));

            return testResultScores;
        }

        public List<DTLStudentAndTestResultSubScore> GetStudentAndTestResultSubScore(int virtualTestId, int classId, string studentIds)
        {
            var testResultSubScores = _dataLockerRepository.GetStudentAndTestResultSubScore(virtualTestId, classId, studentIds);
            var testResultSubScoreIDs = testResultSubScores.Where(x => x.TestResultScoreSubID.HasValue)
                                          .Select(x => x.TestResultScoreSubID.Value).ToList();

            if (testResultSubScoreIDs.Count > 0)
            {
                var testResultSubScoreNotes = _testResultSubScoreNoteRepository.Select()
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultSubScoreID);
                    }, testResultSubScoreIDs)
                    .ToList();

                var testResultScoreArtifacts = _testResultScoreUploadFileRepository.Select()
                    .Where(x => x.TestResultSubScoreID.HasValue)
                    .FilterOnLargeSet(entry => entry,
                    (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultSubScoreID.Value);
                    }, testResultSubScoreIDs)
                    .ToList();

                foreach (var testResultSuScore in testResultSubScores)
                {
                    testResultSuScore.Artifacts = testResultScoreArtifacts.Where(x => x.TestResultSubScoreID == testResultSuScore.TestResultScoreSubID)
                        .Select(x => new TestResultScoreArtifact
                        {
                            Name = x.FileName,
                            IsLink = x.IsUrl,
                            Url = x.IsUrl == true ? x.FileName : "",
                            UploadDate = x.UploadDate,
                            TagValue = x.Tag,
                            DocumentGuid = x.DocumentGuid,
                            CreatedBy = x.CreatedBy
                        })
                        .ToList();

                    testResultSuScore.Notes = testResultSubScoreNotes.Where(x => x.TestResultSubScoreID == testResultSuScore.TestResultScoreSubID).ToList();
                }
            }

            var otherResultStudentIds = GetStudentHasResultInOtherClass(virtualTestId, classId, studentIds);
            testResultSubScores.ForEach(x => x.HasOtherScore = otherResultStudentIds.Contains(x.StudentID));

            return testResultSubScores;
        }

        public List<DTLStudentAndTestResultSubScore> GetStudentAndTestResultSubScoreMultiple(int virtualTestId, int classId, string studentIds, DateTime entryResultDate)
        {
            var testResultSubScores = _dataLockerRepository.GetStudentAndTestResultSubScoreMultiple(virtualTestId, classId, studentIds, entryResultDate).ToList();
            var testResultSubScoreIDs = testResultSubScores.Where(x => x.TestResultScoreSubID.HasValue)
                                          .Select(x => x.TestResultScoreSubID.Value).ToList();

            if (testResultSubScoreIDs.Count > 0)
            {
                var testResultSubScoreNotes = _testResultSubScoreNoteRepository.Select()
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultSubScoreID);
                    }, testResultSubScoreIDs)
                    .ToList();

                var testResultScoreArtifacts = _testResultScoreUploadFileRepository.Select()
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultSubScoreID.Value);
                    }, testResultSubScoreIDs)
                    .ToList();

                foreach (var testResultSuScore in testResultSubScores)
                {
                    testResultSuScore.Artifacts = testResultScoreArtifacts.Where(x => x.TestResultSubScoreID == testResultSuScore.TestResultScoreSubID)
                        .Select(x => new TestResultScoreArtifact
                        {
                            Name = x.FileName,
                            IsLink = x.IsUrl,
                            Url = x.IsUrl == true ? x.FileName : "",
                            UploadDate = x.UploadDate,
                            TagValue = x.Tag,
                            DocumentGuid = x.DocumentGuid,
                            CreatedBy = x.CreatedBy
                        })
                        .ToList();

                    testResultSuScore.Notes = testResultSubScoreNotes.Where(x => x.TestResultSubScoreID == testResultSuScore.TestResultScoreSubID).ToList();
                }
            }

            var otherResultStudentIds = GetStudentHasResultInOtherClass(virtualTestId, classId);
            testResultSubScores.ForEach(x => x.HasOtherScore = otherResultStudentIds.Contains(x.StudentID));

            return testResultSubScores;
        }
        public VirtualTestCustomScore GetVirtualTestCustomScoreByVirtualTestCustomScoreID(int virtualTestCustomScoreID)
        {
            return _virtualTestCustomScoreRepository.Select().Where(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreID).FirstOrDefault();
        }

        public VirtualTestCustomScore GetVirtualTestCustomScore(int virtualTestId)
        {
            var virtualTestVirtualTestCustomScore = _virtualTestVirtualTestCustomScore.Select().Where(x => x.VirtualTestId == virtualTestId).FirstOrDefault();
            if (virtualTestVirtualTestCustomScore != null)
            {
                return _virtualTestCustomScoreRepository.Select().Where(x => x.VirtualTestCustomScoreId == virtualTestVirtualTestCustomScore.VirtualTestCustomScoreId).FirstOrDefault();
            }

            return null;
        }

        public List<VirtualTestCustomMetaData> GetVirtualTestCustomMetaDataByVirtualTestCustomScoreId(int virtualTestCustomScoreId)
        {
            var results = _virtualTestCustomMetaDataRepository.Select().Where(x => x.VirtualTestCustomScoreID == virtualTestCustomScoreId).ToList();
            MapMetaValues(virtualTestCustomScoreId, results);
            return results;
        }

        public List<VirtualTestCustomMetaData> GetVirtualTestCustomMetaDataByVirtualTestCustomSubScoreID(int virtualTestCustomSubScoreId)
        {
            var results =  _virtualTestCustomMetaDataRepository.Select().Where(x => x.VirtualTestCustomSubScoreID == virtualTestCustomSubScoreId).ToList();
            if(results.Count > 0 )
            {
                var templateId = results.FirstOrDefault().VirtualTestCustomScoreID;
                MapMetaValues(templateId, results);
            }
            return results;
        }
        public void MapMetaValues(int templateId, List<VirtualTestCustomMetaData> results)
        {
            var customMetaSettings = _virtualTestCustomMetaDataRepository.GetCustomMetaSettings(templateId);
            foreach (var result in results)
            {
                var minMaxValue = new CustomMetaSettingMinMaxDto();
                if (result.VirtualTestCustomSubScoreID != null)
                {
                    minMaxValue = customMetaSettings.FirstOrDefault(i => i.VirtualTestCustomSubScoreId == result.VirtualTestCustomSubScoreID
                    && i.ScoreType == result.ScoreType);
                }
                else
                {
                    minMaxValue = customMetaSettings.FirstOrDefault(i => (i.VirtualTestCustomSubScoreId == null || i.VirtualTestCustomSubScoreId == 0)
                    && i.ScoreType == result.ScoreType);
                }
                if (minMaxValue != null)
                {
                    var decimalScale = (int)JObject.Parse(result.MetaData)["DecimalScale"];
                    result.MetaData = result.MetaData
                        .JsonModify("$.MinValue", CommonUtils.RoundNumber(minMaxValue.MinValue, decimalScale))
                        .JsonModify("$.MaxValue", CommonUtils.RoundNumber(minMaxValue.MaxValue, decimalScale));

                }
            }
        }
        public List<VirtualTestCustomSubScore> GetVirtualTestCustomSubScores(int virtualTestCustomScoreId)
        {
            return _virtualTestCustomSubScoreRepository.Select().Where(x => x.VirtualTestCustomScoreId == virtualTestCustomScoreId).ToList();
        }
        public void SaveEntryResults(string testresultsXML, string testresultscoresXML, string testresultsubscoresXML, string testresultscoreNotesXML, string testresultSubScoreNotesXML, string testresultscoreUploadFileXML, string testresultsubScoreUploadFileXML, string testresultidDelete)
        {
            _dataLockerRepository.SaveEntryResults(testresultsXML, testresultscoresXML, testresultsubscoresXML, testresultscoreNotesXML, testresultSubScoreNotesXML, testresultscoreUploadFileXML, testresultsubScoreUploadFileXML, testresultidDelete);
        }

        public void SaveEntryResultsMultiple(string testresultsXML, string testresultscoresXML, string testresultsubscoresXML, string testresultscoreNotesXML, string testresultSubScoreNotesXML, string testresultscoreUploadFileXML, string testresultsubScoreUploadFileXML, string testresultidDelete)
        {
            _dataLockerRepository.SaveEntryResultsMultiple(testresultsXML, testresultscoresXML, testresultsubscoresXML, testresultscoreNotesXML, testresultSubScoreNotesXML, testresultscoreUploadFileXML, testresultsubScoreUploadFileXML, testresultidDelete);
        }

        public DTLAutoSaveResultData GetAutoSaveData(int virtualTestId, int classId, int userId)
        {
            return
                _autoSaveRepository.Select()
                    .Where(x => x.ClassId == classId && x.VirtualTestId == virtualTestId && x.UserId == userId)
                    .FirstOrDefault();
        }
        public DTLAutoSaveResultData GetAutoSaveDataBaseOnDate(int virtualTestId, int classId, int userId, DateTime date)
        {
            return
                _autoSaveRepository.Select()
                    .Where(x => x.ClassId == classId && x.VirtualTestId == virtualTestId && x.UserId == userId && x.ResultDate == date)
                    .FirstOrDefault();
        }
        public void AutoSaveResult(DTLAutoSaveResultData data)
        {
            _autoSaveRepository.Save(data);
        }

        public void AutoSaveResultBaseDate(DTLAutoSaveResultData data)
        {
            _dataLockerRepository.AutoSaveResultDataBaseDate(data);
        }

        public void DeleteAllAutoSaveData(int virtualTestId, int classId)
        {
            _dataLockerRepository.DeleteAllAutoSaveData(virtualTestId, classId);
        }

        public void DeleteAllAutoSaveDataBaseDate(int virtualTestId, int classId, DateTime dateSave)
        {
            _dataLockerRepository.DeleteAllAutoSaveDataBaseDate(virtualTestId, classId, dateSave);
        }

        public void DeleteAutoSaveData(int virtualTestId, int classId, int userId)
        {
            var data = new DTLAutoSaveResultData() { VirtualTestId = virtualTestId, ClassId = classId, UserId = userId };
            _autoSaveRepository.Delete(data);
        }
        public void DeleteAutoSaveMultiDate(int virtualTestId, int classId, int userId, DateTime? resultDate)
        {
            var data = new DTLAutoSaveResultData() { VirtualTestId = virtualTestId, ClassId = classId, UserId = userId, ResultDate = resultDate };
            _autoSaveRepository.Delete(data);
        }
        public List<DTLFormModel> GetFormsByBankID(int bankId, bool? isFromMultiDate, bool usingMultiDate)
        {
            return _dataLockerRepository.GetFormsByBankID(bankId, isFromMultiDate ?? false, usingMultiDate);
        }

        public IEnumerable<Guid?> GetDocumentGuidsByTestResultScore(IEnumerable<int?> testResultScoreIds)
        {
            return _testResultScoreUploadFileRepository.Select()
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultScoreID.Value);
                    }, testResultScoreIds)
                    .ToList().Where(p => p.DocumentGuid.HasValue && p.DocumentGuid != Guid.Empty)
                    .Select(p => p.DocumentGuid);
        }

        public IEnumerable<Guid?> GetDocumentGuidsByTestResultSubScore(IEnumerable<int?> testResultSubScoreIds)
        {
            return _testResultScoreUploadFileRepository.Select()
                    .FilterOnLargeSet(entry => entry, (subset) =>
                    {
                        return entry => subset.Contains(entry.TestResultSubScoreID.Value);
                    }, testResultSubScoreIds)
                    .ToList().Where(p => p.DocumentGuid.HasValue && p.DocumentGuid != Guid.Empty)
                    .Select(p => p.DocumentGuid);
        }
        #region Methods

        public List<ResultEntryScoreModel> GetVirtualTestCustomScoreInfo(VirtualTestCustomScore virtualTestCustomScore, List<VirtualTestCustomMetaData> virtualTestCustomMetaDatas, int districtID)
        {
            var scoreInfos = new List<ResultEntryScoreModel>();
            var scoreTypes = new List<(string Name, string Label, Func<VirtualTestCustomScore, bool?> UseGetter, string MetaDataKey, Func<VirtualTestCustomScore, string> LabelGetter)>()
            {
                ("Raw", "Raw", v => v.UseRaw, "Raw", null),
                ("Scaled", "Scaled", v => v.UseScaled, "Scaled", null),
                ("Percent", "Percent", v => v.UsePercent, "Percent", null),
                ("Percentile", "Percentile", v => v.UsePercentile, "Percentile", null),
                ("CustomN_1", null, v => v.UseCustomN1.GetValueOrDefault(), "CustomN_1", v => v.CustomN1Label),
                ("CustomN_2", null, v => v.UseCustomN2.GetValueOrDefault(), "CustomN_2", v => v.CustomN2Label),
                ("CustomN_3", null, v => v.UseCustomN3.GetValueOrDefault(), "CustomN_3", v => v.CustomN3Label),
                ("CustomN_4", null, v => v.UseCustomN4.GetValueOrDefault(), "CustomN_4", v => v.CustomN4Label),
                ("CustomA_1", null, v => v.UseCustomA1.GetValueOrDefault(), "CustomA_1", v => v.CustomA1Label),
                ("CustomA_2", null, v => v.UseCustomA2.GetValueOrDefault(), "CustomA_2", v => v.CustomA2Label),
                ("CustomA_3", null, v => v.UseCustomA3.GetValueOrDefault(), "CustomA_3", v => v.CustomA3Label),
                ("CustomA_4", null, v => v.UseCustomA4.GetValueOrDefault(), "CustomA_4", v => v.CustomA4Label),
                ("Artifact", "Artifact", v => v.UseArtifact, "Artifact", null),
                ("Note", null, v => v.UseNote, VirtualTestCustomMetaData.NOTE_COMMENT, null)
            };

            int noteCount = 0;
            foreach (var scoreType in scoreTypes)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, scoreType.MetaDataKey);
                if (scoreType.UseGetter(virtualTestCustomScore).HasValue && scoreType.UseGetter(virtualTestCustomScore).Value && metaData != null)
                {                    
                    if (scoreType.Name == "Artifact")
                    {
                        var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<EntryResultArtifactFileTypeGroupViewModel>>
                            (_districtDecodeService.GetAssessmentArtifactFileTypeGroups(districtID));
                        metaData.EntryResultArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();
                        scoreInfos.Add(new ResultEntryScoreModel
                        {
                            ScoreName = "Artifact",
                            ScoreLable = "Artifact",
                            MetaData = metaData,
                            Order = metaData.Order.GetValueOrDefault()
                        });                        
                    }
                    else if (scoreType.Name == "Note" && metaData.ListNoteComment != null)
                    {
                        foreach (var note in metaData.ListNoteComment)
                        {
                            noteCount++;
                            var meta = new VirtualTestCustomMetaModel
                            {
                                DefaultValue = note.DefaultValue,
                                Description = note.Description,
                                NoteType = note.NoteType
                            };
                            scoreInfos.Add(new ResultEntryScoreModel
                            {
                                ScoreName = $"note_{noteCount}",
                                ScoreLable = note.NoteName,
                                MetaData = meta,
                                Order = note.Order.GetValueOrDefault()
                            });
                        }
                    }
                    else
                    {
                        var scoreLabel = scoreType.LabelGetter != null ? scoreType.LabelGetter(virtualTestCustomScore) : scoreType.Label;
                        scoreInfos.Add(new ResultEntryScoreModel
                        {
                            ScoreName = scoreType.Name,
                            ScoreLable = scoreLabel,
                            MetaData = metaData,
                            Order = metaData.Order.GetValueOrDefault()
                        });
                    }
                }
            }
            scoreInfos = scoreInfos.OrderBy(x => x.Order).ToList();
            return scoreInfos;
        }

        public List<ResultEntryScoreModel> GetVirtualTestCustomScoreInfo(VirtualTestCustomSubScore virtualTestCustomSubScore, List<VirtualTestCustomMetaData> virtualTestCustomMetaDatas, int districtID)
        {
            var scoreInfos = new List<ResultEntryScoreModel>();
            var scoreTypes = new List<(string Name, string Label, Func<VirtualTestCustomSubScore, bool?> UseGetter, string MetaDataKey, Func<VirtualTestCustomSubScore, string> LabelGetter)>()
            {
                ("Raw", "Raw", v => v.UseRaw, "Raw", null),
                ("Scaled", "Scaled", v => v.UseScaled, "Scaled", null),
                ("Percent", "Percent", v => v.UsePercent, "Percent", null),
                ("Percentile", "Percentile", v => v.UsePercentile, "Percentile", null),
                ("CustomN_1", null, v => v.UseCustomN1.GetValueOrDefault(), "CustomN_1", v => v.CustomN1Label),
                ("CustomN_2", null, v => v.UseCustomN2.GetValueOrDefault(), "CustomN_2", v => v.CustomN2Label),
                ("CustomN_3", null, v => v.UseCustomN3.GetValueOrDefault(), "CustomN_3", v => v.CustomN3Label),
                ("CustomN_4", null, v => v.UseCustomN4.GetValueOrDefault(), "CustomN_4", v => v.CustomN4Label),
                ("CustomA_1", null, v => v.UseCustomA1.GetValueOrDefault(), "CustomA_1", v => v.CustomA1Label),
                ("CustomA_2", null, v => v.UseCustomA2.GetValueOrDefault(), "CustomA_2", v => v.CustomA2Label),
                ("CustomA_3", null, v => v.UseCustomA3.GetValueOrDefault(), "CustomA_3", v => v.CustomA3Label),
                ("CustomA_4", null, v => v.UseCustomA4.GetValueOrDefault(), "CustomA_4", v => v.CustomA4Label),
                ("Artifact", "Artifact", v => v.UseArtifact, "Artifact", null),
                ("Note", null, v => v.UseNote, VirtualTestCustomMetaData.NOTE_COMMENT, null)
            };

            int noteCount = 0;
            foreach (var scoreType in scoreTypes)
            {
                var metaData = GetMetaData(virtualTestCustomMetaDatas, scoreType.MetaDataKey);
                if (scoreType.UseGetter(virtualTestCustomSubScore).HasValue && scoreType.UseGetter(virtualTestCustomSubScore).Value && metaData != null)
                {
                    if (scoreType.Name == "Artifact")
                    {
                        var assessmentArtifactFileTypeGroups = Mapper.Map<IEnumerable<EntryResultArtifactFileTypeGroupViewModel>>
                            (_districtDecodeService.GetAssessmentArtifactFileTypeGroups(districtID));
                        metaData.EntryResultArtifactFileTypeGroupViewModel = assessmentArtifactFileTypeGroups.ToList();
                        scoreInfos.Add(new ResultEntryScoreModel
                        {
                            ScoreName = "Artifact",
                            ScoreLable = "Artifact",
                            MetaData = metaData,
                            Order = metaData.Order.GetValueOrDefault()
                        });
                    }
                    else if (scoreType.Name == "Note" && metaData.ListNoteComment != null)
                    {
                        foreach (var note in metaData.ListNoteComment)
                        {
                            noteCount++;
                            var meta = new VirtualTestCustomMetaModel
                            {
                                DefaultValue = note.DefaultValue,
                                Description = note.Description,
                                NoteType = note.NoteType
                            };
                            scoreInfos.Add(new ResultEntryScoreModel
                            {
                                ScoreName = $"note_{noteCount}",
                                ScoreLable = note.NoteName,
                                MetaData = meta,
                                Order = note.Order.GetValueOrDefault()
                            });
                        }
                    }
                    else
                    {
                        var scoreLabel = scoreType.LabelGetter != null ? scoreType.LabelGetter(virtualTestCustomSubScore) : scoreType.Label;
                        scoreInfos.Add(new ResultEntryScoreModel
                        {
                            ScoreName = scoreType.Name,
                            ScoreLable = scoreLabel,
                            MetaData = metaData,
                            Order = metaData.Order.GetValueOrDefault()
                        });
                    }
                }
            }
            scoreInfos = scoreInfos.OrderBy(x => x.Order).ToList();
            return scoreInfos;
        }

        private VirtualTestCustomMetaModel GetMetaData(List<VirtualTestCustomMetaData> virtualTestCustomMetaDatas, string scoreType)
        {
            var metaData = virtualTestCustomMetaDatas.Where(x => x.ScoreType == scoreType).FirstOrDefault();
            return metaData != null ? metaData.ParseMetaToObject() : null;
        }

        private IEnumerable<int> GetStudentHasResultInOtherClass(int virtualTestId, int classId, string strStudentIds = null)
        {
            var studentIds = Enumerable.Empty<int>();

            if (!string.IsNullOrEmpty(strStudentIds))
            {
                studentIds = strStudentIds.ToIntArray();
            }
            else
            {
                studentIds = _classStudentRepository.Select().Where(x => x.ClassId == classId).Select(x => x.StudentId).ToArray();
            }

            if (!studentIds.Any())
            {
                return Enumerable.Empty<int>();
            }

            return _testResultRepository.Select()
                .Where(x => x.VirtualTestId == virtualTestId && x.ClassId != classId && studentIds.Contains(x.StudentId))
                .Select(x => x.StudentId)
                .ToArray(); ;
        }

        public void DeleteDocumentByTestResult(IEnumerable<int?> testResultScoreIds, IEnumerable<int?> testResultSubScoreIds, List<Guid?> updatedDocumentGuids)
        {
            var currentDocumentGuids = new List<Guid?>();
            if (testResultScoreIds != null && testResultScoreIds.Any())
            {
                currentDocumentGuids.AddRange(GetDocumentGuidsByTestResultScore(testResultScoreIds));
            }

            if (testResultSubScoreIds != null && testResultSubScoreIds.Any())
            {
                currentDocumentGuids.AddRange(GetDocumentGuidsByTestResultSubScore(testResultSubScoreIds));
            }

            var deletedDocumentGuids = currentDocumentGuids.Except(updatedDocumentGuids);
            if (deletedDocumentGuids.Any())
            {
                _documentManagementService.DeleteDocuments(deletedDocumentGuids.ToList());
            }
        }

        #endregion
    }
}
