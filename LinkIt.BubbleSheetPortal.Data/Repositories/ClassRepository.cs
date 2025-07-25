using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using Envoc.Core.Shared.Extensions;
using AutoMapper;
using LinkIt.BubbleSheetPortal.Models.DTOs.Classes;
using LinkIt.BubbleSheetPortal.Models.DTOs.AggregateSubjectMapping;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class ClassRepository : IRepository<Class>, IClassRepository
    {
        private readonly Table<ClassEntity> table;
        private readonly Table<ClassMetaEntity> tableClassMeta;
        private readonly DbDataContext dbContext;

        public ClassRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<ClassEntity>();
            tableClassMeta = StudentDataContext.Get(connectionString).GetTable<ClassMetaEntity>();
            dbContext = DbDataContext.Get(connectionString);
            Mapper.CreateMap<Class, ClassEntity>();
        }
        public bool SaveClassMeta(List<CreateClassMetas> classMetaDatas)
        {
            foreach (var item in classMetaDatas)
            {
                var entities = tableClassMeta.Where(w => w.ClassID == item.ClassId).ToList();

                var entitiesToCreate = item.ClassMetas
                    .Where(x => !entities.Any(y => y.ClassMetaID == x.ClassMetaID))
                    .Select(x => new ClassMetaEntity
                    {
                        ClassID = item.ClassId,
                        Data = x.Data,
                        Name = "Subject",
                        CreatedBy = "Portal",
                        CreatedDate = DateTime.UtcNow
                    })
                    .ToList();

                var entitiesToUpdate = entities
                    .Where(x => item.ClassMetas.Any(y => y.ClassMetaID == x.ClassMetaID && x.Data != y.Data))
                    .Select(x =>
                    {
                        var classMeta = item.ClassMetas.FirstOrDefault(y => y.ClassMetaID == x.ClassMetaID && x.Data != y.Data);
                        if (classMeta != null)
                        {
                            x.Data = classMeta.Data;
                            x.CreatedBy = "Portal";
                            x.ModifiedDate = DateTime.UtcNow;
                        }
                        return x;
                    })
                    .ToList();

                var entitiesToDelete = entities
                    .Where(x => !item.ClassMetas.Any(y => y.ClassMetaID == x.ClassMetaID))
                    .ToList();

                if (entitiesToCreate.Any())
                {
                    tableClassMeta.InsertAllOnSubmit(entitiesToCreate);
                }

                if (entitiesToDelete.Any())
                {
                    tableClassMeta.DeleteAllOnSubmit(entitiesToDelete);
                }

                if (entitiesToCreate.Any() || entitiesToUpdate.Any() || entitiesToDelete.Any())
                {
                    tableClassMeta.Context.SubmitChanges();
                }
            }           

            return true;
        }

        public List<ClassMetaDto> GetMetaClassByClassId(List<int> classId)
        {
            return
                tableClassMeta.Where(o => classId.Contains(o.ClassID))
                    .Select(x => new ClassMetaDto
                    {
                        ClassMetaID = x.ClassMetaID,
                        ClassID = x.ClassID,
                        Name = x.Name,
                        Data = x.Data
                    }).ToList();
        }

        public Class GetClassByID(int classID)
        {
            var result = table.Where(o => o.ClassID == classID).Select(x => new Class
            {
                Id = x.ClassID,
                Name = x.Name,
                Course = x.Course,
                Section = x.Section,
                SchoolId = x.SchoolID,
                UserId = x.UserID,
                DistrictTermId = x.DistrictTermID,
                TermId = x.TermID,
                CourseNumber = x.CourseNumber,
                Locked = x.Locked,
                ClassType = x.ClassTypeID,
                TeacherId = x.TeacherID,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                ModifiedUser = x.ModifiedUser,
                ModifiedBy = x.ModifiedBy,
                DistrictId = x.DistrictID,
                SISID = x.SISID
            }).FirstOrDefault();

            return result;
        }

        public IQueryable<Class> Select()
        {
            var now = DateTime.UtcNow.Date;
            return
                table.Where(o => o.DistrictTermEntity.DateEnd == null || o.DistrictTermEntity.DateEnd > now)
                    .Select(x => new Class
                    {
                        Id = x.ClassID,
                        Name = x.Name,
                        Course = x.Course,
                        Section = x.Section,
                        SchoolId = x.SchoolID,
                        UserId = x.UserID,
                        DistrictTermId = x.DistrictTermID,
                        TermId = x.TermID,
                        CourseNumber = x.CourseNumber,
                        Locked = x.Locked,
                        ClassType = x.ClassTypeID,
                        TeacherId = x.TeacherID,
                        CreatedDate = x.CreatedDate,
                        ModifiedDate = x.ModifiedDate,
                        ModifiedUser = x.ModifiedUser,
                        ModifiedBy = x.ModifiedBy,
                        DistrictId = x.DistrictID,
                        SISID = x.SISID
                    });
        }

        public IQueryable<Class> SelectWithoutFilterByActiveTerm()
        {
            return table.Select(x => new Class
            {
                Id = x.ClassID,
                Name = x.Name,
                Course = x.Course,
                Section = x.Section,
                SchoolId = x.SchoolID,
                UserId = x.UserID,
                DistrictTermId = x.DistrictTermID,
                TermId = x.TermID,
                CourseNumber = x.CourseNumber,
                Locked = x.Locked,
                ClassType = x.ClassTypeID,
                TeacherId = x.TeacherID,
                CreatedDate = x.CreatedDate,
                ModifiedDate = x.ModifiedDate,
                ModifiedUser = x.ModifiedUser,
                ModifiedBy = x.ModifiedBy,
                DistrictId = x.DistrictID,
                SISID = x.SISID
            });
        }

        public void Save(Class item)
        {
            var entity = table.FirstOrDefault(x => x.ClassID.Equals(item.Id));

            if (entity.IsNull())
            {
                entity = new ClassEntity();
                table.InsertOnSubmit(entity);
            }

            Mapper.Map(item, entity);
            entity.ModifiedDate = DateTime.UtcNow;
            entity.ClassTypeID = item.ClassType;
            table.Context.SubmitChanges();
            item.Id = entity.ClassID;
        }

        public void Delete(Class item)
        {
            if (item.IsNotNull())
            {
                dbContext.DeleteClass(item.Id);
            }
        }

        public Class GetClassBySchoolTermAndUser(int districtId, int schoolId, int districtTermId, string strClassName)
        {
            var result = table.Where(o => o.DistrictID == districtId
                                          && o.SchoolID == schoolId
                                          && o.DistrictTermID == districtTermId
                                          && o.Name.Equals(strClassName))
                .Select(x => new Class
                {
                    Id = x.ClassID,
                    Name = x.Name,
                    Course = x.Course,
                    Section = x.Section,
                    SchoolId = x.SchoolID,
                    UserId = x.UserID,
                    DistrictTermId = x.DistrictTermID,
                    TermId = x.TermID,
                    CourseNumber = x.CourseNumber,
                    Locked = x.Locked,
                    ClassType = x.ClassTypeID,
                    TeacherId = x.TeacherID,
                    CreatedDate = x.CreatedDate,
                    ModifiedDate = x.ModifiedDate,
                    ModifiedUser = x.ModifiedUser,
                    ModifiedBy = x.ModifiedBy,
                    DistrictId = x.DistrictID,
                    SISID = x.SISID
                }).FirstOrDefault();

            return result;
        }

        public List<ListItem> GetClassBySchoolAndTerm(int districtId, int schoolId, int districtTermId, int userId, int roleId)
        {
            var result =
                dbContext.GetClassBySchoolAndDistrictTerm(districtId, schoolId, districtTermId, userId, roleId)
                .Select(x => new ListItem() { Id = x.ClassID, Name = x.ClassNameCustom }).ToList();
            return result;
        }

        public List<ListItem> GetClassBySchoolAndTermV2(int districtId, string schoolIds, int districtTermId, int userId, int roleId)
        {
            var result =
                dbContext.GetClassBySchoolAndDistrictTermV2(districtId, schoolIds, districtTermId, userId, roleId)
                .Select(x => new ListItem() { Id = x.ClassID, Name = x.ClassNameCustom }).ToList();
            return result;
        }

        public IEnumerable<ListItem> GetClassDistrictTermBySchool(int schoolId, int? userId, int? roleId)
        {
            var classes = dbContext.GetClassBySchool(schoolId, userId, roleId)
                .Select(o => new ListItem
                {
                    Id = o.ClassID.GetValueOrDefault(),
                    Name = o.ClassName
                });

            return classes;
        }
    }
}
