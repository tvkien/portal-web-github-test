using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.Survey;
using LinkIt.BubbleSheetPortal.Models.ManageTest;
using LinkIt.BubbleSheetPortal.Models.SGO;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;
using Newtonsoft.Json;
using LinkIt.BubbleSheetPortal.Models.Old.ManageTest;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using LinkIt.BubbleSheetPortal.Models.Extensions;

namespace LinkIt.BubbleSheetPortal.Services.Survey
{
    public class ManageSurveyService
    {
        private readonly IManageTestRepository _manageTestRepository;
        private readonly IVirtualTestRepository _virtualTestRepository;
        private readonly VirtualQuestionService _virtualQuestionService;
        private readonly VirtualTestCustomScoreService _virtualTestCustomScoreService;
        private readonly VirtualTestCustomSubScoreService _virtualTestCustomSubScoreService;
        private readonly VirtualTestCustomMetaDataService _virtualTestCustomMetaDataService;
        private readonly VirtualTestVirtualTestCustomScoreService _virtualTestVirtualTestCustomScoreService;
        private readonly QTIITemService _qtiItemService;
        private readonly VirtualTestService _virtualTestService;
        private readonly VirtualTestFileService _virtualTestFileService;
        private readonly QuestionGroupService _questionGroupService;
        private readonly VirtualQuestionItemTagService _virtualQuestionItemTagService;
        private readonly VirtualQuestionLessonOneService _virtualQuestionLessonOneService;
        private readonly VirtualQuestionLessonTwoService _virtualQuestionLessonTwoService;
        private readonly MasterStandardService _masterStandardService;
        private readonly VirtualQuestionTopicService _virtualQuestionTopicService;
        private readonly VirtualSectionService _virtualSectionService;
        private readonly VirtualSectionQuestionService _virtualSectionQuestionService;
        private readonly QtiBankService _qtiBankService;
        private readonly QtiGroupService _qtiGroupService;
        private readonly QTIITemService _qtiITemService;
        private readonly DataLockerTemplateService _dataLockerTemplateService;
        private readonly IReadOnlyRepository<QTIItemAnswerScore> _qtiItemAnswerScoreRepository;

        public ManageSurveyService(
            IManageTestRepository manageTestRepository,
            IVirtualTestRepository virtualTestRepository,
            VirtualQuestionService virtualQuestionService,
            VirtualTestCustomScoreService virtualTestCustomScoreService,
            VirtualTestCustomSubScoreService virtualTestCustomSubScoreService,
            VirtualTestCustomMetaDataService virtualTestCustomMetaDataService,
            VirtualTestVirtualTestCustomScoreService virtualTestVirtualTestCustomScoreService,
            QTIITemService qtiItemService,
            VirtualTestService virtualTestService,
            VirtualTestFileService virtualTestFileService,
            QuestionGroupService questionGroupService,
            VirtualQuestionItemTagService virtualQuestionItemTagService,
            VirtualQuestionLessonOneService virtualQuestionLessonOneService,
            VirtualQuestionLessonTwoService virtualQuestionLessonTwoService,
            MasterStandardService masterStandardService,
            VirtualQuestionTopicService virtualQuestionTopicService,
            VirtualSectionService virtualSectionService,
            VirtualSectionQuestionService virtualSectionQuestionService,
            QtiBankService qtiBankService,
            QtiGroupService qtiGroupService,
            QTIITemService qtiITemService,
            DataLockerTemplateService dataLockerTemplateService,
            IReadOnlyRepository<QTIItemAnswerScore> qtiItemAnswerScoreRepository)
        {
            _manageTestRepository = manageTestRepository;
            _virtualTestRepository = virtualTestRepository;
            _virtualQuestionService = virtualQuestionService;
            _virtualTestCustomScoreService = virtualTestCustomScoreService;
            _virtualTestCustomSubScoreService = virtualTestCustomSubScoreService;
            _virtualTestCustomMetaDataService = virtualTestCustomMetaDataService;
            _virtualTestVirtualTestCustomScoreService = virtualTestVirtualTestCustomScoreService;
            _qtiItemService = qtiItemService;
            _virtualTestService = virtualTestService;
            _virtualTestFileService = virtualTestFileService;
            _virtualQuestionService = virtualQuestionService;
            _questionGroupService = questionGroupService;
            _virtualQuestionItemTagService = virtualQuestionItemTagService;
            _virtualQuestionLessonOneService = virtualQuestionLessonOneService;
            _virtualQuestionLessonTwoService = virtualQuestionLessonTwoService;
            _masterStandardService = masterStandardService;
            _virtualQuestionTopicService = virtualQuestionTopicService;
            _virtualSectionService = virtualSectionService;
            _virtualSectionQuestionService = virtualSectionQuestionService;
            _qtiBankService = qtiBankService;
            _qtiGroupService = qtiGroupService;
            _qtiITemService = qtiITemService;
            _dataLockerTemplateService = dataLockerTemplateService;
            _qtiItemAnswerScoreRepository = qtiItemAnswerScoreRepository;
        }

