using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemorySchoolStudentDataRepository : IRepository<SchoolStudentData>
    {
        private List<SchoolStudentData> table;
        private static int nextUniqueID = 1;

        public InMemorySchoolStudentDataRepository()
        {
            table = AddSchoolStudentData();
        }

        private List<SchoolStudentData> AddSchoolStudentData()
        {
            return new List<SchoolStudentData>
                       {
                           new SchoolStudentData{ StudentID = 1, SchoolID = 1, SchoolStudentID = 1, DateEnd = DateTime.Now, DateStart = DateTime.Now.AddDays(-180), Active = true},
                           new SchoolStudentData{ StudentID = 2, SchoolID = 1, SchoolStudentID = 2, DateEnd = DateTime.Now, DateStart = DateTime.Now.AddDays(-180), Active = true},
                           new SchoolStudentData{ StudentID = 3, SchoolID = 2, SchoolStudentID = 3, DateEnd = DateTime.Now, DateStart = DateTime.Now.AddDays(-180), Active = true}
                       };
        }

        public IQueryable<SchoolStudentData> Select()
        {
            return table.AsQueryable();
        }

        public void Save(SchoolStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolStudentID.Equals(item.SchoolStudentID));

            if (entity.IsNull())
            {
                item.SchoolStudentID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<SchoolStudentData, SchoolStudentData>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(SchoolStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolStudentID.Equals(item.SchoolStudentID));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}