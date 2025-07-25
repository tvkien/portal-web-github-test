using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTIItemRepository : IQTIItemRepository
    {
        private readonly Table<QTIItemEntity> table;
        private readonly Table<QTIItemView> view;
        private readonly AssessmentDataContext _assessmentDataContext;

        public QTIItemRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            var dataContext = DbDataContext.Get(connectionString);
            table = dataContext.GetTable<QTIItemEntity>();
            view = dataContext.GetTable<QTIItemView>();

            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<QTIItemData> Select()
        {
            return view.Select(x => new QTIItemData
            {
                AnswerIdentifiers = x.AnswerIdentifiers,
                CorrectAnswer = x.CorrectAnswer,
                FilePath = x.FilePath,
                InteractionCount = x.InteractionCount,
                OldMasterCode = x.OldMasterCode,
                ParentID = x.ParentID,
                PointsPossible = x.PointsPossible,
                QTIGroupID = x.QTIGroupID,
                QTIItemID = x.QTIItemID,
                QTISchemaID = x.QTISchemaID,
                QuestionOrder = x.QuestionOrder,
                ResponseIdentifier = x.ResponseIdentifier,
                ResponseProcessing = x.ResponseProcessing,
                ResponseProcessingTypeID = x.ResponseProcessingTypeID,
                SourceID = x.SourceID,
                Title = x.Title,
                Updated = x.Updated,
                UrlPath = x.UrlPath,
                UserID = x.UserID,
                XmlContent = x.XmlContent,
                //VirtualQuestionCount = 0,
                Tests = x.Tests,
                StandardNumberList = x.StandardNumberList,
                TopicList = x.TopicList,
                SkillList = x.SkillList,
                OtherList = x.OtherList,
                ItemTagList = x.ItemTagList,
                DataFileUploadTypeId = x.DataFileUploadTypeID,
                QtiItemIdSource = x.QtiItemIdSource,
                CreatedDate = x.CreatedDate,
                UpdatedByUserID = x.UpdatedByUserID,
                RevertedFromQTIItemHistoryID = x.RevertedFromQTIItemHistoryID,
                Description = x.Description
            });
        }

        public void Save(QTIItemData item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemID.Equals(item.QTIItemID));

            if (entity.IsNull())
            {
                entity = new QTIItemEntity();
                table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            table.Context.SubmitChanges();
            item.QTIItemID = entity.QTIItemID;
        }

        public void Delete(QTIItemData item)
        {
            var entity = table.FirstOrDefault(x => x.QTIItemID.Equals(item.QTIItemID));

            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public string Delete(int qtiItemId, int userId)
        {
            var obj = _assessmentDataContext.DeleteQTIItem(qtiItemId, userId).FirstOrDefault();
            if (obj != null)
                return obj.returnMessage;
            else
            {
                return string.Empty;
            }
        }

        private void MapModelToEntity(QTIItemEntity entity, QTIItemData item)
        {
            entity.AnswerIdentifiers = item.AnswerIdentifiers;
            entity.CorrectAnswer = item.CorrectAnswer;
            entity.FilePath = item.FilePath;
            entity.InteractionCount = item.InteractionCount;
            entity.OldMasterCode = item.OldMasterCode;
            entity.ParentID = item.ParentID;
            entity.PointsPossible = item.PointsPossible;
            entity.QTIGroupID = item.QTIGroupID;
            entity.QTIItemID = item.QTIItemID;
            entity.QTISchemaID = item.QTISchemaID;
            entity.QuestionOrder = item.QuestionOrder;
            entity.ResponseIdentifier = item.ResponseIdentifier;
            entity.ResponseProcessing = item.ResponseProcessing;
            entity.ResponseProcessingTypeID = item.ResponseProcessingTypeID;
            entity.SourceID = item.SourceID;
            entity.Title = item.Title;
            entity.Updated = item.Updated;
            entity.UrlPath = item.UrlPath;
            entity.UserID = item.UserID;
            entity.XmlContent = item.XmlContent;
            entity.DataFileUploadTypeID = item.DataFileUploadTypeId;
            entity.QtiItemIdSource = item.QtiItemIdSource;
            entity.CreatedDate = item.CreatedDate;
            entity.UpdatedByUserID = item.UpdatedByUserID;
            entity.RevertedFromQTIItemHistoryID = item.RevertedFromQTIItemHistoryID;
            entity.Description = item.Description;
        }

        public void TMCopyStandardsFromQTIItem(int virtualQuestionID, int qtiItemID, int stateID)
        {
            _assessmentDataContext.TMCopyStandardsFromQTIItem(virtualQuestionID, qtiItemID, stateID);
        }

        public void CopyConditionalLogicsFromQTIItemToNewVirtualQuestion(int virtualQuestionID, int qtiItemID)
        {
            _assessmentDataContext.CopyConditionalLogicsFromQTIItemToNewVirtualQuestion(virtualQuestionID, qtiItemID);
        }

        public void AddListQtiItemToVirtualSection(int virtualTestId, int virtualSectionId, string qtiItemIds, int? questionGroupId)
        {
            _assessmentDataContext.AddListQtiItemToVirtualSection(virtualTestId, virtualSectionId, qtiItemIds, questionGroupId);
        }

        public string DuplicateListQTIItem(int userId, int? qtiGroupId, string qtiItemIds)
        {
            var returnQtiItemIds = "";
            _assessmentDataContext.DuplicateListQTIItem(userId, qtiGroupId, qtiItemIds, ref returnQtiItemIds);
            return returnQtiItemIds;
        }

        public List<QTI3pItem> GetQti3PItemsByQtibankId(int qtibankId)
        {
            return _assessmentDataContext.GetQTIITemsByQTIBankId(qtibankId).Select(o => new QTI3pItem()
            {
                QTI3pItemID = o.QTIItemID,
                XmlContent = o.XmlContent
            }).ToList();
        }

        public void TMAddQtiItemRelatedInfoFromLibrary(int qtiItemId, int qti3pItemId)
        {
            _assessmentDataContext.TMAddQtiItemRelatedInfoFromLibrary(qtiItemId, qti3pItemId);
        }

        public List<QtiItem> GetQtiItemsByFilter(QtiItemFilters filter, int? userId, int districtId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInboxXML)
        {
            bool? personal = null;

            if (!string.IsNullOrEmpty(filter.Personal))
            {
                personal = filter.Personal.Equals("true");
            }

            return
                _assessmentDataContext.GetQtiItemsByFilters(filter.ForCheckingRight, filter.QtiItemIdString,
                    filter.FindResultWith.HasValue ? filter.FindResultWith.Value : 1,
                    filter.Keyword, filter.QtiBankId, filter.ItemSetId, filter.Standard,
                    filter.Topic, filter.Skill, filter.Other, filter.SelectedTags, filter.DistrictCategoryId,
                    personal, userId, districtId,
                    filter.PassageId, filter.PassageNumber, filter.TextTypeId, filter.TextSubTypeId,
                    filter.WordCountID, filter.FleschKincaidId, filter.PassageGradeId, filter.PassageSubject,
                    startIndex, pageSize, sortColumns, searchColumns, searchInboxXML, filter.ItemTypeId, filter.TitleSpecial ,filter.ItemTitle, filter.ItemDescription, filter.sSearch, filter.SelectedItemIds).Select(o => new QtiItem()
                    {
                        QtiItemId = o.QTIItemID,
                        XmlContent = o.XmlContent,
                        TotalRow = o.TotalRow,
                        GroupName = o.GroupName,
                        BankName = o.BankName,
                        Topic = o.Topic,
                        Skill = o.Skill,
                        Other = o.Other,
                        Standard = o.Standard,
                        DistrictTag = o.DistrictTag,
                        Title = o.Title,
                        Description = o.Description,
                        QTIBankID = o.QTIBankID,
                        QTIGroupID = o.QTIGroupID
                    }).ToList();
        }

        public void UpdateQtiItemVirtualQuestion(int qtiItemId, int pointsPossible, int? resetRubric = 0)
        {
            _assessmentDataContext.UpdateQtiItemVirtualQuestion(qtiItemId, pointsPossible, resetRubric);
        }

        public void DeleteQtiItemAnswerScoreAndVirtualQuestionAnswerScore(int qtiItemId)
        {
            _assessmentDataContext.DeleteQtiItemAnswerScoreAndVirtualQuestionAnswerScore(qtiItemId);
        }

        public bool CheckAccessQTI3p(int userId, int districtId, Qti3pLicensesEnum qti3pLicensesEnum)
        {
            return _assessmentDataContext.fnCheckAccessQTI3p(userId, districtId, (int)qti3pLicensesEnum) ?? false;
        }

        public bool CheckShowQtiItem(int userId, int virtualQuestionId, int districtId)
        {
            var result = _assessmentDataContext.CheckShowQtiItem(userId, virtualQuestionId, districtId);
            return result.Any(x => x.Total > 0);
        }

        public List<PassageItem> GetQtiItemsByFiltersPassage(QtiItemFilters filter, int? userId, int districtId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInboxXML)
        {
            return
                _assessmentDataContext.GetQtiItemsByFiltersPassage(
                    filter.FindResultWith ?? 1,
                    filter.Keyword, filter.QtiBankId, filter.ItemSetId, filter.Standard,
                    filter.Topic, filter.Skill, filter.Other, filter.SelectedTags, 0,
                    filter.IsPersonalSearch, filter.IsDistrictSearch, userId, districtId,
                    filter.PassageId, filter.PassageNumber, filter.TextTypeId, filter.TextSubTypeId,
                    filter.WordCountID, filter.FleschKincaidId, filter.PassageGradeId, filter.PassageSubject,
                    startIndex, pageSize, sortColumns, searchColumns, searchInboxXML, filter.ItemTypeId, filter.SelectedItemIds).Select(o => new PassageItem()
                    {
                        QTIRefObjectID = o.QTIRefObjectID,
                        Source = o.Source,
                        Number = o.Number,
                        Name = o.NAME,
                        Subject = o.Subject,
                        GradeName = o.GradeName,
                        TextType = o.TextType,
                        TextSubType = o.TextSubType,
                        FleschKinkaidName = o.FleschKinkaidName,
                        ItemsMatchCount = o.ItemsMatchCount,
                        ItemsMatchXml = o.ItemsMatchXml,
                        ItemsAllCount = o.ItemsAllCount,
                        ItemsAllXml = o.ItemsAllXml,
                        TotalRow = o.TotalRow,
                        HasQTI3pPassage = o.HasQTI3pPassage,
                        CreatedBy = o.UserID,
                        DistrictID = o.DistrictID,
                        CanEdit = o.CanEdit
                    }).ToList();
        }

        public void UpdateItemPassage(int qtiItemId, List<int> qtiRefObjectIds, List<int> qti3PPassageNumbers)
        {
            var qtiRefObjectIdsString = string.Empty;
            var qti3PPassageNumbersString = string.Empty;
            if (qtiRefObjectIds != null)
            {
                qtiRefObjectIdsString = string.Join(",", qtiRefObjectIds);
            }
            if (qti3PPassageNumbers != null)
            {
                qti3PPassageNumbersString = string.Join(",", qti3PPassageNumbers);
            }
            _assessmentDataContext.UpdateItemPassage(qtiItemId, qtiRefObjectIdsString, qti3PPassageNumbersString);
        }

        public void CreateStateStandardSubjectsForItem3pLibraryFilter(int dataFileUploadLogId)
        {
            _assessmentDataContext.CreateStateStandardSubjectsForItem3pLibraryFilter(dataFileUploadLogId);
        }

        public void DuplicateAlgorithmQTIItemGrading(int qtiItemId, int newQtiItemId, int userId)
        {
            _assessmentDataContext.DuplicateAlgorithmQTIItemGrading(qtiItemId, newQtiItemId, userId);
        }

        public bool IsHavingStudentTakeTest(int? qtiitemId)
        {
            var result = _assessmentDataContext.IsHavingStudentTakeTest(qtiitemId).FirstOrDefault();
            return result != null && (result.IsHavingStudentTakeTest ?? false);
        }

        public bool IsHavingAnswer(int? qtiitemId, int? qtiitemSubId)
        {
            var result = _assessmentDataContext.IsHavingAnswer(qtiitemId, qtiitemSubId).FirstOrDefault();
            return result != null && (result.IsHavingAnswer ?? false);
        }

        public IList<int> GetRefObjectIdsByQtiItemIds(string qtiItemIds)
        {
            return _assessmentDataContext
                .GetRefObjectIdsByQtiItemIds(qtiItemIds)
                .Select(p => (int)p.QtiRefObjectId)
                .ToList();
        }

        public IList<ListBatchQTIItemID> GetListBatchesQTIItemID(int districtID, int batchSize)
        {
            return _assessmentDataContext.GetListBatchesQTIItemID(districtID, batchSize)
                            .Select(o => new ListBatchQTIItemID
                            {
                                FromQTIItemID = o.FromQTIItemID,
                                ToQTIItemID = o.ToQTIItemID
                            }).ToList();
        }

        public (string CategoriesName,IList<ItemLibraryExportData> DataExport) GetItemLibraryToExportByDistrictID(int districtID, int fromQTIItemID, int toQTIItemID)
        {
            var itemLibraryData = _assessmentDataContext.GetItemLibraryToExportByDistrictID(districtID, fromQTIItemID, toQTIItemID);
            var categoriesName = itemLibraryData.GetResult<string>().First();
            var itemLibraryExportData = itemLibraryData.GetResult<GetItemLibraryToExportResult>().ToList();
            var exportData = itemLibraryExportData.Select(o => new ItemLibraryExportData
             {
                 QTIItemID = o.QTIItemID,
                 Title = o.Title,
                 QTIItemTags = o.QTIItemTags,
                 QTISchemaName = o.QTISchemaName,
                 ItemSetName = o.ItemSetName,
                 PointsPossible = o.PointsPossible,
                 QTIBankName = o.QTIBankName,
                 RelatedTests = o.RelatedTests,
                 StandardNumbers = o.StandardNumbers,
                 ItemPassages = o.ItemPassages,
                 MediaResources = o.MediaResources,
                 CorrectAnswer = o.CorrectAnswer,
                 QTISchemaID = o.QTISchemaID,
                 XmlContent = o.XmlContent,
                 ResponseProcessing = o.ResponseProcessing,
                 AlgorithmicExpression = o.AlgorithmicExpression
             }).ToList();
            return (categoriesName, exportData);            
        }

        public IList<PassageLibraryExportData> GetPassageLibraryToExportByDistrictID(int districtID, int userID, int roleID)
        {
            return _assessmentDataContext.GetPassageLibraryToExportByDistrictID(districtID, userID, roleID)
                            .Select(o => new PassageLibraryExportData
                            {
                                PassageNumber = o.PassageNumber,
                                PassageName = o.PassageName,
                                Grade = o.Grade,
                                Subject = o.Subject,
                                TextType = o.TextType,
                                TextSubType = o.TextSubType,
                                FleschKinkaid = o.FleschKinkaid,
                                AssociatedQTIItems = o.AssociatedQTIItems,
                                QTICount = o.QTICount,
                                Fullpath = o.Fullpath
                            }).ToList();
        }


        public List<PassageItemFromItemLibrary> GetQtiItemsByFiltersPassageFromItemLibrary(QtiItemFilters filter, int? userId, int districtId,
                                                  int startIndex, int pageSize, string sortColumns,
                                                  string searchColumns, string searchInboxXML)
        {
            return
                _assessmentDataContext.GetQtiItemsByFiltersPassageFromItemLibrary(
                    filter.SelectedItemIds,
                    filter.Keyword, filter.QtiBankId, filter.ItemSetId, filter.Standard,
                    filter.Topic, filter.Skill, filter.Other, filter.SelectedTags, 0,
                    filter.IsPersonalSearch, filter.IsDistrictSearch, userId, districtId,
                    filter.PassageId, filter.PassageNumber, filter.TextTypeId, filter.TextSubTypeId,
                    filter.WordCountID, filter.FleschKincaidId, filter.PassageGradeId, filter.PassageSubject,
                    startIndex, pageSize, sortColumns, searchColumns, searchInboxXML, filter.ItemTypeId).Select(o => new PassageItemFromItemLibrary()
                    {
                        QTIRefObjectID = o.QTIRefObjectID,
                        Source = o.Source,
                        Number = o.Number,
                        Name = o.NAME,
                        Subject = o.Subject,
                        GradeName = o.GradeName,
                        TextType = o.TextType,
                        TextSubType = o.TextSubType,
                        FleschKinkaidName = o.FleschKinkaidName,
                        ItemsAllCount = o.ItemsMatchCount,
                        ItemsAllXml = o.ItemsMatchXml,
                        TotalRow = o.TotalRow,
                        HasQTI3pPassage = o.HasQTI3pPassage
                    }).ToList();
        }


    }
}
