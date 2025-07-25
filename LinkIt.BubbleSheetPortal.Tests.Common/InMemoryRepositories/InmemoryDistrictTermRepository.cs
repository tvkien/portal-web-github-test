using System;
using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryDistrictTermRepository : IReadOnlyRepository<DistrictTerm>, IRepository<DistrictTerm>
    {
        private readonly List<DistrictTerm> table;

        public InMemoryDistrictTermRepository()
        {
            table = AddDistrictTerms();
        }

        private List<DistrictTerm> AddDistrictTerms()
        {
            return new List<DistrictTerm>()
            {
                new DistrictTerm{ DistrictTermID = 1, Name = "District Term 1", DistrictID = 1, DateStart= new DateTime(2012, 6, 4), DateEnd = new DateTime(2012, 6, 8), Active = true, Code = "123", CreatedByUserID = 1, UpdatedByUserID = 1, DateCreated = DateTime.Now, DateUpdated = DateTime.Now },
                new DistrictTerm{ DistrictTermID = 2, Name = "District Term 2", DistrictID = 1, DateStart= new DateTime(2012, 5, 4), DateEnd = new DateTime(2012, 5, 8), Active = true, Code = "123", CreatedByUserID = 1, UpdatedByUserID = 1, DateCreated = DateTime.Now, DateUpdated = DateTime.Now },
                new DistrictTerm{ DistrictTermID = 3, Name = "District Term 3", DistrictID = 1, DateStart= new DateTime(2012, 6, 4), DateEnd = new DateTime(2012, 6, 8), Active = true, Code = "123", CreatedByUserID = 1, UpdatedByUserID = 1, DateCreated = DateTime.Now, DateUpdated = DateTime.Now },
                new DistrictTerm{ DistrictTermID = 4, Name = "District Term 4", DistrictID = 2, DateStart= new DateTime(2012, 6, 4), DateEnd = new DateTime(2012, 6, 8), Active = true, Code = "123", CreatedByUserID = 1, UpdatedByUserID = 1, DateCreated = DateTime.Now, DateUpdated = DateTime.Now },
                new DistrictTerm{ DistrictTermID = 5, Name = "District Term 5", DistrictID = 3, DateStart= new DateTime(2012, 6, 4), DateEnd = new DateTime(2012, 6, 8), Active = true, Code = "123", CreatedByUserID = 1, UpdatedByUserID = 1, DateCreated = DateTime.Now, DateUpdated = DateTime.Now }
            };
        }

        public IQueryable<DistrictTerm> Select()
        {
            return table.AsQueryable();
        }

        public void Delete(DistrictTerm item)
        {
            throw new NotImplementedException();
        }

        public void Save(DistrictTerm item)
        {
            throw new NotImplementedException();
        }
    }
}
