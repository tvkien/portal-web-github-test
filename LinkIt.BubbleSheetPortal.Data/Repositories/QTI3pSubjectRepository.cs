using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class QTI3pSubjectRepository : IQTI3pSubjectRepository
    {
        private readonly Table<QTI3pSubjectEntity> table;
        private readonly AssessmentDataContext _context;

        public QTI3pSubjectRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<QTI3pSubjectEntity>();
            _context = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<QTI3pSubject> Select()
        {
            return table.Select(x => new QTI3pSubject
                                {
                                    SubjectID = x.SubjectID,
                                    Name = x.Name
                                });
        }

        public List<StateStandardSubject> GetSubjectByStateCode(string stateCode)
        {
            return _context.QTIGetETSStateStandardSubjectsByStateAndArchivedStatus(stateCode).Select(o => new StateStandardSubject()
            {
                StateCode = o.State,
                StateId = o.StateID ?? 0,
                SubjectName = o.Subject,
                Year = o.Year
            }).ToList();
        }

        public List<StateSubjectGrade> GetGradeByStateCodeAndSubject(string stateCode, string subject, int qti3pSourceId)
        {
            return _context.QTIGetETSStateStandardGradesByStateAndSubject(stateCode, subject, qti3pSourceId).Select(o => new StateSubjectGrade()
            {
                GradeID = o.GradeID ?? 0,
                GradeName = o.Grade,
                GradeOrder = o.GradeOrder ?? 0,
                StateCode = o.State,
                SubjectName = o.Subject,

            }).ToList();
        }

        public List<QTI3pItem> GetQti3ItemByFilters(QTI3pItemFilters obj, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs)
        {
            if (obj != null)
            {
                if (obj.Searchkey == null) obj.Searchkey = string.Empty;
                if (obj.Difficulty == null) obj.Difficulty = string.Empty;
                if (obj.Blooms == null) obj.Blooms = string.Empty;
                if (obj.Subject == null) obj.Subject = string.Empty;
                if (obj.PassageNumber == null || obj.PassageNumber == "null" || obj.PassageNumber.ToLower().Equals("select")) obj.PassageNumber = string.Empty;
                if (obj.PassageSubject == null || obj.PassageSubject == "null" || obj.PassageSubject == "0") obj.PassageSubject = string.Empty;

                //TODO: revert when go Staging GetListQTI3PItemsByFilters
                return _context.GetListQTI3PItemsByFilters(
                    obj.FindResultWith.HasValue ? obj.FindResultWith.Value : 1,
                    obj.Searchkey,
                    obj.Qti3pSourceId,
                    obj.DifficultyId,
                    obj.Difficulty,
                    obj.BloomsID,
                    obj.Blooms,
                    obj.GradeID,
                    obj.SubjectID,
                    obj.Subject,
                    obj.StateStandardIdString,
                    obj.QTI3pDOKID,
                    listDistrictIDs,
                    obj.PassageId,
                    obj.PassageNumber,
                    obj.TextTypeId,
                    obj.TextSubTypeId,
                    obj.WordCountID,
                    obj.FleschKincaidId,
                    obj.PassageGradeId,
                    obj.PassageSubject,
                    obj.PassageTypeId,
                    obj.PassageGenreId,
                    startIndex,
                    pageSize,
                    sortColumns,
                    searchColumns,
                    searchInBox,
                    obj.IsRestricted,
                    obj.ItemTypeId,
                    obj.QTI3pItemLanguage,
                    obj.QTI3pPassageLanguage,
                    obj.ItemTitle,
                    obj.ItemDescription,
                    obj.sSearch,
                    obj.SelectedItemIds)
                    .Select(o => new QTI3pItem()
                    {
                        QTI3pItemID = o.QTI3pItemID,
                        XmlContent = o.XmlContent,
                        TotalRow = o.TotalRow,
                        Subject = o.Subject,
                        Difficulty = o.Difficulty,
                        BloomsTaxonomy = o.BloomsTaxonomy,
                        GradeName = o.GradeName,
                        Standard = o.Standard,
                        PassageNumber = o.PassageNumber,
                        PassageGrade = o.PassageGrade,
                        PassageSubject = o.PassageSubject,
                        PassageWordCount = o.PassageWordCount,
                        PassageTextType = o.PassageTextType,
                        PassageTextSubType = o.PassageTextSubType,
                        PassageFleschKinkaid = o.PassageFleschKinkaid,
                        Qti3pItemDOK = o.Qti3pItemDOK,
                        UrlPath = o.UrlPath,
                        From3pUpload = o.From3pUpload ?? false,
                        Title = o.Title,
                        Description = o.Description
                    }).ToList();
            }
            return new List<QTI3pItem>();
        }

        public List<CriteriaSchema> GetCriteriaSchemasQtiItem(int UserID, int DistrictID, int? ItemBankID, int? QtiGroupID,bool isGetByPersonal, bool isGetByDistrict)
        {
            return _context.GetCriteriaSchemasQtiItem(UserID, DistrictID, ItemBankID, QtiGroupID,isGetByPersonal, isGetByDistrict)
                    .Select(x => new CriteriaSchema()
                    {
                        QTISchemaID = x.QTISchemaID,
                        QTISchemaName = x.QTISchemaName
                    }).ToList();
        }

        public List<CriteriaSchema> GetCriteriaSchemasQti3pItem(string strDistrictIDs)
        {
            return _context.GetCriteriaSchemasQti3pItem(strDistrictIDs)
                    .Select(x => new CriteriaSchema()
                    {
                        QTISchemaID = x.QTISchemaID,
                        QTISchemaName = x.QTISchemaName
                    }).ToList();
        }

        public List<MasterStandard> GetGradeByStateCodeAndSubjectAndGrade(string stateCode, string subject, string grade)
        {
           return _context.QTIGetETSStateStandardsByStateAndSubjectAndGradesNew(stateCode, subject, grade)
                .Select(o => new MasterStandard()
                {
                    MasterStandardID = o.MasterStandardID ?? 0,
                    State = o.State,
                    Document = o.Document,
                    Subject = o.Subject,
                    Year = o.Year ?? 0,
                    Grade = o.Grade,
                    Level = o.Level ?? 1,
                    Children = o.Children ?? 0,
                    Label = o.Label,
                    Number = o.Number,
                    Title = o.Title,
                    Description = o.Description,
                    GUID = o.GUID,
                    ParentGUID = o.ParentGUID,
                    LoGrade = o.LoGrade,
                    HiGrade = o.HiGrade,
                    LowGradeID = o.LowGradeID,
                    HighGradeID = o.HighGradeID,
                    StateId = o.StateID,
                    Archived = o.Archived ?? false
                }).ToList();
        }
        public List<StateStandardSubject> GetStateStandardSubjectsForItem3pLibraryFilter(string stateCode, int? qti3pSourceId)
        {
            return _context.GetStateStandardSubjectsForItem3pLibraryFilter(stateCode, qti3pSourceId).Select(o => new StateStandardSubject()
            {
                StateCode = o.State,
                StateId = o.StateID ?? 0,
                SubjectName = o.Subject,
                Year = o.Year
            }).ToList();
        }

        public List<PassageItem3p> GetQti3pItemsByFiltersPassage(QTI3pItemFilters obj, int userId, int startIndex, int pageSize,
            string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs)
        {
            if (obj != null)
            {
                if (obj.Searchkey == null) obj.Searchkey = string.Empty;
                if (obj.Difficulty == null) obj.Difficulty = string.Empty;
                if (obj.Blooms == null) obj.Blooms = string.Empty;
                if (obj.Subject == null) obj.Subject = string.Empty;
                if (obj.PassageNumber == null || obj.PassageNumber == "null" || obj.PassageNumber.ToLower().Equals("select")) obj.PassageNumber = string.Empty;
                if (obj.PassageSubject == null || obj.PassageSubject == "null" || obj.PassageSubject == "0") obj.PassageSubject = string.Empty;
                if (obj.PassageTitle == null) obj.PassageTitle = string.Empty;

                //GetQti3pItemsByFiltersPassage
                return _context.GetQti3pItemsByFiltersPassage(obj.FindResultWith.HasValue ? obj.FindResultWith.Value : 1,obj.Qti3pSourceId, obj.Searchkey,
                    obj.DifficultyId, obj.Difficulty, obj.BloomsID, obj.Blooms, obj.GradeID, obj.SubjectID,
                    obj.Subject, obj.StateStandardIdString, obj.QTI3pDOKID,userId, listDistrictIDs, obj.PassageId, obj.PassageNumber,
                    obj.TextTypeId, obj.TextSubTypeId, obj.WordCountID, obj.FleschKincaidId, obj.PassageGradeId,
                    obj.PassageSubject,obj.PassageTypeId, obj.PassageGenreId, startIndex, pageSize, sortColumns, searchColumns, searchInBox, obj.IsRestricted, obj.ItemTypeId, obj.QTI3pItemLanguage, obj.QTI3pPassageLanguage, obj.Lexilemin, obj.Lexilemax, obj.SelectedItemIds)
                    .Select(o => new PassageItem3p()
                    {
                        QTI3pPassageID = o.QTI3pPassageID,
                        Source = o.Source,
                        Number = o.Number,
                        Name = o.NAME,
                        Subject = o.Subject,
                        GradeID = o.GradeID,
                        GradeName = o.GradeName,
                        TextType = o.TextType,
                        WordCound = o.WordCount,
                        TextSubType = o.TextSubType,
                        FleschKinkaidName = o.FleschKinkaidName,
                        PassageType = o.PassageType,
                        PassageGenre = o.PassageGenre,
                        Lexile = o.Lexile,
                        Spache = o.Spache,
                        DaleChall = o.DaleChall,
                        RMM = o.RMM,
                        ItemsMatchCount = o.ItemsMatchCount,
                        ItemsMatchXml = o.ItemsMatchXml,
                        ItemsAllCount = o.ItemsAllCount,
                        ItemsAllXml = o.ItemsAllXml,
                        TotalRow = o.TotalRow

                    }).ToList();
            }
            return new List<PassageItem3p>();
        }

        public List<QTI3pDOK> GetQti3pDOK(int qti3pSourceId)
        {
            return _context.GetQti3pDOK(qti3pSourceId).Select(x=> new QTI3pDOK()
            {
                QTI3pDOKID = x.QTI3pDOKID,
                Code = x.Code,
                Name = x.Name
            }).ToList();
        }

        public List<ListItemExtra> GetAllPassageForCertica()
        {
            var listQT3pPassage = _context.GetAllPassageTitleAndNumberForCertica()                
                .Select(o => new ListItemExtra()
                {
                    Id = o.QTI3pPassageID,
                    Name = o.PassageTitle,
                    ExtraField = o.Number
                }).ToList();
            if (listQT3pPassage != null && listQT3pPassage.Count > 0)
            {
                return listQT3pPassage.OrderBy(o => o.Name).ToList();
            }
            return new List<ListItemExtra>();
        }
         
        public List<PassageItem3pFromItemLibrary> GetQti3pItemsByFiltersPassageFromItemLibrary(QTI3pItemFilters obj, int userId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs)
        {
            if (obj != null)
            {
                if (obj.Searchkey == null) obj.Searchkey = string.Empty;
                if (obj.Difficulty == null) obj.Difficulty = string.Empty;
                if (obj.Blooms == null) obj.Blooms = string.Empty;
                if (obj.Subject == null) obj.Subject = string.Empty;
                if (obj.PassageNumber == null || obj.PassageNumber == "null" || obj.PassageNumber.ToLower().Equals("select")) obj.PassageNumber = string.Empty;
                if (obj.PassageSubject == null || obj.PassageSubject == "null" || obj.PassageSubject == "0") obj.PassageSubject = string.Empty;
                if (obj.PassageTitle == null) obj.PassageTitle = string.Empty;

                //GetQti3pItemsByFiltersPassage
                return _context.GetQti3pItemsByFiltersPassageFromItemLibrary(obj.SelectedItemIds, obj.Qti3pSourceId, obj.Searchkey,
                    obj.DifficultyId, obj.Difficulty, obj.BloomsID, obj.Blooms, obj.GradeID, obj.SubjectID,
                    obj.Subject, obj.StateStandardIdString, obj.QTI3pDOKID, userId, listDistrictIDs, obj.PassageId, obj.PassageNumber,
                    obj.TextTypeId, obj.TextSubTypeId, obj.WordCountID, obj.FleschKincaidId, obj.PassageGradeId,
                    obj.PassageSubject, obj.PassageTypeId, obj.PassageGenreId, startIndex, pageSize, sortColumns, searchColumns, searchInBox, obj.IsRestricted, obj.ItemTypeId, obj.QTI3pItemLanguage, obj.QTI3pPassageLanguage, obj.Lexilemin, obj.Lexilemax)
                    .Select(o => new PassageItem3pFromItemLibrary()
                    {
                        QTI3pPassageID = o.QTI3pPassageID,
                        Source = o.Source,
                        Number = o.Number,
                        Name = o.NAME,
                        Subject = o.Subject,
                        GradeID = o.GradeID,
                        GradeName = o.GradeName,
                        TextType = o.TextType,
                        WordCound = o.WordCount,
                        TextSubType = o.TextSubType,
                        FleschKinkaidName = o.FleschKinkaidName,
                        PassageType = o.PassageType,
                        PassageGenre = o.PassageGenre,
                        Lexile = o.Lexile,
                        Spache = o.Spache,
                        DaleChall = o.DaleChall,
                        RMM = o.RMM,
                        ItemsAllCount = o.ItemsMatchCount,
                        ItemsAllXml = o.ItemsMatchXml,
                        TotalRow = o.TotalRow

                    }).ToList();
            }

            return new List<PassageItem3pFromItemLibrary>();
        }
    }
}
