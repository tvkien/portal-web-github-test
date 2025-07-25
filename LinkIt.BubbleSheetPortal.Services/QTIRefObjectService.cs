using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.QTIRefObject;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QTIRefObjectService
    {
        private readonly IRepository<QtiRefObject> _repository;
        private readonly IQTIRefObjectRepository _qtiRefObjectRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IRepository<QTI3pPassage> _qTI3pPassageRepository;

        public QTIRefObjectService(
            IRepository<QtiRefObject> repository,
            IQTIRefObjectRepository qtiRefObjectRepository,
            IGradeRepository gradeRepository,
            IRepository<QTI3pPassage> qTI3pPassageRepository)
        {
            _repository = repository;
            _qtiRefObjectRepository = qtiRefObjectRepository;
            _gradeRepository = gradeRepository;
            _qTI3pPassageRepository = qTI3pPassageRepository;
        }

        public IQueryable<QtiRefObject> GetAll()
        {
            return _repository.Select();
        }

        public IEnumerable<QtiRefObject> GetQtiRefObjects(IEnumerable<int> qTIRefObjectIds)
        {
            return _qtiRefObjectRepository.GetAllQtiRefObjects().Where(x => qTIRefObjectIds.Contains(x.QTIRefObjectID)).ToList();
        }

        public QtiRefObject GetById(int qtiRefObjectId)
        {
            return _qtiRefObjectRepository.GetAllQtiRefObjects().FirstOrDefault(en => en.QTIRefObjectID == qtiRefObjectId);
        }

        public IQueryable<QtiRefObject> GetQtiRefObject(GetQtiRefObjectFilter filter)
        {
            if (string.IsNullOrEmpty(filter.Subject))
                filter.Subject = null;
            if (string.IsNullOrEmpty(filter.Name))
                filter.Name = null;

            return _qtiRefObjectRepository.GetQtiRefObject(filter).ToList().AsQueryable();
        }

        public List<ListItemStr> GetPassageSubjects(bool isIncludeQti3p = false)
        {
            var result = new List<ListItemStr>();

            var data = _repository.Select().GroupBy(a => a.Subject) //distinct
                   .Select(g => g.First())
                   .Select(x => x.Subject)
                   .OrderBy(x => x)
                   .ToList();

            if (isIncludeQti3p)
            {
                var qti3pSubjects = _qTI3pPassageRepository.Select()
                    .Select(x => x.Subject)
                    .Distinct()
                    .ToList();
                data = data.Concat(qti3pSubjects).Distinct().OrderBy(x => x).ToList();
            }
            
            foreach (var subject in data)
            {
                if (!string.IsNullOrEmpty(subject))
                {
                    var item = new ListItemStr();
                    item.Id = subject.Replace(" ", "").ToLower();
                    item.Name = subject;
                    result.Add(item);
                }
            }

            return result;
        }

        public void Save(QtiRefObject item)
        {
            _repository.Save(item);
        }

        public void Delete(int qtiRefObjectId)
        {
            _repository.Delete(new QtiRefObject { QTIRefObjectID = qtiRefObjectId });
        }

        public List<Grade> GetAssignedGradesForPassages(bool isIncludeQti3p = false)
        {
            List<int?> gradeIdList = _repository.Select().Select(x => x.GradeID).Distinct().ToList();

            if(isIncludeQti3p)
            {
                var qti3pGradeIds = _qTI3pPassageRepository.Select()
                    .Where(x => x.GradeID > 0)
                    .Select(x => x.GradeID)
                    .Distinct()
                    .ToList();
                gradeIdList = gradeIdList.Concat(qti3pGradeIds).Distinct().ToList();
            }

            var assignedGrades = _gradeRepository.Select().Where(x => gradeIdList.Contains(x.Id)).ToList();
            return assignedGrades.OrderBy(x => x.Order).ToList();
        }

        public List<ListItem> GetQtiRefObjectNumber()
        {
            var temp = _qtiRefObjectRepository.GetAllQtiRefObjects().OrderBy(x => x.QTIRefObjectFileRef);
            var data = temp.Select(o => new ListItem()
            {
                Id = o.QTIRefObjectID,
                Name = o.QTIRefObjectFileRef.ToString()
            }).ToList();
            return data;
        }

        public bool HasRightToEdit(User currentUser, int qtiRefObjectId)
        {
            try
            {
                if (currentUser.IsPublisher)
                {
                    return true;
                }
                return _qtiRefObjectRepository.GetHasRightToEditPassage(currentUser.Id, currentUser.DistrictId.GetValueOrDefault(), qtiRefObjectId);
            }
            catch
            {
                return false;
            }
        }

        public int CountVirtualTestByRefObjectId(int qtiRefObjectId, int virtualTestId)
        {
            var results = _qtiRefObjectRepository.GetQtiRefObjectVirtualTests(qtiRefObjectId)
                .Select(o => o.VirtualTestId)
                .Distinct()
                .Where(x => x != virtualTestId)
                .Count();

            return results;
        }
    }
}
