using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOStudentFilterService 
    {
        private readonly IRepository<SGOStudentFilter> _repository;

        public SGOStudentFilterService(IRepository<SGOStudentFilter> repository)
        {
            _repository = repository;
        }

        public void Save(SGOStudentFilter obj)
        {
            if(obj !=null)
                _repository.Save(obj);
        }

        public List<int> GetGenderIdsBySGOId(int sgoId)
        {
            return
                _repository.Select()
                    .Where(o => o.SGOID == sgoId && o.FilterType == (int) SGOStudentFilterType.Gender)
                    .Select(o => o.FilterID)
                    .ToList();
        }
        public List<int> GetRaceIdsBySGOId(int sgoId)
        {
            return
                _repository.Select()
                    .Where(o => o.SGOID == sgoId && o.FilterType == (int)SGOStudentFilterType.Race)
                    .Select(o => o.FilterID)
                    .ToList();
        }
        public List<int> GetProgramIdsBySGOId(int sgoId)
        {
            return
                _repository.Select()
                    .Where(o => o.SGOID == sgoId && o.FilterType == (int)SGOStudentFilterType.Program)
                    .Select(o => o.FilterID)
                    .ToList();
        }
        public int GetSelectedStateBySGOId(int sgoId)
        {
            var query =
                _repository.Select()
                    .FirstOrDefault(o => o.SGOID == sgoId && o.FilterType == (int) SGOStudentFilterType.State);
            if (query != null)
            {
                return query.FilterID;
            }
            return 0;
        }
        public int GetSelectedDistrictBySGOId(int sgoId)
        {
            var query =
                _repository.Select()
                    .FirstOrDefault(o => o.SGOID == sgoId && o.FilterType == (int)SGOStudentFilterType.District);
            if (query != null)
            {
                return query.FilterID;
            }
            return 0;
        }

        public List<SGOStudentFilter> GetListSGOStudentFilterBySGOID(int sgoId)
        {
            return _repository.Select().Where(o => o.SGOID == sgoId).ToList();
        }

        public void SaveDistrictIdAndStateIdBySGOId(int sgoId, int stateId, int districtId)
        {
            if (districtId > 0 && !_repository.Select().Any(o =>
                o.SGOID == sgoId && o.FilterType == (int) SGOStudentFilterType.District &&
                o.FilterID == districtId))
            {
                _repository.Save(new SGOStudentFilter()
                {
                    FilterID = districtId,
                    FilterType = (int)SGOStudentFilterType.District,
                    SGOID = sgoId
                });
            }
            if (stateId > 0 && !_repository.Select().Any(o =>
               o.SGOID == sgoId && o.FilterType == (int)SGOStudentFilterType.State &&
               o.FilterID == stateId))
            {
                _repository.Save(new SGOStudentFilter()
                {
                    FilterID = stateId,
                    FilterType = (int)SGOStudentFilterType.State,
                    SGOID = sgoId
                });
            }
        }

        public List<SGOStudentFilter> GetListTermSelectedBySGOID(int sgoId)
        {
            return _repository.Select().Where(o => o.SGOID == sgoId && o.FilterType == (int)SGOStudentFilterType.DistrictTerm).ToList();
        }
        public List<SGOStudentFilter> GetListClassSelectedBySGOID(int sgoId)
        {
            return _repository.Select().Where(o => o.SGOID == sgoId && o.FilterType == (int)SGOStudentFilterType.Class).ToList();
        }
    }
}
