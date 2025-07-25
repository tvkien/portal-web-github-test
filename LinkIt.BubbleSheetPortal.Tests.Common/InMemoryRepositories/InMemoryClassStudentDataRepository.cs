using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryClassStudentDataRepository : IRepository<ClassStudentData>
    {
        private List<ClassStudentData> table;
        private static int nextUniqueID = 1;

        public InMemoryClassStudentDataRepository()
        {
            table = AddClassStudentData();
        }

        private List<ClassStudentData> AddClassStudentData()
        {
            return new List<ClassStudentData>
                       {
                           new ClassStudentData{ ClassID = 1, StudentID = 10, ClassStudentID = 5, DistrictID = 5, SISID = 152},
                           new ClassStudentData{ ClassID = 1, StudentID = 11, ClassStudentID = 6, DistrictID = 5, SISID = 153},
                           new ClassStudentData{ ClassID = 2, StudentID = 11, ClassStudentID = 7, DistrictID = 5, SISID = 154},
                           new ClassStudentData{ ClassID = 2, StudentID = 13, ClassStudentID = 8, DistrictID = 5, SISID = 155}
                           
                       };
        }

        public IQueryable<ClassStudentData> Select()
        {
            return table.AsQueryable();
        }

        public void Save(ClassStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.ClassStudentID.Equals(item.ClassStudentID));

            if (entity.IsNull())
            {
                item.ClassStudentID = nextUniqueID;
                nextUniqueID++;
                table.Add(item);
            }
            else
            {
                Mapper.CreateMap<ClassStudentData, ClassStudentData>();
                Mapper.Map(item, entity);
            }
        }

        public void Delete(ClassStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.ClassStudentID.Equals(item.ClassStudentID));
            if (entity.IsNotNull())
            {
                table.Remove(entity);
            }
        }
    }
}