        public List<BankData> GetSurveyBanksByUserID(int userID, int roleId, int districtId, bool showArchived, bool filterByDistrict = true)
        {
            return _manageTestRepository.GetSurveyBanksByUserID(userID, roleId, districtId, showArchived, filterByDistrict).ToList();
        }

        public void ProcessingScoreNameSurveyTemplate(VirtualQuestionData virtualQuestion, string oldScoreName, string itemNumberLabel, int qtiSchemaId, User user)
        {
            var virtualTest_VirtualTestCustomScore = _virtualTestVirtualTestCustomScoreService.GetByVirtualTestId(virtualQuestion.VirtualTestID);
            if (string.IsNullOrEmpty(oldScoreName) && !string.IsNullOrEmpty(virtualQuestion.ScoreName))
            {
                //insert sub                
                if (virtualTest_VirtualTestCustomScore is null)
                {
                    var virtualTest = _virtualTestRepository.Select().FirstOrDefault(x => x.VirtualTestID == virtualQuestion.VirtualTestID);
                    var virtualTestCustomScore = _virtualTestCustomScoreService.CreateCustomScoreForSurvey(virtualTest.Name, user);

                    virtualTest_VirtualTestCustomScore = new VirtualTestVirtualTestCustomScore() { VirtualTestId = virtualQuestion.VirtualTestID, VirtualTestCustomScoreId = virtualTestCustomScore.VirtualTestCustomScoreId };
                    _virtualTestVirtualTestCustomScoreService.Save(virtualTest_VirtualTestCustomScore);
                }

                var subScore = CreateVirtualTestCustomSubScore(virtualQuestion, virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId, itemNumberLabel);
                if (qtiSchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable)
                    CreateVirtualTestCustomMetaData(virtualQuestion.QTIItemID ?? 0, virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId, subScore.VirtualTestCustomSubScoreId);
            }
            else if (string.IsNullOrEmpty(virtualQuestion.ScoreName))
            {
                //delete sub                
                DeleteSubScoreAndRelationShip(virtualQuestion.ScoreName, virtualTest_VirtualTestCustomScore);
            }
            else if (virtualTest_VirtualTestCustomScore != null && !string.IsNullOrEmpty(oldScoreName))
            {
                //update sub name
                var subscore = _virtualTestCustomSubScoreService.GetByName(virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId, oldScoreName);
                if (subscore != null)
                {
                    subscore.Name = virtualQuestion.ScoreName;
                    _virtualTestCustomSubScoreService.Save(subscore);
                }
            }
        }
        public void ProcessingUpdateQuestionType(int oldQTISchemaId, int newQTISchemaId, int qtiitemId, int virtualQuestionId)
        {
            if (virtualQuestionId > 0)
            {
                var virtualQuestion = _virtualQuestionService.GetQuestionDataById(virtualQuestionId);
                var virtualTest = _virtualTestService.GetVirtualTestById(virtualQuestion.VirtualTestID);
                if (virtualTest.DatasetOriginID == (int)DataSetOriginEnum.Survey && !string.IsNullOrEmpty(virtualQuestion.ScoreName))
                {
                    //update Template
                    var virtualTest_VirtualTestCustomScore = _virtualTestVirtualTestCustomScoreService.GetByVirtualTestId(virtualQuestion.VirtualTestID);
                    if (newQTISchemaId == (int)QTISchemaEnum.ExtendedText)
                    {
                        DeleteSubScoreAndRelationShip(virtualQuestion.ScoreName, virtualTest_VirtualTestCustomScore);

                        virtualQuestion.ScoreName = null;
                        _virtualQuestionService.Save(virtualQuestion);
                    }
                    else
                    {
                        var subscore = _virtualTestCustomSubScoreService.GetByName(virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId, virtualQuestion.ScoreName);
                        if (newQTISchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable && subscore != null)
                        {
                            if (subscore.UseCustomA1.GetValueOrDefault() && !subscore.UseCustomN1.GetValueOrDefault())
                            {
                                subscore.UseCustomN1 = true;
                                subscore.CustomN1Label = subscore.CustomA1Label;
                                subscore.UseCustomA1 = false;
                                subscore.CustomA1Label = null;
                                _virtualTestCustomSubScoreService.Save(subscore);
                            }
                            var metaDatas = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(subscore.VirtualTestCustomSubScoreId);
                            foreach (var metaData in metaDatas)
                            {
                                _virtualTestCustomMetaDataService.Delete(metaData);
                            }

                            CreateVirtualTestCustomMetaData(qtiitemId, subscore.VirtualTestCustomScoreId, subscore.VirtualTestCustomSubScoreId);
                        }
                        else if (oldQTISchemaId == (int)QTISchemaEnum.ChoiceMultipleVariable && subscore != null &&
                            (newQTISchemaId == (int)QTISchemaEnum.Choice || newQTISchemaId == (int)QTISchemaEnum.ChoiceMultiple))
                        {
                            if (!subscore.UseCustomA1.GetValueOrDefault() && subscore.UseCustomN1.GetValueOrDefault())
                            {
                                subscore.UseCustomA1 = true;
                                subscore.CustomA1Label = subscore.CustomN1Label;
                                subscore.UseCustomN1 = false;
                                subscore.CustomN1Label = null;
                                _virtualTestCustomSubScoreService.Save(subscore);
                            }

                            var metaDatas = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(subscore.VirtualTestCustomSubScoreId);
                            foreach (var metaData in metaDatas)
                            {
                                _virtualTestCustomMetaDataService.Delete(metaData);
                            }
                        }
                    }
                }
            }
        }
        private void DeleteSubScoreAndRelationShip(string scoreName, VirtualTestVirtualTestCustomScore virtualTest_VirtualTestCustomScore)
        {
            var subscore = _virtualTestCustomSubScoreService.GetByName(virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId, scoreName);
            if (subscore != null)
            {
                var metaDatas = _virtualTestCustomMetaDataService.GetMetaDataOfSubscore(subscore.VirtualTestCustomSubScoreId);
                foreach (var metaData in metaDatas)
                {
                    _virtualTestCustomMetaDataService.Delete(metaData);
                }
                _virtualTestCustomSubScoreService.Delete(subscore);
            }

            var remainSubscores = _virtualTestCustomSubScoreService.GetSubscoreOfTemplate(virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId);
            if (remainSubscores.Count == 0) //delete virtualTestCustomScore
            {
                var virtualTestCustomScore = _virtualTestCustomScoreService.GetCustomScoreByID(virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId);
                _virtualTestCustomScoreService.Delete(virtualTestCustomScore);
                _virtualTestVirtualTestCustomScoreService.Delete(virtualTest_VirtualTestCustomScore);
            }
        }

