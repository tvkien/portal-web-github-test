using System.Collections.Generic;
using System;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class StateService
    {
        private readonly IStateRepository repository;

        public StateService(IStateRepository repository)
        {
            this.repository = repository;
        }

        public IQueryable<State> GetStates()
        {
            return repository.Select().Where(x => x.Name != string.Empty).OrderBy(x => x.Name);
        }

        public State GetStateByZipCode(int zipCode)
        {
            return repository.Select().ToList().FirstOrDefault(x => IsZipCodeValid(x.ZipRange, zipCode));
        }

        private bool IsZipCodeValid(string zipRange, int zipCode)
        {
            if(!string.IsNullOrEmpty(zipRange))
            {
                var ranges = zipRange.Split(';');
                foreach (var range in ranges)
                {
                    var minRange = Convert.ToInt32(range.Split('-')[0]);
                    var maxRange = Convert.ToInt32(range.Split('-')[1]);

                    if (zipCode >= minRange && zipCode <= maxRange)
                        return true;
                }
            }

            return false;
        }

        public State GetStateById(int? stateId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(stateId));
        }
        public List<State> GetStateForUser(int userId, int districtId, bool useStateOfDistrictSchool = false, bool useStateSchool = false)
        {
            return repository.GetStateForUser(userId, districtId, useStateOfDistrictSchool, useStateSchool);
        }
        public List<int> GetStateIdForUser(int userId, int districtId, bool useStateOfDistrictSchool = false, bool useStateSchool = false)
        {
            return repository.GetStateForUser(userId, districtId, useStateOfDistrictSchool, useStateSchool).Select(x => x.Id).ToList();
        }

        /// <summary>
        /// Get states by list id
        /// </summary>
        /// <param name="stateIds"></param>
        /// <returns></returns>
        public IQueryable<State> GetStatesByIds(List<int> stateIds)
        {
            var states = repository.Select().Where(x => stateIds.Contains(x.Id));
            return states;
        }

        public string GetTimeZoneId(int stateId)
        {
            return repository.GetTimeZoneId(stateId);
        }

    }
}
