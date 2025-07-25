using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common.DataFileUpload;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTI3Service
    {
        private readonly IQTI3pSubjectRepository _subjectRepository;
        private readonly IReadOnlyRepository<QTI3pPassage> _passageRepository;
        private readonly IReadOnlyRepository<QTI3pFleschKinkaid> _fleschKinkaidRepository;
        private readonly IReadOnlyRepository<QTI3pTextType> _textTypeRepository;
        private readonly IReadOnlyRepository<QTI3pWordCount> _wordCountRepository;
        private readonly IReadOnlyRepository<QTI3pDifficulty> _difficultyRepository;
        private readonly IReadOnlyRepository<QTI3pBlooms> _bloomsRepository;

        private readonly IReadOnlyRepository<CriteriaGrade> _criteriaGradeRepository;
        private readonly IReadOnlyRepository<PassageGrade> _passageGradeRepository;

        private readonly IReadOnlyRepository<QTI3pDOK> _dokRepository;
        private readonly IReadOnlyRepository<QTI3pItem> _qti3PItemRepository;
        private readonly IRepository<Qti3pProgressPassageType> _qti3pProgressPassageTypeRepository;
        private readonly IRepository<Qti3pProgressPassageGenre> _qti3pProgressPassageGenreRepository;
        private readonly IReadOnlyRepository<QTI3PSubjectGradeLicenses> _qti3PSubjectLicensesRepository;

        public QTI3Service(IQTI3pSubjectRepository qTI3pSubject, IReadOnlyRepository<QTI3pPassage> passageRepository,
            IReadOnlyRepository<QTI3pFleschKinkaid> fleschKinkaidRepository, IReadOnlyRepository<QTI3pTextType> textTypeRepository,
            IReadOnlyRepository<QTI3pWordCount> wordCountRepository, IReadOnlyRepository<QTI3pDifficulty> difficultyRepository,
            IReadOnlyRepository<CriteriaGrade> criteriaGradeRepository, IReadOnlyRepository<PassageGrade> passageGradeRepository,
            IReadOnlyRepository<QTI3pBlooms> bloomsRepository, IReadOnlyRepository<QTI3pDOK> dokRepository, IReadOnlyRepository<QTI3pItem> qti3PItemRepository,
            IRepository<Qti3pProgressPassageType> qti3pProgressPassageTypeRepository,
            IRepository<Qti3pProgressPassageGenre> qti3pProgressPassageGenreRepository,
            IReadOnlyRepository<QTI3PSubjectGradeLicenses> qti3PSubjectLicensesRepository)
        {
            _subjectRepository = qTI3pSubject;
            _passageRepository = passageRepository;
            _fleschKinkaidRepository = fleschKinkaidRepository;
            _textTypeRepository = textTypeRepository;
            _wordCountRepository = wordCountRepository;
            _difficultyRepository = difficultyRepository;
            _bloomsRepository = bloomsRepository;
            _criteriaGradeRepository = criteriaGradeRepository;
            _passageGradeRepository = passageGradeRepository;
            _dokRepository = dokRepository;
            _qti3PItemRepository = qti3PItemRepository;
            _qti3pProgressPassageTypeRepository = qti3pProgressPassageTypeRepository;
            _qti3pProgressPassageGenreRepository = qti3pProgressPassageGenreRepository;
            _qti3PSubjectLicensesRepository = qti3PSubjectLicensesRepository;
        }

        public List<QTI3pSubject> GetQti3PSubjects()
        {
            return _subjectRepository.Select().ToList();
        }

        public List<QTI3pPassage> GetQti3Passage()
        {
            return _passageRepository.Select().OrderBy(o => o.Number).ToList();
        }

        public List<QTI3pFleschKinkaid> GetFleschKinkaids()
        {
            return _fleschKinkaidRepository.Select().ToList();
        }

        public List<QTI3pTextType> GetQti3PTextTypes(int typeId)
        {
            return _textTypeRepository.Select().Where(o => o.TypeID == typeId).OrderBy(o => o.Name).ToList();
        }

        public List<QTI3pWordCount> GetQti3PWordCounts()
        {
            return _wordCountRepository.Select().ToList();
        }

        public List<QTI3pDifficulty> GetQti3pDifficulties()
        {
            return _difficultyRepository.Select().ToList();
        }

        public List<QTI3pBlooms> GetQti3pBloomses()
        {
            return _bloomsRepository.Select().ToList();
        }

        public List<string> GetPassageSubject()
        {
            return _passageRepository.Select().Where(x => x.Subject != "" && x.Qti3pSourceID == (int)QTI3pSourceEnum.Mastery).Select(o => o.Subject).Distinct().ToList();
        }

        public List<StateStandardSubject> GetStandardSubjects(string stateCode)
        {
            return _subjectRepository.GetSubjectByStateCode(stateCode);
        }

        public List<StateSubjectGrade> GetGradeByStateCodeAndSubject(string stateCode, string subject, int qti3pSourceId)
        {
            return _subjectRepository.GetGradeByStateCodeAndSubject(stateCode, subject, qti3pSourceId);
        }

        public List<QTI3pItem> GetQti3PItemsByFilter(QTI3pItemFilters objFilters, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs)
        {
            return _subjectRepository.GetQti3ItemByFilters(objFilters, startIndex, pageSize, sortColumns, searchColumns, searchInBox, listDistrictIDs);
        }

        public List<MasterStandard> GetQti3PMasterStandardByStateSubjectAndGrade(string stateCode, string subject, string grade)
        {
            return _subjectRepository.GetGradeByStateCodeAndSubjectAndGrade(stateCode, subject, grade);
        }

        public List<CriteriaGrade> GetCriteriaGrades()
        {
            return _criteriaGradeRepository.Select().OrderBy(o => o.Order).ToList();
        }

        public List<CriteriaSchema> GetCriteriaSchemasQtiItem(int UserID, int DistrictID, int? ItemBankID, int? QtiGroupID, bool isGetByPersonal, bool isGetByDistrict)
        {
            return _subjectRepository.GetCriteriaSchemasQtiItem(UserID, DistrictID, ItemBankID, QtiGroupID, isGetByPersonal, isGetByDistrict);
        }

        public List<CriteriaSchema> GetCriteriaSchemaQti3pItem(string strDistrictIDs)
        {
            return _subjectRepository.GetCriteriaSchemasQti3pItem(strDistrictIDs);
        }

        public List<PassageGrade> GetPassageGrades()
        {
            return _passageGradeRepository.Select().OrderBy(o => o.Order).ToList();
        }

        public List<StateStandardSubject> GetStateStandardSubjectsForItem3pLibraryFilter(string stateCode, int? qti3pSourceId)
        {
            return _subjectRepository.GetStateStandardSubjectsForItem3pLibraryFilter(stateCode, qti3pSourceId);
        }

        public List<QTI3pDOK> GetQti3pDOKs(int qti3pSourceId)
        {
            //return _dokRepository.Select().ToList();
            return _subjectRepository.GetQti3pDOK(qti3pSourceId);
        }

        public QTI3pPassage GetQti3PassageByName(string passageName)
        {
            return _passageRepository.Select().Where(x => x.PassageName.ToLower().Equals(passageName.ToLower())).FirstOrDefault();
        }

        public List<PassageItem3p> GetQti3pItemsByFiltersPassage(QTI3pItemFilters objFilters, int userId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs)
        {
            return _subjectRepository.GetQti3pItemsByFiltersPassage(objFilters, userId, startIndex, pageSize, sortColumns, searchColumns, searchInBox, listDistrictIDs);
        }

        public IQueryable<QTI3pItem> GetQti3pItems(List<int> qti3pItemIdList)
        {
            return _qti3PItemRepository.Select().Where(x => qti3pItemIdList.Contains(x.QTI3pItemID));
        }

        public QTI3pItem GetQti3pItem(int qti3pItemID)
        {
            return _qti3PItemRepository.Select().FirstOrDefault(x => x.QTI3pItemID == qti3pItemID);
        }

        public List<Qti3pProgressPassageType> GetQti3pProgressPassageType()
        {
            return _qti3pProgressPassageTypeRepository.Select().ToList();
        }

        public List<Qti3pProgressPassageGenre> GetQti3pProgressPassageGenre()
        {
            return _qti3pProgressPassageGenreRepository.Select().ToList();
        }

        public IQueryable<ListItem> GetQTI3PPassageTitle(int qti3pSourceID)
        {
            if (qti3pSourceID == (int)QTI3pSourceEnum.Mastery)
            {
                const string SPANISH_PREFIX = "SP_";
                var listQTI3pPassage = _subjectRepository.GetAllPassageForCertica();
                var listReturn = listQTI3pPassage.Where(o => !string.IsNullOrEmpty(o.Name) && !o.ExtraField.StartsWith(SPANISH_PREFIX))
                    .Select(o => new ListItem()
                    {
                        Id = o.Id,
                        Name = o.Name
                    }).ToList();
                var listSpanishReturn = listQTI3pPassage.Where(o => !string.IsNullOrEmpty(o.Name) && o.ExtraField.StartsWith(SPANISH_PREFIX))
                    .Select(o => new ListItem()
                    {
                        Id = o.Id,
                        Name = string.Concat(o.Name, " (Spanish)")
                    }).ToList();
                listReturn.AddRange(listSpanishReturn);
                return listReturn.OrderBy(o => o.Name).AsQueryable();
            }

            return _passageRepository.Select().Where(x => x.Qti3pSourceID == qti3pSourceID && x.PassageTitle != null).OrderBy(x => x.PassageTitle).ThenBy(x => x.Number).Select(o => new ListItem()
            {
                Id = o.QTI3pPassageID,
                Name = GetPassageTitleWithLanguage(o)
            });
        }

        private string GetPassageTitleWithLanguage(QTI3pPassage qTI3PPassage)
        {
            const string SPANISH_PREFIX = "SP_";

            if (qTI3PPassage.Number.Contains(SPANISH_PREFIX))
                return string.Concat(qTI3PPassage.PassageTitle, " (Spanish)");

            return qTI3PPassage.PassageTitle;
        }

        public IQueryable<ListItem> GetQTI3PPassageNumber(int qti3pSourceID)
        {
            return _passageRepository.Select().Where(x => x.Qti3pSourceID == qti3pSourceID).OrderBy(x => x.Number).Select(o => new ListItem()
            {
                Id = o.QTI3pPassageID,
                Name = o.Number
            });
        }

        public List<QTI3PSubjectGradeLicenses> GetSubjectGradeLicenses(List<int> districtIds)
        {
            return _qti3PSubjectLicensesRepository.Select().Where(x => districtIds.Contains(x.DistrictID) && x.Status == 1).ToList();
        }

        public List<QTI3PSubjectGradeLicenses> GetSubjectLicenses(List<int> districtIds)
        {
            return _qti3PSubjectLicensesRepository.Select().Where(x => districtIds.Contains(x.DistrictID) && x.Status == 1 && !x.GradeID.HasValue).ToList();
        }

        public List<QTI3PSubjectGradeLicenses> GetGradetLicenses(List<int> districtIds, string subject = null)
        {
            return _qti3PSubjectLicensesRepository.Select().Where(x => districtIds.Contains(x.DistrictID) && x.Status == 1 && x.GradeID.HasValue && (x.Subject == null || x.Subject == "" || x.Subject == subject)).ToList();
        }

        public List<PassageItem3pFromItemLibrary> GetQti3pItemsByFiltersPassageFromItemLibrary(QTI3pItemFilters objFilters, int userId, int startIndex, int pageSize, string sortColumns, string searchColumns, string searchInBox, string listDistrictIDs)
        {
            return _subjectRepository.GetQti3pItemsByFiltersPassageFromItemLibrary(objFilters, userId, startIndex, pageSize, sortColumns, searchColumns, searchInBox, listDistrictIDs);
        }


    }
}