        public void UpdateSubScoreLabelSurveyTemplate(SurveyItem item)
        {
            if (!string.IsNullOrEmpty(item.Items))
                item.VirtualQuestionItemNumbers = JsonConvert.DeserializeObject<List<VirtualQuestionItemNumber>>(item.Items);
            _manageTestRepository.UpdateSubScoreLabelSurveyTemplate(item);
        }
        private VirtualTestCustomSubScore CreateVirtualTestCustomSubScore(VirtualQuestionData virtualQuestion, int virtualTestCustomScoreId, string itemNumberLabel)
        {
            var qtiitem = _qtiItemService.GetQtiItemById(virtualQuestion.QTIItemID ?? 0);
            var isCustomNumber = qtiitem?.QTISchemaID == (int)QTISchemaType.ChoiceMultipleVariable;
            if (!string.IsNullOrEmpty(virtualQuestion.QuestionNumber))
                itemNumberLabel = virtualQuestion.QuestionNumber;
            if (!string.IsNullOrEmpty(virtualQuestion.QuestionLabel))
                itemNumberLabel = $"{itemNumberLabel}{virtualQuestion.QuestionLabel}";

            itemNumberLabel = $"{itemNumberLabel}";
            var subscore = new VirtualTestCustomSubScore()
            {
                Name = virtualQuestion.ScoreName,
                VirtualTestCustomScoreId = virtualTestCustomScoreId,
                UseCustomA1 = !isCustomNumber,
                UseCustomN1 = isCustomNumber,
                CustomA1Label = !isCustomNumber ? itemNumberLabel : null,
                CustomN1Label = isCustomNumber ? itemNumberLabel : null,
                Sequence = virtualQuestion.QuestionOrder
            };
            _virtualTestCustomSubScoreService.Save(subscore);
            return subscore;
        }

