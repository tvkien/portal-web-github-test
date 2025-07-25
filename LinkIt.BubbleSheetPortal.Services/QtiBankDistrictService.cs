using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class QtiBankDistrictService
    {
        private readonly IRepository<QtiBankDistrict> _repository;
        private readonly IReadOnlyRepository<District> _districtRepository;

        public QtiBankDistrictService(IRepository<QtiBankDistrict> repository,  IReadOnlyRepository<District> districtRepository)
        {
            _repository = repository;
            _districtRepository = districtRepository;
        }

        public QtiBankDistrict GetById(int qtiBankDistrictId)
        {
            return _repository.Select().FirstOrDefault(x => x.QtiBankDistrictId == qtiBankDistrictId);
        }
        
        public void Delete(QtiBankDistrict qtiBankDistrict)
        {
            _repository.Delete(qtiBankDistrict);
        }

        public IQueryable<District> GetUnPublishedDistrict(int stateId, int qtiBankId)
        {
            var districts = _districtRepository.Select().Where(x => x.StateId.Equals(stateId)).ToList();
            var publishedDistricts = _repository.Select().Where(x => x.QtiBankId.Equals(qtiBankId)).ToList();

            var unpublishedDistrictIds = districts.Select(x => x.Id).Except(publishedDistricts.Select(x => x.DistrictId));

            return districts.Where(x => unpublishedDistrictIds.Contains(x.Id)).AsQueryable();
        }

        public IQueryable<QtiBankDistrict> Select()
        {
            return _repository.Select();
        }

        public void Save(QtiBankDistrict qtiBankDistrict)
        {
            _repository.Save(qtiBankDistrict);
        }
    }
}
