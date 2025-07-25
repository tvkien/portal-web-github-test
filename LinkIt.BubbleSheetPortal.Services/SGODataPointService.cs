using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGODataPointService
    {
        private readonly ISGODataPointRepository sgoDataPointRepository;
        private readonly ISGORepository _repository;
        private readonly IRepository<SGODataPointBand> sgoDataPointBandRepository;
        private readonly IRepository<SGOStudentDataPoint> sgoStudentDataPointRepository;
        private readonly IReadOnlyRepository<TestResultScore> testResultScoreRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IBankRepository _bankRepository;
        private readonly ISGORepository _sgoRepository;
        private readonly IReadOnlyRepository<District> _districtRepository;
        private readonly IReadOnlyRepository<Configuration> _configurationRepository;
        private readonly ISGOAuditTrailRepository _sgoAuditTrailRepository;
        private readonly IRepository<SGODataPointClusterScore> _sgoDataPointClusterScoreRepository;
        private readonly IReadOnlyRepository<AchievementLevelSetting> _achievementLevelSettingRepository;
        private readonly IReadOnlyRepository<Test> _testRepository;

        public SGODataPointService(ISGODataPointRepository sgoDataPointRepository, ISGORepository repository,
            IRepository<SGODataPointBand> sgoDataPointBandRepository,
            IRepository<SGOStudentDataPoint> sgoStudentDataPointRepository,
            IReadOnlyRepository<TestResultScore> testResultScoreRepository,
            ISubjectRepository subjectRepository,
            IGradeRepository gradeRepository,
            IBankRepository bankRepository,
            ISGORepository sgoRepository,
            IReadOnlyRepository<District> districtRepository,
            IReadOnlyRepository<Configuration> configurationRepository,
            ISGOAuditTrailRepository sgoAuditTrailRepository, 
            IRepository<SGODataPointClusterScore> sgoDataPointClusterScoreRepository,
            IReadOnlyRepository<AchievementLevelSetting> achievementLevelSettingRepository,
            IReadOnlyRepository<Test> testRepository)
        {
            this.sgoDataPointRepository = sgoDataPointRepository;
            _repository = repository;
            this.sgoDataPointBandRepository = sgoDataPointBandRepository;
            this.sgoStudentDataPointRepository = sgoStudentDataPointRepository;
            this.testResultScoreRepository = testResultScoreRepository;
            _subjectRepository = subjectRepository;
            _gradeRepository = gradeRepository;
            _bankRepository = bankRepository;
            _sgoRepository = sgoRepository;
            _districtRepository = districtRepository;
            _configurationRepository = configurationRepository;
            _sgoAuditTrailRepository = sgoAuditTrailRepository;
            _sgoDataPointClusterScoreRepository = sgoDataPointClusterScoreRepository;
            _achievementLevelSettingRepository = achievementLevelSettingRepository;
            _testRepository = testRepository;
        }

        public IEnumerable<SGODataPoint> GetDataPointBySGOID(int SGOID)
        {
            var dataPoints = sgoDataPointRepository.Select().Where(x => x.SGOID == SGOID && !x.IsTemporary).ToList();
            var testIds = dataPoints.Select(x => x.VirtualTestID).ToArray();
            return from point in dataPoints
                    join test in _testRepository.Select().Where(x => testIds.Contains(x.Id)).ToList() on point.VirtualTestID equals test.Id into Tests
                    from test in Tests.DefaultIfEmpty()
                    select new SGODataPoint
                    {
                        SGODataPointID = point.SGODataPointID,
                        Name = point.Name,
                        SubjectName = point.SubjectName,
                        GradeID = point.GradeID,
                        VirtualTestID = point.VirtualTestID,
                        AttachScoreUrl = point.AttachScoreUrl,
                        SGOID = point.SGOID,
                        Weight = point.Weight,
                        TotalPoints = point.TotalPoints,
                        Type = point.Type,
                        AchievementLevelSettingID = point.AchievementLevelSettingID,
                        ResultDate = point.ResultDate,
                        RationaleGuidance = point.RationaleGuidance,
                        ScoreType = point.ScoreType,
                        ImprovementBasedDataPoint = point.ImprovementBasedDataPoint,
                        DataSetCategoryID = test != null ? test.DataSetCategoryID.GetValueOrDefault() : 0,
                        IsTemporary = point.IsTemporary,
                    };
        }


        public IEnumerable<SGODataPoint> GetPreAssessmentDataPointBySGOID(int SGOID)
        {

            var dataPoints = sgoDataPointRepository.Select()
                .Where(
                    x =>
                        x.SGOID == SGOID 
                        && x.Type != (int) SGODataPointTypeEnum.PostAssessment 
                        && x.Type != (int) SGODataPointTypeEnum.PostAssessmentToBeCreated
                        && x.Type != (int) SGODataPointTypeEnum.PostAssessmentCustom                            
                        && (x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                            && (x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1 
                            || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2
                            || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA3
                            || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA4)) == false
                        ).ToList();
            var testIds = dataPoints.Select(x => x.VirtualTestID).ToArray();
            return dataPoints.
                Join(_testRepository.Select().Where(x => testIds.Contains(x.Id)).ToList(),
                        point => point.VirtualTestID, test => test.Id,
                        (point, test) => new SGODataPoint
                        {
                            SGODataPointID = point.SGODataPointID,
                            Name = point.Name,
                            SubjectName = point.SubjectName,
                            GradeID = point.GradeID,
                            VirtualTestID = point.VirtualTestID,
                            AttachScoreUrl = point.AttachScoreUrl,
                            SGOID = point.SGOID,
                            Weight = point.Weight,
                            TotalPoints = point.TotalPoints,
                            Type = point.Type,
                            AchievementLevelSettingID = point.AchievementLevelSettingID,
                            ResultDate = point.ResultDate,
                            RationaleGuidance = point.RationaleGuidance,
                            ScoreType = point.ScoreType,
                            ImprovementBasedDataPoint = point.ImprovementBasedDataPoint,
                            DataSetCategoryID = test.DataSetCategoryID.GetValueOrDefault()
                        });
        }

        public SGODataPoint GetById(int sgoDataPointId)
        {
            var dataPoint = sgoDataPointRepository.GetByID(sgoDataPointId);
            if (dataPoint != null) dataPoint.IsCustomCutScore = CheckIsCustomCutScore(dataPoint);
            return dataPoint;
        }

        public void Save(SGODataPoint sgoDataPoint)
        {
            var actionDetailFormat = "<dataPoints><datapointid>{0}</datapointid></dataPoints>";

            var entity =
                sgoDataPointRepository.Select()
                    .FirstOrDefault(o => o.SGODataPointID == sgoDataPoint.SGODataPointID);

            sgoDataPointRepository.Save(sgoDataPoint);

            var drafStatus = (int) SGOStatusType.Draft;
            var isDrafStatus = _repository.Select().Any(o => o.SGOID == sgoDataPoint.SGOID && o.SGOStatusID == drafStatus);
            if (!isDrafStatus && (entity == null || entity.AttachScoreUrl != sgoDataPoint.AttachScoreUrl))
            {
                var auditTrail = new SGOAuditTrailData
                {
                    SGOActionTypeID = (int)SGOActionTypeEnum.AddAttachmentToDataPoint,
                    SGOID = sgoDataPoint.SGOID,
                    ActionDetail = string.Format(actionDetailFormat, sgoDataPoint.SGODataPointID),
                    ChangedOn = DateTime.UtcNow,
                    ChagedByUserID = sgoDataPoint.UserID
                };
                _sgoAuditTrailRepository.Save(auditTrail);
            }
        }

        public void Delete(SGODataPoint sgoDataPoint)
        {
            var sgoDataPointBands =
                sgoDataPointBandRepository.Select().Where(x => x.SGODataPointID == sgoDataPoint.SGODataPointID);
            foreach (var sgoDataPointBand in sgoDataPointBands)
            {
                sgoDataPointBandRepository.Delete(sgoDataPointBand);
            }

            sgoDataPointRepository.Delete(sgoDataPoint);
        }

        public List<SGOStudentScoreInDataPoint> GetScoreInDataPoint(int dataPointId, int? virtualTestCustomSubScoreId = 0)
        {
            var dataPoint = GetById(dataPointId);
            if (dataPoint != null)
            {
                var dataPointType =
                    (SGODataPointTypeEnum) Enum.Parse(typeof (SGODataPointTypeEnum), dataPoint.Type.ToString());
                switch (dataPointType)
                {
                    case SGODataPointTypeEnum.PostAssessment:
                    case SGODataPointTypeEnum.PostAssessmentCustom:
                        break;
                    case SGODataPointTypeEnum.PreAssessmentCustom:
                        return _repository.GetStudentScorePreCustomAssessmentLinkIt(dataPointId, virtualTestCustomSubScoreId);
                    case SGODataPointTypeEnum.PreAssessment:
                        return _repository.GetStudentScorePreAssessmentLinkIt(dataPointId);
                    case SGODataPointTypeEnum.PreAssessmentHistorical:
                        return _repository.GetStudentScorePreAssessmentLegacy(dataPointId);
                    case SGODataPointTypeEnum.PreAssessmentExternal:
                        return _repository.GetStudentScorePreAssessmentExternal(dataPointId);
                    case SGODataPointTypeEnum.PostAssessmentToBeCreated:
                        break;
                }
            }
            return new List<SGOStudentScoreInDataPoint>();
        }

        public void InitDefaultBandForDataPoint(int dataPointId)
        {
            var dataPoint = GetById(dataPointId);
            if (dataPoint != null)
            {
                var dataPointType =
                    (SGODataPointTypeEnum) Enum.Parse(typeof (SGODataPointTypeEnum), dataPoint.Type.ToString());
                switch (dataPointType)
                {
                    case SGODataPointTypeEnum.PostAssessment:
                        break;
                    case SGODataPointTypeEnum.PreAssessment:
                        InitDefaultBandForLinkItAndExternalTest(dataPoint);
                        break;
                    case SGODataPointTypeEnum.PreAssessmentHistorical:
                        InitDefaultBandForLegacyTest(dataPoint);
                        break;
                    case SGODataPointTypeEnum.PreAssessmentExternal:
                        InitDefaultBandForLinkItAndExternalTest(dataPoint);
                        break;
                    case SGODataPointTypeEnum.PostAssessmentToBeCreated:
                        break;
                }
            }
        }

        public void InitDefaultBandForLinkItAndExternalTest(SGODataPoint dataPoint)
        {
            var hasDataPointBand =
                sgoDataPointBandRepository.Select().Any(x => x.SGODataPointID == dataPoint.SGODataPointID);
            if (hasDataPointBand) return;
            List<SGODataPointBand> lst = CreateDefaultBandForLinkItAndExternalTest(dataPoint);
            foreach (var sgoDataPointBand in lst)
            {
                sgoDataPointBandRepository.Save(sgoDataPointBand);
            }
        }

        public void InitDefaultBandForLegacyTest(SGODataPoint dataPoint)
        {
            var hasDataPointBand =
               sgoDataPointBandRepository.Select().Any(x => x.SGODataPointID == dataPoint.SGODataPointID);
            if (hasDataPointBand) return;
            //TODO: this method is not tested yet
            var listDefaultBand = CreateDefaultBandForLegacyTest(dataPoint);
            if (listDefaultBand == null || !listDefaultBand.Any()) return;

            foreach (var sgoDataPointBand in listDefaultBand)
            {
                if (sgoDataPointBand != null)
                    sgoDataPointBandRepository.Save(sgoDataPointBand);
            }
        }

        public List<SGODataPointBand> CreateDefaultBandForLegacyTest(SGODataPoint dataPoint)
        {
            var scoreType = GetDataPointScoreType(dataPoint);

            var scoreAchievementData = _repository.GetScoreToCreateDefaultBandForHistoricalTest(dataPoint.SGODataPointID);
            var listDefaultBand = new List<SGODataPointBand>();

            if (scoreAchievementData.Any())
            {
                var achievementLevels =
                    scoreAchievementData.Select(x => x.AchievementLevel).Distinct().OrderBy(x => x).ToList();
                var achievementValue = scoreAchievementData.First()
                    .AchievementValueString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var achievementLabel = scoreAchievementData.First()
                    .AchievementLabelString.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries).ToList();
                var testResultScores = scoreAchievementData.Select(x => new
                {
                    AchievementLevel = x.AchievementLevel,
                    Score = GetScoreAchievementByType((SGOScoreTypeEnum)scoreType, x)
                }).ToList();
                foreach (var achievementLevel in achievementLevels)
                {
                    var achievementLevelIndex = achievementValue.IndexOf(achievementLevel.ToString());
                    var bandName = string.Empty;
                    if (achievementLevelIndex >= 0 && achievementLevelIndex < achievementLabel.Count)
                    {
                        bandName = achievementLabel[achievementLevelIndex];
                    }
                    var band = new SGODataPointBand();
                    band.Name = bandName;
                    band.SGODataPointID = dataPoint.SGODataPointID;
                    band.LowValue = (double) testResultScores.Where(x => x.AchievementLevel == achievementLevel)
                        .Min(x => x.Score);
                    band.HighValue = (double) testResultScores.Where(x => x.AchievementLevel == achievementLevel)
                        .Max(x => x.Score);
                    band.AchievementLevel = achievementLevel;
                    listDefaultBand.Add(band);
                }
            }

            var achievementLevelSettingLabels = GetAchievementLevelSettingLabelsOfSgoDataPoint(dataPoint.SGODataPointID);
            var orderDefaultBand =
                achievementLevelSettingLabels.Select(label => listDefaultBand.FirstOrDefault(x => x.Name == label))
                    .Where(item => item != null)
                    .ToList();

            return orderDefaultBand.ToList();
        }

        private decimal GetScoreAchievementByType(SGOScoreTypeEnum scoreType, SGOScoreAchievmentData scoreAchievmentData)
        {
            switch (scoreType)
            {
                case SGOScoreTypeEnum.ScoreRaw: return scoreAchievmentData.ScoreRaw;
                case SGOScoreTypeEnum.ScoreScaled: return scoreAchievmentData.ScoreScaled;
                case SGOScoreTypeEnum.ScorePercent: return scoreAchievmentData.ScorePercent;
                case SGOScoreTypeEnum.ScoreIndex: return scoreAchievmentData.ScoreIndex;
                case SGOScoreTypeEnum.ScoreLexile: return scoreAchievmentData.ScoreLexile;
                case SGOScoreTypeEnum.ScoreCustomN1: return scoreAchievmentData.ScoreCustomN_1;
                case SGOScoreTypeEnum.ScoreCustomN2: return scoreAchievmentData.ScoreCustomN_2;
                case SGOScoreTypeEnum.ScoreCustomN3: return scoreAchievmentData.ScoreCustomN_3;
                case SGOScoreTypeEnum.ScoreCustomN4: return scoreAchievmentData.ScoreCustomN_4;
                default: return scoreAchievmentData.ScorePercentage;
            }
        }

        private List<string> GetAchievementLevelSettingLabelsOfSgoDataPoint(int sgoDataPointId)
        {
            var sgoDataPoint = sgoDataPointRepository.Select().FirstOrDefault(x => x.SGODataPointID == sgoDataPointId);
            if (sgoDataPoint != null)
            {
                var test = _testRepository.Select().FirstOrDefault(x => x.Id == sgoDataPoint.VirtualTestID);
                var achievementLevelSetting =
                    _achievementLevelSettingRepository.Select()
                        .FirstOrDefault(x => x.AchievementLevelSettingID == test.AchievementLevelSettingID.GetValueOrDefault());
                if (achievementLevelSetting != null)
                {
                    return achievementLevelSetting.LabelString.Split(';').ToList();
                }
            }

            return new List<string>();
        }

        public List<SGODataPointBand> GetDataPointBandByDataPointID(int dataPointId)
        {
            return sgoDataPointBandRepository.Select().Where(x => x.SGODataPointID == dataPointId).ToList();
        }

        public void AssignStudentToDataPointBand(int sgoDataPointBandID, string studentIDs)
        {
            _repository.AssignStudentToDataPointBand(sgoDataPointBandID, studentIDs);
        }

        public void UpdateWeightById(int dataPointId, int weight)
        {
            var obj = sgoDataPointRepository.Select().FirstOrDefault(o => o.SGODataPointID == dataPointId);
            obj.Weight = weight;
            sgoDataPointRepository.Save(obj);
        }

        public void UpdateDataPointBand(SGODataPointBand obj)
        {
            sgoDataPointBandRepository.Save(obj);
        }

        public void AddDataPointBand(List<SGODataPointBand> lst)
        {
            foreach (var sgoDataPointBand in lst)
            {
                sgoDataPointBandRepository.Save(sgoDataPointBand);
            }
        }

        public void DeleteDataPointBand(List<SGODataPointBand> lstDataPointBand)
        {
            //foreach (var sgoDataPointBand in lstDataPointBand)
            //{
            //    sgoDataPointBandRepository.Delete(sgoDataPointBand);
            //}
            var dataPointBandIDs = string.Join(",", lstDataPointBand.Select(x => x.SGODataPointBandID).ToList());
            _repository.DeleteDataPointBand(dataPointBandIDs);
        }

        public bool CheckIsCustomCutScore(SGODataPoint dataPoint)
        {
            var lstDefaultBands = new List<SGODataPointBand>();
            var dataPointType =
                (SGODataPointTypeEnum) Enum.Parse(typeof (SGODataPointTypeEnum), dataPoint.Type.ToString());
            switch (dataPointType)
            {
                case SGODataPointTypeEnum.PostAssessment:
                    return false;
                case SGODataPointTypeEnum.PreAssessment:
                    lstDefaultBands = CreateDefaultBandForLinkItAndExternalTest(dataPoint).OrderBy(o => o.LowValue).ToList();
                    break;
                case SGODataPointTypeEnum.PreAssessmentHistorical:
                    lstDefaultBands = CreateDefaultBandForLegacyTest(dataPoint).Where(o => o != null).OrderBy(o => o.LowValue).ToList();
                    break;

                case SGODataPointTypeEnum.PreAssessmentExternal:
                    lstDefaultBands = CreateDefaultBandForLinkItAndExternalTest(dataPoint).OrderBy(o => o.LowValue).ToList();
                    break;
                case SGODataPointTypeEnum.PostAssessmentToBeCreated:
                    return false;
            }
            var lstDataPointBands = GetDataPointBandByDataPointID(dataPoint.SGODataPointID).OrderBy(o => o.LowValue).ToList();
            if (lstDataPointBands.Count != lstDefaultBands.Count)
                return true;
            for(int i = 0 ;i<lstDefaultBands.Count ; i++)
            {
                if (lstDataPointBands[i].LowValue != lstDefaultBands[i].LowValue
                    || lstDataPointBands[i].LowValue != lstDefaultBands[i].LowValue )
                {
                    return true;
                }
            }
            return false;
        }

        internal List<SGODataPointBand> CreateDefaultBandForLinkItAndExternalTest(SGODataPoint dataPoint)
        {
            var lst = new List<SGODataPointBand>();
            //create default band
            lst.Add(new SGODataPointBand()
            {
                Name = "Below 40%",
                SGODataPointID = dataPoint.SGODataPointID,
                LowValue = 0,
                HighValue = 39.99,
            });

            lst.Add(new SGODataPointBand()
            {
                Name = "40 - 60%",
                SGODataPointID = dataPoint.SGODataPointID,
                LowValue = 40,
                HighValue = 59.99,
            });


            lst.Add(new SGODataPointBand()
            {
                Name = "60 - 80%",
                SGODataPointID = dataPoint.SGODataPointID,
                LowValue = 60,
                HighValue = 79.99,
            });
            lst.Add(new SGODataPointBand()
            {
                Name = "Above 80%",
                SGODataPointID = dataPoint.SGODataPointID,
                LowValue = 80,
                HighValue = 100,
            });
            return lst;
        }

        public bool CanSaveSGODataPoint(SGODataPointSaveDTO sgoDataPoint)
        {
            if (sgoDataPoint == null || sgoDataPoint.SGODataPoint == null) return false;
            var types = GetPreOrPostSGODataPointTypes(sgoDataPoint.SGODataPoint.Type);
            var totalDataPointWithSameType =
                     sgoDataPointRepository.Select()
                         .Count(
                             o =>
                                 o.SGOID == sgoDataPoint.SGODataPoint.SGOID &&
                                 o.SGODataPointID != sgoDataPoint.SGODataPoint.SGODataPointID &&
                                 types.Contains(o.Type)) + 1;
            var maxSGODataPoint = GetMaxSGODataPoint(sgoDataPoint);
            var result = totalDataPointWithSameType <= maxSGODataPoint;

            return result;
        }

        public int GetMaxSGODataPoint(SGODataPointSaveDTO sgoDataPoint)
        {
            if (sgoDataPoint == null || sgoDataPoint.SGODataPoint == null) return 0;

            var types = GetSGODataPointPreTypes();
            if (types.Contains(sgoDataPoint.SGODataPoint.Type)) return sgoDataPoint.SGOMAXPreAssessment;

            types = GetSGODataPointPostTypes();
            if (types.Contains(sgoDataPoint.SGODataPoint.Type)) return sgoDataPoint.SGOMAXPostAssessment;

            return 0;
        }

        public List<int> GetPreOrPostSGODataPointTypes(int sgoDataPointType)
        {
            var types = GetSGODataPointPreTypes();
            if (types.Contains(sgoDataPointType)) return types;

            types = GetSGODataPointPostTypes();
            if (types.Contains(sgoDataPointType)) return types;

            return new List<int>();
        }

        public List<int> GetSGODataPointPreTypes()
        {
            var result = new List<int>
            {
                (int) SGODataPointTypeEnum.PreAssessment,
                (int) SGODataPointTypeEnum.PreAssessmentExternal,
                (int) SGODataPointTypeEnum.PreAssessmentHistorical,
                (int) SGODataPointTypeEnum.PreAssessmentCustom
            };

            return result;
        }

        public List<int> GetSGODataPointPostTypes()
        {
            var result = new List<int>
            {
                (int) SGODataPointTypeEnum.PostAssessment,
                (int) SGODataPointTypeEnum.PostAssessmentToBeCreated,
                (int) SGODataPointTypeEnum.PostAssessmentCustom,
                (int) SGODataPointTypeEnum.PostAssessmentExternal,
                (int) SGODataPointTypeEnum.PostAssessmentHistorical
            };

            return result;
        }

        public Subject GetSubjectToCreateExternalBank(string subjectName, int gradeID, int sgoID)
        {
            if (string.IsNullOrWhiteSpace(subjectName)) return null;

            var sgo = _sgoRepository.Select().FirstOrDefault(o => o.SGOID == sgoID);
            if (sgo == null) return null;

            var district = _districtRepository.Select().FirstOrDefault(o => o.Id == sgo.DistrictID);

            var subjectQuery =
                _subjectRepository.Select()
                    .Where(o => subjectName == o.Name && o.GradeId == gradeID);
            if (district != null)
            {
                subjectQuery = subjectQuery.Where(o => o.StateId == district.StateId);
            }

            var result = subjectQuery.OrderBy(o=>o.Id).FirstOrDefault();

            return result;
        }

        public Bank GetOrCreateBankForExternalTest(int? subjectID, int? GradeID, int? createdByUserID)
        {
            if (!subjectID.HasValue || !GradeID.HasValue) return null;

            var subject = _subjectRepository.Select().FirstOrDefault(o => o.Id == subjectID.Value);
            if (subject == null) return null;

            var grade = _gradeRepository.Select().FirstOrDefault(o => o.Id == GradeID.Value);
            if (grade == null) return null;

            var bankName = GenerateExternalBankName(subject.Name, grade.Name);
            if (string.IsNullOrWhiteSpace(bankName)) return null;

            var bank =
                _bankRepository.Select()
                    .FirstOrDefault(o => bankName == o.Name);
            int bankAuthorID = 0;
            if (bank == null)
            {
                if (!createdByUserID.HasValue)
                {
                    var previewTestTeacherID =
                        _configurationRepository.Select()
                            .Where(
                                o =>
                                    Constanst.Configuration_PreviewTestTeacherID == o.Name)
                            .Select(o => o.Value)
                            .FirstOrDefault();
                    int.TryParse(previewTestTeacherID, out bankAuthorID);
                }
                else
                {
                    bankAuthorID = createdByUserID.Value;
                }

                var newBank = new Bank
                {
                    Name = bankName,
                    SubjectID = subjectID.Value,
                    CreatedByUserId = bankAuthorID,
                    BankAccessID = 1,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _bankRepository.Save(newBank);

                return newBank;
            }

            return bank;
        }

        //SGOBank_<subject>_Grade<grade>
        public string GenerateExternalBankName(string subjectName, string gradeName)
        {
            if (string.IsNullOrWhiteSpace(subjectName) || string.IsNullOrWhiteSpace(gradeName)) return null;

            var format = "SGOBank_{0}_Grade{1}";
            var result = string.Format(format, subjectName, gradeName);

            return result;
        }
        private int GetDataPointScoreType(SGODataPoint sgoDataPoint)
        {
            var scoreType = (int)SGOScoreTypeEnum.ScoreRaw; //default scoreType is scoreRaw
            if (sgoDataPoint.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical
                && sgoDataPoint.ScoreType.HasValue)
            {
                scoreType = sgoDataPoint.ScoreType.Value;
            }
            return scoreType;
        }

        public int ValidatePostAssessmentLinkit(int sgoId)
        {
            var sgoObject = _sgoRepository.Select().FirstOrDefault(x => x.SGOID == sgoId);

            var v = sgoDataPointRepository
                .Select().FirstOrDefault(o => o.SGOID == sgoId
                                              && (o.Type == (int) SGODataPointTypeEnum.PostAssessment ||
                                                  o.Type == (int) SGODataPointTypeEnum.PostAssessmentExternal ||
                                                  o.Type == (int) SGODataPointTypeEnum.PostAssessmentHistorical ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom));
            if (v == null)
                return 0;

            if (sgoObject != null && sgoObject.TargetScoreType == 4)
                return 1;

            return _sgoRepository.CheckScoreSGOResults(sgoId, v.SGODataPointID) ? 1 : 2;
        }

        public bool IsExistStudentHasScoreNull(int sgoId, int? sgoDataPointId)
        {
            var sgo = _sgoRepository.Select().FirstOrDefault(x => x.SGOID == sgoId);
            if (sgo == null)
                return true;

            var dataPoints = sgoDataPointRepository.Select().Where(o => o.SGOID == sgoId);
            var postAssessment = dataPoints.FirstOrDefault(o => o.Type == (int)SGODataPointTypeEnum.PostAssessment ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);

            if (postAssessment == null) return true;

            if (!sgoDataPointId.HasValue || sgoDataPointId.Value == 0)
                sgoDataPointId = postAssessment.SGODataPointID;

            return _sgoRepository.IsExistStudentHasScoreNull(sgo.SGOID, sgoDataPointId.GetValueOrDefault());
        }

        public bool CheckScoreTestResultStudent(int sgoId, int virtualTestId)
        {
            bool IsPostAssessmentHasResult, IsPreAssessmentHasResult = false;
            var sgo = _sgoRepository.Select().FirstOrDefault(x => x.SGOID == sgoId);
            if (sgo == null)
                return false;

            var dataPoints = sgoDataPointRepository.Select().Where(o => o.SGOID == sgoId);
            var postAssessment = dataPoints.FirstOrDefault(o => o.Type == (int)SGODataPointTypeEnum.PostAssessment ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical ||
                                                  o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom);

            if (postAssessment == null) return false;
            
            if (!dataPoints.Any(o => o.Type == (int)SGODataPointTypeEnum.PreAssessment ||
                                                 o.Type == (int)SGODataPointTypeEnum.PreAssessmentExternal ||
                                                 o.Type == (int)SGODataPointTypeEnum.PreAssessmentHistorical ||
                                                 o.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom))
                return false;

            var preAssessment = dataPoints.FirstOrDefault(o =>o.ImprovementBasedDataPoint == 1);

            //check score test result PostAssessmentHasResult based on virtualTest or not 
            IsPostAssessmentHasResult = _sgoRepository.CheckScoreTestResultStudent(sgoId,postAssessment.SGODataPointID, virtualTestId);
            if (preAssessment == null && (sgo.TargetScoreType == (int)SGOTargetScoreTypeEnum.AchieveOnPostAssessment || sgo.TargetScoreType == (int)SGOTargetScoreTypeEnum.AchieveXofYOnPostAssessment || sgo.TargetScoreType == (int) SGOTargetScoreTypeEnum.ManualScoring)) //Target score = 1,3,4
            {
                return IsPostAssessmentHasResult;
            }
            else
            {
                //check score test result PreAssessmentHasResult for target core = 2
                if (preAssessment == null)
                    return false;

                IsPreAssessmentHasResult = _sgoRepository.CheckScoreTestResultStudent(sgoId, preAssessment.SGODataPointID, 0);
                return IsPostAssessmentHasResult && IsPreAssessmentHasResult;
            }
        }
        public bool HasPostAccessmentToBeCreated(int sgoId)
        {
            return  sgoDataPointRepository.Select().Any(o => o.SGOID == sgoId && o.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated);
        }
        public decimal ToBeCreatedTotalPointPossibleBySgoId(int sgoId)
        {
            var v = sgoDataPointRepository.Select().FirstOrDefault(o => o.SGOID == sgoId && o.Type == (int)SGODataPointTypeEnum.PostAssessmentToBeCreated);
            if (v != null)
                return v.TotalPoints;
            return 0;
        }

        public bool HasPostAssessment(int sgoId)
        {
            return
                sgoDataPointRepository.Select()
                    .Any(o => o.SGOID == sgoId && (o.Type == (int) SGODataPointTypeEnum.PostAssessmentToBeCreated
                                                   || o.Type == (int) SGODataPointTypeEnum.PostAssessment
                                                   || o.Type == (int) SGODataPointTypeEnum.PostAssessmentCustom
                                                   || o.Type == (int)SGODataPointTypeEnum.PostAssessmentExternal
                                                   || o.Type == (int)SGODataPointTypeEnum.PostAssessmentHistorical));
        }

        public bool HasPostAssessmentCustom(int sgoId)
        {
            return sgoDataPointRepository.Select().Any(o => o.SGOID == sgoId && (o.Type == (int)SGODataPointTypeEnum.PostAssessmentCustom ));
        }

        public List<SGODataPointBand> GetDataPointBandsBySGOID(int sgoID)
        {
            var listDataPointBandIds = sgoDataPointRepository.Select().Where(x => x.SGOID.Equals(sgoID))
                .Select(x => x.SGODataPointID).ToList();
            return
                sgoDataPointBandRepository.Select().Where(x => listDataPointBandIds.Contains(x.SGODataPointID)).ToList();
        }

        public double CalculateDataPointWeight(List<SGODataPoint> listDataPoints, int dataPointId)
        {
            var preAssessmentDataPoint = listDataPoints.Where(
                    x => x.Type != (int)SGODataPointTypeEnum.PostAssessment
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentToBeCreated
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentCustom
						&& x.Type != (int)SGODataPointTypeEnum.PostAssessmentExternal
                        && x.Type != (int)SGODataPointTypeEnum.PostAssessmentHistorical
                        && (x.Type == (int)SGODataPointTypeEnum.PreAssessmentCustom
                            && (x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA1
                                || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA2
                                || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA3
                                || x.ScoreType == (int)SGOScoreTypeEnum.ScoreCustomA4)) == false
                    ).ToList();
            var sumWeight = preAssessmentDataPoint.Any()
                    ? preAssessmentDataPoint.Select(x => x.Weight).Sum()
                    : 0;
            var selectedDataPoint = preAssessmentDataPoint.FirstOrDefault(x => x.SGODataPointID.Equals(dataPointId));

            if (selectedDataPoint != null)
            {
                return sumWeight == 0
                    ? 0
                    : selectedDataPoint.Weight == 0
                        ? 0
                        : selectedDataPoint.Weight*100/sumWeight;
            }

            return 0d;
        }

        public int GetDataPointClusterFilterCount(int sgoDataPointId)
        {
            var count = _sgoDataPointClusterScoreRepository.Select()
                    .Count(x => x.SGODataPointID == sgoDataPointId);
            return count;
        }
    }
}
