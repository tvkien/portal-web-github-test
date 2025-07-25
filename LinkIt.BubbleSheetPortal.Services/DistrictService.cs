using System;
using System.Linq;
using System.Security.Cryptography;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictService
    {
        private readonly IReadOnlyRepository<District> repository;
        private readonly IRepository<AuthorGroupDistrict> authorGroupDistrictRepository;
        private readonly IReadOnlyRepository<DistrictMeta> districtMetaRepository;
        private readonly IReadOnlyRepository<Configuration> configurationRepository;


        public DistrictService(
            IReadOnlyRepository<District> repository,
            IRepository<AuthorGroupDistrict> authorGroupDistrictRepository,
            IReadOnlyRepository<DistrictMeta> districtMetaRepository,
            IReadOnlyRepository<Configuration> configurationRepository
            )
        {
            this.repository = repository;
            this.authorGroupDistrictRepository = authorGroupDistrictRepository;
            this.districtMetaRepository = districtMetaRepository;
            this.configurationRepository = configurationRepository;
        }

        public IQueryable<District> GetDistricts()
        {
            return repository.Select();
        }

        public IQueryable<District> GetDistrictsByStateId(int stateId)
        {
            return repository.Select().Where(x => x.StateId.Equals(stateId) && x.DistrictGroupId != 54);
        }

        public District GetDistrictById(int districtId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(districtId));
        }

        public IQueryable<District> GetDistrictsUserHasAccessTo(int districtId)
        {
            return repository.Select().Where(x => x.Id.Equals(districtId));
        }

        public int GetLiCodeBySubDomain(string subDomain)
        {
            var district = repository.Select().FirstOrDefault(x => x.LICode.Equals(subDomain));
            return district.IsNull() ? 0 : district.Id;
        }

        public List<District> GetDistrictsByKeyWords(DistrictSearchingObject searchObj)
        {
            if (searchObj.IsNull())
            {
                searchObj = new DistrictSearchingObject 
                {
                    KeyWords = string.Empty,
                    PageIndex = 0,
                    PageSize = 10,
                    TotalRecords = 0
                };
            }            
            if (!string.IsNullOrEmpty(searchObj.KeyWords))
            {
                IQueryable<District> matchDistricts = repository.Select().Where(x => x.Name.Contains(searchObj.KeyWords));
                searchObj.TotalRecords = matchDistricts.Count();
                return matchDistricts.OrderBy(m => m.Name).Skip(searchObj.PageIndex * searchObj.PageSize).Take(searchObj.PageSize).ToList();
            }
            searchObj.TotalRecords = 0;
            return new List<District>();            
        }

        public IQueryable<District> GetDistrictsByAuthorGroupId(int authorGroupId)
        {
            var districtIds =
                authorGroupDistrictRepository.Select()
                    .Where(x => x.AuthorGroupId == authorGroupId)
                    .Select(x => x.DistrictId)
                    .Distinct()
                    .ToList();

            var query = repository.Select().Where(x => districtIds.Contains(x.Id));
            return query;
        }
        public IQueryable<District> FilterDistricByIds(List<int> districtIds)
        {
            return repository.Select().Where(x => districtIds.Contains(x.Id));
        }

        /// <summary>
        /// Get list state id from list dictricid
        /// </summary>
        /// <param name="districtIds"></param>
        /// <returns></returns>
        public List<int> GetStateIdByDictricIds(List<int> districtIds)
        {
            //Get state
            var states = from a in repository.Select()
                         where districtIds.Contains(a.Id)
                         group a by a.StateId
                             into g
                             select new
                             {
                                 StateId = g.Key
                             };
            //return list id type int
            return states.Select(a => a.StateId).Distinct().ToList();
        }

        public District GetDistrictByCode(string code)
        {
            return repository.Select().FirstOrDefault(x => x.LICode.Equals(code));
        }

        public bool ShowWidgets(int districtId)
        {
            var widgetsOption = configurationRepository.Select().FirstOrDefault(x => x.Name == ContaintUtil.SHOW_WIDGETS_OPTION);
            if (widgetsOption == null)
            {
                return false;
            }

            var districtTypes = configurationRepository.Select().FirstOrDefault(x => x.Name == ContaintUtil.ENABLE_HOME_PAGE_WIDGETS_FOR);
            if (districtTypes == null || string.IsNullOrEmpty(districtTypes.Value))
            {
                return false;
            }

            switch (widgetsOption.Value)
            {
                case ContaintUtil.SHOW_WIDGETS_ALL:
                    return true;

                case ContaintUtil.SHOW_WIDGETS_SPECIFIED:
                    var districtTypeList = districtTypes.Value.Split('|');
                    return districtMetaRepository.Select().Any(x => x.DistrictID == districtId && x.Name == ContaintUtil.DISTRICT_TYPE && districtTypeList.Contains(x.Data));

                case ContaintUtil.SHOW_WIDGETS_NONE:
                    return false;

                default:
                    return false;
            }
        }
    }
}
