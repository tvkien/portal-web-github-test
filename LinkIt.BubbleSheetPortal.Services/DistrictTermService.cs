using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DistrictTermService
    {
        private readonly IReadOnlyRepository<DistrictTerm> repository;
        private readonly IRepository<DistrictTerm> insertUpdaterepository;

        public DistrictTermService(IReadOnlyRepository<DistrictTerm> repository, IRepository<DistrictTerm> insertUpdaterepository)
        {
            this.repository = repository;
            this.insertUpdaterepository = insertUpdaterepository;
        }

        public IQueryable<DistrictTerm> GetDistrictTermByDistrictID(int districtId)
        {
            return repository.Select().Where(d => d.DistrictID == districtId && (d.DateEnd >= DateTime.Now || d.DateEnd == null));
        }

        public IQueryable<DistrictTerm> GetAllTermsByDistrictID(int districtId)
        {
            return repository.Select().Where(d => d.DistrictID == districtId);
        }

        public DistrictTerm GetDistrictTermByNameAndDate(int districtId, DateTime startDate, DateTime endDate, string districtTermName)
        {
            return
                repository.Select().FirstOrDefault(
                    x =>
                    x.DistrictID == districtId && x.DateStart == startDate && x.DateEnd == endDate &&
                    x.Name.Equals(districtTermName));
        }


        public DistrictTerm GetDistrictTermById(int termId)
        {
            return repository.Select().FirstOrDefault(x => x.DistrictTermID.Equals(termId));
        }

        public void Save(DistrictTerm term)
        {
            insertUpdaterepository.Save(term);
        }

        public List<DistrictTerm> GetSchoolByNames(IEnumerable<string> lstDistrictTermName, int districtId)
        {
            var query = repository.Select().Where(o => o.DistrictID == districtId && lstDistrictTermName.Contains(o.Name));
            return query.ToList();
        }

        public IQueryable<DistrictTerm> Select()
        {
            return repository.Select();
        }

        public int GetSurveyDistrictTermId(int districtId, string strName)
        {
            var objDistrictTerm = repository.Select().FirstOrDefault(x => x.DistrictID == districtId && x.Name == strName );
            if (objDistrictTerm != null)
                return objDistrictTerm.DistrictTermID;

            return 0;
        }
    }
}
