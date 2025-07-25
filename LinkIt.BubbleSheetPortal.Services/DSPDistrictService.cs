using System.Linq;
using System.Security.Cryptography;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DSPDistrictService
    {
       
        private readonly IDspDistrictRepository _repository;

        public DSPDistrictService(IDspDistrictRepository repository)
        {

            this._repository = repository;
           
        }
        public IQueryable<DSPDistrict> GetAll()
        {
            return _repository.Select();
        }
        public List<int> GetMemberDistrictId(int organizationDistrictId)
        {
            return _repository.Select().Where(x=>x.OrganizationDistrictID==organizationDistrictId).Select(x=>x.MemberDistrictID).ToList();
        }
        public bool HasMemberDistrict(int organizationDistrictId)
        {
            return _repository.Select().Any(x => x.OrganizationDistrictID == organizationDistrictId);
        }
        public List<int> GetDistrictsByUserId(int userId)
        {
            var ids = _repository.GetDistricIdbyNetWorkAdmin(userId);
            return ids; 
        }
        public IQueryable<District> GetDistrictMembers(int organizationDistrictId)
        {
            return this._repository.GetDistrictMembers(organizationDistrictId);
        }

        public IQueryable<State> GetStateByDistrictNetWorkAdmin(int organizationId)
        {
            return _repository.GetStateByDistrictNetWorkAdmin(organizationId);
        }
    }
}