using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class RaceService
    {
        private readonly IRepository<Race> repository;

        public RaceService(IRepository<Race> repository)
        {
            this.repository = repository;
        }

        public IQueryable<Race> GetRacesByDistrictID(int districtId)
        {
            return repository.Select().Where(r => r.DistrictID.Equals(districtId)).OrderBy(r => r.Name).ThenBy(r => r.Code).ThenBy(r => r.AltCode);
        }

        public IQueryable<Race> GetRacesByRaceList(List<int> raceIds)
        {
            return repository.Select().Where(r => raceIds.Contains(r.Id)).OrderBy(r => r.Name).ThenBy(r => r.Code).ThenBy(r => r.AltCode);
        }

        public void AddRace(Race race)
        {
            repository.Save(race);
        }

        public int GetRaceForCreateStudent(int raceId, int districtId)
        {
            if (raceId > 0)
                return raceId;
            var vRace = repository.Select().FirstOrDefault(o => o.DistrictID == districtId && (o.Code.Equals("U") || o.Name.Equals("Unknown")));
            if (vRace != null)
                return vRace.Id;

            var unknowRace = new Race
            {
                Code = "U",
                DistrictID = districtId,
                Name = "Unknown"
            };
            repository.Save(unknowRace);
            return unknowRace.Id;
        }
    }
}