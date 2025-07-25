using System.Collections.Generic;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.DataFileUpload;
using LinkIt.BubbleSheetPortal.Models.TestMaker;
using LinkIt.BubbleSheetPortal.Services.TestMaker;
using S3Library;
using System.Text.RegularExpressions;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestMaker;
using CsvHelper;
using System.Web;
using System.Net.Http;
using LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker;
using LinkIt.BubbleSheetPortal.Models.Algorithmic;
using System.Xml.Serialization;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Models.DTOs;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIITemService
    {
        private readonly IQTIItemRepository _qtiItemQtiItemRepository;
        private readonly IAlgorithmQTIItemGradingRepository _algorithmicQtiItemRepository;
        private readonly IQTIItemHistoryRepository _qtiItemQtiItemHistoryRepository;
        private readonly IReadOnlyRepository<QTI3pItem> _itemRepository;
        private readonly IInsertDeleteRepository<QTIItemSub> _qtiItemSubRepository;
        private readonly IReadOnlyRepository<QTIItemSub> _qtiItemSubReadOnlyRepository;
        private readonly IInsertDeleteRepository<QTIItemAnswerScore> _qtiItemAnswerScoreInsertDeleteRepository;
        private readonly IReadOnlyRepository<QTIItemAnswerScore> _qtiItemAnswerScoreRepository;
        private readonly IRepository<QTIItemTopic> _qtiItemTopicRepository;
        private readonly IRepository<QTIItemLessonOne> _qtiItemLessonOneRepository;
        private readonly IRepository<QTIItemLessonTwo> _qtiItemLessonTwoRepository;
        private readonly IQTIItemStateStandardRepository _qtiItemStateStandardRepository;
        private readonly IRepository<QtiRefObject> _qtiRefObjectRepository;
        private readonly IRepository<QtiRefObjectHistory> _qtiRefObjectHistoryRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IReadOnlyRepository<PassageGradeQti> _passageGradeQtiRepository;
        private readonly IRepository<VirtualQuestionData> _virtualQuestionRepository;
        private readonly IRepository<VirtualQuestionSub> _virtualQuestionSubRepository;
        private readonly IRepository<VirtualQuestionAnswerScore> _virtualQuestionAnswerScoreRepository;
        public readonly QTIItemConvert _qTIItemConvert;
        private readonly IRepository<QTI3pItemToPassage> _qTI3pItemToPassageRepository;
        private readonly IRepository<QtiItemQTI3pPassage> _qtiItemQTI3pPassageRepository;
        private readonly IQTIGroupRepository _qtiGroupRepository;
        private readonly IRepository<DataFileUploadLog> _dataFileUploadLogRepository;
        private readonly IRepository<QtiItemItemTag> _qtiItemItemTagRepository;
        private readonly IRepository<DataFileUploadResourceLog> _dataFileUploadResourceLogRepository;
        private readonly IRepository<DataFileUploadPassage> _dataFileUploadPassageRepository;
        private readonly IRepository<QtiItemDataFileUploadPassage> _qtiItemDataFileUploadPassageRepository;
        private readonly IRepository<QTI3pItem> _qTI3pItemRepository;
        private readonly IRepository<QTI3pPassage> _qTI3pPassageRepository;
        private readonly IMasterStandardRepository _masterStandardRepository;
        private readonly IReadOnlyRepository<QTI3pDOK> _qTi3pDOKRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<QTI3pItemStateStandard> _qTI3pItemStateStandardRepository;
        private readonly IRepository<QTI3pItemDOK> _qTI3pItemDOKRepository;
        private readonly IRepository<QTI3pPassageProgress> _qti3PPassageProgressRepository;
        private readonly IRepository<Qti3pProgressPassageGenre> _qti3PProgressPassageGenreRepository;
        private readonly IRepository<Qti3pProgressPassageType> _qti3PProgressPassageTypeResRepository;
        private readonly IReadOnlyRepository<Grade> _gradeRepository;
        private readonly IRepository<QTI3pTextType> _textTypeRepository;
        private readonly IReadOnlyRepository<QTI3pWordCount> _wordCountRepository;
        private readonly IRepository<ItemTag> _itemTagRepository;
        private readonly IReadOnlyRepository<QTI3pFleschKinkaid> _fleschKinkaidRepository;
        private readonly IReadOnlyRepository<QTI3pContentArea> _contentAreaRepository;
        private readonly IReadOnlyRepository<QTI3pSubject> _subjectRepository;
        private readonly IReadOnlyRepository<QTI3pBlooms> _bloomRepository;
        private readonly IReadOnlyRepository<AnswerSubData> _answerSubRepository;
        private readonly IReadOnlyRepository<QtiOnlineTestSessionAnswerSubData> _testSessionAnswerSubRepository;
        private readonly IAnswerRepository _answerRepository;
        private readonly IRepository<QtiOnlineTestSessionAnswer> _testSessionAnswerRepository;
        private readonly IReadOnlyRepository<QTI3pDifficulty> _diffRepository;
        private readonly IRepository<QTI3pItemDependencyDeleteHistory> _qTI3pItemDpDelHistoryRepository;
        private readonly IRepository<QTI3pItemUpdateHistory> _qTI3pItemUpdateHistoryRepository;
        private readonly IMultiPartQtiItemExpressionRepository _multiPartQtiItemExpressionRepository;
        private readonly IS3Service _s3Service;
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IVirtualTestRepository _virtualTestRepository;
        private readonly IRubricModuleQueryService _rubricQuestionCategoryService;
        private readonly ExportAnswerKeyService _exportAnswerKeyService;

        public QTIITemService(IQTIItemRepository qtiItemRepository,
                              IAlgorithmQTIItemGradingRepository algorithmicQtiItemRepository,
                              IQTIItemHistoryRepository qTIItemHistoryRepository,
                              IReadOnlyRepository<QTI3pItem> itemRepository,
                              IInsertDeleteRepository<QTIItemSub> qtiItemSubRepository,
                              IReadOnlyRepository<QTIItemAnswerScore> qtiItemAnswerScoreRepository,
                              IInsertDeleteRepository<QTIItemAnswerScore> qtiItemAnswerScoreInsertDeleteRepository,
                              IRepository<QTIItemTopic> qtiItemTopicRepository,
                              IRepository<QTIItemLessonOne> qtiItemLessonOneRepository,
                              IRepository<QTIItemLessonTwo> qtiItemLessonTwoRepository,
                              IQTIItemStateStandardRepository qtiItemStateStandardRepository,
                              IRepository<QtiRefObject> qtiRefObjectRepository,
                              IRepository<QtiRefObjectHistory> qtiRefObjectHistoryRepository,
                              IStateRepository stateRepository,
                              IReadOnlyRepository<QTIItemSub> qtiItemSubReadOnlyRepository,
                              IReadOnlyRepository<PassageGradeQti> passageGradeQtiRepository,
                              IRepository<VirtualQuestionData> virtualQuestionRepository,
                              IRepository<VirtualQuestionSub> virtualQuestionSubRepository,
                              IRepository<VirtualQuestionAnswerScore> virtualQuestionAnswerScoreRepository,
                              QTIItemConvert qTIItemConvert,
                              IRepository<QTI3pItemToPassage> qTI3pItemToPassageRepository,
                              IRepository<QtiItemQTI3pPassage> qtiItemQTI3pPassageRepository,
                              IQTIGroupRepository qtiGroupRepository,
                              IRepository<QtiItemItemTag> qtiItemItemTagRepository,
                              IRepository<DataFileUploadLog> dataFileUploadLogRepository,
                              IRepository<DataFileUploadResourceLog> dataFileUploadResourceLogRepository,
                              IRepository<DataFileUploadPassage> dataFileUploadPassageRepository,
                              IRepository<QtiItemDataFileUploadPassage> qtiItemDataFileUploadPassageRepository,
                              IRepository<QTI3pItem> qTI3pItemRepository,
                              IRepository<QTI3pPassage> qTI3pPassageRepository,
                              IMasterStandardRepository masterStandardRepository,
                              IReadOnlyRepository<QTI3pDOK> qTi3pDOKRepository,
                              IRepository<User> userRepository,
                              IRepository<QTI3pItemStateStandard> qTI3pItemStateStandardRepository,
                              IRepository<QTI3pItemDOK> qTI3pItemDOKRepository,
                              IRepository<QTI3pPassageProgress> qti3PPassageProgressRepository,
                              IRepository<Qti3pProgressPassageGenre> qti3PProgressPassageGenreRepository,
                              IRepository<Qti3pProgressPassageType> qti3PProgressPassageTypeResRepository,
                              IReadOnlyRepository<Grade> gradeRepository,
                              IRepository<QTI3pTextType> textTypeRepository,
                              IReadOnlyRepository<QTI3pWordCount> wordCountRepository,
                              IRepository<ItemTag> itemTagRepository,
                              IReadOnlyRepository<QTI3pFleschKinkaid> fleschKinkaidRepository,
                              IReadOnlyRepository<QTI3pContentArea> contentAreaRepository,
                              IReadOnlyRepository<QTI3pSubject> subjectRepository,
                              IReadOnlyRepository<QTI3pBlooms> bloomRepository,
                              IReadOnlyRepository<AnswerSubData> answerSubRepository,
                              IReadOnlyRepository<QtiOnlineTestSessionAnswerSubData> testSessionAnswerSubRepository,
                              IAnswerRepository answerRepository,
                              IRepository<QtiOnlineTestSessionAnswer> testSessionAnswerRepository,
                              IReadOnlyRepository<QTI3pDifficulty> diffRepository,
                              IRepository<QTI3pItemDependencyDeleteHistory> qTI3pItemDpDelHistoryRepository,
                              IRepository<QTI3pItemUpdateHistory> qTI3pItemUpdateHistoryRepository,
                              IMultiPartQtiItemExpressionRepository multiPartQtiItemExpressionRepository
, IS3Service s3Service, IVirtualTestRepository virtualTestRepository, IRubricModuleQueryService rubricQuestionCategoryService, ExportAnswerKeyService exportAnswerKeyService)
        {
            _qtiItemQtiItemRepository = qtiItemRepository;
            _algorithmicQtiItemRepository = algorithmicQtiItemRepository;
            _qtiItemQtiItemHistoryRepository = qTIItemHistoryRepository;
            _itemRepository = itemRepository;
            _qtiItemSubRepository = qtiItemSubRepository;
            _qtiItemAnswerScoreRepository = qtiItemAnswerScoreRepository;
            _qtiItemAnswerScoreInsertDeleteRepository = qtiItemAnswerScoreInsertDeleteRepository;
            _qtiItemTopicRepository = qtiItemTopicRepository;
            _qtiItemLessonOneRepository = qtiItemLessonOneRepository;
            _qtiItemLessonTwoRepository = qtiItemLessonTwoRepository;
            _qtiItemStateStandardRepository = qtiItemStateStandardRepository;
            _qtiRefObjectRepository = qtiRefObjectRepository;
            _qtiRefObjectHistoryRepository = qtiRefObjectHistoryRepository;
            _stateRepository = stateRepository;
            _qtiItemSubReadOnlyRepository = qtiItemSubReadOnlyRepository;
            _passageGradeQtiRepository = passageGradeQtiRepository;
            _virtualQuestionRepository = virtualQuestionRepository;
            _virtualQuestionSubRepository = virtualQuestionSubRepository;
            _virtualQuestionAnswerScoreRepository = virtualQuestionAnswerScoreRepository;
            _qTIItemConvert = qTIItemConvert;
            _qTI3pItemToPassageRepository = qTI3pItemToPassageRepository;
            _qtiItemQTI3pPassageRepository = qtiItemQTI3pPassageRepository;
            _qtiGroupRepository = qtiGroupRepository;
            _dataFileUploadLogRepository = dataFileUploadLogRepository;
            _qtiItemItemTagRepository = qtiItemItemTagRepository;
            _dataFileUploadResourceLogRepository = dataFileUploadResourceLogRepository;
            _dataFileUploadPassageRepository = dataFileUploadPassageRepository;
            _qtiItemDataFileUploadPassageRepository = qtiItemDataFileUploadPassageRepository;
            _qTI3pItemRepository = qTI3pItemRepository;
            _qTI3pPassageRepository = qTI3pPassageRepository;
            _masterStandardRepository = masterStandardRepository;
            _qTi3pDOKRepository = qTi3pDOKRepository;
            _qTI3pItemDOKRepository = qTI3pItemDOKRepository;
            _userRepository = userRepository;
            _qTI3pItemStateStandardRepository = qTI3pItemStateStandardRepository;
            _qti3PPassageProgressRepository = qti3PPassageProgressRepository;
            _qti3PProgressPassageGenreRepository = qti3PProgressPassageGenreRepository;
            _qti3PProgressPassageTypeResRepository = qti3PProgressPassageTypeResRepository;
            _gradeRepository = gradeRepository;
            _textTypeRepository = textTypeRepository;
            _wordCountRepository = wordCountRepository;
            _itemTagRepository = itemTagRepository;
            _fleschKinkaidRepository = fleschKinkaidRepository;
            _contentAreaRepository = contentAreaRepository;
            _subjectRepository = subjectRepository;
            _bloomRepository = bloomRepository;
            _answerSubRepository = answerSubRepository;
            _testSessionAnswerSubRepository = testSessionAnswerSubRepository;
            _answerRepository = answerRepository;
            _testSessionAnswerRepository = testSessionAnswerRepository;
            _diffRepository = diffRepository;
            _qTI3pItemDpDelHistoryRepository = qTI3pItemDpDelHistoryRepository;
            _qTI3pItemUpdateHistoryRepository = qTI3pItemUpdateHistoryRepository;
            _multiPartQtiItemExpressionRepository = multiPartQtiItemExpressionRepository;
            _s3Service = s3Service;
            _virtualTestRepository = virtualTestRepository;
            _rubricQuestionCategoryService = rubricQuestionCategoryService;
            _exportAnswerKeyService = exportAnswerKeyService;
        }

        public IQueryable<QTIItemData> SelectQTIItems()
        {
            return _qtiItemQtiItemRepository.Select();
        }

        public void Save(QTIItemData obj, int? questionUploadOrder = null)
        {
            if (questionUploadOrder.HasValue)
            {
                obj.QuestionOrder = questionUploadOrder.Value;
            }
            else
            {
                var questionOrder = 0;
                if (_qtiItemQtiItemRepository.Select().Any(o => o.QTIGroupID == obj.QTIGroupID))
                    questionOrder =
                        _qtiItemQtiItemRepository.Select().Where(o => o.QTIGroupID == obj.QTIGroupID).Max(
                            o => o.QuestionOrder);
                if (obj.QTIItemID == 0) //incase a new item is saved
                {
                    obj.QuestionOrder = questionOrder + 1;
                }
            }

            _qtiItemQtiItemRepository.Save(obj);
        }

        public void SaveItemSub(QTIItemSub obj)
        {
            _qtiItemSubRepository.Save(obj);
        }

        public void SaveAnswerScore(QTIItemAnswerScore obj)
        {
            _qtiItemAnswerScoreInsertDeleteRepository.Save(obj);
        }

        /// <summary>
        /// Step 1: Clone new QtiItem row ==> {NewId}
        /// Step 2: Detect all img and audio file in XmlContent (detect img tags for image and itemBody, simpleChoice tags for audio and move them from Clone ItemSet_{OldId} folder to ItemSet_{NewId} folder
        /// Step 3: Update XmlContent: Replace string [ItemSet_{OldId}/] with [ItemSet_{NewId}/] (update image and audio paths)
        /// Step 4: Clone Ref Objects. Detect all Ref Object (object tag) in XmlContent files, for each Ref Object
        ///   Step 4.1: Clone new QtiRefObject row ==> {NewObjectId}
        ///   Step 4.2: Update XmlContent: update refObjectID attribute from oldObjectId to newObjectId
        ///   Step 4.3: Clone file RO_{OldObjectId}.xml to RO_{NewObjectId}.xml
        ///     Step 4.3.b: Update RO_{NewObjectId}.xml content: replace string ["RO/RO_{OldObjectId}_media/] with ["RO/RO_{NewObjectId}_media/] (update image and audio paths)
        ///   Step 4.4: Clone RO_{OldObjectId}_media folder to RO_{NewObjectId}_media folder
        /// Step 5: Clone QtiItem AnswerScore (QtiItemAnswerScore table)
        /// Step 6: Clone QtiItem State Standard (QtiItemStateStandard table)
        /// Step 7: Clone Tag (QtiItemTopic, QtiItemLessonOne, QtiItemLessonTwo tables)

        public string DuplicateListQTIItem(int userId, string qtiItemIdString, int? qTIGroupID, bool? uploadS3,
            string s3BucketName, string s3FolderName, string s3Domain)
        {
            var qtiItemIds = qtiItemIdString.Split(',');
            Dictionary<int, Dictionary<string, string>> oldNewItemMapDic =
                new Dictionary<int, Dictionary<string, string>>();
            foreach (var qtiItemId in qtiItemIds)
            {
                var qtiItem =
                    _qtiItemQtiItemRepository.Select().FirstOrDefault(o => o.QTIItemID == Convert.ToInt32(qtiItemId));
                var qtiGroupID = qTIGroupID.HasValue ? qTIGroupID.Value : qtiItem.QTIGroupID;

                // Step 2: Detect all img and audio file in XmlContent (detect img tags for image and itemBody, simpleChoice tags for audio and move them from Clone ItemSet_{OldId} folder to ItemSet_{NewId} folder
                var mediaDic = CloneMediaFileOfQtiTem(qtiItem.QTIGroupID, qtiGroupID, qtiItem.XmlContent, uploadS3, s3BucketName,
                    s3FolderName, s3Domain);
                //update media file name inside xml content
                if (mediaDic != null && !oldNewItemMapDic.ContainsKey(qtiItem.QTIItemID))
                {
                    oldNewItemMapDic.Add(qtiItem.QTIItemID, mediaDic);
                }
            }

            var returnQtiItemIds = _qtiItemQtiItemRepository.DuplicateListQTIItem(userId, qTIGroupID, qtiItemIdString);
            qtiItemIds = returnQtiItemIds.Split(',');
            foreach (var qtiItemId in qtiItemIds)
            {
                if (!string.IsNullOrEmpty(qtiItemId))
                {
                    var qtiItem =
                    _qtiItemQtiItemRepository.Select().FirstOrDefault(o => o.QTIItemID == Convert.ToInt32(qtiItemId));
                    if (oldNewItemMapDic.ContainsKey(qtiItem.QtiItemIdSource.GetValueOrDefault()))
                    {
                        var mediaDic = (Dictionary<string, string>)oldNewItemMapDic[qtiItem.QtiItemIdSource.GetValueOrDefault()];
                        foreach (var mediaPair in mediaDic)
                        {
                            qtiItem.XmlContent = qtiItem.XmlContent.Replace(mediaPair.Key, mediaPair.Value);
                        }
                        _qtiItemQtiItemRepository.Save(qtiItem);
                    }
                }
            }
            return returnQtiItemIds;
        }

        public QTIItemData DuplicateQTIItem(int userId, int qtiItemID, int? qTIGroupID, bool? uploadS3, string s3BucketName,
            string s3FolderName, string s3Domain)

        {
            var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(o => o.QTIItemID == qtiItemID);
            if (qtiItem == null) return null;
            var newQtiItem = new QTIItemData
            {
                AnswerIdentifiers = qtiItem.AnswerIdentifiers,
                CorrectAnswer = qtiItem.CorrectAnswer,
                FilePath = qtiItem.FilePath,
                InteractionCount = qtiItem.InteractionCount,
                OldMasterCode = qtiItem.OldMasterCode,
                ParentID = qtiItem.ParentID,
                PointsPossible = qtiItem.PointsPossible,
                QTIGroupID = qTIGroupID.HasValue ? qTIGroupID.Value : qtiItem.QTIGroupID,
                QTISchemaID = qtiItem.QTISchemaID,
                ResponseIdentifier = qtiItem.ResponseIdentifier,
                ResponseProcessing = qtiItem.ResponseProcessing,
                ResponseProcessingTypeID = qtiItem.ResponseProcessingTypeID,
                SourceID = qtiItem.SourceID,
                Title = qtiItem.Title,
                UrlPath = qtiItem.UrlPath,
                UserID = userId,
                XmlContent = qtiItem.XmlContent,
                QtiItemIdSource = qtiItemID,
                Description = qtiItem.Description,
                Updated = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
            };

            //It's better to clone media files first, because if there's any error here then no qti item created to avoid item created without media files associated.
            // Step 1: Detect all img and audio file in XmlContent (detect img tags for image and itemBody, simpleChoice tags for audio and move them from Clone ItemSet_{OldId} folder to ItemSet_{NewId} folder
            var mediaDic = CloneMediaFileOfQtiTem(qtiItem.QTIGroupID, newQtiItem.QTIGroupID, newQtiItem.XmlContent, uploadS3,
                s3BucketName, s3FolderName, s3Domain);

            var xmlContent = newQtiItem.XmlContent;
            //update media file name inside xml content
            if (mediaDic != null)
            {
                foreach (var mediaPair in mediaDic)
                {
                    xmlContent = xmlContent.Replace(mediaPair.Key, mediaPair.Value);
                }
            }
            // Step 2: Clone new QtiItem row ==> {NewId}
            int questionOrder = 0;
            if (_qtiItemQtiItemRepository.Select().Any(o => o.QTIGroupID == newQtiItem.QTIGroupID))
            {
                questionOrder =
                    _qtiItemQtiItemRepository.Select().Where(o => o.QTIGroupID == newQtiItem.QTIGroupID).Max(
                        o => o.QuestionOrder);
            }
            newQtiItem.QuestionOrder = questionOrder + 1;

            _qtiItemQtiItemRepository.Save(newQtiItem);

            // Step 3: Update XmlContent: Replace string ["ItemSet_{OldId}/] with ["ItemSet_{NewId}/] (update image and audio paths)
            if (qtiItem.QTIGroupID != newQtiItem.QTIGroupID)
            {
                xmlContent = xmlContent.Replace("\"ItemSet_" + qtiItem.QTIGroupID + "/",
                    "\"ItemSet_" + newQtiItem.QTIGroupID + "/");
                xmlContent = xmlContent.Replace("\"/ItemSet_" + qtiItem.QTIGroupID + "/",
                    "\"/ItemSet_" + newQtiItem.QTIGroupID + "/");
            }

            //// Step 4: Clone Ref Objects
            //xmlContent = CloneRefObjects(xmlContent, userId); //no need to create new passage any more

            //// Update xmlContent of qtiItem
            newQtiItem.XmlContent = xmlContent;

            _qtiItemQtiItemRepository.Save(newQtiItem);

            // Step 5: Clone QtiItem AnswerScore (QtiItemAnswerScore table)
            DuplicateQtiItemAnswerScore(qtiItem.QTIItemID, newQtiItem.QTIItemID);

            // Step 5.a: Clone QtiItemSub (QtiItemSub table)
            DuplicateQtiItemSub(qtiItem.QTIItemID, newQtiItem.QTIItemID);

            // Step 6: Clone QtiItem State Standard (QtiItemStateStandard table)
            DuplicateQtiItemStateStandard(qtiItem.QTIItemID, newQtiItem.QTIItemID);

            // Step 7: Clone Tag (QtiItemTopic, QtiItemLessonOne, QtiItemLessonTwo tables)
            DuplicateQtiItemTag(qtiItem.QTIItemID, newQtiItem.QTIItemID);

            // Step 8: Clone AlgorithmQTIItemGrading table
            DuplicateAlgorithmQTIItemGrading(qtiItem.QTIItemID, newQtiItem.QTIItemID, userId);

            // Step 9: Clone MultiPart Expression
            if (qtiItem.QTISchemaID == (int)QtiSchemaEnum.MultiPart)
            {
                DuplicateMultiPartExpression(qtiItem.QTIItemID, newQtiItem.QTIItemID, userId);
            }

            return newQtiItem;
        }

        public void CopyConditionalLogicsFromQTIItemToNewVirtualQuestion(int virtualQuestionID, int qTIItemID)
        {
            _qtiItemQtiItemRepository.CopyConditionalLogicsFromQTIItemToNewVirtualQuestion(virtualQuestionID, qTIItemID);
        }

        private Dictionary<string, string> CloneMediaFileOfQtiTem(int qtiGroupId, int newQtiGroupId, string xmlContent, bool? uploadS3,
            string s3BucketName, string s3FolderName, string s3Domain)
        {
            // Do not clone image and audio when clone qtitem in the same itemset
            if (qtiGroupId == newQtiGroupId)
                return null;
            s3Domain = s3Domain.RemoveEndSlash();
            s3BucketName = s3BucketName.RemoveStartSlash();
            s3BucketName = s3BucketName.RemoveEndSlash();
            s3FolderName = s3FolderName.RemoveStartSlash();
            s3FolderName = s3FolderName.RemoveEndSlash();

            var fileNameList = DetectMediaFileInXmlContent(xmlContent, "ItemSet_" + qtiGroupId);
            var fileNameDic = new Dictionary<string, string>();
            foreach (var fileName in fileNameList)
            {
                //alwasy copy on S3
                if (fileName.ToLower().StartsWith("http"))
                {
                    //check if there's an existing file name on the destination or not

                    var newFileName = fileName.Replace("ItemSet_" + qtiGroupId.ToString(), "ItemSet_" + newQtiGroupId.ToString());
                    if (UrlUtil.CheckUrlStatus(newFileName))
                    {
                        //add timestamp to the new file name
                        newFileName = newFileName.AddTimestampToFilePath();
                        if (!fileNameDic.ContainsKey(fileName))
                        {
                            fileNameDic.Add(fileName, newFileName);//keep the new file name to replace in xmlcontent later
                        }
                    }
                    CloneMediaFileOnS3(qtiGroupId, newQtiGroupId, fileName, newFileName, s3BucketName, s3FolderName);
                }
                else
                {
                    //fileName such as egypt-photo-countries.jpg
                    //check if the new file name is existing on S3 or not
                    //build a new url like this https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_16818/Magicka_ABC-201510230219092512-201510230244315401.mp4
                    //while s3Domain like https://s3.amazonaws.com/
                    //s3BucketName like testitemmedia/
                    //s3FolderName like Vina/
                    var newFileUrlS3 = string.Empty;
                    if (string.IsNullOrEmpty(s3FolderName))
                    {
                        newFileUrlS3 = string.Format("{0}/ItemSet_{1}/{2}", UrlUtil.GenerateS3Subdomain(s3Domain, s3BucketName).RemoveEndSlash(),
                        newQtiGroupId, fileName.RemoveStartSlash());
                    }
                    else
                    {
                        newFileUrlS3 = string.Format("{0}/{1}/ItemSet_{2}/{3}", UrlUtil.GenerateS3Subdomain(s3Domain, s3BucketName).RemoveEndSlash(), s3FolderName.RemoveEndSlash().RemoveStartSlash(),
                        newQtiGroupId, fileName.RemoveStartSlash());
                    }
                    if (UrlUtil.CheckUrlStatus(newFileUrlS3))
                    {
                        newFileUrlS3 = newFileUrlS3.AddTimestampToFilePath();//ad timestamp to avoid overwriting
                    }
                    if (!fileNameDic.ContainsKey(Path.GetFileName(fileName)))
                    {
                        fileNameDic.Add(Path.GetFileName(fileName), Path.GetFileName(newFileUrlS3));//keep the new file name to replace in xmlcontent later
                    }

                    var oldFileUrlS3 = string.Empty;
                    if (string.IsNullOrEmpty(s3FolderName))
                    {
                        oldFileUrlS3 = string.Format("{0}/ItemSet_{1}/{2}", UrlUtil.GenerateS3Subdomain(s3Domain, s3BucketName).RemoveEndSlash(),
                        qtiGroupId, fileName.RemoveStartSlash());
                    }
                    else
                    {
                        oldFileUrlS3 = string.Format("{0}/{1}/ItemSet_{2}/{3}", UrlUtil.GenerateS3Subdomain(s3Domain, s3BucketName).RemoveEndSlash(), s3FolderName.RemoveEndSlash().RemoveStartSlash(),
                       qtiGroupId, fileName.RemoveStartSlash());
                    }

                    if (UrlUtil.CheckUrlStatus(oldFileUrlS3))
                    {
                        CloneMediaFileOnS3(qtiGroupId, newQtiGroupId, oldFileUrlS3, newFileUrlS3, s3BucketName,
                            s3FolderName);
                    }
                    //else
                    //{
                    //    //try to find the file in web server to upload to S3
                    //    string itemSetPath = ConfigurationManager.AppSettings["TestItemMediaPath"] + "\\ItemSet_" +
                    //               qtiGroupId.ToString();
                    //    string oldFilePath = itemSetPath + "\\" + fileName;

                    //    if (File.Exists(oldFilePath))
                    //    {
                    //        using (FileStream fsSource = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read))
                    //        {
                    //            var s3Service = new S3Service(300000); // 5 minutes
                    //            string s3ItemSetPath = "ItemSet_" + newQtiGroupId.ToString();
                    //            string newFileName = Path.GetFileName(newFileUrlS3);
                    //            if (newFileName.StartsWith("/") || newFileName.StartsWith("\\"))
                    //            {
                    //                newFileName = newFileName.Substring(1, newFileName.Length - 1);
                    //            }
                    //            var s3FullFileName = s3ItemSetPath + "/" + newFileName;
                    //            var uploadResult = s3Service.UploadRubricFile(s3BucketName, s3FolderName + "/" + s3FullFileName, fsSource);
                    //            if (!uploadResult.IsSuccess)
                    //            {
                    //                throw new Exception("Could not upload: " + Path.GetFileName(oldFilePath));
                    //            }
                    //        }
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("Could not file: " + Path.GetFileName(oldFilePath));
                    //    }
                    //}
                }
            }
            return fileNameDic;
        }

        // itemSetKeyword e.g: ItemSet_356
        private List<string> DetectMediaFileInXmlContent(string xmlContent, string itemSetKeyword)
        {
            var filePaths = new List<string>();
            var xdoc = new XmlContentProcessing(xmlContent);

            // Detect all img
            var imgNodes = xdoc.GetElementsByTagName("img");
            foreach (XmlNode node in imgNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "src");
                if (string.IsNullOrEmpty(attr))
                {
                    attr = GetNodeAttribute(node, "source");//sometime the img source is source not src
                }

                if (!string.IsNullOrWhiteSpace(attr)
                    && attr.ToLower().Contains(itemSetKeyword.ToLower()))
                {
                    if (attr.ToLower().StartsWith("http"))
                    {
                        //media from s3, such as https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_16816/12002134_1185498241466589_3504280842555770200_n-201510220647415096.jpg
                        //this file must be copied as well
                        if (attr.ToLower().IndexOf("amazonaws.com") >= 0)
                        {
                            if (!filePaths.Contains(attr))
                            {
                                filePaths.Add(attr);
                            }
                        }
                    }
                    else
                    {
                        //var fileName = Path.GetFileName(attr);
                        //if (!filePaths.Contains(fileName))
                        //{
                        //    filePaths.Add(fileName);
                        //}
                        filePaths.Add(attr.Replace(itemSetKeyword, string.Empty).RemoveStartSlash());//there are sometime files locate inside sub folder, such as ItemSet_17040/ppg/examview/5th_Benchmark1_2012_ONLINE/mc005-1.jpg
                    }
                }
            }
            // Detect all image HotSpot
            var imageHotSpotNodes = xdoc.GetElementsByTagName("imageHotSpot");
            foreach (XmlNode node in imageHotSpotNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "src");
                if (!string.IsNullOrWhiteSpace(attr)
                    && attr.ToLower().Contains(itemSetKeyword.ToLower()))
                {
                    if (attr.ToLower().StartsWith("http"))
                    {
                        //media from s3, such as https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_16816/12002134_1185498241466589_3504280842555770200_n-201510220647415096.jpg
                        //this file must be copied as well
                        if (attr.ToLower().IndexOf("amazonaws.com") >= 0)
                        {
                            if (!filePaths.Contains(attr))
                            {
                                filePaths.Add(attr);
                            }
                        }
                    }
                    else
                    {
                        //var fileName = Path.GetFileName(attr);
                        //if (!filePaths.Contains(fileName))
                        //{
                        //    filePaths.Add(fileName);
                        //}
                        filePaths.Add(attr.Replace(itemSetKeyword, string.Empty).RemoveStartSlash());//there are sometime files locate inside sub folder, such as ItemSet_17040/ppg/examview/5th_Benchmark1_2012_ONLINE/mc005-1.jpg
                    }
                }
            }

            //Detech destination images
            var destinationObjectNodes = xdoc.GetElementsByTagName("destinationObject");
            foreach (XmlNode node in destinationObjectNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "src");
                if (!string.IsNullOrWhiteSpace(attr)
                    && attr.ToLower().Contains(itemSetKeyword.ToLower()))
                {
                    if (attr.ToLower().StartsWith("http"))
                    {
                        //media from s3, such as https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_16816/12002134_1185498241466589_3504280842555770200_n-201510220647415096.jpg
                        //this file must be copied as well
                        if (attr.ToLower().IndexOf("amazonaws.com") >= 0)
                        {
                            if (!filePaths.Contains(attr))
                            {
                                filePaths.Add(attr);
                            }
                        }
                    }
                    else
                    {
                        //var fileName = Path.GetFileName(attr);
                        //if (!filePaths.Contains(fileName))
                        //{
                        //    filePaths.Add(fileName);
                        //}
                        filePaths.Add(attr.Replace(itemSetKeyword, string.Empty).RemoveStartSlash());//there are sometime files locate inside sub folder, such as ItemSet_17040/ppg/examview/5th_Benchmark1_2012_ONLINE/mc005-1.jpg
                    }
                }
            }
            // Detect all audio
            var bodyNodes = xdoc.GetElementsByTagName("itemBody");
            foreach (XmlNode node in bodyNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "audioRef");
                if (!string.IsNullOrWhiteSpace(attr) && attr.Contains(itemSetKeyword))
                {
                    if (attr.ToLower().StartsWith("http"))
                    {
                        //such as https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_16816/Square-201510220650145487.mp3
                        if (attr.ToLower().IndexOf("amazonaws.com") >= 0)
                        {
                            if (!filePaths.Contains(attr))
                            {
                                filePaths.Add(attr);
                            }
                        }
                    }
                    else
                    {
                        //var fileName = Path.GetFileName(attr);
                        //if (!filePaths.Contains(fileName))
                        //{
                        //    filePaths.Add(fileName);
                        //}
                        filePaths.Add(attr.Replace(itemSetKeyword, string.Empty).RemoveStartSlash());//there are sometime files locate inside sub folder, such as ItemSet_17040/ppg/examview/5th_Benchmark1_2012_ONLINE/mc005-1.jpg
                    }
                }
            }

            var simpleChoiceNodes = xdoc.GetElementsByTagName("simpleChoice");
            foreach (XmlNode node in simpleChoiceNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "audioRef");
                if (!string.IsNullOrWhiteSpace(attr) && attr.Contains(itemSetKeyword))
                {
                    if (attr.ToLower().StartsWith("http"))
                    {
                        //such as https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_16816/Square-201510220650145487.mp3
                        if (attr.ToLower().IndexOf("amazonaws.com") >= 0)
                        {
                            if (!filePaths.Contains(attr))
                            {
                                filePaths.Add(attr);
                            }
                        }
                    }
                    else
                    {
                        //var fileName = Path.GetFileName(attr);
                        //if (!filePaths.Contains(fileName))
                        //{
                        //    filePaths.Add(fileName);
                        //}
                        filePaths.Add(attr.Replace(itemSetKeyword, string.Empty).RemoveStartSlash());//there are sometime files locate inside sub folder, such as ItemSet_17040/ppg/examview/5th_Benchmark1_2012_ONLINE/mc005-1.jpg
                    }
                }
            }

            // Detect all Videos
            var sourceNodes = xdoc.GetElementsByTagName("source");
            foreach (XmlNode node in sourceNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "src");//video is always in S3
                if (!string.IsNullOrWhiteSpace(attr)
                    && attr.ToLower().Contains(itemSetKeyword.ToLower()))
                {
                    if (!filePaths.Contains(attr))
                    {
                        filePaths.Add(attr);
                    }
                }
            }

            return filePaths;
        }

        private string GetNodeAttribute(XmlNode node, string attributeName)
        {
            if (node == null || string.IsNullOrEmpty(attributeName)) return null;
            if (node.Attributes == null) return null;
            if (node.Attributes[attributeName] == null) return null;
            var result = node.Attributes[attributeName].Value;
            return result;
        }

        private static void SetNodeAttribute(XmlNode node, string attributeName, string value)
        {
            if (node == null || string.IsNullOrEmpty(attributeName)) return;
            if (node.Attributes == null) return;
            if (node.Attributes[attributeName] == null)
            {
                XmlUtils.AddAttribute(node, attributeName, value);
            }
            node.Attributes[attributeName].Value = value;
        }

        // Clone one file from oldItemSet folder to newItemSet folder
        private void CloneMediaFileOnS3(int qtiGroupId, int newQtiGroupId, string oldFileUrl, string newFileUrl,
            string s3BucketName, string s3FolderName)
        {
            //var rawOldFilename = Path.GetFileName(oldFileUrl).Replace("%20", " ");//S3 does not accept %20 as blank, so it's necessary to manually replace %20
            //var rawNewFilename = Path.GetFileName(newFileUrl).Replace("%20", " ");
            oldFileUrl = oldFileUrl.Replace("%20", " ");//S3 does not accept %20 as blank, so it's necessary to manually replace %20
            newFileUrl = newFileUrl.Replace("%20", " ");
            //var oldS3FileName = s3FolderName + "/ItemSet_" + qtiGroupId + "/" + rawOldFilename.RemoveStartSlash();
            //get the file name to download from S3 ( sometime the oldFileUrl looks like https://testitemmedia.s3.amazonaws.com/Vina/ItemSet_17040/ppg/examview/5th_Benchmark1_2012_ONLINE/mc005-1.jpg )
            if (string.IsNullOrEmpty(s3FolderName))
            {
                var folderIdx = oldFileUrl.IndexOf("ItemSet_" + qtiGroupId.ToString());
                var oldS3FileName = string.Empty;
                if (folderIdx >= 0)
                {
                    oldS3FileName = oldFileUrl.Substring(folderIdx, oldFileUrl.Length - folderIdx);
                }

                //string s3ItemSetPath = "ItemSet_" + newQtiGroupId;
                folderIdx = newFileUrl.IndexOf("ItemSet_" + newQtiGroupId.ToString());
                var newS3FileName = string.Empty;
                if (folderIdx >= 0)
                {
                    newS3FileName = newFileUrl.Substring(folderIdx, newFileUrl.Length - folderIdx);
                }

                _s3Service.CopyFile(s3BucketName, oldS3FileName, newS3FileName);
            }
            else
            {
                var folderIdx = oldFileUrl.IndexOf(s3FolderName);
                var oldS3FileName = string.Empty;
                if (folderIdx >= 0)
                {
                    oldS3FileName = oldFileUrl.Substring(folderIdx, oldFileUrl.Length - folderIdx);
                }

                //string s3ItemSetPath = "ItemSet_" + newQtiGroupId;
                folderIdx = newFileUrl.IndexOf(s3FolderName);
                var newS3FileName = string.Empty;
                if (folderIdx >= 0)
                {
                    newS3FileName = newFileUrl.Substring(folderIdx, newFileUrl.Length - folderIdx);
                }

                _s3Service.CopyFile(s3BucketName, oldS3FileName, newS3FileName);
            }
        }

        private string CloneRefObjects(string xmlContent, int userId, string testItemMediaPath)
        {
            var refObjects = DetectRefObjectInXmlContent(xmlContent);

            foreach (var refObjectIdValue in refObjects)
            {
                var refObjectId = Convert.ToInt32(refObjectIdValue);
                var newRefObject = CloneQtiRefObject(refObjectId, userId);
                if (newRefObject != null)
                {
                    xmlContent = UpdateRefObjectIdOfXmlContent(xmlContent, refObjectId, newRefObject.QTIRefObjectID);
                    CloneROFile(refObjectId, newRefObject.QTIRefObjectID, testItemMediaPath);
                    CloneQtiRefObjectFolder(refObjectId, newRefObject.QTIRefObjectID);
                }
            }

            return xmlContent;
        }

        private void CloneROFile(int refObjectId, int newRefObjectId, string testItemMediaPath)
        {
            try
            {
                string configPath = string.Empty;
                string oldROPath = configPath + "\\RO\\RO_" + refObjectId.ToString() + ".xml";
                string newROPath = configPath + "\\RO\\RO_" + newRefObjectId.ToString() + ".xml";
                if (File.Exists(oldROPath))
                {
                    string text = File.ReadAllText(oldROPath);
                    text = text.Replace("RO/RO_" + refObjectId + "_media/", "RO/RO_" + newRefObjectId + "_media/");
                    File.WriteAllText(newROPath, text);
                }
            }
            catch (Exception)
            {
            }
        }

        private string UpdateRefObjectIdOfXmlContent(string xmlContent, int refObjectId, int newRefObjectId)
        {
            //var xdoc = new XmlDocument();
            //xdoc.LoadXml(xmlContent);
            var xdoc = new XmlContentProcessing(xmlContent);

            // Detect all img
            var refObjectNodes = xdoc.GetElementsByTagName("object");
            foreach (XmlNode node in refObjectNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "refObjectID");
                if (!string.IsNullOrWhiteSpace(attr) && attr == refObjectId.ToString())
                {
                    SetNodeAttribute(node, "refObjectID", newRefObjectId.ToString());
                }
            }

            return xdoc.GetXmlContent();
        }

        private QtiRefObject CloneQtiRefObject(int refObjectId, int userId)
        {
            var qtiRefObject =
                _qtiRefObjectRepository.Select().FirstOrDefault(en => en.QTIRefObjectID == refObjectId);

            if (qtiRefObject == null) return null;
            var newQtiRefObject = new QtiRefObject
            {
                DistrictId = qtiRefObject.DistrictId,
                FleschKincaidID = qtiRefObject.FleschKincaidID,
                GradeID = qtiRefObject.GradeID,
                Name = qtiRefObject.Name,
                OldMasterCode = qtiRefObject.OldMasterCode,
                QTIRefObjectFileRef = qtiRefObject.QTIRefObjectFileRef,
                StateId = qtiRefObject.StateId,
                Subject = qtiRefObject.Subject,
                TextSubTypeID = qtiRefObject.TextSubTypeID,
                TextTypeID = qtiRefObject.TextSubTypeID,
                TypeID = qtiRefObject.TypeID,
                UserID = userId
            };

            _qtiRefObjectRepository.Save(newQtiRefObject);

            return newQtiRefObject;
        }

        private List<string> DetectRefObjectInXmlContent(string xmlContent)
        {
            var refObjects = new List<string>();

            //var xdoc = new XmlDocument();
            //xdoc.LoadXml(xmlContent);
            var xdoc = new XmlContentProcessing(xmlContent);

            // Detect all img
            var refObjectNodes = xdoc.GetElementsByTagName("object");
            foreach (XmlNode node in refObjectNodes)
            {
                if (node.Attributes == null) continue;
                var attr = GetNodeAttribute(node, "refObjectID");
                if (!string.IsNullOrWhiteSpace(attr))
                {
                    if (refObjects.All(en => en != attr))
                        refObjects.Add(attr);
                }
            }

            return refObjects;
        }

        private void CloneQtiRefObjectFolder(int qtiRefObjectId, int newQtiRefObjectId)
        {
            try
            {
                //string configPath = ConfigurationManager.AppSettings["TestItemMediaPath"] + "\\RO";
                string configPath = string.Empty;
                string oldRefObjectPath = configPath + "\\RO_" + qtiRefObjectId + "_media";
                string newRefObjectPath = configPath + "\\RO_" + newQtiRefObjectId + "_media";
                if (File.Exists(oldRefObjectPath))
                {
                    if (!Directory.Exists(newRefObjectPath))
                    {
                        Directory.CreateDirectory(newRefObjectPath);
                    }

                    var files = Directory.GetFiles(oldRefObjectPath);
                    foreach (var filePath in files)
                    {
                        var newFilePath = filePath.Replace(oldRefObjectPath, newRefObjectPath);
                        File.Copy(filePath, newFilePath, true);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void DuplicateQtiItemAnswerScore(int qtiItemId, int newQtiItemId)
        {
            var qtiItemAnswerScores = _qtiItemAnswerScoreRepository.Select().Where(en => en.QTIItemId == qtiItemId);
            foreach (var item in qtiItemAnswerScores)
            {
                var newQtiItemAnswerScore = new QTIItemAnswerScore
                {
                    Answer = item.Answer,
                    QTIItemId = newQtiItemId,
                    ResponseIdentifier = item.ResponseIdentifier,
                    Score = item.Score,
                    AnswerText = item.AnswerText
                };
                _qtiItemAnswerScoreInsertDeleteRepository.Save(newQtiItemAnswerScore);
            }
        }

        private void DuplicateQtiItemSub(int qtiItemId, int newQtiItemId)
        {
            var qtiItemSubs = _qtiItemSubReadOnlyRepository.Select().Where(en => en.QTIItemId == qtiItemId);
            foreach (var item in qtiItemSubs)
            {
                var newQtiItemSub = new QTIItemSub
                {
                    CorrectAnswer = item.CorrectAnswer,
                    PointsPossible = item.PointsPossible,
                    QTIItemId = newQtiItemId,
                    QTISchemaId = item.QTISchemaId,
                    ResponseIdentifier = item.ResponseIdentifier,
                    ResponseProcessing = item.ResponseProcessing,
                    ResponseProcessingTypeId = item.ResponseProcessingTypeId,
                    SourceId = item.SourceId,
                    Updated = item.Updated,
                    Depending = item.Depending,
                    Major = item.Major
                };
                _qtiItemSubRepository.Save(newQtiItemSub);
            }
        }

        private void DuplicateQtiItemStateStandard(int qtiItemId, int newQtiItemId)
        {
            var qtiItemStateStandards = _qtiItemStateStandardRepository.Select().Where(en => en.QTIItemID == qtiItemId).ToList();

            if (qtiItemStateStandards.Count <= 0)
            {
                return;
            }

            qtiItemStateStandards.ForEach(item =>
            {
                item.QTIItemStateStandardID = 0;
                item.QTIItemID = newQtiItemId;
            });

            _qtiItemStateStandardRepository.InsertMultipleRecord(qtiItemStateStandards);
        }

        private void DuplicateQtiItemTag(int qtiItemId, int newQtiItemId)
        {
            var qtiItemTopics = _qtiItemTopicRepository.Select().Where(en => en.QTIItemID == qtiItemId);
            foreach (var item in qtiItemTopics)
            {
                var newQtiItemTopic = new QTIItemTopic
                {
                    Name = item.Name,
                    QTIItemID = newQtiItemId,
                    TopicId = item.TopicId
                };
                _qtiItemTopicRepository.Save(newQtiItemTopic);
            }

            var qtiItemLessonOnes = _qtiItemLessonOneRepository.Select().Where(en => en.QTIItemID == qtiItemId);
            foreach (var item in qtiItemLessonOnes)
            {
                var newQtiItemLessonOne = new QTIItemLessonOne
                {
                    Name = item.Name,
                    QTIItemID = newQtiItemId,
                    LessonOneID = item.LessonOneID
                };
                _qtiItemLessonOneRepository.Save(newQtiItemLessonOne);
            }

            var qtiItemLessonTwos = _qtiItemLessonTwoRepository.Select().Where(en => en.QTIItemID == qtiItemId);
            foreach (var item in qtiItemLessonTwos)
            {
                var newQtiItemLessonTwo = new QTIItemLessonTwo
                {
                    Name = item.Name,
                    QTIItemID = newQtiItemId,
                    LessonTwoID = item.LessonTwoID
                };
                _qtiItemLessonTwoRepository.Save(newQtiItemLessonTwo);
            }
            var qtiItemItemtag = _qtiItemItemTagRepository.Select().Where(en => en.QtiItemID == qtiItemId);
            foreach (var item in qtiItemItemtag)
            {
                var newQtiItemItemtag = new QtiItemItemTag()
                {
                    QtiItemID = newQtiItemId,
                    ItemTagID = item.ItemTagID
                };
                _qtiItemItemTagRepository.Save(newQtiItemItemtag);
            }
        }

        public void TMCopyStandardsFromQTIItem(int virtualQuestionID, int qtiItemID, int stateID)
        {
            _qtiItemQtiItemRepository.TMCopyStandardsFromQTIItem(virtualQuestionID, qtiItemID, stateID);
        }

        public List<QTI3pItem> GetItemsByQtibankId(int qtibankId)
        {
            return _qtiItemQtiItemRepository.GetQti3PItemsByQtibankId(qtibankId);
        }

        public QTIItemData TMAddItemFromLibrary(int qti3pItemId, int qTIGroupID, int userId)
        {
            QTIItemData result = null;

            try
            {
                var qTI3pItem = _itemRepository.Select().FirstOrDefault(x => x.QTI3pItemID == qti3pItemId);
                if (qTI3pItem == null)
                {
                    return result;
                }
                var qTISchemaID = qTI3pItem.QTISchemaID == 0 ? 1 : qTI3pItem.QTISchemaID;
                if (qTI3pItem.FilePath == null)
                {
                    qTI3pItem.FilePath = string.Empty;
                }
                if (qTI3pItem.UrlPath == null)
                {
                    qTI3pItem.UrlPath = string.Empty;
                }

                var qtiXmlUtil = new QTIXmlUtil();

                string answerIdentifiers = "";

                result = new QTIItemData
                {
                    Title = "",
                    QTISchemaID = qTISchemaID,
                    CorrectAnswer = qTI3pItem.CorrectAnswer,
                    FilePath = qTI3pItem.FilePath,
                    UrlPath = qTI3pItem.UrlPath,
                    UserID = userId,
                    QTIGroupID = qTIGroupID,
                    QuestionOrder = 0,
                    // Question order is updated to max question order by TMAddQtiItemRelatedInfoFromLibrary store
                    SourceID = qTI3pItem.QTI3pItemID,
                    AnswerIdentifiers = answerIdentifiers,
                    PointsPossible = 1,
                    ResponseIdentifier = "RESPONSE",
                    ResponseProcessing = "<process  method=\"default\" />",
                    ResponseProcessingTypeID = 1,
                    Updated = DateTime.Now
                };
                if (qTI3pItem.QTI3pSourceID == (int)QTI3pSourceEnum.Mastery) //the old kind of NWEA
                {
                    qTI3pItem.XmlContent = qTI3pItem.XmlContent.Replace("<math", "&#160;<math")
                        .Replace("</math>", "</math>&#160;");
                    qTI3pItem.XmlContent = qTI3pItem.XmlContent.Replace("&nbsp;", "&#160;");
                    //var temp = string.Format("<![CDATA[{0}]]>", Guid.NewGuid().ToString());//no use CDATA because ConvertToLinkitXML will convert it into something like <span styleName="bold class1" class="bold class1"> ???
                    var temp = Guid.NewGuid().ToString();
                    //qTI3pItem.XmlContent = qTI3pItem.XmlContent.Replace("&#160;", temp);
                    qTI3pItem.XmlContent = qTI3pItem.XmlContent.Replace("&#", temp); //now general for all &#...

                    var qtiItemXmlContent = qtiXmlUtil.ConvertToLinkitXML(qTI3pItem.XmlContent,
                        "http://" + qTI3pItem.UrlPath,
                        qTISchemaID.ToString(), ref answerIdentifiers);

                    result.AnswerIdentifiers = answerIdentifiers;
                    result.XmlContent = qtiItemXmlContent.Replace(temp, "&#");
                }
                else
                {
                    result.XmlContent = qTI3pItem.XmlContent;//no need to convert for progressive
                    result.ResponseIdentifier = "RESPONSE_1";
                    if (qTI3pItem.QTISchemaID == (int)QtiSchemaEnum.Complex)
                    {
                        result.ResponseIdentifier = "multi";
                    }
                }

                //replace character ’ because Flash can not view ’
                result.XmlContent = result.XmlContent.Replace("’", "'");
                //replace character “ by " and ” by " because Flash can not view “ and ”
                result.XmlContent = result.XmlContent.Replace("“", "\"");
                result.XmlContent = result.XmlContent.Replace("”", "\"");
                _qtiItemQtiItemRepository.Save(result);

                //Insert QtiItemQTI3pPassage
                //Get all the associated QTI3pPassage of this item
                var itemPassages =
                    _qTI3pItemToPassageRepository.Select().Where(x => x.Qti3pItemId == qTI3pItem.QTI3pItemID);
                foreach (var qti3PItemToPassage in itemPassages)
                {
                    var qtiItemQTI3pPassage = new QtiItemQTI3pPassage
                    {
                        QtiItemId = result.QTIItemID,
                        QTI3pPassageId = qti3PItemToPassage.Qti3pItemPassageId
                    };
                    _qtiItemQTI3pPassageRepository.Save(qtiItemQTI3pPassage);
                }

                _qtiItemQtiItemRepository.TMAddQtiItemRelatedInfoFromLibrary(result.QTIItemID,

                                                                             qTI3pItem.QTI3pItemID);
            }
            catch (Exception ex)
            {
                return result;
            }

            return result;
        }

        public string UpdateImgPathOfXmlContent(string xmlCotent, string urlRoot)
        {
            var qtiXmlUtil = new QTIXmlUtil();
            return qtiXmlUtil.UpdateImgPathOfXmlContent(xmlCotent, urlRoot);
        }

        public string Delete(int qtiItemId, int userId)
        {
            return _qtiItemQtiItemRepository.Delete(qtiItemId, userId);
        }

        public List<QtiItem> GetQtiItemsByFilter(QtiItemFilters filter, int? userId,
                                                 int districtId, int startIndex, int pageSize, string sortColumns,
                                                 string searchColumns, string searchInboxXML)
        {
            return _qtiItemQtiItemRepository.GetQtiItemsByFilter(filter, userId, districtId, startIndex,
                                                                 pageSize, sortColumns, searchColumns, searchInboxXML);
        }

        public QTIItemData GetQtiItemById(int id)
        {
            return _qtiItemQtiItemRepository.Select().FirstOrDefault(x => x.QTIItemID == id);
        }

        public IList<GetMostRecentItemVersionsDto> GetMostRecentItemVersions(int stateId, int qtiItemId, int numberOfVersions)
        {
            var versions = new List<GetMostRecentItemVersionsDto>();

            var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(x => x.QTIItemID == qtiItemId);
            if (qtiItem == null)
            {
                return versions;
            }

            var userId = qtiItem.UpdatedByUserID ?? qtiItem.UserID;
            var user = _userRepository.Select().FirstOrDefault(x => x.Id == userId);
            var fullName = user != null ? user.LastName + ", " + user.FirstName : null;

            var qtiItemHistories = _qtiItemQtiItemHistoryRepository.Select()
                .Where(x => x.QTIItemID == qtiItemId)
                .OrderByDescending(x => x.ChangedDate)
                .Take(numberOfVersions - 1)
                .ToArray();

            QTIItemHistory revertedQtiItemHistory = null;
            if (qtiItem.RevertedFromQTIItemHistoryID.HasValue)
            {
                revertedQtiItemHistory = qtiItemHistories.FirstOrDefault(x => x.QTIItemHistoryID == qtiItem.RevertedFromQTIItemHistoryID.Value);
                if (revertedQtiItemHistory == null)
                {
                    revertedQtiItemHistory = _qtiItemQtiItemHistoryRepository.Select().FirstOrDefault(x => x.QTIItemHistoryID == qtiItem.RevertedFromQTIItemHistoryID.Value);
                }
            }

            var timeZoneId = _stateRepository.GetTimeZoneId(stateId);

            versions.Add(new GetMostRecentItemVersionsDto
            {
                QTIItemHistoryID = 0,
                QTIItemID = qtiItemId,
                ChangedDate = (qtiItem.Updated ?? qtiItem.CreatedDate).ConvertTimeFromUtc(timeZoneId).ToString("s"),
                XmlContent = qtiItem.XmlContent,
                AuthorID = userId,
                AuthorFullName = fullName,
                RevertedFromDate = revertedQtiItemHistory?.ChangedDate.ConvertTimeFromUtc(timeZoneId).ToString("s")
            });

            if (qtiItemHistories.Any())
            {
                var authorIds = qtiItemHistories.Select(x => x.AuthorID).Distinct().ToArray();
                var authors = _userRepository.Select()
                    .Where(x => authorIds.Contains(x.Id))
                    .Select(x => new
                    {
                        x.Id,
                        FullName = x.LastName + ", " + x.FirstName
                    })
                    .ToArray();

                versions.AddRange((
                    from qih in qtiItemHistories
                    join u in authors on qih.AuthorID equals u.Id into userGroup
                    from u in userGroup.DefaultIfEmpty()
                    select new GetMostRecentItemVersionsDto
                    {
                        QTIItemHistoryID = qih.QTIItemHistoryID,
                        QTIItemID = qtiItemId,
                        ChangedDate = qih.ChangedDate.ConvertTimeFromUtc(timeZoneId).ToString("s"),
                        XmlContent = qih.XmlContent,
                        AuthorID = qih.AuthorID,
                        AuthorFullName = u?.FullName,
                    }));
            }
            
            return versions;
        }

        public bool RevertItem(int currentUserId, int virtualQuestionId, int qtiItemId, int qtiItemHistoryId, out RevertItemOutputDto output)
        {
            output = new RevertItemOutputDto();

            var qtiItemHistory = _qtiItemQtiItemHistoryRepository.Select()
                .FirstOrDefault(x => x.QTIItemHistoryID == qtiItemHistoryId && x.QTIItemID == qtiItemId);
            if (qtiItemHistory == null)
                return false;

            var qtiItem = _qtiItemQtiItemRepository.Select()
                .FirstOrDefault(x => x.QTIItemID.Equals(qtiItemId));
            if (qtiItem == null)
                return false;

            var qtiItemHistoryTestTaker = _qTIItemConvert.ConvertFromXmlContent(qtiItemHistory.XmlContent);
            output.OldQTISchemaID = qtiItem.QTISchemaID;
            output.OldQTISchemaID = qtiItemHistoryTestTaker.QTISchemaID;

            var virtualQuestions = _virtualQuestionRepository.Select().Where(en => en.QTIItemID == qtiItemId);

            SaveQtiItemHistory(qtiItem);
            CorrectConstructedResponseQuestion(currentUserId, qtiItem, virtualQuestionId, ref qtiItemHistoryTestTaker);
            SaveQtiItem(currentUserId, qtiItem, qtiItemHistory, qtiItemHistoryTestTaker);
            SaveQTIITemSub(qtiItemId, qtiItemHistoryTestTaker, virtualQuestions);
            output.VirtualQuestions = virtualQuestions.ToList();
            SaveQTIItemAnswerScores(qtiItemId, qtiItemHistoryTestTaker, virtualQuestions);

            return true;
        }

        public QTIItemHistory GetQTIItemHistoryByQtiItemHistoryId(int qtiItemHistoryId)
        {
            return _qtiItemQtiItemHistoryRepository.Select().FirstOrDefault(x => x.QTIItemHistoryID ==  qtiItemHistoryId);
        }

        private void CorrectConstructedResponseQuestion(int currentUserId, QTIItemData qtiItem, int virtualQuestionId, ref QTIItemTestMaker qtiItemHistoryTestTaker)
        {
            var currentXDoc = new XmlDocument();
            currentXDoc.LoadXml(qtiItem.XmlContent);
            var currentResponseDeclarations = currentXDoc.GetElementsByTagName("responseDeclaration");
            List<string> currentMethods = new List<string>();
            foreach (XmlNode node in currentResponseDeclarations)
            {
                currentMethods.Add(XmlUtils.GetNodeAttribute(node, "method"));
            }

            var revertedXDoc = new XmlDocument();
            revertedXDoc.LoadXml(qtiItemHistoryTestTaker.XmlContent);
            var revertedResponseDeclarations = revertedXDoc.GetElementsByTagName("responseDeclaration");
            List<string> revertedMethods = new List<string>();
            foreach (XmlNode node in revertedResponseDeclarations)
            {
                revertedMethods.Add(XmlUtils.GetNodeAttribute(node, "method"));
            }

            var mustRemoveRubricData = MustRemoveRubricData(qtiItem, currentMethods, revertedMethods) ? 1 : 0;
            _qtiItemQtiItemRepository.UpdateQtiItemVirtualQuestion(qtiItem.QTIItemID, qtiItemHistoryTestTaker.PointsPossible, mustRemoveRubricData);

            if (qtiItem.QTISchemaID != (int)QTISchemaEnum.ExtendedText && qtiItemHistoryTestTaker.QTISchemaID == (int)QTISchemaEnum.ExtendedText)
            {
                foreach (XmlNode node in revertedResponseDeclarations)
                {
                    XmlUtils.SetNodeAttribute(node, "method", "default");
                }

                qtiItemHistoryTestTaker = _qTIItemConvert.ConvertFromXmlContent(revertedXDoc.OuterXml);
            }

            if (qtiItem.QTISchemaID == (int)QtiSchemaEnum.MultiPart && qtiItemHistoryTestTaker.QTISchemaID != (int)QtiSchemaEnum.MultiPart)
            {
                var emptyMultiPartExpressionXml = BuildMultiPartExpressionXml(new List<MultiPartExpressionDto>());
                _multiPartQtiItemExpressionRepository.SaveExpression(qtiItem.QTIItemID, virtualQuestionId, emptyMultiPartExpressionXml, currentUserId);
            }

            if (currentMethods.Any(x => x == "algorithmic") && !revertedMethods.Any(x => x == "algorithmic"))
            {
                var emptyExpressionXml = BuildExpressionXml(new List<AlgorithmicExpression>());
                _algorithmicQtiItemRepository.AlgorithmicSaveExpression(qtiItem.QTIItemID, virtualQuestionId, emptyExpressionXml, currentUserId);
            }
        }

        public string BuildExpressionXml(List<AlgorithmicExpression> expressions)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(List<AlgorithmicExpression>));
                xs.Serialize(sw, expressions);
                try
                {
                    var xml = sw.ToString();
                    return xml;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public string BuildMultiPartExpressionXml(List<MultiPartExpressionDto> expressions)
        {
            using (var sw = new StringWriter())
            {
                var xs = new XmlSerializer(typeof(List<MultiPartExpressionDto>));
                xs.Serialize(sw, expressions);
                try
                {
                    var xml = sw.ToString();
                    return xml;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        private bool MustRemoveRubricData(QTIItemData qtiItem, IList<string> currentMethods, IList<string> revertedMethods)
        {
            if (qtiItem.QTISchemaID != (int)QTISchemaEnum.ExtendedText)
            {
                return false;
            }

            if (!currentMethods.Any(x => x == "rubricBasedGrading"))
            {
                return false;
            }

            if (!revertedMethods.Any(x => x == "rubricBasedGrading"))
            {
                return true;
            }

            return false;
        }

        private void SaveQtiItemHistory(QTIItemData qtiItem)
        {
            var itemHistory = new QTIItemHistory
            {
                ChangedDate = qtiItem.Updated ?? qtiItem.CreatedDate,
                QTIItemID = qtiItem.QTIItemID,
                XmlContent = qtiItem.XmlContent,
                AuthorID = qtiItem.UpdatedByUserID ?? qtiItem.UserID,
            };

            _qtiItemQtiItemHistoryRepository.Save(itemHistory);
        }

        private void SaveQtiItem(int currentUserId, QTIItemData qtiItem, QTIItemHistory qtiItemHistory, QTIItemTestMaker qtiItemTestTaker)
        {
            qtiItem.AnswerIdentifiers = qtiItemTestTaker.AnswerIdentifiers;
            qtiItem.PointsPossible = qtiItemTestTaker.PointsPossible;
            qtiItem.QTISchemaID = qtiItemTestTaker.QTISchemaID;
            qtiItem.ResponseIdentifier = qtiItemTestTaker.ResponseIdentifier;
            qtiItem.ResponseProcessing = qtiItemTestTaker.ResponseProcessing;
            qtiItem.CorrectAnswer = qtiItemTestTaker.CorrectAnswer;
            qtiItem.ResponseProcessingTypeID = qtiItemTestTaker.ResponseProcessingTypeID;
            qtiItem.XmlContent = qtiItemTestTaker.XmlContent;
            qtiItem.UpdatedByUserID = currentUserId;
            qtiItem.Updated = DateTime.UtcNow;
            qtiItem.RevertedFromQTIItemHistoryID = qtiItemHistory.QTIItemHistoryID;

            _qtiItemQtiItemRepository.Save(qtiItem);
        }

        private void SaveQTIITemSub(int qtiItemId, QTIItemTestMaker qtiItemTestTaker, IQueryable<VirtualQuestionData> virtualQuestions)
        {
            var oldQtiItemSubs = _qtiItemSubReadOnlyRepository.Select().Where(en => en.QTIItemId == qtiItemId);
            foreach (var oldQtiItemSub in oldQtiItemSubs)
            {
                // Update old QtiItemSub having the same ResponeIdentifier
                if (qtiItemTestTaker.QTIItemSubTestMakers != null && qtiItemTestTaker.QTIItemSubTestMakers.Any(en => en.ResponseIdentifier == oldQtiItemSub.ResponseIdentifier))
                {
                    var newQtiItemSub =
                        qtiItemTestTaker.QTIItemSubTestMakers.SingleOrDefault(
                            en => en.ResponseIdentifier == oldQtiItemSub.ResponseIdentifier);
                    oldQtiItemSub.CorrectAnswer = newQtiItemSub.CorrectAnswer.ConvertFromUnicodeToWindow1252();
                    oldQtiItemSub.CorrectAnswer = ServiceUtil.RollbackSpace(oldQtiItemSub.CorrectAnswer);
                    oldQtiItemSub.PointsPossible = newQtiItemSub.PointsPossible;
                    oldQtiItemSub.QTISchemaId = newQtiItemSub.QTISchemaID;
                    oldQtiItemSub.ResponseIdentifier = newQtiItemSub.ResponseIdentifier;
                    oldQtiItemSub.ResponseProcessing = ServiceUtil.RollbackSpace(newQtiItemSub.ResponseProcessing);
                    oldQtiItemSub.ResponseProcessingTypeId = newQtiItemSub.ResponseProcessingTypeID;
                    oldQtiItemSub.Updated = DateTime.Now;
                    oldQtiItemSub.Major = newQtiItemSub.Major;
                    oldQtiItemSub.Depending = newQtiItemSub.Depending;
                    _qtiItemSubRepository.Save(oldQtiItemSub);

                    // Update PointsPossible of VirtualQuestionSub
                    var virtualQuestionSubs =
                        _virtualQuestionSubRepository.Select().Where(en => en.QTIItemSubId == oldQtiItemSub.QTIItemSubId);
                    foreach (var virtualQuestionSub in virtualQuestionSubs)
                    {
                        virtualQuestionSub.PointsPossible = oldQtiItemSub.PointsPossible;
                        _virtualQuestionSubRepository.Save(virtualQuestionSub);
                    }
                }
                else
                {
                    // Delete VirtualQuestionSub of this QtiItemSub
                    var virtualQuestionSubs =
                        _virtualQuestionSubRepository.Select().Where(en => en.QTIItemSubId == oldQtiItemSub.QTIItemSubId);
                    foreach (var virtualQuestionSub in virtualQuestionSubs)
                    {
                        _virtualQuestionSubRepository.Delete(virtualQuestionSub);
                    }

                    // Delete this QtiItemSub
                    _qtiItemSubRepository.Delete(oldQtiItemSub);
                }
            }

            // Insert new qtiItemSubs and qtiItemAnswerScores
            var qtiItemSubs = qtiItemTestTaker.QTIItemSubTestMakers;
            if (qtiItemSubs != null)
            {
                foreach (var qtiItemSubTestMaker in qtiItemSubs)
                {
                    if (!oldQtiItemSubs.Any(en => en.ResponseIdentifier == qtiItemSubTestMaker.ResponseIdentifier))
                    {
                        var qtiItemSubData = new QTIItemSub
                        {
                            CorrectAnswer = qtiItemSubTestMaker.CorrectAnswer.ConvertFromUnicodeToWindow1252(),
                            PointsPossible = qtiItemSubTestMaker.PointsPossible,
                            QTIItemId = qtiItemId,
                            QTISchemaId = qtiItemSubTestMaker.QTISchemaID,
                            ResponseIdentifier = qtiItemSubTestMaker.ResponseIdentifier,
                            ResponseProcessing = qtiItemSubTestMaker.ResponseProcessing,
                            ResponseProcessingTypeId = qtiItemSubTestMaker.ResponseProcessingTypeID,
                            SourceId = null,
                            Updated = DateTime.Now,
                            Depending = qtiItemSubTestMaker.Depending,
                            Major = qtiItemSubTestMaker.Major
                        };
                        qtiItemSubData.CorrectAnswer = ServiceUtil.RollbackSpace(qtiItemSubData.CorrectAnswer);
                        qtiItemSubData.ResponseProcessing = ServiceUtil.RollbackSpace(qtiItemSubData.ResponseProcessing);
                        SaveItemSub(qtiItemSubData);

                        // Insert VirtualQuestionSub for each virtual question
                        foreach (var virtualQuestion in virtualQuestions)
                        {
                            var virtualQuestionSub =
                                new VirtualQuestionSub
                                {
                                    PointsPossible = qtiItemSubData.PointsPossible,
                                    QTIItemSubId = qtiItemSubData.QTIItemSubId,
                                    VirtualQuestionId = virtualQuestion.VirtualQuestionID
                                };
                            _virtualQuestionSubRepository.Save(virtualQuestionSub);
                        }
                    }
                }
            }
        }

        private void SaveQTIItemAnswerScores(int qtiItemId, QTIItemTestMaker qtiItemTestTaker, IQueryable<VirtualQuestionData> virtualQuestions)
        {
            // Delete old QtiItemAnswerScores and VirtualQuestionAnswerScore
            _qtiItemQtiItemRepository.DeleteQtiItemAnswerScoreAndVirtualQuestionAnswerScore(qtiItemId);//Optimize:use stored proc

            // Add new QtiItemAnswerScore and VirtualQuestionAnswerScore
            var qtiItemAnswerScores = qtiItemTestTaker.QTIITemAnswerScoreTestMakers;
            if (qtiItemAnswerScores != null)
            {
                foreach (var qtiItemAnswerScoreTestMaker in qtiItemAnswerScores)
                {
                    var qtiItemAnswerScoreData = new QTIItemAnswerScore()
                    {
                        Answer = qtiItemAnswerScoreTestMaker.Answer.ConvertFromUnicodeToWindow1252(),
                        QTIItemId = qtiItemId,
                        ResponseIdentifier = qtiItemAnswerScoreTestMaker.ResponseIdentifier,
                        Score = qtiItemAnswerScoreTestMaker.Score.ToString(),
                        AnswerText = qtiItemAnswerScoreTestMaker.AnswerText
                    };
                    qtiItemAnswerScoreData.Answer = ServiceUtil.RollbackSpace(qtiItemAnswerScoreData.Answer);
                    SaveAnswerScore(qtiItemAnswerScoreData);

                    // Insert VirtualQuestionAnswerScore for each virtual question
                    foreach (var virtualQuestion in virtualQuestions)
                    {
                        var virtualQuestionAnswerScore =
                            new VirtualQuestionAnswerScore
                            {
                                QTIItemAnswerScoreId = qtiItemAnswerScoreData.QTIItemAnswerScoreId,
                                Score = string.IsNullOrEmpty(qtiItemAnswerScoreData.Score) ? 0 : Convert.ToInt32(qtiItemAnswerScoreData.Score),
                                VirtualQuestionId = virtualQuestion.VirtualQuestionID
                            };
                        _virtualQuestionAnswerScoreRepository.Save(virtualQuestionAnswerScore);
                    }
                }
            }
        }

        public List<QTIItemData> GetQtiItemByQtiBankId(int bankId)
        {
            return _qtiItemQtiItemRepository.Select().Where(x => x.QTIGroupID == bankId).ToList();
        }

        public void UpdateQtiItem(QTIItemData item)
        {
            _qtiItemQtiItemRepository.Save(item);
        }

        public IQueryable<QTIItemData> GetAllQtiItem()
        {
            return _qtiItemQtiItemRepository.Select();
        }

        public QTI3pItem GetQti3pItemById(int id)
        {
            return _itemRepository.Select().FirstOrDefault(x => x.QTI3pItemID == id);
        }

        public IQueryable<QTI3pItem> GetQti3pItemByIds(IEnumerable<int> ids)
        {
            return _itemRepository.Select().Where(x => ids.Contains(x.QTI3pItemID));
        }

        public QTIItemData CreateQTIItem(QTIItemTestMaker qtiItemTemTestMaker, bool? noDuplicateAnswers, int? dataFileUploadTypeId = null, int? questionOrder = null, string description = "")
        {
            if (qtiItemTemTestMaker == null) return null;

            var qtiItem = new QTIItemData
            {
                QTIGroupID = qtiItemTemTestMaker.QTIGroupID,
                UserID = qtiItemTemTestMaker.UserID,
                Title = qtiItemTemTestMaker.Title,

                AnswerIdentifiers = qtiItemTemTestMaker.AnswerIdentifiers,

                PointsPossible = qtiItemTemTestMaker.PointsPossible,
                QTISchemaID = qtiItemTemTestMaker.QTISchemaID,
                ResponseIdentifier = qtiItemTemTestMaker.ResponseIdentifier,
                ResponseProcessing = qtiItemTemTestMaker.ResponseProcessing,
                CorrectAnswer = qtiItemTemTestMaker.CorrectAnswer.ConvertFromUnicodeToWindow1252(),
                ResponseProcessingTypeID = qtiItemTemTestMaker.ResponseProcessingTypeID,
                XmlContent = qtiItemTemTestMaker.XmlContent,
                FilePath = "",
                UrlPath = "",
                Updated = DateTime.UtcNow,
                DataFileUploadTypeId = dataFileUploadTypeId,
                CreatedDate = DateTime.UtcNow,
                Description = description
            };
            qtiItem.ResponseProcessing = ServiceUtil.RollbackSpace(qtiItem.ResponseProcessing);
            qtiItem.CorrectAnswer = ServiceUtil.RollbackSpace(qtiItem.CorrectAnswer);
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.UploadComposite)
            {
                if (noDuplicateAnswers.HasValue && noDuplicateAnswers.Value == true)
                {
                    qtiItem.ResponseProcessingTypeID = 5;//LNKT-9040:In Flash version, QTIItem record with this option checked will have ResponseProcessingTypeID = 5
                }
            }
            Save(qtiItem, questionOrder);

            var qtiItemSubs = qtiItemTemTestMaker.QTIItemSubTestMakers;
            if (qtiItemSubs != null)
            {
                foreach (var qtiItemSubTestMaker in qtiItemSubs)
                {
                    var qtiItemSubData = new QTIItemSub
                    {
                        CorrectAnswer = qtiItemSubTestMaker.CorrectAnswer.ConvertFromUnicodeToWindow1252(),
                        PointsPossible = qtiItemSubTestMaker.PointsPossible,
                        QTIItemId = qtiItem.QTIItemID,
                        QTISchemaId = qtiItemSubTestMaker.QTISchemaID,
                        ResponseIdentifier = qtiItemSubTestMaker.ResponseIdentifier,
                        ResponseProcessing = qtiItemSubTestMaker.ResponseProcessing,
                        ResponseProcessingTypeId = qtiItemSubTestMaker.ResponseProcessingTypeID,
                        SourceId = null,
                        Updated = DateTime.Now,
                        Depending = qtiItemSubTestMaker.Depending,
                        Major = qtiItemSubTestMaker.Major
                    };
                    qtiItemSubData.CorrectAnswer = ServiceUtil.RollbackSpace(qtiItemSubData.CorrectAnswer);
                    qtiItemSubData.ResponseProcessing = ServiceUtil.RollbackSpace(qtiItemSubData.ResponseProcessing);
                    SaveItemSub(qtiItemSubData);
                }
            }

            var qtiItemAnswerScores = qtiItemTemTestMaker.QTIITemAnswerScoreTestMakers;
            if (qtiItemAnswerScores != null)
            {
                foreach (var qtiItemAnswerScoreTestMaker in qtiItemAnswerScores)
                {
                    var qtiItemAnswerScoreData = new QTIItemAnswerScore()
                    {
                        Answer = qtiItemAnswerScoreTestMaker.Answer.ConvertFromUnicodeToWindow1252(),
                        QTIItemId = qtiItem.QTIItemID,
                        ResponseIdentifier = qtiItemAnswerScoreTestMaker.ResponseIdentifier,
                        Score = qtiItemAnswerScoreTestMaker.Score.ToString(),
                        AnswerText = qtiItemAnswerScoreTestMaker.AnswerText
                    };
                    qtiItemAnswerScoreData.Answer = ServiceUtil.RollbackSpace(qtiItemAnswerScoreData.Answer);
                    SaveAnswerScore(qtiItemAnswerScoreData);
                }
            }

            return qtiItem;
        }

        public QTIItemData UpdateQTIItem(int currentUserId, int qtiItemId, QTIItemTestMaker qtiItemTemTestMaker, bool? noDuplicateAnswers, int? resetRubric, string title, string description)
        {
            if (qtiItemTemTestMaker == null) return null;

            // Update QtiItem data
            var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(en => en.QTIItemID == qtiItemId);

            if (qtiItem.QTISchemaID != qtiItemTemTestMaker.QTISchemaID || qtiItem.XmlContent != qtiItemTemTestMaker.XmlContent)
            {
                SaveQtiItemHistory(qtiItem);
            }

            qtiItem.Title = title;
            qtiItem.Description = description;
            qtiItem.AnswerIdentifiers = qtiItemTemTestMaker.AnswerIdentifiers;
            qtiItem.PointsPossible = qtiItemTemTestMaker.PointsPossible;
            qtiItem.QTISchemaID = qtiItemTemTestMaker.QTISchemaID;
            qtiItem.ResponseIdentifier = qtiItemTemTestMaker.ResponseIdentifier;
            qtiItem.ResponseProcessing = qtiItemTemTestMaker.ResponseProcessing;
            qtiItem.ResponseProcessing = ServiceUtil.RollbackSpace(qtiItem.ResponseProcessing);
            qtiItem.CorrectAnswer = qtiItemTemTestMaker.CorrectAnswer.ConvertFromUnicodeToWindow1252();
            qtiItem.CorrectAnswer = ServiceUtil.RollbackSpace(qtiItem.CorrectAnswer);
            qtiItem.ResponseProcessingTypeID = qtiItemTemTestMaker.ResponseProcessingTypeID;
            qtiItem.XmlContent = qtiItemTemTestMaker.XmlContent;
            qtiItem.Updated = DateTime.UtcNow;
            qtiItem.UpdatedByUserID = currentUserId;
            qtiItem.RevertedFromQTIItemHistoryID = null;
            if (qtiItem.QTISchemaID == (int)QTISchemaEnum.UploadComposite && noDuplicateAnswers == true)
            {
                qtiItem.ResponseProcessingTypeID = 5;//LNKT-9040:In Flash version, QTIItem record with this option checked will have ResponseProcessingTypeID = 5
            }
            _qtiItemQtiItemRepository.Save(qtiItem);

            _qtiItemQtiItemRepository.UpdateQtiItemVirtualQuestion(qtiItemId, qtiItem.PointsPossible, resetRubric);

            var virtualQuestions = _virtualQuestionRepository.Select().Where(en => en.QTIItemID == qtiItemId);
            SaveQTIITemSub(qtiItemId, qtiItemTemTestMaker, virtualQuestions);
            SaveQTIItemAnswerScores(qtiItemId, qtiItemTemTestMaker, virtualQuestions);

            return qtiItem;
        }

        public bool IsHavingStudentTakeTest(IEnumerable<(string QtiItemId, string ExtendedText)> qtiItemAnswerKeys)
        {
            var qtiItemAnswerKeyPairs = qtiItemAnswerKeys
                .Select(x => new
                {
                    QtiItemId = int.TryParse(x.QtiItemId, out var qtiItemId) ? qtiItemId : default(int?),
                    QtiSchemaId = bool.TryParse(x.ExtendedText, out var isExtendedText) && isExtendedText ?
                        (int)QTISchemaEnum.ExtendedText : (int)QTISchemaEnum.Choice
                })
                .Where(x => x.QtiItemId.HasValue)
                .GroupBy(x => x.QtiItemId.Value)
                .ToDictionary(x => x.Key, x => x.First().QtiSchemaId);

            var qtiItems = _qtiItemQtiItemRepository
                .Select()
                .Where(x => qtiItemAnswerKeyPairs.Keys.Contains(x.QTIItemID))
                .Select(x => new { x.QTIItemID, x.QTISchemaID });

            foreach (var item in qtiItems)
            {
                if (qtiItemAnswerKeyPairs.TryGetValue(item.QTIItemID, out var qtiSchemaId) && qtiSchemaId != item.QTISchemaID)
                {
                    var isHavingStudentTakeTest = _qtiItemQtiItemRepository.IsHavingStudentTakeTest(item.QTIItemID);
                    if (isHavingStudentTakeTest) return true;
                }
            }
            return false;
        }

        public bool CheckIsConflictConstraintUpdate(QTIItemCheckConflictConstrainParameter param)
        {
            var isHavingStudentTakeTest = _qtiItemQtiItemRepository.IsHavingStudentTakeTest(param.QtiItemId);
            var isHavingRetake = param.IsVirtualTestHasRetakeRequest.HasValue && param.IsVirtualTestHasRetakeRequest == true;
            if (isHavingStudentTakeTest || isHavingRetake)
            {
                if (param.IsChangeAnswerChoice) return true;

                var qtiItemTemTestMaker = _qTIItemConvert.ConvertFromXmlContent(param.XmlContent);
                var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(en => en.QTIItemID == param.QtiItemId);
                if (qtiItem != null)
                {
                    if (qtiItem.QTISchemaID == (int)QTISchemaEnum.UploadComposite)
                    {
                        var oldQtiItemSubs = _qtiItemSubReadOnlyRepository.Select().Where(en => en.QTIItemId == param.QtiItemId);
                        if (qtiItemTemTestMaker.QTIItemSubTestMakers == null || oldQtiItemSubs == null || qtiItemTemTestMaker.QTIItemSubTestMakers.Count < oldQtiItemSubs.Count())
                            return true;

                        if (isHavingRetake && qtiItemTemTestMaker.QTIItemSubTestMakers.Count > oldQtiItemSubs.Count())
                            return true;

                        foreach (var qtiItemSub in oldQtiItemSubs)
                        {
                            if (qtiItemTemTestMaker.QTIItemSubTestMakers != null && qtiItemTemTestMaker.QTIItemSubTestMakers.Any())
                            {
                                var qtiitemSubNew =
                                        qtiItemTemTestMaker.QTIItemSubTestMakers.FirstOrDefault(x => x.ResponseIdentifier == qtiItemSub.ResponseIdentifier);
                                if (qtiitemSubNew != null)
                                {
                                    var isConflict = IsConflictConstraintUpdate(qtiItemSub.QTISchemaId, qtiitemSubNew.QTISchemaID, qtiItem.XmlContent, param.XmlContent, qtiItemSub.ResponseIdentifier);
                                    if (isConflict) return true;
                                }
                            }
                        }
                    }
                    else
                    
                        return IsConflictConstraintUpdate(qtiItem.QTISchemaID, qtiItemTemTestMaker.QTISchemaID, qtiItem.XmlContent, param.XmlContent, qtiItem.ResponseIdentifier);
                    
                }
            }
            return false;
        }

        private bool IsConflictConstraintUpdate(int qtiSchemaIdOld, int qtiSchemaIdNew, string xmlContentOld, string xmlContentNew, string responseIdentifier)
        {
            var listSchemaIdValid = new List<int>() { 1, 3 };
            if (listSchemaIdValid.Contains(qtiSchemaIdOld) && listSchemaIdValid.Contains(qtiSchemaIdNew))
            {
                var isTrueFalseQuestionOld = false;
                var isTrueFalseQuestionNew = false;
                if (qtiSchemaIdOld == 1)
                    isTrueFalseQuestionOld = _qTIItemConvert.IsTrueFalseQuestion(xmlContentOld, responseIdentifier);
                if (qtiSchemaIdNew == 1)
                    isTrueFalseQuestionNew = _qTIItemConvert.IsTrueFalseQuestion(xmlContentNew, responseIdentifier);

                if ((isTrueFalseQuestionOld && !isTrueFalseQuestionNew) || (!isTrueFalseQuestionOld && isTrueFalseQuestionNew))
                    return true;
            }
            else if (qtiSchemaIdOld != qtiSchemaIdNew)
                return true;

            return false;
        }

        public List<StateStandardSubject> GetStandardSubjects(string stateCode)
        {
            return _qtiItemStateStandardRepository.GetStandardSubjects(stateCode);
        }

        public List<StateSubjectGrade> GetGradeByStateCodeAndSubject(string stateCode, string subject)
        {
            return _qtiItemStateStandardRepository.GetGradeByStateCodeAndSubject(stateCode, subject);
        }

        public IQueryable<PassageGradeQti> GetPassageQtiGrade()
        {
            return _passageGradeQtiRepository.Select();
        }

        public List<State> GetStatesQTIItem(int? userId, int? districtId, int? userIdStateForUser)
        {
            return _qtiItemStateStandardRepository.GetStatesQTIItem(userId, districtId, userIdStateForUser);
        }

        public List<State> GetStatesQTI3pItem(int? qti3pSourceId, int? userId)
        {
            return _qtiItemStateStandardRepository.GetStatesQTI3pItem(qti3pSourceId, userId);
        }

        public List<StateStandardSubject> GetStateStandardSubjectsForItemLibraryFilter(string stateCode, int? userId, int? districtId)
        {
            return _qtiItemStateStandardRepository.GetStateStandardSubjectsForItemLibraryFilter(stateCode, userId, districtId);
        }

        public List<StateSubjectGrade> GetGradesByStateAndSubjectForItemLibraryFilter(string stateCode, string subject, int? userId, int? districtId)
        {
            return _qtiItemStateStandardRepository.GetGradesByStateAndSubjectForItemLibraryFilter(stateCode, subject, userId, districtId);
        }

        public void SaveAnswerKey(int qtiItemId, string newCorrectAnswer, int newNumberOfChoice, int? newQTIItemPoints, bool isExtendedText, out string error, out int questionOrder)
        {
            error = string.Empty;
            questionOrder = 0;
            var qtiItem = _qtiItemQtiItemRepository.Select().First(x => x.QTIItemID == qtiItemId);
            if (qtiItem == null)
            {
                error = "Can not find item id=" + qtiItemId;
                return;
            }
            questionOrder = qtiItem.QuestionOrder;
            //Only update for Multiple Choice and Extended Text
            if (qtiItem.QTISchemaID != (int)QTISchemaEnum.Choice && qtiItem.QTISchemaID != (int)QTISchemaEnum.ExtendedText)
            {
                return;
            }

            string originalXmlContent = qtiItem.XmlContent;
            int originalQTISchemaID = qtiItem.QTISchemaID;
            string originalCorrectAnswer = qtiItem.CorrectAnswer;
            int originalPointsPossible = qtiItem.PointsPossible;
            string originalAnswerIdentifiers = qtiItem.AnswerIdentifiers;
            int newPoints = newQTIItemPoints ?? originalPointsPossible;

            int newSchemaId = (int)QTISchemaEnum.Choice; //QTISchemaId = 1 - Multi select - single choice
            if (isExtendedText)
            {
                newSchemaId = (int)QTISchemaEnum.ExtendedText;
                //QTISchemaId = 10. There'are only two editable answer key are SchemaId = 1 and SchemaId = 10
                newCorrectAnswer = "O"; //Default CorrectAnswer of QtiSchemaID = 10 is always 'O'
            }
            string answerIdentifiers = string.Empty;
            newCorrectAnswer = newCorrectAnswer.ToUpper();
            if (newSchemaId == (int)QTISchemaEnum.Choice)
            {
                if (!CheckAndGetAnswerIdentifiers(newCorrectAnswer, newNumberOfChoice, out answerIdentifiers))
                {
                    error = string.Format("Question Order #{0}: Correct Answer is invalid.", qtiItem.QuestionOrder);
                    return;
                }
            }

            //Step 2: Check if there's any change
            if (qtiItem.QTISchemaID != newSchemaId || qtiItem.CorrectAnswer != newCorrectAnswer ||
                qtiItem.AnswerIdentifiers != answerIdentifiers || qtiItem.PointsPossible != newPoints)
            {
                try
                {
                    //Convert from Multiple Choice to Exteneded Text
                    if (qtiItem.QTISchemaID == (int)QTISchemaEnum.Choice && newSchemaId == (int)QTISchemaEnum.ExtendedText)
                    {
                        try
                        {
                            var xmlContentProcessing = new XmlContentProcessing(qtiItem.XmlContent);
                            xmlContentProcessing.ConvertXmlContentFromMultipleChoiceToExtendedText(newPoints);
                            qtiItem.XmlContent = xmlContentProcessing.GetXmlContent();
                        }
                        catch (Exception)
                        {
                            error = string.Format("Question Order #{0}: Can not convert to Extended Text right now.", qtiItem.QuestionOrder);
                            return;
                        }
                    }
                    if (qtiItem.QTISchemaID == (int)QTISchemaEnum.ExtendedText &&
                        newSchemaId == (int)QTISchemaEnum.Choice)
                    {
                        try
                        {
                            qtiItem.XmlContent = ConvertXmlContentFromExtendedTextToMultipleChoice(qtiItem.XmlContent, newPoints, newCorrectAnswer, answerIdentifiers);
                        }
                        catch (Exception)
                        {
                            error = string.Format("Question Order #{0}: Can not convert to Multiple Choice right now.", qtiItem.QuestionOrder);
                            return;
                        }
                    }
                    //If Points has been changed
                    if (qtiItem.PointsPossible != newPoints)
                    {
                        try
                        {
                            qtiItem.XmlContent = UpdateXmlContentToUpdatePoint(qtiItem.XmlContent, newPoints);
                        }
                        catch (Exception)
                        {
                            error = string.Format("Question Order #{0}: Can not update new Points right now.", qtiItem.QuestionOrder);
                            return;
                        }
                    }
                    //Multiple Choice: If Number of Choice or Correct Answer has been changed
                    if (qtiItem.QTISchemaID == (int)QTISchemaEnum.Choice && newSchemaId == (int)QTISchemaEnum.Choice &&
                        (qtiItem.AnswerIdentifiers != answerIdentifiers || qtiItem.CorrectAnswer != newCorrectAnswer))
                    {
                        try
                        {
                            qtiItem.XmlContent = UpdateXmlContentOfMultipleChoiceForNumberOfChoiceAndCorrectAnswer(qtiItem.XmlContent, qtiItem.PointsPossible,
                                                                                       qtiItem.CorrectAnswer, newCorrectAnswer,
                                                                                       qtiItem.AnswerIdentifiers, answerIdentifiers);
                        }
                        catch (Exception)
                        {
                            error = string.Format("Question Order #{0}: Can not change Correct Answer, Number of Choice right now.", qtiItem.QuestionOrder);
                            return;
                        }
                    }
                    //Check ResponseIdentifer
                    if (string.IsNullOrWhiteSpace(qtiItem.ResponseIdentifier))
                    {
                        try
                        {
                            qtiItem.ResponseIdentifier = GetResponseIdentifierFromXmlContent(qtiItem.XmlContent);
                        }
                        catch (Exception)
                        {
                            qtiItem.ResponseIdentifier = string.Empty;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(qtiItem.ResponseIdentifier))
                    {
                        error = string.Format("Question Order #{0}: Can not define Response Identifer.", qtiItem.QuestionOrder);
                        return;
                    }
                    //Need update QtiItem.XmlContent and QTIItemAnswerScore
                    var orginalVirtualQuestionAnswerScore = new List<VirtualQuestionAnswerScore>();

                    //According to Flash: Flash will delete QTIItemAnswerScore and insert a new record for any update, a VirtualQuestionAnswerScore which reference to QTIItemAnswerScore will be deleted and then insert a new record along with new  QTIItemAnswerScore
                    if (qtiItem.CorrectAnswer != newCorrectAnswer || qtiItem.PointsPossible != newPoints)
                    {
                        var qtiItemAnswerScore =
                            _qtiItemAnswerScoreRepository.Select().FirstOrDefault(en => en.QTIItemId == qtiItemId);

                        try
                        {
                            if (qtiItemAnswerScore != null)
                            {
                                //Get all VirtualQuestionAnswerScore that reference to QTIItemAnswerScore
                                orginalVirtualQuestionAnswerScore = _virtualQuestionAnswerScoreRepository.Select().Where(
                                    x => x.QTIItemAnswerScoreId == qtiItemAnswerScore.QTIItemAnswerScoreId).ToList();
                                //Delete all VirtualQuestionAnswerScore that reference to QTIItemAnswerScore
                                foreach (var virtualQuestionAnswerScore in orginalVirtualQuestionAnswerScore)
                                {
                                    _virtualQuestionAnswerScoreRepository.Delete(virtualQuestionAnswerScore);
                                }

                                _qtiItemAnswerScoreInsertDeleteRepository.Delete(qtiItemAnswerScore);
                            }
                        }
                        catch (Exception)
                        {
                            error = string.Format("Question Order #{0}: Can not delete QtiItemAnswerScore for updating.", qtiItem.QuestionOrder);
                            return;
                        }

                        try
                        {
                            var newQtiItemAnswerScore = new QTIItemAnswerScore();
                            newQtiItemAnswerScore.QTIItemId = qtiItem.QTIItemID;
                            newQtiItemAnswerScore.ResponseIdentifier = qtiItem.ResponseIdentifier;
                            newQtiItemAnswerScore.Answer = newCorrectAnswer;
                            newQtiItemAnswerScore.Score = newPoints.ToString();

                            _qtiItemAnswerScoreInsertDeleteRepository.Save(newQtiItemAnswerScore);
                            //Insert all orginal VirtualQuestionAnswerScore that reference to QTIItemAnswerScore
                            foreach (var virtualQuestionAnswerScore in orginalVirtualQuestionAnswerScore)
                            {
                                var newVirtualQuestionAnswerScore = new VirtualQuestionAnswerScore
                                {
                                    VirtualQuestionId = virtualQuestionAnswerScore.VirtualQuestionId,
                                    QTIItemAnswerScoreId = newQtiItemAnswerScore.QTIItemAnswerScoreId,
                                    Score = int.Parse(newQtiItemAnswerScore.Score)
                                };
                                _virtualQuestionAnswerScoreRepository.Save(newVirtualQuestionAnswerScore);
                                //Need to update Point Possible for VirtualQuestion
                                var virtualQuestion =
                                    _virtualQuestionRepository.Select().FirstOrDefault(
                                        x => x.VirtualQuestionID == newVirtualQuestionAnswerScore.VirtualQuestionId);
                                if (virtualQuestion != null)
                                {
                                    virtualQuestion.PointsPossible = newVirtualQuestionAnswerScore.Score;
                                    //for schemaid = 1 and 10, VirtualQuestionAnswerScore.Score is the same as virtualQuestion.PointsPossible
                                    _virtualQuestionRepository.Save(virtualQuestion);
                                }
                            }
                        }
                        catch (Exception)
                        {
                            error = string.Format("Question Order #{0}: Can not insert QtiItemAnswerScore for updating.", qtiItem.QuestionOrder);
                            //recover the deleted record QtiItemAnswerScore if it was failed to insert new record
                            if (qtiItemAnswerScore != null)
                            {
                                _qtiItemAnswerScoreInsertDeleteRepository.Save(qtiItemAnswerScore);

                                //Recoverd all deleted VirtualQuestionAnswerScore that reference to QTIItemAnswerScore
                                foreach (var virtualQuestionAnswerScore in orginalVirtualQuestionAnswerScore)
                                {
                                    var newVirtualQuestionAnswerScore = new VirtualQuestionAnswerScore
                                    {
                                        VirtualQuestionId = virtualQuestionAnswerScore.VirtualQuestionId,
                                        QTIItemAnswerScoreId = qtiItemAnswerScore.QTIItemAnswerScoreId,
                                        Score = int.Parse(qtiItemAnswerScore.Score)
                                    };
                                    _virtualQuestionAnswerScoreRepository.Save(newVirtualQuestionAnswerScore);
                                }
                            }

                            return;
                        }
                    }

                    //According to Flash, a Extended Text will have CorrectAnswer is '0' and AnswerIdentifiers is empty
                    qtiItem.QTISchemaID = newSchemaId;
                    qtiItem.CorrectAnswer = newCorrectAnswer;
                    qtiItem.PointsPossible = newPoints;
                    qtiItem.AnswerIdentifiers = answerIdentifiers;

                    try
                    {
                        _qtiItemQtiItemRepository.Save(qtiItem);
                    }
                    catch (Exception)
                    {
                        error = string.Format("There was some error. Can not save Question Order #{0}",
                                          qtiItem.QuestionOrder);
                        //recover the orginal value
                        qtiItem.XmlContent = originalXmlContent;
                        qtiItem.QTISchemaID = originalQTISchemaID;
                        qtiItem.CorrectAnswer = originalCorrectAnswer;
                        qtiItem.PointsPossible = originalPointsPossible;
                        qtiItem.AnswerIdentifiers = originalAnswerIdentifiers;

                        _qtiItemQtiItemRepository.Save(qtiItem);

                        var qtiItemAnswerScore =
                            _qtiItemAnswerScoreRepository.Select().FirstOrDefault(en => en.QTIItemId == qtiItemId);

                        if (qtiItemAnswerScore != null)
                        {
                            qtiItemAnswerScore.Answer = originalCorrectAnswer;
                            qtiItemAnswerScore.Score = originalPointsPossible.ToString();
                            _qtiItemAnswerScoreInsertDeleteRepository.Save(qtiItemAnswerScore);
                        }
                    }
                }
                catch (Exception)
                {
                    error = string.Format("There was some error. Can not save Question Order #{0}",
                                          qtiItem.QuestionOrder);
                }
            }
        }

        public bool CheckAndGetAnswerIdentifiers(string correctAnswer, int numberOfChoice, out string answerIdentifiers)
        {
            answerIdentifiers = string.Empty;
            List<string> alphabetAnswerIdentifiers =
                new List<string>(new string[]
                                     {
                                         "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"
                                         , "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF",
                                         "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP"
                                     }); //Flash does not use 'U'
            List<string> numberAnswerIdentifiers =
                new List<string>(new string[]
                                     {
                                         "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14",
                                         "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27",
                                         "28", "29", "30", "31", "32", "33", "34", "35", "36", "37", "38", "39", "40", "41"
                                     });
            int temp;

            if (int.TryParse(correctAnswer, out temp))
            {
                //Answer is number
                for (int i = 0; i < numberOfChoice; i++)
                {
                    answerIdentifiers += numberAnswerIdentifiers[i] + ";";
                }
            }
            else
            {
                for (int i = 0; i < numberOfChoice; i++)
                {
                    answerIdentifiers += alphabetAnswerIdentifiers[i] + ";";
                }
            }
            if (answerIdentifiers.Length > 0)
            {
                //remove the last
                answerIdentifiers = answerIdentifiers.Remove(answerIdentifiers.Length - 1, 1);
            }
            //check if correctAnswer is in answerIdentifiers or not
            var answers = answerIdentifiers.Split(';');
            return answers.Any(x => x.Equals(correctAnswer));
        }

        public void SaveAnswerKeyByUploadedFile(int qtiItemGroupId, StreamReader fileStream, out bool success, out string errorMessage, out Dictionary<int, string> errorProcess)
        {
            success = true;
            errorMessage = string.Empty;
            errorProcess = new Dictionary<int, string>();
            //Read line by line in to a string
            List<string> lines = new List<string>();
            try
            {
                while (fileStream.Peek() >= 0)
                {
                    lines.Add(fileStream.ReadLine());
                }
            }
            catch (Exception)
            {
                success = false;
                errorMessage = "Can not read file content.";
            }

            //Ignore the first line, because the first line is the header line
            //Question Order	Correct Answer	Number of choice	Points	Extended Text (Y/N)
            for (int i = 1; i < lines.Count; i++)
            {
                //Process each line
                string[] line = new string[0];
                try
                {
                    line = lines[i].Split(',');
                }
                catch (Exception)
                {
                    errorProcess.Add(i, string.Format("Error on line {0}. Can not read Question Order, Correct Answer, Number of choice, Points, Extended Text (Y/N)", i + 1));
                }
                if (!errorProcess.Any(x => x.Key == i))
                {
                    if (line.Count() < 5)
                    {
                        errorProcess.Add(1, string.Format("Error on line {0}. Lack of information. There must be enough Question Order, Correct Answer, Number of choice, Points, Extended Text (Y/N)", i + 1));
                    }
                }
                int questionOrder = 0;
                if (!errorProcess.Any(x => x.Key == i))
                {
                    //Check question order
                    if (!int.TryParse(line[0], out questionOrder))
                    {
                        errorProcess.Add(i, string.Format("Error on line {0}. Question Order #{1} is invalid, it must be a number.", i + 1, line[0]));
                    }
                }
                int numberOfChoice = 0;
                if (!errorProcess.Any(x => x.Key == i))
                {
                    if (!int.TryParse(line[2], out numberOfChoice))
                    {
                        errorProcess.Add(i, string.Format("Error on line {0}. Question Order #{1}. Number of choice is invalid, it must be a number.", i + 1, questionOrder));
                    }
                    if (numberOfChoice > 25)
                    {
                        errorProcess.Add(i, string.Format("Error on line {0}. Question Order #{1}. Number of choice is invalid, max value is 25.", i + 1, questionOrder));
                    }
                }
                int points = 0;
                if (!errorProcess.Any(x => x.Key == i))
                {
                    if (!int.TryParse(line[3], out points))
                    {
                        errorProcess.Add(i, string.Format("Error on line {0}. Question Order #{1}. Points is invalid, it must be a number.", i + 1, questionOrder));
                    }
                }
                bool isExtendedText = false;
                if (!errorProcess.Any(x => x.Key == i))
                {
                    if (line[4].Trim().Length == 0 || line[4].Trim().Length > 1)
                    {
                        errorProcess.Add(i, string.Format("Error on line {0}.Question Order #{1}. Extended Text is invalid, it must be 'Y' or 'N'.", i + 1, questionOrder));
                    }
                    else
                    {
                        if (!(line[4].ToUpper().Equals("Y") || line[4].ToUpper().Equals("N")))
                        {
                            errorProcess.Add(i, string.Format("Error on line {0}.Question Order #{1}. Extended Text is invalid, it must be 'Y' or 'N'.", i + 1, questionOrder));
                        }
                        else
                        {
                            if (line[4].ToUpper().Equals("Y"))
                            {
                                isExtendedText = true;
                            }
                            if (line[4].ToUpper().Equals("N"))
                            {
                                isExtendedText = false;
                            }
                        }
                    }
                }
                var qtiItem = new QTIItemData();
                if (!errorProcess.Any(x => x.Key == i))
                {
                    if (_qtiItemQtiItemRepository.Select().Any(x => x.QTIGroupID == qtiItemGroupId && x.QuestionOrder == questionOrder))
                    {
                        qtiItem =
                            _qtiItemQtiItemRepository.Select().First(
                                x => x.QTIGroupID == qtiItemGroupId && x.QuestionOrder == questionOrder);
                    }
                    else
                    {
                        errorProcess.Add(i,
                                             string.Format(
                                                 "Error on line {0}.Question Order #{1}. There's no item in this group with Question Order {2}",
                                                 i + 1, questionOrder, questionOrder));
                    }
                }
                if (!errorProcess.Any(x => x.Key == i))
                {
                    try
                    {
                        string error;
                        SaveAnswerKey(qtiItem.QTIItemID,
                                      line[1],
                                      numberOfChoice,
                                      points,
                                      isExtendedText,
                                      out error,
                                      out questionOrder);
                        if (!string.IsNullOrEmpty(error))
                        {
                            errorProcess.Add(i, string.Format("Error on line {0}. Question Order {1}. {2}", i + 1, questionOrder, error));
                        }
                    }
                    catch (Exception ex)
                    {
                        errorProcess.Add(i, string.Format("Error on line {0}.Question Order {1}. {2}", i + 1, questionOrder, ex.Message));
                    }
                }
            }
            if (errorProcess.Any())
            {
                success = false;
            }
        }

        public string ConvertXmlContentFromExtendedTextToMultipleChoice(string xmlContent, int newPoint, string newCorrectAnswer, string answerIdentifiers)
        {
            //Made XmlContent change base on what Flash
            //Example:
            //extended text before converting to multiple choice
            //<assessmentItem xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd" adaptive="false" timeDependent="false" toolName="linkitTLF" toolVersion="1.0" qtiSchemeID="10" xmlns="http://www.imsglobal.org/xsd/imsqti_v2p0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><stylesheet href="stylesheet/linkitStyleSheet.css" type="text/css"/><responseDeclaration identifier="RESPONSE_2" baseType="string" cardinality="single" method="default" caseSensitive="false" type="string" pointsValue="4"/><itemBody><div styleName="mainBody" class="mainBody"><p><span>Extended Flash</span></p></div><extendedTextInteraction responseIdentifier="RESPONSE_2" expectedLength="200"/></itemBody></assessmentItem>

            //after converting to multiple choice
            //<assessmentItem xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd" adaptive="false" timeDependent="false" toolName="linkitTLF" toolVersion="1.0" qtiSchemeID="1" xmlns="http://www.imsglobal.org/xsd/imsqti_v2p0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"><stylesheet href="stylesheet/linkitStyleSheet.css" type="text/css"/><responseDeclaration identifier="RESPONSE_3" baseType="identifier" cardinality="single" method="default" caseSensitive="false" type="string" pointsValue="1"><correctResponse><value>A</value></correctResponse></responseDeclaration><itemBody><div styleName="mainBody" class="mainBody"><p><span>Extended Flash</span></p></div><choiceInteraction responseIdentifier="RESPONSE_3" shuffle="false" maxChoices="1"><simpleChoice identifier="A"><div class="answer" styleName="answer"><p><span>Answer A </span></p></div></simpleChoice><simpleChoice identifier="B"><div class="answer" styleName="answer"><p><span>Answer B</span></p></div></simpleChoice><simpleChoice identifier="C"><div class="answer" styleName="answer"><p><span>Answer C</span></p></div></simpleChoice><simpleChoice identifier="D"><div class="answer" styleName="answer"><p><span>Answer D</span></p></div></simpleChoice></choiceInteraction></itemBody></assessmentItem>
            //Base on that example (compare xmlcontent)=> What need to be done
            //1. Update pointsValue in tag responseDeclaration
            //2. Add tag correctResponse inside tag responseDeclaration,
            //3. Add tag choiceInteraction inside tag itemBody
            //4. Remove new tag <extendedTextInteraction responseIdentifier="RESPONSE_2" expectedLength="200"/> inside tag itemBody

            string result = string.Empty;
            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            //1. Update pointsValue in tag responseDeclaration
            var responseDeclaration = doc.GetElementsByTagName("responseDeclaration")[0];
            try
            {
                responseDeclaration.Attributes["pointsValue"].Value = newPoint.ToString();
            }
            catch (Exception)
            {
                var pointsValueAttr = doc.CreateAttribute("pointsValue");
                pointsValueAttr.Value = newPoint.ToString();
                responseDeclaration.Attributes.Append(pointsValueAttr);
            }

            //2. Add tag correctResponse inside tag responseDeclaration, it looks like <correctResponse><value>A</value></correctResponse>
            var correctResponse = doc.CreateNode(XmlNodeType.Element, "correctResponse", doc.DocumentElement.NamespaceURI);
            var value = doc.CreateNode(XmlNodeType.Element, "value", doc.NamespaceURI);
            value.InnerText = newCorrectAnswer;
            correctResponse.AppendChild(value);
            responseDeclaration.AppendChild(correctResponse);

            //3. Add tag choiceInteraction inside tag itemBody, it looks like <choiceInteraction responseIdentifier="RESPONSE_3" shuffle="false" maxChoices="1"><simpleChoice identifier="A"><div class="answer" styleName="answer"><p><span>Answer A </span></p></div></simpleChoice><simpleChoice identifier="B"><div class="answer" styleName="answer"><p><span>Answer B</span></p></div></simpleChoice><simpleChoice identifier="C"><div class="answer" styleName="answer"><p><span>Answer C</span></p></div></simpleChoice><simpleChoice identifier="D"><div class="answer" styleName="answer"><p><span>Answer D</span></p></div></simpleChoice></choiceInteraction>
            // Note that responseIdentifier is the same as identifier of responseDeclaration
            var itemBodyNode = doc.GetElementsByTagName("itemBody")[0];

            var choiceInteraction = doc.CreateNode(XmlNodeType.Element, "choiceInteraction", doc.DocumentElement.NamespaceURI);
            var responseIdentifier = doc.CreateAttribute("responseIdentifier");
            responseIdentifier.Value = responseDeclaration.Attributes["identifier"].Value;
            choiceInteraction.Attributes.Append(responseIdentifier);

            var shuffle = doc.CreateAttribute("shuffle");
            shuffle.Value = "false";
            choiceInteraction.Attributes.Append(shuffle);

            var maxChoices = doc.CreateAttribute("maxChoices");
            maxChoices.Value = "1";
            choiceInteraction.Attributes.Append(maxChoices);

            //<simpleChoice identifier="A"><div class="answer" styleName="answer"><p><span>Answer A </span></p></div></simpleChoice><simpleChoice identifier="B"><div class="answer" styleName="answer"><p><span>Answer B</span></p></div></simpleChoice>
            var answers = answerIdentifiers.Split(';');
            if (answers != null)
            {
                foreach (var answer in answers)
                {
                    var simpleChoice = CreateDefaultSimpleChoiceNode(doc, answer);
                    choiceInteraction.AppendChild(simpleChoice);
                }
            }
            itemBodyNode.AppendChild(choiceInteraction);
            //4. Remove tag <extendedTextInteraction responseIdentifier="RESPONSE_2" expectedLength="200"/> inside tag itemBody
            var extendedTextInteraction = doc.GetElementsByTagName("extendedTextInteraction")[0];
            var parent = extendedTextInteraction.ParentNode;
            parent.RemoveChild(extendedTextInteraction);

            result = doc.OuterXml;

            return result;
        }

        public string UpdateXmlContentOfMultipleChoiceForNumberOfChoiceAndCorrectAnswer(string xmlContent, int currentPoint, string currentCorrectAnswer, string newCorrectAnswer, string currentAnswerIdentifiers, string newAnswerIdentifiers)
        {
            string result = string.Empty;
            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            //1. Update CorrectAnswer
            if (!currentCorrectAnswer.Equals(newCorrectAnswer))
            {
                //<correctResponse>
                //    <value>A</value>
                //</correctResponse>
                XmlNode correctResponse = doc.GetElementsByTagName("correctResponse")[0];
                var value = correctResponse.FirstChild;
                value.InnerText = newCorrectAnswer;
            }
            //2. Update Number of Choice

            if (!currentAnswerIdentifiers.Equals(newAnswerIdentifiers))
            {
                //Update choice
                //<choiceInteraction responseIdentifier="RESPONSE_3" shuffle="false" maxChoices="1"><simpleChoice identifier="A"><div class="answer" styleName="answer"><p><span>Answer A </span></p></div></simpleChoice><simpleChoice identifier="B"><div class="answer" styleName="answer"><p><span>Answer B</span></p></div></simpleChoice><simpleChoice identifier="C"><div class="answer" styleName="answer"><p><span>Answer C</span></p></div></simpleChoice><simpleChoice identifier="D"><div class="answer" styleName="answer"><p><span>Answer D</span></p></div></simpleChoice></choiceInteraction>
                XmlNode choiceInteraction = doc.GetElementsByTagName("choiceInteraction")[0];

                var currentAnswers = currentAnswerIdentifiers.Split(';').OrderBy(x => x).ToArray();
                var answers = newAnswerIdentifiers.Split(';').ToArray();
                if (answers.Count() > currentAnswers.Count())
                {
                    //Add more answer
                    //XmlNodeList currentSimpleChoices = doc.GetElementsByTagName("simpleChoice");
                    //while (currentSimpleChoices.Count > 0)
                    //{
                    //    var parent = currentSimpleChoices[0].ParentNode;
                    //    parent.RemoveChild(currentSimpleChoices[0]);
                    //}

                    for (int i = currentAnswers.Count(); i < answers.Count(); i++)
                    {
                        var simpleChoice = CreateDefaultSimpleChoiceNode(doc, answers[i]);
                        choiceInteraction.AppendChild(simpleChoice);
                    }
                }
                if (answers.Count() < currentAnswers.Count())
                {
                    //Remove some answers, start from the last
                    for (int i = currentAnswers.Count(); i > answers.Count(); i--)
                    {
                        var lastSimpleChoice = choiceInteraction.LastChild;
                        choiceInteraction.RemoveChild(lastSimpleChoice);
                    }
                }
            }

            result = doc.OuterXml;

            return result;
        }

        private XmlNode CreateDefaultSimpleChoiceNode(XmlDocument doc, string answer)
        {
            //<simpleChoice identifier="A"><div class="answer" styleName="answer"><p><span>Answer A </span></p></div></simpleChoice><simpleChoice identifier="B"><div class="answer" styleName="answer"><p><span>Answer B</span></p></div></simpleChoice>
            var simpleChoice = doc.CreateNode(XmlNodeType.Element, "simpleChoice", doc.DocumentElement.NamespaceURI);
            var identifier = doc.CreateAttribute("identifier");
            identifier.Value = answer;
            simpleChoice.Attributes.Append(identifier);

            var div = doc.CreateNode(XmlNodeType.Element, "div", doc.DocumentElement.NamespaceURI);
            var classAttr = doc.CreateAttribute("class");
            classAttr.Value = "answer";
            div.Attributes.Append(classAttr);

            var styleName = doc.CreateAttribute("styleName");
            styleName.Value = "answer";
            div.Attributes.Append(styleName);

            var p = doc.CreateNode(XmlNodeType.Element, "p", doc.DocumentElement.NamespaceURI);
            var span = doc.CreateNode(XmlNodeType.Element, "span", doc.DocumentElement.NamespaceURI);
            //span.InnerText = string.Format("Answer {0}", answer);
            span.InnerText = "Answer...";//According to Flash
            p.AppendChild(span);
            div.AppendChild(p);
            simpleChoice.AppendChild(div);

            return simpleChoice;
        }

        public string UpdateXmlContentToUpdatePoint(string xmlContent, int newPoint)
        {
            string result = string.Empty;

            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            var responseDeclaration = doc.GetElementsByTagName("responseDeclaration")[0];
            try
            {
                responseDeclaration.Attributes["pointsValue"].Value = newPoint.ToString();
            }
            catch (Exception)
            {
                var pointsValueAttr = doc.CreateAttribute("pointsValue");
                pointsValueAttr.Value = newPoint.ToString();
                responseDeclaration.Attributes.Append(pointsValueAttr);
            }

            result = doc.OuterXml;

            return result;
        }

        public string GetResponseIdentifierFromXmlContent(string xmlContent)
        {
            string result = string.Empty;

            var doc = ServiceUtil.LoadXmlDocument(xmlContent);

            var responseDeclaration = doc.GetElementsByTagName("responseDeclaration")[0];

            result = responseDeclaration.Attributes["identifier"].Value;

            return result;
        }

        public int InsertDefaultMultipleChoices(int currentUserId, int qtiGroupId, int numberOfChoice, string correctAnswer, out string error)
        {
            //Create xmlcontent for Default Multiple Choices
            //It looks like
            /*
             <assessmentItem xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd" adaptive="false" timeDependent="false" xmlUnicode="true" toolName="linkitTLF" toolVersion="2.0" qtiSchemeID="1" xmlns="http://www.imsglobal.org/xsd/imsqti_v2p0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
	            <stylesheet href="stylesheet/linkitStyleSheet.css" type="text/css"/>
	            <responseDeclaration identifier="RESPONSE_1" baseType="identifier" cardinality="single" method="default" caseSensitive="false" type="string" pointsValue="1" >
		            <correctResponse>
			            <value>A</value>
		            </correctResponse>
	            </responseDeclaration>
	            <itemBody >
		            <div class="mainBody" styleName="mainBody">
			            <br />
			            <p>Multiple choice by Portal</p>
			            <choiceInteraction responseIdentifier="RESPONSE_1" shuffle="false" maxChoices="1">
				            <simpleChoice identifier="A">
					            <div class="answer" styleName="answer">Answer 1</div>
				            </simpleChoice>
				            <simpleChoice identifier="B">
					            <div class="answer" styleName="answer">Answer 2</div>
				            </simpleChoice>
				            <simpleChoice identifier="C">
					            <div class="answer" styleName="answer">Answer 3</div>
				            </simpleChoice>
				            <simpleChoice identifier="D">
					            <div class="answer" styleName="answer">Answer 4</div>
				            </simpleChoice>
			            </choiceInteraction>
			            <br />
			            <br />
		            </div>
	            </itemBody>
            </assessmentItem>
             */
            List<string> alphabetAnswerIdentifiers =
              new List<string>(new string[]
                                     {
                                          "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"
                                         , "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF",
                                         "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP"
                                     }); //Flash does not use 'U'

            StringBuilder xmlContent = new StringBuilder();
            xmlContent.Append("<assessmentItem xsi:schemaLocation=\"http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd\" adaptive=\"false\" timeDependent=\"false\" xmlUnicode=\"true\" toolName=\"linkitTLF\" toolVersion=\"2.0\" qtiSchemeID=\"1\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_v2p0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">");
            xmlContent.Append("<stylesheet href=\"stylesheet/linkitStyleSheet.css\" type=\"text/css\"/>");
            xmlContent.Append("<responseDeclaration identifier=\"RESPONSE_1\" baseType=\"identifier\" cardinality=\"single\" method=\"default\" caseSensitive=\"false\" type=\"string\" pointsValue=\"1\" >");
            xmlContent.Append("<correctResponse>");
            xmlContent.Append(string.Format("<value>{0}</value>", correctAnswer));
            xmlContent.Append("</correctResponse>");
            xmlContent.Append("</responseDeclaration>");
            xmlContent.Append("<itemBody>");
            xmlContent.Append("<div class=\"mainBody\" styleName=\"mainBody\">");
            xmlContent.Append("<br />");
            xmlContent.Append("<p>Question content...</p>");
            xmlContent.Append("<choiceInteraction responseIdentifier=\"RESPONSE_1\" shuffle=\"false\" maxChoices=\"1\">");

            for (int i = 0; i < numberOfChoice; i++)
            {
                xmlContent.Append(string.Format("<simpleChoice identifier=\"{0}\">", alphabetAnswerIdentifiers[i]));
                xmlContent.Append(string.Format("<div class=\"answer\" styleName=\"answer\">Answer {0}</div>", (i + 1).ToString()));
                xmlContent.Append("</simpleChoice>");
            }

            xmlContent.Append("</choiceInteraction>");
            xmlContent.Append("<br />");
            xmlContent.Append("<br />");
            xmlContent.Append(" </div>");
            xmlContent.Append("</itemBody>");
            xmlContent.Append("</assessmentItem>");

            error = string.Empty;

            return CreateQTIItem(currentUserId, qtiGroupId, xmlContent.ToString().RemoveZeroWidthSpaceCharacterFromUnicodeString(), null, out error);
        }

        public int InsertDefaultExtendedText(int currentUserId, int qtiGroupId, out string error)
        {
            string xmlContent = string.Empty;
            xmlContent += "<assessmentItem xsi:schemaLocation=\"http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd\" adaptive=\"false\" timeDependent=\"false\" xmlUnicode=\"true\" toolName=\"linkitTLF\" toolVersion=\"2.0\" qtiSchemeID=\"10\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_v2p0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">";
            xmlContent += "<stylesheet href=\"stylesheet/linkitStyleSheet.css\" type=\"text/css\"/>";
            xmlContent += "<responseDeclaration identifier=\"RESPONSE_1\" baseType=\"string\" cardinality=\"single\" method=\"default\" caseSensitive=\"false\" type=\"string\" pointsValue=\"1\" />";
            xmlContent += "<itemBody >";
            xmlContent += "<div class=\"mainBody\" styleName=\"mainBody\">";
            xmlContent += "<br />";
            xmlContent += "<br />";
            xmlContent += "<extendedTextInteraction responseIdentifier=\"RESPONSE_1\" expectedLength=\"50000\"/>";
            xmlContent += "<br />&#160;</div>";
            xmlContent += "</itemBody>";
            xmlContent += "</assessmentItem>";

            error = string.Empty;

            return CreateQTIItem(currentUserId, qtiGroupId, xmlContent.RemoveZeroWidthSpaceCharacterFromUnicodeString(), null, out error);
        }

        public int CreateQTIItem(int currentUserId, int qtiGroupId, string xmlContent, bool? noDuplicateAnswers, out string error, int? dataFileUploadTypeId = null, int? questionOrder = null, bool isSurvey = false, string title = "", string description = "")

        {
            error = string.Empty;
            // Keep original xml content to save to database to store special characters (these characters can be decode to html characters to be parsed
            var orgXmlConent = xmlContent;
            xmlContent = ServiceUtil.ReplaceSpace(xmlContent);//fix for chrome

            xmlContent = ServiceUtil.EncodeXmlContent(xmlContent);

            if (string.IsNullOrEmpty(xmlContent))
            {
                error = "Xml Content could not be empty!";
                return 0;
            }

            var qtiItemTestTaker = _qTIItemConvert.ConvertFromXmlContent(xmlContent);
            qtiItemTestTaker.UserID = currentUserId;
            qtiItemTestTaker.QTIGroupID = qtiGroupId;
            qtiItemTestTaker.Title = title;

            QTIItemData qtiItemData = null;

            try
            {
                if (isSurvey && qtiItemTestTaker.QTISchemaID == (int)QtiSchemaEnum.ChoiceMultipleVariable && qtiItemTestTaker.QTIITemAnswerScoreTestMakers.Any())
                    qtiItemTestTaker.PointsPossible = qtiItemTestTaker.QTIITemAnswerScoreTestMakers.Max(x => x.Score);

                qtiItemTestTaker.XmlContent = orgXmlConent.ConvertFromUnicodeToWindow1252();
                qtiItemData = CreateQTIItem(qtiItemTestTaker, noDuplicateAnswers, dataFileUploadTypeId, questionOrder, description: description);
            }
            catch (Exception ex)
            {
                error = "There was some error, can not create item right now.";
                return 0;
            }

            if (qtiItemData != null)
            {
                return qtiItemData.QTIItemID;
            }
            else
            {
                error = "There was some error, can not create item right now.";
                return 0;
            }
        }

        public int UpdateItem(int currentUserId, int qtiItemId, string xmlContent, bool? noDuplicateAnswers, int? resetRubric, out string error, string title, string description, bool isSurvey = false)
        {
            error = string.Empty;
            // Keep original xml content to save to database to store special characters (these characters can be decode to html characters to be parsed
            var orgXmlConent = xmlContent;
            xmlContent = ServiceUtil.ReplaceSpace(xmlContent);
            xmlContent = ServiceUtil.EncodeXmlContent(xmlContent);

            if (string.IsNullOrEmpty(xmlContent))
            {
                error = "Xml Content could not be empty!";
                return 0;
            }

            var qtiItemTestTaker = _qTIItemConvert.ConvertFromXmlContent(xmlContent);
            QTIItemData qtiItemData = null;

            try
            {
                if (isSurvey && qtiItemTestTaker.QTISchemaID == (int)QtiSchemaEnum.ChoiceMultipleVariable && qtiItemTestTaker.QTIITemAnswerScoreTestMakers.Any())
                    qtiItemTestTaker.PointsPossible = qtiItemTestTaker.QTIITemAnswerScoreTestMakers.Max(x => x.Score);

                qtiItemTestTaker.XmlContent = orgXmlConent.ConvertFromUnicodeToWindow1252();
                qtiItemData = UpdateQTIItem(currentUserId, qtiItemId, qtiItemTestTaker, noDuplicateAnswers, resetRubric, title, description);
            }
            catch (Exception ex)
            {
                error = "There was some error, can not update item right now.";
                return 0;
            }

            if (qtiItemData != null)
            {
                return qtiItemData.QTIItemID;
            }
            else
            {
                error = "There was some error, can not update item right now.";
                return 0;
            }
        }

        private string KeepSpecialCharactersBeforeConvertToWindow1252(string xmlContent)
        {
            // If do not replace "×" will be convert to "Ã—"
            if (!string.IsNullOrEmpty(xmlContent))
                return xmlContent.Replace("×", "&#215;");

            return xmlContent;
        }

        #region For Clone VirtualTest [TungTran Add]

        public int DuplicateQTIItemForTest(int userId, int qtiItemID, int qTIGroupId, int questionId,
             int newQuestionId, bool? uploadS3, string s3BucketName, string s3FolderName, string s3Domain)
        {
            var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(o => o.QTIItemID == qtiItemID);
            if (qtiItem == null) return 0;
            var newQtiItem = new QTIItemData
            {
                AnswerIdentifiers = qtiItem.AnswerIdentifiers,
                CorrectAnswer = qtiItem.CorrectAnswer,
                FilePath = qtiItem.FilePath,
                InteractionCount = qtiItem.InteractionCount,
                OldMasterCode = qtiItem.OldMasterCode,
                ParentID = qtiItem.ParentID,
                PointsPossible = qtiItem.PointsPossible,
                QTIGroupID = qTIGroupId,
                QTISchemaID = qtiItem.QTISchemaID,
                ResponseIdentifier = qtiItem.ResponseIdentifier,
                ResponseProcessing = qtiItem.ResponseProcessing,
                ResponseProcessingTypeID = qtiItem.ResponseProcessingTypeID,
                SourceID = qtiItem.SourceID,
                Title = qtiItem.Title,
                UrlPath = qtiItem.UrlPath,
                UserID = userId,
                XmlContent = qtiItem.XmlContent,
                Description = qtiItem.Description,
                Updated = DateTime.UtcNow,
                CreatedDate = DateTime.UtcNow,
                QtiItemIdSource = qtiItemID
            };
            int questionOrder = 0;
            if (_qtiItemQtiItemRepository.Select().Any(o => o.QTIGroupID == newQtiItem.QTIGroupID))
            {
                questionOrder =
                    _qtiItemQtiItemRepository.Select().Where(o => o.QTIGroupID == newQtiItem.QTIGroupID).Max(
                        o => o.QuestionOrder);
            }
            newQtiItem.QuestionOrder = questionOrder + 1;

            var xmlContent = newQtiItem.XmlContent;

            // Step 2: Detect all img and audio file in XmlContent (detect img tags for image and itemBody, simpleChoice tags for audio and move them from Clone ItemSet_{OldId} folder to ItemSet_{NewId} folder
            var mediaDic = CloneMediaFileOfQtiTem(qtiItem.QTIGroupID, newQtiItem.QTIGroupID, newQtiItem.XmlContent, uploadS3, s3BucketName, s3FolderName, s3Domain);
            //update media file name inside xml content
            if (mediaDic != null)
            {
                foreach (var mediaPair in mediaDic)
                {
                    xmlContent = xmlContent.Replace(mediaPair.Key, mediaPair.Value);
                }
            }

            // Step 3: Update XmlContent: Replace string ["ItemSet_{OldId}/] with ["ItemSet_{NewId}/] (update image and audio paths)
            if (qtiItem.QTIGroupID != newQtiItem.QTIGroupID)
            {
                xmlContent = xmlContent.Replace("\"ItemSet_" + qtiItem.QTIGroupID + "/",
                                              "\"ItemSet_" + newQtiItem.QTIGroupID + "/");
            
                xmlContent = xmlContent.Replace("/ItemSet_" + qtiItem.QTIGroupID + "/",
                                              "/ItemSet_" + newQtiItem.QTIGroupID + "/");
            }

            newQtiItem.XmlContent = xmlContent;

            _qtiItemQtiItemRepository.Save(newQtiItem);

            // Step 5: Clone QtiItem AnswerScore (QtiItemAnswerScore table)
            DuplicateQtiItemAnswerScoreForTest(qtiItem.QTIItemID, newQtiItem.QTIItemID, questionId, newQuestionId);

            // Step 5.a: Clone QtiItemSub (QtiItemSub table)
            DuplicateQtiItemSubForTest(qtiItem.QTIItemID, newQtiItem.QTIItemID, questionId, newQuestionId);

            // Step 6: Clone QtiItem State Standard (QtiItemStateStandard table)
            DuplicateQtiItemStateStandard(qtiItem.QTIItemID, newQtiItem.QTIItemID);

            // Step 7: Clone Tag (QtiItemTopic, QtiItemLessonOne, QtiItemLessonTwo tables)
            DuplicateQtiItemTag(qtiItem.QTIItemID, newQtiItem.QTIItemID);

            return newQtiItem.QTIItemID;
        }

        private void DuplicateQtiItemAnswerScoreForTest(int qtiItemId, int newQtiItemId, int questionId, int newQuestionId)
        {
            var qtiItemAnswerScores = _qtiItemAnswerScoreRepository.Select().Where(en => en.QTIItemId == qtiItemId);
            bool isExistQuestionAnswerScore = _virtualQuestionAnswerScoreRepository.Select().Any(o => o.VirtualQuestionId == questionId);

            foreach (var item in qtiItemAnswerScores)
            {
                var newQtiItemAnswerScore = new QTIItemAnswerScore
                {
                    Answer = item.Answer,
                    QTIItemId = newQtiItemId,
                    ResponseIdentifier = item.ResponseIdentifier,
                    Score = item.Score,
                    AnswerText = item.AnswerText
                };
                _qtiItemAnswerScoreInsertDeleteRepository.Save(newQtiItemAnswerScore);
                //TODO: Clone VirtualQuestionAnswerScore
                if (isExistQuestionAnswerScore)
                {
                    DuplicateVirtualQuestionAnswerScore(questionId, item.QTIItemAnswerScoreId, newQuestionId, newQtiItemAnswerScore.QTIItemAnswerScoreId);
                }
            }
        }

        private void DuplicateVirtualQuestionAnswerScore(int questionId, int qTIItemAnswerScoreId, int newQuestionId, int newQTIItemAnswerScoreId)
        {
            var old = _virtualQuestionAnswerScoreRepository.Select()
                        .FirstOrDefault(o => o.VirtualQuestionId == questionId && o.QTIItemAnswerScoreId == qTIItemAnswerScoreId);
            if (old != null)
            {
                old.VirtualQuestionAnswerScoreId = 0;
                old.VirtualQuestionId = newQuestionId;
                old.QTIItemAnswerScoreId = newQTIItemAnswerScoreId;
                _virtualQuestionAnswerScoreRepository.Save(old);
            }
        }

        private void DuplicateQtiItemSubForTest(int qtiItemId, int newQtiItemId, int questionId, int newQuestionId)
        {
            var qtiItemSubs = _qtiItemSubReadOnlyRepository.Select().Where(en => en.QTIItemId == qtiItemId);
            bool isExistQuestionSub = _virtualQuestionSubRepository.Select().Any(o => o.VirtualQuestionId == questionId);
            //TODO: Some time one QTIITem belong many question. we only Duplicate VirtualQuestionSub when isExistQuestionSub == true
            foreach (var item in qtiItemSubs)
            {
                var newQtiItemSub = new QTIItemSub
                {
                    CorrectAnswer = item.CorrectAnswer,
                    PointsPossible = item.PointsPossible,
                    QTIItemId = newQtiItemId,
                    QTISchemaId = item.QTISchemaId,
                    ResponseIdentifier = item.ResponseIdentifier,
                    ResponseProcessing = item.ResponseProcessing,
                    ResponseProcessingTypeId = item.ResponseProcessingTypeId,
                    SourceId = item.SourceId,
                    Updated = item.Updated,
                    Depending = item.Depending,
                    Major = item.Major
                };
                _qtiItemSubRepository.Save(newQtiItemSub);
                if (isExistQuestionSub)
                {
                    DuplicateVirtualQuestionSub(questionId, item.QTIItemSubId, newQuestionId, newQtiItemSub.QTIItemSubId);
                }
            }
        }

        private void DuplicateVirtualQuestionSub(int questionId, int qTIITemSubId, int newQuestionId, int newQTIITemSubId)
        {
            var old = _virtualQuestionSubRepository.Select().FirstOrDefault(o => o.VirtualQuestionId == questionId && o.QTIItemSubId == qTIITemSubId);
            if (old != null)
            {
                old.VirtualQuestionSubId = 0;
                old.VirtualQuestionId = newQuestionId;
                old.QTIItemSubId = newQTIITemSubId;
                _virtualQuestionSubRepository.Save(old);
            }
        }

        #endregion For Clone VirtualTest [TungTran Add]

        #region Copy/Move Items

        public void MoveItems(int qtiItemId, int toQtiGroupId, int userId, bool? uploadS3, string s3BucketName,
                              string s3FolderName, string s3Domain)
        {
            var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(o => o.QTIItemID == qtiItemId);
            if (qtiItem != null)
            {
                int oldQtiGroupId = qtiItem.QTIGroupID;
                qtiItem.Updated = DateTime.UtcNow;

                // Detect all img and audio file in XmlContent (detect img tags for image and itemBody, simpleChoice tags for audio and move them from Clone ItemSet_{OldId} folder to ItemSet_{NewId} folder
                var mediaDic = CloneMediaFileOfQtiTem(qtiItem.QTIGroupID, toQtiGroupId, qtiItem.XmlContent, uploadS3, s3BucketName, s3FolderName, s3Domain);
                //update media file name inside xml content
                if (mediaDic != null)
                {
                    foreach (var mediaPair in mediaDic)
                    {
                        qtiItem.XmlContent = qtiItem.XmlContent.Replace(mediaPair.Key, mediaPair.Value);
                    }
                }

                //update ItemSet_ID in xmlContent;
                // Update XmlContent: Replace string ["ItemSet_{OldId}/] with ["ItemSet_{NewId}/] (update image and audio paths)
                qtiItem.XmlContent = qtiItem.XmlContent.Replace("\"ItemSet_" + qtiItem.QTIGroupID + "/",
                                             "\"ItemSet_" + toQtiGroupId.ToString() + "/");
                qtiItem.XmlContent = qtiItem.XmlContent.Replace("\"/ItemSet_" + qtiItem.QTIGroupID + "/",
                                             "\"/ItemSet_" + toQtiGroupId.ToString() + "/");
                qtiItem.XmlContent = qtiItem.XmlContent.Replace("ItemSet_" + qtiItem.QTIGroupID + "/",
                                             "ItemSet_" + toQtiGroupId.ToString() + "/");
                //change the group
                qtiItem.QTIGroupID = toQtiGroupId;
                //Add to the end of the new QtiGroup
                int questionOrder = 0;
                if (_qtiItemQtiItemRepository.Select().Any(o => o.QTIGroupID == toQtiGroupId))
                {
                    questionOrder =
                        _qtiItemQtiItemRepository.Select().Where(o => o.QTIGroupID == toQtiGroupId).Max(
                            o => o.QuestionOrder);
                }
                qtiItem.QuestionOrder = questionOrder + 1;
                _qtiItemQtiItemRepository.Save(qtiItem);
                _qtiGroupRepository.ReassignQuestionOrder(oldQtiGroupId);
            }
        }

        public QTIItemData CopyItems(int qtiItemId, int toQtiGroupId, int userId, bool? uploadS3,
             string s3BucketName, string s3FolderName, string s3Domain)
        {
            var qtiItem = _qtiItemQtiItemRepository.Select().FirstOrDefault(o => o.QTIItemID == qtiItemId);
            var qtiBankId =
                _qtiGroupRepository.Select()
                    .Where(x => x.QtiGroupId == qtiItem.QTIGroupID)
                    .Select(x => x.QtiBankId)
                    .FirstOrDefault();

            if (qtiItem != null)
            {
                var newQtiItem = new QTIItemData
                {
                    AnswerIdentifiers = qtiItem.AnswerIdentifiers,
                    CorrectAnswer = qtiItem.CorrectAnswer,
                    FilePath = qtiItem.FilePath,
                    InteractionCount = qtiItem.InteractionCount,
                    OldMasterCode = qtiItem.OldMasterCode,
                    ParentID = qtiItem.ParentID,
                    PointsPossible = qtiItem.PointsPossible,
                    QTIGroupID = toQtiGroupId,
                    QTISchemaID = qtiItem.QTISchemaID,
                    ResponseIdentifier = qtiItem.ResponseIdentifier,
                    ResponseProcessing = qtiItem.ResponseProcessing,
                    ResponseProcessingTypeID = qtiItem.ResponseProcessingTypeID,
                    SourceID = qtiItem.SourceID,
                    Title = qtiItem.Title,
                    UrlPath = qtiItem.UrlPath,
                    UserID = userId,
                    XmlContent = qtiItem.XmlContent,
                    QtiItemIdSource = qtiItemId,
                    Updated = DateTime.UtcNow,
                    CreatedDate = DateTime.UtcNow,
                    Description = qtiItem.Description
                };

                // Detect all img and audio file in XmlContent (detect img tags for image and itemBody, simpleChoice tags for audio and move them from Clone ItemSet_{OldId} folder to ItemSet_{NewId} folder
                var mediaDic = CloneMediaFileOfQtiTem(qtiItem.QTIGroupID, newQtiItem.QTIGroupID, newQtiItem.XmlContent, uploadS3,
                    s3BucketName, s3FolderName, s3Domain);
                var xmlContent = newQtiItem.XmlContent;
                //update media file name inside xml content
                if (mediaDic != null)
                {
                    foreach (var mediaPair in mediaDic)
                    {
                        xmlContent = xmlContent.Replace(mediaPair.Key, mediaPair.Value);
                    }
                }
                int questionOrder = 0;
                if (_qtiItemQtiItemRepository.Select().Any(o => o.QTIGroupID == toQtiGroupId))
                {
                    questionOrder =
                        _qtiItemQtiItemRepository.Select().Where(o => o.QTIGroupID == toQtiGroupId).Max(
                            o => o.QuestionOrder);
                }
                newQtiItem.QuestionOrder = questionOrder + 1;

                // Save new qtiItem
                _qtiItemQtiItemRepository.Save(newQtiItem);

                //update ItemSet_ID in xmlContent;
                // Update XmlContent: Replace string ["ItemSet_{OldId}/] with ["ItemSet_{NewId}/] (update image and audio paths)
                xmlContent = xmlContent.Replace("\"ItemSet_" + qtiItem.QTIGroupID + "/",
                    "\"ItemSet_" + toQtiGroupId.ToString() + "/");
                xmlContent = xmlContent.Replace("\"/ItemSet_" + qtiItem.QTIGroupID + "/",
                    "\"/ItemSet_" + toQtiGroupId.ToString() + "/");
                xmlContent = xmlContent.Replace("ItemSet_" + qtiItem.QTIGroupID + "/",
                    "ItemSet_" + toQtiGroupId.ToString() + "/");

                //// Step 4: Clone Ref Objects
                //xmlContent = CloneRefObjects(xmlContent, userId); //no need to create new passage any more

                //// Update xmlContent of qtiItem
                newQtiItem.XmlContent = xmlContent;

                _qtiItemQtiItemRepository.Save(newQtiItem);

                // Step 5: Clone QtiItem AnswerScore (QtiItemAnswerScore table)
                DuplicateQtiItemAnswerScore(qtiItem.QTIItemID, newQtiItem.QTIItemID);

                // Step 5.a: Clone QtiItemSub (QtiItemSub table)
                DuplicateQtiItemSub(qtiItem.QTIItemID, newQtiItem.QTIItemID);

                // Step 6: Clone QtiItem State Standard (QtiItemStateStandard table)
                DuplicateQtiItemStateStandard(qtiItem.QTIItemID, newQtiItem.QTIItemID);

                // Step 7: Clone Tag (QtiItemTopic, QtiItemLessonOne, QtiItemLessonTwo tables)
                DuplicateQtiItemTag(qtiItem.QTIItemID, newQtiItem.QTIItemID);

                // Step 8: Clone AlgorithmQTIItemGrading table
                DuplicateAlgorithmQTIItemGrading(qtiItem.QTIItemID, newQtiItem.QTIItemID, userId);

                // Step 9: Clone MultiPart Expression
                if (qtiItem.QTISchemaID == (int)QtiSchemaEnum.MultiPart)
                {
                    DuplicateMultiPartExpression(qtiItem.QTIItemID, newQtiItem.QTIItemID, userId);
                }

                return newQtiItem;
            }

            return null;
        }

        #endregion Copy/Move Items

        public bool CheckAccessQTI3p(int userId, int districtId, Qti3pLicensesEnum qti3pLicensesEnum)
        {
            return _qtiItemQtiItemRepository.CheckAccessQTI3p(userId, districtId, qti3pLicensesEnum);
        }

        public bool CheckShowQtiItem(int userId, int virtualQuestionId, int districtId)
        {
            return _qtiItemQtiItemRepository.CheckShowQtiItem(userId, virtualQuestionId, districtId);
        }

        #region Examview

        public string ProcessExamviewUploadFiles(int currentUserId, int qtiGroupId, string extractedFoler)
        {
            string result = string.Empty;
            XmlDocument doc = new XmlDocument();
            // Try to Open imsmanifest.xml
            try
            {
                var imsmanifestPath = string.Format("{0}/{1}", extractedFoler, "imsmanifest.xml");
                if (!System.IO.File.Exists(imsmanifestPath))
                {
                    return "The uploading file does not contain imsmanifest.xml, please double check and try again.";
                }
                var imsmanifestContent = System.IO.File.ReadAllText(imsmanifestPath);
                doc.LoadXml(imsmanifestContent);
            }
            catch
            {
                return "Can not load imsmanifest.xml";
            }
            var baseurl = string.Empty;
            var resourceXml = string.Empty;
            try
            {
                // Get baseurl from the manifest
                var resourceNode = doc.GetElementsByTagName("resource");
                if (resourceNode.Count == 0)
                {
                    return "Can not find tag resource in  imsmanifest.xml";
                }
                // Get baseurl from the manifest
                baseurl = resourceNode[0].Attributes["baseurl"].Value;
                if (string.IsNullOrEmpty(baseurl))
                {
                    return "Can not find attribute baseurl in tag resource in  imsmanifest.xml";
                }

                // Get file from the manifest
                resourceXml = resourceNode[0].Attributes["file"].Value;
                if (string.IsNullOrEmpty(resourceXml))
                {
                    return "Can not find attribute file in tag resource in  imsmanifest.xml";
                }
                // Check if the ItemSet media folder exists, if not exist, create it
                //var testItemMediaPath = ConfigurationManager.AppSettings["TestItemMediaPath"];
                //var testItemMediaPath = string.Empty;
                //if (string.IsNullOrEmpty(testItemMediaPath))
                //{
                //    return "Can not find config TestItemMediaPath, please contact Admin";
                //}

                //testItemMediaPath = testItemMediaPath.Replace("\\", "/");
                //if (testItemMediaPath[testItemMediaPath.Length - 1] == '/')
                //{
                //    testItemMediaPath = testItemMediaPath.Substring(0, testItemMediaPath.Length - 1);//make sure examviewUploadPath does not end with '/'
                //}
                //var itemSetPath = string.Format("{0}/ItemSet_{1}", testItemMediaPath, qtiGroupId);
                //try
                //{
                //    if (!Directory.Exists(itemSetPath))
                //    {
                //        Directory.CreateDirectory(itemSetPath);
                //    }
                //    // Copy the contents of the base resources to the item set media folder
                //    ServiceUtil.DirectoryCopy(string.Format("{0}/{1}", extractedFoler, baseurl), itemSetPath, true,false);

                //}
                //catch
                //{
                //    return "Can not copy resource file to itemset folder, please contact Admin";
                //}

                // Load the resource XML data file
                var resourceXmlPath = string.Format("{0}/{1}", extractedFoler, resourceXml);
                var resourceXmlContent = System.IO.File.ReadAllText(resourceXmlPath);

                ExamviewXmlContentProcessing resourceDoc = null;
                try
                {
                    resourceDoc = new ExamviewXmlContentProcessing(resourceXmlContent);
                }
                catch
                {
                    return "Can not load " + resourceXml;
                    //delete the itemset folder
                }
                //Get QUESTION
                var questionNodes = resourceDoc.GetElementsByTagName("QUESTION");
                Dictionary<string, int> pointPossibleDictionary = new Dictionary<string, int>();
                foreach (var questionNode in questionNodes)
                {
                    pointPossibleDictionary.Add(((XmlNode)questionNode).Attributes["id"].Value, Int32.Parse(((XmlNode)questionNode).Attributes["points"].Value));
                }

                // Iterate through the questions
                var questionMultipleChoiceNodes = resourceDoc.GetElementsByTagName("QUESTION_MULTIPLECHOICE");
                //Get the number of qti items in the item set
                var countQtiItems = this.GetAllQtiItem()
                    .Where(x => x.QTIGroupID == qtiGroupId).Select(x => x.QTIItemID).Count();
                var questionOrder = countQtiItems + 1;
                string errorQuestion = string.Empty;
                string questionId = string.Empty;
                for (int i = 0; i < questionMultipleChoiceNodes.Count; i++)
                //for (int i = 4; i < 5; i++)
                {
                    try
                    {
                        questionId = questionMultipleChoiceNodes[i].Attributes["id"].Value;
                        var xmlContent = ConvertExamviewToLinkitFormat(questionMultipleChoiceNodes[i], pointPossibleDictionary);
                        xmlContent = XmlUtils.UpdateImageUrlsToLinkit(xmlContent, qtiGroupId);
                        xmlContent = resourceDoc.GetOrginalContent(xmlContent);
                        xmlContent = xmlContent.RemoveTroublesomeCharacters();

                        var qtiItemId = CreateQTIItem(currentUserId, qtiGroupId, xmlContent, null, out errorQuestion);

                        if (!string.IsNullOrEmpty(errorQuestion))
                        {
                            result += string.Format("Can not import question {0}. Error {1}", questionId, errorQuestion);
                        }
                        if (qtiItemId <= 0)
                        {
                            result += string.Format("Can not import question {0}", questionId);
                        }
                    }
                    catch (Exception ex)
                    {
                        result += string.Format("Can not import question {0}. Error {1}", questionId, ex.Message);
                    }
                }
            }
            catch
            {
                return "Can not process this file";
            }
            return result;
        }

        private string ConvertExamviewToLinkitFormat(XmlNode questionMultipleChoiceNode, Dictionary<string, int> pointPossibleDictionary)
        {
            XmlNode bodyNode = null;
            List<XmlNode> answers = new List<XmlNode>();
            XmlNode gradable = null;
            for (int i = 0; i < questionMultipleChoiceNode.ChildNodes.Count; i++)
            {
                if (questionMultipleChoiceNode.ChildNodes[i].NodeType == XmlNodeType.Element
                    && questionMultipleChoiceNode.ChildNodes[i].Name.ToUpper().Equals("BODY"))
                {
                    bodyNode = questionMultipleChoiceNode.ChildNodes[i];
                }
                if (questionMultipleChoiceNode.ChildNodes[i].NodeType == XmlNodeType.Element
                    && questionMultipleChoiceNode.ChildNodes[i].Name.ToUpper().Equals("ANSWER"))
                {
                    answers.Add(questionMultipleChoiceNode.ChildNodes[i]);
                }
                if (questionMultipleChoiceNode.ChildNodes[i].NodeType == XmlNodeType.Element
                    && questionMultipleChoiceNode.ChildNodes[i].Name.ToUpper().Equals("GRADABLE"))
                {
                    gradable = questionMultipleChoiceNode.ChildNodes[i];
                }
            }
            var id = questionMultipleChoiceNode.Attributes["id"].Value;
            int pointPossible = pointPossibleDictionary[id];
            var answer_id = string.Empty;
            if (gradable != null)
            {
                for (int i = 0; i < gradable.ChildNodes.Count; i++)
                {
                    if (gradable.ChildNodes[i].NodeType == XmlNodeType.Element &&
                        gradable.ChildNodes[i].Name.ToUpper().Equals("CORRECTANSWER"))
                    {
                        answer_id = gradable.ChildNodes[i].Attributes["answer_id"].Value;
                    }
                }
            }
            var position = -1;
            for (int i = 0; i < answers.Count; i++)
            {
                if (answers[i].Attributes["id"].Value.Equals(answer_id))
                {
                    position = Int32.Parse(answers[i].Attributes["position"].Value);
                }
            }

            List<string> alphabetAnswerIdentifiers =
              new List<string>(new string[]
                                     {
                                          "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P"
                                         , "Q", "R", "S", "T", "V", "W", "X", "Y", "Z", "AA", "AB", "AC", "AD", "AE", "AF",
                                         "AG", "AH", "AI", "AJ", "AK", "AL", "AM", "AN", "AO", "AP"
                                     }); //Flash does not use 'U'
            var correctAnswer = alphabetAnswerIdentifiers[position - 1];
            StringBuilder xmlContent = new StringBuilder();
            xmlContent.Append("<assessmentItem xsi:schemaLocation=\"http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd\" adaptive=\"false\" timeDependent=\"false\" xmlUnicode=\"true\" toolName=\"linkitTLF\" toolVersion=\"2.0\" qtiSchemeID=\"1\" xmlns=\"http://www.imsglobal.org/xsd/imsqti_v2p0\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" importfrom=\"examview\">");
            xmlContent.Append("<stylesheet href=\"stylesheet/linkitStyleSheet.css\" type=\"text/css\"/>");
            xmlContent.Append(string.Format("<responseDeclaration identifier=\"RESPONSE_1\" baseType=\"identifier\" cardinality=\"single\" method=\"default\" caseSensitive=\"false\" type=\"string\" pointsValue=\"{0}\" >", pointPossible));
            xmlContent.Append("<correctResponse>");
            xmlContent.Append(string.Format("<value>{0}</value>", correctAnswer));
            xmlContent.Append("</correctResponse>");
            xmlContent.Append("</responseDeclaration>");
            xmlContent.Append("<itemBody>");
            xmlContent.Append("<div class=\"mainBody\" styleName=\"mainBody\">");

            var bodyContent = string.Empty; ; //TEXT/CDATA;
            for (int i = 0; i < bodyNode.ChildNodes.Count; i++)
            {
                if (bodyNode.ChildNodes[i].Name.ToUpper().Equals("TEXT"))
                {
                    for (int j = 0; j < bodyNode.ChildNodes[i].ChildNodes.Count; j++)
                    {
                        if (bodyNode.ChildNodes[i].ChildNodes[j].NodeType == XmlNodeType.CDATA &&
                            bodyNode.ChildNodes[i].ChildNodes[j].Name.ToLower().Equals("#cdata-section"))
                        {
                            bodyContent = bodyNode.ChildNodes[i].ChildNodes[j].InnerText;
                            break;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(bodyContent))
                {
                    break;
                }
            }
            bodyContent = CleanUpHtmlToXml(bodyContent);
            bodyContent = RemoveFontSize(bodyContent);

            xmlContent.Append(bodyContent);
            xmlContent.Append("<choiceInteraction responseIdentifier=\"RESPONSE_1\" shuffle=\"false\" maxChoices=\"1\">");

            for (int i = 0; i < answers.Count; i++)
            {
                xmlContent.Append(string.Format("<simpleChoice identifier=\"{0}\">", alphabetAnswerIdentifiers[i]));

                string answerText = string.Empty;
                //TEXT->CDATA
                for (int j = 0; j < answers[i].ChildNodes.Count; j++)
                {
                    if (answers[i].ChildNodes[j].Name.ToUpper().Equals("TEXT"))
                    {
                        for (int k = 0; k < answers[i].ChildNodes[j].ChildNodes.Count; k++)
                        {
                            if (answers[i].ChildNodes[j].ChildNodes[k].NodeType == XmlNodeType.CDATA &&
                                answers[i].ChildNodes[j].ChildNodes[k].Name.ToLower().Equals("#cdata-section"))
                            {
                                answerText = answers[i].ChildNodes[j].ChildNodes[k].InnerText;
                                break;
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(answerText))
                    {
                        break;
                    }
                }
                answerText = CleanUpHtmlToXml(answerText);
                answerText = RemoveFontSize(answerText);

                xmlContent.Append(string.Format("<div class=\"answer\" styleName=\"answer\"><p><span>{0}</span></p></div>", answerText));

                xmlContent.Append("</simpleChoice>");
            }

            xmlContent.Append("</choiceInteraction>");
            xmlContent.Append(" </div>");
            xmlContent.Append("</itemBody>");
            xmlContent.Append("</assessmentItem>");

            return xmlContent.ToString();
        }

        private string CleanUpHtmlToXml(string content)
        {
            var result = content.Replace("<br>", "<br/>");
            //because <img has no close tag so it is necessary to add / to the end > to make />
            string pattern = "<img[^<,>]+(?>)";
            string replacement = "$0/";
            Regex rgx = new Regex(pattern);
            result = rgx.Replace(result, replacement);
            result = result.Replace("//>", "/>");

            var temp = string.Format("<TEMP_ROOT>{0}</TEMP_ROOT>", result);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(temp);
            ReplaceNodeNames(doc, "sub", "span", "smallText sub", "sub");
            ReplaceNodeNames(doc, "sup", "span", "smallText sup", "sup");
            ReplaceNodeNames(doc, "b", "span", "bold", "bold");
            ReplaceNodeNames(doc, "u", "span", "underline", "underline");
            result = doc.FirstChild.InnerXml;
            return result;
        }

        private string RemoveFontSize(string content)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(string.Format("<TEMP_ROOT>{0}</TEMP_ROOT>", content));
            var spanNodes = doc.GetElementsByTagName("span");
            if (spanNodes.Count == 0)
            {
                return content;
            }
            for (int i = 0; i < spanNodes.Count; i++)
            {
                if (spanNodes[i].Attributes.Count > 0)
                {
                    if (spanNodes[i].Attributes["style"] != null)
                    {
                        if (spanNodes[i].Attributes["style"].Value.ToLower().IndexOf("font-") >= 0)
                        {
                            spanNodes[i].Attributes["style"].Value = string.Empty;
                            //var firstChild = spanNodes[i].FirstChild;
                            //if (firstChild != null)
                            //{
                            //    doc.ReplaceChild(firstChild, spanNodes[i]);
                            //}
                        }
                    }
                }
            }
            return doc.FirstChild.InnerXml;
        }

        private string ReplaceNodeNames(XmlDocument doc, string nodeName, string newNodeName, string className, string styleName)
        {
            var oldNodes = doc.GetElementsByTagName(nodeName);
            for (int i = 0; i < oldNodes.Count; i++)
            {
                var newNode = doc.CreateElement(newNodeName);
                if (oldNodes[i].Attributes != null)
                {
                    //copy attributes
                    for (int j = 0; j < oldNodes[i].Attributes.Count; j++)
                    {
                        newNode.Attributes.Append(oldNodes[i].Attributes[j]);
                    }
                    //copy child nodes
                    for (int k = 0; k < oldNodes[i].ChildNodes.Count; k++)
                    {
                        newNode.AppendChild(oldNodes[i].ChildNodes[k].CloneNode(true));
                    }
                    //set attribute class
                    if (newNode.Attributes["class"] == null)
                    {
                        var classAtt = doc.CreateAttribute("class");
                        classAtt.Value = className;
                        newNode.Attributes.Append(classAtt);
                    }
                    else
                    {
                        newNode.Attributes["class"].Value = newNode.Attributes["class"].Value ??
                                                            string.Empty + " " + className;
                    }
                    //set attribute class
                    if (newNode.Attributes["styleName"] == null)
                    {
                        var styleNameAtt = doc.CreateAttribute("styleName");
                        styleNameAtt.Value = styleName;
                        newNode.Attributes.Append(styleNameAtt);
                    }
                    else
                    {
                        newNode.Attributes["styleName"].Value = newNode.Attributes["styleName"].Value ??
                                                            string.Empty + " " + styleName;
                    }
                    oldNodes[i].ParentNode.ReplaceChild(newNode, oldNodes[i]);
                }
            }
            return doc.OuterXml;
        }

        #endregion Examview

        public List<PassageItem> GetQtiItemsByFiltersPassage(QtiItemFilters filter, int? userId, int districtId,
            int startIndex, int pageSize, string sortColumns,
            string searchColumns, string searchInboxXML)
        {
            return _qtiItemQtiItemRepository.GetQtiItemsByFiltersPassage(filter, userId, districtId, startIndex,
                                                                pageSize, sortColumns, searchColumns, searchInboxXML);
        }

        public void UpdateItemPassage(int qtiItemId, List<int> qtiRefObjectIdList, List<int> qti3pPassageNumberList)
        {
            _qtiItemQtiItemRepository.UpdateItemPassage(qtiItemId, qtiRefObjectIdList, qti3pPassageNumberList);
        }

        #region Data Upload

        public string InsertDataUpload(DataFileUploaderParameter parameter, ref List<DataFileUploaderResource> questionResources, ref DataFileUploaderResult uploadResult, int itemTagCategoryId)
        {
            var result = string.Empty;
            for (int i = 0; i < questionResources.Count; i++)
            {
                var resource = questionResources[i];

                if (string.IsNullOrEmpty(resource.XmlContent))
                {
                    resource.Error = " Unable to convert to Linkit xml content format";
                }
                else if (string.IsNullOrEmpty(resource.Error))
                {
                    //No storing files on local any more
                    //if (parameter.SaveUploadItemToLocal)
                    //{
                    //    SaveItemMediaToLocal(ref uploadResult, ref resource, parameter);
                    //    if (!string.IsNullOrEmpty(resource.Error))
                    //    {
                    //        result = resource.Error;
                    //        uploadResult.Error = "One ore more media files of " + resource.ResourceFileName + " could not be copied. " + resource.Error;
                    //        uploadResult.Result = "There's one resource could not be imported.";
                    //        break;
                    //    }
                    //}

                    SaveItemMediaToS3(ref uploadResult, ref resource, parameter);

                    if (!string.IsNullOrEmpty(resource.Error))
                    {
                        result = resource.Error;
                        uploadResult.Error = "One ore more media files of " + resource.ResourceFileName + " could not be uploaded to S3. " + resource.Error;
                        uploadResult.Result = "There's one resource could not be imported.";
                        break;
                    }

                    //Create QtiItem at the end
                    //if (string.IsNullOrEmpty(result))
                    {
                        string errorQuestion;
                        int qtiItemId = 0;
                        if (parameter.UploadTo3pItem)
                        {
                            QTI3pItem qti3pItem = null;
                            if (parameter.UploadTo3pItem && parameter.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery)
                            {
                                var title = resource.Title;
                                qti3pItem = _qTI3pItemRepository.Select().Where(x => x.Title == title).FirstOrDefault();
                            }

                            resource.ProcessingStep.Append("->Create QTI3pItem.");
                            qtiItemId = CreateQTI3pItem(parameter, resource, out errorQuestion, qti3pItem);
                        }
                        else
                        {
                            resource.ProcessingStep.Append("->Create QTIItem.");
                            qtiItemId = CreateQTIItem(parameter.CurrentUserId, parameter.QtiGroupId,
                                resource.XmlContent, null,
                                out errorQuestion, uploadResult.DataFileUploadTypeId, resource.QuestionOrder);
                        }
                        if (!string.IsNullOrEmpty(errorQuestion))
                        {
                            if (qtiItemId <= 0)
                            {
                                result += string.Format("Can not create qtiItem from file \"{0}\". Error: {1}",
                                    resource.ResourceFileName, errorQuestion);
                                resource.Error =
                                    string.Format("Can not create qtiItem from file \"{0}\". Error: {1}",
                                        resource.ResourceFileName, errorQuestion);
                            }
                            resource.ErrorDetail = errorQuestion;
                        }
                        else
                        {
                            if (qtiItemId <= 0)
                            {
                                result += string.Format("Can not create qtiItem from file \"{0}\"",
                                    resource.ResourceFileName);
                                resource.Error = "Can not create qtiItem";
                            }
                        }

                        if (qtiItemId > 0)
                        {
                            resource.QtiItemId = qtiItemId;
                            resource.ProcessingStep.Append(string.Format("->Done Create QTIItemID {0}.",
                                qtiItemId));

                            if (!parameter.UploadTo3pItem)
                            {
                                //save to QtiItemDataFileUploadPassage
                                SaveQtiItemDataFileUploadPassage(resource, qtiItemId);
                                //save DOK to QTIITemITemTag
                                SaveDOKToQTIITemITemTag(resource, qtiItemId, itemTagCategoryId);
                            }
                            else
                            {
                                //save to QTI3pItemToPassage
                                SaveQTI3pItemToPassage(resource, qtiItemId);
                            }
                        }
                        else
                        {
                            resource.ProcessingStep.Append("->Create QTIItem failed.");
                        }
                    }
                }
            }

            return result;
        }

        public void ProcessPassage(ref DataFileUploaderResult uploadResult, DataFileUploaderParameter parameter)
        {
            //process passages
            var dataFileUploadLogId = parameter.DataFileUploadLogId;
            var passageFileList = new List<string>();
            foreach (var resource in uploadResult.Resources)
            {
                if (resource.IsValidQuestionResourceFile)
                {
                    foreach (var passage in resource.PassageList)
                    {
                        if (!passageFileList.Contains(passage))
                        {
                            passageFileList.Add(passage);
                        }
                    }
                }
            }
            foreach (var passageFile in passageFileList)
            {
                // find the resource which is this passage
                DataFileUploaderResource resourcePassage = null;
                foreach (var resource in uploadResult.Resources)
                {
                    if (resource.ResourceFileName == passageFile)
                    {
                        resourcePassage = resource;
                        break;
                    }
                }

                if (resourcePassage != null && !passageFile.ToLower().StartsWith("http")
                    && !string.IsNullOrEmpty(passageFile))
                {
                    resourcePassage.DataFileUploadFileType = DataFileUploadFileTypeEnum.Passage;
                    //if (parameter.SaveUploadItemToLocal)
                    //{
                    //    if (!parameter.UploadTo3pItem)
                    //    {
                    //        SavePassageToLocal(ref uploadResult, ref resourcePassage, parameter);
                    //    }

                    //}

                    //Always upload passage to S3
                    SavePassageToS3(ref uploadResult, ref resourcePassage, parameter);
                }

                if (!string.IsNullOrEmpty(uploadResult.Result))
                {
                    return; //if there is any error while processing a passage, stop and alert message to user
                }
            }

            if (!string.IsNullOrEmpty(uploadResult.Result))
            {
                return;
            }

            //Update xmlcontent for passage
            var actualPassageList =
                uploadResult.Resources.Where(x => x.DataFileUploadFileType == DataFileUploadFileTypeEnum.Passage)
                    .ToList();

            for (int k = 0; k < actualPassageList.Count; k++)
            {
                var passage = actualPassageList[k];
                var passageFile = passage.ResourceFileName;

                SavePassageMetaData(uploadResult, passage, parameter, dataFileUploadLogId);

                if (!string.IsNullOrEmpty(uploadResult.Result))
                {
                    return;
                }
            }
        }

        private string CreateS3PassageForUploading(DataFileUploaderParameter parameter, string passageFile)
        {
            //testitemmedia > (Staging) > passages > <folder name> / passages / filename

            //folder name = uploaded file name - .zip

            var s3FilePath = string.Empty;
            if (parameter.UploadTo3pItem)
            {
                if (parameter.S3TestMedia.ToLower().StartsWith("http")) //absolute link
                {
                    s3FilePath = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(), passageFile.RemoveStartSlash());
                    return s3FilePath;
                }
            }

            var s3Folder = parameter.AUVirtualTestFolder;
            s3Folder = s3Folder.RemoveStartSlash();

            var subFolder = Path.GetFileNameWithoutExtension(parameter.ZipFileName);
            subFolder = subFolder.Replace(".zip", "");
            subFolder.AdjustFileNameForPDFPrinting(); //adjust file name

            var passageFileName = passageFile;
            if (passageFileName.StartsWith("/"))
            {
                passageFileName = passageFileName.Remove(0, 1);
            }
            passageFileName = Path.GetFileName(passageFileName);//passages/5578.html -> get only 5578.html

            if (parameter.UploadTo3pItem)
                s3FilePath = string.Format("{0}/passages/{1}", parameter.S3TestMedia, passageFileName);
            else
            {
                if (string.IsNullOrEmpty(s3Folder)) //AUVirtualTestFolder in production is empty
                {
                    s3FilePath = string.Format("passages/{0}/passages/{1}", subFolder, passageFileName);
                }
                else
                {
                    s3FilePath = string.Format("{0}/passages/{1}/passages/{2}", s3Folder, subFolder, passageFileName);
                }
            }

            s3FilePath = s3FilePath.Replace("//", "/");
            s3FilePath = s3FilePath.Replace("\\\\", "\\");
            return s3FilePath;
        }

        private string CreateS3PassageLink(DataFileUploaderParameter parameter, string passageFile)
        {
            //testitemmedia > (Staging) > passages > <folder name> / passages / filename

            //folder name = uploaded file name - .zip
            if (parameter.UploadTo3pItem)
            {
                if (parameter.S3TestMedia.ToLower().StartsWith("http")) //use full s3 link
                {
                    var result = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(),
                        passageFile.RemoveStartSlash());
                    return result;
                }
            }
            var s3Domain = parameter.S3Domain;
            var bucket = parameter.AUVirtualTestBucketName.RemoveStartSlash().RemoveEndSlash();
            //if (bucket.StartsWith("/"))
            //{
            //    bucket = bucket.Remove(0, 1);
            //}
            string s3PassageForUploading = CreateS3PassageForUploading(parameter, passageFile);
            //if (s3Domain.EndsWith("/"))
            //{
            //    s3Domain = s3Domain.Substring(0, s3Domain.Length - 1);
            //}
            //if (s3Domain.EndsWith("\\"))
            //{
            //    s3Domain = s3Domain.Substring(0, s3Domain.Length - 1);
            //}
            if (s3Domain.ToLower().StartsWith("https"))
            {
                s3Domain = s3Domain.Replace("https://", "https://" + bucket + ".");
            }
            else
            {
                //start with http
                s3Domain = s3Domain.Replace("http://", "http://" + bucket + ".");
            }
            return string.Format("{0}/{1}", s3Domain.RemoveEndSlash(), s3PassageForUploading.RemoveStartSlash());
        }

        private int CreateQTI3pItem(DataFileUploaderParameter parameter, DataFileUploaderResource resource, out string error, QTI3pItem qti3pItem)
        {
            var xmlContent = resource.XmlContent;
            error = string.Empty;
            // Keep original xml content to save to database to store special characters (these characters can be decode to html characters to be parsed
            var orgXmlConent = xmlContent;
            xmlContent = ServiceUtil.ReplaceSpace(xmlContent);//fix for chrome

            xmlContent = ServiceUtil.EncodeXmlContent(xmlContent);

            if (string.IsNullOrEmpty(xmlContent))
            {
                error = "Xml Content could not be empty!";
                return 0;
            }

            var qtiItemTestTaker = _qTIItemConvert.ConvertFromXmlContent(xmlContent);
            qtiItemTestTaker.Title = resource.Title;

            QTI3pItem qti3pItemData = null;

            try
            {
                if (orgXmlConent.IsUnicode())
                {
                    orgXmlConent = orgXmlConent.CleanUpXmlContentInput();
                    qtiItemTestTaker.XmlContent = orgXmlConent.ConvertFromUnicodeToWindow1252();
                }
                else
                {
                    qtiItemTestTaker.XmlContent = orgXmlConent;
                }
                resource.XmlContent = qtiItemTestTaker.XmlContent;//save history of XmlContent of resource
                qti3pItemData = CreateQTI3pItem(qtiItemTestTaker, parameter, resource, qti3pItem);
            }
            catch (Exception ex)
            {
                error = "There was some error, can not create item right now.";
                return 0;
            }

            if (qti3pItemData != null)
            {
                return qti3pItemData.QTI3pItemID;
            }
            else
            {
                error = "There was some error, can not create item right now.";
                return 0;
            }
        }

        private QTI3pItem CreateQTI3pItem(QTIItemTestMaker qtiItemTemTestMaker, DataFileUploaderParameter parameter, DataFileUploaderResource resource, QTI3pItem qti3pItem)
        {
            if (qtiItemTemTestMaker == null) return null;
            //check exists to update
            if (qti3pItem == null)
            {
                qti3pItem = new QTI3pItem();
                qti3pItem.Title = qtiItemTemTestMaker.Title;
                qti3pItem.QTISchemaID = resource.QtiSchemaID;
                qti3pItem.CorrectAnswer = qtiItemTemTestMaker.CorrectAnswer.ConvertFromUnicodeToWindow1252();
                qti3pItem.OriginPath = string.Format("{0}/{1}", parameter.ExtractedFoler, resource.ResourceFileName);
                qti3pItem.XmlContent = qtiItemTemTestMaker.XmlContent;
                qti3pItem.XmlSource = resource.OriginalContent;
                qti3pItem.QTI3pSourceID = parameter.QTI3pSourceId;
                qti3pItem.UrlPath = parameter.ItemSetPath;
                qti3pItem.From3pUpload = true;
                qti3pItem.CorrectAnswer = ServiceUtil.RollbackSpace(qti3pItem.CorrectAnswer);
                if (parameter.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery)
                {
                    SetQti3pItem(parameter, resource, qti3pItem);
                }
            }
            else
            {
                if (qti3pItem.Title != qtiItemTemTestMaker.Title)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("Tile", qti3pItem.QTI3pItemID, qti3pItem.Title, qtiItemTemTestMaker.Title);
                    qti3pItem.Title = qtiItemTemTestMaker.Title;
                }

                if (qti3pItem.QTISchemaID != qtiItemTemTestMaker.QTISchemaID)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("QTISchemaID", qti3pItem.QTI3pItemID, qti3pItem.QTISchemaID.ToString(), qtiItemTemTestMaker.QTISchemaID.ToString());
                    qti3pItem.QTISchemaID = qtiItemTemTestMaker.QTISchemaID;
                }

                qtiItemTemTestMaker.CorrectAnswer = qtiItemTemTestMaker.CorrectAnswer.ConvertFromUnicodeToWindow1252();
                if (qti3pItem.CorrectAnswer != qtiItemTemTestMaker.CorrectAnswer)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("CorrectAnswer", qti3pItem.QTI3pItemID, qti3pItem.CorrectAnswer, qtiItemTemTestMaker.CorrectAnswer);
                    qti3pItem.CorrectAnswer = qtiItemTemTestMaker.CorrectAnswer;
                }

                var originPath = string.Format("{0}/{1}", parameter.ExtractedFoler, resource.ResourceFileName);
                if (qti3pItem.OriginPath != originPath)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("OriginPath", qti3pItem.QTI3pItemID, qti3pItem.OriginPath, originPath);
                    qti3pItem.OriginPath = originPath;
                }
                if (qti3pItem.XmlContent != qtiItemTemTestMaker.XmlContent)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("XmlContent", qti3pItem.QTI3pItemID, qti3pItem.XmlContent, qtiItemTemTestMaker.XmlContent);
                    qti3pItem.XmlContent = qtiItemTemTestMaker.XmlContent;

                    if (!qti3pItem.From3pUpload)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("UrlPath", qti3pItem.QTI3pItemID, "false", "true");
                        qti3pItem.From3pUpload = true;
                    }
                }
                if (qti3pItem.XmlSource != resource.OriginalContent)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("XmlSource", qti3pItem.QTI3pItemID, qti3pItem.XmlSource, resource.OriginalContent);
                    qti3pItem.XmlSource = resource.OriginalContent;
                }
                if (qti3pItem.UrlPath != parameter.ItemSetPath)
                {
                    //tracking update
                    SaveQTI3pItemUpDateHistory("UrlPath", qti3pItem.QTI3pItemID, qti3pItem.UrlPath, parameter.ItemSetPath);
                    qti3pItem.UrlPath = parameter.ItemSetPath;
                }
                SetQti3pItem(parameter, resource, qti3pItem, false);
            }

            _qTI3pItemRepository.Save(qti3pItem);
            return qti3pItem;
        }

        private void SaveQTI3pItemUpDateHistory(string columnName, int qTI3pItemID, string oldValue, string newValue)
        {
            var trackingQti3pItem = new QTI3pItemUpdateHistory()
            {
                ColumnName = columnName,
                QTI3pItemID = qTI3pItemID,
                OldValue = oldValue,
                NewValue = newValue
            };
            _qTI3pItemUpdateHistoryRepository.Save(trackingQti3pItem);
        }

        private void SetQti3pItem(DataFileUploaderParameter parameter, DataFileUploaderResource resource, QTI3pItem qti3pItem, bool isNew = true)
        {
            if (parameter.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery)
            {
                int? subjectId = null;
                var subject =
                    _subjectRepository.Select().FirstOrDefault(x => x.Name.ToLower() == resource.Subject.ToLower());
                if (subject != null)
                    subjectId = subject.SubjectID;

                int? bloomId = null;
                var bloom =
                    _bloomRepository.Select().FirstOrDefault(x => x.Name.ToLower() == resource.BloomsTaxonomy.ToLower());
                if (bloom != null)
                    bloomId = bloom.BloomsId;

                int? diffId = null;
                var diff =
                    _diffRepository.Select().FirstOrDefault(x => x.Name.ToLower() == resource.Difficulty.ToLower());
                if (diff != null)
                    diffId = diff.ItemDifficultyID;

                var gradeId = GetGradeIdFromGradeLevel(resource.GradeLevel, parameter.GradeMappingList);
                if (isNew)
                {
                    qti3pItem.Difficulty = resource.Difficulty;
                    qti3pItem.Pvalue = resource.PValue;
                    qti3pItem.Subject = resource.Subject;
                    qti3pItem.GradeLevel = resource.GradeLevel;
                    qti3pItem.GradeID = gradeId;
                    qti3pItem.BloomsTaxonomy = resource.BloomsTaxonomy;
                    qti3pItem.ABStandardGUIDs = resource.ABStandardGUIDs;
                    qti3pItem.Identifier = resource.Identifier;
                    qti3pItem.ContentFocus = resource.ContentFocus;
                    qti3pItem.SubjectID = subjectId;
                    qti3pItem.BloomsID = bloomId;
                    qti3pItem.ItemDifficultyID = diffId;
                }
                else
                {
                    if (qti3pItem.Difficulty != resource.Difficulty)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("Difficulty", qti3pItem.QTI3pItemID, qti3pItem.Difficulty, resource.Difficulty);
                        qti3pItem.Difficulty = resource.Difficulty;
                    }
                    if (qti3pItem.Pvalue != resource.PValue)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("PValue", qti3pItem.QTI3pItemID, qti3pItem.Pvalue.ToString(), resource.PValue.ToString());
                        qti3pItem.Pvalue = resource.PValue;
                    }
                    if (qti3pItem.Subject != resource.Subject)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("Subject", qti3pItem.QTI3pItemID, qti3pItem.Subject, resource.Subject);
                        qti3pItem.Subject = resource.Subject;
                    }
                    if (qti3pItem.GradeLevel != resource.GradeLevel)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("GradeLevel", qti3pItem.QTI3pItemID, qti3pItem.GradeLevel, resource.GradeLevel);
                        qti3pItem.GradeLevel = resource.GradeLevel;
                    }
                    if (qti3pItem.GradeID != gradeId)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("GradeID", qti3pItem.QTI3pItemID, qti3pItem.GradeID.ToString(), gradeId.ToString());
                        qti3pItem.GradeID = gradeId;
                    }
                    if (qti3pItem.BloomsTaxonomy != resource.BloomsTaxonomy)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("BloomsTaxonomy", qti3pItem.QTI3pItemID, qti3pItem.BloomsTaxonomy, resource.BloomsTaxonomy);
                        qti3pItem.BloomsTaxonomy = resource.BloomsTaxonomy;
                    }
                    if (qti3pItem.ABStandardGUIDs != resource.ABStandardGUIDs)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("ABStandardGUIDs", qti3pItem.QTI3pItemID, qti3pItem.ABStandardGUIDs, resource.ABStandardGUIDs);
                        qti3pItem.ABStandardGUIDs = resource.ABStandardGUIDs;
                    }
                    if (qti3pItem.Identifier != resource.Identifier)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("Identifier", qti3pItem.QTI3pItemID, qti3pItem.Identifier, resource.Identifier);
                        qti3pItem.Identifier = resource.Identifier;
                    }
                    if (qti3pItem.ContentFocus != resource.ContentFocus)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("ContentFocus", qti3pItem.QTI3pItemID, qti3pItem.ContentFocus, resource.ContentFocus);
                        qti3pItem.ContentFocus = resource.ContentFocus;
                    }
                    if (qti3pItem.SubjectID != subjectId)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("SubjectID", qti3pItem.QTI3pItemID, qti3pItem.SubjectID.ToString(), subjectId.ToString());
                        qti3pItem.SubjectID = subjectId;
                    }
                    if (qti3pItem.BloomsID != bloomId)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("BloomsID", qti3pItem.QTI3pItemID, qti3pItem.BloomsID.ToString(), bloomId.ToString());
                        qti3pItem.BloomsID = bloomId;
                    }
                    if (qti3pItem.ItemDifficultyID != diffId)
                    {
                        //tracking update
                        SaveQTI3pItemUpDateHistory("ItemDifficultyID", qti3pItem.QTI3pItemID, qti3pItem.ItemDifficultyID.ToString(), diffId.ToString());
                        qti3pItem.ItemDifficultyID = diffId;
                    }
                }
            }
        }

        public QTIItemData ConvertFrom3pItemUploadedToItem(int qti3pItemId, int qtiGroupId, int userId,
            bool uploadS3, string s3BucketName, string s3FolderName)
        {
            var qti3pUpload = this.GetQti3pItemById(qti3pItemId);
            if (qti3pUpload != null)
            {
                //get img from xmlContent to copy to itemset
                XmlContentProcessing doc = new XmlContentProcessing(qti3pUpload.XmlContent);
                XmlNodeList imgNodeList = doc.GetElementsByTagName("img");
                if (imgNodeList != null || imgNodeList.Count > 0)
                {
                    //string newItemSetPath = testItemMediaPath.RemoveEndSlash() + "\\ItemSet_" + qtiGroupId.ToString();
                    for (int i = 0; i < imgNodeList.Count; i++)
                    {
                        XmlNode imgNode = imgNodeList[i];
                        var src = XmlUtils.GetNodeAttributeCaseInSensitive(imgNode, "src");
                        try
                        {
                            //get value of src
                            if (!string.IsNullOrWhiteSpace(src))
                            {
                                var fileName = Path.GetFileName(src);
                                //var newImagePath = newItemSetPath + "\\images" + "\\" + fileName;
                                if (!src.ToLower().StartsWith("http"))
                                {
                                    //create folder itemset
                                    var imgPath = string.Format("{0}/{1}",
                                        ConfigurationManager.AppSettings["ThirdPartyItemMediaPath"],
                                        src.RemoveStartSlash());
                                    // Copy img to itemset folder
                                    if (File.Exists(imgPath))
                                    {
                                        //var subfolderPath = newItemSetPath + "\\images";
                                        //if (!Directory.Exists(subfolderPath))
                                        //{
                                        //    Directory.CreateDirectory(subfolderPath);
                                        //}

                                        //File.Copy(imgPath, newImagePath, true);

                                        string s3ItemSetPath = "ItemSet_" + qtiGroupId.ToString();
                                        var s3FullFileName = s3ItemSetPath + "/images/" + fileName;
                                        //if (uploadS3)
                                        {
                                            using (
                                                FileStream fsSource = new FileStream(imgPath, FileMode.Open,
                                                    FileAccess.Read))
                                            {
                                                _s3Service.UploadRubricFile(s3BucketName, s3FolderName + s3FullFileName,
                                                    fsSource);
                                            }
                                        }

                                        //update ItemSet_ID in xmlContent;
                                        qti3pUpload.XmlContent = qti3pUpload.XmlContent.Replace(src, s3FullFileName);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }

                //because xmlcontent was encoded to window 1252 when saving, now it's necessary to convert into unicode
                var xmlContent = qti3pUpload.XmlContent.ConvertFromWindow1252ToUnicode();
                //Create a new QtiItem
                var error = string.Empty;
                var qtiItemId = this.CreateQTIItem(userId, qtiGroupId,
                    xmlContent, false, out error, null);//use this method to make sure all thing is ok
                if (string.IsNullOrEmpty(error))
                {
                    if (qtiItemId > 0)
                    {
                        //get qtiitem to make some update
                        var qtiItem = this.GetQtiItemById(qtiItemId);
                        qtiItem.SourceID = qti3pUpload.QTI3pItemID;
                        this.Save(qtiItem);

                        //add relationInfo
                        var stateStandards = _qTI3pItemStateStandardRepository.Select().Where(x => x.Qti3pItemId == qti3pItemId);
                        foreach (var stateStandard in stateStandards)
                        {
                            var qtiitemStandard = new QTIItemStateStandard()
                            {
                                QTIItemID = qtiItemId,
                                StateStandardID = stateStandard.StateStandardId
                            };
                            _qtiItemStateStandardRepository.Save(qtiitemStandard);
                        }
                        return qtiItem;
                    }
                }
            }
            return null;
        }

        private string ReadPassageContent(ref DataFileUploaderResource resourcePassage, DataFileUploaderParameter parameter)
        {
            resourcePassage.ProcessingStep.Append(" -> Read passage content from file");
            string passagePath = string.Format("{0}/{1}", parameter.ExtractedFoler.RemoveEndSlash(), resourcePassage.ResourceFileName.RemoveStartSlash());
            var passageContent = File.ReadAllText(passagePath);
            return passageContent;
        }

        private string AdjustPassageContent(string passageContent)
        {
            //remove <?xml version="1.0" encoding="utf-8"?>
            passageContent = passageContent.Replace("<?xml version=\"1.0\" encoding=\"utf-8\"?>", "");
            //format the content for the passage
            string newPassageContent =
                string.Format(
                    "<passage toolName=\"linkitTLF\" xmlUnicode=\"true\" toolVersion=\"2.0\"><div class=\"passage\" styleName=\"passage\">{0}</div></passage>", passageContent);
            return newPassageContent;
        }

        private void SavePassageToLocal(ref DataFileUploaderResult uploadResult, ref DataFileUploaderResource resourcePassage, DataFileUploaderParameter parameter)
        {
            string destinationPassagePath = string.Empty;

            try
            {
                destinationPassagePath = string.Empty;
                // Copy passage to itemset folder ( for back up), It will be upload to S3 for real usage
                //var destinationPassageFolder = string.Format("{0}/passages", parameter.ItemSetPath.RemoveEndSlash());
                var destinationPassageFolder = parameter.ItemSetPath.RemoveEndSlash();
                if (!Directory.Exists(destinationPassageFolder))
                {
                    Directory.CreateDirectory(destinationPassageFolder);
                }
                destinationPassagePath = string.Format("{0}/{1}", destinationPassageFolder,
                    Path.GetFileName(resourcePassage.ResourceFileName));

                resourcePassage.ProcessingStep.Append(" -> Start copying passage");
                //check folder

                //File.Copy(passagePath, destinationPassagePath, true);
                //Need to add
                //<passage toolName="linkitTLF" xmlUnicode="true" toolVersion="2.0" >
                //<div class="passage" styleName="passage">
                //to the passage content
                string passageContent = ReadPassageContent(ref resourcePassage, parameter);
                var newPassageContent = AdjustPassageContent(passageContent);

                //write new content to a new location
                //if (File.Exists(destinationPassagePath))
                //{
                //    File.Delete(destinationPassagePath);
                //}
                //File.WriteAllText(destinationPassagePath, newPassageContent);
                resourcePassage.ProcessingStep.Append("-> Copying passage successfully");
                resourcePassage.ProcessingStep.Append("-> Copying passage images");
                if (resourcePassage.MediaFileList != null)
                {
                    foreach (var image in resourcePassage.MediaFileList)
                    {
                        string imageDestinationPath = string.Format("{0}/{1}", destinationPassageFolder.RemoveEndSlash(),
                            image.RemoveStartSlash());
                        var directoryImage = Path.GetDirectoryName(imageDestinationPath);
                        if (!Directory.Exists(directoryImage))
                        {
                            Directory.CreateDirectory(directoryImage);
                        }
                        //copy images
                        string imagePath = string.Format("{0}/{1}", parameter.ExtractedFoler.RemoveEndSlash(), image.RemoveStartSlash());
                        if (File.Exists(imagePath))
                        {
                            File.Copy(imagePath, imageDestinationPath, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Due to copying for backup,ignore the erro if any
                //resourcePassage.Error =  "Can not copy this passage"; //ignore it,not set value
                resourcePassage.ProcessingStep.Append("-> Copying passage failed with exception");
                resourcePassage.ErrorDetail = ex.GetFullExceptionMessage();
            }
        }

        private void SavePassageToS3(ref DataFileUploaderResult uploadResult,
            ref DataFileUploaderResource resourcePassage, DataFileUploaderParameter parameter)
        {
            try
            {
                var passageContent = ReadPassageContent(ref resourcePassage, parameter);
                if (string.IsNullOrEmpty(passageContent))
                {
                    uploadResult.Result = "Can not read passage content from file";
                    uploadResult.Error = uploadResult.Result;
                }
                var newPassageContent = AdjustPassageContent(passageContent);
                //update absolute S3 link for images file on Passage
                foreach (var image in resourcePassage.MediaFileList)
                {
                    var s3FullLinkImage = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(),
                        image.RemoveStartSlash());
                    if (parameter.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery)
                    {
                        var format = string.Format("{0}{1}", "../", image);
                        newPassageContent = newPassageContent.Replace(format, s3FullLinkImage);
                    }
                    else
                    {
                        newPassageContent = newPassageContent.Replace(image, s3FullLinkImage);
                    }
                }
                // convert string to stream
                byte[] byteArray = Encoding.UTF8.GetBytes(newPassageContent);

                resourcePassage.ProcessingStep.Append("-> Start uploading to S3");
                using (MemoryStream inputStream = new MemoryStream(byteArray))
                {
                    var s3FilePath = CreateS3PassageForUploading(parameter, resourcePassage.ResourceFileName);
                    if (s3FilePath.ToLower().StartsWith("http")) //full link
                    {
                        s3FilePath = s3FilePath.Replace(parameter.S3Subdomain, "");//remove sub domain to get right linkt to upload to S3
                    }

                    var uploadS3Result = _s3Service.UploadRubricFile(parameter.AUVirtualTestBucketName,
                        s3FilePath, inputStream);
                    if (uploadS3Result == null || !uploadS3Result.IsSuccess)
                    {
                        resourcePassage.ProcessingStep.Append("-> Uploading S3 failed");
                        resourcePassage.Error = string.Format("Can not upload {0} to S3 {1}",
                            resourcePassage.ResourceFileName,
                            s3FilePath);
                        uploadResult.Error = resourcePassage.Error;
                        uploadResult.Result = "Can not upload passage to S3."; // message to user

                        if (uploadResult != null)
                        {
                            resourcePassage.ErrorDetail =
                                string.Format("ErrorCode:{0},ErrorMessage:{1},ReturnValue:{2}",
                                    uploadS3Result.ErrorCode ?? string.Empty,
                                    uploadS3Result.ErrorMessage ?? string.Empty,
                                    uploadS3Result.ReturnValue ?? string.Empty);
                            uploadResult.Error = resourcePassage.ErrorDetail;
                        }
                    }
                    else
                    {
                        resourcePassage.ProcessingStep.Append("-> Sending S3 successfully.");
                        resourcePassage.ProcessingStep.Append("-> Sending passage images to S3");
                        if (resourcePassage.MediaFileList != null)
                        {
                            foreach (var image in resourcePassage.MediaFileList)
                            {
                                string imagePath = string.Format("{0}/{1}", parameter.ExtractedFoler.RemoveEndSlash(), image.RemoveStartSlash());

                                using (
                                    FileStream inputImageStream = new FileStream(imagePath, FileMode.Open,
                                        FileAccess.Read))
                                {
                                    var s3ImageFilePath = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(), image.RemoveStartSlash());
                                    if (s3ImageFilePath.ToLower().StartsWith("http")) //full link
                                    {
                                        s3ImageFilePath = s3ImageFilePath.Replace(parameter.S3Subdomain, "");//remove sub domain to get right linkt to upload to S3
                                    }
                                    var uploadS3ImageResult =
                                        _s3Service.UploadRubricFile(parameter.AUVirtualTestBucketName,
                                            s3ImageFilePath, inputImageStream);
                                    if (uploadS3ImageResult == null || !uploadS3ImageResult.IsSuccess)
                                    {
                                        resourcePassage.ProcessingStep.Append("-> Uploading image of passage to S3 failed");
                                        resourcePassage.Error = string.Format("Can not upload image of passage {0} to S3", image);
                                        uploadResult.Error = resourcePassage.Error;
                                        uploadResult.Result = "Can not upload image passage to S3."; // message to user
                                    }
                                }
                                if (!string.IsNullOrEmpty(uploadResult.Error))
                                {
                                    return;//stop if there's any error
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resourcePassage.ProcessingStep.Append("-> Uploading S3 failed with exception");
                resourcePassage.Error = string.Format("Can not upload {0} to S3 ", resourcePassage.ResourceFileName);
                resourcePassage.ErrorDetail = ex.Message;

                uploadResult.Error = resourcePassage.Error;
                uploadResult.Result = "Can not upload passage to S3."; // message to user
            }
        }

        private void SavePassageMetaData(DataFileUploaderResult uploadResult,
            DataFileUploaderResource passage, DataFileUploaderParameter parameter, int dataFileUploadLogId)
        {
            var passageFile = passage.ResourceFileName;
            var dataFileUploadPassageID = 0;
            var qti3pPassageID = 0;
            if (!parameter.UploadTo3pItem)//Upload at Item Set
            {
                string trackedObject = string.Empty;
                try
                {
                    //save passage information into table
                    var uploadPassage = new DataFileUploadPassage()
                    {
                        DataFileUploadTypeID = uploadResult.DataFileUploadTypeId,
                        FileName = passageFile,
                        Fullpath =
                            CreateS3PassageLink(parameter, passageFile),
                        DataFileUploadLogID = dataFileUploadLogId
                    };
                    passage.ProcessingStep.Append(" -> Start insert into DataFileUploadPassage");
                    trackedObject = string.Format(" uploadPassage: DataFileUploadTypeID={0},FileName={1},Fullpath={2},DataFileUploadLogID={3}",
                        uploadPassage.DataFileUploadTypeID,
                        uploadPassage.FileName,
                        uploadPassage.Fullpath,
                        uploadPassage.DataFileUploadLogID);
                    _dataFileUploadPassageRepository.Save(uploadPassage);
                    dataFileUploadPassageID = uploadPassage.DataFileUploadPassageID;
                    if (dataFileUploadPassageID == 0)
                    {
                        uploadResult.Result = "Can not save passage.";
                        uploadResult.Error = "Can not save passage to table DataFileUploadPassage.";
                        passage.Error = uploadResult.Result;
                        passage.ErrorDetail = uploadResult.Error;
                        passage.ErrorDetail += trackedObject;
                    }
                }
                catch (Exception ex)
                {
                    uploadResult.Result = "Can not save passage.";
                    uploadResult.Error = "Can not save passage to table DataFileUploadPassage." +
                                         ex.GetFullExceptionMessage(); // message to user
                    passage.Error = uploadResult.Result;
                    passage.ErrorDetail = uploadResult.Error;
                    passage.ErrorDetail += trackedObject;
                }
            }
            else
            {
                string trackedObject = string.Empty;
                try
                {
                    //check passage is exists or not
                    var oldPassage = _qTI3pPassageRepository.Select().Where(x => x.Identifier == passage.Identifier).FirstOrDefault();

                    //Create QTI3pPassage object
                    var qTI3pPassage = CreateQTI3pPassageObject(passage, parameter, oldPassage);
                    passage.ProcessingStep.Append(" -> Start insert into QTI3pPassage");
                    trackedObject =
                        string.Format(
                            "qTI3pPassage: PassageName={0},Fullpath={1},Number={2},Qti3pSourceID={3},Identifier={4},PassageTitle={5}",
                            qTI3pPassage.PassageName,
                            qTI3pPassage.Fullpath,
                            qTI3pPassage.Number,
                            qTI3pPassage.Qti3pSourceID,
                            qTI3pPassage.Identifier,
                            qTI3pPassage.PassageTitle);
                    _qTI3pPassageRepository.Save(qTI3pPassage);
                    qti3pPassageID = qTI3pPassage.QTI3pPassageID;
                    if (qti3pPassageID == 0)
                    {
                        uploadResult.Result = "Can not save passage.";
                        uploadResult.Error = "Can not save passage to table QTI3pPassage.";
                        passage.Error = uploadResult.Result;
                        passage.ErrorDetail = uploadResult.Error;
                        passage.ErrorDetail += trackedObject;
                    }
                    int? passageTypeId = null;
                    if (!string.IsNullOrEmpty(passage.PassageType))
                    {
                        passage.ProcessingStep.Append(" -> Start Getting PassageType: " + passage.PassageType);
                        var passageTypeName = passage.PassageType.ToLower();
                        var passageType =
                            _qti3PProgressPassageTypeResRepository.Select()
                                .Where(
                                    x => x.Qti3pProgressPassageTypeName.ToLower().Equals(passageTypeName))
                                .FirstOrDefault();
                        if (passageType == null)
                        {
                            //create a new one
                            passage.ProcessingStep.Append(" -> Start Saving PassageType: " + passage.PassageType);
                            passageType = new Qti3pProgressPassageType()
                            {
                                Qti3pProgressPassageTypeName = passage.PassageType
                            };
                            _qti3PProgressPassageTypeResRepository.Save(passageType);
                            passageTypeId = passageType.Qti3pProgressPassageTypeID;
                        }
                        else
                        {
                            passageTypeId = passageType.Qti3pProgressPassageTypeID;
                        }
                        passage.ProcessingStep.Append(" -> PassageTypeID: " + passageTypeId.ToString());
                    }
                    int? passageGenreId = null;
                    if (!string.IsNullOrEmpty(passage.Genre))
                    {
                        passage.ProcessingStep.Append(" -> Start Getting PassageGenre: " + passage.Genre);
                        var passageGenreName = passage.Genre.ToLower();
                        var passageGenre =
                            _qti3PProgressPassageGenreRepository.Select()
                                .Where(
                                    x => x.Qti3pProgressPassageGenreName.ToLower().Equals(passageGenreName))
                                .FirstOrDefault();
                        if (passageGenre == null)
                        {
                            //create a new one
                            passageGenre = new Qti3pProgressPassageGenre()
                            {
                                Qti3pProgressPassageGenreName = passage.Genre
                            };
                            passage.ProcessingStep.Append(" -> Start Saving PassageGenre: " + passage.Genre);
                            _qti3PProgressPassageGenreRepository.Save(passageGenre);
                            passageGenreId = passageGenre.Qti3pProgressPassageGenreID;
                        }
                        else
                        {
                            passageGenreId = passageGenre.Qti3pProgressPassageGenreID;
                        }
                        passage.ProcessingStep.Append(" -> PassageGenreID: " + passageGenreId.ToString());
                    }
                    var qti3pPassageProgress = new QTI3pPassageProgress()
                    {
                        Qti3pPassageID = qti3pPassageID,
                        Qti3pProgressPassageTypeID = passageTypeId,
                        Qti3pProgressPassageGenreID = passageGenreId,
                        Lexile = passage.Lexile,
                        Spache = passage.Spache,
                        DaleChall = passage.DaleChall,
                        RMM = passage.RMM
                    };
                    passage.ProcessingStep.Append(" -> Start Saving qti3pPassageProgress" + passage.Genre);
                    _qti3PPassageProgressRepository.Save(qti3pPassageProgress);
                }
                catch (Exception ex)
                {
                    uploadResult.Result = "Can not save passage.";
                    uploadResult.Error = "Can not save passage to table QTI3pPassage." +
                                         ex.GetFullExceptionMessage(); // message to user

                    passage.Error = uploadResult.Result;
                    passage.ErrorDetail = uploadResult.Error;
                    passage.ErrorDetail += trackedObject;
                }
            }

            //update xmlcontent for all resource which use this passage
            var s3PassageLink = CreateS3PassageLink(parameter, passageFile);

            for (int i = 0; i < uploadResult.Resources.Count; i++)
            {
                if (!string.IsNullOrEmpty(uploadResult.Result))
                {
                    return;
                }
                var resource = uploadResult.Resources[i];

                if (resource.PassageList.Contains(passageFile))
                {
                    resource.ProcessingStep.Append("->Start updating passage for xmlcontent.");
                    try
                    {
                        XmlContentProcessing doc = new XmlContentProcessing(resource.XmlContent);
                        if (doc == null)
                        {
                            resource.Error = "Can not update passage for item";
                            resource.ErrorDetail =
                                "Can not load resource XmlContent to update passage for item, can not load into XmlContentProcessing";

                            uploadResult.Result = resource.Error;
                            uploadResult.Error = resource.ErrorDetail;

                            return;
                        }
                        if (!doc.IsXmlLoadedSuccess)
                        {
                            resource.ProcessingStep.Append(
                                "->Load xmlcontent to update passage failed with exception.");
                            resource.Error = "Can not update passage for item.";
                            resource.ErrorDetail =
                                "Can not load resource XmlContent to update passage for item. Error:" +
                                doc.LoadXmlContentException;

                            uploadResult.Result = resource.Error;
                            uploadResult.Error = resource.ErrorDetail;
                            return;
                        }

                        XmlNodeList objectNodeList = doc.GetElementsByTagName("object");
                        for (int j = 0; j < objectNodeList.Count; j++)
                        {
                            XmlNode objectNode = objectNodeList[j];
                            //get attribute
                            var data = XmlUtils.GetNodeAttribute(objectNode, "data");
                            if (data == passageFile)
                            {
                                XmlUtils.SetOrUpdateNodeAttribute(ref objectNode, "data", s3PassageLink);
                                XmlUtils.AddAttribute(objectNode, "type", "text/html");
                                XmlUtils.AddAttribute(objectNode, "class", "referenceObject");
                                XmlUtils.AddAttribute(objectNode, "stylename", "referenceObject");
                                XmlUtils.AddAttribute(objectNode, "dataFileUploadPassageID", dataFileUploadPassageID.ToString());
                                XmlUtils.AddAttribute(objectNode, "dataFileUploadTypeID", uploadResult.DataFileUploadTypeId.ToString());
                                XmlUtils.AddAttribute(objectNode, "Qti3pPassageID", qti3pPassageID.ToString());
                                XmlUtils.AddAttribute(objectNode, "Qti3pSourceID", parameter.QTI3pSourceId.ToString());
                            }
                        }
                        resource.XmlContent = doc.GetXmlContent();
                        if (string.IsNullOrEmpty(resource.XmlContent))
                        {
                            resource.Error = "Can not update passage for item";
                            resource.ErrorDetail = "XmlContent is empty after updating objecb tag";

                            uploadResult.Result = resource.Error;
                            uploadResult.Error = resource.ErrorDetail;
                            return;
                        }
                        if (parameter.UploadTo3pItem)
                            resource.QTI3pPassageIdList.Add(qti3pPassageID);
                        else
                            resource.DataFileUploadPassageIdList.Add(dataFileUploadPassageID);
                    }
                    catch (Exception ex)
                    {
                        resource.ProcessingStep.Append(
                            "->Updating passage for xmlcontent failed with exception.");
                        resource.Error = "Can not update passage for item";
                        resource.ErrorDetail = ex.GetFullExceptionMessage();

                        resource.Error = resource.Error;
                        resource.ErrorDetail = resource.ErrorDetail;
                        return;
                    }
                }
            }
        }

        private QTI3pPassage CreateQTI3pPassageObject(DataFileUploaderResource passage,
            DataFileUploaderParameter parameter, QTI3pPassage qTI3pPassage = null)
        {
            var passageFile = passage.ResourceFileName;
            var passageFileName = passageFile;
            if (passageFileName.StartsWith("/"))
            {
                passageFileName = passageFileName.Remove(0, 1);
            }
            passage.ProcessingStep.Append(" -> Start Get Passage name");
            passageFileName = Path.GetFileName(passageFileName); //passages/5578.html -> get only 5578.html
            //var passageNumber = passageFileName.Split('.')[0];
            passage.ProcessingStep.Append(" -> Start Get Passage number");
            var passageNumber = passage.Identifier.Split('-')[1]; //Identifier looks like passage-5578 -> get 5578 only
            if ((qTI3pPassage == null && parameter.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery) || parameter.QTI3pSourceId != (int)QTI3pSourceEnum.Mastery)
            {
                qTI3pPassage = new QTI3pPassage();
            }
            qTI3pPassage.PassageName = passageFileName;
            qTI3pPassage.Fullpath = CreateS3PassageLink(parameter, passageFile);
            qTI3pPassage.Number = passageNumber;
            qTI3pPassage.Qti3pSourceID = parameter.QTI3pSourceId;
            qTI3pPassage.Identifier = passage.Identifier;
            qTI3pPassage.PassageTitle = TruncateToFixedLength(passage.PassageTitle, 200);

            if (parameter.QTI3pSourceId == (int)QTI3pSourceEnum.Mastery)
            {
                int? contentAreaId = null;
                var contentArea =
                    _contentAreaRepository.Select().FirstOrDefault(x => x.Name.ToLower() == passage.PContentArea.ToLower());
                if (contentArea != null)
                    contentAreaId = contentArea.ContentAreaId;

                int? textTypeId = null;
                if (!string.IsNullOrWhiteSpace(passage.PTextType))
                {
                    var textType =
                    _textTypeRepository.Select().FirstOrDefault(x => x.Name.ToLower() == passage.PTextType.ToLower() && x.TypeID == 1);
                    if (textType == null)
                    {
                        textType = new QTI3pTextType();
                        textType.TypeID = 1;
                        textType.Name = passage.PTextType;
                        _textTypeRepository.Save(textType);
                    }
                    textTypeId = textType.TextTypeID;
                }

                int? textSubTypeId = null;
                if (!string.IsNullOrWhiteSpace(passage.PTextSubType))
                {
                    var textSubType =
                        _textTypeRepository.Select()
                            .FirstOrDefault(x => x.Name.ToLower() == passage.PTextSubType.ToLower() && x.TypeID == 2);
                    if (textSubType == null)
                    {
                        textSubType = new QTI3pTextType();
                        textSubType.TypeID = 2;
                        textSubType.Name = passage.PTextSubType;
                        _textTypeRepository.Save(textSubType);
                    }

                    textSubTypeId = textSubType.TextTypeID;
                }

                int? wordCountId = null;
                var wordCount =
                    _wordCountRepository.Select().FirstOrDefault(x => x.Name.ToLower() == passage.PWordCount.ToLower());
                if (wordCount != null)
                    wordCountId = wordCount.WordCountID;

                int? fleschKincaidId = null;
                var fleschKincaid =
                    _fleschKinkaidRepository.Select().FirstOrDefault(x => x.Name.ToLower() == passage.PFleschKincaid.ToLower());
                if (fleschKincaid != null)
                    fleschKincaidId = fleschKincaid.FleschKincaidID;

                qTI3pPassage.GradeLevel = passage.PGradeLevel;
                qTI3pPassage.Subject = passage.PSubject;
                qTI3pPassage.PassageStimulus = TruncateToFixedLength(passage.PStimulus, 50);
                qTI3pPassage.ContentArea = TruncateToFixedLength(passage.PContentArea, 50);
                qTI3pPassage.TextType = TruncateToFixedLength(passage.PTextType, 50);
                qTI3pPassage.TextSubType = TruncateToFixedLength(passage.PTextSubType, 50);
                qTI3pPassage.PassageSource = TruncateToFixedLength(passage.PassageSource, 50);
                qTI3pPassage.WordCount = TruncateToFixedLength(passage.PWordCount, 50);
                qTI3pPassage.Ethnicity = TruncateToFixedLength(passage.PEthnicity, 50);
                qTI3pPassage.CommissionedStatus = TruncateToFixedLength(passage.PCommissionedStatus, 50);
                qTI3pPassage.FleschKincaid = TruncateToFixedLength(passage.PFleschKincaid, 50);
                qTI3pPassage.Gender = TruncateToFixedLength(passage.PGender, 50);
                qTI3pPassage.MultiCultural = TruncateToFixedLength(passage.PMultiCultural, 50);
                qTI3pPassage.CopyrightYear = TruncateToFixedLength(passage.PCopyrightYear, 50);
                qTI3pPassage.CopyrightOwner = TruncateToFixedLength(passage.PCopyrightOwner, 50);
                qTI3pPassage.PassageSourceTitle = TruncateToFixedLength(passage.PSourceTitle, 50);
                qTI3pPassage.Author = TruncateToFixedLength(passage.PAuthor, 50);
                qTI3pPassage.GradeID = GetGradeIdFromGradeLevel(passage.PGradeLevel, parameter.GradeMappingList);
                qTI3pPassage.ContentAreaID = contentAreaId;
                qTI3pPassage.TextTypeID = textTypeId;
                qTI3pPassage.TextSubTypeID = textSubTypeId;
                qTI3pPassage.WordCountID = wordCountId;
                qTI3pPassage.FleschKincaidID = fleschKincaidId;
            }
            return qTI3pPassage;
        }

        private string TruncateToFixedLength(string input, int length)
        {
            if (string.IsNullOrEmpty(input) || input.Length <= length)
                return input;
            return input.Substring(0, length);
        }

        private int GetGradeIdFromGradeLevel(string gradeLevel, List<GradeMapping> gradeMappings)
        {
            if (gradeMappings.Any())
            {
                var grade = gradeMappings.FirstOrDefault(x => x.GradeLevel == gradeLevel);
                if (grade != null)
                    return grade.GradeId;
            }
            return 0;
        }

        public void SaveItemMediaToLocal(ref DataFileUploaderResult uploadResult,
            ref DataFileUploaderResource resource, DataFileUploaderParameter parameter)
        {
            if (resource.MediaFileList.Count > 0)
            {
                resource.ProcessingStep.Append(string.Format("->Start copying {0} media files.",
                    resource.MediaFileList.Count));
            }
            int countSuccessCopiedFiles = 0;
            foreach (var mediaFile in resource.MediaFileList)
            {
                try
                {
                    if (!mediaFile.ToLower().StartsWith("http") &&
                        !string.IsNullOrEmpty(mediaFile))
                    {
                        var imgPath = string.Format("{0}/{1}", parameter.ExtractedFoler,
                            mediaFile);
                        // Copy img to itemset folder
                        var copiedImgPath = string.Format("{0}/{1}", parameter.ItemSetPath,
                            mediaFile);
                        var copiedImgFolder = Path.GetDirectoryName(copiedImgPath); //get path without file name
                        if (!Directory.Exists(copiedImgFolder))
                        {
                            Directory.CreateDirectory(copiedImgFolder);
                        }
                        File.Copy(imgPath, copiedImgPath, true);
                        countSuccessCopiedFiles++;
                    }
                }
                catch (Exception ex)
                {
                    resource.Error = string.Format("Can not copy {0} ", mediaFile);
                    resource.ErrorDetail = ex.Message;
                }
            }
            if (resource.MediaFileList.Count > 0)
            {
                resource.ProcessingStep.Append(string.Format(
                    "->Finish copying {0} media files.",
                    countSuccessCopiedFiles));
            }
        }

        public void SaveItemMediaToS3(ref DataFileUploaderResult uploadResult,
            ref DataFileUploaderResource resource, DataFileUploaderParameter parameter)
        {
            if (resource.MediaFileList.Count > 0)
            {
                resource.ProcessingStep.Append(string.Format("->Start uploading {0} media files to S3.",
                    resource.MediaFileList.Count));
            }
            int countSuccessUploadS3Files = 0;
            foreach (var mediaFile in resource.MediaFileList)
            {
                //start upload to S3
                try
                {
                    var fileFullPath = string.Format("{0}/{1}", parameter.ExtractedFoler,
                        mediaFile);
                    using (
                        FileStream inputStream = new FileStream(fileFullPath, FileMode.Open,
                            FileAccess.Read))
                    {
                        // 5 minutes
                        var s3FilePath = string.Empty;
                        if (parameter.S3TestMedia.ToLower().StartsWith("http"))
                        {
                            //use absolute s3 link
                            s3FilePath = string.Format("{0}/{1}", parameter.S3TestMedia.RemoveEndSlash(),
                                mediaFile.RemoveStartSlash());
                            //remove subdomain to enable uploading to S3
                            s3FilePath = s3FilePath.Replace(parameter.S3Subdomain, "");
                        }
                        else
                        {
                            s3FilePath = parameter.S3TestMedia + "/" + mediaFile;
                            s3FilePath = s3FilePath.Replace("//", "/");
                        }

                        var uploadS3Result =
                            _s3Service.UploadRubricFile(parameter.AUVirtualTestBucketName,
                                s3FilePath,
                                inputStream);
                        if (uploadS3Result == null || !uploadS3Result.IsSuccess)
                        {
                            resource.Error = string.Format("Can not upload {0} to S3 {1}",
                                mediaFile, s3FilePath);
                            if (uploadResult != null)
                            {
                                resource.ErrorDetail = string.Format(
                                    "ErrorCode:{0},ErrorMessage:{1},ReturnValue:{2}",
                                    uploadS3Result.ErrorCode,
                                    uploadS3Result.ErrorMessage, uploadS3Result.ReturnValue);
                            }
                        }
                        else
                        {
                            countSuccessUploadS3Files++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    resource.Error = string.Format("Can not upload {0} to S3 ", mediaFile);
                    resource.ErrorDetail = ex.Message;
                }
            }

            if (resource.MediaFileList.Count > 0)
            {
                resource.ProcessingStep.Append(
                    string.Format("->Finish uploading S3 {0} media files.",
                        countSuccessUploadS3Files));
            }
        }

        private void SaveDOKToQTIITemITemTag(DataFileUploaderResource resource, int qtiItemId, int itemTagCategoryId)
        {
            if (resource.DOK > 0)
            {
                try
                {
                    if (itemTagCategoryId > 0)
                    {
                        var dok = _qTi3pDOKRepository.Select().FirstOrDefault(x => x.QTI3pDOKID == resource.DOK);
                        if (dok != null)
                        {
                            var dokName = dok.Name;
                            var itemtag =
                                _itemTagRepository.Select()
                                    .FirstOrDefault(
                                        x =>
                                            x.ItemTagCategoryID == itemTagCategoryId &&
                                            x.Name.ToLower() == dokName.ToLower());
                            if (itemtag == null)
                            {
                                itemtag = new ItemTag()
                                {
                                    ItemTagCategoryID = itemTagCategoryId,
                                    Name = dokName
                                };
                                _itemTagRepository.Save(itemtag);
                            }
                            var qtiItemItemTag = new QtiItemItemTag()
                            {
                                ItemTagID = itemtag.ItemTagID,
                                QtiItemID = qtiItemId
                            };

                            _qtiItemItemTagRepository.Save(qtiItemItemTag);
                        }
                    }
                }
                catch (Exception)
                {
                    //nothing
                }
            }
        }

        private void SaveQtiItemDataFileUploadPassage(DataFileUploaderResource resource, int qtiItemId)
        {
            var dataFileUploadPassageIdList = resource.DataFileUploadPassageIdList.Distinct().ToList();
            if (dataFileUploadPassageIdList.Count > 0)
            {
                resource.ProcessingStep.Append(
                        string.Format(
                            "->Insert into QtiItemDataFileUploadPassage for dataFileUploadPassageIdList: {0}",
                            string.Join(",", dataFileUploadPassageIdList)));
            }
            foreach (var dataFileUploadPassageId in dataFileUploadPassageIdList)
            {
                var itemPassage = new QtiItemDataFileUploadPassage()
                {
                    QtiItemID = qtiItemId,
                    DataFileUploadPassageID = dataFileUploadPassageId
                };
                _qtiItemDataFileUploadPassageRepository.Save(itemPassage);
            }

            //save to QTIItemStateStandard
            foreach (var guid in resource.GUIDList)
            {
                var stateStandard = _masterStandardRepository.Select().FirstOrDefault(x => x.GUID == guid && !x.Archived);
                if (stateStandard != null)
                {
                    var qtiStateStandard = new QTIItemStateStandard()
                    {
                        QTIItemID = qtiItemId,
                        StateStandardID = stateStandard.MasterStandardID
                    };

                    _qtiItemStateStandardRepository.Save(qtiStateStandard);
                }
            }
        }

        private void SaveQTI3pItemToPassage(DataFileUploaderResource resource, int qtiItemId)
        {
            //save to QTI3pItemToPassage
            var qti3pPassageIdList = resource.QTI3pPassageIdList.Distinct().ToList();
            if (qti3pPassageIdList.Count > 0)
            {
                resource.ProcessingStep.Append(
                    string.Format(
                        "->Insert into QTI3pItemToPassage for qti3pPassageIdList: {0}",
                        string.Join(",", qti3pPassageIdList)));
            }
            var oldQti3pItemPassages =
                _qTI3pItemToPassageRepository.Select().Where(x => x.Qti3pItemId == resource.QtiItemId).ToList();
            var oldQti3pItemPassageIds = oldQti3pItemPassages.Select(x => x.Qti3pItemPassageId);
            var newQti3PassageIds = qti3pPassageIdList.Where(x => !oldQti3pItemPassageIds.Contains(x)).ToList();
            foreach (var qti3pPassageId in newQti3PassageIds)
            {
                var itemToPassage = new QTI3pItemToPassage()
                {
                    Qti3pItemId = resource.QtiItemId,
                    Qti3pItemPassageId = qti3pPassageId
                };

                _qTI3pItemToPassageRepository.Save(itemToPassage);
            }

            //delete oldQti3pItemPassages is not in qti3pPassageIDList
            var delQti3PassageIds = oldQti3pItemPassageIds.Where(x => !qti3pPassageIdList.Contains(x)).ToList();
            foreach (var qti3pPassageId in delQti3PassageIds)
            {
                var itemToPassage = oldQti3pItemPassages.FirstOrDefault(x => x.Qti3pItemPassageId == qti3pPassageId);
                _qTI3pItemToPassageRepository.Delete(itemToPassage);

                //save to history
                var trackingPassage = new QTI3pItemDependencyDeleteHistory()
                {
                    QTI3pItemID = resource.QtiItemId,
                    TableName = "QTI3pItemToPassage",
                    DenpendencyEntityValue = qti3pPassageId.ToString()
                };
                _qTI3pItemDpDelHistoryRepository.Save(trackingPassage);
            }

            var oldQTI3pItemStateStandards =
                   _qTI3pItemStateStandardRepository.Select().Where(x => x.Qti3pItemId == resource.QtiItemId);
            var oldQTI3pItemStateStandardIds = oldQTI3pItemStateStandards.Select(x => x.StateStandardId);
            //save to QTI3pItemStateStandard
            var listQti3pItemSSs = new List<int>();
            foreach (var guid in resource.GUIDList)
            {
                var stateStandard = _masterStandardRepository.Select()
                    .FirstOrDefault(x => x.GUID == guid && !x.Archived);
                if (stateStandard != null)
                {
                    listQti3pItemSSs.Add(stateStandard.MasterStandardID);
                }
            }
            var newQti3pSS = listQti3pItemSSs.Where(x => !oldQTI3pItemStateStandardIds.Contains(x));
            foreach (var masterStandardID in newQti3pSS)
            {
                var qti3pStateStandard = new QTI3pItemStateStandard()
                {
                    Qti3pItemId = qtiItemId,
                    StateStandardId = masterStandardID
                };

                _qTI3pItemStateStandardRepository.Save(qti3pStateStandard);
            }

            //delete oldQti3pItemStateStandard not in oldQTI3pItemStateStandards
            var delQti3pSS = oldQTI3pItemStateStandardIds.Where(x => !listQti3pItemSSs.Contains(x));
            foreach (var ssId in delQti3pSS)
            {
                var ss = oldQTI3pItemStateStandards.FirstOrDefault(x => x.StateStandardId == ssId);
                _qTI3pItemStateStandardRepository.Delete(ss);

                //save to history
                var trackingStateStandard = new QTI3pItemDependencyDeleteHistory()
                {
                    QTI3pItemID = resource.QtiItemId,
                    TableName = "QTI3pItemStateStandard",
                    DenpendencyEntityValue = ssId.ToString()
                };
                _qTI3pItemDpDelHistoryRepository.Save(trackingStateStandard);
            }

            //save to QTI3pItemDOK
            if (resource.DOK > 0 || !string.IsNullOrEmpty(resource.DOKCode))
            {
                var dok = _qTi3pDOKRepository.Select().FirstOrDefault(x => x.QTI3pDOKID == resource.DOK || x.Code == resource.DOKCode);
                if (dok != null)
                {
                    var qti3pItemDOK =
                        _qTI3pItemDOKRepository
                            .Select()
                            .FirstOrDefault(x => x.Qti3pItemId == qtiItemId);
                    if (qti3pItemDOK == null)
                    {
                        qti3pItemDOK = new QTI3pItemDOK()
                        {
                            Qti3pItemId = qtiItemId,
                            Qti3pDOK = dok.QTI3pDOKID
                        };
                    }
                    else if (qti3pItemDOK.Qti3pDOK != dok.QTI3pDOKID)
                    {
                        qti3pItemDOK.Qti3pDOK = dok.QTI3pDOKID;

                        //save to history
                        var trackingStateStandard = new QTI3pItemDependencyDeleteHistory()
                        {
                            QTI3pItemID = resource.QtiItemId,
                            TableName = "QTI3pItemDOK",
                            DenpendencyEntityValue = dok.QTI3pDOKID.ToString()
                        };
                        _qTI3pItemDpDelHistoryRepository.Save(trackingStateStandard);
                    }
                    _qTI3pItemDOKRepository.Save(qti3pItemDOK);
                }
            }
        }

        public void SaveDataFileUploadLog(DataFileUploadLog dataFileUploadLog)
        {
            _dataFileUploadLogRepository.Save(dataFileUploadLog);
        }

        public DataFileUploadLog GetDataFileUploadLogById(int dataFileUploadLogId)
        {
            return _dataFileUploadLogRepository
                        .Select().FirstOrDefault(x => x.DataFileUploadLogId == dataFileUploadLogId);
        }

        public void SaveDataFileUploadResourceLog(DataFileUploadResourceLog dataFileUploadResourceLog)
        {
            _dataFileUploadResourceLogRepository.Save(dataFileUploadResourceLog);
        }

        public void ReassignQuestionOrder(int oldQtiGroupId)
        {
            _qtiGroupRepository.ReassignQuestionOrder(oldQtiGroupId);
        }

        public void CreateStateStandardSubjectsForItem3pLibraryFilter(int dataFileUploadLogId)
        {
            _qtiItemQtiItemRepository.CreateStateStandardSubjectsForItem3pLibraryFilter(dataFileUploadLogId);
        }

        public void InsertDataFileUploadResourceLog(bool isUploadTo3pItem, List<DataFileUploaderResource> resources, int dataFileUploadLogId)
        {
            foreach (var resource in resources)
            {
                {
                    var dataFileUploadResourceLog = new DataFileUploadResourceLog()
                    {
                        DataFileUploadLogId = dataFileUploadLogId,
                        ResourceFileName = resource.ResourceFileName,
                        OriginalContent = resource.OriginalContent,
                        InteractionType = resource.InteractionType,
                        ProcessingStep = resource.ProcessingStep.ToString(),
                        IsValidQuestionResourceFile = resource.IsValidQuestionResourceFile,
                        QtiSchemaId = resource.QtiSchemaID,
                        XmlContent = resource.XmlContent,
                        QtiItemId = isUploadTo3pItem ? 0 : resource.QtiItemId,
                        QTI3pItemId = isUploadTo3pItem ? resource.QtiItemId : 0,
                        Error = resource.Error,
                        ErrorDetail = resource.ErrorDetail,
                    };

                    _dataFileUploadResourceLogRepository.Save(dataFileUploadResourceLog);
                }
            }
        }

        #endregion Data Upload

        #region Algorithm

        private void DuplicateAlgorithmQTIItemGrading(int qtiItemId, int newQtiItemId, int userId)
        {
            //TODO: call store clone AlgorithmQTIItemGrading old & new.
            // (store name: DuplicateAlgorithmQTIItemGrading ).
            // DuplicateAlgorithmQTIItemGrading(qtiItemId, newQtiItemId, userId);
            _qtiItemQtiItemRepository.DuplicateAlgorithmQTIItemGrading(qtiItemId, newQtiItemId, userId);
        }

        #endregion Algorithm

        private void DuplicateMultiPartExpression(int oldQtiItemId, int newQtiItemId, int userId)
        {
            _multiPartQtiItemExpressionRepository.DuplicateExpression(oldQtiItemId, newQtiItemId, userId);
        }

        public string ProcessExportItemLibraryByDistrictID(int districtID)
        {
            var listBatch = _qtiItemQtiItemRepository.GetListBatchesQTIItemID(districtID, 50000);

            if (!listBatch.Any()) return String.Empty;

            var result = new List<ItemLibraryExportData>();

            var choiceVariableAnswerScores = new List<ChoiceVariableVirtualQuestionAnswerScore>();

            var complexAnswerScores = new List<ComplexVirtualQuestionAnswerScore>();

            var rubricAnswerScores = new List<RubricQuestionCategoryDto>();
            var catagoriesName = string.Empty;

            foreach (var item in listBatch)
            {
                var data = _qtiItemQtiItemRepository.GetItemLibraryToExportByDistrictID(districtID, item.FromQTIItemID, item.ToQTIItemID);
                catagoriesName = data.CategoriesName;
                result.AddRange(data.DataExport);
            }

            var choiceVariableSchemas = new List<int> {
                (int)QtiSchemaEnum.ChoiceMultipleVariable,
                (int)QtiSchemaEnum.TextEntry,
                (int)QtiSchemaEnum.TextHotSpot,
                (int)QtiSchemaEnum.ImageHotSpot,
                (int)QtiSchemaEnum.TableHotSpot,
                (int)QtiSchemaEnum.NumberLineHotSpot,
                (int)QtiSchemaEnum.Complex
            };
            var choiceVariableQtiItemIDs = result
                .Where(x => choiceVariableSchemas.Contains(x.QTISchemaID))
                .Select(x => x.QTIItemID)
                .ToList();

            var complexQtiItemIDs = result
                .Where(x => x.QTISchemaID == (int)QtiSchemaEnum.Complex)
                .Select(x => x.QTIItemID)
                .ToList();

            var qtiItemAnswerScores = new List<QTIItemAnswerScoreInfoDTO>();
            for (var i = 0; i < choiceVariableQtiItemIDs.Count; i += 2000)
            {
                var ids = choiceVariableQtiItemIDs.Skip(i).Take(2000);
                var scores = _qtiItemAnswerScoreRepository.Select()
                    .Where(x => ids.Contains(x.QTIItemId))
                    .Select(x => new QTIItemAnswerScoreInfoDTO
                    {
                        QTIItemAnswerScoreID = x.QTIItemAnswerScoreId,
                        QTIItemID = x.QTIItemId,
                        Answer = x.Answer,
                        ResponseIdentifier = x.ResponseIdentifier,
                        Score = x.Score
                    })
                    .OrderBy(x => x.QTIItemAnswerScoreID)
                    .ToList();

                foreach (var item in scores)
                {
                    item.ScoreValue = CommonUtils.ConverStringToInt(item.Score, 0);
                }

                qtiItemAnswerScores.AddRange(scores);
            }

            var qtiItemSubs = new List<QTIItemSubInfoDto>();
            for (int i = 0; i < complexQtiItemIDs.Count; i += 2000)
            {
                var ids = complexQtiItemIDs.Skip(i).Take(2000);

                var subs = _qtiItemSubReadOnlyRepository.Select()
                    .Where(x => ids.Contains(x.QTIItemId))
                    .Select(x => new QTIItemSubInfoDto
                    {
                        CorrectAnswer = x.CorrectAnswer,
                        PointsPossible = x.PointsPossible,
                        QTIItemID = x.QTIItemId,
                        QTISchemaID = x.QTISchemaId,
                        ResponseIdentifier = x.ResponseIdentifier,
                        ResponseProcessingTypeID = x.ResponseProcessingTypeId
                    });
                qtiItemSubs.AddRange(subs);
            }

            result.AsParallel().WithDegreeOfParallelism(20).ForAll(item =>
            {
                item.AnswerKey = _exportAnswerKeyService.GetQTIItemAnswerKey(new AnswerKeyData
                {
                    XmlContent = item.XmlContent,
                    ResponseProcessing = item.ResponseProcessing,
                    QTISchemaID = item.QTISchemaID,
                    IsRubricBasedQuestion = false,
                    CorrectAnswer = item.CorrectAnswer,
                    PointsPossible = item.PointsPossible,
                    AlgorithmicExpression = item.AlgorithmicExpression,
                    QTIItemID = item.QTIItemID,
                    QTIItemAnswerScores = qtiItemAnswerScores,
                    QTIItemSubs = qtiItemSubs
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

            return ExportItemLibraryToCSV(result, catagoriesName);
        }

        private string ExportItemLibraryToCSV(IList<ItemLibraryExportData> itemLibraryData,string catagoriesName)
        {
            var ms = new MemoryStream();
            TextWriter textWriter = null;
            CsvWriter csvWriter = null;
            try
            {
                textWriter = new StreamWriter(ms);
                csvWriter = new CsvWriter(textWriter);
                var streamReader = new StreamReader(ms);
                WriteHeaderItemLibrary(csvWriter, catagoriesName);
                foreach (var item in itemLibraryData)
                {
                    WriteGeneralInformationItemLibrary(csvWriter, item);
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

        private void WriteHeaderItemLibrary(CsvWriter csvWriter, string catagoriesName)
        {
            csvWriter.WriteField("QTI_Item_ID");
            csvWriter.WriteField("QTI_Item_Title");           
            csvWriter.WriteField("Item_Type");
            csvWriter.WriteField("Item_Points_Possible");
            csvWriter.WriteField("Standard_Numbers");
            csvWriter.WriteField("Item_Bank_Name");
            csvWriter.WriteField("Item_Set_Name");
            csvWriter.WriteField("Item_Passages");
            csvWriter.WriteField("Related_Tests");
            csvWriter.WriteField("Media_Resources");
            csvWriter.WriteField("Correct_Answer");
            csvWriter.WriteField(catagoriesName);
            csvWriter.NextRecord();
        }

        private void WriteGeneralInformationItemLibrary(CsvWriter csvWriter, ItemLibraryExportData itemLibraryObject)
        {
            csvWriter.WriteField(itemLibraryObject.QTIItemID);
            csvWriter.WriteField(itemLibraryObject.Title ?? string.Empty);           
            csvWriter.WriteField(itemLibraryObject.QTISchemaName ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.PointsPossible);
            csvWriter.WriteField(itemLibraryObject.StandardNumbers ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.QTIBankName ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.ItemSetName ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.ItemPassages ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.RelatedTests ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.MediaResources ?? string.Empty);
            csvWriter.WriteField(itemLibraryObject.AnswerKey);
            csvWriter.WriteField(itemLibraryObject.QTIItemTags ?? string.Empty);
        }

        public string ProcessExportPassageLibraryByDistrictID(int districtID, int userID, int roleID, string upLoadBucketName, string AUVirtualTestROFolder)
        {
            var data = _qtiItemQtiItemRepository.GetPassageLibraryToExportByDistrictID(districtID, userID, roleID);

            if (!data.Any()) return String.Empty;

            var fileNotFound = false;

            data.AsParallel().WithDegreeOfParallelism(20).ForAll(item =>
            {
                try
                {
                    var mediaNames = string.Empty;

                    if (String.IsNullOrEmpty(item.Fullpath))
                    {
                        var xmlContent = PassageUtil.GetS3PassageContent(_s3Service, int.Parse(item.PassageNumber), upLoadBucketName, AUVirtualTestROFolder, out fileNotFound);

                        if (!string.IsNullOrEmpty(xmlContent))
                        {
                            mediaNames += GetMediaNameFromString(xmlContent, $"RO_{item.PassageNumber}_media");
                        }
                    }
                    else
                    {
                        HttpResponseMessage response = _httpClient.GetAsync(item.Fullpath).Result;
                        var passageHtmlContent = response.Content.ReadAsStringAsync().Result;

                        if (!string.IsNullOrEmpty(passageHtmlContent))
                        {
                            mediaNames += GetMediaNameFromString(passageHtmlContent, "images/");
                            mediaNames += GetMediaNameFromString(passageHtmlContent, "videos/");
                        }
                    }
                    if (!String.IsNullOrEmpty(mediaNames))
                    {
                        item.MediaResources = mediaNames.Substring(1);
                    }
                }
                catch (Exception ex)
                {
                }
            });

            return ExportPassageLibraryToCSV(data);
        }

        private string GetMediaNameFromString (string strContent, string folderName)
        {
            var mediaNames = string.Empty;
            var fileName = "";
            var link = "";
            var startPos = 0;
            var nextPos = 0;      
            var slashPos = 0;

            while (strContent.IndexOf(folderName, startPos) > 0)
            {
                startPos = strContent.IndexOf(folderName, startPos);
                nextPos = strContent.IndexOf('"', startPos);
                if (nextPos > 0)
                {
                    link = strContent.Substring(startPos, nextPos - startPos);
                    if (link.LastIndexOf('/') > 0)
                    {
                        slashPos = link.LastIndexOf('/');
                        fileName = HttpUtility.UrlDecode(link.Substring(slashPos + 1, link.Length - slashPos - 1));
                        mediaNames = mediaNames + '|' + '"' + fileName + '"';
                    }
                    startPos = nextPos;
                }
            }
            return mediaNames;
        }

        private string ExportPassageLibraryToCSV(IList<PassageLibraryExportData> passageLibraryData)
        {
            var ms = new MemoryStream();
            TextWriter textWriter = null;
            CsvWriter csvWriter = null;
            try
            {
                textWriter = new StreamWriter(ms);
                csvWriter = new CsvWriter(textWriter);
                var streamReader = new StreamReader(ms);
                WriteHeaderPassageLibrary(csvWriter);
                foreach (var item in passageLibraryData)
                {
                    WriteGeneralInformationPassageLibrary(csvWriter, item);
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

        private void WriteHeaderPassageLibrary(CsvWriter csvWriter)
        {
            csvWriter.WriteField("Passage_Number");
            csvWriter.WriteField("Passage_Name");
            csvWriter.WriteField("Grade");
            csvWriter.WriteField("Subject");
            csvWriter.WriteField("Text_Type");
            csvWriter.WriteField("Text_Sub_Type");
            csvWriter.WriteField("Flesch_Kincaid");
            csvWriter.WriteField("Associated_QTI_Items");
            csvWriter.WriteField("QTI_Count");
            csvWriter.WriteField("Media_Resources");
            csvWriter.NextRecord();
        }

        private void WriteGeneralInformationPassageLibrary(CsvWriter csvWriter, PassageLibraryExportData passageLibraryObject)
        {
            csvWriter.WriteField(passageLibraryObject.PassageNumber);
            csvWriter.WriteField(passageLibraryObject.PassageName);
            csvWriter.WriteField(passageLibraryObject.Grade);
            csvWriter.WriteField(passageLibraryObject.Subject);
            csvWriter.WriteField(passageLibraryObject.TextType);
            csvWriter.WriteField(passageLibraryObject.TextSubType);
            csvWriter.WriteField(passageLibraryObject.FleschKinkaid);
            var associateLength = passageLibraryObject.AssociatedQTIItems?.Length;
            if (associateLength > 32767) // max characters in one cell excel
            {
                var array = passageLibraryObject.AssociatedQTIItems.Split('|');
                if (array.Any())
                {
                    var resultString = "";
                    for (int i = 0; i < array.Length; i++)
                    {
                        resultString += array[i];
                        if (resultString.Length >= 32767)
                        {
                            resultString = resultString.Remove(resultString.LastIndexOf("|"));
                            resultString = resultString.Remove(resultString.LastIndexOf("|"));
                            csvWriter.WriteField(resultString);
                            i--;
                            resultString = array[i] + "|";
                        }
                        else
                            resultString += "|";
                    }
                    csvWriter.WriteField(resultString.Remove(resultString.Length - 1));
                }
            }
            else
            {
                csvWriter.WriteField(passageLibraryObject.AssociatedQTIItems);
            }
            csvWriter.WriteField(passageLibraryObject.QTICount);
            csvWriter.WriteField(passageLibraryObject.MediaResources);
        }

        public List<PassageItemFromItemLibrary> GetQtiItemsByFiltersPassageFromItemLibrary(QtiItemFilters filter, int? userId, int districtId,
            int startIndex, int pageSize, string sortColumns,
            string searchColumns, string searchInboxXML)
        {
            return _qtiItemQtiItemRepository.GetQtiItemsByFiltersPassageFromItemLibrary(filter, userId, districtId, startIndex,
                                                                pageSize, sortColumns, searchColumns, searchInboxXML);
        }
    }
}
