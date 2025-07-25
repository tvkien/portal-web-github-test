using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTIItemRepository : IReadOnlyRepository<QTIItemData>
    {
        IQueryable<QTIItemData> Select();

        void Save(QTIItemData item);

        void Delete(QTIItemData item);

        string Delete(int qtiItemId, int userId);

        void TMCopyStandardsFromQTIItem(int virtualQuestionID, int qtiItemID, int stateID);

        void TMAddQtiItemRelatedInfoFromLibrary(int qtiItemID, int qti3pItemID);

        List<QTI3pItem> GetQti3PItemsByQtibankId(int qtibankId);

        List<QtiItem> GetQtiItemsByFilter(QtiItemFilters filter, int? userId, int districtId,
                                                 int startIndex, int pageSize, string sortColumns,
                                                 string searchColumns, string searchInboxXML);

        void UpdateQtiItemVirtualQuestion(int qtiItemId, int pointsPossible, int? resetRubric = 0);

        void DeleteQtiItemAnswerScoreAndVirtualQuestionAnswerScore(int qtiItemId);

        void AddListQtiItemToVirtualSection(int virtualTestId, int virtualSectionId, string qtiItemIds, int? questionGroupId);

        string DuplicateListQTIItem(int userId, int? qtiGroupId, string qtiItemIds);

        bool CheckAccessQTI3p(int userId, int districtId, Qti3pLicensesEnum qti3pLicensesEnum);

        bool CheckShowQtiItem(int userId, int virtualQuestionId, int districtId);

        List<PassageItem> GetQtiItemsByFiltersPassage(QtiItemFilters filter, int? userId, int districtId,
                                                  int startIndex, int pageSize, string sortColumns,
                                                  string searchColumns, string searchInboxXML);

        void UpdateItemPassage(int qtiItemId, List<int> qtiRefObjectIds, List<int> qti3PPassageNumbers);

        void CreateStateStandardSubjectsForItem3pLibraryFilter(int dataFileUploadLogId);

        void DuplicateAlgorithmQTIItemGrading(int qtiItemId, int newQtiItemId, int userId);

        bool IsHavingStudentTakeTest(int? qtiitemId);

        bool IsHavingAnswer(int? qtiitemId, int? qtiitemSubId);

        void CopyConditionalLogicsFromQTIItemToNewVirtualQuestion(int virtualQuestionID, int qtiItemID);

        IList<int> GetRefObjectIdsByQtiItemIds(string qtiItemIds);

        (string CategoriesName, IList<ItemLibraryExportData> DataExport) GetItemLibraryToExportByDistrictID(int districtID, int fromQTIItemID, int toQTIItemID);

        IList<PassageLibraryExportData> GetPassageLibraryToExportByDistrictID(int districtID, int userID, int roleID);

        IList<ListBatchQTIItemID> GetListBatchesQTIItemID(int districtID, int batchSize);

        List<PassageItemFromItemLibrary> GetQtiItemsByFiltersPassageFromItemLibrary(QtiItemFilters filter, int? userId, int districtId,
                                                  int startIndex, int pageSize, string sortColumns,
                                                  string searchColumns, string searchInboxXML);
    }
}