        private void CreateVirtualTestCustomMetaData(int qtiitemId, int virtualTestCustomScoreId, int virtualTestCustomSubScoreId)
        {
            var qtiitemAnswerScores = _qtiItemAnswerScoreRepository.Select().Where(x => x.QTIItemId == qtiitemId).ToList();
            var selectOptions = qtiitemAnswerScores.Select(x => new VirtualTestCustomMetaSelectListOptionModel
            {
                Label = x.AnswerText,
                Option = x.Score?.Trim()
            }).ToList();
            var meta = new VirtualTestCustomMetaModel()
            {
                DataType = "Numeric",
                SelectListOptions = selectOptions,
                FormatOption = "LabelValueText",
                DisplayOption = "both"
            };
            var metaData = new VirtualTestCustomMetaData()
            {
                VirtualTestCustomScoreID = virtualTestCustomScoreId,
                VirtualTestCustomSubScoreID = virtualTestCustomSubScoreId,
                Order = 1,
                ScoreType = VirtualTestCustomMetaData.CustomN_1,
                MetaData = meta.GetJsonString()
            };
            _virtualTestCustomMetaDataService.Save(metaData);
        }
        public void CopySurvey(VirtualTestData virtualTestData, SurveyCloneInformationDto cloneInformation)
        {
            int oldTestId = virtualTestData.VirtualTestID;
            virtualTestData.VirtualTestID = 0;
            virtualTestData.CreatedDate = DateTime.UtcNow;
            virtualTestData.UpdatedDate = DateTime.UtcNow;
            virtualTestData.AuthorUserID = cloneInformation.UserId;

            _virtualTestService.Save(virtualTestData);
            // Clone Relationship
            CloneRelationShipVirtualTest(oldTestId, virtualTestData.VirtualTestID);

            //Clone Question, VirtualSection, VirtualSectionQuestion
            CloneQuestions(oldTestId, virtualTestData.VirtualTestID, virtualTestData.Name, cloneInformation);

            //Udpate BaseVirtualQuestion
            _virtualTestService.UpdateBaseVirtualQuestionClone(oldTestId, virtualTestData.VirtualTestID);

            //clone survey template
            var virtualTest_VirtualTestCustomScore = _virtualTestVirtualTestCustomScoreService.GetByVirtualTestId(oldTestId);
            if (virtualTest_VirtualTestCustomScore != null)
            {
                var newTemplateId = _dataLockerTemplateService.CopyTemplateByID(virtualTest_VirtualTestCustomScore.VirtualTestCustomScoreId, cloneInformation.UserId, virtualTestData.Name);
                _virtualTestVirtualTestCustomScoreService.Save(new VirtualTestVirtualTestCustomScore()
                {
                    VirtualTestId = virtualTestData.VirtualTestID,
                    VirtualTestCustomScoreId = newTemplateId
                });
            }
        }
        private void CloneRelationShipVirtualTest(int oldTestId, int newTestId)
        {
            var virtualTestFile = _virtualTestFileService.GetFirstOrDefaultByVirtualTest(oldTestId);
            if (virtualTestFile != null)
            {
                virtualTestFile.VirtualTestFileId = 0;
                virtualTestFile.VirtualTestId = newTestId;
                _virtualTestFileService.Save(virtualTestFile);
            }
        }

