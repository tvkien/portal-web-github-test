using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.QuestionGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using LinkIt.BubbleSheetPortal.Models.Constants;
using LinkIt.BubbleSheetPortal.Models.SGO;
using System.IO;
using CsvHelper;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Models.TestMaker;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestService
    {
        private readonly IVirtualTestRepository virtualTestRepository;
        private readonly IRepository<VirtualSection> virtualSectionRepository;
        private readonly IRepository<VirtualSectionQuestion> virtualSectionQuestionRepository;
        private readonly IRepository<VirtualQuestionData> virtualQuestionRepository;
        private readonly IQTIItemRepository qtiItemRepository;
        private readonly ITestResultRepository testResultRepository;
        private readonly IVirtualQuestionPassageNoShuffleRepository vqNoShuffleRepository;
        private readonly IRepository<QuestionGroup> questionGroup;
        private readonly IReadOnlyRepository<QuestionGroupQuestion> questiongroupquestion;
        private readonly IRepository<VirtualTestMeta> virtualTestMetaRepository;
        private readonly DistrictDecodeService _districtDecodeService;
        private readonly ConfigurationService _configurationService;
        private readonly IRubricModuleQueryService _rubricQuestionCategoryService;
        private readonly ExportAnswerKeyService _exportAnswerKeyService;

        public VirtualTestService(IVirtualTestRepository virtualTestRepository,
            IRepository<VirtualSection> virtualSectionRepository,
            IRepository<VirtualSectionQuestion> virtualSectionQuestionRepository,
            IRepository<VirtualQuestionData> virtualQuestionRepository,
            IQTIItemRepository qtiItemRepository, ITestResultRepository testResultRepository,
            IVirtualQuestionPassageNoShuffleRepository vqNoShuffleRepository,
            IRepository<QuestionGroup> questionGroup, IReadOnlyRepository<QuestionGroupQuestion> questiongroupquestion,
            IRepository<VirtualTestMeta> virtualTestMetaRepository, DistrictDecodeService districtDecodeService,
            ConfigurationService configurationService, IRubricModuleQueryService rubricQuestionCategoryService,
            ExportAnswerKeyService exportAnswerKeyService)
        {
            this.virtualTestRepository = virtualTestRepository;
            this.virtualSectionRepository = virtualSectionRepository;
            this.virtualSectionQuestionRepository = virtualSectionQuestionRepository;
            this.qtiItemRepository = qtiItemRepository;
            this.testResultRepository = testResultRepository;
            this.virtualQuestionRepository = virtualQuestionRepository;
            this.vqNoShuffleRepository = vqNoShuffleRepository;
            this.questionGroup = questionGroup;
            this.questiongroupquestion = questiongroupquestion;
            this.virtualTestMetaRepository = virtualTestMetaRepository;
            this._districtDecodeService = districtDecodeService;
            this._configurationService = configurationService;
            this._rubricQuestionCategoryService = rubricQuestionCategoryService;
            this._exportAnswerKeyService = exportAnswerKeyService;
        }

        public IQueryable<VirtualTestData> Select()
        {
            return virtualTestRepository.Select();
        }

        public void Save(VirtualTestData virtualTestData)
        {
            virtualTestRepository.Save(virtualTestData);
        }

        public S3VirtualTest CreateS3Object(int virtualTestId)
        {
            return CreateS3ObjectFromStoredProc(virtualTestId); //optimize, reduce time of querying database
        }

        private S3VirtualTest CreateS3ObjectFromStoredProc(int virtualTestId)
        {
            var virtualTest = virtualTestRepository.Select().FirstOrDefault(en => en.VirtualTestID == virtualTestId);
            if (virtualTest == null)
                return null;

            var isCustomItemNaming = IsCustomItemNaming(virtualTestId);
            var s3VirtualTest = new S3VirtualTest()
            {
                sections = new List<S3VirtualSection>(),
                testData = new S3TestData
                {
                    testInstruction = string.IsNullOrWhiteSpace(virtualTest.Instruction) ? string.Empty : virtualTest.Instruction,
                    testTitle = virtualTest.Name
                },
                virtualTestID = virtualTest.VirtualTestID,
                isCustomItemNaming = isCustomItemNaming
            };

            var virtualSections = virtualSectionRepository.Select().Where(en => en.VirtualTestId == virtualTestId).OrderBy(x => x.Order);
            var virtualQuestionS3Object = virtualTestRepository.GetVirtualQuestionToCreateS3Object(virtualTestId).ToList();

            var lstQuestionGroupQuestion = questiongroupquestion.Select().Where(o => o.VirtualTestID == virtualTestId).ToList();

            bool hasUseRationale = false;

            foreach (var virtualSection in virtualSections)
            {
                var s3VirtualSection = new S3VirtualSection
                {
                    items = new List<S3VirtualQuestion>(),
                    qtiGroupID = 0,
                    sectionData = new S3SectionData
                    {
                        sectionAudioRef = virtualSection.AudioRef,
                        sectionInstruction = string.IsNullOrWhiteSpace(virtualSection.Instruction) ? string.Empty : virtualSection.Instruction,
                        sectionTitle = virtualSection.Title,
                        isTutorialMode = virtualSection.Mode == 1
                    },
                    sectionID = virtualSection.VirtualSectionId,
                    questionGroups = questionGroup.Select().Where(o => o.VirtualTestId == virtualTestId && o.VirtualSectionID == virtualSection.VirtualSectionId)
                    .Select(o => new S3QuestionGroup()
                    {
                        questionGroupID = o.QuestionGroupID,
                        xmlContent = o.XmlContent,
                        displayPosition = o.DisplayPosition,
                        questionGroupTitle = o.Title
                    }).ToList()
                };

                var virtualSectionQuestionS3Objects =
                    virtualQuestionS3Object.Where(x => x.VirtualSectionID == virtualSection.VirtualSectionId).OrderBy(
                        x => x.VirtualSectionQuestionOrder).ToList();

                foreach (var virtualSectionQuestionS3Object in virtualSectionQuestionS3Objects)
                {
                    if (s3VirtualSection.qtiGroupID == 0)
                        s3VirtualSection.qtiGroupID = virtualSectionQuestionS3Object.QTIGroupID;

                    var xmlContent = virtualSectionQuestionS3Object.XmlContent;

                    if (hasUseRationale == false)
                    {
                        var xDoc = ServiceUtil.LoadXmlDocument(xmlContent);
                        var nodesGuidance = xDoc.SelectNodes("//@typemessage");
                        var nodesFillInBlank = xDoc.GetElementsByTagName("responseRubric");
                        if (nodesGuidance.Count > 0 || nodesFillInBlank.Count > 0)
                        {
                            hasUseRationale = true;
                        }
                    }

                    //bind noshuffle attribute
                    var passageNoShuffles =
                        vqNoShuffleRepository.Select()
                            .Where(x => x.VirtualQuestionID == virtualSectionQuestionS3Object.VirtualQuestionID).ToList();
                    if (passageNoShuffles.Any())
                    {
                        var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                        var dataFileUploadPassageIds = passageNoShuffles.Select(x => x.DataFileUploadPassageID).ToList();
                        var qti3pPassageIds = passageNoShuffles.Select(x => x.QTI3pPassageID).ToList();
                        var qtiRefObjectIds = passageNoShuffles.Select(x => x.QTIRefObjectID).ToList();
                        var dataList = passageNoShuffles.Select(x => x.PassageURL).ToList();
                        xmlContent = xmlContentProcessing.AddNoshuffleAttrForPassage(dataFileUploadPassageIds,
                    qti3pPassageIds, qtiRefObjectIds, dataList);
                    }

                    //TODO:
                    var questionInGroup = lstQuestionGroupQuestion.FirstOrDefault(o => o.VirtualQuestionID == virtualSectionQuestionS3Object.VirtualQuestionID);

                    s3VirtualSection.items.Add(new S3VirtualQuestion
                    {
                        xmlContent = xmlContent,
                        pointsPossible = virtualSectionQuestionS3Object.PointsPossible,
                        qtiItemID = virtualSectionQuestionS3Object.QTIItemID,
                        qtiSchemaID = virtualSectionQuestionS3Object.QTISchemaID,
                        virtualQuestionID = virtualSectionQuestionS3Object.VirtualQuestionID,
                        questionOrder = virtualSectionQuestionS3Object.QuestionOrder,
                        baseVirtualQuestionID = virtualSectionQuestionS3Object.BaseVirtualQuestionID,
                        questionGroupID = questionInGroup?.QuestionGroupID,
                        questionLabel = virtualSectionQuestionS3Object.QuestionLabel
                    });
                }
                s3VirtualTest.sections.Add(s3VirtualSection);
            }

            //If there's any virtualsectionId = 0, add to default section
            if (virtualQuestionS3Object.Exists(x => x.VirtualSectionID == 0))
            {
                //Create a default section
                var defaultSection = new S3VirtualSection
                {
                    items = new List<S3VirtualQuestion>(),
                    qtiGroupID = 0,
                    sectionData = new S3SectionData
                    {
                        sectionAudioRef = "",
                        sectionInstruction = "",
                        sectionTitle = ""
                    },
                    sectionID = 0,
                    questionGroups = questionGroup.Select()
                    .Where(o => o.VirtualTestId == virtualTestId && (!o.VirtualSectionID.HasValue || o.VirtualSectionID.Value == 0))
                    .Select(o => new S3QuestionGroup()
                    {
                        questionGroupID = o.QuestionGroupID,
                        xmlContent = o.XmlContent,
                        displayPosition = o.DisplayPosition,
                        questionGroupTitle = o.Title
                    }).ToList()
                };

                //Assign all virtual question belong to virtualsectionId = 0 to this Default section, order by VirtualQuestion.Order
                var virtualSectionQuestionS3Objects =
                    virtualQuestionS3Object.Where(x => x.VirtualSectionID == 0).OrderBy(
                        x => x.QuestionOrder).ToList();

                foreach (var virtualSectionQuestionS3Object in virtualSectionQuestionS3Objects)
                {
                    if (defaultSection.qtiGroupID == 0)
                        defaultSection.qtiGroupID = virtualSectionQuestionS3Object.QTIGroupID;

                    var xmlContent = virtualSectionQuestionS3Object.XmlContent;
                    if (hasUseRationale == false)
                    {
                        var xDoc = ServiceUtil.LoadXmlDocument(xmlContent);
                        var nodesGuidance = xDoc.SelectNodes("//@typemessage");
                        var nodesFillInBlank = xDoc.GetElementsByTagName("responseRubric");
                        if (nodesGuidance.Count > 0 || nodesFillInBlank.Count > 0)
                        {
                            hasUseRationale = true;
                        }
                    }

                    //bind noshuffle attribute
                    var passageNoShuffles =
                        vqNoShuffleRepository.Select()
                            .Where(x => x.VirtualQuestionID == virtualSectionQuestionS3Object.VirtualQuestionID).ToList();
                    if (passageNoShuffles.Any())
                    {
                        var xmlContentProcessing = new XmlContentProcessing(xmlContent);
                        var dataFileUploadPassageIds = passageNoShuffles.Select(x => x.DataFileUploadPassageID).ToList();
                        var qti3pPassageIds = passageNoShuffles.Select(x => x.QTI3pPassageID).ToList();
                        var qtiRefObjectIds = passageNoShuffles.Select(x => x.QTIRefObjectID).ToList();
                        var dataList = passageNoShuffles.Select(x => x.PassageURL).ToList();
                        xmlContent = xmlContentProcessing.AddNoshuffleAttrForPassage(dataFileUploadPassageIds,
                    qti3pPassageIds, qtiRefObjectIds, dataList);
                    }

                    var questionInGroupOfSectionDefault = lstQuestionGroupQuestion.FirstOrDefault(o => o.VirtualQuestionID == virtualSectionQuestionS3Object.VirtualQuestionID);

                    defaultSection.items.Add(new S3VirtualQuestion
                    {
                        //xmlContent = RemoveCorrectResponseData(virtualSectionQuestionS3Object.XmlContent),
                        xmlContent = xmlContent,
                        //pointsPossible = qtiItem.PointsPossible,
                        pointsPossible = virtualSectionQuestionS3Object.PointsPossible,
                        // PointsPossible is gotten from VirtualQuestion not QtiItem
                        qtiItemID = virtualSectionQuestionS3Object.QTIItemID,
                        qtiSchemaID = virtualSectionQuestionS3Object.QTISchemaID,
                        virtualQuestionID = virtualSectionQuestionS3Object.VirtualQuestionID,
                        questionOrder = virtualSectionQuestionS3Object.QuestionOrder,
                        baseVirtualQuestionID = virtualSectionQuestionS3Object.BaseVirtualQuestionID,
                        questionGroupID = questionInGroupOfSectionDefault?.QuestionGroupID,
                        questionLabel = virtualSectionQuestionS3Object.QuestionLabel
                    });
                }
                s3VirtualTest.sections.Insert(0, defaultSection);//The Default section will be inserted at the first
            }

            virtualTest.HasUseRationale = hasUseRationale;
            virtualTestRepository.Save(virtualTest);

            return s3VirtualTest;
        }

        public string RemoveCorrectResponseData(string xmlContent)
        {
            var xdoc = new XmlDocument();

            //var htmlEntityReplace = string.Format("<![CDATA[{0}]]>", Guid.NewGuid());
            //xmlContent = xmlContent.Replace("&#", htmlEntityReplace);
            XmlSpecialCharToken xmlSpecialCharToken = new XmlSpecialCharToken();
            xmlContent = xmlContent.ReplaceXmlSpecialChars(xmlSpecialCharToken);
            xdoc.LoadXml(xmlContent);
            var correctResponseNodes = xdoc.GetElementsByTagName("correctResponse");
            for (int i = correctResponseNodes.Count - 1; i >= 0; i--)
            {
                XmlNode correctResponseNode = correctResponseNodes[i];
                var parentNode = correctResponseNode.ParentNode;
                parentNode.RemoveChild(correctResponseNode);
            }

            return xdoc.OuterXml.RecoverXmlSpecialChars(xmlSpecialCharToken);
        }

        public bool ExistTestName(int bankId, string strTextName)
        {
            return virtualTestRepository.Select().Any(o => o.BankID == bankId && o.Name.ToLower() == strTextName.ToLower());
        }

        public bool ExistTestNameUpdate(int bankId, int virtualTestId, string strTextName)
        {
            return virtualTestRepository.Select().Any(o => o.BankID == bankId && o.VirtualTestID != virtualTestId && o.Name.ToLower() == strTextName.ToLower());
        }

        public VirtualTestData GetTestById(int virtualTestId)
        {
            return virtualTestRepository.Select().FirstOrDefault(o => o.VirtualTestID == virtualTestId);
        }

        /// <summary>
        /// Get VirtualTest by VirtualtestID with VirtualTestSourceID <> 3
        /// This is Linkit Test. Not Legacy Test.
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <returns></returns>
        public VirtualTestData GetLinkitTestById(int virtualTestId)
        {
            return virtualTestRepository.Select()
                .FirstOrDefault(o => o.VirtualTestID == virtualTestId && o.VirtualTestSourceID != 3);
        }

        public VirtualTestProperty GetVirtualTestProperty(int virtualTestId, int roleId, int districtId)
        {
            return virtualTestRepository.GetVirtualTestProperties(virtualTestId, roleId, districtId);
        }

        /// <summary>
        /// Update TestName VirtualTest
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <param name="testName"></param>
        /// <returns></returns>
        public bool UpdateTestName(int virtualTestId, string testName, int? dataSetCategoryId = null)
        {
            var vTest = virtualTestRepository.Select().FirstOrDefault(o => o.VirtualTestID == virtualTestId);
            if (vTest != null)
            {
                vTest.Name = testName;
                vTest.UpdatedDate = DateTime.UtcNow;
                vTest.DatasetCategoryID = dataSetCategoryId;
                virtualTestRepository.Save(vTest);
                return true;
            }
            return false;
        }

        public bool UpdateTestName(int virtualTestId, string testName)
        {
            var vTest = virtualTestRepository.Select().FirstOrDefault(o => o.VirtualTestID == virtualTestId);
            if (vTest != null)
            {
                UpdateAssociatedVirtualTestRetakes(virtualTestId, vTest.Name, testName);

                vTest.Name = testName;
                vTest.UpdatedDate = DateTime.UtcNow;
                virtualTestRepository.Save(vTest);
                return true;
            }
            return false;
        }

        public int CanDeleteVirtualTestByID(int virtualTestId)
        {
            return virtualTestRepository.CanDeleteVirtualTestById(virtualTestId);
        }

        public void DeleteVirtualTestByID(int virtualTestId, int userId, int roleId, out string error)
        {
            virtualTestRepository.DeleteVirtualTestById(virtualTestId, userId, roleId, out error);
        }

        public VirtualQuestionProperties GetVirtualQuestionProperties(int virtualQuestionId)
        {
            return virtualTestRepository.GetVirtualQuestionProperties(virtualQuestionId);
        }

        public IQueryable<VirtualSectionQuestionQtiItem> GetVirtualSectionQuestionQtiItem(int virtualTestId)
        {
            return virtualTestRepository.GetVirtualSectionQuestionQtiItem(virtualTestId);
        }

        public IQueryable<VirtualQuestionWithCorrectAnswer> GetVirtualQuestionWithCorrectAnswer(int virtualTestId)
        {
            return virtualTestRepository.GetVirtualQuestionWithCorrectAnswer(virtualTestId);
        }

        public void RemoveVirtualSection(int virtualSectionId, out string error)
        {
            virtualTestRepository.RemoveVirtualSection(virtualSectionId, out error);
        }

        public void ReassignVirtualQuestionOrder(int virtualTestID)
        {
            virtualTestRepository.ReassignVirtualQuestionOrder(virtualTestID);
        }

        public void AddQtiItemsToVirtualSection(int virtualTestId, string qtiItemIdString, int virtualSectionId, int stateId)
        {
            List<int> idList = ServiceUtil.GetIdListFromIdString(qtiItemIdString);

            //Get the max QuestionOrder of
            var virtualTestQuestionOrder = 0;
            if (virtualQuestionRepository.Select().Any(x => x.VirtualTestID == virtualTestId))
            {
                virtualTestQuestionOrder = virtualQuestionRepository.Select().Where(x => x.VirtualTestID == virtualTestId).Max(x => x.QuestionOrder);
            }

            var virtualSectionQuestionOrder = 0;
            //if (virtualSectionId > 0) //It is now allow to add to default section
            {
                if (virtualSectionQuestionRepository.Select().Any(x => x.VirtualSectionId == virtualSectionId && x.VirtualTestId == virtualTestId))
                {
                    virtualSectionQuestionOrder =
                                                virtualSectionQuestionRepository.Select().Where(x => x.VirtualSectionId == virtualSectionId && x.VirtualTestId == virtualTestId).Max

                            (x => x.Order);
                }
            }

            foreach (var id in idList)
            {
                //if (id > 0 && virtualSectionId > 0)
                if (id > 0) //Now allow  virtualSectionId = 0 for Default Section
                {
                    //Copy from QtiItem to VirtualQuestion and then add to VirtualSectionQuestion
                    var qtiItem = qtiItemRepository.Select().FirstOrDefault(x => x.QTIItemID == id);
                    if (qtiItem != null)
                    {
                        virtualTestQuestionOrder++;
                        var virtualQuestion = new VirtualQuestionData
                        {
                            VirtualTestID = virtualTestId,
                            QTIItemID = qtiItem.QTIItemID,
                            PointsPossible = qtiItem.PointsPossible,
                            QuestionOrder = virtualTestQuestionOrder
                        };
                        //Linkit allows more than one QTIItemID in a VirtualTestID
                        virtualQuestionRepository.Save(virtualQuestion);

                        if (virtualQuestion.VirtualQuestionID > 0)
                        {
                            //Copy QTIItemStateStandard to VirtualQuestionStateStandard
                            //QTIItemLessonOne to VirtualQuestionLessonOne
                            //QTIItemLessonTwo to VirtualQuestionLessonTwo
                            //QTIItemTopic to VirtualQuestionTopic
                            //QTIItemSub to VirtualQuestionSub
                            //QTIItemAnswerScore to VirtualQuestionAnswerScore
                            //QtiItemItemTag to VirtualQuestionItemTag
                            qtiItemRepository.TMCopyStandardsFromQTIItem(virtualQuestion.VirtualQuestionID, id, stateId);

                            //Add VirtualQuestion to Section
                            //Linkit doesn't allows more than one QTIItemID in a VirtualTestID
                            if (!virtualSectionQuestionRepository.Select().Any(x => x.VirtualSectionId == virtualSectionId && x.VirtualQuestionId == virtualQuestion.VirtualQuestionID))
                            {
                                virtualSectionQuestionOrder++;
                                var virtualSectionQuestion = new VirtualSectionQuestion
                                {
                                    VirtualSectionId = virtualSectionId,
                                    VirtualQuestionId =
                                                                         virtualQuestion.VirtualQuestionID,
                                    Order = virtualSectionQuestionOrder
                                };
                                virtualSectionQuestionRepository.Save(virtualSectionQuestion);
                            }
                        }
                    }
                }
            }
            //Reset Order in the section
            virtualTestRepository.ReassignVirtualSectionQuestionOrder(virtualTestId, virtualSectionId);
            //Reset VirtualQuestion.QuestionOrder
            virtualTestRepository.ReassignVirtualQuestionOrder(virtualTestId);
        }

        public void RemoveVirtualQuestion(int virtualQuestionId, out string error)
        {
            virtualTestRepository.RemoveVirtualQuestion(virtualQuestionId, out error);
        }

        public bool CanRemoveVirtualQuestion(string virtualQuestionIds, out string error)
        {
            return virtualTestRepository.CanRemoveVirtualQuestion(virtualQuestionIds, out error);
        }

        public IQueryable<ComplexVirtualQuestionAnswerScore> GetComplexVirtualQuestionAnswerScores(int virtualQuestionId)
        {
            return virtualTestRepository.GetComplexVirtualQuestionAnswerScores(virtualQuestionId);
        }

        public void UpdateComplexVirtualQuestionAnswerScores(int virtualQuestionId, string responseIdentifier, int score,
                                             int subPointsPossible, int pointsPossible, out string error)
        {
            error = string.Empty;
            virtualTestRepository.UpdateComplexVirtualQuestionAnswerScores(virtualQuestionId, responseIdentifier, score, subPointsPossible, pointsPossible, out error);
        }

        public void ReassignVirtualSectionOrder(int virtualTestID)
        {
            virtualTestRepository.ReassignVirtualSectionOrder(virtualTestID);
        }

        public void ReassignVirtualSectionQuestionOrder(int virtualTestId, int virtualSectionId)
        {
            virtualTestRepository.ReassignVirtualSectionQuestionOrder(virtualTestId, virtualSectionId);
        }

        public bool MoveVirtualSectionQuestion(int virtualTestId, int sourceIndex, int sourceSectionId, int targetIndex,
                                        int targetSectionId)
        {
            virtualTestRepository.MoveVirtualSectionQuestion(virtualTestId, sourceIndex, sourceSectionId,
                                                                targetIndex, targetSectionId);
            return true;
        }

        public bool MoveManyVirtualQuestionGroup(int virtualTestId, string virtualQuestionIdString, int? sourceQuestionGroupId, int targetVirtualSectionId, int? targetQuestionGroupId, int targetIndex)
        {
            virtualTestRepository.MoveManyVirtualQuestionGroup(virtualTestId, virtualQuestionIdString, sourceQuestionGroupId, targetVirtualSectionId, targetQuestionGroupId, targetIndex);
            return true;
        }

        public void MoveVirtualSection(int virtualTestId, int sourceIndex, int targetIndex)
        {
            virtualTestRepository.MoveVirtualSection(virtualTestId, sourceIndex, targetIndex);
        }

        public void AddQtiItemToVirtualSection(int virtualTestId, int questionOrder, int orderInSection, int qtiItemId, int virtualSectionId,
            int stateId, int? questionGroupId)
        {
            var qtiItem = qtiItemRepository.Select().FirstOrDefault(x => x.QTIItemID == qtiItemId);
            if (qtiItem == null) return;
            CreateVirtualQuestion(virtualTestId, questionOrder, orderInSection, qtiItem, virtualSectionId, stateId);
        }

        public void AddQtiItemToVirtualSection(int virtualTestId, int virtualSectionId, int stateId, string qtiItemIds, int? questionGroupId)
        {
            qtiItemRepository.AddListQtiItemToVirtualSection(virtualTestId, virtualSectionId, qtiItemIds, questionGroupId);

            //var questionOrder = GetMaxQuestionOrderInVirtualTest(virtualTestId);
            //var orderInSection = GetMaxQuestionOrderInVirtualSection(virtualTestId, virtualSectionId);
            //if (virtualSectionId <= 0) orderInSection = questionOrder;

            //foreach (var qtiItemId in qtiItemIds)
            //{
            //    questionOrder++;
            //    orderInSection++;

            //    var qtiItem = qtiItemRepository.Select().FirstOrDefault(x => x.QTIItemID == qtiItemId);
            //    if (qtiItem == null) return;
            //    CreateVirtualQuestion(virtualTestId, questionOrder, orderInSection, qtiItem, virtualSectionId, stateId);
            //}
        }

        public int GetMaxQuestionOrderInVirtualTest(int virtualTestID)
        {
            var result = 0;
            if (virtualQuestionRepository.Select().Any(x => x.VirtualTestID == virtualTestID))
            {
                result = virtualQuestionRepository.Select().Where(x => x.VirtualTestID == virtualTestID).Max(x => x.QuestionOrder);
            }

            return result;
        }

        public int GetMaxQuestionOrderInVirtualSection(int virtualTestId, int virtualSectionID)
        {
            //Need virtualTestId because incase virtualSectionID = 0
            var result = 0;
            if (virtualSectionQuestionRepository.Select().Any(x => x.VirtualTestId == virtualTestId && x.VirtualSectionId == virtualSectionID))
            {
                result =
                    virtualSectionQuestionRepository.Select().Where(x => x.VirtualTestId == virtualTestId && x.VirtualSectionId == virtualSectionID).Max
                        (x => x.Order);
            }

            return result;
        }

        public void CreateVirtualQuestion(int virtualTestId, int questionOrder, int orderInSection, QTIItemData qtiItem, int virtualSectionID, int stateID)
        {
            if (qtiItem == null) return;

            var virtualQuestion = new VirtualQuestionData
            {
                VirtualTestID = virtualTestId,
                QTIItemID = qtiItem.QTIItemID,
                PointsPossible = qtiItem.PointsPossible,
                QuestionOrder = questionOrder
            };
            virtualQuestionRepository.Save(virtualQuestion);

            //Copy QTIItemStateStandard to VirtualQuestionStateStandard
            //QTIItemLessonOne to VirtualQuestionLessonOne
            //QTIItemLessonTwo to VirtualQuestionLessonTwo
            //QTIItemTopic to VirtualQuestionTopic
            //QTIItemSub to VirtualQuestionSub
            //QTIItemAnswerScore to VirtualQuestionAnswerScore
            //QtiItemItemTag to VirtualQuestionItemTag
            qtiItemRepository.TMCopyStandardsFromQTIItem(virtualQuestion.VirtualQuestionID, qtiItem.QTIItemID, stateID);

            var virtualSectionQuestion = new VirtualSectionQuestion
            {
                VirtualSectionId = virtualSectionID,
                VirtualQuestionId =
                    virtualQuestion.VirtualQuestionID,
                Order = orderInSection
            };
            virtualSectionQuestionRepository.Save(virtualSectionQuestion);
        }

        /// <summary>
        /// Updaet virtual test to new bankId
        /// </summary>
        /// <param name="virtualTestId"></param>
        /// <param name="bankId"></param>
        /// <returns></returns>
        public bool UpdateBankId(int virtualTestId, int bankId, ref bool isDuplicated)
        {
            isDuplicated = false;

            var vTest = virtualTestRepository.Select().FirstOrDefault(o => o.VirtualTestID == virtualTestId);
            if (vTest != null)
            {
                var destTest = virtualTestRepository.Select().FirstOrDefault(o => o.Name == vTest.Name && o.BankID == bankId);
                if (destTest != null)
                {
                    isDuplicated = true;
                    return false;
                }

                vTest.BankID = bankId;
                vTest.UpdatedDate = DateTime.UtcNow;
                virtualTestRepository.Save(vTest);
                return true;
            }
            return false;
        }

        public VirtualTestData GetTestByTestResultID(int testResultID)
        {
            var testResult = testResultRepository.Select().SingleOrDefault(x => x.TestResultId == testResultID);
            if (testResult != null)
            {
                return virtualTestRepository.Select().SingleOrDefault(x => x.VirtualTestID == testResult.VirtualTestId);
            }
            return null;
        }

        public List<GhostQuestion> GetGhostQuestions(int baseVirtualQuestionID)
        {
            var result = virtualTestRepository.GetGhostQuestions(baseVirtualQuestionID);
            return result;
        }

        public int GetMaxQuestionOrder(int virtualTestID)
        {
            var result = virtualTestRepository.GetMaxQuestionOrder(virtualTestID);
            return result ?? 0;
        }

        public void ReassignBaseVirtualSectionQuestionOrder(int virtualTestId, int? virtualSectionId)
        {
            virtualTestRepository.ReassignBaseVirtualSectionQuestionOrder(virtualTestId, virtualSectionId);
        }

        public List<VirtualSectionQuestion> GetBaseQuestions(int virtualTestId, int virtualQuestionId)
        {
            return virtualTestRepository.GetBaseQuestions(virtualTestId, virtualQuestionId);
        }

        public IQueryable<VirtualQuestionS3Object> GetVirtualQuestionToCreateS3Object(int virtualTestId)
        {
            return virtualTestRepository.GetVirtualQuestionToCreateS3Object(virtualTestId);
        }

        public void UpdateBaseVirtualQuestionClone(int oldVirtualTestId, int newVirtualTestId)
        {
            virtualTestRepository.UpdateBaseVirtualQuestionClone(oldVirtualTestId, newVirtualTestId);
        }

        public void MoveManyVirtualSectionQuestion(int virtualTestId, string virtualQuestionIdString, int sourceIndex,
            int sourceSectionId, int targetIndex,
            int targetSectionId, out string message)
        {
            virtualTestRepository.MoveManyVirtualSectionQuestion(virtualTestId, virtualQuestionIdString, sourceIndex, sourceSectionId, targetIndex, targetSectionId, out message);
        }

        public IQueryable<ChoiceVariableVirtualQuestionAnswerScore> GetChoiceVariableVirtualQuestionAnswerScores(
            int virtualQuestionId)
        {
            return virtualTestRepository.GetChoiceVariableVirtualQuestionAnswerScores(virtualQuestionId);
        }

        public bool ConflictNameInBank(VirtualTestData virtualTest)
        {
            if (virtualTest == null) return false;
            var query =
                virtualTestRepository.Select().Where(o => o.BankID == virtualTest.BankID && o.Name == virtualTest.Name);
            var result = true;
            if (virtualTest.VirtualTestID == 0)
            {
                result = query.Any();
            }
            else
            {
                result = query.Any(o => o.VirtualTestID != virtualTest.VirtualTestID);
            }

            return result;
        }

        public IQueryable<ListItem> GetFormByFormBankId(int bankId)
        {
            var result = virtualTestRepository.Select().Where(x => x.BankID.Equals(bankId) && x.VirtualTestSourceID == (int)VirtualTestSourceEnum.Legacy);
            return result.Select(x => new ListItem { Id = x.VirtualTestID, Name = x.Name }).OrderBy(x => x.Name);
        }

        public IQueryable<VirtualTestCustomScore> GetVirtualTestByFormBankId(int bankId)
        {
            var result = virtualTestRepository.Select().Where(x => x.BankID.Equals(bankId) && x.VirtualTestSourceID == (int)VirtualTestSourceEnum.Legacy);
            return result.Select(s => new VirtualTestCustomScore
            {
                VirtualTestCustomScoreId = s.VirtualTestID,
                Name = s.Name,
                DatasetCategoryID = s.DatasetCategoryID,
                DataSetOriginID = s.OriginalTestID
            }).OrderBy(x => x.Name);
        }

        public List<int> GetDataLockerBank(List<int> banksID)
        {
            var result = virtualTestRepository.Select()
                .Where(x => banksID.Contains(x.BankID) && x.VirtualTestSourceID == (int)VirtualTestSourceEnum.Legacy && x.VirtualTestType == (int)VirtualTestSourceEnum.EtsPregenerated)
                .GroupBy(m => m.BankID)
                .Select(m => m.Key).ToList();

            return result;
        }

        public AlgorithmicMaxPoint GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(int virtualQuestionId, int qtiItemId)
        {
            var result = virtualTestRepository.GetMaxPointAlgorithmicByVirtualQuestionIDAndQTIItemID(virtualQuestionId, qtiItemId);
            return result;
        }

        public void SaveVirtualTestMeta(VirtualTestMeta item)
        {
            virtualTestMetaRepository.Save(item);
        }

        public void DeleteVirtualTestMeta(VirtualTestMeta item)
        {
            virtualTestMetaRepository.Delete(item);
        }

        public bool IsCustomItemNaming(int virtualTestId)
        {
            var data =
                virtualTestMetaRepository.Select()
                    .Where(x => x.VirtualTestID == virtualTestId && x.Name == Constanst.IsCustomItemNaming);
            if (data.Any())
                return true;
            return false;
        }

        /// <summary>
        /// Return bankID, testID
        /// </summary>
        /// <param name="testId"></param>
        /// <returns></returns>
        public List<Tuple<int, int>> GetBankIds(List<int> testId)
        {
            var result = virtualTestRepository.Select()
                .Select(x => new { x.VirtualTestID, x.BankID })
                .FilterOnLargeSet(entry => entry, (subset) =>
                {
                    return entry => subset.Contains(entry.VirtualTestID);
                }, testId.Distinct())
                .Select(m => new Tuple<int, int>(m.BankID, m.VirtualTestID)).ToList();
            return result;
        }

        public string GetRubricDescription(int testId)
        {
            return virtualTestRepository.GetVirtualTestByID(testId).Instruction;
        }

        public List<VirtualTestOrder> GetVirtualTestOrders(int districtId)
        {
            var result = virtualTestRepository.GetVirtualTestOrders(districtId);
            return result;
        }

        public VirtualTestData GetVirtualTestById(int virtualTestId)
        {
            return virtualTestRepository.GetVirtualTestByID(virtualTestId);
        }

        public VirtualTestTypeEnum GetVirtualTestTypeById(int virtualTestId)
        {
            var virtualTest = virtualTestRepository.GetVirtualTestByID(virtualTestId);
            var type = VirtualTestTypeEnum.Unknown;

            if (virtualTest != null)
            {
                if (virtualTest.VirtualTestSourceID == (int)VirtualTestSourceEnum.LinkitPublished || virtualTest.VirtualTestSourceID == (int)VirtualTestSourceEnum.LinkitCustom)
                {
                    type = VirtualTestTypeEnum.Linkit;
                }
                else if (virtualTest.VirtualTestSourceID == (int)VirtualTestSourceEnum.Legacy && virtualTest.VirtualTestType == (int)VirtualTestSourceEnum.EtsPregenerated)
                {
                    type = VirtualTestTypeEnum.DataLocker;
                }
                else
                {
                    type = VirtualTestTypeEnum.NonLinkit;
                }
            }

            return type;
        }

        public int GetDataSetCategoryIDByVirtualTestID(int virtualTestId)
        {
            var virtualTest = virtualTestRepository.GetVirtualTestByID(virtualTestId);
            return virtualTest != null ? virtualTest.DatasetCategoryID.GetValueOrDefault() : 0;
        }

        public int CountOpendedQuestionPerTest(int virtualtestId)
        {
            return virtualTestRepository.CountOpendedQuestionPerTest(virtualtestId);
        }

        public void DeleteVirtualQuestionBranchingByTestID(int virtualTestId)
        {
            virtualTestRepository.DeleteVirtualQuestionBranchingByTestID(virtualTestId);
        }

        public void DeleteVirtualSectionBranchingByTestID(int virtualTestId)
        {
            virtualTestRepository.DeleteVirtualSectionBranchingByTestID(virtualTestId);
        }

        public bool GetNumberQuestionsByTestId(int virtualTestId)
        {
            var objTest = virtualTestRepository.Select().FirstOrDefault(o => o.VirtualTestID == virtualTestId);
            if (objTest != null)
                return objTest.IsNumberQuestions.GetValueOrDefault();
            return false;
        }

        public bool UpdateNumberQuestionByTestId(int virtualTestId, bool isNumberQuestion)
        {
            var vTest = virtualTestRepository.Select().FirstOrDefault(o => o.VirtualTestID == virtualTestId);
            if (vTest != null)
            {
                vTest.IsNumberQuestions = isNumberQuestion;
                vTest.UpdatedDate = DateTime.UtcNow;
                virtualTestRepository.Save(vTest);
                return true;
            }
            return false;
        }

        public void DeleteVirtualQuestionBranchingAlgorithmByTestID(int virtualTestId)
        {
            virtualTestRepository.DeleteVirtualQuestionBranchingAlgorithmByTestID(virtualTestId);
        }

        public bool GetIsOverwriteValue(int districtId)
        {
            var overWriteDistrictDecode = _districtDecodeService.GetDistrictDecodesOfSpecificDistrictByLabel(districtId, TextConstants.IS_OVERWRITE_RESULTS).FirstOrDefault();
            var value = false;

            if (overWriteDistrictDecode != null)
            {
                bool.TryParse(overWriteDistrictDecode.Value, out value);
            }
            else
            {
                var isOverwriteResults = _configurationService.GetConfigurationByKey(TextConstants.IS_OVERWRITE_RESULTS);
                if (isOverwriteResults != null && !string.IsNullOrEmpty(isOverwriteResults.Value))
                    bool.TryParse(isOverwriteResults.Value, out value);
            }
            return value;
        }
        public bool CheckHasTestResultByTestId(int testId)
        {
            var testResults = testResultRepository.Select().FirstOrDefault(x => x.VirtualTestId == testId);
            return testResults != null;
        }

        public List<VirtualTestData> GetSurveyTestByBank(int bankId)
        {
            return virtualTestRepository.Select().Where(p => p.BankID == bankId).ToList();
        }

        public void UpdateVirtualSection(int virtualQuestionId, int virtualSectionId)
        {
            var errorMessage = string.Empty;

            var virtualQuestion = virtualQuestionRepository.Select().FirstOrDefault(x => x.VirtualQuestionID == virtualQuestionId);
            var maxQuestionSection = virtualSectionQuestionRepository.Select().Where(x => x.VirtualSectionId == virtualSectionId)
                .Select(o => o.Order).ToList();
            var maxQuestionSectionIndex = 0;
            if (maxQuestionSection.Count > 0)
                maxQuestionSectionIndex = maxQuestionSection.Max();

            MoveManyVirtualQuestionGroup(virtualQuestion.VirtualTestID, virtualQuestionId.ToString(), null, virtualSectionId, null, 1);
            MoveManyVirtualSectionQuestion(virtualQuestion.VirtualTestID, virtualQuestionId.ToString(), 0, 0, maxQuestionSectionIndex, virtualSectionId, out errorMessage);
        }

        public List<VirtualTestIncludeQtiItemDto> GetVirtualTestsByQtiItems(IEnumerable<int?> qtiItemIds)
        {
            var vqData = virtualQuestionRepository.Select().Where(x => qtiItemIds.Contains(x.QTIItemID)).ToList();
            var vqTestIds = vqData.Select(x => x.VirtualTestID).Distinct().ToArray();

            var data = virtualTestRepository.Select()
                .Where(x => vqTestIds.Contains(x.VirtualTestID))
                .Select(x => new
                {
                    x.VirtualTestID,
                    x.Name
                })
                .ToList();

            return data.Select(x => new VirtualTestIncludeQtiItemDto
            {
                Name = x.Name,
                VirtualTestID = x.VirtualTestID,
            }).ToList();
        }
        public void ChangePositionVirtualSection(int virtualTestId, int sourceIndex, int targetIndex)
        {
            virtualTestRepository.ChangePositionVirtualSection(virtualTestId, sourceIndex, targetIndex);
        }       
        public List<ImageIndexInQuestion> GetImageIndexInComplex(int virtualTestId)
        {
            var result = new List<ImageIndexInQuestion>();
            var complexs = virtualTestRepository.GetConstructedResponseQuestions(virtualTestId);
            var xmlContentSerializer = new XmlContentSerializer();
            foreach (var complex in complexs)
            {
                var imageIndexs = xmlContentSerializer.GetImageIndexInQuestion(complex.XmlContent);
                result.AddRange(imageIndexs.Select(x => new ImageIndexInQuestion()
                {
                    VirtualQuestionId = complex.VirtualQuestionId,
                    ResponseIdentifier = x.Key,
                    Index = x.Value
                }));
            }
            return result;
        }
        public List<ConstructedResponseQuestion> GetConstructedResponseQuestion(int virtualTestId)
        {            
            return virtualTestRepository.GetConstructedResponseQuestions(virtualTestId);            
        }
        public IList<int> GetVirtualTestIdsByQTIItemId(int qtiItemId)
        {
            return virtualQuestionRepository.Select()
                .Where(x => x.QTIItemID == qtiItemId)
                .Select(x => x.VirtualTestID).Distinct().ToList();
        }
        public bool IsVirtualTestHasRetake(int virtualTestId)
        {
            return virtualTestRepository.Select()
                .Any(x => x.OriginalTestID.HasValue && x.OriginalTestID == virtualTestId);
        }

        public VirtualTestData GetRetakeVirtualTestByTestName(int originalTestId, string testName)
        {
            return virtualTestRepository.Select().FirstOrDefault(x => x.OriginalTestID == originalTestId && x.Name.Equals(testName));
        }
        public void UpdateAssociatedVirtualTestRetakes(int virtualTestId, string oldOriginalTestName, string newOriginalTestName)
        {
            var virtualTestsRetake = virtualTestRepository.Select()
                .Where(x => x.OriginalTestID.HasValue && x.OriginalTestID == virtualTestId).ToList();

            foreach (var test in virtualTestsRetake)
            {
                test.Name = test.Name.Replace(oldOriginalTestName, newOriginalTestName);
                test.UpdatedDate = DateTime.UtcNow;
                virtualTestRepository.Save(test);
            }
        }

        public void ClonePBSForTestRetake(int oldTestId, int newTestId)
        {
            virtualTestRepository.ClonePBSForTestRetake(oldTestId, newTestId);
        }

        public GetTestByAdvanceFilter GetTestByAdvanceFilter(GetTestByAdvanceFilterRequest request)
        {
            return virtualTestRepository.GetTestByAdvanceFilter(request);
        }

        public string ProcessExportTestProperty(int districtID, string strVirtualTestID)
        {
            var data = virtualTestRepository.GetTestPropertyToExportByVirtualTestIDs(districtID, strVirtualTestID);
            var dataExport = data.DataExport;
            if (!dataExport.Any()) return String.Empty;            

            var choiceVariableAnswerScores = new List<ChoiceVariableVirtualQuestionAnswerScore>();

            var complexAnswerScores = new List<ComplexVirtualQuestionAnswerScore>();

            var rubricAnswerScores = new List<RubricQuestionCategoryDto>();

            var strChoiceVariableVirtualQuestionID = string.Join(",", dataExport.Where(x => x.QTISchemaID == (int)QtiSchemaEnum.ChoiceMultipleVariable || x.QTISchemaID == (int)QtiSchemaEnum.TextEntry
                || x.QTISchemaID == (int)QtiSchemaEnum.TextHotSpot || x.QTISchemaID == (int)QtiSchemaEnum.ImageHotSpot
                || x.QTISchemaID == (int)QtiSchemaEnum.TableHotSpot || x.QTISchemaID == (int)QtiSchemaEnum.NumberLineHotSpot || x.QTISchemaID == (int)QtiSchemaEnum.Complex).Select(x => x.VirtualQuestionID));
            choiceVariableAnswerScores.AddRange(virtualTestRepository.GetChoiceVariableVirtualQuestionAnswerScoresByVirtualQuestionIDs(strChoiceVariableVirtualQuestionID));

            var strComplexVirtualQuestionID = string.Join(",", dataExport.Where(x => x.QTISchemaID == (int)QtiSchemaEnum.Complex).Select(x => x.VirtualQuestionID));
            complexAnswerScores.AddRange(virtualTestRepository.GetComplexVirtualQuestionAnswerScoresByVirtualQuestionIDs(strComplexVirtualQuestionID));

            var arrRubricVirtualQuestionID = data.DataExport.Where(x => x.IsRubricBasedQuestion == true).Select(x => x.VirtualQuestionID).ToArray();
            rubricAnswerScores.AddRange(_rubricQuestionCategoryService.GetRubicQuestionCategoriesByVirtualQuestionIds(arrRubricVirtualQuestionID).ToList());

            dataExport.AsParallel().WithDegreeOfParallelism(20).ForAll(item =>
            {
                item.AnswerKey = _exportAnswerKeyService.GetAnswerKey(new AnswerKeyData
                {
                    XmlContent = item.XmlContent,
                    ResponseProcessing = item.ResponseProcessing,
                    QTISchemaID = item.QTISchemaID,
                    IsRubricBasedQuestion = item.IsRubricBasedQuestion,
                    CorrectAnswer = item.CorrectAnswer,
                    PointsPossible = item.PointsPossible.HasValue ? item.PointsPossible.Value : 0,
                    AlgorithmicExpression = item.AlgorithmicExpression,
                    VirtualQuestionID = item.VirtualQuestionID,
                    ChoiceVariableAnswerScores = choiceVariableAnswerScores,
                    ComplexAnswerScores = complexAnswerScores,
                    RubricAnswerScores = rubricAnswerScores
                });
                if (item.QTISchemaID == (int)QTISchemaEnum.ExtendedText)
                {
                    var drawable = XmlUtils.IsDrawable(item.XmlContent);
                    if (drawable)
                    {
                        item.QTISchemaName = "Drawing Response";
                    }
                }
            });

            return ExportTestPropertyToCSV(dataExport, data.CategoriesName);
        }

        private string ExportTestPropertyToCSV(IList<TestPropertyExportData> testData,string categoryName)
        {
            var ms = new MemoryStream();
            TextWriter textWriter = null;
            CsvWriter csvWriter = null;
            try
            {
                textWriter = new StreamWriter(ms);
                csvWriter = new CsvWriter(textWriter);
                var streamReader = new StreamReader(ms);
                WriteHeaderTestProperty(csvWriter, categoryName);
                foreach (var item in testData)
                {
                    WriteGeneralInformationTestProperty(csvWriter, item);
                    csvWriter.NextRecord();
                }
                textWriter.Flush();
                ms.Position = 0;
                return streamReader.ReadToEnd();
            }
            finally
            {
                if (textWriter != null) textWriter.Dispose();
                if (csvWriter != null) csvWriter.Dispose();
            }
        }

        private void WriteHeaderTestProperty(CsvWriter csvWriter, string categoryName)
        {            
            csvWriter.WriteField("Test_Name");
            csvWriter.WriteField("Test_ID");
            csvWriter.WriteField("Test_Created_On");
            csvWriter.WriteField("Test_Updated_On");
            csvWriter.WriteField("Test_Created_By");
            csvWriter.WriteField("Test_Question_Count");
            csvWriter.WriteField("Test_Total_Points_Possible");
            csvWriter.WriteField("Test_Results_Count");
            csvWriter.WriteField("Earliest_Result_Date");
            csvWriter.WriteField("Most_Recent_Result_Date");
            csvWriter.WriteField("Test_Category");
            csvWriter.WriteField("Test_Bank_Name");
            csvWriter.WriteField("Test_Bank_Subject");
            csvWriter.WriteField("Test_Bank_Grade");
            csvWriter.WriteField("Interview-Style_Assessment");
            csvWriter.WriteField("Question_Number");
            csvWriter.WriteField("Passage_Number");
            csvWriter.WriteField("QTI_Item_ID");
            csvWriter.WriteField("QTI_Item_Title");            
            csvWriter.WriteField("Item_Type");
            csvWriter.WriteField("Item_Points_Possible");
            csvWriter.WriteField("Standard_Numbers");
            csvWriter.WriteField("Correct_Answer");          
            csvWriter.WriteField(categoryName);
            csvWriter.NextRecord();
        }

        private void WriteGeneralInformationTestProperty(CsvWriter csvWriter, TestPropertyExportData testObject)
        {
            csvWriter.WriteField(testObject.TestName);
            csvWriter.WriteField(testObject.VirtualTestID);
            if (testObject.CreatedDate.HasValue)
            {
                csvWriter.WriteField(testObject.CreatedDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            if (testObject.UpdatedDate.HasValue)
            {
                csvWriter.WriteField(testObject.UpdatedDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(testObject.CreatedBy);
            csvWriter.WriteField(testObject.QuestionCount);
            csvWriter.WriteField(testObject.TotalPointsPossible);
            csvWriter.WriteField(testObject.TestResultCount);
            if (testObject.EarliestResultDate.HasValue)
            {
                csvWriter.WriteField(testObject.EarliestResultDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            if (testObject.MostRecentResultDate.HasValue)
            {
                csvWriter.WriteField(testObject.MostRecentResultDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(testObject.TestCategory);
            csvWriter.WriteField(testObject.BankName);
            csvWriter.WriteField(testObject.BankSubject);
            csvWriter.WriteField(testObject.BankGrade);
            if (testObject.InterviewStyleAssessment.HasValue)
            {
                csvWriter.WriteField(testObject.InterviewStyleAssessment.Value);
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            if (testObject.QuestionNumber.HasValue)
            {
                csvWriter.WriteField(testObject.QuestionNumber.Value);
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(testObject.PassageNumber ?? string.Empty);
            if (testObject.QTIItemID.HasValue)
            {
                csvWriter.WriteField(testObject.QTIItemID.Value);
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(testObject.QTIItemTitle ?? string.Empty);            
            csvWriter.WriteField(testObject.QTISchemaName ?? string.Empty);
            if (testObject.PointsPossible.HasValue)
            {
                csvWriter.WriteField(testObject.PointsPossible.Value);
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(testObject.StandardNumbers ?? string.Empty);
            csvWriter.WriteField(testObject.AnswerKey);
            csvWriter.WriteField(testObject.VirtualQuestionTags ?? string.Empty);
        }

        public string ProcessExportTestLibraryByDistrictID(int districtID)
        {
            var data = virtualTestRepository.GetTestLibraryToExportByDistrictID(districtID);

            if (!data.Any()) return String.Empty;

            return ExportTestLibraryToCSV(data);
        }

        private string ExportTestLibraryToCSV(IList<TestLibraryExportData> itemLibraryData)
        {
            var ms = new MemoryStream();
            TextWriter textWriter = null;
            CsvWriter csvWriter = null;
            try
            {
                textWriter = new StreamWriter(ms);
                csvWriter = new CsvWriter(textWriter);
                var streamReader = new StreamReader(ms);
                WriteHeaderTestLibrary(csvWriter);
                foreach (var item in itemLibraryData)
                {
                    WriteGeneralInformationTestLibrary(csvWriter, item);
                    csvWriter.NextRecord();
                }
                textWriter.Flush();
                ms.Position = 0;
                return streamReader.ReadToEnd();
            }
            finally
            {
                if (textWriter != null) textWriter.Dispose();
                if (csvWriter != null) csvWriter.Dispose();
            }
        }

        private void WriteHeaderTestLibrary(CsvWriter csvWriter)
        {
            csvWriter.WriteField("Test_Name");
            csvWriter.WriteField("Test_ID");
            csvWriter.WriteField("Test_Created_On");
            csvWriter.WriteField("Test_Updated_On");
            csvWriter.WriteField("Test_Created_By");
            csvWriter.WriteField("Test_Question_Count");
            csvWriter.WriteField("Test_Total_Points_Possible");
            csvWriter.WriteField("Test_Results_Count");
            csvWriter.WriteField("Earliest_Result_Date");
            csvWriter.WriteField("Most_Recent_Result_Date");
            csvWriter.WriteField("Test_Category");
            csvWriter.WriteField("Interview-Style_Assessment");
            csvWriter.WriteField("Bank_Name");
            csvWriter.WriteField("Bank_Grade");
            csvWriter.WriteField("Bank_Subject");
            csvWriter.NextRecord();
        }

        private void WriteGeneralInformationTestLibrary(CsvWriter csvWriter, TestLibraryExportData itemLibraryObject)
        {
            csvWriter.WriteField(itemLibraryObject.TestName);
            csvWriter.WriteField(itemLibraryObject.VirtualTestID);
            if (itemLibraryObject.CreatedDate.HasValue)
            {
                csvWriter.WriteField(itemLibraryObject.CreatedDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            if (itemLibraryObject.UpdatedDate.HasValue)
            {
                csvWriter.WriteField(itemLibraryObject.UpdatedDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(itemLibraryObject.CreatedBy);
            csvWriter.WriteField(itemLibraryObject.QuestionCount);
            csvWriter.WriteField(itemLibraryObject.TotalPointsPossible);
            csvWriter.WriteField(itemLibraryObject.TestResultCount);
            if (itemLibraryObject.EarliestResultDate.HasValue)
            {
                csvWriter.WriteField(itemLibraryObject.EarliestResultDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            if (itemLibraryObject.MostRecentResultDate.HasValue)
            {
                csvWriter.WriteField(itemLibraryObject.MostRecentResultDate.Value.ToString("MM/dd/yyyy"));
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(itemLibraryObject.TestCategory);
            if (itemLibraryObject.InterviewStyleAssessment.HasValue)
            {
                csvWriter.WriteField(itemLibraryObject.InterviewStyleAssessment.Value);
            }
            else
            {
                csvWriter.WriteField(string.Empty);
            }
            csvWriter.WriteField(itemLibraryObject.BankName);
            csvWriter.WriteField(itemLibraryObject.BankGrade);
            csvWriter.WriteField(itemLibraryObject.BankSubject);
        }
    }
}
