using System;
using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryDistrictTermClassRepository : IReadOnlyRepository<DistrictTermClass>
    {
        private readonly List<DistrictTermClass> table;

        public InMemoryDistrictTermClassRepository()
        {
            table = AddDistrictTermClass();
        }

        private List<DistrictTermClass> AddDistrictTermClass()
        {
            DateTime day = DateTime.Now;
            return new List<DistrictTermClass> 
            {
                new DistrictTermClass{ DistrictId = 1, DateStart = day.AddYears(1), DateEnd = day.AddYears(2), ClassId=1,DistrictTermId=1,SchoolId=123,UserId=16,TermId=1,TeacherId=1},
                new DistrictTermClass{ DistrictId = 2, DateStart = day.AddYears(-2), DateEnd = day.AddYears(-1),DistrictTermId=2, SchoolId=2, UserId=2,TermId=2, TeacherId=2},
                new DistrictTermClass{ DistrictId = 1, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=3, SchoolId=123, UserId=15,TermId=3, TeacherId=3},
                new DistrictTermClass{ DistrictId = 4, DateStart = day.AddYears(-2), DateEnd = day.AddYears(-1),DistrictTermId=4, SchoolId=4, UserId=4,TermId=4, TeacherId=4},
                new DistrictTermClass{ DistrictId = 5, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=5, SchoolId=5, UserId=5,TermId=5, TeacherId=5},
                new DistrictTermClass{ DistrictId = 6, DateStart = day.AddYears(-2), DateEnd = day.AddYears(-1),DistrictTermId=6, SchoolId=6, UserId=6,TermId=6, TeacherId=6},
                new DistrictTermClass{ DistrictId = 7, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=7, SchoolId=7, UserId=7,TermId=7, TeacherId=7},
                new DistrictTermClass{ DistrictId = 8, DateStart = day.AddYears(-2), DateEnd = day.AddYears(-1),DistrictTermId=8, SchoolId=8, UserId=8,TermId=8, TeacherId=8},
                new DistrictTermClass{ DistrictId = 9, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=9, SchoolId=9, UserId=9,TermId=9, TeacherId=9},
                new DistrictTermClass{ DistrictId = 10, DateStart = day.AddYears(-2), DateEnd = day.AddYears(-1),DistrictTermId=10, SchoolId=10, UserId=10,TermId=10, TeacherId=10},
                new DistrictTermClass{ DistrictId = 11, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=11, SchoolId=11, UserId=11,TermId=11, TeacherId=11},
                new DistrictTermClass{ DistrictId = 12, DateStart = day.AddYears(-2), DateEnd = day.AddYears(-1),DistrictTermId=12, SchoolId=12, UserId=12,TermId=12, TeacherId=12},
                new DistrictTermClass{ DistrictId = 13, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=13, SchoolId=13, UserId=13,TermId=13, TeacherId=13},
                new DistrictTermClass{ DistrictId = 14, DateStart = day.AddYears(1), DateEnd = day.AddYears(2),DistrictTermId=14, SchoolId=14, UserId=14,TermId=14, TeacherId=14},
                new DistrictTermClass{ DistrictId = 15, DateStart = day.AddYears(-1), DateEnd = day.AddYears(1),DistrictTermId=15, SchoolId=15, UserId=15,TermId=15, TeacherId=15}
            };
        }

        public IQueryable<DistrictTermClass> Select()
        {
            return table.AsQueryable();
        }
    }
}