        private void CloneQuestions(int oldTestId, int newTestId, string testName, SurveyCloneInformationDto cloneInformation)
        {
            var qtiItems = new Dictionary<int, CloneQTIItemDto>();
            var cloneVirtualQuestion = new List<CloneVirtualQuestion>();

            //TODO: Create VirtualQuestion
            CloneQuestionsByTestId(oldTestId, newTestId, qtiItems, cloneVirtualQuestion);

            //TODO: Clone Question RelationShip
            CloneQuestionRelationship(cloneVirtualQuestion);

            //TODO: Create Section & SectionQuestion
            CloneVirtualSection(oldTestId, newTestId, qtiItems);

            //TODO: Create QtiItem
            CloneQTIITemAndUpdateQuestions(testName, qtiItems, cloneInformation);
        }

        private void CloneQuestionsByTestId(int oldTestId, int newTestId, Dictionary<int, CloneQTIItemDto> qtiItems, List<CloneVirtualQuestion> cloneVQs)
        {
            var lstOldQuestions = _virtualQuestionService.GetVirtualQuestionByVirtualTestID(oldTestId);

            if (lstOldQuestions.Count == 0)
            {
                return;
            }

            var oldQuestions = lstOldQuestions.Select(x => new { x.VirtualQuestionID, x.QTIItemID, x.QuestionOrder }).ToList();

            var newQuestions = lstOldQuestions.Clone();
            newQuestions.ForEach(item =>
            {
                item.VirtualQuestionID = 0;
                item.VirtualTestID = newTestId;
            });

            _virtualQuestionService.InsertMultipleRecord(newQuestions);

            oldQuestions.ForEach(oldQuestion =>
            {
                var newQuestion = newQuestions.FirstOrDefault(x => x.QTIItemID == oldQuestion.QTIItemID && x.QuestionOrder == oldQuestion.QuestionOrder);

                qtiItems.Add(oldQuestion.VirtualQuestionID, new CloneQTIItemDto()
                {
                    NewQuestionID = newQuestion.VirtualQuestionID,
                    OldQTIITemID = oldQuestion.QTIItemID ?? 0
                });

                cloneVQs.Add(new CloneVirtualQuestion
                {
                    OldVirtualQuestionID = oldQuestion.VirtualQuestionID,
                    NewVirtualQuestionID = newQuestion.VirtualQuestionID,
                });
            });

            var dcQuestionGroupId = new Dictionary<int, int>();

            var oldVirtualQeustionIDs = oldQuestions.Select(x => x.VirtualQuestionID).ToList();
            var virtualQuestionGroups = _questionGroupService.GetVirtualQuestionGroupsByVirtualQuestionIds(oldVirtualQeustionIDs);

            if (virtualQuestionGroups.Any())
            {
                var questionGroupIDs = virtualQuestionGroups.Select(x => x.QuestionGroupID).ToList();
                var questionGroups = _questionGroupService.GetQuestionGroups(oldTestId, questionGroupIDs);

                foreach (var oldQuestion in lstOldQuestions)
                {
                    var newVirtualQuestionID = newQuestions.FirstOrDefault(x => x.QTIItemID == oldQuestion.QTIItemID && x.QuestionOrder == oldQuestion.QuestionOrder).VirtualQuestionID;
                    var vQuestionGroup = virtualQuestionGroups.FirstOrDefault(x => x.VirtualQuestionID == oldQuestion.VirtualQuestionID);

                    if (vQuestionGroup != null)
                    {
                        var questionGroup = questionGroups.FirstOrDefault(x => x.QuestionGroupID == vQuestionGroup.QuestionGroupID);

                        QuestionGroup questionGroupItem = new QuestionGroup();
                        if (!dcQuestionGroupId.ContainsKey(vQuestionGroup.QuestionGroupID))
                        {
                            questionGroupItem.QuestionGroupID = 0;
                            questionGroupItem.VirtualTestId = newTestId;
                            questionGroupItem.Order = vQuestionGroup.Order;
                            questionGroupItem.XmlContent = questionGroup.XmlContent;
                            questionGroupItem.VirtualSectionID = questionGroup.VirtualSectionID;
                            questionGroupItem.Title = questionGroup.Title;
                            questionGroupItem.DisplayPosition = questionGroup.DisplayPosition;
                            _questionGroupService.SaveQuestionGroup(questionGroupItem);
                            dcQuestionGroupId.Add(vQuestionGroup.QuestionGroupID, questionGroupItem.QuestionGroupID);
                        }

                        VirtualQuestionGroup itemvirtualQuestionGroup = new VirtualQuestionGroup();
                        itemvirtualQuestionGroup.VirtualQuestionGroupID = 0;
                        itemvirtualQuestionGroup.VirtualQuestionID = newVirtualQuestionID;
                        itemvirtualQuestionGroup.QuestionGroupID = (dcQuestionGroupId[vQuestionGroup.QuestionGroupID] != 0) ? dcQuestionGroupId[vQuestionGroup.QuestionGroupID] : questionGroupItem.QuestionGroupID;
                        itemvirtualQuestionGroup.Order = vQuestionGroup.Order;
                        _questionGroupService.SaveVirtualQuestionGroup(itemvirtualQuestionGroup);
                    }
                }
            }
        }

