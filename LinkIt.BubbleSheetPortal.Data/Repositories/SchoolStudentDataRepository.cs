using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using AutoMapper;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class SchoolStudentDataRepository : IRepository<SchoolStudentData>
    {
        private readonly Table<SchoolStudentDataEntity> table;

        public SchoolStudentDataRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<SchoolStudentDataEntity>();
            Mapper.CreateMap<SchoolStudentData, SchoolStudentDataEntity>();
        }

        public IQueryable<SchoolStudentData> Select()
        {
            return table.Select(x => new SchoolStudentData
                {
                    SchoolStudentID = x.SchoolStudentID,
                    SchoolID = x.StudentID,
                    StudentID = x.StudentID,
                    Active = x.Active.HasValue && x.Active.Value,
                    DateStart = x.DateStart.HasValue ? x.DateStart.Value : DateTime.MinValue ,
                    DateEnd = x.DateEnd.HasValue ? x.DateEnd.Value : DateTime.MinValue 
                });
        }

        public void Delete(SchoolStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolID.Equals(item.SchoolID) && x.StudentID.Equals(item.StudentID));
            if (entity.IsNotNull())
            {
                table.DeleteOnSubmit(entity);
                table.Context.SubmitChanges();
            }
        }

        public void Save(SchoolStudentData item)
        {
            var entity = table.FirstOrDefault(x => x.SchoolID.Equals(item.SchoolID) && x.StudentID.Equals(item.StudentID));

            if (entity.IsNull())
            {
                entity = new SchoolStudentDataEntity()
                {
                    SchoolID = item.SchoolID,
                    StudentID = item.StudentID,
                    Active = false
                };
                table.InsertOnSubmit(entity);
            }
            entity.Active = item.Active;
            //Mapper.Map(item, entity); //TODO: auto map assign primary key -> throw exception.
            table.Context.SubmitChanges();
            item.SchoolStudentID = entity.SchoolStudentID;
        }
    }
}