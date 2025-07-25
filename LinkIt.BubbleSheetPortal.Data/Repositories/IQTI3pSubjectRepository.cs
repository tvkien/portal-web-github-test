using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IQTI3pSubjectRepository : IReadOnlyRepository<QTI3pSubject>
    {
        List<StateStandardSubject> GetSubjectByStateCode(string stateCode);
        List<StateSubjectGrade> GetGradeByStateCodeAndSubject(string stateCode, string subject, int qti3pSourceId);
        List<MasterStandard> GetGradeByStateCodeAndSubjectAndGrade(string stateCode, string subject, string grade);
        List<QTI3pItem> GetQti3ItemByFilters(QTI3pItemFilters obj, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs);
        List<StateStandardSubject> GetStateStandardSubjectsForItem3pLibraryFilter(string stateCode, int? qti3pSourceId);
        List<PassageItem3p> GetQti3pItemsByFiltersPassage(QTI3pItemFilters obj, int userId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs);
        List<QTI3pDOK> GetQti3pDOK(int qti3pSourceId);
        List<CriteriaSchema> GetCriteriaSchemasQtiItem(int UserID, int DistrictID, int? ItemBankID, int? QtiGroupID, bool isGetByPersonal, bool isGetByDistrict);
        List<CriteriaSchema> GetCriteriaSchemasQti3pItem(string strDistrictIDs);

        List<ListItemExtra> GetAllPassageForCertica();

        List<PassageItem3pFromItemLibrary> GetQti3pItemsByFiltersPassageFromItemLibrary(QTI3pItemFilters obj, int userId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs);
    }
}