        /// <summary>
        /// Clone VirtualQuestionAnswerScore. This function include when clone QTIITem
        /// </summary>
        /// <param name="qtiItems"></param>
        private void CloneQuestionRelationship(List<CloneVirtualQuestion> cloneVQs)
        {
            if (cloneVQs.Count == 0)
            {
                return;
            }

            _virtualQuestionItemTagService.CloneVirtualQuestionItemTag(cloneVQs);

            _virtualQuestionLessonOneService.CloneVirtualQuestionLessonOne(cloneVQs);

            _virtualQuestionLessonTwoService.CloneVirtualQuestionLessonTwo(cloneVQs);

            _masterStandardService.CloneVirtualQuestionStateStandard(cloneVQs);

            _virtualQuestionTopicService.CloneVirtualQuestionTopic(cloneVQs);
        }

        private void CloneVirtualSection(int oldTestId, int newTestId, Dictionary<int, CloneQTIItemDto> qtiItems)
        {
            var sections = _virtualSectionService.GetVirtualSectionByVirtualTest(oldTestId);
            if (sections.Count > 0)
            {
                foreach (VirtualSection section in sections)
                {
                    var virtualSection = _virtualSectionService.GetVirtualSectionById(section.VirtualSectionId);
                    int oldVirtualSectionId = virtualSection.VirtualSectionId;
                    virtualSection.VirtualTestId = newTestId;
                    virtualSection.VirtualSectionId = 0;
                    _virtualSectionService.Save(virtualSection);
                    CloneVirtualSectionQuestion(oldTestId, oldVirtualSectionId, virtualSection, qtiItems);
                    var listQuestionGroup = _questionGroupService.GetListQuestionGroupByVirtualTestIdAndSectionId(newTestId, oldVirtualSectionId);
                    _questionGroupService.UpdateSectionIdToQuestionGroup(virtualSection.VirtualSectionId, listQuestionGroup);
                }
            }
        }

        private void CloneVirtualSectionQuestion(int oldVirtualTestId, int oldVirtualSectionId, VirtualSection newVirtualSection, Dictionary<int, CloneQTIItemDto> qtiItems)
        {
            var sectionQuestions = _virtualSectionQuestionService.GetVirtualSectionQuestionBySection(oldVirtualTestId, oldVirtualSectionId);
            if (sectionQuestions.Count > 0)
            {
                foreach (VirtualSectionQuestion sectionQuestion in sectionQuestions)
                {
                    var cSectionQuestion = _virtualSectionQuestionService.GetVirtualSectionQuestionById(sectionQuestion.VirtualSectionQuestionId);
                    int oldVirtualQuestionId = cSectionQuestion.VirtualQuestionId;
                    int newVirtualQuestionId = qtiItems[oldVirtualQuestionId].NewQuestionID;
                    cSectionQuestion.VirtualSectionQuestionId = 0;
                    cSectionQuestion.VirtualSectionId = newVirtualSection.VirtualSectionId;
                    cSectionQuestion.VirtualQuestionId = newVirtualQuestionId;
                    _virtualSectionQuestionService.Save(cSectionQuestion);
                }
            }
        }

        private void CloneQTIITemAndUpdateQuestions(string testName, Dictionary<int, CloneQTIItemDto> qtiItems, SurveyCloneInformationDto cloneInformation)
        {
            var vQtiGroup = _qtiGroupService.CreateItemSetByUserId(cloneInformation.UserId, null, testName);

            CloneQTIItemWithQTIIGroupId(vQtiGroup.QtiGroupId, qtiItems, cloneInformation);
        }

        private void CloneQTIItemWithQTIIGroupId(int qtiigroupId, Dictionary<int, CloneQTIItemDto> qtiItems, SurveyCloneInformationDto cloneInformation)
        {
            if (qtiigroupId > 0 && qtiItems.Count > 0)
            {
                //Need to upload media file (image,audio),if any, to S3
                var bucketName = cloneInformation.AUVirtualTestBucketName;
                var folder = cloneInformation.AUVirtualTestFolder;

                foreach (KeyValuePair<int, CloneQTIItemDto> qtiItem in qtiItems)
                {
                    // Clone QTIITem
                    int newQTIITeamId = _qtiITemService.DuplicateQTIItemForTest(cloneInformation.UserId, qtiItem.Value.OldQTIITemID, qtiigroupId, qtiItem.Key,
                        qtiItem.Value.NewQuestionID, true, bucketName, folder, cloneInformation.S3Domain);

                    qtiItems[qtiItem.Key].NewQTIITemID = newQTIITeamId;

                    // Update QuestionID
                    _virtualQuestionService.UpdateQIITemIdbyQuestionId(qtiItem.Value.NewQuestionID, newQTIITeamId);
                }
            }
        }

        public IEnumerable<ReviewSurveyData> GetReviewSurveys(int userID, int roleId, int? districtId, int? termId, int? surveyAssignmentType, int? surveyBankId, int? surveyId, bool showActiveAssignment, string sort, string search, int? skip, int? take)
        {
            var results = _manageTestRepository.GetReviewSurveys(userID, roleId, districtId, termId, surveyAssignmentType, surveyBankId, surveyId, showActiveAssignment, sort, search, skip, take);

            return results;
        }
    }
}
